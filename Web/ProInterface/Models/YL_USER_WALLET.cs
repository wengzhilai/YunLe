
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 用户钱包
    /// </summary>
    public class YL_USER_WALLET
    {

            /// <summary>
            /// ID
            /// </summary>
            [Required]
            [Range(0, 2147483647)]
            [Display(Name = "ID")]
            public int ID { get; set; }
            /// <summary>
            /// 钱包
            /// </summary>
            [Range(0, 2147483647)]
            [Display(Name = "钱包")]
            public Nullable<decimal> WALLET { get; set; }
            
       
    }
}
