
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 短信发送
    /// </summary>
    public class SMS_SEND
    {

            /// <summary>
            /// KEY
            /// </summary>
            [Required]
            [StringLength(32)]
            [Display(Name = "KEY")]
            public string KEY { get; set; }
            /// <summary>
            /// MESSAGE_ID
            /// </summary>
            [Range(0, 2147483647)]
            [Display(Name = "MESSAGE_ID")]
            public Nullable<Int64> MESSAGE_ID { get; set; }
            /// <summary>
            /// 电话号码
            /// </summary>
            [Required]
            [StringLength(50)]
            [Display(Name = "电话号码")]
            public string PHONE_NO { get; set; }
            /// <summary>
            /// 添加时间
            /// </summary>
            [Display(Name = "添加时间")]
            public Nullable<DateTime> ADD_TIME { get; set; }
            /// <summary>
            /// 发送时间
            /// </summary>
            [Display(Name = "发送时间")]
            public Nullable<DateTime> SEND_TIME { get; set; }
            /// <summary>
            /// 发送内容
            /// </summary>
            [Required]
            [StringLength(500)]
            [Display(Name = "发送内容")]
            public string CONTENT { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            [StringLength(15)]
            [Display(Name = "状态")]
            public string STAUTS { get; set; }
            
       
    }
}
