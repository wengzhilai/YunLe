using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ProInterface.Models
{
    public class TFlow:FLOW
    {
        public TFlow()
        {
            FlowList = new List<TFlowFlownodeFlow>();
            Idxy = new List<IdXY>();
            AllFlownode = new List<SelectListItem>();
        }
        public IList<TFlowFlownodeFlow> FlowList { get; set; }
        public IList<SelectListItem> AllFlownode { get; set; }
        public IList<IdXY> Idxy { get; set; }
        /// <summary>
        /// 流程节点JSON
        /// </summary>
        public string FlowListStr { get; set; }


    }
}
