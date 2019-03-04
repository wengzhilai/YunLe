
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 口径脚本
    /// </summary>
    public class TScript : SCRIPT
    {
        public TScript()
        {
            ScriptGroupList = new List<SCRIPT_GROUP_LIST>();
        }
        /// <summary>
        /// 任务列表
        /// </summary>
        [Display(Name = "任务列表")]
        public IList<SCRIPT_GROUP_LIST> ScriptGroupList { get; set; }
        /// <summary>
        /// 任务列表
        /// </summary>
        [Display(Name = "任务列表")]
        public string ScriptGroupListJosn { get; set; }
    }
}


