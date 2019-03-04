
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 营销活动类型
    /// </summary>
    public class BULLETIN_TYPE
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
            [StringLength(80)]
            [Display(Name = "名称")]
            public string NAME { get; set; }
            
       
    }
}
