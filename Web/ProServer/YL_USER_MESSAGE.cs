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
    
    public partial class YL_USER_MESSAGE
    {
        public int MESSAGE_ID { get; set; }
        public int USER_ID { get; set; }
        public string PHONE_NO { get; set; }
        public string STATUS { get; set; }
        public System.DateTime STATUS_TIME { get; set; }
        public string REPLY { get; set; }
        public string PUSH_TYPE { get; set; }
    
        public virtual YL_MESSAGE YL_MESSAGE { get; set; }
        public virtual YL_USER YL_USER { get; set; }
    }
}