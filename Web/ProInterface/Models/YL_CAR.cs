
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 车辆
    /// </summary>
    public class YL_CAR
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        [StringLength(10)]
        [Display(Name = "车牌号")]
        public string PLATE_NUMBER { get; set; }
        /// <summary>
        /// 车辆类型
        /// </summary>
        [StringLength(10)]
        [Display(Name = "车辆类型")]
        public string CAR_TYPE { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        [StringLength(50)]
        [Display(Name = "品牌")]
        public string BRAND { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        [StringLength(50)]
        [Display(Name = "型号")]
        public string MODEL { get; set; }
        /// <summary>
        /// 价格(万)
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "价格(万)")]
        public Nullable<decimal> PRICE { get; set; }
        /// <summary>
        /// 车架型号
        /// </summary>
        [StringLength(50)]
        [Display(Name = "车架型号")]
        public string FRAME_NUMBER { get; set; }
        /// <summary>
        /// 发动机号
        /// </summary>
        [StringLength(50)]
        [Display(Name = "发动机号")]
        public string ENGINE_NUMBER { get; set; }
        /// <summary>
        /// 过户日期
        /// </summary>
        [Display(Name = "过户日期")]
        public Nullable<DateTime> TRANSFER_DATA { get; set; }
        /// <summary>
        /// 注册日期
        /// </summary>
        [Display(Name = "注册日期")]
        public Nullable<DateTime> REG_DATA { get; set; }
        /// <summary>
        /// 行驶证照片正
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "行驶证照片正")]
        public Nullable<int> DRIVING_PIC_ID { get; set; }
        /// <summary>
        /// 行驶证照片背
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "行驶证照片背")]
        public Nullable<int> DRIVING_PIC_ID1 { get; set; }
        /// <summary>
        /// 默认
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "默认")]
        public Nullable<int> IS_DEFAULT { get; set; }
        /// <summary>
        /// 身份证照片正
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "身份证照片正")]
        public Nullable<int> ID_NO_PIC_ID { get; set; }
        /// <summary>
        /// 身份证照片背
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "身份证照片背")]
        public Nullable<int> ID_NO_PIC_ID1 { get; set; }


        /// <summary>
        /// 发票
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "发票")]
        public Nullable<int> BILL_PIC_ID { get; set; }

        /// <summary>
        /// 合格证
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "合格证")]
        public Nullable<int> CERTIFICATE_PIC_ID { get; set; }

        /// <summary>
        /// 新车
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "新车")]
        public int IS_NEW { get; set; }
    }
}
