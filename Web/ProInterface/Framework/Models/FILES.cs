
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 文件表
    /// </summary>
    public class FILES
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "名称")]
        public string NAME { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        [Required]
        [StringLength(200)]
        [Display(Name = "路径")]
        public string PATH { get; set; }
        /// <summary>
        /// USER_ID
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "USER_ID")]
        public Nullable<int> USER_ID { get; set; }
        /// <summary>
        /// 大小
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "大小")]
        public int LENGTH { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        [Display(Name = "添加时间")]
        public Nullable<DateTime> UPLOAD_TIME { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "备注")]
        public string REMARK { get; set; }
        /// <summary>
        /// 相对路径
        /// </summary>
        [StringLength(254)]
        [Display(Name = "相对路径")]
        public string URL { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        [StringLength(50)]
        [Display(Name = "文件类型")]
        public string FILE_TYPE { get; set; }


    }
}
