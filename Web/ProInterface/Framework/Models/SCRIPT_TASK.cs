
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 任务
    /// </summary>
    public class SCRIPT_TASK
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public Int64 ID { get; set; }
        /// <summary>
        /// 口径脚本ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "口径脚本ID")]
        public Int64 SCRIPT_ID { get; set; }
        /// <summary>
        /// 任务脚本
        /// </summary>
        [Required]
        [Display(Name = "任务脚本")]
        public string BODY_TEXT { get; set; }
        /// <summary>
        /// 脚本哈希值
        /// </summary>
        [Required]
        [StringLength(255)]
        [Display(Name = "脚本哈希值")]
        public string BODY_HASH { get; set; }
        /// <summary>
        /// 运行状态
        /// </summary>
        [Required]
        [StringLength(10)]
        [Display(Name = "运行状态")]
        public string RUN_STATE { get; set; }
        /// <summary>
        /// 时间表达式
        /// </summary>
        [StringLength(30)]
        [Display(Name = "时间表达式")]
        public string RUN_WHEN { get; set; }
        /// <summary>
        /// 脚本参数
        /// </summary>
        [StringLength(255)]
        [Display(Name = "脚本参数")]
        public string RUN_ARGS { get; set; }

        /// <summary>
        /// 运行时间
        /// </summary>
        [StringLength(20)]
        [Display(Name = "运行时间")]
        [Description(@"
支持的格式：
-1d
1d
-1m
1m
20140101
")]
        public string RUN_DATA { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "日志类型")]
        public Nullable<Int16> LOG_TYPE { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        [StringLength(255)]
        [Display(Name = "任务类型")]
        public string DSL_TYPE { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        [StringLength(10)]
        [Display(Name = "任务状态")]
        public string RETURN_CODE { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        public Nullable<DateTime> START_TIME { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        public Nullable<DateTime> END_TIME { get; set; }
        /// <summary>
        /// 禁用时间
        /// </summary>
        [Display(Name = "禁用时间")]
        public Nullable<DateTime> DISABLE_DATE { get; set; }
        /// <summary>
        /// 禁用原因
        /// </summary>
        [StringLength(50)]
        [Display(Name = "禁用原因")]
        public string DISABLE_REASON { get; set; }
        /// <summary>
        /// 服务标识
        /// </summary>
        [StringLength(50)]
        [Display(Name = "服务标识")]
        public string SERVICE_FLAG { get; set; }
        /// <summary>
        /// REGION
        /// </summary>
        [StringLength(10)]
        [Display(Name = "REGION")]
        public string REGION { get; set; }

        /// <summary>
        /// 脚本组ID
        /// </summary>
        [Required]
        [Display(Name = "脚本组ID")]
        public Int64? GROUP_ID { get; set; }
    }
}
