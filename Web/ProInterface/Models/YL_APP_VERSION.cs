
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class YL_APP_VERSION
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// IS_NEW
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "IS_NEW")]
        public Int16 IS_NEW { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "类型")]
        public string TYPE { get; set; }
        /// <summary>
        /// 更新说明
        /// </summary>
        [StringLength(1000)]
        [Display(Name = "更新说明")]
        public string REMARK { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        public Nullable<DateTime> UPDATE_TIME { get; set; }

        [StringLength(1000)]
        [Display(Name = "更新地址")]
        public string UPDATE_URL { get; set; }
    }
}
