
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 客户
    /// </summary>
    public class YL_CLIENT: TUser
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [StringLength(2)]
        [Display(Name = "性别")]
        public string SEX { get; set; }
        /// <summary>
        /// 联系地址
        /// </summary>
        [StringLength(200)]
        [Display(Name = "联系地址")]
        public string ADDRESS { get; set; }
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
        /// 备注
        /// </summary>
        [StringLength(200)]
        [Display(Name = "备注")]
        public string REMARK { get; set; }
        /// <summary>
        /// 业务员
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "业务员")]
        public Nullable<int> SALESMAN_ID { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        [StringLength(20)]
        [Display(Name = "身份证号")]
        public string ID_NO { get; set; }
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
        /// 驾驶证照片正
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "驾驶证照片正")]
        public Nullable<int> DRIVER_PIC_ID { get; set; }
        /// <summary>
        /// 驾驶证照片背
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "驾驶证照片背")]
        public Nullable<int> DRIVER_PIC_ID1 { get; set; }


    }
}
