
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 提醒
    /// </summary>
    public class USER_MESSAGE:MESSAGE
    {

            /// <summary>
            /// MESSAGE_ID
            /// </summary>
            [Required]
            [Range(0, 2147483647)]
            [Display(Name = "MESSAGE_ID")]
            public int MESSAGE_ID { get; set; }
            /// <summary>
            /// 接收用户
            /// </summary>
            [Required]
            [Range(0, 2147483647)]
            [Display(Name = "接收用户")]
            public int USER_ID { get; set; }
            /// <summary>
            /// 接收用户电话
            /// </summary>
            [StringLength(20)]
            [Display(Name = "接收用户电话")]
            public string PHONE_NO { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            [StringLength(10)]
            [Display(Name = "状态")]
            public string STATUS { get; set; }
            /// <summary>
            /// 状态时间
            /// </summary>
            [Display(Name = "状态时间")]
            public Nullable<DateTime> STATUS_TIME { get; set; }
            /// <summary>
            /// 回复信息
            /// </summary>
            [StringLength(500)]
            [Display(Name = "回复信息")]
            public string REPLY { get; set; }
            /// <summary>
            /// 推送方式
            /// </summary>
            [StringLength(10)]
            [Display(Name = "推送方式")]
            public string PUSH_TYPE { get; set; }
            
       
    }
}
