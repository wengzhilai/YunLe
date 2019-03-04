using ProInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProServer
{
    public partial class Service
    {
        public bool UserCheckFunctioAuthority(string loginKey, ref ErrorInfo err, MethodBase methodBase)
        {

            //GlobalUser gu = Global.GetUser(loginKey);
            //if (gu == null)
            //{
            //    err.IsError = true;
            //    err.Message = "登录超时，请重新登录";
            //    return false;
            //}
            return true;
        }
        public void UserWriteLog(string loginKey, MethodBase methodBase, StatusType.UserLogType userLogType)
        {
            
        }


        /// <summary>
        /// 分析运行时间
        /// </summary>
        /// <param name="runDataStr"></param>
        /// <returns></returns>
        public DateTime AnalysisRunDate(string runDataStr)
        {
            var reDate = DateTime.Now;
            if (string.IsNullOrEmpty(runDataStr)) return reDate;
            runDataStr = runDataStr.ToLower();
            try
            {
                switch (runDataStr.Substring(runDataStr.Length - 1))
                {
                    case "m":
                        reDate = reDate.AddMonths(Convert.ToInt32(runDataStr.Substring(0, runDataStr.Length - 1)));
                        break;
                    case "d":
                        reDate = reDate.AddDays(Convert.ToInt32(runDataStr.Substring(0, runDataStr.Length - 1)));
                        break;
                    default:
                        if (runDataStr.Length == 6)
                        {
                            runDataStr = runDataStr + "01";
                        }
                        else
                        {
                            if (runDataStr.Length < 6)
                            {
                                return reDate;
                            }
                        }
                        reDate = Convert.ToDateTime(runDataStr.Substring(0, 4) + "-" + runDataStr.Substring(4, 2) + "-" + runDataStr.Substring(6, 2));
                        break;
                }
                return reDate;
            }
            catch
            {
                return reDate;
            }
        }

    }
}
