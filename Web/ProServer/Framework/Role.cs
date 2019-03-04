
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Linq;
using System.Reflection;
using System.Text;
using ProInterface.Models;
using ProInterface;
using System.Linq.Expressions;
using LINQExtensions;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Validation;

namespace ProServer
{
    public partial class Service : IRole
    {
        public bool RoleSave(string loginKey, ref ErrorInfo err, TRole inEnt, IList<string> allPar)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_ROLE.SingleOrDefault(a => a.ID == inEnt.ID);

                    bool isAdd = false;
                    if (ent == null)
                    {
                        isAdd = true;
                        ent = Fun.ClassToCopy<ProInterface.Models.ROLE, YL_ROLE>(inEnt);
                        ent.ID = Fun.GetSeqID<YL_ROLE>();
                    }
                    else
                    {
                        ent = Fun.ClassToCopy<ProInterface.Models.ROLE, YL_ROLE>(inEnt, ent, allPar);
                    }

                    ent.YL_MODULE.Clear();
                    IList<int> moduleID = inEnt.ModuleAllStr.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                    ent.YL_MODULE = db.YL_MODULE.Where(x => moduleID.Contains(x.ID)).ToList();
                    inEnt.RoleConfigs = JSON.EncodeToEntity<IList<ROLE_CONFIG>>(inEnt.RoleConfigsStr);
                    foreach (var t in inEnt.RoleConfigs)
                    {
                        var cfg = ent.YL_ROLE_CONFIG.SingleOrDefault(x => x.NAME == t.NAME);
                        if (cfg == null)
                        {
                            ent.YL_ROLE_CONFIG.Add(new YL_ROLE_CONFIG
                            {
                                NAME = t.NAME,
                                ROLE_ID = ent.ID,
                                VALUE = t.VALUE
                            });
                        }
                        else
                        {
                            cfg.VALUE = t.VALUE;
                        }
                    }
                    foreach (var t in ent.YL_ROLE_CONFIG.ToList())
                    {
                        if (inEnt.RoleConfigs.SingleOrDefault(x => x.NAME == t.NAME) == null)
                        {
                            db.YL_ROLE_CONFIG.Remove(t);
                        }
                    }

                    if (isAdd)
                    {
                        db.YL_ROLE.Add(ent);
                    }
                    db.SaveChanges();
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Edit);
                    return true;
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = e.Message;
                    err.Excep = e;
                    return false;
                }
            }
        }

        public TRole RoleSingle(string loginKey, ref ErrorInfo err, int id)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                var reEnt = db.YL_ROLE.Where(x=>x.ID==id).ToList();
                TRole tmp = new TRole();
                IList<KV> allPara = new List<KV>();
                try
                {
                    if (CFG.ConfigRolePara != null)
                    {
                        allPara = JSON.EncodeToEntity<IList<KV>>(CFG.ConfigRolePara);
                    }
                }
                catch { }

                if (reEnt.Count > 0)
                {
                    tmp = Fun.ClassToCopy<YL_ROLE, ProInterface.Models.TRole>(reEnt[0]);
                    tmp.ModuleAllStr = ",";
                    foreach(var t in reEnt[0].YL_MODULE.ToList())
                    {
                        tmp.ModuleAllStr += t.ID + ",";
                    }
                    tmp.RoleConfigs = Fun.ClassListToCopy<YL_ROLE_CONFIG, ROLE_CONFIG>(reEnt[0].YL_ROLE_CONFIG.ToList());
                }
                //添加
                foreach (var t in allPara)
                {
                    var cfg=tmp.RoleConfigs.SingleOrDefault(x => x.NAME == t.K);
                    if (cfg == null)
                    {
                        tmp.RoleConfigs.Add(new ROLE_CONFIG { NAME = t.K, REMARK = t.V });
                    }
                    else
                    {
                        cfg.REMARK = t.V;
                    }
                }
                //删除
                foreach (var t in tmp.RoleConfigs)
                {
                    if (allPara.SingleOrDefault(x => x.K == t.NAME) == null)
                    {
                        tmp.RoleConfigs.Remove(t);
                    }
                }
                tmp.RoleConfigsStr = JSON.DecodeToStr(tmp.RoleConfigs);
                return tmp;
            }
        }


        public bool RoleDelete(string loginKey, ref ErrorInfo err, int id)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_ROLE.SingleOrDefault(a => a.ID == id);
                    ent.YL_BULLETIN.Clear();
                    ent.YL_FLOW_FLOWNODE_FLOW.Clear();
                    ent.YL_FUNCTION.Clear();
                    ent.YL_MODULE.Clear();
                    ent.YL_USER.Clear();
                    //ent.YL_APP_MEUN.Clear();
                    foreach (var t in ent.YL_ROLE_CONFIG.ToList())
                    {
                        db.YL_ROLE_CONFIG.Remove(t);
                    }
                    db.YL_ROLE.Remove(ent);
                    db.SaveChanges();
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Delete);
                    return true;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = e.Message;
                    return false;
                }
            }
        }

        public IList<System.Web.Mvc.SelectListItem> RoleGetNoAuthority(int roleId, int queryId)
        {
            using (DBEntities db = new DBEntities())
            {
                var queryEnt = db.YL_QUERY.SingleOrDefault(x => x.ID == queryId);
                IList<System.Web.Mvc.SelectListItem> reEnt = new List<System.Web.Mvc.SelectListItem>();
                IList<QueryRowBtn> rb = new List<QueryRowBtn>();
                IList<QueryRowBtn> hb = new List<QueryRowBtn>();
                try
                {
                    if (!string.IsNullOrEmpty(queryEnt.ROWS_BTN))
                    {
                        rb = JSON.EncodeToEntity<IList<QueryRowBtn>>(queryEnt.ROWS_BTN);
                    }
                }
                catch { }
                try
                {
                    if (!string.IsNullOrEmpty(queryEnt.HEARD_BTN))
                    {
                        hb = JSON.EncodeToEntity<IList<QueryRowBtn>>(queryEnt.HEARD_BTN);
                    }
                }
                catch { }
                foreach (var t in hb)
                {
                    rb.Add(t);
                }

                if (queryEnt.AUTO_LOAD != 1)
                {
                    rb.Add(new QueryRowBtn { Name = "查询" });
                }

                reEnt = rb.Select(x => new System.Web.Mvc.SelectListItem { Value = x.Name, Text = x.Name, Selected = false }).ToList();
                var roleAuth = db.YL_ROLE_QUERY_AUTHORITY.SingleOrDefault(x => x.ROLE_ID == roleId && x.QUERY_ID == queryId);
                if (roleAuth != null && roleAuth.NO_AUTHORITY!=null)
                {
                    foreach (var t in roleAuth.NO_AUTHORITY.Split(','))
                    {
                        var tmp = reEnt.Where(x => x.Value == t).ToList();
                        foreach (var x in tmp)
                        {
                            x.Selected = true;
                        }
                    }
                }
                return reEnt;
            }
        }


        public bool RoleSaveNoAuthority(string loginKey, ref ErrorInfo err, int roleId, int queryId, string AuthArr)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                var roleAuth = db.YL_ROLE_QUERY_AUTHORITY.SingleOrDefault(x => x.ROLE_ID == roleId && x.QUERY_ID == queryId);
                if (roleAuth == null)
                {
                    db.YL_ROLE_QUERY_AUTHORITY.Add(new YL_ROLE_QUERY_AUTHORITY { NO_AUTHORITY = AuthArr, QUERY_ID = queryId, ROLE_ID = roleId });
                }
                else
                {
                    roleAuth.NO_AUTHORITY = AuthArr;
                }
                db.SaveChanges();
            }
            return true;
        }
    }
}
