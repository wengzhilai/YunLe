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
    
    public partial class YL_USER_ADDRESS
    {
        public int ID { get; set; }
        public Nullable<int> USER_ID { get; set; }
        public string NAME { get; set; }
        public string PHONE { get; set; }
        public string CITY { get; set; }
        public string COUNTY { get; set; }
        public string ADDRESS { get; set; }
        public string LANG { get; set; }
        public string LAT { get; set; }
        public Nullable<short> IS_DEFAULT { get; set; }
    
        public virtual YL_USER YL_USER { get; set; }
    }
}