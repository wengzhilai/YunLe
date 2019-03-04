
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 用户提现
    /// </summary>
    public class YL_USER_WITHDRAW
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
            /// CARD_ID
            /// </summary>
            [Required]
            [Range(0, 2147483647)]
            [Display(Name = "CARD_ID")]
            public int CARD_ID { get; set; }
            /// <summary>
            /// 金额
            /// </summary>
            [Required]
            [Range(0, 2147483647)]
            [Display(Name = "金额")]
            public decimal MONEY { get; set; }
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
            /// <summary>
            /// 申请时间
            /// </summary>
            [Required]
            [Display(Name = "申请时间")]
            public DateTime CARETE_TIEM { get; set; }
            /// <summary>
            /// 提现备注
            /// </summary>
            [StringLength(100)]
            [Display(Name = "提现备注")]
            public string REMARK { get; set; }
            
       
    }
}
