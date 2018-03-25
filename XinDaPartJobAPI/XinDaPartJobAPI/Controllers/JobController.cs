﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using FrameWork.Common;
using FrameWork.Common.Const;
using FrameWork.Common.Enum;
using FrameWork.Common.Models;
using FrameWork.Entity.ViewModel;
using FrameWork.Entity.ViewModel.Job;
using FrameWork.Entity.ViewModel.SignIn;
using FrameWork.Web;

namespace XinDaPartJobAPI.Controllers
{
    public class JobController : AdminControllerBase
    {
        [HttpPost]
        [Route("api/Job/GetJobList")]
        public object GetJobList(GetJobListReq request)
        {
            var result = new BaseViewModel()
            {
                Info = CommonData.FailStr,
                Message = CommonData.FailStr,
                Msg = false,
                ResultCode = CommonData.FailCode
            };

            var userInfo = RedisInfoHelper.GetRedisModel(request.Token);

            var jobInfoList = JobService.GetJobList(request, userInfo.CityId);

            //todo:获取广告信息

            var getJobListRespInfoList = new GetJobListRespInfo();

            var firstOrDefualtJobInfo = jobInfoList.FirstOrDefault();
            if (firstOrDefualtJobInfo != null)
            {
                getJobListRespInfoList.IsEnd = !PageHelper.JudgeNextPage(firstOrDefualtJobInfo.TotalNum, request.Page, request.PageSize);
            }

            foreach (var jobInfo in jobInfoList)
            {
                var getJobListRespInfo = new JobInfoList
                {
                    JobId = jobInfo.JobId,
                    JobEmployerId = jobInfo.JobEmployerId,
                    JobEmployerName = EnumHelper.GetDescription(jobInfo.JobEmployerLevel),
                    JobName = jobInfo.JobName,
                    JobPay = $"{jobInfo.SalaryLower}-{jobInfo.SalaryUpper}",
                    JobPayUnit = jobInfo.Unit,
                    JobAddress = jobInfo.JobAddress,
                    JobTime = jobInfo.JobTime,
                    JobMember = jobInfo.JobMember,
                    IsSelf = jobInfo.IsSelf,
                    IsAdvert = false,
                    IsPractice = jobInfo.IsPractice,
                };
                getJobListRespInfoList.JobInfoList.Add(getJobListRespInfo);
            }

            result.Info = getJobListRespInfoList;
            result.Message = CommonData.SuccessStr;
            result.ResultCode = CommonData.SuccessCode;
            result.Msg = true;

            return result.ToJson();

        }

        /// <summary>
        /// 获取兼职岗位详情
        /// </summary>
        [HttpPost]
        [Route("api/Job/GetPartJob")]
        public object GetPartJob(GetPartJobRequest request)
        {
            var redisModel = RedisInfoHelper.GetRedisModel(request.Token);
            var userId = redisModel.UserId;
            if (redisModel.Mark == TokenMarkEnum.Enterprise)
                userId = 0;
            var model = JobService.GetPartJob(request.JobId, userId);
            var jobAddr = JobService.GetJobAdderssList(request.JobId);
            //TODO:广告列表没有返回
            var viewModel = new GetPartJobViewModel().GetViewModel(model, jobAddr, CacheContext.DicRegions, request);
            var result = new BaseViewModel
            {
                Info = viewModel,
                Message = CommonData.SuccessStr,
                Msg = true,
                ResultCode = CommonData.SuccessCode
            };
            return result;
        }

        /// <summary>
        /// 获取全职岗位详情
        /// </summary>
        [HttpPost]
        [Route("api/Job/GetFullJob")]
        public object GetFullJob(GetFullJobRequest request)
        {
            var redisModel = RedisInfoHelper.GetRedisModel(request.Token);
            var userId = redisModel.UserId;
            if (redisModel.Mark == TokenMarkEnum.Enterprise)
                userId = 0;
            var model = JobService.GetFullJob(request.JobId, userId);
            var jobAddr = JobService.GetJobAdderssList(request.JobId);
            var welfares = JobService.GetJobWelfareList(request.JobId);

            //TODO:广告列表没有返回
            var viewModel = new GetFullJobViewModel().GetViewModel(model, jobAddr, CacheContext.DicRegions, welfares, request);
            var result = new BaseViewModel
            {
                Info = viewModel,
                Message = CommonData.SuccessStr,
                Msg = true,
                ResultCode = CommonData.SuccessCode
            };
            return result;
        }

        /// <summary>
        /// 获取用户要投递的兼职简历列表
        /// </summary>
        [HttpPost]
        [Route("api/Job/GetUserPostPartCVList")]
        public object GetUserPostPartCVList(GetUserPostPartCVListRequest request)
        {
            var redisModel = RedisInfoHelper.GetRedisModel(request.Token);
            var userId = 0;
            if (redisModel.Mark == TokenMarkEnum.Enterprise)
                userId = 0;
            var models = JobService.GetUserPostPartCVList(userId);
            var viewModels = new GetUserPostPartCVListViewModel().GetViewModel(models,request.JobCategoryName);
            var result = new BaseViewModel
            {
                Info = viewModels,
                Message = CommonData.SuccessStr,
                Msg = true,
                ResultCode = CommonData.SuccessCode
            };
            return result;
        }

        /// <summary>
        /// 用户投递简历到某个岗位
        /// </summary>
        [HttpPost]
        [Route("api/Job/UserPostCV")]
        public object UserPostCV(UserPostCVRequest request)
        {
            var result = new BaseViewModel
            {
                Info = CommonData.SuccessStr,
                Message = CommonData.SuccessStr,
                Msg = true,
                ResultCode = CommonData.SuccessCode
            };
            var redisModel = RedisInfoHelper.GetRedisModel(request.Token);
            var userId = redisModel.UserId;
            if (redisModel.Mark == TokenMarkEnum.Enterprise)
            {
                result = new BaseViewModel
                {
                    Info = CommonData.EPNotPostCV,
                    Message = CommonData.EPNotPostCV,
                    Msg = false,
                    ResultCode = CommonData.FailCode
                };
            }
            JobService.UserPostCV(userId, request.CVId, request.JobId);
            return result;
        }

        /// <summary>
        /// 获取结算方式接口
        /// </summary>
        [HttpPost]
        [Route("api/Job/GetPayWay")]
        public object GetPayWay(GetPayWayRequest request)
        {
            var redisModel = RedisInfoHelper.GetRedisModel(request.Token);
            var models = CacheContext.PayWays;
            var viewModels = new GetPayWayViewModel().GetViewModels(models);
            var result = new BaseViewModel
            {
                Info = viewModels,
                Message = CommonData.SuccessStr,
                Msg = true,
                ResultCode = CommonData.SuccessCode
            };
            return result;
        }

        /// <summary>
        /// 获取首页基础数据
        /// </summary>
        [HttpPost]
        [Route("api/Job/GetIndexBaseData")]
        public object GetIndexBaseData(GetIndexBaseDataReq request)
        {
            var result = new BaseViewModel
            {
                Info = CommonData.FailStr,
                Message = CommonData.FailStr,
                Msg = false,
                ResultCode = CommonData.FailCode
            };
            var redisModel = RedisInfoHelper.GetRedisModel(request.Token);
            var userId = redisModel.UserId;
            
            var resultInfo=new GetIndexBaseDataRespInfo();

            //地区数据
            resultInfo.Region = CacheContext.DicRegions.Where(r => !r.IsDel&&r.ParentId==redisModel.CityId).Select(r=>new RegionListItem{RegionId = r.Id,Name = r.Description}).ToList();
            //foreach (var jb in JobEmployerLevelEnum)
            //{
            //    resultInfo.JobType = CacheContext.DicRegions.Where(r => !r.IsDel&&r.ParentId== redisModel.CityId).Select(r=>new RegionListItem{RegionId = r.Id,Name = r.Description}).ToList();
            //}

            //雇主级别数据
            
            foreach (JobEmployerLevelEnum jobEmployerLevel in Enum.GetValues(typeof(JobEmployerLevelEnum)))
            {
                var currentEmployer = new EmployerListItem
                {
                    EmployerId = ((int)jobEmployerLevel).ToString(),
                    Name = EnumHelper.GetDescription(jobEmployerLevel)
                };
                resultInfo.Employer.Add(currentEmployer);
            }

            return result;
        }
    }
}
