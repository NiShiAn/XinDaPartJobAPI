﻿/************************************************************************************
 *      Copyright (C) 2015 yuwei,All Rights Reserved
 *      File:
 *                GetEPContactsModel.cs
 *      Description:
 *            GetEPContactsModel
 *            企业数据库相关操作
 *      Author:
 *                yxw
 *                
 *                
 *      Finish DateTime:
 *                2018/3/20 08:24:36
 *      History:
 ***********************************************************************************/

using System.Collections.Generic;
using FrameWork.Common;
using FrameWork.Common.Models;
using FrameWork.Entity.Entity;
using FrameWork.Entity.Model.EP;
using FrameWork.Entity.ViewModel.EP;
using FrameWork.Entity.ViewModel.Job;

namespace FrameWork.Interface
{
    public interface IEPService : IBaseService<T_Enterprise>
    {
        /// <summary>
        /// 获取招聘联系人列表
        /// </summary>
        List<T_EPHiringManager> GetEpContacts(int epId);

        /// <summary>
        /// 删除机构下的招聘联系人
        /// </summary>
        /// <param name="epContactsId">招聘联系人id</param>
        void DelEPContacts(int epContactsId);

        /// <summary>
        /// 保存企业招聘联系人
        /// </summary>
        void SaveEPContacts(SaveEPContactsRequest request, int epId);

        /// <summary>
        /// 获取联系人详情
        /// </summary>
        T_EPHiringManager GetEPContactsDetails(int epContactsId);

        /// <summary>
        /// 获取子账号列表
        /// </summary>
        List<GetAccountListModel> GetAccountList(int epId, string cityId);

        /// <summary>
        /// 更新或新增该企业子账号，或者修改主账号手机号
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="epId">企业id</param>
        /// <param name="subAccoundId">账号id</param>
        int AddOrEditAccount(string phone, int epId,int subAccoundId);

        /// <summary>
        /// 删除子账号
        /// </summary>
        int DelAccount(int subAccoundId);

        /// <summary>
        /// 获取账号实体
        /// </summary>
        /// <param name="accountId">账号id</param>
        T_EPAccount GetAccount(int accountId);

        /// <summary>
        /// 获取所有的权限
        /// </summary>
        List<T_AccountPermission> GetAllPermissions();

        /// <summary>
        /// 更新账号的权限
        /// </summary>
        int UpdateAccountPer(int accountId, string pIds);

        /// <summary>
        /// 根据企业Id获取企业详情
        /// </summary>
        /// <param name="ePId">企业Id</param>
        GetEPDetailInfo GetEpDetailInfoById(int ePId);

        /// <summary>
        /// 根据企业Id获取企业发布的岗位列表
        /// </summary>
        /// <param name="ePId">企业Id</param>
        List<JobListItem> GetEpJobListById(int ePId);

        /// <summary>
        /// 根据企业Id获取企业的图片信息列表
        /// </summary>
        /// <param name="ePId">企业Id</param>
        List<CompanyImgListItem> GetEpImgListById(int ePId);

        /// <summary>
        /// 保存或认证企业信息
        /// </summary>
        int SaveEP(RedisModel redisModel, SaveEPRequest request, RegionModel regionModel);

        /// <summary>
        /// 获取企业的详情
        /// </summary>
        /// <param name="ePId">企业id</param>
        T_Enterprise GetEnterprise(int ePId);

        /// <summary>
        /// 获取企业的实景图片
        /// </summary>
        /// <param name="ePId">企业id</param>
        List<T_EPBgImg> GetBgImgs(int ePId);

        /// <summary>
        /// 获取企业中心信息
        /// </summary>
        GetEPCenterModel GetEPCenter(RedisModel redisModel);
    }
}
