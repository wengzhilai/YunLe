using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterface.Models
{
    public class YL_BBS
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "用户")]
        public Nullable<int> USER_ID { get; set; }
        /// <summary>
        /// 案件
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "案件")]
        public Nullable<int> MESSAGE_TYPE_ID { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "地区")]
        public Nullable<int> DISTRICT_ID { get; set; }
        /// <summary>
        /// PARENT_ID
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "PARENT_ID")]
        public Nullable<int> PARENT_ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "名称")]
        public string NAME { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        public string CONTENT { get; set; }
        /// <summary>
        /// 发起时间
        /// </summary>
        [Required]
        [Display(Name = "发起时间")]
        public DateTime ADD_TIME { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Required]
        [StringLength(10)]
        [Display(Name = "状态")]
        public string STATUS { get; set; }
        /// <summary>
        /// 状态时间
        /// </summary>
        [Required]
        [Display(Name = "状态时间")]
        public DateTime STATUS_TIME { get; set; }
        /// <summary>
        /// 星数
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "星数")]
        public int STAR_NUM { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [StringLength(10)]
        [Display(Name = "类型")]
        public string TYPE { get; set; }
    }
}
