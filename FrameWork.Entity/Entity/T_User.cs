﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//     稳定好用的Orm【Chloe:http://www.52chloe.com/】
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using PetaPoco;
namespace FrameWork.Entity.Entity
{
    /// <summary>
    /// 实体类T_User。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [TableName("T_User")]
    [PrimaryKey("Id",true)]
    public partial class T_User
    {
        #region Model
		/// <summary>
		/// 
		/// </summary>	
		public int Id { get; set; }

		/// <summary>
		/// 
		/// </summary>	
		public int? TrainingInstitutionId { get; set; }

		/// <summary>
		/// 
		/// </summary>	
		public string Name { get; set; }

		/// <summary>
		/// 用户名
		/// </summary>	
		public string Account { get; set; }

		/// <summary>
		/// 手机号
		/// </summary>	
		public string Phone { get; set; }

		/// <summary>
		/// 
		/// </summary>	
		public string Sex { get; set; }

		/// <summary>
		/// 头像
		/// </summary>	
		public string HeadImg { get; set; }

		/// <summary>
		/// 密码，默认111111，使用MD5加密
		/// </summary>	
		public string Pwd { get; set; }

		/// <summary>
		/// _0：普通后台用户 1：审核老师
		/// </summary>	
		public int? Type { get; set; }

		/// <summary>
		/// 
		/// </summary>	
		public bool? IsDel { get; set; }

		/// <summary>
		/// 
		/// </summary>	
		public bool? IsUsed { get; set; }

		/// <summary>
		/// 
		/// </summary>	
		public DateTime? CreateTime { get; set; }

		#endregion
}
}