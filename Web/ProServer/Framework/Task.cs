
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
    public partial class Service : ITask
    {
        public bool TaskHandle(string loginKey, ref ErrorInfo err, TTask inEnt)
        {
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return false;
            }
            using (DBEntities db = new DBEntities())
            {
                var allFIleId = "";
                if (!string.IsNullOrEmpty(inEnt.AllFilesStr))
                {
                    var allFile = JSON.EncodeToEntity<IList<FILES>>(inEnt.AllFilesStr);
                    allFIleId = string.Join(",", allFile.Select(x => x.ID));
                }
                if (inEnt.FLOW_ID != null)
                {
                    var reBool = FunTask.FlowSubmit(db, ref err, gu, inEnt.NowFlowId, inEnt.TaskContent, allFIleId, inEnt.UserIdArrStr, inEnt.ROLE_ID_STR, inEnt.IsStage, inEnt.NowSubmitType);
                }
                else {
                    var reBool = FunTask.NoFlowSubmit(db, ref err, gu, inEnt.NowFlowId, inEnt.TaskContent, allFIleId, inEnt.UserIdArrStr, inEnt.ROLE_ID_STR, inEnt.IsStage, inEnt.NowSubmitType);

                }
                if (err.IsError)
                {
                    return false;
                }
                db.SaveChanges();
                return true;
            }
        }

        public TTask TaskSingle(string loginKey, ref ErrorInfo err, int id)
        {
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }

            using (DBEntities db = new DBEntities())
            {
                var allFlownodeId = db.YL_FLOW_FLOWNODE_FLOW.Where(x => x.YL_ROLE.Where(y => gu.RoleID.Contains(y.ID)).Count() > 0).Select(x => x.FROM_FLOWNODE_ID).ToList();

                var task = db.YL_TASK.SingleOrDefault(x => x.ID == id);

                var tmp = Fun.ClassToCopy<YL_TASK, TTask>(task);
                var createUser = db.YL_USER.SingleOrDefault(x => x.ID == task.CREATE_USER);
 
                tmp.CreatePhone = createUser.LOGIN_NAME;


                IList<TTaskFlow> AllFlow = new List<TTaskFlow>();
                foreach (var flow in task.YL_TASK_FLOW.OrderBy(x => x.ID).ToList())
                {
                    TTaskFlow nowFlow = Fun.ClassToCopy<YL_TASK_FLOW, TTaskFlow>(flow);
                    nowFlow.FlowId = (flow.YL_TASK.FLOW_ID == null) ? 0 : flow.YL_TASK.FLOW_ID.Value;
                    if (!string.IsNullOrEmpty(flow.ROLE_ID_STR))
                    {
                        var allRoleId = flow.ROLE_ID_STR.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                        nowFlow.RoleList = db.YL_ROLE.Where(x => allRoleId.Contains(x.ID)).ToList().Select(x => new KTV { K = x.ID.ToString(), V = x.NAME, child = x.YL_USER.ToList().Select(y => new KTV { K = y.ID.ToString(), V = y.NAME }).ToList() }).ToList();
                    }

                    nowFlow.TaskName = flow.YL_TASK.TASK_NAME;
                    nowFlow.TaskRemark = flow.YL_TASK.REMARK;
                    if (flow.YL_TASK_FLOW2 != null && flow.YL_TASK_FLOW2.YL_TASK_FLOW_HANDLE.Count() > 0)
                    {
                        var handle = flow.YL_TASK_FLOW2.YL_TASK_FLOW_HANDLE.ToList()[0];
                        nowFlow.SendUserId = handle.DEAL_USER_ID;
                        nowFlow.SendUserName = handle.DEAL_USER_NAME;
                    }

                    foreach (var handle in flow.YL_TASK_FLOW_HANDLE.ToList())
                    {
                        var tmpHandle = Fun.ClassToCopy<YL_TASK_FLOW_HANDLE, TTaskFlowHandle>(handle);
                        tmpHandle.AllFiles = Fun.ClassListToCopy<YL_FILES, FILES>(handle.YL_FILES.ToList());
                        nowFlow.AllHandle.Add(tmpHandle);
                        nowFlow.DealUserName = handle.DEAL_USER_NAME;
                    }

                    AllFlow.Add(nowFlow);
                }
                tmp.AllFlow = AllFlow;
                #region 计算所有按钮
                var nowTaskFlowList = task.YL_TASK_FLOW.Where(x => x.IS_HANDLE == 0 && (x.HANDLE_USER_ID == gu.UserId || (x.FLOWNODE_ID != null && allFlownodeId.Contains(x.FLOWNODE_ID.Value)))).ToList();
                if (nowTaskFlowList.Count() > 0)
                {
                    var nowTaskFlow = nowTaskFlowList[0];
                    tmp.NowFlowId = nowTaskFlow.ID;
                    var allBut = db.YL_FLOW_FLOWNODE_FLOW.Where(x => x.FROM_FLOWNODE_ID == nowTaskFlow.FLOWNODE_ID && x.FLOW_ID == task.FLOW_ID).ToList();
                    tmp.AllButton = allBut.Select(x => x.STATUS).ToList();

                    if (AllFlow.Count() > 3 && !nowTaskFlow.YL_TASK_FLOW2.DEAL_STATUS.Equals("驳回")) tmp.AllButton.Insert(0, "驳回");

                }
                #endregion

                return tmp;
            }
        }


        public TTaskFlow TaskFlowSingle(int id)
        {
            using (DBEntities db = new DBEntities())
            {
                TTaskFlow reEnt = new TTaskFlow();
                var ent = db.YL_TASK_FLOW.SingleOrDefault(x => x.ID == id);
                if (ent == null)
                {
                    return reEnt;
                }
                reEnt = Fun.ClassToCopy<YL_TASK_FLOW, TTaskFlow>(ent);


                return reEnt;
            }
        }

        public TNode TaskNodeSingle(string loginKey, ref ErrorInfo err, int taskFlowID)
        {

            GlobalUser gu = Global.GetUser(loginKey);
            using (DBEntities db = new DBEntities())
            {
                var reEnt = db.YL_TASK_FLOW.SingleOrDefault(x => x.ID == taskFlowID);
                var ent = new ProInterface.Models.TNode();

                FunTask.SetFlowBaseInfo(db, ent, reEnt);
                ent.FlowNodeName = db.YL_FLOW_FLOWNODE.SingleOrDefault(x => x.ID == reEnt.FLOWNODE_ID).NAME;

                //获取主流程所经过节点
                IList<YL_FLOW_FLOWNODE> arr_FLOW_FLOWNODE = reEnt.YL_TASK.YL_TASK_FLOW.Where(x => x.FLOWNODE_ID < 8888).OrderByDescending(x => x.ID).Select(p => db.YL_FLOW_FLOWNODE.SingleOrDefault(x => x.ID == p.FLOWNODE_ID)).Distinct().ToList();
                //IList<YL_FLOW_FLOWNODE> arr_FLOW_FLOWNODE = db.YL_FLOW_FLOWNODE_FLOW.Where(x => x.FLOW_ID == 1 && x.FROM_FLOWNODE_ID < 9999).Select(x => x.YL_FLOW_FLOWNODE).Distinct().ToList();
                ent.AllFlowNode = arr_FLOW_FLOWNODE.Select(p => (new SelectListItem { Text = p.NAME, Value = p.ID.ToString() })).ToList();
                ent.ShowUrl = reEnt.SHOW_URL;
                return ent;
            }

        }
        public TNodeFlow TaskNodeFlowSingle(string loginKey, ref ErrorInfo err, int taskFlowID)
        {

            GlobalUser gu = Global.GetUser(loginKey);
            using (DBEntities db = new DBEntities())
            {
                var reEnt = db.YL_TASK_FLOW.SingleOrDefault(x => x.ID == taskFlowID);
                var ent = new ProInterface.Models.TNodeFlow();
                ent.ID = reEnt.ID;
                //ent.FlowID = reEnt.YL_TASK.FLOW_ID;
                return ent;
            }

        }

        public bool TaskNodeSave(string loginKey, ref ErrorInfo err, ProInterface.Models.TNode inEnt)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            GlobalUser gu = Global.GetUser(loginKey);
            using (DBEntities db = new DBEntities())
            {

                FunTask.UpDataTaskFlow(gu, ref err, db, inEnt);
                if (err.IsError)
                {
                    return false;
                }

                YL_TASK_FLOW taskFlow = db.YL_TASK_FLOW.SingleOrDefault(x => x.ID == inEnt.ID);

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (DbEntityValidationException error)
                {
                    err.IsError = true;
                    err.Message = error.Message;
                    return false;
                }
            }
        }

        public bool SupTaskNodeSave(string loginKey, ref ErrorInfo err, ProInterface.Models.TNode inEnt)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            GlobalUser gu = Global.GetUser(loginKey);
            using (DBEntities db = new DBEntities())
            {

                FunTask.SupUpDataTaskFlow(gu, ref err, db, inEnt.ID, inEnt.ToFlowNodeID, inEnt.Remark);
                if (err.IsError)
                {
                    return false;
                }

                YL_TASK_FLOW taskFlow = db.YL_TASK_FLOW.SingleOrDefault(x => x.ID == inEnt.ID);

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (DbEntityValidationException error)
                {
                    err.IsError = true;
                    err.Message = error.Message;
                    return false;
                }
            }
        }

        public string TaskGetLastTaskFlowShowUrl(int flowID, int nodeID)
        {
            using (DBEntities db = new DBEntities())
            {
                var lastNode = db.YL_TASK_FLOW.SingleOrDefault(x => x.ID == flowID).YL_TASK.YL_TASK_FLOW.Where(x => x.FLOWNODE_ID == nodeID && x.IS_HANDLE == 1).OrderByDescending(x => x.ID).ToList();
                if (lastNode.Count > 0)
                {
                    string url = lastNode[0].SHOW_URL;
                    if (url.IndexOf('?') > 0)
                    {
                        url += "&id=" + lastNode[0].ID;
                    }
                    else
                    {
                        url += "?id=" + lastNode[0].ID;
                    }
                    return url;
                }
                return "";
            }
        }

        public TFlow TaskGetTaskFlow(string loginKey, ref ErrorInfo err, int taskId)
        {
            using (DBEntities db = new DBEntities())
            {
                var taskEnt = db.YL_TASK.SingleOrDefault(x => x.ID == taskId);
                return FlowSingle(loginKey, ref err, taskEnt.FLOW_ID);
            }
        }


        #region 默认方法

        /// <summary>
        /// 修改任务
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改任务</returns>
        public bool TaskSave(string loginKey, ref ProInterface.ErrorInfo err, ProInterface.Models.TTask inEnt, IList<string> allPar)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    GlobalUser gu = Global.GetUser(loginKey);
                    if (gu == null)
                    {
                        err.IsError = true;
                        err.Message = "登录超时";
                        return false;
                    }
                    var ent = db.YL_TASK.SingleOrDefault(a => a.ID == inEnt.ID);
                    YL_TASK_FLOW taskFlow0 = new YL_TASK_FLOW();
                    bool isAdd = false;
                    if (ent == null)
                    {
                        isAdd = true;
                        if (inEnt.FLOW_ID != null && inEnt.FLOW_ID.Value!=0)
                        {
                            #region 非任务工单
                            YL_FLOW flow = db.YL_FLOW.SingleOrDefault(x => x.ID == inEnt.FLOW_ID);
                            TNode tnode = new TNode();
                            tnode.FlowID = inEnt.FLOW_ID.Value;
                            tnode.TaskName = inEnt.TASK_NAME;
                            tnode.AllFilesStr = inEnt.AllFilesStr;
                            tnode.UserIdArrStr = inEnt.UserIdArrStr;
                            tnode.Remark = inEnt.REMARK;
                            FunTask.StartTask(db, ref err, gu, tnode, inEnt.NowSubmitType);

                            #endregion
                        }
                        else
                        {
                            #region 任务工单
                            FunTask.StartTaskNoFlow(db, ref err, gu, inEnt.TASK_NAME, inEnt.REMARK, inEnt.AllFilesStr, inEnt.UserIdArrStr, inEnt.ROLE_ID_STR);
                            #endregion
                        }
                    }
                    else
                    {
                        ent = Fun.ClassToCopy<ProInterface.Models.TASK, YL_TASK>(inEnt, ent, allPar);
                    }
                    db.SaveChanges();
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Edit);
                    return true;
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
        }

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">主键ID</param>
        /// <returns>查询一条</returns>
        public ProInterface.Models.TASK TaskSingleId(string loginKey, ref ProInterface.ErrorInfo err, int keyId)
        {

            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_TASK.SingleOrDefault(x => x.ID == keyId);
                var reEnt = new ProInterface.Models.TASK();
                if (ent != null)
                {
                    reEnt = Fun.ClassToCopy<YL_TASK, ProInterface.Models.TASK>(ent);
                }
                return reEnt;
            }
        }


        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">删除任务</param>
        /// <returns>删除任务</returns>
        public bool TaskDelete(string loginKey, ref ProInterface.ErrorInfo err, int keyId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {

                    string delSql = @"
DELETE FROM YL_TASK_FLOW_HANDLE_USER WHERE TASK_FLOW_ID IN (SELECT ID FROM YL_TASK_FLOW WHERE TASK_ID={0});
DELETE FROM YL_TASK_FLOW_HANDLE_FILES WHERE FLOW_HANDLE_ID IN (SELECT ID FROM YL_TASK_FLOW_HANDLE WHERE TASK_FLOW_ID IN (SELECT ID FROM YL_TASK_FLOW WHERE TASK_ID={0}));
DELETE FROM YL_TASK_FLOW_HANDLE WHERE TASK_FLOW_ID IN (SELECT ID FROM YL_TASK_FLOW WHERE TASK_ID={0});
DELETE FROM YL_TASK_FLOW WHERE TASK_ID={0};
DELETE FROM YL_TASK WHERE ID={0};
                    ";
                    delSql = string.Format(delSql, keyId);
                    int i = 0;
                    foreach (var t in delSql.Split(';'))
                    {
                        i = ExecuteNonQuery(1, t);
                    }
                    return true;

                }
                catch {
                    return false;
                }
            }
        }



        #endregion


        public IList<TTaskFlow> TaskGetTreeFlow(string loginKey, ref ErrorInfo err, int taskId)
        {
            using (DBEntities db = new DBEntities())
            {
                var allFlow = db.YL_TASK_FLOW.Where(x => x.TASK_ID == taskId).ToList();
                IList<TTaskFlow> reFlow = new List<TTaskFlow>();
                foreach (var t in allFlow.Where(x => x.PARENT_ID == null).OrderBy(x => x.ID).ToList())
                {
                    TTaskFlow tmp = new TTaskFlow();
                    tmp = Fun.ClassToCopy<YL_TASK_FLOW, TTaskFlow>(t);
                    if (t.IS_HANDLE == 0)
                    {
                        tmp.DealRole = "待处理";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(t.ROLE_ID_STR))
                        {
                            tmp.DealRole = t.DEAL_STATUS;
                        }
                        else
                        {
                            List<int> role = tmp.ROLE_ID_STR.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                            tmp.DealRole = string.Format("分发{0}", string.Join(",", db.YL_ROLE.Where(x => role.Contains(x.ID)).Select(x => x.NAME).ToList()));
                        }
                        var allChildre = db.YL_TASK_FLOW.Where(x => x.PARENT_ID == t.ID).ToList();
                        if (allChildre.Count() == 0)
                        {
                            if (t.YL_TASK_FLOW2 != null)
                            {
                                if (t.YL_TASK_FLOW2.YL_TASK_FLOW_HANDLE.Count() > 0)
                                {
                                    var userId=t.YL_TASK_FLOW2.HANDLE_USER_ID;
                                    var userTmp = db.YL_USER.SingleOrDefault(x => x.ID == userId);
                                    if (userTmp != null)
                                    {
                                        tmp.NextDealUserName = userTmp.NAME;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var userIdArr = allChildre.Select(x => x.HANDLE_USER_ID).ToList();
                            tmp.NextDealUserName = string.Join(",", db.YL_USER.Where(x=>userIdArr.Contains(x.ID)).Select(x => x.NAME).ToList());
                        }
                        tmp.children = TaskGetTreeFlowChildren(loginKey, ref err, t.ID, allFlow);
                    }
                    reFlow.Add(tmp);
                }
                return reFlow;
            }
        }


        public IList<TTaskFlow> TaskGetTreeFlowChildren(string loginKey, ref ErrorInfo err, int flowId,IList<YL_TASK_FLOW> allData)
        {
            using (DBEntities db = new DBEntities())
            {
                IList<TTaskFlow> reFlow = new List<TTaskFlow>();
                foreach (var t in allData.Where(x => x.PARENT_ID == flowId).OrderBy(x=>x.ID).ToList())
                {
                    TTaskFlow tmp = new TTaskFlow();
                    tmp = Fun.ClassToCopy<YL_TASK_FLOW, TTaskFlow>(t);
                    if (t.IS_HANDLE == 0)
                    {
                        tmp.DealRole = "待处理";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(t.ROLE_ID_STR))
                        {
                            tmp.DealRole = t.DEAL_STATUS;
                        }
                        else
                        {
                            List<int> role = tmp.ROLE_ID_STR.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                            tmp.DealRole = string.Format("分发{0}", string.Join(",", db.YL_ROLE.Where(x => role.Contains(x.ID)).Select(x => x.NAME).ToList()));
                        }
                        var allChildre = db.YL_TASK_FLOW.Where(x => x.PARENT_ID == t.ID).ToList();
                        if (allChildre.Count() == 0)
                        {
                            if (t.YL_TASK_FLOW2 != null)
                            {
                                if (t.YL_TASK_FLOW2.YL_TASK_FLOW_HANDLE.Count() > 0)
                                {
                                    var userId = t.YL_TASK_FLOW2.HANDLE_USER_ID;
                                    var userTmp = db.YL_USER.SingleOrDefault(x => x.ID == userId);
                                    if (userTmp != null)
                                    {
                                        tmp.NextDealUserName = userTmp.NAME;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var userIdArr = allChildre.Select(x => x.HANDLE_USER_ID).ToList();
                            tmp.NextDealUserName = string.Join(",", db.YL_USER.Where(x => userIdArr.Contains(x.ID)).Select(x => x.NAME).ToList());
                        }
                        tmp.children = TaskGetTreeFlowChildren(loginKey, ref err, t.ID, allData);
                    }
                    reFlow.Add(tmp);
                }
                return reFlow;
            }
        }
    }
}
