using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterface.Models
{
    public class YlTeam:YL_TEAM
    {
        public YlTeam()
        {
            allOrder = new List<YlOrder>();
            allSalesman = new List<YlSalesman>();
        }
        public int orderNum { get; set; }
        public string qrcode { get; set; }
        public int orderNumSucc { get; set; }
        public IList<YlOrder> allOrder { get; set; }
        public IList<YlSalesman> allSalesman { get; set; }
    }
}
