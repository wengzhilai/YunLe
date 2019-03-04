
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 角色配置
    /// </summary>
    public class ROLE_CONFIG
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// ROLE_ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ROLE_ID")]
        public int ROLE_ID { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [StringLength(10)]
        [Display(Name = "类型")]
        public string TYPE { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
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


    }
}
