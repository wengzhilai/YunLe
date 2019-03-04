
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 流程节点
    /// </summary>
    public class FLOW_FLOWNODE
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
            [StringLength(100)]
            [Display(Name = "名称")]
            public string NAME { get; set; }
            /// <summary>
            /// 处理地址
            /// </summary>
            [StringLength(200)]
            [Display(Name = "处理地址")]
            public string HANDLE_URL { get; set; }
            /// <summary>
            /// 展示地址
            /// </summary>
            [StringLength(200)]
            [Display(Name = "展示地址")]
            public string SHOW_URL { get; set; }
            
       
    }
}
