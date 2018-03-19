﻿using System;
using PetaPoco;

namespace FrameWork.Entity.Entity
{
    [TableName("T_ReportCVImg")]
    [PrimaryKey("Id")]
    public class T_ReportCVImg
    {
        
        /// <summary>
        /// - 
        /// </summary>
        public int Id {get;set;}

        /// <summary>
        /// 举报记录id 
        /// </summary>
        public int ReportCVId {get;set;}

        /// <summary>
        /// 举报说明图片地址 
        /// </summary>
        public string PicUrl {get;set;}

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
