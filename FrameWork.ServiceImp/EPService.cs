﻿/************************************************************************************
 *      Copyright (C) 2015 yuwei,All Rights Reserved
 *      File:
 *                EPService.cs
 *      Description:
 *            EPService
 *      Author:
 *                yxw
 *                
 *                
 *      Finish DateTime:
 *                2018/3/20 08:27:01
 *      History:
 ***********************************************************************************/


using System.Collections.Generic;
using FrameWork.Common;
using FrameWork.Common.Models;
using FrameWork.Entity.Entity;
using FrameWork.Entity.Model.EP;
using FrameWork.Entity.ViewModel.EP;
using FrameWork.Interface;

namespace FrameWork.ServiceImp
{
    /// <summary>
    /// EPService
    /// </summary>
    public class EPService : BaseService<T_Enterprise>, IEPService
    {
        /// <summary>
        /// 获取招聘联系人列表
        /// </summary>
        public List<T_EPHiringManager> GetEpContacts(int epId)
        {
            var sql = @";SELECT * FROM dbo.T_EPHiringManager WHERE EnterpriseId = @epId AND IsDel = 0";
            return DbPartJob.Fetch<T_EPHiringManager>(sql, new { epId });
        }

        /// <summary>
        /// 删除机构下的招聘联系人
        /// </summary>
        /// <param name="epContactsId">招聘联系人id</param>
        public void DelEPContacts(int epContactsId)
        {
            var sql = @";UPDATE dbo.T_EPHiringManager SET IsDel = 1 WHERE Id = @epContactsId";

            DbPartJob.Execute(sql, new { epContactsId });
        }

        /// <summary>
        /// 保存企业招聘联系人
        /// </summary>
        public void SaveEPContacts(SaveEPContactsRequest request, int epId)
        {
            var sql = @";
IF EXISTS (SELECT * FROM dbo.T_EPHiringManager WHERE Phone = @Phone AND IsDel = 0)
	BEGIN
	    UPDATE
		dbo.T_EPHiringManager
		SET
			HeadPicUrl = @HeadImg,
			Name = @ContactsName
		WHERE
			Phone = @Phone AND IsDel = 0
	END
ELSE
	BEGIN
	    INSERT
		INTO
        dbo.T_EPHiringManager
                ( EnterpriseId ,
                  Name ,
                  Phone ,
                  HeadPicUrl ,
                  AuthStatus ,
                  IsDel ,
                  ModifyUserId ,
                  ModifyTime ,
                  CreateUserId ,
                  CreateTime
                )
        VALUES  ( @epId , -- EnterpriseId - int
                  @ContactsName , -- Name - nvarchar(50)
                  @Phone , -- Phone - nvarchar(15)
                  @HeadImg , -- HeadPicUrl - nvarchar(255)
                  0 , -- AuthStatus - tinyint
                  0 , -- IsDel - bit
                  0 , -- ModifyUserId - int
                  GETDATE() , -- ModifyTime - datetime
                  0 , -- CreateUserId - int
                  GETDATE()  -- CreateTime - datetime
                )
	END";

            DbPartJob.Execute(sql, new { request.Phone, request.ContactsName, request.HeadImg, epId });
        }

        /// <summary>
        /// 获取联系人详情
        /// </summary>
        public T_EPHiringManager GetEPContactsDetails(int epContactsId)
        {
            var sql = @";SELECT * FROM dbo.T_EPHiringManager WHERE Id = @epContactsId AND IsDel = 0";
            return DbPartJob.FirstOrDefault<T_EPHiringManager>(sql, new { epContactsId });
        }

        /// <summary>
        /// 获取子账号列表
        /// </summary>
        public List<GetAccountListModel> GetAccountList(int epId, string cityId)
        {
            var sql = @";
SELECT
	ep.Logo,
	epa.Id AS AccountId,
	epa.Phone,
	epa.Type AS AccountType,
	(SELECT TOP 1 v.Name FROM dbo.T_EPVIP ev LEFT JOIN dbo.T_VIPInfo v ON ev.VIPInfoId = v.Id WHERE ev.IsDel = 0 AND v.IsDel = 0 AND ev.EnterpriseId = ep.Id AND ev.CityId = @cityId)VipType,
	(SELECT COUNT(1) FROM dbo.T_EPVIP ev LEFT JOIN dbo.T_VIPInfo v ON ev.VIPInfoId = v.Id WHERE ev.IsDel = 0 AND v.IsDel = 0 AND ev.EnterpriseId = @epId )VipCount
FROM
	dbo.T_Enterprise ep 
	LEFT JOIN dbo.T_EPAccount epa ON ep.Id = epa.EnterpriseId
WHERE
	ep.IsDel = 0 AND epa.IsDel = 0
	AND ep.Id = @epId
";
            return DbPartJob.Fetch<GetAccountListModel>(sql, new { epId, cityId });
        }

        /// <summary>
        /// 更新或新增该企业子账号，或者修改主账号手机号
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="epId">企业id</param>
        /// <param name="subAccoundId">账号id</param>
        public int AddOrEditAccount(string phone, int epId, int subAccoundId)
        {
            var checkSql = @";SELECT count(1) FROM dbo.T_EPAccount WHERE Phone = @phone AND IsDel = 0 AND id != @subAccoundId";
            var count = DbPartJob.ExecuteScalar<int>(checkSql, new { phone, subAccoundId });
            if (count > 0)
                return -1;
            var sql = @";
IF EXISTS (SELECT 1 FROM dbo.T_EPAccount WHERE id = @subAccoundId AND IsDel = 0)
	BEGIN
	    UPDATE dbo.T_EPAccount SET Phone = @phone ,ModifyTime = GETDATE() WHERE id = @subAccoundId AND IsDel = 0
	END
ELSE
	BEGIN
	    INSERT
        INTO
        dbo.T_EPAccount
                ( EnterpriseId ,
                  Phone ,
                  PermissionIds ,
                  Type ,
                  Status ,
                  Note ,
                  IsDel ,
                  ModifyUserId ,
                  ModifyTime ,
                  CreateUserId ,
                  CreateTime
                )
        VALUES  ( @epId , -- EnterpriseId - int
                  @phone , -- Phone - nvarchar(15)
                  N'/1/2/3/4/5/6/7/8/9/' , -- PermissionIds - nvarchar(200)
                  2 , -- Type - tinyint
                  1 , -- Status - tinyint
                  N'' , -- Note - nvarchar(500)
                  0 , -- IsDel - bit
                  0 , -- ModifyUserId - int
                  GETDATE() , -- ModifyTime - datetime
                  0 , -- CreateUserId - int
                  GETDATE()  -- CreateTime - datetime
                )
	END";
            return DbPartJob.Execute(sql, new { epId, phone, subAccoundId });
        }

        /// <summary>
        /// 删除子账号
        /// </summary>
        public int DelAccount(int subAccoundId)
        {
            var sql = @";UPDATE dbo.T_EPAccount SET IsDel = 1 WHERE Id = @subAccoundId AND IsDel = 0 AND Type = 2";

            return DbPartJob.Execute(sql, new { subAccoundId });
        }

        /// <summary>
        /// 获取账号实体
        /// </summary>
        /// <param name="accountId">账号id</param>
        public T_EPAccount GetAccount(int accountId)
        {
            var sql = @"SELECT * FROM dbo.T_EPAccount WHERE Id = @accountId";
            return DbPartJob.FirstOrDefault<T_EPAccount>(sql, new { accountId });
        }

        /// <summary>
        /// 获取所有的权限
        /// </summary>
        public List<T_AccountPermission> GetAllPermissions()
        {
            var sql = @"where isdel = 0  AND IsUsed = 1";
            return DbPartJob.Fetch<T_AccountPermission>(sql);
        }

        /// <summary>
        /// 更新账号的权限
        /// </summary>
        public int UpdateAccountPer(int accountId, string pIds)
        {
            var sql = @";UPDATE dbo.T_EPAccount SET PermissionIds = @pIds WHERE Id = @accountId";
            return DbPartJob.Execute(sql, new { accountId, pIds });
        }

        public GetEPDetailInfo GetEpDetailInfoById(int ePId)
        {
            var sql = @"SELECT  Id CompanyId,
		                        Level CompanyEmployerId,
                                ShortName CompanyName,
                                [Address] CompanyAddress,
		                        Name CompanyFullName,
		                        Brief CompanyDesc
                        FROM    dbo.T_Enterprise
                        WHERE 1=1
                        AND Id=@ePId
                        AND IsDel=0";
            return DbPartJob.FirstOrDefault<GetEPDetailInfo>(sql, new { ePId });
        }

        public List<JobListItem> GetEpJobListById(int ePId)
        {
            var sql = @"SELECT  job.Id JobId ,
                                job.Type JobType ,
                                job.Name JobName ,
                                job.SalaryLower ,
                                job.SalaryUpper ,
                                payway.Unit JobPayUnit,
                                ( CASE WHEN ( SELECT TOP 1
                                                        epaddress.AreaId
                                                FROM      dbo.T_JobAddress jobaddress
                                                        LEFT JOIN dbo.T_EPAddress epaddress ON epaddress.Id = jobaddress.EPAddressId
                                                WHERE     1 = 1
                                                        AND jobaddress.IsDel = 0
                                                        AND epaddress.IsDel = 0
                                                        AND job.Id = jobaddress.JobId
                                            ) = '-1' THEN '不限地点'
                                        ELSE ( SELECT TOP 1
                                                        dicregion.Description
                                                FROM      dbo.T_JobAddress jobaddress
                                                        LEFT JOIN dbo.T_EPAddress epaddress ON epaddress.Id = jobaddress.EPAddressId
                                                        LEFT JOIN dbo.DicRegion dicregion ON epaddress.AreaId = dicregion.Id
                                                WHERE     1 = 1
                                                        AND jobaddress.IsDel = 0
                                                        AND epaddress.IsDel = 0
                                                        AND dicregion.IsDel = 0
                                                        AND job.Id = jobaddress.JobId
                                            )
                                    END ) JobAddress ,
                                job.WorkTime JobTime ,
                                ( SELECT TOP 1
                                            vipinfo.Name
                                    FROM      dbo.T_VIPInfo vipinfo
                                            LEFT JOIN dbo.T_EPVIP epvip ON epvip.VIPInfoId = vipinfo.Id
                                    WHERE     epvip.EnterpriseId = 1
                                            AND vipinfo.IsDel = 0
                                            AND epvip.IsDel = 0
                                    ORDER BY  vipinfo.OldPrice DESC
                                ) JobMember ,
                                job.IsPractice IsPractice
                        FROM    dbo.T_Job job
                                LEFT JOIN dbo.T_PayWay payway ON payway.Id = job.PayWayId
                        WHERE   1 = 1
                                AND job.EnterpriseId = @ePId;";
            return DbPartJob.Fetch<JobListItem>(sql, new { ePId });
        }

        public List<CompanyImgListItem> GetEpImgListById(int ePId)
        {
            var sql = @"SELECT  Id ,
                                PicUrl Url
                        FROM    dbo.T_EPBgImg
                        WHERE 1=1
                        AND IsDel=0
                        AND EnterpriseId=@ePId;";
            return DbPartJob.Fetch<CompanyImgListItem>(sql, new { ePId });
        }

        /// <summary>
        /// 保存或认证企业信息
        /// </summary>
        public int SaveEP(RedisModel redisModel, SaveEPRequest request, RegionModel regionModel)
        {
            var where = string.Empty;
            if (request.IsAuth)
                where = @",CheckStatus = 1";
            var sql = $@";
                UPDATE 
	                dbo.T_Enterprise
                SET
	                Name = @CompanyName ,
	                ShortName = @CompanyShort,
	                ProvinceId = @ProvinceId,
	                CityId = @CityId,
                    AreaId= @AreaId,
	                Address = @Address,
	                Lng = @Lng,
	                Lat = @Lat,
	                Brief = @CompanyDesc,
	                Logo = @CompanyLogo,
	                AuthPicUrl = @AuthPicUrl,
	                ModifyTime = GETDATE(),
	                ModifyUserId = @UserId
                    {where}
                WHERE
	                Id = @EPId
                ";
            DbPartJob.Execute(sql, new
            {
                redisModel.EPId,
                redisModel.UserId,
                request.CompanyName,
                request.CompanyShort,
                regionModel.ProvinceId,
                regionModel.CityId,
                regionModel.AreaId,
                regionModel.Address,
                request.Lng,
                request.Lat,
                request.CompanyDesc,
                request.CompanyLogo,
                request.AuthPicUrl
            });
            //删除原来的数据
            var delSql = @";UPDATE dbo.T_EPBgImg SET IsDel = 1 WHERE EnterpriseId = @EPId";
            DbPartJob.Execute(delSql, new { redisModel.EPId });
            //插入新的数据
            foreach (var photo in request.CompanyPhotos)
            {
                var insertSql = @";
                    INSERT
                    INTO
                    dbo.T_EPBgImg
                            ( EnterpriseId ,
                              PicUrl ,
                              IsDel ,
                              CreateUserId ,
                              CreateTime
                            )
                    VALUES  ( @EPId , -- Id - int
                              @photo , -- PicUrl - nvarchar(255)
                              0 , -- IsDel - bit
                              @UserId , -- CreateUserId - int
                              GETDATE()  -- CreateTime - datetime
                            )";
                DbPartJob.Execute(insertSql, new { redisModel.UserId, redisModel.EPId, photo });
            }

            return 1;
        }

        /// <summary>
        /// 获取企业的详情
        /// </summary>
        /// <param name="ePId">企业id</param>
        public T_Enterprise GetEnterprise(int ePId)
        {
            var sql = @";SELECT * FROM dbo.T_Enterprise WHERE Id = @ePId";
            return DbPartJob.FirstOrDefault<T_Enterprise>(sql, new { ePId });
        }

        /// <summary>
        /// 获取企业的实景图片
        /// </summary>
        /// <param name="ePId">企业id</param>
        public List<T_EPBgImg> GetBgImgs(int ePId)
        {
            var sql = @";
                    SELECT
	                    *
                    FROM
	                    dbo.T_EPBgImg
                    WHERE
	                    EnterpriseId = @ePId AND IsDel = 0";

            return DbPartJob.Fetch<T_EPBgImg>(sql, new { ePId });
        }

        /// <summary>
        /// 获取企业中心信息
        /// </summary>
        public GetEPCenterModel GetEPCenter(RedisModel redisModel)
        {
            var sql = @";
SELECT
	ep.Name,
	ep.Level,
	ep.Logo,
	ep.TotalIntegral,
	(SELECT COUNT(1) FROM dbo.T_Job j WHERE j.IsDel = 0 AND j.EnterpriseId = ep.Id)JobCount,
	(SELECT COUNT(1) FROM dbo.T_Job j LEFT JOIN dbo.T_CVAutoMatchJob cmj ON j.Id = cmj.JobId LEFT JOIN dbo.T_CV cv ON cv.Id = cmj.CvId WHERE j.IsDel = 0 AND cmj.EPIsDel = 0 AND cv.IsDel = 0 AND j.EnterpriseId = ep.Id)AutoJobCount,
	(SELECT COUNT(1) FROM dbo.T_Job j LEFT JOIN dbo.T_CVDelivery cd ON cd.JobId = j.Id LEFT JOIN dbo.T_CV cv ON cv.Id = cd.CvId WHERE j.IsDel = 0 AND cd.EPIsDel= 0 AND cv.IsDel = 0 AND j.EnterpriseId = ep.Id)CVCount,
	(SELECT COUNT(1) FROM dbo.T_EPCV ecv LEFT JOIN dbo.T_CV cv ON ecv.CvId = cv.Id WHERE cv.IsDel = 0 AND ecv.IsDel = 0 AND ecv.EnterpriseId = ep.Id)BuyCVCount,
	(SELECT COUNT(1) FROM dbo.T_EPHiringManager h WHERE h.EnterpriseId = ep.Id AND ep.IsDel = 0 AND h.AuthStatus = 1)AuthCount, --认证的数量
	(SELECT COUNT(1) FROM dbo.T_EPHiringManager h WHERE h.EnterpriseId = ep.Id AND ep.IsDel = 0)HTotalCount,  --总数量
	ep.AccountMax,
	(SELECT COUNT(1) FROM dbo.T_EPAccount ea WHERE ea.EnterpriseId = ep.Id AND ea.IsDel = 0 )AccountCount,
    (SELECT ea.Type FROM dbo.T_EPAccount ea WHERE ea.Id = @UserId  )AccountType,
	(SELECT v.Name FROM dbo.T_EPVIP ev LEFT JOIN dbo.T_VIPInfo v ON ev.VIPInfoId = v.Id WHERE ev.IsDel = 0 AND v.IsDel = 0 AND ev.EnterpriseId = ep.Id AND ev.CityId = @CityId)VipName,
	(SELECT ev.PassDate FROM dbo.T_EPVIP ev WHERE ev.IsDel = 0 AND ev.EnterpriseId = ep.Id AND ev.CityId = @CityId)VipPassDate
FROM
	dbo.T_Enterprise ep
WHERE
	ep.Id = @EPId AND ep.IsDel = 0 ";
            return DbPartJob.FirstOrDefault<GetEPCenterModel>(sql, new {redisModel.EPId, redisModel.CityId,redisModel.UserId});
        }
    }
}
