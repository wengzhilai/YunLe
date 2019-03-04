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
    public partial class Service : IClient
    {
        public bool ClientDelete(string loginKey, ref ErrorInfo err, int keyId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_CLIENT.SingleOrDefault(a => a.ID == keyId);
                    foreach (var t in ent.YL_ORDER.ToList())
                    {
                        OrderDelete(loginKey, ref err, t.ID);
                    }
                    ent.YL_CAR.Clear();
                    db.YL_CLIENT.Remove(ent);
                    db.SaveChanges();
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Delete);
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

        public bool ClientSave(string loginKey, ref ErrorInfo err, YlClient inEnt, IList<string> allPar)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_CLIENT.SingleOrDefault(a => a.ID == inEnt.ID);
                    bool isAdd = false;
                    if (ent == null)
                    {
                        err.IsError = true;
                        err.Message = "后台暂不支持添加";
                        return false;
                    }
                    else
                    {
                        ent = Fun.ClassToCopy(inEnt, ent, allPar);
                        var user = UserSave(loginKey, ref err, inEnt, allPar);
                    }

                    if (isAdd)
                    {
                        db.YL_CLIENT.Add(ent);
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

        public YlClient ClientSingleId(string loginKey, ref ErrorInfo err, int keyId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_CLIENT.SingleOrDefault(x => x.ID == keyId);
                var reEnt = new ProInterface.Models.YlClient();
                if (ent != null)
                {
                    reEnt = Fun.ClassToCopy<YL_CLIENT, ProInterface.Models.YlClient>(ent);
                    reEnt = Fun.ClassToCopy(ent.YL_USER, reEnt);

                    if (ent.ID_NO_PIC_ID != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.ID_NO_PIC_ID);
                        if (image != null) reEnt.idNoUrl = image.URL;
                    }
                    if (ent.DRIVER_PIC_ID != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.DRIVER_PIC_ID);
                        if (image != null) reEnt.driverPicUrl = image.URL;
                    }

                    if (ent.YL_USER.ICON_FILES_ID != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.YL_USER.ICON_FILES_ID);
                        if (image != null) reEnt.iconURL = image.URL;
                    }
                }
                return reEnt;
            }
        }
    }
}
