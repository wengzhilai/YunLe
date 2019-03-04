
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 保险公司
    /// </summary>
    public class YL_INSURER
    {

            /// <summary>
            /// ID
            /// </summary>
            [Required]
            [Range(0, 2147483647)]
            [Display(Name = "ID")]
            public int ID { get; set; }
            /// <summary>
            /// 保险公司名称
            /// </summary>
            [StringLength(50)]
            [Display(Name = "保险公司名称")]
            public string NAME { get; set; }
            /// <summary>
            /// 图片ID
            /// </summary>
            [Range(0, 2147483647)]
            [Display(Name = "图片ID")]
            public Nullable<int> PIC_ID { get; set; }
            
       
    }
}
