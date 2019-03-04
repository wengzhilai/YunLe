using Microsoft.CSharp;
using ProInterface;
using ProInterface.Models;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ProServer
{
    /// <summary>  
    /// 代码执行类  
    /// </summary>  
    public partial class Service : ICostlist
    {
        public bool CostlistSave(string loginKey, ref ErrorInfo err, ProInterface.Models.YL_COSTLIST inEnt, IList<string> allPar)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    GlobalUser gu = Global.GetUser(loginKey);

                    var ent = db.YL_COSTLIST.SingleOrDefault(a => a.ID == inEnt.ID);
                    bool isAdd = false;
                    if (ent == null)
                    {
                        isAdd = true;
                        ent = Fun.ClassToCopy<ProInterface.Models.YL_COSTLIST, YL_COSTLIST>(inEnt);
                        ent.CREATE_USER_ID = gu.UserId;
                        ent.CREATE_TIME = DateTime.Now;
                    }
                    else
                    {
                        ent = Fun.ClassToCopy<ProInterface.Models.YL_COSTLIST, YL_COSTLIST>(inEnt, ent, allPar);
                    }

                    if (isAdd)
                    {
                        db.YL_COSTLIST.Add(ent);
                    }
                    db.SaveChanges();
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Edit);
                    return true;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return false;
                }
            }
        }

        public bool CostlistSaveByPay(ref ErrorInfo err, ProInterface.Models.YL_COSTLIST inEnt)
        {
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = Fun.ClassToCopy<ProInterface.Models.YL_COSTLIST, YL_COSTLIST>(inEnt);
                    db.YL_COSTLIST.Add(ent);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return false;
                }
            }
        }
    }
}
