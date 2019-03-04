using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface.Models.Api
{
    public class ApiLogingBean
    {
        public string userName { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string loginName { get; set; }
        public string password { get; set; }
        public string imei { get; set; }
        public string version { get; set; }
        public string code { get; set; }
        public string openid { get; set; }
        public string userId { get; set; }
        public string pollCode { get; set; }
        public int type { get; set; }
    }
}
