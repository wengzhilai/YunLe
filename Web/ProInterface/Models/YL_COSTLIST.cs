
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 费用明细
    /// </summary>
    public class YL_COSTLIST
    {

            /// <summary>
            /// ID
            /// </summary>
            [Required]
            [Range(0, 2147483647)]
            [Display(Name = "ID")]
            public int ID { get; set; }
            /// <summary>
            /// 归属用户ID
            /// </summary>
            [Range(0, 2147483647)]
            [Display(Name = "归属用户ID")]
            public Nullable<int> USER_ID { get; set; }
            /// <summary>
            /// 定单号
            /// </summary>
            [StringLength(20)]
            [Display(Name = "定单号")]
            public string ORDER_NO { get; set; }
            /// <summary>
            /// 金额
            /// </summary>
            [Range(0, 2147483647)]
            [Display(Name = "金额")]
            public Nullable<decimal> COST { get; set; }
            /// <summary>
            /// 说明
            /// </summary>
            [Display(Name = "说明")]
            public string REMARK { get; set; }
            /// <summary>
            /// 创建人
            /// </summary>
            [Range(0, 2147483647)]
            [Display(Name = "创建人")]
            public Nullable<int> CREATE_USER_ID { get; set; }
            /// <summary>
            /// 添加时间
            /// </summary>
            [Display(Name = "添加时间")]
            public Nullable<DateTime> CREATE_TIME { get; set; }
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
            /// 来源类型
            /// </summary>
            [StringLength(10)]
            [Display(Name = "来源类型")]
            public string TYPE { get; set; }
            
       
    }
}
