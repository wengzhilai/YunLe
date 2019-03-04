using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    /// <summary>
    /// 只包含Key和Value
    /// </summary>
    public class IdText
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// 打开目标的对象
        /// </summary>
        public string target { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool selected { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public IList<IdTextAttr> attributes { get; set; }
    }
    public class IdTextAttr
    {
        public string url { get; set; }
    }
}
