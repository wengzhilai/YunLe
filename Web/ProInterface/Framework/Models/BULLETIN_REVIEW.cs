
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 公告回复
    /// </summary>
    public class BULLETIN_REVIEW
    {

            /// <summary>
            /// ID
            /// </summary>
            [Required]
            [Range(0, 2147483647)]
            [Display(Name = "ID")]
            public int ID { get; set; }
            /// <summary>
            /// PARENT_ID
            /// </summary>
            [Range(0, 2147483647)]
            [Display(Name = "PARENT_ID")]
            public Nullable<int> PARENT_ID { get; set; }
            /// <summary>
            /// 归属公告
            /// </summary>
            [Required]
            [Range(0, 2147483647)]
            [Display(Name = "归属公告")]
            public int BULLETIN_ID { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            [StringLength(50)]
            [Display(Name = "名称")]
            public string NAME { get; set; }
            /// <summary>
            /// 内容
            /// </summary>
            [Display(Name = "内容")]
            public string CONTENT { get; set; }
            /// <summary>
            /// 发起人
            /// </summary>
            [Required]
            [Range(0, 2147483647)]
            [Display(Name = "发起人")]
            public int USER_ID { get; set; }
            /// <summary>
            /// 发起时间
            /// </summary>
            [Required]
            [Display(Name = "发起时间")]
            public DateTime ADD_TIME { get; set; }
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
