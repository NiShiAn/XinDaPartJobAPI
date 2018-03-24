﻿/************************************************************************************
 *      Copyright (C) 2015 yuwei,All Rights Reserved
 *      File:
 *                ReportController.cs
 *      Description:
 *            举报相关
 *      Author:
 *                yxw
 *                
 *                
 *      Finish DateTime:
 *                2018/3/24 21:23:40
 *      History:
 ***********************************************************************************/


using System.Web.Http;
using FrameWork.Common.Const;
using FrameWork.Entity.ViewModel;
using FrameWork.Entity.ViewModel.Report;
using FrameWork.Web;

namespace XinDaPartJobAPI.Controllers
{
    /// <summary>
    /// ReportController
    /// </summary>
    public class ReportController:AdminControllerBase
    {
        /// <summary>
        /// 举报简历
        /// </summary>
        [HttpPost]
        [Route("api/Report/ReportCV")]
        public object ReportCV(ReportCVRequest request)
        {
            
            var result = new BaseViewModel
            {
                Info = "",
                Message = CommonData.SuccessStr,
                Msg = true,
                ResultCode = CommonData.SuccessCode
            };
            return result;
        }

    }
}