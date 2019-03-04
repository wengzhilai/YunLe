
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 业务员
    /// </summary>
    public class YL_SALESMAN : TUser
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// 团队
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "团队")]
        public Nullable<int> TEAM_ID { get; set; }
        /// <summary>
        /// 推荐码
        /// </summary>
        [StringLength(10)]
        [Display(Name = "推荐码")]
        public string REQUEST_CODE { get; set; }
        /// <summary>
        /// PARENT_ID
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "PARENT_ID")]
        public Nullable<int> PARENT_ID { get; set; }
        /// <summary>
        /// 手机唯一标识
        /// </summary>
        [StringLength(50)]
        [Display(Name = "手机唯一标识")]
        public string IMEI { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [StringLength(2)]
        [Display(Name = "性别")]
        public string SEX { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        [StringLength(20)]
        [Display(Name = "身份证号")]
        public string ID_NO { get; set; }
        /// <summary>
        /// 身份证图片
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "身份证图片")]
        public Nullable<int> ID_NO_PIC { get; set; }

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
    }
}
