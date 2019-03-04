
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 车辆
    /// </summary>
    public class YL_ORDER
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// CAR_ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "CAR_ID")]
        public int CAR_ID { get; set; }
        /// <summary>
        /// CLIENT_ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "CLIENT_ID")]
        public int CLIENT_ID { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        [StringLength(50)]
        [Display(Name = "订单号")]
        public string ORDER_NO { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        [StringLength(100)]
        [Display(Name = "分类")]
        public string ORDER_TYPE { get; set; }
        /// <summary>
        /// 付款状态
        /// </summary>
        [StringLength(10)]
        [Display(Name = "付款状态")]
        public string PAY_STATUS { get; set; }

        /// <summary>
        /// 状态时间
        /// </summary>
        [Display(Name = "状态时间")]
        public Nullable<DateTime> PAY_STAUTS_TIME { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "金额")]
        public Nullable<decimal> COST { get; set; }
        /// <summary>
        /// 下单时间
        /// </summary>
        [Display(Name = "下单时间")]
        public DateTime CREATE_TIME { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        [StringLength(10)]
        [Display(Name = "经度")]
        public string LANG { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        [StringLength(10)]
        [Display(Name = "纬度")]
        public string LAT { get; set; }
        /// <summary>
        /// 评价分数
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "评价分数")]
        public Nullable<int> APPRAISE_SCORE { get; set; }
        /// <summary>
        /// 评价内容
        /// </summary>
        [StringLength(100)]
        [Display(Name = "评价内容")]
        public string APPRAISE_CONTENT { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(1000)]
        [Display(Name = "备注")]
        public string REMARK { get; set; }

        /// <summary>
        /// 重要
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "重要")]
        public int VITAL { get; set; }

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
        public DateTime STATUS_TIME { get; set; }
    }
}
