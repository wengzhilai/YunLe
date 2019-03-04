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
    public partial class Service : IOrderRescue
    {
        
        public bool OrderRescueSave(string loginKey, ref ErrorInfo err, ProInterface.Models.YlOrderRescue inEnt, IList<string> allPar)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_ORDER_RESCUE.SingleOrDefault(a => a.ID == inEnt.ID);
                    bool isAdd = false;
                    if (ent == null)
                    {
                        err.IsError = true;
                        err.Message = "该订单不能在电脑端添加，请在手机上添加";
                        return false;
                    }
                    else
                    {
                        OrderSaveFun(Global.GetUser(loginKey),ref err, db, inEnt, allPar);
                        ent = Fun.ClassToCopy<ProInterface.Models.YL_ORDER_RESCUE, YL_ORDER_RESCUE>(inEnt, ent, allPar);
                    }

                    if (isAdd)
                    {
                        db.YL_ORDER_RESCUE.Add(ent);
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

        public ProInterface.Models.YlOrderRescue OrderRescueSingleId(string loginKey, ref ErrorInfo err, int keyId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_ORDER_RESCUE.SingleOrDefault(x => x.ID == keyId);
                var reEnt = new ProInterface.Models.YlOrderRescue();
                if (ent != null)
                {
                    reEnt = Fun.ClassToCopy<YlOrder, ProInterface.Models.YlOrderRescue>(OrderSingleId(loginKey,ref err, keyId));
                    reEnt = Fun.ClassToCopy<YL_ORDER_RESCUE, ProInterface.Models.YlOrderRescue>(ent, reEnt);
                    reEnt.GarageName = ent.YL_GARAGE.NAME;
                }
                return reEnt;
            }
        }

        public bool OrderRescueDelete(string loginKey, ref ErrorInfo err, int keyId)
        {
            throw new NotImplementedException();
        }

    }
}
