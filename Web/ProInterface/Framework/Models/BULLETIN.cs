
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 公告
    /// </summary>
    public class BULLETIN
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        [StringLength(255)]
        [Display(Name = "标题")]
        public string TITLE { get; set; }
        /// <summary>
        /// 公告封面
        /// </summary>
        [StringLength(255)]
        [Display(Name = "公告封面")]
        public string PIC { get; set; }
        /// <summary>
        /// 公告类型
        /// </summary>
        [StringLength(50)]
        [Display(Name = "公告类型")]
        public string TYPE_CODE { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        public string CONTENT { get; set; }
        /// <summary>
        /// 发布人
        /// </summary>
        [Required]
        [StringLength(255)]
        [Display(Name = "发布人")]
        public string PUBLISHER { get; set; }
        /// <summary>
        /// 生效时间
        /// </summary>
        [Required]
        [Display(Name = "生效时间")]
        public DateTime ISSUE_DATE { get; set; }
        /// <summary>
        /// 显示
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "显示")]
        public Int16 IS_SHOW { get; set; }
        /// <summary>
        /// 重要
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "重要")]
        public Int16 IS_IMPORT { get; set; }
        /// <summary>
        /// 置顶
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "置顶")]
        public Int16 IS_URGENT { get; set; }
        /// <summary>
        /// 自动打开
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "自动打开")]
        public Int16 AUTO_PEN { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [Display(Name = "创建时间")]
        public DateTime CREATE_TIME { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [Required]
        [Display(Name = "修改时间")]
        public DateTime UPDATE_TIME { get; set; }
        /// <summary>
        /// REGION
        /// </summary>
        [Required]
        [StringLength(10)]
        [Display(Name = "REGION")]
        public string REGION { get; set; }


    }
}
