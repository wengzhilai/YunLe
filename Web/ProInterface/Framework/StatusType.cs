using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace ProInterface
{
    /// <summary>
    ///  
    /// </summary>
    public static class StatusType
    {






        /// <summary>
        ///用户操作日志类型
        ///<para>其它、登陆、查询、添加、修改、删除、导入、导出、审核、分析</para>
        /// </summary>
        public enum UserLogType
        {
            /// <summary>
            /// 其它
            /// </summary>
            [Description("其它")]
            Other = 1,
            /// <summary>
            /// 登陆
            /// </summary>
            [Description("登陆")]
            Login=1,
            /// <summary>
            /// 查询
            /// </summary>
            [Description("查询")]
            Select=2,
            /// <summary>
            /// 添加
            /// </summary>
            [Description("添加")]
            Add=3,
            /// <summary>
            /// 修改
            /// </summary>
            [Description("修改")]
            Edit=4,
            /// <summary>
            /// 删除
            /// </summary>
            [Description("删除")]
            Delete=5,
            /// <summary>
            /// 导入
            /// </summary>
            [Description("导入")]
            Import=6,
            /// <summary>
            /// 导出
            /// </summary>
            [Description("导出")]
            Export=7,
            /// <summary>
            /// 审核
            /// </summary>
            [Description("审核")]
            Auditing=8,
            /// <summary>
            /// 分析
            /// </summary>
            [Description("分析")]
            Analyse=9
        }



        /// <summary>
        ///模块状态
        ///<para>字段：PCSS_MODULE => STATUS</para>
        ///<para>隐藏、正常</para>
        /// </summary>
        public enum ModuleStatus
        {
            /// <summary>
            /// 隐藏
            /// </summary>
            [Description("隐藏")]
            Hide = 0,
            /// <summary>
            /// 正常
            /// </summary>
            [Description("正常")]
            Normal = 1
        }





        /// <summary>
        /// 文件类型
        ///<para>未使用、已使用</para>
        /// </summary>
        public enum IsUse
        {
            /// <summary>
            /// 未使用
            /// </summary>
            [Description("未使用")]
            No = 0,
            /// <summary>
            /// 已使用
            /// </summary>
            [Description("已使用")]
            Yes = 1,
            /// <summary>
            /// 等待中
            /// </summary>
            [Description("等待中")]
            Waiting = 2
        }

        
        
        #region 得到描述信息

        /// <summary>
        /// 获取枚举变量值的 Description 属性
        /// 
        /// </summary>
        /// <param name="obj">枚举变量</param>
        /// <returns>如果包含 Description 属性，则返回 Description 属性的值，否则返回枚举变量值的名称</returns>
        public static string GetDescription(this object obj)
        {
            return GetDescription(obj, false);
        }

        /// <summary>
        /// 获取枚举变量值的 Description 属性
        /// </summary>
        /// <param name="obj">枚举变量</param>
        /// <param name="isTop">是否改变为返回该类、枚举类型的头 Description 属性，而不是当前的属性或枚举变量值的 Description 属性</param>
        /// <returns>如果包含 Description 属性，则返回 Description 属性的值，否则返回枚举变量值的名称</returns>
        public static string GetDescription(this object obj, bool isTop)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            try
            {
                Type _enumType = obj.GetType();
                DescriptionAttribute dna = null;
                if (isTop)
                {
                    dna = (DescriptionAttribute)Attribute.GetCustomAttribute(_enumType, typeof(DescriptionAttribute));
                }
                else
                {
                    FieldInfo fi = _enumType.GetField(Enum.GetName(_enumType, obj));
                    dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                }
                if (dna != null && string.IsNullOrEmpty(dna.Description) == false)
                    return dna.Description;
            }
            catch
            {
            }
            return obj.ToString();
        } 

        
        #endregion
    }
}
