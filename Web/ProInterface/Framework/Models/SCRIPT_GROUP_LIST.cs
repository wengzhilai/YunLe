
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 脚本组列表
    /// </summary>
    public class SCRIPT_GROUP_LIST
    {
        public string CODE { get; set; }
        public string NAME { get; set; }
        /// <summary>
        /// 脚本
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "脚本")]
        public int SCRIPT_ID { get; set; }
        /// <summary>
        /// 脚本组
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "脚本组")]
        public int GROUP_ID { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "排序")]
        public int ORDER_INDEX { get; set; }


    }
}
