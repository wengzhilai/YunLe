
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 维护站
    /// </summary>
    public class YL_GARAGE
    {

            /// <summary>
            /// ID
            /// </summary>
            [Required]
            [Range(0, 2147483647)]
            [Display(Name = "ID")]
            public int ID { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            [Required]
            [StringLength(200)]
            [Display(Name = "名称")]
            public string NAME { get; set; }
            /// <summary>
            /// 地点
            /// </summary>
            [StringLength(200)]
            [Display(Name = "地点")]
            public string ADDRESS { get; set; }
            /// <summary>
            /// 经度
            /// </summary>
            [StringLength(10)]
            [Display(Name = "经度")]
            public string LANG { get; set; }
            /// <summary>
            /// 纬度
            /// </summary>
            [StringLength(10)]
            [Display(Name = "纬度")]
            public string LAT { get; set; }
            /// <summary>
            /// 联系电话
            /// </summary>
            [StringLength(20)]
            [Display(Name = "联系电话")]
            public string PHONE { get; set; }
            /// <summary>
            /// 图片ID
            /// </summary>
            [Range(0, 2147483647)]
            [Display(Name = "图片ID")]
            public Nullable<int> PIC_ID { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            [StringLength(200)]
            [Display(Name = "备注")]
            public string REMARK { get; set; }
            
       
    }
}
