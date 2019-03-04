using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterface.Models
{
    public class YlInsureProduct:YL_INSURER_PRODUCT
    {
        public YlInsureProduct()
        {
            ChildItem = new List<YlInsureProduct>();
        }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 最大赔率
        /// </summary>
        public string maxPay { get; set; }
        /// <summary>
        /// 是否选择
        /// </summary>
        public bool isCheck { get; set; }

        /// <summary>
        /// 保险公司名称
        /// </summary>
        public string InsureName { get; set; }

        public IList<YlInsureProduct> ChildItem { get; set; }
}
}
