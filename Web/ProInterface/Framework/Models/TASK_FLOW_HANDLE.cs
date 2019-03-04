
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 流转回复明细
    /// </summary>
    public class TASK_FLOW_HANDLE
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// TASK_FLOW_ID
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "TASK_FLOW_ID")]
        public Nullable<int> TASK_FLOW_ID { get; set; }
        /// <summary>
        /// 处理人
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "处理人")]
        public Nullable<int> DEAL_USER_ID { get; set; }
        /// <summary>
        /// 处理人姓名
        /// </summary>
        [StringLength(50)]
        [Display(Name = "处理人姓名")]
        public string DEAL_USER_NAME { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        [Display(Name = "处理时间")]
        public Nullable<DateTime> DEAL_TIME { get; set; }

        /// <summary>
        /// 处理说明
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "处理说明")]
        public string CONTENT { get; set; }


    }
}
