
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 团队
    /// </summary>
    public class YL_TEAM
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }


        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "归属")]
        public int DISTRICT_ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [StringLength(80)]
        [Display(Name = "名称")]
        public string NAME { get; set; }
        /// <summary>
        /// 推荐码
        /// </summary>
        [StringLength(10)]
        [Display(Name = "推荐码")]
        public string REQUEST_CODE { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        [StringLength(1000)]
        [Display(Name = "说明")]
        public string REMARK { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "创建人")]
        public int ADD_USER_ID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [Display(Name = "创建时间")]
        public DateTime CREATE_TIME { get; set; }

        /// <summary>
        /// 团队长ID
        /// </summary>
        [StringLength(500)]
        [Display(Name = "团队长")]
        public string LEAD_ID_STR { get; set; }
    }
}
