﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using FrameWork.Common;
using FrameWork.Common.Const;
using FrameWork.Common.Enum;
using FrameWork.Common.Models;
using FrameWork.Common.SmsHelper.Sms;
using FrameWork.Entity.Entity;
using FrameWork.Entity.Model.Account;
using FrameWork.Entity.ViewModel;
using FrameWork.Entity.ViewModel.Account;
using FrameWork.Web;
using Newtonsoft.Json.Linq;

namespace XinDaPartJobAPI.Controllers
{
    public class AccountController : AdminControllerBase
    {
        /// <summary>
        /// 腾讯云发送短信帮助类
        /// </summary>
        private static readonly SmsSingleSender _smsSingleSender = new SmsSingleSender();

        /// <summary>
        /// 普通用户授权接口
        /// </summary>
        [HttpPost]
        [Route("api/Account/GetUserInfo")]
        public object GetUserInfo(GetUserInfoRequest request)
        {
            var weChatAppId = ConfigurationManager.AppSettings["WeChatAppId"];
            var weChatSecret = ConfigurationManager.AppSettings["WeChatSecret"];
            var url = $@"https://api.weixin.qq.com/sns/jscode2session?appid={weChatAppId}&secret={weChatSecret}&js_code={request.Code}&grant_type=authorization_code";
            var rs = HttpClientHelper.SendMessage(url);
            var openidModel = JObject.Parse(rs).GetValue("openid");
            //var openidModel = "wx123456789";
            var result = new BaseViewModel
            {
                Info = CommonData.FailStr,
                Message = CommonData.FailStr,
                Msg = false,
                ResultCode = CommonData.FailCode
            };

            if (openidModel != null)
            {
                request.OpenId = openidModel.ToString();
                var model = AccountService.GetUserInfo(request);

                var viewModel = new GetUserInfoViewModel().GetViewModel(model);
                viewModel.Token = GetToken(model, request);
                result = new BaseViewModel
                {
                    Info = viewModel,
                    Message = CommonData.SuccessStr,
                    Msg = true,
                    ResultCode = CommonData.SuccessCode
                };
            }
            return result.ToJson();
        }

        /// <summary>
        /// 获取这个用户的token值，并把这个用户相关的信息存到缓存中
        /// </summary>
        /// <param name="model">用户信息实体</param>
        /// <param name="request">接口参数</param>
        private string GetToken(T_User model, GetUserInfoRequest request)
        {
            var token = GuidHelper.GetPrimarykey();
            var oldToken = RedisInfoHelper.RedisManager.Getstring("uid" + model.Id);
            if (!string.IsNullOrEmpty(oldToken))//如果是重新登录，移除原来的token对应的值
            {
                oldToken = oldToken.Replace("\"", "");
                RedisInfoHelper.RedisManager.Remove(oldToken);
            }
            var rdModel = new RedisModel
            {
                DicRegionId = request.City,
                EPId = 0,
                Mark = TokenMarkEnum.User,
                OpenId = model.WxAccount,
                Token = token,
                UserId = model.Id,
                WxName = model.WxName
            };
            RedisInfoHelper.RedisManager.Set("uid" + model.Id, token, DateTime.Now.AddDays(1));
            RedisInfoHelper.RedisManager.Set(token, rdModel.ToJsonStr(), DateTime.Now.AddDays(1));

            return token;
        }

        /// <summary>
        /// 获取手机号验证码
        /// </summary>
        [HttpPost]
        [Route("api/Account/GetPhoneCode")]
        public object GetPhoneCode(GetPhoneCodeRequest request)
        {
            var random = new Random();
            var code = random.Next(1000, 9999).ToString();
            RedisInfoHelper.RedisManager.Set(request.Phone, code, DateTime.Now.AddMinutes(3));//再次存储验证码到缓存中，防止上次验证码过期，过期时间默认3分钟
            var r = _smsSingleSender.SendWithParam("86", request.Phone, CommonData.TemplateId, new List<string> { code }, "", "", "");
            var result = new BaseViewModel
            {
                Info = r.errmsg,
                Message = CommonData.SuccessStr,
                Msg = true,
                ResultCode = CommonData.SuccessCode
            };

            if (r.result != 0)
            {
                result = new BaseViewModel
                {
                    Info = CommonData.SendFailStr,
                    Message = CommonData.SendFailStr,
                    Msg = false,
                    ResultCode = CommonData.FailCode
                };
            }

            return result.ToJson();
        }

        /// <summary>
        /// 企业登录
        /// </summary>
        [HttpPost]
        [Route("api/Account/EPLogin")]
        public object EPLogin(EPLoginRequest request)
        {
            var result = new BaseViewModel
            {
                Info = CommonData.FailStr,
                Message = CommonData.FailStr,
                Msg = false,
                ResultCode = CommonData.FailCode
            };
            /**
             1.判断手机号是否已经绑定到某个企业。
             2.如果未绑定某个企业，则创建一个企业，并把该手机号绑定未该企业的主账号。
             3.如果绑定了某个企业，获取该账号的类型，是否为主账号。                
             */

            if (string.IsNullOrEmpty(request.VerifyCode))//验证码不能为空
            {
                result.Info = CommonData.CodeNotNULL;
                result.Message = CommonData.CodeNotNULL;
                return result;
            }
            var viewModel = new EPLoginViewModel();
            var oldCode = RedisInfoHelper.RedisManager.Getstring(request.Phone);
            if (!string.IsNullOrEmpty(oldCode))//缓存未过期
            {
                oldCode = oldCode.Replace("\"", "");
                if (!oldCode.Equals(request.VerifyCode))//验证码不正确
                {
                    result.Info = CommonData.CodeNotCorrect;
                    result.Message = CommonData.CodeNotCorrect;
                    return result;
                }
            }
            else
            {
                result.Info = CommonData.CodePassdate;
                result.Message = CommonData.CodePassdate;
                return result;
            }
            var userModel = AccountService.EpLogin(request);
            if (userModel == null) //没有与任何机构绑定
            {
                AccountService.EPLoginForInsert(request);
                userModel = AccountService.EpLogin(request);
                viewModel.IsMainAccount = true;
                viewModel.Token = GetEPToken(userModel, request);
                result = new BaseViewModel
                {
                    Info = viewModel,
                    Message = CommonData.SuccessStr,
                    Msg = false,
                    ResultCode = CommonData.SuccessCode
                };
            }
            else  //已经与机构绑定
            {
                if (userModel.EPStatus == (byte)AccountStatus.IllegalNotUsed)
                {
                    result.Info = CommonData.AccountException;
                    result.Message = CommonData.AccountException;
                }
                if (userModel.EPAStatus == (byte)AccountStatus.IllegalNotUsed)
                {
                    result.Info = CommonData.AccountException;
                    result.Message = CommonData.AccountException;
                }
                if (userModel.EPStatus == (byte)AccountStatus.NotUsed)
                {
                    result.Info = CommonData.AccountPassdate;
                    result.Message = CommonData.AccountPassdate;
                }
                if (userModel.EPAStatus == (byte)AccountStatus.NotUsed)
                {
                    result.Info = CommonData.AccountPassdate;
                    result.Message = CommonData.AccountPassdate;
                }
                if (userModel.EPAStatus == (byte)AccountStatus.Using && userModel.EPStatus == (byte)AccountStatus.Using)
                {
                    viewModel.IsMainAccount = userModel.Type == (byte)AccountType.Main;
                    viewModel.Token = GetEPToken(userModel, request);
                    result = new BaseViewModel
                    {
                        Info = viewModel,
                        Message = CommonData.SuccessStr,
                        Msg = true,
                        ResultCode = CommonData.SuccessCode
                    };
                }
            }

            return result.ToJson();
        }

        /// <summary>
        /// 获取这个用户的token值，并把这个用户相关的信息存到缓存中
        /// </summary>
        /// <param name="model">企业信息实体</param>
        /// <param name="request">接口参数</param>
        private string GetEPToken(EPLoginModel model, EPLoginRequest request)
        {
            var token = GuidHelper.GetPrimarykey();
            var oldToken = RedisInfoHelper.RedisManager.Getstring("epid" + model.EPId);
            if (!string.IsNullOrEmpty(oldToken))//如果是重新登录，移除原来的token对应的值
            {
                oldToken = oldToken.Replace("\"", "");
                RedisInfoHelper.RedisManager.Remove(oldToken);
            }
            var rdModel = new RedisModel
            {
                DicRegionId = request.City,
                EPId = model.EPId,
                Mark = TokenMarkEnum.Enterprise,
                OpenId = request.OpenId,
                Token = token,
                UserId = model.EPAId,
                WxName = string.Empty
            };

            RedisInfoHelper.RedisManager.Set("epid" + model.EPId, token, DateTime.Now.AddDays(1));
            RedisInfoHelper.RedisManager.Set(token, rdModel.ToJsonStr(), DateTime.Now.AddDays(1));

            return token;
        }

    }
}
