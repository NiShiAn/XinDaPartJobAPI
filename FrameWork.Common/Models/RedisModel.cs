﻿/************************************************************************************
 *      Copyright (C) 2015 yuwei,All Rights Reserved
 *      File:
 *                RedisModel.cs
 *      Description:
 *            RedisModel
 *      Author:
 *                yxw
 *                
 *                
 *      Finish DateTime:
 *                2018/3/16 17:02:47
 *      History:
 ***********************************************************************************/


namespace FrameWork.Common.Models
{
    /// <summary>
    /// RedisModel
    /// </summary>
    public class RedisModel
    {
        /// <summary>
        /// 个人登录时未求职者用户id，企业登录时为企业用户id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 企业id
        /// </summary>
        public int EPId { set; get; }

        /// <summary>
        /// 用户标识符 GUID字符串
        /// </summary>
        public string Token { set; get; }

        /// <summary>
        /// 微信账户唯一标识符
        /// </summary>
        public string OpenId { set; get; }

        /// <summary>
        /// 微信用户名 
        /// </summary>
        public string WxName { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { set; get; }

        /// <summary>
        /// 当前所在城市id
        /// </summary>
        public string CityId { get; set; }

        /// <summary>
        /// 身份标志，0.缓存失效，1.求职者，2.企业
        /// </summary>
        public TokenMarkEnum Mark { set; get; }

        /// <summary>
        /// 是否是主账号
        /// </summary>
        public bool IsMainAccount { set; get; }
    }
}
