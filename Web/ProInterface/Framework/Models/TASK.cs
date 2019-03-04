
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 任务
    /// </summary>
    public class TASK
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "任务类型")]
        [Description(@"
任务工单：只能是上级对平级或下级发起
其它类型的工单，由系统指定流程，完成特定的任务
")]
        public int? FLOW_ID { get; set; }
        /// <summary>
        /// 工单编号
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "工单编号")]
        public string TASK_NAME { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public Nullable<DateTime> CREATE_TIME { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "创建人")]
        public Nullable<int> CREATE_USER { get; set; }
        /// <summary>
        /// 创建姓名
        /// </summary>
        [StringLength(50)]
        [Display(Name = "创建姓名")]
        public string CREATE_USER_NAME { get; set; }
        /// <summary>
        /// 工单状态
        /// </summary>
        [StringLength(50)]
        [Display(Name = "工单状态")]
        public string STATUS { get; set; }
        /// <summary>
        /// 状态时间
        /// </summary>
        [Display(Name = "状态时间")]
        public Nullable<DateTime> STATUS_TIME { get; set; }
        /// <summary>
        /// 内容说明
        /// </summary>
        [Required]
        [Display(Name = "内容说明")]
        public string REMARK { get; set; }
        /// <summary>
        /// REGION
        /// </summary>
        [StringLength(10)]
        [Display(Name = "REGION")]
        public string REGION { get; set; }

        /// <summary>
        /// 关联
        /// </summary>
        [StringLength(10)]
        [Display(Name = "关联")]
        public string KEY { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        public Nullable<DateTime> START_TIME { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        [Display(Name = "过期时间")]
        public Nullable<DateTime> END_TIME { get; set; }

        /// <summary>
        /// 可选择角色
        /// </summary>
        [StringLength(200)]
        [Display(Name = "可选择角色")]
        public string ROLE_ID_STR { get; set; }
    }
}
