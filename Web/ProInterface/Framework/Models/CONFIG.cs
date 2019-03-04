
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class CONFIG
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// TYPE
        /// </summary>
        [StringLength(10)]
        [Display(Name = "TYPE")]
        public string TYPE { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        [Required]
        [StringLength(32)]
        [Display(Name = "代码")]
        public string CODE { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "名称")]
        public string NAME { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        [StringLength(300)]
        [Display(Name = "值")]
        public string VALUE { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        [StringLength(500)]
        [Display(Name = "说明")]
        public string REMARK { get; set; }
        /// <summary>
        /// REGION
        /// </summary>
        [Required]
        [StringLength(10)]
        [Display(Name = "REGION")]
        public string REGION { get; set; }


    }
}
