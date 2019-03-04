
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 系统用户
    /// </summary>
    public class USER
    {

            /// <summary>
            /// ID
            /// </summary>
            [Required]
            [Display(Name = "ID")]
            public int ID { get; set; }
            /// <summary>
            /// 姓名
            /// </summary>
            [StringLength(80)]
            [Display(Name = "姓名")]
            public string NAME { get; set; }
            /// <summary>
            /// 登录名
            /// </summary>
            [Required]
            [StringLength(20)]
            [Display(Name = "登录名")]
            public string LOGIN_NAME { get; set; }
        /// <summary>
        /// 头像图片
        /// </summary>
        [Display(Name = "头像图片")]
        public int? ICON_FILES_ID { get; set; }
        /// <summary>
        /// 归属地
        /// </summary>
        [Display(Name = "归属地")]
            public int DISTRICT_ID { get; set; }
            /// <summary>
            /// 锁定
            /// </summary>
            [Display(Name = "锁定")]
            public Int16 IS_LOCKED { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            [Display(Name = "创建时间")]
            public Nullable<DateTime> CREATE_TIME { get; set; }
            /// <summary>
            /// 登录次数
            /// </summary>
            [Display(Name = "登录次数")]
            public Nullable<int> LOGIN_COUNT { get; set; }
            /// <summary>
            /// 最后登录时间
            /// </summary>
            [Display(Name = "最后登录时间")]
            public Nullable<DateTime> LAST_LOGIN_TIME { get; set; }
            /// <summary>
            /// 最后离开时间
            /// </summary>
            [Display(Name = "最后离开时间")]
            public Nullable<DateTime> LAST_LOGOUT_TIME { get; set; }
            /// <summary>
            /// 最后活动时间
            /// </summary>
            [Display(Name = "最后活动时间")]
            public Nullable<DateTime> LAST_ACTIVE_TIME { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            [StringLength(2000)]
            [Display(Name = "备注")]
            public string REMARK { get; set; }
            /// <summary>
            /// 归属
            /// </summary>
            [StringLength(10)]
            [Display(Name = "归属")]
            public string REGION { get; set; }
            
       
    }
}
