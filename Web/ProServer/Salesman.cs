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
    public partial class Service : ISalesman
    {
        public bool SalesmanDelete(string loginKey, ref ErrorInfo err, int keyId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_SALESMAN.SingleOrDefault(a => a.ID == keyId);

                    foreach (var t in ent.YL_CLIENT.ToList())
                    {
                        t.YL_SALESMAN = null;
                    }

                    if (ent.YL_USER.YL_GARAGE != null)
                    {
                        ent.YL_USER.YL_GARAGE.Clear();
                    }

                    db.YL_SALESMAN.Remove(ent);
                   
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

        public bool SalesmanSave(string loginKey, ref ErrorInfo err, YlSalesman inEnt, IList<string> allPar)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_SALESMAN.SingleOrDefault(a => a.ID == inEnt.ID);
                    if (ent == null)
                    {

                        //增加业务员组织
                        var dis = DistrictAdd(null, ref err, new DISTRICT
                        {
                            NAME = inEnt.NAME,
                            PARENT_ID = inEnt.DISTRICT_ID
                        });

                        var user = new TUser
                        {
                            DISTRICT_ID = dis.ID,
                            PHONE_NO = inEnt.LOGIN_NAME,
                            RoleAllID = "3",
                            LOGIN_NAME = inEnt.LOGIN_NAME,
                            IS_LOCKED = 0,
                            NAME = inEnt.NAME
                        };
                        user = UserGetAndSave(inEnt.authToken, ref err, user, null);

                        if (err.IsError)
                        {
                            return false;
                        }

                        ent = Fun.ClassToCopy<ProInterface.Models.YlSalesman, YL_SALESMAN>(inEnt);
                        ent.ID = user.ID;
                        ent.REQUEST_CODE = "A" + ent.ID;
                        ent.STATUS_TIME = DateTime.Now;
                        ent.TEAM_ID = inEnt.DISTRICT_ID;
                        ent.STATUS = "注册";
                        db.YL_SALESMAN.Add(ent);
                    }
                    else
                    {
                        //团队和推荐码一样

                        if (err.IsError) return false;
                        ent = Fun.ClassToCopy<ProInterface.Models.YlSalesman, YL_SALESMAN>(inEnt, ent, allPar);
                        ent.YL_USER = Fun.ClassToCopy<ProInterface.Models.YlSalesman, YL_USER>(inEnt, ent.YL_USER, allPar);
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

        public YlSalesman SalesmanSingleId(string loginKey, ref ErrorInfo err, int keyId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_SALESMAN.SingleOrDefault(x => x.ID == keyId);
                
                var reEnt = new ProInterface.Models.YlSalesman();
                if (ent != null)
                {
                    if (string.IsNullOrEmpty(ent.REQUEST_CODE))
                    {
                        ent.REQUEST_CODE = "A" + ent.ID;
                        db.SaveChanges();
                    }
                    reEnt = Fun.ClassToCopy<YL_SALESMAN, ProInterface.Models.YlSalesman>(ent);
                    reEnt = Fun.ClassToCopy<YL_USER, ProInterface.Models.YlSalesman>(ent.YL_USER, reEnt);
                    if (ent.ID_NO_PIC != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.ID_NO_PIC);
                        if (image != null) reEnt.idNoUrl = image.URL;
                    }

                    if (ent.YL_USER.ICON_FILES_ID != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.YL_USER.ICON_FILES_ID);
                        if (image != null) reEnt.iconURL = image.URL;
                    }
                    var allGarageList = ent.YL_USER.YL_GARAGE.ToList();
                    if(allGarageList.Count()>0)
                    {
                        reEnt.AllGarageIdStr = string.Join(",", ent.YL_USER.YL_GARAGE.Select(x => x.ID).ToList());
                        reEnt.garage = Fun.ClassToCopy<YL_GARAGE, YlGarage>(allGarageList[0]);
                    }
                }

                return reEnt;
            }
        }
    }
}
