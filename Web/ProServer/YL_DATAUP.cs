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
    
    public partial class YL_DATAUP
    {
        public YL_DATAUP()
        {
            this.YL_DATAUP_FILED = new HashSet<YL_DATAUP_FILED>();
        }
    
        public int ID { get; set; }
        public string NAME { get; set; }
        public string REMARK { get; set; }
    
        public virtual ICollection<YL_DATAUP_FILED> YL_DATAUP_FILED { get; set; }
    }
}
