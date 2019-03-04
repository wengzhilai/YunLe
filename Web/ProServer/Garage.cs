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
    public partial class Service : IGarage
    {
        public bool GarageSave(string loginKey, ref ErrorInfo err, YlGarage inEnt, IList<string> allPar)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_GARAGE.SingleOrDefault(a => a.ID == inEnt.ID);
                    bool isAdd = false;
                    if (ent == null)
                    {
                        isAdd = true;
                        ent = Fun.ClassToCopy<ProInterface.Models.YL_GARAGE, YL_GARAGE>(inEnt);
                    }
                    else
                    {
                        ent = Fun.ClassToCopy<ProInterface.Models.YL_GARAGE, YL_GARAGE>(inEnt, ent, allPar);
                    }

                    if (isAdd)
                    {
                        db.YL_GARAGE.Add(ent);
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

        public YlGarage GarageSingleId(string loginKey, ref ErrorInfo err, int keyId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_GARAGE.SingleOrDefault(x => x.ID == keyId);
                var reEnt = new ProInterface.Models.YlGarage();
                if (ent != null)
                {
                    reEnt = Fun.ClassToCopy<YL_GARAGE, ProInterface.Models.YlGarage>(ent);
                }
                if (ent.PIC_ID != null)
                {
                    reEnt.picIdUrl = db.YL_FILES.Single(x => x.ID == ent.PIC_ID).URL;
                }
                return reEnt;
            }
        }

        public YlSalesman GarageUserSingleId(string loginKey, ref ErrorInfo err, int keyId)
        {

            YlSalesman garageUser = SalesmanSingleId(loginKey,ref err, keyId);
            using (DBEntities db = new DBEntities())
            {
                var user = db.YL_USER.SingleOrDefault(x => x.ID == keyId);

                garageUser.AllGarageIdStr =string.Join(",",user.YL_GARAGE.Select(x => x.ID).ToList());
            }
            return garageUser;
        }

        public YlSalesman GarageUserSave(string loginKey, ref ErrorInfo err, YlSalesman inEnt, IList<string> allPar)
        {
            if (string.IsNullOrEmpty(inEnt.AllGarageIdStr))
            {
                err.IsError = true;
                err.Message = "维修站不能为空";
                return null;
            }

            inEnt.RoleAllID = "22,3";
            allPar.Add("RoleAllID");
            YlSalesman garageUser = new YlSalesman();
            using (DBEntities db = new DBEntities())
            {
                var user = db.YL_USER.SingleOrDefault(x => x.LOGIN_NAME == inEnt.LOGIN_NAME);
                if (user != null)
                {
                    inEnt.RoleAllID = inEnt.RoleAllID + "," + string.Join(",", user.YL_ROLE.Select(x => x.ID).ToList());
                }
                garageUser = Fun.ClassToCopy<TUser, YlSalesman>(UserSave(loginKey, ref err, inEnt, allPar));
                if (err.IsError)
                {
                    return null;
                }
                user = db.YL_USER.SingleOrDefault(x => x.ID == garageUser.ID);
                var allGarageIdList = inEnt.AllGarageIdStr.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                var allGarage = db.YL_GARAGE.Where(x => allGarageIdList.Contains(x.ID)).ToList();
                user.YL_GARAGE.Clear();
                user.YL_GARAGE = allGarage;

                if (user.YL_SALESMAN == null)
                {
                    var addSalesman = Fun.ClassToCopy<YlSalesman, YL_SALESMAN>(inEnt);
                    addSalesman.ID = user.ID;
                    addSalesman.STATUS_TIME = DateTime.Now;
                    db.YL_SALESMAN.Add(addSalesman);
                }
                else
                {
                    user.YL_SALESMAN= Fun.ClassToCopy<YlSalesman, YL_SALESMAN>(inEnt, user.YL_SALESMAN, allPar);
                }

                db.SaveChanges();
            }
            return garageUser;
        }

        public IList<YlGarage> GarageFindAllMy(string loginKey, ref ErrorInfo err)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                IList< YL_GARAGE> allList = new List<YL_GARAGE>();
                var gu = Global.GetUser(loginKey);
                if (gu != null && gu.RoleID.Contains(22))
                {
                    var user = db.YL_USER.SingleOrDefault(x => x.ID == gu.UserId);
                    allList = user.YL_GARAGE.ToList();
                }
                else
                {
                    allList = db.YL_GARAGE.ToList();
                }
                return Fun.ClassListToCopy<YL_GARAGE, ProInterface.Models.YlGarage>(allList);
            }
        }

        public bool GarageUserDelete(string loginKey, ref ErrorInfo err, int keyId)
        {
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_USER.SingleOrDefault(a => a.ID == keyId);
                    ent.YL_GARAGE.Clear();
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
    }
}
