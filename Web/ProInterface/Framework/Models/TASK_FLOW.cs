
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 任务流转明细
    /// </summary>
    public class TASK_FLOW:TTask
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// PARENT_ID
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "PARENT_ID")]
        public Nullable<int> PARENT_ID { get; set; }

        /// <summary>
        /// 相同节点
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "EQUAL_ID")]
        public Nullable<int> EQUAL_ID { get; set; }

        /// <summary>
        /// TASK_ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "TASK_ID")]
        public int TASK_ID { get; set; }
        /// <summary>
        /// LEVEL_ID
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "LEVEL_ID")]
        public Nullable<int> LEVEL_ID { get; set; }
        /// <summary>
        /// 当前节点ID
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "当前节点ID")]
        public Nullable<int> FLOWNODE_ID { get; set; }

        /// <summary>
        /// 是否处理
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "是否处理")]
        public int IS_HANDLE { get; set; }
        /// <summary>
        /// 处理状态类型
        /// </summary>
        [StringLength(50)]
        [Display(Name = "处理状态类型")]
        public string DEAL_STATUS { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(100)]
        [Display(Name = "名称")]
        public string NAME { get; set; }
        /// <summary>
        /// 处理地址
        /// </summary>
        [StringLength(200)]
        [Display(Name = "处理地址")]
        public string HANDLE_URL { get; set; }
        /// <summary>
        /// 展示地址
        /// </summary>
        [StringLength(200)]
        [Display(Name = "展示地址")]
        public string SHOW_URL { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        [Display(Name = "过期时间")]
        public Nullable<DateTime> EXPIRE_TIME { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        [Display(Name = "处理时间")]
        public Nullable<DateTime> DEAL_TIME { get; set; }
        

        /// <summary>
        /// 受理时间
        /// </summary>
        [Display(Name = "受理时间")]
        public Nullable<DateTime> ACCEPT_TIME { get; set; }
 
        /// <summary>
        /// 指定人
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "指定人")]
        public Nullable<int> HANDLE_USER_ID { get; set; }
    }
}
