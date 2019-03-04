
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 用户地址
    /// </summary>
    public class YL_USER_ADDRESS
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// USER_ID
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "USER_ID")]
        public Nullable<int> USER_ID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [StringLength(20)]
        [Display(Name = "姓名")]
        public string NAME { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [StringLength(30)]
        [Display(Name = "联系电话")]
        public string PHONE { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        [StringLength(20)]
        [Display(Name = "城市")]
        public string CITY { get; set; }
        /// <summary>
        /// 区县
        /// </summary>
        [StringLength(20)]
        [Display(Name = "区县")]
        public string COUNTY { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        [StringLength(100)]
        [Display(Name = "详细地址")]
        public string ADDRESS { get; set; }
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
        /// 默认
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "默认")]
        public Nullable<Int16> IS_DEFAULT { get; set; }


    }
}
