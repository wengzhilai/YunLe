using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LINQExtensions;
using System.Linq.Expressions;
using System.IO;
using ProInterface;
using ProInterface.Models;


namespace ProServer
{
    public partial class Service : IFiles
    {
        public FILES FilesAdd(string loginKey, ref ErrorInfo err, FILES inEnt)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            using (DBEntities db = new DBEntities())
            {
                inEnt.ID = Fun.GetSeqID<YL_FILES>();
                inEnt.USER_ID = gu.UserId;
                YL_FILES reEnt = Fun.ClassToCopy<ProInterface.Models.FILES, YL_FILES>(inEnt);
                reEnt = db.YL_FILES.Add(reEnt);
                try
                {
                    db.SaveChanges();
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Add);
                    inEnt.ID = reEnt.ID;
                    inEnt.UPLOAD_TIME = null;
                    return inEnt;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = e.Message;
                    err.Excep = e;
                    return null;
                }
            }
        }


        public bool FilesDelete(string loginKey, ref ErrorInfo err, int id)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_FILES.SingleOrDefault(a => a.ID == id);
                    db.YL_FILES.Remove(ent);
                    db.SaveChanges();
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Delete);
                    return true;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = e.Message;
                    err.Excep = e;
                    return false;
                }
            }
        }


        public FILES FilesSingle(string loginKey, ref ErrorInfo err, int id)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                YL_FILES ent = db.YL_FILES.SingleOrDefault(x => x.ID == id);
                FILES reEnt = Fun.ClassToCopy<YL_FILES, FILES>(ent);
                return reEnt;
            }
        }
    }
}
