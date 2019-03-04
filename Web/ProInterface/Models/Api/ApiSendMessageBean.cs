using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface.Models.Api
{
    public class ApiSendMessageBean : USER_MESSAGE
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }
    }
}
