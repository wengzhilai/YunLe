
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
    public partial class Service : ITaskFlow
    {

        public TTaskFlow TaskFlowSingleId(string loginKey, ref ErrorInfo err, int keyId)
        {
            GlobalUser gu = Global.GetUser(loginKey);
            using (DBEntities db = new DBEntities())
            {
                var reEnt = db.YL_TASK_FLOW.SingleOrDefault(x => x.ID == keyId);
                var ent = new ProInterface.Models.TTaskFlow();
                ent = Fun.ClassToCopy<YL_TASK, ProInterface.Models.TTaskFlow>(reEnt.YL_TASK);
                ent = Fun.ClassToCopy<YL_TASK_FLOW, ProInterface.Models.TTaskFlow>(reEnt);
                var sendUser=db.YL_USER.SingleOrDefault(x=>x.ID==reEnt.YL_TASK.CREATE_USER);
                if(sendUser!=null)
                {
                    ent.DistrictName = sendUser.YL_DISTRICT.NAME;
                }
                ent.TaskContent = reEnt.YL_TASK.REMARK;

                var allUserIdList=reEnt.YL_TASK_FLOW1.Select(x=>x.HANDLE_USER_ID).ToList();
                ent.UserIdArrStr = string.Join(",", allUserIdList);

                return ent;
            }
        }


        
        public IList<SelectListItem> TaskFlowMyAllRole(string loginKey, ref ErrorInfo err)
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
                string idPath=string.Format(".{0}.",gu.DistrictId);
                //我管辖区域的所有用户
                var allUserIdList = db.YL_USER.Where(x => x.YL_DISTRICT.ID_PATH.IndexOf(idPath) > 0 || x.DISTRICT_ID==gu.DistrictId).Select(x=>x.ID).ToList();
                //我管辖区域所有用户拥有的角色
                var allRole = db.YL_ROLE.Where(x => x.YL_USER.Where(y => allUserIdList.Contains(y.ID)).Count() > 0).ToList();
                return allRole.Select(x => new SelectListItem { Value=x.ID.ToString(),Text=x.NAME }).ToList();
            }
        }


        public bool TaskFlowSave(string loginKey, ref ProInterface.ErrorInfo err, ProInterface.Models.TTaskFlow inEnt, IList<string> allPar)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                GlobalUser gu = Global.GetUser(loginKey);
                if (gu == null)
                {
                    err.IsError = true;
                    err.Message = "登录超时";
                    return false;
                }
                var flow = db.YL_TASK_FLOW.Single(x => x.ID == inEnt.ID);
                if (string.IsNullOrEmpty(inEnt.ROLE_ID_STR)) //无转派
                {
                    flow.DEAL_STATUS = "处理完成";
                    if (flow.PARENT_ID != null) //有上级
                    {
                        //是否全部处理
                        var noDealNum = flow.YL_TASK_FLOW2.YL_TASK_FLOW1.Where(x => x.IS_HANDLE == 0 && x.ID != flow.ID).Count();
                        if (noDealNum == 0)
                        {
                            var taskFlowNext2 = new YL_TASK_FLOW();
                            taskFlowNext2.ID = Fun.GetSeqID<YL_TASK_FLOW>();
                            taskFlowNext2.HANDLE_USER_ID = flow.YL_TASK_FLOW2.HANDLE_USER_ID;
                            //taskFlowNext2.DEAL_USER_NAME = flow.YL_TASK_FLOW2.DEAL_USER_NAME;
                            taskFlowNext2.LEVEL_ID = flow.LEVEL_ID + 1;
                            taskFlowNext2.NAME = flow.YL_TASK_FLOW2.NAME;
                            taskFlowNext2.HANDLE_URL = "~/TaskFlow/Handle";
                            taskFlowNext2.SHOW_URL = "~/TaskFlow/Single";
                            taskFlowNext2.IS_HANDLE = 0;
                            taskFlowNext2.PARENT_ID = flow.YL_TASK_FLOW2.PARENT_ID;
                            taskFlowNext2.TASK_ID = flow.TASK_ID;
                            db.YL_TASK_FLOW.Add(taskFlowNext2);
                        }
                    }
                    else
                    {
                        flow.YL_TASK.STATUS = "完成";
                        flow.YL_TASK.STATUS_TIME = DateTime.Now;
                    }
                }
                else //转派
                {
                    flow.DEAL_STATUS = "分派处理";
                    if (!string.IsNullOrEmpty(inEnt.UserIdArrStr))
                    {
                        IList<int> userIdArr = inEnt.UserIdArrStr.Split(',').Where(x => x.IsInt32()).Select(x => Convert.ToInt32(x)).ToList();
                        var allUser = db.YL_USER.Where(x => userIdArr.Contains(x.ID)).ToList();
                        var allRole = db.YL_ROLE.Where(x => x.YL_USER.Where(y => userIdArr.Contains(y.ID)).Count() > 0).ToList();
                        flow.ROLE_ID_STR = string.Join(",", allRole.Select(x => x.ID).ToList());

                        foreach (var t in flow.YL_TASK_FLOW1.ToList())
                        {
                            if (allUser.SingleOrDefault(x => x.ID == t.HANDLE_USER_ID) == null)
                            {
                                db.YL_TASK_FLOW.Remove(t);
                            }
                        }

                        foreach (var user in allUser)
                        {
                            YL_TASK_FLOW taskFlowNext1 = flow.YL_TASK_FLOW1.SingleOrDefault(x=>x.HANDLE_USER_ID==user.ID);
                            if (taskFlowNext1 == null)
                            {
                                taskFlowNext1 = new YL_TASK_FLOW();
                                taskFlowNext1.ID = Fun.GetSeqID<YL_TASK_FLOW>();
                                taskFlowNext1.HANDLE_USER_ID = user.ID;
                                //taskFlowNext1.DEAL_USER_NAME = user.NAME;
                                taskFlowNext1.LEVEL_ID = flow.LEVEL_ID + 1;
                                taskFlowNext1.NAME = "承办人办理";
                                taskFlowNext1.HANDLE_URL = "~/TaskFlow/Handle";
                                taskFlowNext1.SHOW_URL = "~/TaskFlow/Single";
                                taskFlowNext1.IS_HANDLE = 0;
                                taskFlowNext1.YL_TASK_FLOW2 = flow;
                                taskFlowNext1.TASK_ID = flow.TASK_ID;
                                flow.YL_TASK_FLOW1.Add(taskFlowNext1);
                            }
                        }
                    }
                    else //角色下所有用户
                    { 

                    }
                }
                flow.IS_HANDLE = 1;

                //if (!string.IsNullOrEmpty(inEnt.AllFilesStr))
                //{
                //    var fileIdList = ProInterface.JSON.EncodeToEntity<IList<FILES>>(inEnt.AllFilesStr).Select(x => x.ID);
                //    flow.YL_FILES.Clear();
                //    flow.YL_FILES = db.YL_FILES.Where(x => fileIdList.Contains(x.ID)).ToList();
                //}

                //flow.REMARK = inEnt.REMARK;
                db.SaveChanges();

                return true;
            }
        }


        public int TaskFlowAccept(string loginKey, ref ErrorInfo err, string taskFlowIdStr)
        {
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return 0;
            }
            using (DBEntities db = new DBEntities())
            {
                var allIdList = taskFlowIdStr.Split(',').Where(x=>x.IsInt32()).Select(x => Convert.ToInt32(x)).ToList();
                var taskFlow = db.YL_TASK_FLOW.Where(x =>allIdList.Contains(x.ID)).ToList();
                var i = 0;
                foreach (var t in taskFlow)
                {
                    if (t.ACCEPT_TIME == null)
                    {
                        t.ACCEPT_TIME = DateTime.Now;
                        i++;
                    }
                }
                db.SaveChanges();
                return i;
            }
        }
    }
}
