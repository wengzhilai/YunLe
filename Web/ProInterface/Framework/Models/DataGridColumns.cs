using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    /// <summary>
    /// 用于jeasyui 的DataGrid的Columns进行绑定
    /// </summary>
    public class DataGridColumns
    {
        public string field { get; set; }
        public string title { get; set; }
        public int width { get; set; }
        public bool sortable { get; set; }
        public DataGridColumnsEditor editor { get; set; }
    }

    public class DataGridColumnsEditor
    {
        public string type { get; set; }
    }
    public enum DataGridColumnsEditorType
    {
        text, textarea, checkbox, numberbox, validatebox, datebox, combobox, combotree
    }

}
