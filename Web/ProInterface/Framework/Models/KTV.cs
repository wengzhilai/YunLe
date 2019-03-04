using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    /// <summary>
    /// 主键 类型 值
    /// </summary>
    public class KTV:KV
    {
        public KTV()
        {
            TClass =new KV();
        }
        /// <summary>
        /// 类型
        /// </summary>
        public string T { get; set; }
        public KV TClass { get; set; }
        public IList<KTV> child;

    }
}
