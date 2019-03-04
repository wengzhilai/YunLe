using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProServer
{
    public class DbFun
    {
        public static string WhereData(string dbType,string filed,string opType,string value)
        {
            StringBuilder reStr=new StringBuilder();
            switch (dbType)
            {
                case "DB2":
                    reStr.AppendFormat(" {0} {1} TO_DATE('{2}','YYYY-MM-DD') ", filed, opType, value);
                    break;
                case "Oracle":
                    reStr.AppendFormat(" {0} {1} TO_DATE('{2}','YYYY-MM-DD') ", filed, opType, value);
                    break;
                case "Sql":
                    reStr.AppendFormat(" {0} {1} '{2}'", filed, opType, value);
                    break;
                default:
                    reStr.AppendFormat(" {0} {1} '{2}'", filed, opType, value);
                    break;
            }
            return reStr.ToString();
        }
    }
}
