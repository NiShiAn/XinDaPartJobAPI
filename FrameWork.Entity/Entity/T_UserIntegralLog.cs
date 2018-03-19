﻿using System;
using PetaPoco;

namespace FrameWork.Entity.Entity
{
    [TableName("T_UserIntegralLog")]
    [PrimaryKey("Id")]
    public class T_UserIntegralLog
    {
        
        /// <summary>
        /// - 
        /// </summary>
        public int Id {get;set;}

        /// <summary>
        /// 用户id 
        /// </summary>
        public int UserId {get;set;}

        /// <summary>
        /// 增加的积分数 
        /// </summary>
        public int AddValue {get;set;}

        /// <summary>
        /// 加积分原因 
        /// </summary>
        public string AddReason {get;set;}

        /// <summary>
        /// 总积分数 
        /// </summary>
        public int TotalIntegral {get;set;}

        /// <summary>
        /// 是否删除 
        /// </summary>
        public bool IsDel {get;set;}

        /// <summary>
        /// 编辑人id 
        /// </summary>
        public int ModifyUserId {get;set;}

        /// <summary>
        /// 编辑时间 
        /// </summary>
        public DateTime ModifyTime {get;set;}

        /// <summary>
        /// 创建人id 
        /// </summary>
        public int CreateUserId {get;set;}

        /// <summary>
        /// 创建时间 
        /// </summary>
        public DateTime CreateTime {get;set;}

    }
}
