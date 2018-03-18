﻿using System;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FrameWork.Common;
using FrameWork.Common.Const;
using FrameWork.Common.Enum;
using FrameWork.Entity.ViewModel;
using Newtonsoft.Json;

namespace XinDaPartJobAPI.Controllers
{
    /// <summary>
    /// 验证签名，token，参数是否为空    ----时间戳名称：TimeStamp  签名名称：Sign 固定字符串暂定为：（加密固定字符串 经md5加密）abfdb3f36565ecb7d944303845392592
    /// </summary>
    public class RequestHandler : DelegatingHandler
    {
        /// <summary>  
        /// 拦截请求  
        /// </summary>  
        /// <param name="request">请求</param>  
        /// <param name="cancellationToken">用于发送取消操作信号</param>  
        /// <returns></returns>  
        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
#if DEBUG
            return await base.SendAsync(request, cancellationToken);//调试时打开注释
#endif
            //做一些其他安全验证工作，比如Token验证，签名验证  在此共分为3步。
            var methodType = request.Method;

            #region 验证token

            if (methodType.Method == "POST")
            {
                var tokenModel = JsonConvert.DeserializeObject<TokenModel>(request.Content.ReadAsStringAsync().Result);
                var token = tokenModel.Token;
                if (string.IsNullOrEmpty(token))
                {
                    return ReturnHelper(CommonEnum.TokenError, CourseConst.TokenError);
                }
                if (!string.IsNullOrEmpty(token) && !TokenIseffective(tokenModel.Token))
                {
                    return ReturnHelper(CommonEnum.TokenError, CourseConst.TokenError);
                }
            }
            else
            {
                var paramGet = request.RequestUri;    //Get请求
                if (!string.IsNullOrEmpty(paramGet.Query) && paramGet.Query.ToLower().Contains("token"))
                {
                    string[] getTokenStrings = paramGet.Query.Split('&');
                    if (!CheckToken(getTokenStrings))
                    {
                        return ReturnHelper(CommonEnum.TokenError, CourseConst.TokenError);
                    }
                }
            }

            #endregion
            return await base.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// 判断所调用的方法是否需要参数
        /// </summary>
        /// <param name="controllerType">controller类型</param>
        /// <param name="methodName">方法名称</param>
        /// <returns></returns>
        private bool IsHaveParam(Type controllerType, string methodName)
        {
            MethodInfo method = controllerType.GetMethod(methodName);
            if (method == null || method.GetParameters().Length <= 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 验证Token是否有效及其正确
        /// </summary>
        private bool CheckToken(string[] getTokenStrings)
        {
            var token = "";
            var mark = false;
            //获取UserId
            for (var i = 0; i < getTokenStrings.Length; i++)
            {
                if (getTokenStrings[i].ToLower().Contains("token"))
                {
                    token = getTokenStrings[i].Split('=')[1];
                    break;
                }
            }
            if (!string.IsNullOrEmpty(token))
            {
                if (TokenIseffective(token))
                    mark= true;
            }
            //参数中没有UserId,则不需要验证
            return mark;
        }

        /// <summary>
        /// 判断token是否有效
        /// </summary>
        /// <param name="token">登录令牌</param>
        private bool TokenIseffective(string token)
        {
            var mark = false;
            if (!string.IsNullOrEmpty(token))
            {
                var model = RedisInfoHelper.GetRedisModel(token);
                mark = model.Mark > 0;
            }
            return mark;
        }

        /// <summary>
        /// 返回错误消息帮助方法
        /// </summary>
        private HttpResponseMessage ReturnHelper(Enum errorEnum, string message)
        {
            return new
            {
                Msg = false,
                Info = Convert.ToInt32(errorEnum),
                Message = message
            }.ToJson();
        }
    }
}
