using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterface.Models
{
    public class YlInsure:YL_INSURER
    {
        public YlInsure()
        {
            AllProductPrice = new List<YlInsureProduct>();
        }
        public IList<YlInsureProduct> AllProductPrice { get; set; }
    }
}
