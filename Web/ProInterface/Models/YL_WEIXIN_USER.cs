
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 微信粉丝
    /// </summary>
    public class YL_WEIXIN_USER
    {

        /// <summary>
        /// OPENID
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "OPENID")]
        public string OPENID { get; set; }
        /// <summary>
        /// USER_ID
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "USER_ID")]
        public Nullable<int> USER_ID { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "昵称")]
        public string NICKNAME { get; set; }
        /// <summary>
        /// 是否订阅
        /// </summary>
        [Display(Name = "是否订阅")]
        public Nullable<Int16> SUBSCRIBE { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public Nullable<Int16> SEX { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        [StringLength(50)]
        [Display(Name = "城市")]
        public string CITY { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        [StringLength(50)]
        [Display(Name = "国家")]
        public string COUNTRY { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        [StringLength(50)]
        [Display(Name = "省份")]
        public string PROVINCE { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        [StringLength(50)]
        [Display(Name = "语言")]
        public string LANGUAGE { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [StringLength(50)]
        [Display(Name = "头像")]
        public string HEADIMGURL { get; set; }
        /// <summary>
        /// 订阅时间
        /// </summary>
        [Display(Name = "订阅时间")]
        public Nullable<DateTime> TAKE_TIME { get; set; }
        /// <summary>
        /// 退订时间
        /// </summary>
        [Display(Name = "退订时间")]
        public Nullable<DateTime> EXIT_TIME { get; set; }

        /// <summary>
        /// 场景值
        /// </summary>
        [StringLength(64)]
        [Display(Name = "场景值")]
        public string SCENE_STR { get; set; }
    }
}
