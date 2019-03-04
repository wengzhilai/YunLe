using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    public class TTaskFlowHandle : TASK_FLOW_HANDLE
    {
        public TTaskFlowHandle()
        {
            AllFiles = new List<FILES>();
        }

        public IList<FILES> AllFiles { get; set; }
    }
}
