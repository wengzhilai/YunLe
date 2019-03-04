using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    public class TTask:TASK
    {
        public TTask()
        {
            AllFlow = new List<TTaskFlow>();
            AllButton = new List<string>();
        }

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
        /// 归属
        /// </summary>
        [Display(Name = "归属")]
        public string DistrictName { get; set; }

        /// <summary>
        /// 处理内容
        /// </summary>
        [Display(Name = "处理内容")]
        public string TaskContent { get; set; }


        public IList<string> AllButton { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [Display(Name = "联系电话")]
        public string CreatePhone { get; set; }

        public string NowSubmitType { get; set; }
        public int NowFlowId { get; set; }
        public int IsStage { get; set; }
        public int NowNodeFlowId { get; set; }
        [Display(Name = "渠道名称")]
        public string ChanneName { get; set; }
        [Display(Name = "渠道地址")]
        public string ChannelAddress { get; set; }
        [Display(Name = "渠道是否在线")]
        public string ChannelOnLine { get; set; }
    }
}
