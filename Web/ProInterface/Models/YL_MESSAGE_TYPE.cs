
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 提醒类型
    /// </summary>
    public class YL_MESSAGE_TYPE
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// 类型名称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "类型名称")]
        public string NAME { get; set; }
        /// <summary>
        /// 关联的表
        /// </summary>
        [StringLength(50)]
        [Display(Name = "关联的表")]
        public string TABLE_NAME { get; set; }
        /// <summary>
        /// 在用
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "在用")]
        public Nullable<Int16> IS_USE { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        [Display(Name = "备注")]
        public string REMARK { get; set; }


    }
}
