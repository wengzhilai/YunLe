
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 用户银行卡
    /// </summary>
    public class YL_USER_CARD
    {

            /// <summary>
            /// ID
            /// </summary>
            [Required]
            [Range(0, 2147483647)]
            [Display(Name = "ID")]
            public int ID { get; set; }
            /// <summary>
            /// USER_ID
            /// </summary>
            [Required]
            [Range(0, 2147483647)]
            [Display(Name = "USER_ID")]
            public int USER_ID { get; set; }
            /// <summary>
            /// 开户行
            /// </summary>
            [Required]
            [StringLength(200)]
            [Display(Name = "开户行")]
            public string BANK_NAME { get; set; }
            /// <summary>
            /// 开户名
            /// </summary>
            [Required]
            [StringLength(50)]
            [Display(Name = "开户名")]
            public string USER_NAME { get; set; }
            /// <summary>
            /// 卡号
            /// </summary>
            [Required]
            [StringLength(20)]
            [Display(Name = "卡号")]
            public string CARD_NUMBER { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            [Required]
            [StringLength(10)]
            [Display(Name = "状态")]
            public string STATUS { get; set; }
            /// <summary>
            /// 状态时间
            /// </summary>
            [Required]
            [Display(Name = "状态时间")]
            public DateTime STATUS_TIME { get; set; }
            
       
    }
}
