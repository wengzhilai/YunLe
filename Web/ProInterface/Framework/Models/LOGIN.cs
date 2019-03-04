
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 系统用户
    /// </summary>
    public class LOGIN 
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        [StringLength(20)]
        [Display(Name = "登录名")]
        public string LOGIN_NAME { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "密码")]
        public string PASSWORD { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        [StringLength(20)]
        [Display(Name = "电话")]
        public string PHONE_NO { get; set; }
        /// <summary>
        /// 邮件
        /// </summary>
        [StringLength(255)]
        [Display(Name = "邮件")]
        public string EMAIL_ADDR { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        [StringLength(10)]
        [Display(Name = "验证码")]
        public string VERIFY_CODE { get; set; }
        /// <summary>
        /// 验证时间
        /// </summary>
        [Display(Name = "验证时间")]
        public Nullable<DateTime> VERIFY_TIME { get; set; }
        /// <summary>
        /// 锁定
        /// </summary>
        [Display(Name = "锁定")]
        public Nullable<int> IS_LOCKED { get; set; }
        /// <summary>
        /// 修改密码时间
        /// </summary>
        [Display(Name = "修改密码时间")]
        public Nullable<DateTime> PASS_UPDATE_DATE { get; set; }
        /// <summary>
        /// 锁定原因
        /// </summary>
        [StringLength(255)]
        [Display(Name = "锁定原因")]
        public string LOCKED_REASON { get; set; }
        /// <summary>
        /// 是否记住用户名
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// 配置授权
        /// </summary>
        [Display(Name = "配置授权")]
        public string AllOauthStr { get; set; }


    }
}
