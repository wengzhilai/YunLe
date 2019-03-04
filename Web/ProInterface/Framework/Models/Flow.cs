
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 流程
    /// </summary>
    public class FLOW
    {
            /// <summary>
            /// ID
            /// </summary>
            [Required]
            [Display(Name = "ID")]
            public int ID { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            [Required]
            [StringLength(100)]
            [Display(Name = "名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 流程类别
            /// </summary>
            [StringLength(20)]
            [Display(Name = "流程类别")]
            public string FLOW_TYPE { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            [StringLength(100)]
            [Display(Name = "备注")]
            public string REMARK { get; set; }
            /// <summary>
            /// 坐标信息
            /// </summary>
            [StringLength(500)]
            [Display(Name = "坐标信息")]
            public string X_Y { get; set; }
            /// <summary>
            /// REGION
            /// </summary>
            [StringLength(10)]
            [Display(Name = "REGION")]
            public string REGION { get; set; }
            
       
    }
}
