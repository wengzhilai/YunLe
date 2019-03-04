using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterface.Models.Api
{
    public class ApiWeixinPayBean
    {
        public string appId { get; set; }
        public string partnerId { get; set; }
        public string packageValue { get; set; }
        public string nonceStr { get; set; }
        public Int64 timeStamp { get; set; }
        public string prepayId { get; set; }
        public string signType { get; set; }
        public string sign { get; set; }
    }
}
