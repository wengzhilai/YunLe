using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterface.Models
{
    public class YlClient:YL_CLIENT
    {
        public YlClient()
        {
            AllCar = new List<YlCar>();
        }
        public YlCar NowCar { get; set; }
        public IList<YlCar> AllCar { get; set; }
        public string authToken { get; set; }
        public string iconURL { get; set; }
        public string idNoUrl { get; set; }
        public string idNoUrl1 { get; set; }
        public string driverPicUrl { get; set; }
        public string driverPicUrl1 { get; set; }
        public string distictName { get; set; }
        public string allCost { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public YlSalesman SalesmanBean { get; set; }


    }
}
