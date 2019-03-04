using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterface.Models.Api
{
    public class ApiCostListBean:YL_COSTLIST
    {
        public string CaseTypeName { get; set; }
        public string CreateUserName { get; set; }
    }
}
