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
    
    public partial class YL_BULLETIN_LOG
    {
        public int ID { get; set; }
        public int BULLETIN_ID { get; set; }
        public int USER_ID { get; set; }
        public System.DateTime LOOK_TIME { get; set; }
    
        public virtual YL_BULLETIN YL_BULLETIN { get; set; }
    }
}