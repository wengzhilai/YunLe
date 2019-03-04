using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterface.Models
{
    public class YlSalesman:YL_SALESMAN
    {
        public YlSalesman()
        {
            //allOrder = new List<YlOrder>();
            //expireInsureList = new List<YlOrder>();
            //allClient = new List<YlClient>();
        }
        //所有定单数
        public int orderNum { get; set; }
        public string qrcode { get; set; }
        //所有完成的定单数
        public int orderNumSucc { get; set; }
        //所有客户数
        public int clientNum { get; set; }
        //public IList<YlOrder> allOrder { get; set; }
        /// <summary>
        /// 还有3个月过期的保单
        /// </summary>
        //public IList<YlOrder> expireInsureList { get; set; }
        //public IList<YlClient> allClient { get; set; }
        public int allOrderNum { get; set; }
        //所有保单数
        public int expireInsureNum { get; set; }

        public string authToken { get; set; }
        public string iconURL { get; set; }

        public string distictName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string idNoUrl { get; set; }
        public string allCost { get; set; }

        /// <summary>
        /// 归属维修站
        /// </summary>
        public string AllGarageIdStr { get; set; }
        /// <summary>
        /// 归属维修站
        /// </summary>
        public YlGarage garage { get; set; }
    }
}
