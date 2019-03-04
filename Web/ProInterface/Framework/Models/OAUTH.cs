
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 登录
    /// </summary>
    public class OAUTH
    {

        /// <summary>
        /// KEY
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "KEY")]
        public int KEY { get; set; }
        /// <summary>
        /// NAME
        /// </summary>
        [StringLength(50)]
        [Display(Name = "NAME")]
        public string NAME { get; set; }
        /// <summary>
        /// 注册地址
        /// </summary>
        [StringLength(500)]
        [Display(Name = "注册地址")]
        public string REG_URL { get; set; }
        /// <summary>
        /// 登录地址
        /// </summary>
        [StringLength(500)]
        [Display(Name = "登录地址")]
        public string LOGIN_URL { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        [Display(Name = "备注")]
        public string REMARK { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string openId { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string state { get; set; }
        public string access_token { get; set; }
    }
}
