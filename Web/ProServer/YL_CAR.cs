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
    
    public partial class YL_CAR
    {
        public YL_CAR()
        {
            this.YL_ORDER = new HashSet<YL_ORDER>();
            this.YL_CLIENT = new HashSet<YL_CLIENT>();
        }
    
        public int ID { get; set; }
        public string PLATE_NUMBER { get; set; }
        public string CAR_TYPE { get; set; }
        public string BRAND { get; set; }
        public string MODEL { get; set; }
        public Nullable<decimal> PRICE { get; set; }
        public string FRAME_NUMBER { get; set; }
        public string ENGINE_NUMBER { get; set; }
        public Nullable<System.DateTime> TRANSFER_DATA { get; set; }
        public Nullable<System.DateTime> REG_DATA { get; set; }
        public Nullable<int> DRIVING_PIC_ID { get; set; }
        public Nullable<int> IS_DEFAULT { get; set; }
        public Nullable<int> ID_NO_PIC_ID { get; set; }
        public Nullable<int> ID_NO_PIC_ID1 { get; set; }
        public Nullable<int> DRIVING_PIC_ID1 { get; set; }
        public Nullable<int> BILL_PIC_ID { get; set; }
        public Nullable<int> CERTIFICATE_PIC_ID { get; set; }
    
        public virtual ICollection<YL_ORDER> YL_ORDER { get; set; }
        public virtual ICollection<YL_CLIENT> YL_CLIENT { get; set; }
    }
}
