
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 公告回复
    /// </summary>
    public class BulletinReview:BULLETIN_REVIEW
    {

        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [Display(Name = "电话")]
        public string UserPhone { get; set; }

        /// <summary>
        /// 归属
        /// </summary>
        [Display(Name = "归属")]
        public string DistictName { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [Display(Name = "角色")]
        public string UserRole { get; set; }
       
    }
}
