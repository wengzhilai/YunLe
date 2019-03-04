
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 服务器分类
    /// </summary>
    public class DB_SERVER_TYPE
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public Int64 ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(20)]
        [Display(Name = "名称")]
        public string NAME { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        [StringLength(500)]
        [Display(Name = "说明")]
        public string REMARK { get; set; }


    }
}
