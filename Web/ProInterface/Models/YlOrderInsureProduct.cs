using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterface.Models
{
    public class YlOrderInsureProduct:YL_ORDER_INSURE_PRODUCT
    {
        /// <summary>
        /// 保险公司名称
        /// </summary>
        public string InsurerName { get; set; }

        /// <summary>
        /// 保险产品名称
        /// </summary>
        public string InsurerProductName { get; set; }

        /// <summary>
        /// 保险产品名称
        /// </summary>
        public bool isCheck { get; set; }
    }
}
