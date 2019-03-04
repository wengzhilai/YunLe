
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 救援和保养
    /// </summary>
    public class YL_ORDER_RESCUE : YlOrder
    {

        /// <summary>
        /// 维修站
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "维修站")]
        public Nullable<int> GARAGE_ID { get; set; }
        
        /// <summary>
        /// 送修方式
        /// </summary>
        [StringLength(10)]
        [Display(Name = "送修方式")]
        public string REACH_TYPE { get; set; }
        /// <summary>
        /// 送到时间
        /// </summary>
        [Display(Name = "送到时间")]
        public Nullable<DateTime> REACH_TIME { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        [StringLength(80)]
        [Display(Name = "联系人姓名")]
        public string CLIENT_NAME { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        [StringLength(20)]
        [Display(Name = "联系人电话")]
        public string CLIENT_PHONE { get; set; }
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
        /// 故障类型
        /// </summary>
        [StringLength(10)]
        [Display(Name = "故障类型")]
        public string HITCH_TYPE { get; set; }
        /// <summary>
        /// 其它联系方式
        /// </summary>
        [StringLength(50)]
        [Display(Name = "其它联系方式")]
        public string OTHER_PHONE { get; set; }
        /// <summary>
        /// 接车地址
        /// </summary>
        [StringLength(200)]
        [Display(Name = "接车地址")]
        public string ADDRESS { get; set; }
        /// <summary>
        /// 接车时间
        /// </summary>
        [Display(Name = "接车时间")]
        public Nullable<DateTime> PICK_TIME { get; set; }
    }
}
