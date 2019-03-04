
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 步骤
    /// </summary>
    public class FLOW_FLOWNODE_FLOW
    {

            /// <summary>
            /// ID
            /// </summary>
            [Required]
            [Display(Name = "ID")]
            public int ID { get; set; }
            /// <summary>
            /// FLOW_ID
            /// </summary>
            [Display(Name = "FLOW_ID")]
            public int FLOW_ID { get; set; }
            /// <summary>
            /// FROM_FLOWNODE_ID
            /// </summary>
            [Display(Name = "FROM_FLOWNODE_ID")]
            public int FROM_FLOWNODE_ID { get; set; }
            /// <summary>
            /// TO_FLOWNODE_ID
            /// </summary>
            [Display(Name = "TO_FLOWNODE_ID")]
            public int TO_FLOWNODE_ID { get; set; }
            /// <summary>
            /// 处理方式
            /// </summary>
            [Required]
            [Display(Name = "处理方式")]
            public short HANDLE { get; set; }

            /// <summary>
            /// 选择人
            /// </summary>
            [Required]
            [Display(Name = "选择人")]
            public short ASSIGNER { get; set; }

            /// <summary>
            /// 状态
            /// </summary>
            [StringLength(20)]
            [Display(Name = "状态")]
            public string STATUS { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            [StringLength(20)]
            [Display(Name = "备注")]
            public string REMARK { get; set; }


            /// <summary>
            /// 处理时长(小时)
            /// </summary>
            [Display(Name = "处理时长(小时)")]
            public int? EXPIRE_HOUR { get; set; }
    }
}
