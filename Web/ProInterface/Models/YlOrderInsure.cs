using ProInterface.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterface.Models
{
    public class YlOrderInsure:YL_ORDER_INSURE
    {
        public YlOrderInsure()
        {
            SaveProductId = new List<YlOrderInsureProduct>();
            AllInsurePrice = new List<YlInsure>();
            
        }

        public YlInsure Insure { get; set; }
        public IList<YlOrderInsureProduct> SaveProductId { get; set; }
        public IList<YlInsure> AllInsurePrice { get; set; }

        /// <summary>
        /// 被保人
        /// </summary>
        public string recognizeePicUrl { get; set; }
        public string recognizeePicUrl1 { get; set; }

        public string SaveProductIdStr { get; set; }
    }
}
