
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 组织结构
    /// </summary>
    public class DISTRICT
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// 归属
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "归属")]
        public Nullable<int> PARENT_ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [StringLength(255)]
        [Display(Name = "名称")]
        public string NAME { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        [Required]
        [StringLength(255)]
        [Display(Name = "代码")]
        public string CODE { get; set; }
        /// <summary>
        /// 否用
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "否用")]
        public Int16? IN_USE { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "层级")]
        public int LEVEL_ID { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        [StringLength(200)]
        [Display(Name = "路径")]
        public string ID_PATH { get; set; }
        /// <summary>
        /// REGION
        /// </summary>
        [Required]
        [StringLength(10)]
        [Display(Name = "REGION")]
        public string REGION { get; set; }


    }
}
