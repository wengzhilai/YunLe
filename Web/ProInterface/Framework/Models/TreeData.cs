using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    /// <summary>
    /// 用于绑定easyui-combotree
    /// </summary>
    public class TreeData
    {
        public TreeData()
        {
            children = new List<TreeData>();
        }
        /// <summary>
        /// 主键
        /// </summary>
        public int? id { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string iconCls { get; set; }

        /// <summary>
        /// 子项
        /// </summary>
        public IList<TreeData> children { get; set; }

    }
}
