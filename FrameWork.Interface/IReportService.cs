﻿/************************************************************************************
 *      Copyright (C) 2015 yuwei,All Rights Reserved
 *      File:
 *                IReportService.cs
 *      Description:
 *            IReportService
 *      Author:
 *                yxw
 *                
 *                
 *      Finish DateTime:
 *                2018/3/24 21:46:38
 *      History:
 ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrameWork.Common.Models;
using FrameWork.Entity.Entity;
using FrameWork.Entity.ViewModel.Report;

namespace FrameWork.Interface
{
    /// <summary>
    /// IReportService
    /// </summary>
    public interface IReportService:IBaseService<T_ReportCV>
    {
        /// <summary>
        /// 举报简历接口
        /// </summary>
        int ReportCV(ReportCVRequest request, RedisModel redisModel);

        /// <summary>
        /// 举报岗位接口
        /// </summary>
        int ReportJob(ReportJobRequest request, RedisModel redisModel);

        /// <summary>
        /// 获取所有的举报原因列表
        /// </summary>
        List<T_ReportReason> GetReasonList();
    }
}
