
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects;
using System.Linq;
using System.Reflection;
using System.Text;
using ProInterface.Models;
using ProInterface;
using System.Linq.Expressions;
using LINQExtensions;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Data.Entity.Validation;

namespace ProServer
{
    public partial class Service : IFlow
    {

        public TFlow FlowSingle(string loginKey, ref ErrorInfo err, int? id)
        {
            using (DBEntities db = new DBEntities())
            {
                var reEnt = new TFlow();
                var ent = db.YL_FLOW.SingleOrDefault(x => x.ID == id);
                if (ent == null)
                {
                    reEnt.FlowListStr = "[]";
                    reEnt.X_Y = "[]";
                    return reEnt;
                }
                else
                {
                    reEnt = Fun.ClassToCopy<YL_FLOW, ProInterface.Models.TFlow>(ent);
                    foreach (var t in ent.YL_FLOW_FLOWNODE_FLOW.ToList())
                    {
                        var flowEnt = Fun.ClassToCopy<YL_FLOW_FLOWNODE_FLOW, TFlowFlownodeFlow>(t);
                        flowEnt.AllRoleIDList = t.YL_ROLE.Select(x => x.ID).ToList();
                        foreach (var roleID in flowEnt.AllRoleIDList)
                        {
                            flowEnt.AllRoleStr += "," + roleID;
                        }
                        if (flowEnt.AllRoleStr != null && flowEnt.AllRoleStr.Length > 1) flowEnt.AllRoleStr = flowEnt.AllRoleStr.Substring(1);
                        reEnt.FlowList.Add(flowEnt);
                    }

                    reEnt.FlowListStr = JSON.DecodeToStr(reEnt.FlowList);
                    reEnt.AllFlownode = db.YL_FLOW_FLOWNODE.ToList().Select(x => new SelectListItem { Text = x.NAME, Value = x.ID.ToString() }).ToList();
                    return reEnt;
                }
            }

        }

        public bool FlowSave(string loginKey, ref ErrorInfo err, TFlow ent)
        {
            using (DBEntities db = new DBEntities())
            {
                ent.FlowList = JSON.EncodeToEntity<IList<TFlowFlownodeFlow>>(ent.FlowListStr);
                ent.Idxy = JSON.EncodeToEntity<IList<IdXY>>(ent.X_Y);

                if (string.IsNullOrEmpty(ent.FLOW_TYPE))
                {
                    ent.FLOW_TYPE = "默认";
                }

                var useId = ent.FlowList.Select(x => x.FROM_FLOWNODE_ID).ToList();
                foreach (var t in ent.FlowList)
                {
                    if (!useId.Contains(t.TO_FLOWNODE_ID))
                    {
                        useId.Add(t.TO_FLOWNODE_ID);
                    }
                }

                ent.Idxy = ent.Idxy.Where(x => useId.Contains(x.Id)).ToList();

                var flow = db.YL_FLOW.SingleOrDefault(x => x.ID == ent.ID);
                if (flow == null)
                {
                    flow = Fun.ClassToCopy<TFlow, YL_FLOW>(ent);
                    flow.ID = Fun.GetSeqID<YL_FLOW>();
                    db.YL_FLOW.Add(flow);
                }
                else
                {
                    flow = Fun.ClassToCopy<TFlow, YL_FLOW>(ent, flow);
                }

                #region 删除节点
                foreach (var t in flow.YL_FLOW_FLOWNODE_FLOW.ToList())
                {
                    if (ent.FlowList.Where(x => x.FROM_FLOWNODE_ID == t.FROM_FLOWNODE_ID && x.TO_FLOWNODE_ID == t.TO_FLOWNODE_ID).Count() == 0)
                    {
                        t.YL_ROLE.Clear();
                        db.YL_FLOW_FLOWNODE_FLOW.Remove(t);
                    }
                }
                #endregion

                foreach (var t in ent.FlowList)
                {
                    if (!string.IsNullOrEmpty(t.AllRoleStr))
                    {
                        string[] tmpRoleArr = t.AllRoleStr.Trim().Split(',');
                        if (tmpRoleArr.Length > 0)
                        {
                            t.AllRoleIDList = tmpRoleArr.Select(x => Convert.ToInt32(x)).ToList();
                        }
                    }
                    var thisFlow = flow.YL_FLOW_FLOWNODE_FLOW.SingleOrDefault(x => x.FROM_FLOWNODE_ID == t.FROM_FLOWNODE_ID && x.TO_FLOWNODE_ID == t.TO_FLOWNODE_ID);
                    if (thisFlow == null)
                    {
                        YL_FLOW_FLOWNODE_FLOW flowF = Fun.ClassToCopy<TFlowFlownodeFlow, YL_FLOW_FLOWNODE_FLOW>(t);

                        flowF.YL_ROLE = db.YL_ROLE.Where(x => t.AllRoleIDList.Contains(x.ID)).ToList();
                        flowF.ID = Fun.GetSeqID<YL_FLOW_FLOWNODE_FLOW>();
                        flowF.FLOW_ID = flow.ID;
                        db.YL_FLOW_FLOWNODE_FLOW.Add(flowF);
                    }
                    else
                    {
                        thisFlow = Fun.ClassToCopy<TFlowFlownodeFlow, YL_FLOW_FLOWNODE_FLOW>(t, thisFlow);
                        thisFlow.YL_ROLE.Clear();
                        thisFlow.YL_ROLE = db.YL_ROLE.Where(x => t.AllRoleIDList.Contains(x.ID)).ToList();
                        thisFlow = Fun.ClassToCopy<TFlowFlownodeFlow, YL_FLOW_FLOWNODE_FLOW>(t);
                    }
                }
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return false;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return false;
                }

            }
            return true;
        }

        public IList<System.Web.Mvc.SelectListItem> FlowAllFlownode()
        {
            using (DBEntities db = new DBEntities())
            {
                return db.YL_FLOW_FLOWNODE.ToList().Select(x => new System.Web.Mvc.SelectListItem { Value = x.ID.ToString(), Text = x.NAME }).ToList();
            }
        }


        public IList<System.Web.Mvc.SelectListItem> FlowAllRole(string loginKey, ref ErrorInfo err)
        {
            GlobalUser gu = Global.GetUser(loginKey);
            using (DBEntities db = new DBEntities())
            {
                return db.YL_ROLE.ToList().Select(x => new System.Web.Mvc.SelectListItem { Value = x.ID.ToString(), Text = x.NAME }).ToList();
            }
        }

        public IList<System.Web.Mvc.SelectListItem> FlowAll()
        {
            using (DBEntities db = new DBEntities())
            {
                return db.YL_FLOW.ToList().Select(x => new System.Web.Mvc.SelectListItem { Value = x.ID.ToString(), Text = x.NAME }).ToList();
            }
        }

        public IList<SelectListItem> FlowAllMy(string loginKey, ref ErrorInfo err)
        {
            var gu=Global.GetUser(loginKey);
            
            if(gu==null)
            {
                err.IsError=true;
                err.Message="登录超时";
                return null;
            }
            using (DBEntities db = new DBEntities())
            {
                var flow = db.YL_FLOW.ToList();
                var allRole=db.YL_ROLE.ToList().Where(x=>gu.RoleID.Contains(x.ID)).ToList();
                flow=flow.Where(x=>x.YL_FLOW_FLOWNODE_FLOW.Where(y=>y.FROM_FLOWNODE_ID==9999 && allRole.Intersect(y.YL_ROLE).Count()>0).Count()>0).ToList();
                return flow.Select(x => new System.Web.Mvc.SelectListItem { Value = x.ID.ToString(), Text = x.NAME }).ToList();
            }
        }


        public TFlowFlownodeFlow FlowFristNode(string loginKey, ref ErrorInfo err, int flowId, string status)
        {
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_FLOW_FLOWNODE_FLOW.SingleOrDefault(x => x.FLOW_ID == flowId && x.FROM_FLOWNODE_ID == 9999);
                var reEnt=new TFlowFlownodeFlow();
                if (ent != null)
                {
                    List<YL_FLOW_FLOWNODE_FLOW> nextNodeArr = db.YL_FLOW_FLOWNODE_FLOW.Where(x => x.FLOW_ID == flowId && x.FROM_FLOWNODE_ID == ent.TO_FLOWNODE_ID && x.STATUS == status).ToList();
                    if (string.IsNullOrEmpty(status))
                    {
                        nextNodeArr = db.YL_FLOW_FLOWNODE_FLOW.Where(x => x.FLOW_ID == flowId && x.FROM_FLOWNODE_ID == ent.TO_FLOWNODE_ID).ToList();
                    }
                    else
                    {
                        nextNodeArr = db.YL_FLOW_FLOWNODE_FLOW.Where(x => x.FLOW_ID == flowId && x.FROM_FLOWNODE_ID == ent.TO_FLOWNODE_ID && x.STATUS == status).ToList();
                    }
                    if (nextNodeArr.Count() > 0)
                    {
                        var nextNode = nextNodeArr[0];
                        reEnt = Fun.ClassToCopy<YL_FLOW_FLOWNODE_FLOW, TFlowFlownodeFlow>(nextNode);
                        var allNextRole = db.YL_ROLE.Where(x => x.YL_FLOW_FLOWNODE_FLOW.Where(y => y.FROM_FLOWNODE_ID == nextNode.TO_FLOWNODE_ID && y.FLOW_ID == flowId).Count() > 0).ToList();
                        reEnt.AllRoleIDList = allNextRole.Select(x => x.ID).ToList();
                        reEnt.AllRoleStr = string.Join(",", reEnt.AllRoleIDList);
                        reEnt.HandleUrl = nextNode.YL_FLOW_FLOWNODE.HANDLE_URL;
                        reEnt.ShowUrl = nextNode.YL_FLOW_FLOWNODE.SHOW_URL;
                        return reEnt;
                    }
                }
                return null;
            }
        }


        public TFlowFlownodeFlow FlowGetNodeFlow(string loginKey, ref ErrorInfo err, int flowId, int fromFlownodeId, string statusName)
        {
            using (DBEntities db = new DBEntities())
            {
                var nextNode = db.YL_FLOW_FLOWNODE_FLOW.Single(x => x.FLOW_ID == flowId && x.FROM_FLOWNODE_ID == fromFlownodeId && x.STATUS == statusName);
                var reEnt = new TFlowFlownodeFlow();
                if (nextNode != null)
                {
                    reEnt = Fun.ClassToCopy<YL_FLOW_FLOWNODE_FLOW, TFlowFlownodeFlow>(nextNode);
                    reEnt.AllRoleIDList = nextNode.YL_ROLE.Select(x => x.ID).ToList();
                    reEnt.AllRoleStr = string.Join(",", reEnt.AllRoleIDList);
                    reEnt.HandleUrl = nextNode.YL_FLOW_FLOWNODE.HANDLE_URL;
                    reEnt.ShowUrl = nextNode.YL_FLOW_FLOWNODE.SHOW_URL;
                    return reEnt;
                }
                return null;
            }
        }
    }
}
