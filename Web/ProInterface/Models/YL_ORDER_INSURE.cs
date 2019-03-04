
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 投保
    /// </summary>
    public class YL_ORDER_INSURE:YlOrder
    {

        /// <summary>
        /// INSURER_ID
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "INSURER_ID")]
        public Nullable<int> INSURER_ID { get; set; }
        /// <summary>
        /// 购买方式
        /// </summary>
        [StringLength(20)]
        [Display(Name = "购买方式")]
        public string PURCHASE_WAY { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        [Display(Name = "开始日期")]
        public Nullable<DateTime> DATE_START { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        [Display(Name = "结束日期")]
        public Nullable<DateTime> DATE_END { get; set; }
        /// <summary>
        /// 本年过户车
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "本年过户车")]
        public Nullable<Int16> IS_TRANSFER { get; set; }
        /// <summary>
        /// 车主姓名
        /// </summary>
        [StringLength(20)]
        [Display(Name = "车主姓名")]
        public string CAR_USERNAME { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        [StringLength(20)]
        [Display(Name = "身份证号")]
        public string ID_NO { get; set; }
        /// <summary>
        /// 车主联系电话
        /// </summary>
        [StringLength(50)]
        [Display(Name = "车主联系电话")]
        public string CAR_OWNER { get; set; }
        /// <summary>
        /// 被保人姓名
        /// </summary>
        [StringLength(20)]
        [Display(Name = "被保人姓名")]
        public string INSURED_NAME { get; set; }
        /// <summary>
        /// 申请人姓名
        /// </summary>
        [StringLength(20)]
        [Display(Name = "申请人姓名")]
        public string PROPOSER_NAME { get; set; }
        /// <summary>
        /// 配送信息
        /// </summary>
        [StringLength(200)]
        [Display(Name = "配送信息")]
        public string DELIVERY { get; set; }
        /// <summary>
        /// 车主身份证正面
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "车主身份证正面")]
        public Nullable<int> ID_NO_PIC_ID { get; set; }
        /// <summary>
        /// 车主身份证背面
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "车主身份证背面")]
        public Nullable<int> ID_NO_PIC_ID1 { get; set; }
        /// <summary>
        /// 行驶证正面
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "行驶证正面")]
        public Nullable<int> DRIVING_PIC_ID { get; set; }
        /// <summary>
        /// 行驶证背面
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "行驶证背面")]
        public Nullable<int> DRIVING_PIC_ID1 { get; set; }
        /// <summary>
        /// 投保人证件正面
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "投保人证件正面")]
        public Nullable<int> DRIVER_PIC_ID { get; set; }
        /// <summary>
        /// 投保人证件背面
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "投保人证件背面")]
        public Nullable<int> DRIVER_PIC_ID1 { get; set; }
        /// <summary>
        /// 被保人证件正面
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "被保人证件正面")]
        public Nullable<int> RECOGNIZEE_PIC_ID { get; set; }
        /// <summary>
        /// 被保人证件背面
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "被保人证件背面")]
        public Nullable<int> RECOGNIZEE_PIC_ID1 { get; set; }
        /// <summary>
        /// 险种名称
        /// </summary>
        [StringLength(100)]
        [Display(Name = "险种名称")]
        public string INS_NAME { get; set; }
        /// <summary>
        /// 投保人
        /// </summary>
        [StringLength(100)]
        [Display(Name = "投保人")]
        public string BUY_USERNAME { get; set; }
        /// <summary>
        /// 出单员姓名
        /// </summary>
        [StringLength(100)]
        [Display(Name = "出单员姓名")]
        public string MAKE_ORDER_USERNAME { get; set; }
        /// <summary>
        /// 业务性质
        /// </summary>
        [StringLength(100)]
        [Display(Name = "业务性质")]
        public string BUSINESS_NATURE { get; set; }
        /// <summary>
        /// 车辆种类
        /// </summary>
        [StringLength(100)]
        [Display(Name = "车辆种类")]
        public string CAR_TYPE { get; set; }
        /// <summary>
        /// 结算单号
        /// </summary>
        [StringLength(100)]
        [Display(Name = "结算单号")]
        public string ACCOUNT_NO { get; set; }
        /// <summary>
        /// 产品
        /// </summary>
        [StringLength(100)]
        [Display(Name = "产品")]
        public string PRODUCT_NAME { get; set; }
        /// <summary>
        /// 业务归属人
        /// </summary>
        [StringLength(100)]
        [Display(Name = "业务归属人")]
        public string BUSINE_USER { get; set; }
        /// <summary>
        /// 费用比例
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "费用比例")]
        public Nullable<decimal> COST_RATIO { get; set; }
        /// <summary>
        /// 已结算手续费
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "已结算手续费")]
        public Nullable<decimal> SUCC_FEE { get; set; }
        /// <summary>
        /// 渠道代码
        /// </summary>
        [StringLength(100)]
        [Display(Name = "渠道代码")]
        public string CHANNEL_CODE { get; set; }
        /// <summary>
        /// 归属机构
        /// </summary>
        [StringLength(100)]
        [Display(Name = "归属机构")]
        public string BE_GROUP { get; set; }
        /// <summary>
        /// 出单机构
        /// </summary>
        [StringLength(100)]
        [Display(Name = "出单机构")]
        public string MAKE_GROUP { get; set; }
        /// <summary>
        /// 经办人
        /// </summary>
        [StringLength(100)]
        [Display(Name = "经办人")]
        public string OPERATOR_USER { get; set; }
        /// <summary>
        /// 签单保费
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "签单保费")]
        public Nullable<decimal> ALL_COST { get; set; }
        /// <summary>
        /// 实收保费
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "实收保费")]
        public Nullable<decimal> FACT_COST { get; set; }
        /// <summary>
        /// 应付手续费
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "应付手续费")]
        public Nullable<decimal> MEET_COST { get; set; }
        /// <summary>
        /// 结算比例
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "结算比例")]
        public Nullable<decimal> SETTLE_RATIO { get; set; }
        /// <summary>
        /// 待结算手续费
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "待结算手续费")]
        public Nullable<decimal> WAIT_MEET_COST { get; set; }
        /// <summary>
        /// 结算时间
        /// </summary>
        [Display(Name = "结算时间")]
        public Nullable<DateTime> SETTLE_TIME { get; set; }
        /// <summary>
        /// 验车人
        /// </summary>
        [StringLength(100)]
        [Display(Name = "验车人")]
        public string DO_PEOPLE { get; set; }


    }
}
