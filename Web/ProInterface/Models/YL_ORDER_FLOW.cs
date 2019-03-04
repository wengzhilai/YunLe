
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 订单流程
    /// </summary>
    public class YL_ORDER_FLOW:TTaskFlow
    {

        /// <summary>
        /// 订单编号
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "订单编号")]
        public string ORDER_FLOW_NO { get; set; }
        /// <summary>
        /// 付款编号
        /// </summary>
        [StringLength(50)]
        [Display(Name = "付款编号")]
        public string OUT_TRADE_NO { get; set; }
        /// <summary>
        /// ORDER_ID
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "ORDER_ID")]
        public Nullable<int> ORDER_ID { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "层级")]
        public Nullable<int> LEVEL_ID { get; set; }
        /// <summary>
        /// 账户ID
        /// </summary>
        [StringLength(50)]
        [Display(Name = "账户ID")]
        public string SELLER_ID { get; set; }
        /// <summary>
        /// 费用
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "费用")]
        public Nullable<decimal> COST { get; set; }
        /// <summary>
        /// 处理名称
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "处理名称")]
        public string SUBJECT { get; set; }
        /// <summary>
        /// 商品详情
        /// </summary>
        [StringLength(100)]
        [Display(Name = "处理详情")]
        public string BODY { get; set; }
 
        /// <summary>
        /// 发起人
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "发起人")]
        public Nullable<int> BEGIN_USER_ID { get; set; }
        /// <summary>
        /// 发起时间
        /// </summary>
        [Display(Name = "发起时间")]
        public Nullable<DateTime> BEGIN_TIME { get; set; }


    }
}
