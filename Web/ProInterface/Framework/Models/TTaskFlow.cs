using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ProInterface.Models 
{
    public class TTaskFlow : TASK_FLOW
    {
        public TTaskFlow()
        {
            AllHandle = new List<TTaskFlowHandle>();
            RoleList = new List<KTV>();
            children = new List<TTaskFlow>();
            AllHandleFiles = new List<FILES>();
        }

        public int FlowId { get; set; }
        public IList<KTV> RoleList { get; set; }
        public string TaskName { get; set; }
        public string TaskRemark { get; set; }
        public int SendUserId { get; set; }
        public string SendUserName { get; set; }
        public string DealUserName { get; set; }
        public string DealUserDistrictName { get; set; }
        public string DealUserPhone { get; set; }
        public IList<TTaskFlowHandle> AllHandle { get; set; }
        public string AllHandleContent { get; set; }
        public IList<FILES> AllHandleFiles { get; set; }
        public string DealRole { get; set; }
        public string NextDealUserName { get; set; }
        public IList<TTaskFlow> children { get; set; }
        
        
    }
}
