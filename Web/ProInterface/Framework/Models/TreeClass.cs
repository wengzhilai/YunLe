using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    /// <summary>
    /// 用于绑定快速绑定easyui的 ComboTree数据，要配合
    /// loadFilter: function (rows) {
    ///   return convert(rows);
    /// }
    /// </summary>
    public class TreeClass
    {
        public string id { get; set; }
        public string parentId { get; set; }
        public string name { get; set; }
    }
}
