
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 保险公司险种
    /// </summary>
    public class YL_INSURER_PRODUCT
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
        [StringLength(100)]
        [Display(Name = "名称")]
        public string NAME { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        [StringLength(100)]
        [Display(Name = "分类")]
        public string TYPE { get; set; }
        /// <summary>
        /// 是否必须
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "是否必须")]
        public Nullable<int> IS_MUST { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        [StringLength(100)]
        [Display(Name = "分类")]
        public string ALL_ITEM { get; set; }

        /// <summary>
        /// 上级
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "上级")]
        public Nullable<int> PARENT_ID { get; set; }

        /// <summary>
        /// 不计免赔
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "不计免赔")]
        public Nullable<int> IS_ALL { get; set; }

        /// <summary>
        /// 主险
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "主险")]
        public Nullable<int> IS_MAIN { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        [StringLength(20)]
        [Display(Name = "默认值")]
        public string DEFAULT_VAL { get; set; }
        
    }
}
