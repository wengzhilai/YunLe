using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    public class TFlowFlownodeFlow:FLOW_FLOWNODE_FLOW
    {
        public TFlowFlownodeFlow()
        {
            AllRoleIDList = new List<int>();
        }
        public IList<int> AllRoleIDList { get; set; }

        public string AllRoleStr { get; set; }

        /// <summary>
        /// 处理地址
        /// </summary>
        public string HandleUrl { get; set; }
        /// <summary>
        /// 展示地址
        /// </summary>
        public string ShowUrl { get; set; }

        /// <summary>
        /// 开始ID
        /// </summary>
        public int StartId { get; set; }

    }
}
