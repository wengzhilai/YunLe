//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProServer
{
    using System;
    using System.Collections.Generic;
    
    public partial class YL_QUERY
    {
        public YL_QUERY()
        {
            this.YL_ROLE_QUERY_AUTHORITY = new HashSet<YL_ROLE_QUERY_AUTHORITY>();
        }
    
        public int ID { get; set; }
        public string NAME { get; set; }
        public string CODE { get; set; }
        public short AUTO_LOAD { get; set; }
        public int PAGE_SIZE { get; set; }
        public short SHOW_CHECKBOX { get; set; }
        public short IS_DEBUG { get; set; }
        public Nullable<short> FILTR_LEVEL { get; set; }
        public Nullable<int> DB_SERVER_ID { get; set; }
        public string QUERY_CONF { get; set; }
        public string QUERY_CFG_JSON { get; set; }
        public string IN_PARA_JSON { get; set; }
        public string JS_STR { get; set; }
        public string ROWS_BTN { get; set; }
        public string HEARD_BTN { get; set; }
        public string REPORT_SCRIPT { get; set; }
        public string CHARTS_CFG { get; set; }
        public string CHARTS_TYPE { get; set; }
        public string FILTR_STR { get; set; }
        public string REMARK { get; set; }
        public string NEW_DATA { get; set; }
    
        public virtual ICollection<YL_ROLE_QUERY_AUTHORITY> YL_ROLE_QUERY_AUTHORITY { get; set; }
    }
}
