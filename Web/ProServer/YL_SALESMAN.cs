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
    
    public partial class YL_SALESMAN
    {
        public YL_SALESMAN()
        {
            this.YL_CLIENT = new HashSet<YL_CLIENT>();
        }
    
        public int ID { get; set; }
        public Nullable<int> TEAM_ID { get; set; }
        public string REQUEST_CODE { get; set; }
        public Nullable<int> PARENT_ID { get; set; }
        public string IMEI { get; set; }
        public string SEX { get; set; }
        public string ID_NO { get; set; }
        public Nullable<int> ID_NO_PIC { get; set; }
        public string STATUS { get; set; }
        public System.DateTime STATUS_TIME { get; set; }
    
        public virtual ICollection<YL_CLIENT> YL_CLIENT { get; set; }
        public virtual YL_TEAM YL_TEAM { get; set; }
        public virtual YL_USER YL_USER { get; set; }
    }
}