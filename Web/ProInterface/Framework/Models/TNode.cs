using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ProInterface.Models
{
    public class TNode
    {

        public TNode()
        {
            AllRole = new List<SelectListItem>();
            AllStatus = new List<SelectListItem>();
            AllFlowNode = new List<SelectListItem>();
        }

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        public int TaskFlowId { get; set; }


        public int FlowID { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        [Required]
        [Display(Name = "任务名称")]
        public string TaskName { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        public string Remark { get; set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        [Required]
        [Display(Name = "当前状态")]
        public string NowStatus { get; set; }

        /// <summary>
        /// 本节点所有状态
        /// </summary>
        [Display(Name = "所有状态")]
        public IList<SelectListItem> AllStatus { get; set; }

        /// <summary>
        /// 所有角色
        /// </summary>
        [Display(Name = "所有角色")]
        public IList<SelectListItem> AllRole { get; set; }



        /// <summary>
        /// FlowNodeName
        /// </summary>
        [Display(Name = "当前节点名")]
        public string FlowNodeName { get; set; }

        /// <summary>
        /// 主流程所有节点
        /// </summary>
        [Display(Name = "所有节点")]
        public IList<SelectListItem> AllFlowNode { get; set; }

        /// <summary>
        /// 当前选中节点
        /// </summary>
        [Display(Name = "当前选中节点")]
        public int ToFlowNodeID { get; set; }


        /// <summary>
        /// 是否指定人
        /// </summary>
        [Display(Name = "是否指定人")]
        public int Assigner { get; set; }

        [Display(Name = "流转明细")]
        public IList<TTaskFlow> AllFlow { get; set; }

        /// <summary>
        /// 只用于显示
        /// </summary>
        public TTaskFlow AllFlowEnt { get; set; }

        /// <summary>
        /// 处理人
        /// </summary>
        [Required]
        [Display(Name = "处理人")]
        public string UserIdArrStr { get; set; }

        /// <summary>
        /// 任务附件
        /// </summary>
        [Display(Name = "任务附件")]
        public string AllFilesStr { get; set; }

        /// <summary>
        /// 任务附件ID
        /// </summary>
        [Display(Name = "任务附件ID")]
        public string AllFilesIdStr { get; set; }

        /// <summary>
        /// 归属
        /// </summary>
        [Display(Name = "归属")]
        public string DistrictName { get; set; }

        /// <summary>
        /// 任务内容
        /// </summary>
        [Display(Name = "任务内容")]
        public string TaskContent { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [Display(Name = "角色ID")]
        public string RoleIdStr { get; set; }
        /// <summary>
        /// 当前URL
        /// </summary>
        public string ShowUrl { get; set; }

        /// <summary>
        /// 外键
        /// </summary>
        public string TaskKey { get; set; }

        /// <summary>
        /// 是否阶段回复
        /// </summary>
        public int IsStage { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        public Nullable<DateTime> StartTime { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        [Display(Name = "过期时间")]
        public Nullable<DateTime> EndTime { get; set; }

    }
}
