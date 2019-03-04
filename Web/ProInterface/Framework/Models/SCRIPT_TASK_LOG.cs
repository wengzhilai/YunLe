
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 任务日志
    /// </summary>
    public class SCRIPT_TASK_LOG
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public Int64 ID { get; set; }
        /// <summary>
        /// 口径任务ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "口径任务ID")]
        public Int64 SCRIPT_TASK_ID { get; set; }
        /// <summary>
        /// 记录时间
        /// </summary>
        [Required]
        [Display(Name = "记录时间")]
        public DateTime LOG_TIME { get; set; }
        /// <summary>
        /// 日志级别
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "日志级别")]
        public decimal LOG_TYPE { get; set; }
        /// <summary>
        /// 日志说明
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "日志说明")]
        public string MESSAGE { get; set; }
        /// <summary>
        /// SQL内容
        /// </summary>
        [Display(Name = "SQL内容")]
        public string SQL_TEXT { get; set; }


    }
}
