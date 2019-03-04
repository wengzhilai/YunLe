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
    public partial class Service : ITeam
    {
        public bool TeamSave(string loginKey, ref ErrorInfo err, ProInterface.Models.YL_TEAM inEnt, IList<string> allPar)
        {
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return false;
            }
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_TEAM.SingleOrDefault(a => a.ID == inEnt.ID);
                    bool isAdd = false;
                    if (ent == null)
                    {
                        isAdd = true;

                        
                        var dis=DistrictAdd(loginKey, ref err, new DISTRICT {
                            NAME = inEnt.NAME,
                            PARENT_ID = inEnt.DISTRICT_ID,
                            IN_USE=1
                        });
                        if (err.IsError)
                        {
                            return false;
                        }


                        ent = Fun.ClassToCopy<ProInterface.Models.YL_TEAM, YL_TEAM>(inEnt);
                        ent.ID = dis.ID;
                        ent.REQUEST_CODE = "0000" + ent.ID;
                        ent.REQUEST_CODE = "YL" + ent.REQUEST_CODE.Substring(ent.REQUEST_CODE.Length - 4);
                        ent.CREATE_TIME = DateTime.Now;
                        ent.ADD_USER_ID = gu.UserId;

                        err.Message = ent.ID.ToString();
                    }
                    else
                    {
                        DistrictEdit(loginKey, ref err, new DISTRICT
                        {
                            ID=inEnt.ID,
                            NAME = inEnt.NAME,
                            PARENT_ID = inEnt.DISTRICT_ID
                        },allPar);

                        if (err.IsError) return false;

                        ent = Fun.ClassToCopy<ProInterface.Models.YL_TEAM, YL_TEAM>(inEnt, ent, allPar);
                        err.Message = ent.ID.ToString();
                    }

                    if (isAdd)
                    {
                        db.YL_TEAM.Add(ent);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="keyId"></param>
        /// <returns></returns>
        public bool TeamDelete(string loginKey, ref ErrorInfo err, int keyId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_TEAM.SingleOrDefault(a => a.ID == keyId);
                    db.YL_TEAM.Remove(ent);
                    db.SaveChanges();
                    District_Delete(loginKey, ref err, keyId);
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
