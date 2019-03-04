
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 模块
    /// </summary>
    public class MODULE
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// 上级
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "上级")]
        public Nullable<int> PARENT_ID { get; set; }
        /// <summary>
        /// 模块名
        /// </summary>
        [StringLength(60)]
        [Display(Name = "模块名")]
        public string NAME { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "地址")]
        public string LOCATION { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        [StringLength(20)]
        [Display(Name = "代码")]
        public string CODE { get; set; }
        /// <summary>
        /// 调试
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "调试")]
        public Int16 IS_DEBUG { get; set; }
        /// <summary>
        /// 隐藏
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "隐藏")]
        public Int16 IS_HIDE { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "排序")]
        public Int16 SHOW_ORDER { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "描述")]
        public string DESCRIPTION { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "图片")]
        public string IMAGE_URL { get; set; }
        /// <summary>
        /// 桌面角色
        /// </summary>
        [StringLength(200)]
        [Display(Name = "桌面角色")]
        public string DESKTOP_ROLE { get; set; }
        /// <summary>
        /// 宽
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "宽")]
        public Nullable<int> W { get; set; }
        /// <summary>
        /// 高
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "高")]
        public Nullable<int> H { get; set; }


    }
}
