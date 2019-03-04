using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterface.Models.Api
{
    public class ApiInsureBean:YL_INSURER
    {
        public ApiInsureBean()
        {
            AllProductPrice = new List<YlInsureProduct>();
        }
        public IList<YlInsureProduct> AllProductPrice { get; set; }
    }
}
