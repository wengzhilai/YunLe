using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ProInterface;
using ProInterface.Models;

namespace ProServer
{
    public class FunTask
    {
        public static object SubmitTask;

        /// <summary>
        /// 启动一个流程任务
        /// </summary>
        /// <param name="db"></param>
        /// <param name="err"></param>
        /// <param name="gu"></param>
        /// <param name="inEnt">启动内容(FlowID,TaskName,AllFilesStr,UserIdArrStr)</param>
        /// <param name="submitStatu">如果启动时有多个流程需填写此项</param>
        /// <returns></returns>
        public static YL_TASK_FLOW StartTask(DBEntities db, ref ErrorInfo err, GlobalUser gu, TNode inEnt, string submitStatu = null)
        {

            YL_TASK_FLOW reEnt = new YL_TASK_FLOW();
            int StartFlownodeId = 9999;
            //流程
            YL_FLOW flow = db.YL_FLOW.SingleOrDefault(x => x.ID == inEnt.FlowID);

            if (flow == null)
            {
                err.IsError = true;
                err.Message = "流程不存在";
                return null;
            }

            //流程的入口
            YL_FLOW_FLOWNODE_FLOW startFlow = new YL_FLOW_FLOWNODE_FLOW();
            //入口的下一节点
            YL_FLOW_FLOWNODE flowNodeNext = new YL_FLOW_FLOWNODE();
            //流程的入口的下一流程
            YL_FLOW_FLOWNODE_FLOW flowNodeFlowNext = new YL_FLOW_FLOWNODE_FLOW();
            //入口的下一节点下一节点
            YL_FLOW_FLOWNODE flowNodeNextNext = new YL_FLOW_FLOWNODE();
            #region 初始化数据
            startFlow = flow.YL_FLOW_FLOWNODE_FLOW.SingleOrDefault(x => x.FROM_FLOWNODE_ID == StartFlownodeId);
            if (startFlow == null)
            {
                err.IsError = true;
                err.Message = "流程入口错误";
                return null;
            }
            flowNodeNext = db.YL_FLOW_FLOWNODE.SingleOrDefault(x => x.ID == startFlow.TO_FLOWNODE_ID);
            if (flowNodeNext == null)
            {
                err.IsError = true;
                err.Message = "未找到启动后的目标节点";
                return null;
            }
            flowNodeFlowNext = flow.YL_FLOW_FLOWNODE_FLOW.SingleOrDefault(x => x.FROM_FLOWNODE_ID == startFlow.TO_FLOWNODE_ID && x.STATUS == submitStatu);
            if (flowNodeFlowNext == null)
            {
                flowNodeFlowNext = flow.YL_FLOW_FLOWNODE_FLOW.SingleOrDefault(x => x.FROM_FLOWNODE_ID == startFlow.TO_FLOWNODE_ID);
            }
            if (flowNodeFlowNext == null)
            {
                err.IsError = true;
                err.Message = "未找到下一节点";
                return null;
            }
            flowNodeNextNext = db.YL_FLOW_FLOWNODE.SingleOrDefault(x => x.ID == flowNodeFlowNext.TO_FLOWNODE_ID);
            #endregion


            if (flowNodeFlowNext.ASSIGNER == 1 && string.IsNullOrEmpty(inEnt.UserIdArrStr))
            {
                err.IsError = true;
                err.Message = "必须指处理人";
                return null;
            }

            //每启一个任务生成共三条

            YL_TASK task = new YL_TASK();
            #region 初始化任务

            task.ID = Fun.GetSeqID<YL_TASK>();
            task.FLOW_ID = flow.ID;
            task.TASK_NAME = inEnt.TaskName;
            task.CREATE_TIME = DateTime.Now;
            task.CREATE_USER = gu.UserId;
            task.CREATE_USER_NAME = gu.UserName;
            task.REMARK = inEnt.Remark;
            task.KEY = inEnt.TaskKey;
            task.STATUS = "未完成";
            task.START_TIME = inEnt.StartTime;
            task.END_TIME = inEnt.EndTime;
            task.TASK_NAME = task.TASK_NAME.Replace("@(TASK_ID)", task.ID.ToString());
            task.STATUS_TIME = DateTime.Now;

            //保单有效期为72小时
            if (task.FLOW_ID == 1)
            {
                task.END_TIME = DateTime.Now.AddHours(72);
            }
            #endregion

            YL_TASK_FLOW taskFlow0 = new YL_TASK_FLOW();
            #region 初始化启动节点

            taskFlow0.ID = Fun.GetSeqID<YL_TASK_FLOW>();
            taskFlow0.DEAL_STATUS = startFlow.STATUS;
            taskFlow0.HANDLE_USER_ID = gu.UserId;
            taskFlow0.FLOWNODE_ID = StartFlownodeId;
            taskFlow0.HANDLE_URL = startFlow.YL_FLOW_FLOWNODE.HANDLE_URL;
            taskFlow0.SHOW_URL = startFlow.YL_FLOW_FLOWNODE.SHOW_URL;
            taskFlow0.LEVEL_ID = 1;
            taskFlow0.IS_HANDLE = 1;
            taskFlow0.NAME = startFlow.YL_FLOW_FLOWNODE.NAME;
            //taskFlow0.NAME = string.Format("{0}处理",db.YL_ROLE.ToList().SingleOrDefault(x=>gu.RoleID.Contains(x.ID)));
            taskFlow0.START_TIME = DateTime.Now;
            taskFlow0.DEAL_TIME = DateTime.Now;
            taskFlow0.ACCEPT_TIME = DateTime.Now;
            taskFlow0.TASK_ID = task.ID;
            taskFlow0.YL_TASK = task;

            taskFlow0.YL_TASK_FLOW_HANDLE.Add(new YL_TASK_FLOW_HANDLE
            {
                ID = Fun.GetSeqID<YL_TASK_FLOW_HANDLE>(),
                CONTENT = "启动",
                DEAL_TIME = DateTime.Now,
                DEAL_USER_ID = gu.UserId,
                DEAL_USER_NAME = gu.UserName,
                TASK_FLOW_ID = taskFlow0.ID
            });
            #endregion

            task.YL_TASK_FLOW.Add(taskFlow0);
            //如果下级节点不为空
            if (flowNodeNext != null)
            {
                YL_TASK_FLOW taskFlowNext1 = new YL_TASK_FLOW();
                #region 初始化创建节点

                taskFlowNext1.ID = Fun.GetSeqID<YL_TASK_FLOW>();
                taskFlowNext1.DEAL_STATUS = flowNodeFlowNext.STATUS;
                taskFlowNext1.HANDLE_USER_ID = gu.UserId;
                taskFlowNext1.FLOWNODE_ID = flowNodeNext.ID;
                taskFlowNext1.HANDLE_URL = flowNodeNext.HANDLE_URL;
                taskFlowNext1.SHOW_URL = flowNodeNext.SHOW_URL;
                taskFlowNext1.LEVEL_ID = taskFlow0.LEVEL_ID + 1;
                taskFlowNext1.IS_HANDLE = 1;
                taskFlowNext1.NAME = string.Format("{0}处理", string.Join(",", db.YL_ROLE.ToList().Where(x => gu.RoleID.Contains(x.ID)).Select(x => x.NAME)));
                //taskFlowNext1.NAME = flowNodeNext.NAME;
                taskFlowNext1.YL_TASK_FLOW2 = taskFlow0;
                taskFlowNext1.START_TIME = DateTime.Now;
                taskFlowNext1.DEAL_TIME = DateTime.Now;
                taskFlowNext1.ACCEPT_TIME = DateTime.Now;
                taskFlowNext1.TASK_ID = task.ID;
                taskFlowNext1.YL_TASK = task;

                IList<YL_FILES> allFile = new List<YL_FILES>();
                if (!string.IsNullOrEmpty(inEnt.AllFilesStr))
                {
                    var fileIdList = ProInterface.JSON.EncodeToEntity<IList<FILES>>(inEnt.AllFilesStr).Select(x => x.ID);
                    allFile = db.YL_FILES.Where(x => fileIdList.Contains(x.ID)).ToList();
                }

                taskFlowNext1.YL_TASK_FLOW_HANDLE.Add(new YL_TASK_FLOW_HANDLE
                {
                    ID = Fun.GetSeqID<YL_TASK_FLOW_HANDLE>(),
                    CONTENT = inEnt.Remark,
                    DEAL_TIME = DateTime.Now,
                    DEAL_USER_ID = gu.UserId,
                    DEAL_USER_NAME = gu.UserName,
                    TASK_FLOW_ID = taskFlow0.ID,
                    YL_FILES = allFile
                });

                #endregion
                if (flowNodeNextNext != null)
                {


                    YL_TASK_FLOW taskFlowNext2 = new YL_TASK_FLOW();

                    taskFlowNext2.ID = Fun.GetSeqID<YL_TASK_FLOW>();
                    taskFlowNext2.FLOWNODE_ID = flowNodeNextNext.ID;
                    taskFlowNext2.HANDLE_URL = flowNodeNextNext.HANDLE_URL;
                    taskFlowNext2.SHOW_URL = flowNodeNextNext.SHOW_URL;
                    taskFlowNext2.LEVEL_ID = taskFlowNext1.LEVEL_ID + 1;
                    taskFlowNext2.IS_HANDLE = 0;
                    taskFlowNext2.DEAL_STATUS = "待处理";
                    //表示无须受理
                    taskFlowNext2.ACCEPT_TIME = DateTime.Now;
                    taskFlowNext2.START_TIME = DateTime.Now;
                    if (flowNodeFlowNext.EXPIRE_HOUR != 0)
                    {
                        taskFlowNext2.EXPIRE_TIME = taskFlowNext2.START_TIME.AddHours(flowNodeFlowNext.EXPIRE_HOUR);
                    }
                    taskFlowNext2.YL_TASK_FLOW2 = taskFlowNext1;
                    taskFlowNext2.YL_TASK = task;

                    var allRoleFlow = db.YL_ROLE.Where(x => x.YL_FLOW_FLOWNODE_FLOW.Where(y => y.FROM_FLOWNODE_ID == taskFlowNext2.FLOWNODE_ID && y.FLOW_ID == task.FLOW_ID).Count() > 0).ToList();
                    taskFlowNext2.NAME = string.Format("待{0}处理", string.Join(",", allRoleFlow.Select(x => x.NAME)));

                    var server = new Service();
                    if (string.IsNullOrEmpty(inEnt.UserIdArrStr)) inEnt.UserIdArrStr = "";
                    IList<int> nowHandleUserIdList = inEnt.UserIdArrStr.Split(',').Where(x => x.IsInt32()).Select(x => Convert.ToInt32(x)).ToList();
                    var allUser = db.YL_USER.Where(x => nowHandleUserIdList.Contains(x.ID)).ToList();
                    //如果指定人
                    if (allUser.Count() > 0)
                    {
                        nowHandleUserIdList = allUser.Select(x => x.ID).ToList();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(gu.Guid))
                        {
                            nowHandleUserIdList = server.UserGetAllUserById(gu.UserId, ref err, allRoleFlow.Select(x => x.ID).ToList());
                        }
                        else
                        {
                            nowHandleUserIdList = server.UserGetAllUserById(gu.UserId, ref err, allRoleFlow.Select(x => x.ID).ToList(), 2);
                        }
                    }

                    if (nowHandleUserIdList.Count() == 0)
                    {
                        err.IsError = true;
                        err.Message = string.Format("创建失败，请先配置角色[{0}]下的用户", string.Join(",", allRoleFlow.Select(x => x.NAME).ToList()));
                        return null;
                    }

                    var allNextFlow = flow.YL_FLOW_FLOWNODE_FLOW.Where(x => x.FROM_FLOWNODE_ID == flowNodeFlowNext.TO_FLOWNODE_ID && x.HANDLE == 1);

                    if (allNextFlow.Count() == 0) //一人处理即可
                    {
                        foreach (var handleU in nowHandleUserIdList)
                        {
                            taskFlowNext2.YL_TASK_FLOW_HANDLE_USER.Add(new YL_TASK_FLOW_HANDLE_USER { HANDLE_USER_ID = handleU });
                        }
                        taskFlowNext1.YL_TASK_FLOW1.Add(taskFlowNext2);
                    }
                    else //所有人必须处理
                    {
                        foreach (var handleU in nowHandleUserIdList)
                        {
                            var tmpTaskFlow = Fun.ClassToCopy<YL_TASK_FLOW, YL_TASK_FLOW>(taskFlowNext2);
                            tmpTaskFlow.ID = Fun.GetSeqID<YL_TASK_FLOW>();
                            tmpTaskFlow.HANDLE_USER_ID = handleU;
                            db.YL_TASK_FLOW.Add(tmpTaskFlow);
                        }
                    }


                    #region 发送提醒信息
                    //保存信息并用微信发送
                    var msg = string.Format("【云乐享车】您的工单[{0}]已经[{1}]，详细情况请进入系统查询",
                        task.TASK_NAME,
                        "待处理"
                        );

                    List<KV> inPar = new List<KV>();
                    inPar.Add(new KV { V = task.ID.ToString() });
                    var userList = db.YL_USER.Where(x => nowHandleUserIdList.Contains(x.ID)).ToList();
                    foreach (var user in userList)
                    {
                        server.UserMessageSaveAndSend(
                        gu.Guid,
                        ref err,
                        msg,
                        4,
                        new List<int> { user.ID },
                        inPar);
                        //表示微信发送失败，则用短信发送
                        if (err.IsError)
                        {
                            //清除错误
                            err = new ErrorInfo();
                            server.SmsSendOrder(ref err, user.LOGIN_NAME, task.TASK_NAME, "待处理");
                        }
                    }

                    #endregion

                }
                reEnt = taskFlowNext1;
                taskFlow0.YL_TASK_FLOW1.Add(taskFlowNext1);
            }
            task.KEY = reEnt.ID.ToString();
            db.YL_TASK.Add(task);
            return reEnt;
        }

        /// <summary>
        /// 启动一个流程任务
        /// </summary>
        /// <param name="db"></param>
        /// <param name="err"></param>
        /// <param name="gu"></param>
        /// <param name="flowId">流程ID</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="allFileStr">上传的文件</param>
        /// <param name="userIdArrStr">指定处理人</param>
        /// <returns></returns>
        public static YL_TASK_FLOW StartTask(DBEntities db, ref ErrorInfo err, GlobalUser gu, int flowId, string taskName, string content, string allFileStr, string userIdArrStr, string taskKey = "")
        {
            TNode inEnt = new TNode();
            inEnt.FlowID = flowId;
            inEnt.TaskName = taskName;
            inEnt.AllFilesStr = allFileStr;
            inEnt.UserIdArrStr = userIdArrStr;
            inEnt.Remark = content;
            inEnt.TaskKey = taskKey;
            return StartTask(db, ref err, gu, inEnt);
        }

        /// <summary>
        /// 启动任务工单
        /// </summary>
        /// <param name="db"></param>
        /// <param name="err"></param>
        /// <param name="gu"></param>
        /// <param name="taskName"></param>
        /// <param name="allFileStr"></param>
        /// <param name="userIdArrStr"></param>
        /// <param name="roleIdStr"></param>
        /// <returns></returns>
        public static bool StartTaskNoFlow(DBEntities db, ref ErrorInfo err, GlobalUser gu,
            string taskName,
            string content,
            string allFileStr,
            string userIdArrStr,
            string roleIdStr)
        {


            if (string.IsNullOrEmpty(userIdArrStr) && string.IsNullOrEmpty(roleIdStr))
            {
                err.IsError = true;
                err.Message = "请选择处理人或角色";
                return false;
            }

            var ent = new YL_TASK();
            ent.ID = Fun.GetSeqID<YL_TASK>();
            if (string.IsNullOrEmpty(taskName))
            {
                switch (ProInterface.AppSet.CityId)
                {
                    case 852:
                        ent.TASK_NAME = string.Format("ZY-01-{0}-{1}", DateTime.Now.ToString("yyMMdd"), ent.ID.ToString());
                        break;
                    case 855:
                        ent.TASK_NAME = string.Format("QDN-01-{0}-{1}", DateTime.Now.ToString("yyMMdd"), ent.ID.ToString());
                        break;
                }
            }
            else
            {
                ent.TASK_NAME = taskName.Replace("@(TASK_ID)", ent.ID.ToString());
            }
            ent.CREATE_TIME = DateTime.Now;
            ent.CREATE_USER = gu.UserId;
            ent.CREATE_USER_NAME = gu.UserName;
            ent.STATUS = "待处理";
            ent.REMARK = content;
            ent.STATUS_TIME = DateTime.Now;
            ent.START_TIME = DateTime.Now;
            ent.END_TIME = DateTime.Now;

            var taskFlowStart = new YL_TASK_FLOW();
            taskFlowStart.ID = Fun.GetSeqID<YL_TASK_FLOW>();
            taskFlowStart.DEAL_STATUS = "开始";
            taskFlowStart.HANDLE_USER_ID = gu.UserId;
            taskFlowStart.LEVEL_ID = 1;
            taskFlowStart.IS_HANDLE = 1;
            taskFlowStart.NAME = "开始";
            taskFlowStart.TASK_ID = ent.ID;
            taskFlowStart.START_TIME = DateTime.Now;
            taskFlowStart.DEAL_TIME = DateTime.Now;
            taskFlowStart.YL_TASK = ent;
            ent.YL_TASK_FLOW.Add(taskFlowStart);

            var taskFlow0 = new YL_TASK_FLOW();
            taskFlow0.ID = Fun.GetSeqID<YL_TASK_FLOW>();
            taskFlow0.DEAL_STATUS = "创建任务";
            taskFlow0.HANDLE_USER_ID = gu.UserId;
            taskFlow0.LEVEL_ID = 1;
            taskFlow0.IS_HANDLE = 1;
            taskFlow0.NAME = string.Format("{0}创建任务", gu.UserName);
            taskFlow0.TASK_ID = ent.ID;
            taskFlow0.START_TIME = DateTime.Now;
            taskFlow0.DEAL_TIME = DateTime.Now;
            taskFlow0.HANDLE_URL = "~/Task/Handle";
            taskFlow0.SHOW_URL = "~/TaskFlow/Single";
            taskFlow0.YL_TASK = ent;
            taskFlowStart.YL_TASK_FLOW1.Add(taskFlow0);

            taskFlow0.YL_TASK_FLOW_HANDLE.Add(new YL_TASK_FLOW_HANDLE
            {
                ID = Fun.GetSeqID<YL_TASK_FLOW_HANDLE>(),
                CONTENT = content,
                DEAL_TIME = DateTime.Now,
                DEAL_USER_ID = gu.UserId,
                DEAL_USER_NAME = gu.UserName,
                TASK_FLOW_ID = taskFlow0.ID
            });

            ent.YL_TASK_FLOW.Add(taskFlow0);

            if (!string.IsNullOrEmpty(userIdArrStr))
            {
                IList<YL_USER> allUser = new List<YL_USER>();
                if (string.IsNullOrEmpty(userIdArrStr))
                {

                    taskFlow0.ROLE_ID_STR = roleIdStr;
                    IList<int> roleIdArr = roleIdStr.Split(',').Where(x => x.IsInt32()).Select(x => Convert.ToInt32(x)).ToList();
                    var allRoleUser = db.YL_ROLE.Where(x => roleIdArr.Contains(x.ID)).Select(x => x.YL_USER).ToList();
                    foreach (var t in allRoleUser)
                    {
                        allUser = allUser.Union(t).ToList();
                    }
                }
                else
                {
                    IList<int> userIdArr = userIdArrStr.Split(',').Where(x => x.IsInt32()).Select(x => Convert.ToInt32(x)).ToList();
                    allUser = db.YL_USER.Where(x => userIdArr.Contains(x.ID)).ToList();
                    var allRole = db.YL_ROLE.Where(x => x.YL_USER.Where(y => userIdArr.Contains(y.ID)).Count() > 0).ToList();
                    taskFlow0.ROLE_ID_STR = string.Join(",", allRole.Select(x => x.ID).ToList());
                }

                foreach (var user in allUser)
                {
                    YL_TASK_FLOW taskFlowNext1 = new YL_TASK_FLOW();
                    taskFlowNext1.ID = Fun.GetSeqID<YL_TASK_FLOW>();
                    taskFlowNext1.HANDLE_USER_ID = user.ID;
                    taskFlowNext1.LEVEL_ID = taskFlow0.LEVEL_ID + 1;
                    taskFlowNext1.HANDLE_URL = "~/Task/Handle";
                    taskFlowNext1.SHOW_URL = "~/TaskFlow/Single";
                    taskFlowNext1.IS_HANDLE = 0;
                    taskFlowNext1.NAME = string.Format("待{0}处理", string.Join(",", user.YL_ROLE.Select(x => x.NAME)));
                    taskFlowNext1.YL_TASK_FLOW2 = taskFlow0;
                    taskFlowNext1.TASK_ID = ent.ID;
                    taskFlowNext1.START_TIME = DateTime.Now;
                    taskFlowNext1.YL_TASK = ent;
                    taskFlow0.YL_TASK_FLOW1.Add(taskFlowNext1);
                }
            }
            db.YL_TASK.Add(ent);
            return true;
        }

        /// <summary>
        /// 无流程提交
        /// </summary>
        /// <param name="db"></param>
        /// <param name="err"></param>
        /// <param name="gu"></param>
        /// <param name="taskFlowId"></param>
        /// <param name="content"></param>
        /// <param name="allFileStr"></param>
        /// <param name="userIdArrStr"></param>
        /// <param name="roleIdStr"></param>
        /// <param name="IsStage">0表示回复，1表示转派</param>
        /// <param name="butName">转派/回复</param>
        /// <returns></returns>
        public static bool NoFlowSubmit(DBEntities db, ref ErrorInfo err, GlobalUser gu,
            int taskFlowId,
            string content,
            string allFileStr,
            string userIdArrStr,
            string roleIdStr,
            int IsStage,
            string butName)
        {
            if (string.IsNullOrEmpty(content))
            {
                err.IsError = true;
                err.Message = "处理内容不能为空";
                return false;
            }

            var taskFlow = db.YL_TASK_FLOW.SingleOrDefault(x => x.ID == taskFlowId);
            if (taskFlow == null)
            {
                err.IsError = true;
                err.Message = "流程不存在";
                return false;
            }

            switch (butName)
            {
                case "转派":
                    #region 转派
                    if (string.IsNullOrEmpty(userIdArrStr) && string.IsNullOrEmpty(roleIdStr))
                    {
                        err.IsError = true;
                        err.Message = "转派时，转派用户或角色不能为空";
                        return false;
                    }
                    taskFlow.IS_HANDLE = 1;
                    taskFlow.DEAL_STATUS = butName;
                    taskFlow.DEAL_TIME = DateTime.Now;
                    taskFlow.YL_TASK_FLOW_HANDLE.Add(new YL_TASK_FLOW_HANDLE
                    {
                        ID = Fun.GetSeqID<YL_TASK_FLOW_HANDLE>(),
                        CONTENT = content,
                        DEAL_TIME = DateTime.Now,
                        DEAL_USER_ID = gu.UserId,
                        DEAL_USER_NAME = gu.UserName,
                        TASK_FLOW_ID = taskFlow.ID
                    });
                    if (string.IsNullOrEmpty(userIdArrStr))
                    {
                        var roleArr = roleIdStr.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                        var roleList = db.YL_ROLE.Where(x => roleArr.Contains(x.ID)).ToList();

                        taskFlow.NAME = string.Format("转派【{0}】", string.Join(",", roleList.Select(x => x.NAME)));

                        YL_TASK_FLOW taskFlowNext = new YL_TASK_FLOW();
                        taskFlowNext.ID = Fun.GetSeqID<YL_TASK_FLOW>();
                        taskFlowNext.PARENT_ID = (taskFlow.EQUAL_ID == null) ? taskFlow.ID : taskFlow.EQUAL_ID;
                        taskFlowNext.LEVEL_ID = taskFlow.LEVEL_ID + 1;
                        taskFlowNext.IS_HANDLE = 0;
                        taskFlowNext.HANDLE_URL = taskFlow.HANDLE_URL;
                        taskFlowNext.SHOW_URL = taskFlow.SHOW_URL;
                        taskFlowNext.NAME = string.Format("【{0}】处理", string.Join(",", roleList.Select(x => x.NAME)));
                        taskFlowNext.DEAL_STATUS = "待处理";
                        taskFlowNext.YL_TASK_FLOW2 = taskFlow;
                        taskFlowNext.TASK_ID = taskFlow.TASK_ID;
                        taskFlowNext.ROLE_ID_STR = string.Join(",", roleList.Select(x => x.ID).ToList());
                        taskFlowNext.START_TIME = DateTime.Now;
                        db.YL_TASK_FLOW.Add(taskFlowNext);
                    }
                    else
                    {

                        var userrIdArr = userIdArrStr.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                        var userList = db.YL_USER.Where(x => userrIdArr.Contains(x.ID)).ToList();
                        taskFlow.NAME = string.Format("转派【{0}】", string.Join(",", userList.Select(x => x.NAME)));
                        foreach (var user in userList)
                        {
                            YL_TASK_FLOW taskFlowNext = new YL_TASK_FLOW();
                            taskFlowNext.ID = Fun.GetSeqID<YL_TASK_FLOW>();
                            taskFlowNext.PARENT_ID = (taskFlow.EQUAL_ID == null) ? taskFlow.ID : taskFlow.EQUAL_ID;
                            taskFlowNext.LEVEL_ID = taskFlow.LEVEL_ID + 1;
                            taskFlowNext.IS_HANDLE = 0;
                            taskFlowNext.HANDLE_URL = taskFlow.HANDLE_URL;
                            taskFlowNext.SHOW_URL = taskFlow.SHOW_URL;
                            taskFlowNext.NAME = string.Format("【{0}】处理", user.NAME);
                            taskFlowNext.DEAL_STATUS = "待处理";
                            taskFlowNext.YL_TASK_FLOW2 = taskFlow;
                            taskFlowNext.TASK_ID = taskFlow.TASK_ID;
                            taskFlowNext.HANDLE_USER_ID = user.ID;
                            taskFlowNext.ROLE_ID_STR = roleIdStr;
                            taskFlowNext.START_TIME = DateTime.Now;
                            db.YL_TASK_FLOW.Add(taskFlowNext);
                        }
                    }
                    #endregion
                    break;
                case "回复":
                    taskFlow.YL_TASK_FLOW_HANDLE.Add(new YL_TASK_FLOW_HANDLE
                    {
                        ID = Fun.GetSeqID<YL_TASK_FLOW_HANDLE>(),
                        CONTENT = content,
                        DEAL_TIME = DateTime.Now,
                        DEAL_USER_ID = gu.UserId,
                        DEAL_USER_NAME = gu.UserName,
                        TASK_FLOW_ID = taskFlow.ID
                    });


                    if (IsStage == 0)//回复,不是阶段回复
                    {
                        if (taskFlow.YL_TASK_FLOW2 == null)
                        {
                            err.IsError = true;
                            err.Message = "没找到主流程";
                            return false;
                        }

                        if (taskFlow.YL_TASK_FLOW2.PARENT_ID == null)
                        {
                            err.IsError = true;
                            err.Message = "已经是第一级了，不能【回复】。只能【归档】";
                            return false;
                        }
                        taskFlow.NAME = string.Format("【{0}】处理", gu.UserName);

                        taskFlow.IS_HANDLE = 1;
                        taskFlow.DEAL_STATUS = butName;
                        taskFlow.DEAL_TIME = DateTime.Now;
                        //表示还有人已经处理
                        if (taskFlow.YL_TASK_FLOW2.YL_TASK_FLOW1.Where(x => x.IS_HANDLE == 0).Count() == 0)
                        {

                            var taskFlowNext = new YL_TASK_FLOW();
                            taskFlowNext.ID = Fun.GetSeqID<YL_TASK_FLOW>();
                            taskFlowNext.PARENT_ID = taskFlow.YL_TASK_FLOW2.PARENT_ID;
                            taskFlowNext.TASK_ID = taskFlow.YL_TASK_FLOW2.TASK_ID;
                            taskFlowNext.LEVEL_ID = taskFlow.YL_TASK_FLOW2.LEVEL_ID;
                            taskFlowNext.IS_HANDLE = 0;
                            taskFlowNext.NAME = taskFlow.YL_TASK_FLOW2.NAME;
                            taskFlowNext.HANDLE_URL = taskFlow.YL_TASK_FLOW2.HANDLE_URL;
                            taskFlowNext.SHOW_URL = taskFlow.YL_TASK_FLOW2.SHOW_URL;
                            taskFlowNext.START_TIME = DateTime.Now;
                            var nowUser = db.YL_USER.SingleOrDefault(x => x.ID == taskFlow.YL_TASK_FLOW2.HANDLE_USER_ID);
                            taskFlowNext.DEAL_STATUS = "待处理";
                            if (nowUser != null)
                            {
                                taskFlowNext.NAME = string.Format("【{0}】处理", nowUser.NAME);
                            }
                            taskFlowNext.HANDLE_USER_ID = taskFlow.YL_TASK_FLOW2.HANDLE_USER_ID;
                            taskFlowNext.EQUAL_ID = taskFlow.PARENT_ID;
                            db.YL_TASK_FLOW.Add(taskFlowNext);
                        }
                    }
                    else {
                        taskFlow.NAME = string.Format("【{0}】阶段回复", gu.UserName);
                    }
                    break;
                case "归档":
                    taskFlow.IS_HANDLE = 1;
                    taskFlow.DEAL_STATUS = butName;
                    taskFlow.DEAL_TIME = DateTime.Now;

                    YL_TASK_FLOW taskFlowEnd = new YL_TASK_FLOW();
                    taskFlowEnd.ID = Fun.GetSeqID<YL_TASK_FLOW>();
                    taskFlowEnd.PARENT_ID = (taskFlow.EQUAL_ID == null) ? taskFlow.ID : taskFlow.EQUAL_ID;
                    taskFlowEnd.LEVEL_ID = taskFlow.LEVEL_ID + 1;
                    taskFlowEnd.IS_HANDLE = 1;
                    taskFlowEnd.NAME = string.Format("【{0}】归档", gu.UserName);
                    taskFlowEnd.DEAL_STATUS = "归档";
                    taskFlowEnd.TASK_ID = taskFlow.TASK_ID;
                    taskFlowEnd.START_TIME = DateTime.Now;
                    taskFlowEnd.HANDLE_USER_ID = gu.UserId;
                    taskFlowEnd.DEAL_TIME = DateTime.Now;
                    db.YL_TASK_FLOW.Add(taskFlowEnd);
                    break;
                default:
                    err.IsError = true;
                    err.Message = "按钮值不存在";
                    return false;
            }
            taskFlow.YL_TASK.STATUS = butName;
            taskFlow.YL_TASK.STATUS_TIME = DateTime.Now;

            return true;
        }

        /// <summary>
        /// 案件提交
        /// </summary>
        /// <param name="db"></param>
        /// <param name="err"></param>
        /// <param name="gu"></param>
        /// <param name="taskFlowId">任务流程ID</param>
        /// <param name="content">处理内容</param>
        /// <param name="filesIdArrStr">文件ID串</param>
        /// <param name="userIdArrStr">用户ID串</param>
        /// <param name="roleIdArrStr">角色ID串</param>
        /// <param name="IsStage">是否阶段性处理</param>
        /// <param name="butName">按钮名称</param>
        /// <returns></returns>
        public static bool FlowSubmit(DBEntities db, ref ErrorInfo err, GlobalUser gu,
            int taskFlowId,
            string content,
            string filesIdArrStr,
            string userIdArrStr,
            string roleIdArrStr,
            int IsStage,
            string butName)
        {

            YL_TASK_FLOW nowFlow = db.YL_TASK_FLOW.SingleOrDefault(x => x.ID == taskFlowId);
            nowFlow.HANDLE_USER_ID = gu.UserId;

            if (butName.Equals("受理"))
            {
                nowFlow.ACCEPT_TIME = DateTime.Now;
                nowFlow.HANDLE_USER_ID = gu.UserId;
                return true;
            }
            else if (nowFlow.HANDLE_USER_ID != null && nowFlow.HANDLE_USER_ID != gu.UserId)
            {
                err.IsError = true;
                err.Message = "该定单已经被其他人受理了";
                return false;
            }


            if (string.IsNullOrEmpty(content))
            {
                if ("接车,定损,驳回".Split(',').Contains(butName))
                {
                    err.IsError = true;
                    err.Message = "处理内容不能为空";
                    return false;
                }
                content = butName;
            }

            var server = new Service();
            YL_TASK task = nowFlow.YL_TASK;
            YL_USER taskUser = db.YL_USER.SingleOrDefault(x => x.ID == task.CREATE_USER);

            task.STATUS = butName;
            task.STATUS_TIME = DateTime.Now;
            var tmp = new YL_TASK_FLOW_HANDLE(); ;
            tmp.ID = Fun.GetSeqID<YL_TASK_FLOW_HANDLE>();
            tmp.TASK_FLOW_ID = taskFlowId;
            tmp.DEAL_USER_NAME = gu.UserName;
            tmp.DEAL_TIME = DateTime.Now;
            tmp.CONTENT = content;
            tmp.DEAL_USER_ID = gu.UserId;
            if (!string.IsNullOrEmpty(filesIdArrStr))
            {
                var fileIdList = filesIdArrStr.Trim(',').Split(',').Select(x => Convert.ToInt32(x)).ToList();
                tmp.YL_FILES = db.YL_FILES.Where(x => fileIdList.Contains(x.ID)).ToList();
            }
            nowFlow.YL_TASK_FLOW_HANDLE.Add(tmp);

            #region 正常回复

            #region 阶段回复
            if (IsStage == 1)//是阶段回复
            {
                nowFlow.IS_HANDLE = 0;
                nowFlow.DEAL_STATUS = "阶段回复";
                nowFlow.DEAL_TIME = DateTime.Now;
                nowFlow.NAME = string.Format("{0}阶段处理", gu.UserName);
                return true;
            }
            #endregion
            //当前有户可操作的节点
            var allFlownodeId = db.YL_FLOW_FLOWNODE_FLOW.Where(x => x.YL_ROLE.Where(y => gu.RoleID.Contains(y.ID)).Count() > 0).Select(x => x.FROM_FLOWNODE_ID).ToList();
            var nextNode = db.YL_FLOW_FLOWNODE_FLOW.SingleOrDefault(x => x.FROM_FLOWNODE_ID == nowFlow.FLOWNODE_ID && x.FLOW_ID == task.FLOW_ID && x.STATUS == butName);
            if (nextNode == null)
            {
                err.IsError = true;
                err.Message = "没找到下级节点";
                return false;
            }
            int nextFlownodeId = nextNode.TO_FLOWNODE_ID;

            nowFlow.IS_HANDLE = 1;
            nowFlow.DEAL_STATUS = butName;
            nowFlow.DEAL_TIME = DateTime.Now;
            nowFlow.NAME = nowFlow.NAME.Substring(1);
            task.STATUS = nowFlow.DEAL_STATUS;
            task.STATUS_TIME = nowFlow.DEAL_TIME;
            switch (nextNode.HANDLE)
            {
                case 0://一人处理即可
                    foreach (var t in nowFlow.YL_TASK_FLOW2.YL_TASK_FLOW1.ToList())
                    {
                        if (t.ID != nowFlow.ID)//不是现在的流程
                        {
                            t.DEAL_STATUS = "自动处理";
                            t.NAME = string.Format("{0}自动处理", gu.UserName);
                            t.IS_HANDLE = 1;
                        }
                    }
                    break;
                case 1://所有人必须处理
                    foreach (var t in nowFlow.YL_TASK_FLOW2.YL_TASK_FLOW1.ToList())
                    {
                        if (t.ID != nowFlow.ID && t.IS_HANDLE == 0)//不是现在的流程,并且还没有处理
                        {
                            return true;
                        }
                    }
                    break;
            }

            //没指定人，或结束
            if (string.IsNullOrEmpty(userIdArrStr) || nextFlownodeId == 8888 || userIdArrStr.Trim(',') == "")
            {
                #region 没指定人，或结束
                YL_TASK_FLOW endFlow = new YL_TASK_FLOW();
                endFlow.ID = Fun.GetSeqID<YL_TASK_FLOW>();
                endFlow.PARENT_ID = taskFlowId;
                endFlow.TASK_ID = nowFlow.TASK_ID;
                endFlow.LEVEL_ID = nowFlow.LEVEL_ID + 1;
                endFlow.FLOWNODE_ID = nextFlownodeId;
                if (nextFlownodeId == 8888)  //表示是结束
                {
                    #region 结束并发送短信
                    endFlow.IS_HANDLE = 1;
                    endFlow.HANDLE_USER_ID = gu.UserId;
                    endFlow.DEAL_STATUS = "归档";
                    endFlow.DEAL_TIME = DateTime.Now;
                    endFlow.ACCEPT_TIME = DateTime.Now;
                    endFlow.START_TIME = DateTime.Now;
                    endFlow.EXPIRE_TIME = DateTime.Now;
                    endFlow.NAME = string.Format("{0}归档", gu.UserName);
                    db.YL_TASK_FLOW.Add(endFlow);
                    task.STATUS = "完成";

                    if (
                        task.FLOW_ID != null &&
                        (new List<int>() { 1, 2, 3, 4 }).Contains(task.FLOW_ID.Value) &&
                        task.KEY.IsInt32()
                        )
                    {
                        int key = Convert.ToInt32(task.KEY);
                        var order = db.YL_ORDER.SingleOrDefault(x => x.ID == key);
                        if (order != null)
                        {
                            order.PAY_STATUS = "完成";
                            order.PAY_STATUS_TIME = DateTime.Now;
                        }
                    }


                    #region 发送提醒信息
                    //保存信息并用微信发送
                    var msg = string.Format("【云乐享车】您的工单[{0}]已经[{1}]，详细情况请进入系统查询",
                        taskUser.NAME,
                        "已经完成"
                        );

                    List<KV> inPar = new List<KV>();
                    inPar.Add(new KV { V = task.ID.ToString() });
                    server.UserMessageSaveAndSend(
                        gu.Guid,
                        ref err,
                        msg,
                        4,
                        new List<int>() { taskUser.ID },
                        inPar);
                    //表示微信发送失败，则用短信发送
                    if (err.IsError)
                    {
                        err = new ErrorInfo();
                        server.SmsSendOrder(ref err, taskUser.LOGIN_NAME, taskUser.NAME, "已经完成");
                    }

                    #endregion

                    #endregion

                }
                else
                {
                    var allRoleFlow = db.YL_ROLE.Where(x => x.YL_FLOW_FLOWNODE_FLOW.Where(y => y.FROM_FLOWNODE_ID == nextFlownodeId && y.FLOW_ID == task.FLOW_ID).Count() > 0).ToList();
                    var allRoleIdList = allRoleFlow.Select(x => x.ID).ToList();
                    IList<int> nowTaskAllUser = new List<int>();
                    IList<int> nowHandleUserIdList = new List<int>();
                    var startUserId = nowFlow.YL_TASK.YL_TASK_FLOW.SingleOrDefault(x => x.LEVEL_ID == 1).HANDLE_USER_ID;
                    switch (nextNode.ASSIGNER)
                    {
                        //指定角色
                        case 1:
                            nowHandleUserIdList = server.UserGetAllUserById(startUserId.Value, ref err, allRoleFlow.Select(x => x.ID).ToList());
                            nowHandleUserIdList.Add(gu.UserId);
                            nowHandleUserIdList = db.YL_USER.Where(x => nowHandleUserIdList.Contains(x.ID) && x.YL_ROLE.Where(y => allRoleIdList.Contains(y.ID)).Count() > 0).Select(x => x.ID).ToList();
                            break;
                        //发起人
                        case 3:
                            nowHandleUserIdList = new List<int> { startUserId.Value };
                            break;
                        case 4:
                            //所有已经处理过该工单的用户
                            nowHandleUserIdList = db.YL_TASK_FLOW_HANDLE.Where(x => x.YL_TASK_FLOW.TASK_ID == task.ID).Select(x => x.DEAL_USER_ID).ToList();
                            nowHandleUserIdList.Add(gu.UserId);
                            //所有已经处理过该工单的用户,过滤角色
                            nowHandleUserIdList = db.YL_USER.Where(x => nowHandleUserIdList.Contains(x.ID) && x.YL_ROLE.Where(y => allRoleIdList.Contains(y.ID)).Count() > 0).Select(x => x.ID).ToList();
                            break;
                        default:
                            nowHandleUserIdList = server.UserGetAllUserById(startUserId.Value, ref err, allRoleIdList);
                            nowHandleUserIdList.Add(gu.UserId);
                            nowHandleUserIdList = db.YL_USER.Where(x => nowHandleUserIdList.Contains(x.ID) && x.YL_ROLE.Where(y => allRoleIdList.Contains(y.ID)).Count() > 0).Select(x => x.ID).ToList();
                            break;
                    }

                    if (nowHandleUserIdList == null || nowHandleUserIdList.Count() == 0) //上级或没有找到
                    {
                        err.IsError = true;
                        err.Message = "没找到转派后的处理人";
                        return false;
                    }
                    //当是算单时
                    if (butName == "转算单员" && task.FLOW_ID == 1)
                    {
                        Dictionary<int, int> dic = new Dictionary<int, int>();
                        foreach (var t in nowHandleUserIdList)
                        {
                            var count = db.YL_TASK_FLOW.Where(x => x.IS_HANDLE == 0 &&
                             (
                             x.HANDLE_USER_ID == t ||
                             (x.HANDLE_USER_ID == null && x.YL_TASK_FLOW_HANDLE_USER.Where(y => nowHandleUserIdList.Contains(y.HANDLE_USER_ID)).Count() > 0)
                             )
                            ).Count();
                            dic.Add(t, count);
                        }
                        var nowOrder = dic.OrderBy(x => x.Value).ToList();
                        nowHandleUserIdList = new List<int>() { nowOrder[0].Key };
                    }

                    //判断是否重复添加了

                    var flowList = db.YL_TASK_FLOW.Where(x => x.PARENT_ID == taskFlowId && x.TASK_ID == nowFlow.TASK_ID && x.LEVEL_ID == nowFlow.LEVEL_ID + 1 && x.IS_HANDLE == 0).ToList();
                    #region 如果有重复的则进行修改
                    if (flowList.Count() > 0)
                    {
                        foreach (var handleU in nowHandleUserIdList)
                        {
                            var tmpHandleUser = flowList[0].YL_TASK_FLOW_HANDLE_USER.SingleOrDefault(x => x.HANDLE_USER_ID == handleU);
                            if (tmpHandleUser == null)
                            {
                                flowList[0].YL_TASK_FLOW_HANDLE_USER.Add(new YL_TASK_FLOW_HANDLE_USER { HANDLE_USER_ID = handleU });
                            }
                        }
                        flowList[0].IS_HANDLE = 0;
                        return true;
                    }
                    #endregion

                    var node = db.YL_FLOW_FLOWNODE.Single(x => x.ID == nextFlownodeId);
                    endFlow.IS_HANDLE = 0;
                    endFlow.DEAL_STATUS = "待处理";
                    endFlow.HANDLE_URL = node.HANDLE_URL;
                    endFlow.SHOW_URL = node.SHOW_URL;
                    //如为空，则需要受理
                    endFlow.ACCEPT_TIME = DateTime.Now;
                    endFlow.START_TIME = DateTime.Now;
                    endFlow.ROLE_ID_STR = string.Join(",", allRoleFlow.Select(x => x.ID).ToList());
                    endFlow.NAME = string.Format("待{0}处理", string.Join(",", allRoleFlow.Select(x => x.NAME).ToList()));

                    //一人处理即可，只在YL_TASK_FLOW_HANDLE_USER，添加可以操作的人员
                    if (nextNode.HANDLE == 0)
                    {
                        foreach (var handleU in nowHandleUserIdList)
                        {
                            endFlow.YL_TASK_FLOW_HANDLE_USER.Add(new YL_TASK_FLOW_HANDLE_USER { HANDLE_USER_ID = handleU });
                        }
                        db.YL_TASK_FLOW.Add(endFlow);
                    }
                    else //所有人必须处理
                    {
                        foreach (var handleU in nowHandleUserIdList)
                        {
                            var tmpTaskFlow = Fun.ClassToCopy<YL_TASK_FLOW, YL_TASK_FLOW>(endFlow);
                            tmpTaskFlow.ID = Fun.GetSeqID<YL_TASK_FLOW>();
                            tmpTaskFlow.HANDLE_USER_ID = handleU;
                            tmpTaskFlow.YL_TASK_FLOW_HANDLE_USER.Add(new YL_TASK_FLOW_HANDLE_USER { HANDLE_USER_ID = handleU });
                            db.YL_TASK_FLOW.Add(tmpTaskFlow);
                        }
                    }


                    #region 发送提醒信息
                    //保存信息并用微信发送
                    var msg = string.Format("【云乐享车】您的工单[{0}]已经[{1}]，详细情况请进入系统查询",
                        task.TASK_NAME,
                        butName
                        );

                    List<KV> inPar = new List<KV>();
                    inPar.Add(new KV { V = task.ID.ToString() });
                    var userList = db.YL_USER.Where(x => nowHandleUserIdList.Contains(x.ID)).ToList();
                    foreach (var user in userList)
                    {
                        server.UserMessageSaveAndSend(
                        gu.Guid,
                        ref err,
                        msg,
                        4,
                        new List<int> { user.ID },
                        inPar);
                        //表示微信发送失败，则用短信发送
                        if (err.IsError)
                        {
                            err = new ErrorInfo();
                            server.SmsSendOrder(ref err, user.LOGIN_NAME, taskUser.NAME, butName);
                        }
                    }

                    if ( task.FLOW_ID!=null && task.CREATE_USER!=null && !nowHandleUserIdList.Contains(task.CREATE_USER.Value))
                    {
                        var armInt = new List<int> { 2, 3 };
                        //流程在救援和维修的时候，并且创建人不是当前操作的人
                        if (armInt.Contains(task.FLOW_ID.Value) && task.CREATE_USER!=gu.UserId)
                        {
                            var crateUser = db.YL_USER.SingleOrDefault(x => x.ID == task.CREATE_USER);
                            if (crateUser != null)
                            {
                                server.UserMessageSaveAndSend(
                                gu.Guid,
                                ref err,
                                msg,
                                4,
                                new List<int> { crateUser.ID },
                                inPar);
                                //表示微信发送失败，则用短信发送
                                if (err.IsError)
                                {
                                    err = new ErrorInfo();
                                    server.SmsSendOrder(ref err, crateUser.LOGIN_NAME, task.TASK_NAME, butName);
                                }
                            }
                        }
                    }

                    #endregion

                }
                #endregion
            }
            else
            {
                #region 指定人
                var allUserId = userIdArrStr.Trim(',').Split(',').Select(x => Convert.ToInt32(x)).ToList();
                foreach (var t in allUserId)
                {
                    YL_TASK_FLOW endFlow = new YL_TASK_FLOW();
                    endFlow.ID = Fun.GetSeqID<YL_TASK_FLOW>();
                    endFlow.PARENT_ID = taskFlowId;
                    endFlow.TASK_ID = nowFlow.TASK_ID;
                    endFlow.LEVEL_ID = nowFlow.LEVEL_ID + 1;
                    endFlow.FLOWNODE_ID = nextFlownodeId;
                    var node = db.YL_FLOW_FLOWNODE.Single(x => x.ID == nextFlownodeId);
                    endFlow.IS_HANDLE = 0;
                    endFlow.ACCEPT_TIME = DateTime.Now;
                    endFlow.DEAL_STATUS = "待处理";
                    endFlow.NAME = string.Format("待【{0}】处理", node.NAME);
                    endFlow.HANDLE_URL = node.HANDLE_URL;
                    endFlow.SHOW_URL = node.SHOW_URL;
                    endFlow.ROLE_ID_STR = string.Join(",", nextNode.YL_ROLE.Select(x => x.ID).ToList());
                    endFlow.HANDLE_USER_ID = t;
                    endFlow.START_TIME = DateTime.Now;
                    db.YL_TASK_FLOW.Add(endFlow);
                }
                #endregion
            }
            #endregion


            if (
                            task.FLOW_ID != null &&
                            (new List<int>() { 1, 2, 3, 4 }).Contains(task.FLOW_ID.Value) &&
                            task.KEY.IsInt32()
                            )
            {
                int key = Convert.ToInt32(task.KEY);
                var order = db.YL_ORDER.SingleOrDefault(x => x.ID == key);
                order.STATUS = butName;
                switch (task.FLOW_ID)
                {
                    case 1:
                        switch (butName)
                        {
                            case "转算单员":
                                order.STATUS = "待投保";
                                break;
                            case "提交付款":
                                order.STATUS = "待投保";
                                break;
                        }
                        break;
                }
                order.STATUS_TIME = DateTime.Now;
            }

            return true;
        }


        public static TTask TaskSingle(string loginKey, ref ErrorInfo err, int id, int flowId = 0)
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
                if (task == null)
                {
                    task = db.YL_TASK_FLOW.SingleOrDefault(x => x.ID == flowId).YL_TASK;
                }

                if (task == null)
                {
                    err.IsError = true;
                    err.Message = "任务不存在";
                    return null;
                }

                var tmp = Fun.ClassToCopy<YL_TASK, TTask>(task);
                var createUser = db.YL_USER.SingleOrDefault(x => x.ID == task.CREATE_USER);
                #region 计算渠道信息
                //if (!string.IsNullOrEmpty(task.KEY))
                //{
                //    var channelId=task.KEY.Split('|')[0];
                //    var channel = db.YL_CHANNEL_INFO.SingleOrDefault(x=>x.ID==channelId);
                //    var channelOnline = db.YL_CHANNEL_COMPUTER.Where(x => x.CHANNEL_ID == channelId && x.STATUS == "在线").Count();
                //    if (channel != null)
                //    {
                //        tmp.ChanneName = channel.CHANNEL_NAME;
                //        tmp.ChannelAddress = channel.CHANNEL_ADDR;
                //        tmp.ChannelOnLine = string.Format("当前{0}台终端在线", channelOnline);
                //    }
                //}
                //else if (createUser.YL_USER_INFO != null && createUser.YL_USER_INFO.YL_CHANNEL_INFO.Count() > 0)
                //{
                //    var channel = createUser.YL_USER_INFO.YL_CHANNEL_INFO.ToList()[0];
                //    tmp.ChanneName = channel.CHANNEL_NAME;
                //    tmp.ChannelAddress = channel.CHANNEL_ADDR;
                //}
                #endregion
                tmp.CreatePhone = createUser.LOGIN_NAME;


                IList<TTaskFlow> AllFlow = new List<TTaskFlow>();
                foreach (var flow in task.YL_TASK_FLOW.OrderBy(x => x.ID).ToList())
                {
                    if (flow.FLOWNODE_ID == 8888) continue;
                    if (task.CREATE_USER_NAME.Equals("系统派发"))
                    {
                        if (flow.LEVEL_ID < 3) continue;
                    }
                    else
                    {
                        if (flow.LEVEL_ID < 2) continue;
                    }
                    TTaskFlow nowFlow = Fun.ClassToCopy<YL_TASK_FLOW, TTaskFlow>(flow);

                    if (string.IsNullOrEmpty(flow.NAME))
                    {
                        var allHandle = flow.YL_TASK_FLOW_HANDLE.ToList();
                        var allHandleAllUser = flow.YL_TASK_FLOW_HANDLE_USER.ToList();
                        if (allHandle.Count() > 0)
                        {
                            nowFlow.NAME = allHandle[0].DEAL_USER_NAME + "处理";

                        }
                        else if (allHandleAllUser.Count() > 0)
                        {
                            nowFlow.NAME = "待处理";
                        }
                    }

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
                    nowFlow.AllHandleContent = string.Join(";", flow.YL_TASK_FLOW_HANDLE.Select(x => x.CONTENT).ToList());

                    foreach (var handle in flow.YL_TASK_FLOW_HANDLE.ToList())
                    {
                        var tmpHandle = Fun.ClassToCopy<YL_TASK_FLOW_HANDLE, TTaskFlowHandle>(handle);
                        var dealUser = db.YL_USER.SingleOrDefault(x => x.ID == tmpHandle.DEAL_USER_ID);
                        foreach (var file in handle.YL_FILES.ToList())
                        {
                            var tmpFile = Fun.ClassToCopy<YL_FILES, FILES>(file);
                            tmpHandle.AllFiles.Add(tmpFile);
                            nowFlow.AllHandleFiles.Add(tmpFile);
                        }
                        nowFlow.AllHandle.Add(tmpHandle);
                        nowFlow.DealUserName = handle.DEAL_USER_NAME;
                        if (dealUser != null)
                        {
                            nowFlow.DealRole = string.Join(",", dealUser.YL_ROLE.ToList().Select(x => x.NAME).ToArray());

                            var ent = dealUser.YL_DISTRICT;
                            IList<string> idArr = new List<string>();
                            while (ent != null)
                            {
                                idArr.Add(ent.NAME);
                                ent = ent.YL_DISTRICT2;
                            }
                            idArr = idArr.Reverse().ToList();

                            nowFlow.DealUserDistrictName = string.Join(".", idArr);
                            nowFlow.DealUserPhone = dealUser.LOGIN_NAME;

                        }
                    }

                    AllFlow.Add(nowFlow);
                }
                tmp.AllFlow = AllFlow;
                #region 计算所有按钮


                var nowTaskFlowList = task.YL_TASK_FLOW.Where(x =>
                    x.IS_HANDLE == 0 &&
                    (
                        x.HANDLE_USER_ID == gu.UserId ||
                        x.YL_TASK_FLOW_HANDLE_USER.Count(y => y.HANDLE_USER_ID == gu.UserId) < 0 ||
                        (
                            x.FLOWNODE_ID != null && allFlownodeId.Contains(x.FLOWNODE_ID.Value)
                        )
                     )
                ).ToList();
                if (nowTaskFlowList.Count() > 0)
                {
                    var nowTaskFlow = nowTaskFlowList[0];
                    tmp.NowFlowId = nowTaskFlow.ID;
                    if (nowTaskFlow.FLOWNODE_ID != null) //非任务工单
                    {

                        if (nowTaskFlow.ACCEPT_TIME == null)
                        {
                            // tmp.AllButton = new List<string> { "驳回", "受理" };

                            tmp.AllButton = new List<string> { "受理" };
                        }
                        else
                        {
                            tmp.NowNodeFlowId = nowTaskFlow.FLOWNODE_ID.Value;
                            var allBut = db.YL_FLOW_FLOWNODE_FLOW.Where(x =>
                            x.FROM_FLOWNODE_ID == nowTaskFlow.FLOWNODE_ID &&
                            x.FLOW_ID == task.FLOW_ID &&
                            x.YL_ROLE.Where(y => gu.RoleID.Contains(y.ID)).Count() > 0
                            ).ToList();
                            tmp.AllButton = allBut.Select(x => x.STATUS).ToList();
                            //if (task.YL_TASK_FLOW.Count() > 3 && !nowTaskFlow.YL_TASK_FLOW2.DEAL_STATUS.Equals("驳回")) tmp.AllButton.Insert(0, "驳回");
                        }
                    }
                    else {
                        if (nowTaskFlow.YL_TASK_FLOW2.PARENT_ID == null)
                        {
                            tmp.AllButton = new List<string>() { "转派", "归档" };
                        }
                        else
                        {
                            tmp.AllButton = new List<string>() { "转派", "回复" };
                        }
                    }

                }
                #endregion

                return tmp;
            }
        }


        public static TTask TaskGetCreateSingle(string loginKey, ref ErrorInfo err, int flowId)
        {
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            TTask reEnt = new TTask();
            using (DBEntities db = new DBEntities())
            {
                var allFlowNode = db.YL_FLOW_FLOWNODE_FLOW.Where(x => x.FLOW_ID == flowId).ToList();
                var startNode = allFlowNode.SingleOrDefault(x => x.FROM_FLOWNODE_ID == 9999);
                var allNextFlownode = allFlowNode.Where(x => x.FROM_FLOWNODE_ID == startNode.TO_FLOWNODE_ID && x.YL_ROLE.Where(y => gu.RoleID.Contains(y.ID)).Count() > 0).ToList();
                if (allNextFlownode.Count() == 0)
                {
                    err.IsError = true;
                    err.Message = "该用户无权创建工单";
                    return null;
                }
                reEnt.AllButton = allNextFlownode.Select(x => x.STATUS).ToList();
            }
            return reEnt;
        }

        /// <summary>
        /// 更新流程修改,用于其它页面修改后，引发的流程必须包括，现在流程ID,和按钮类型
        /// </summary>
        /// <param name="gu"></param>
        /// <param name="err"></param>
        /// <param name="db"></param>
        /// <param name="taskFlowID"></param>
        /// <param name="nowStatus"></param>
        /// <param name="reMark"></param>
        /// <returns></returns>
        public static bool UpDataTaskFlow(GlobalUser gu, ref ErrorInfo err, DBEntities db, TNode node)
        {

            YL_TASK_FLOW taskFlow = db.YL_TASK_FLOW.SingleOrDefault(x => x.ID == node.ID);
            if (taskFlow == null)
            {
                err.IsError = true;
                err.Message = "程序节点不存在";
                return false;
            }
            //如果已经处理，则跳过
            if (taskFlow.IS_HANDLE == 1)
            {
                return true;
            }
            YL_FLOW flow = taskFlow.YL_TASK.YL_FLOW;
            if (flow != null)
            {

                //终点流程
                IList<YL_FLOW_FLOWNODE_FLOW> endFlow = flow.YL_FLOW_FLOWNODE_FLOW.Where(x => x.TO_FLOWNODE_ID == 8888).ToList();
                //当前流程节点
                YL_FLOW_FLOWNODE flowNode = db.YL_FLOW_FLOWNODE.SingleOrDefault(x => x.ID == taskFlow.FLOWNODE_ID);
                if (string.IsNullOrEmpty(node.NowStatus))
                {
                    node.NowStatus = flow.YL_FLOW_FLOWNODE_FLOW.SingleOrDefault(y => y.FROM_FLOWNODE_ID == flowNode.ID).STATUS;
                }
                //目标节点
                taskFlow.DEAL_STATUS = node.NowStatus;
                taskFlow.HANDLE_USER_ID = gu.UserId;
                taskFlow.IS_HANDLE = 1;

                taskFlow.YL_TASK_FLOW_HANDLE.Add(new YL_TASK_FLOW_HANDLE
                {
                    ID = Fun.GetSeqID<YL_TASK_FLOW_HANDLE>(),
                    CONTENT = node.Remark,
                    DEAL_TIME = DateTime.Now,
                    DEAL_USER_ID = gu.UserId,
                    DEAL_USER_NAME = gu.UserName,
                    TASK_FLOW_ID = taskFlow.ID
                });

                YL_FLOW_FLOWNODE_FLOW flowNodeFlowNext = flow.YL_FLOW_FLOWNODE_FLOW.SingleOrDefault(y => y.FROM_FLOWNODE_ID == flowNode.ID && y.STATUS == node.NowStatus);
                if (flowNodeFlowNext == null) return true;
                //如果是结束节
                if (flowNodeFlowNext.TO_FLOWNODE_ID == 8888)
                {
                    if (endFlow.Count == 0)
                    {
                        node.NowStatus = "完成";
                    }
                    else
                    {
                        node.NowStatus = endFlow[0].STATUS;
                    }
                    taskFlow.YL_TASK.STATUS = node.NowStatus;
                    taskFlow.YL_TASK.STATUS_TIME = DateTime.Now;
                    return true;
                }


                //下步节点
                YL_FLOW_FLOWNODE flowNodeNext = db.YL_FLOW_FLOWNODE.SingleOrDefault(x => x.ID == flowNodeFlowNext.TO_FLOWNODE_ID); ;
                if (flowNodeNext == null)
                {
                    return false;
                }
                taskFlow.HANDLE_URL = flowNode.HANDLE_URL;
                taskFlow.SHOW_URL = flowNode.SHOW_URL;
                taskFlow.NAME = flowNode.NAME;


                //所有人必须处理
                if (flowNodeFlowNext.HANDLE == 1)
                {
                    var noDealNum = taskFlow.YL_TASK_FLOW2.YL_TASK_FLOW1.Where(x => x.IS_HANDLE == 0 && x.ID != flow.ID).Count();
                    //表示还有人没有处理，退出
                    if (noDealNum > 0) return true;
                }
                else if (flowNodeFlowNext.HANDLE == 0)
                {
                    taskFlow.YL_TASK.STATUS = node.NowStatus;
                    taskFlow.YL_TASK.STATUS_TIME = DateTime.Now;

                    string showUrl = flowNodeNext.SHOW_URL;
                    if (flowNodeNext.HANDLE_URL == "~/Task/Handle")
                    {
                        showUrl = string.Format("{0}?id={1}", taskFlow.SHOW_URL, taskFlow.ID);
                    }

                    switch (flowNodeFlowNext.ASSIGNER)
                    {
                        case 0://指定角色
                            if (flowNodeFlowNext.YL_ROLE.Where(x => gu.RoleID.Contains(x.ID)).Count() == 0 && taskFlow.HANDLE_USER_ID != gu.UserId)
                            {
                                err.IsError = true;
                                err.Message = "该用户无操作权限";
                                return false;
                            }
                            YL_TASK_FLOW taskFlowNext = new YL_TASK_FLOW();
                            taskFlowNext.TASK_ID = taskFlow.TASK_ID;
                            taskFlowNext.LEVEL_ID = taskFlow.LEVEL_ID + 1;
                            taskFlowNext.FLOWNODE_ID = flowNodeNext.ID;
                            taskFlowNext.HANDLE_URL = flowNodeNext.HANDLE_URL;
                            taskFlowNext.SHOW_URL = showUrl;
                            taskFlowNext.IS_HANDLE = 0;
                            taskFlowNext.NAME = flowNodeNext.NAME;
                            taskFlowNext.START_TIME = DateTime.Now;

                            taskFlow.YL_TASK_FLOW1.Add(taskFlowNext);
                            break;
                        case 1://指定人
                            var allUserIdList = node.UserIdArrStr.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                            foreach (var t in allUserIdList)
                            {
                                YL_TASK_FLOW temp = new YL_TASK_FLOW();
                                temp.TASK_ID = taskFlow.TASK_ID;
                                temp.LEVEL_ID = taskFlow.LEVEL_ID + 1;
                                temp.FLOWNODE_ID = flowNodeNext.ID;
                                temp.HANDLE_URL = flowNodeNext.HANDLE_URL;
                                temp.SHOW_URL = showUrl;
                                temp.HANDLE_USER_ID = t;
                                temp.IS_HANDLE = 0;
                                temp.NAME = flowNodeNext.NAME;
                                taskFlow.YL_TASK_FLOW1.Add(temp);
                            }
                            break;
                        case 2://返回发起人
                            taskFlowNext = new YL_TASK_FLOW();
                            taskFlowNext.TASK_ID = taskFlow.TASK_ID;
                            taskFlowNext.LEVEL_ID = taskFlow.LEVEL_ID + 1;
                            taskFlowNext.FLOWNODE_ID = flowNodeNext.ID;
                            taskFlowNext.HANDLE_URL = flowNodeNext.HANDLE_URL;
                            taskFlowNext.SHOW_URL = showUrl;
                            taskFlowNext.HANDLE_USER_ID = taskFlow.YL_TASK_FLOW2.HANDLE_USER_ID;
                            taskFlowNext.IS_HANDLE = 0;
                            taskFlowNext.NAME = flowNodeNext.NAME;
                            taskFlowNext.START_TIME = DateTime.Now;
                            taskFlow.YL_TASK_FLOW1.Add(taskFlowNext);
                            break;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 超级管理员更改主流程
        /// </summary>
        /// <param name="gu"></param>
        /// <param name="err"></param>
        /// <param name="db"></param>
        /// <param name="taskFlowID"></param>
        /// <param name="nowFlowNodeID"></param>
        /// <param name="reMark"></param>
        /// <returns></returns>
        public static bool SupUpDataTaskFlow(GlobalUser gu, ref ErrorInfo err, DBEntities db, int taskFlowID, int toFlowNodeID, string reMark)
        {

            YL_TASK_FLOW taskFlow = db.YL_TASK_FLOW.SingleOrDefault(x => x.ID == taskFlowID);
            if (taskFlow.FLOWNODE_ID == toFlowNodeID)
            {
                err.IsError = true;
                err.Message = "不能选择当前所在节点";
                return false;
            }
            //如果已经处理，则跳过
            if (taskFlow.IS_HANDLE == 1)
            {
                return true;
            }

            YL_FLOW flow = taskFlow.YL_TASK.YL_FLOW;
            //流程的终点ID
            int endFlowNodeID = 0;
            #region 查找流程的终点ID
            IList<int> toAr = flow.YL_FLOW_FLOWNODE_FLOW.Select(x => x.TO_FLOWNODE_ID).ToList();
            IList<int> fromAr = flow.YL_FLOW_FLOWNODE_FLOW.Select(x => x.FROM_FLOWNODE_ID).ToList();
            foreach (var t in toAr)
            {
                if (!fromAr.Contains(t))
                {
                    endFlowNodeID = t;
                }
            }
            #endregion
            //终点流程
            IList<YL_FLOW_FLOWNODE_FLOW> endFlow = flow.YL_FLOW_FLOWNODE_FLOW.Where(x => x.TO_FLOWNODE_ID == endFlowNodeID).ToList();

            //YL_FLOW_FLOWNODE flowNode = db.YL_FLOW_FLOWNODE.SingleOrDefault(x => x.ID == taskFlow.FLOWNODE_ID);
            //if (string.IsNullOrEmpty(nowStatus))
            //{
            //    nowStatus = flow.YL_FLOW_FLOWNODE_FLOW.SingleOrDefault(y => y.FROM_FLOWNODE_ID == flowNode.ID).STATUS;
            //}
            //YL_FLOW_FLOWNODE_FLOW flowNodeFlowNext = flow.YL_FLOW_FLOWNODE_FLOW.SingleOrDefault(y => y.FROM_FLOWNODE_ID == flowNode.ID && y.STATUS == nowStatus);
            //if (flowNodeFlowNext.YL_ROLE.Where(x => gu.RoleID.Contains(x.ID)).Count() == 0)
            //{
            //    err.IsError = true;
            //    err.Message = "该用户无操作权限";
            //    return false;
            //}

            //要跳转到的流程节点
            YL_FLOW_FLOWNODE flowNodeNext = null;
            flowNodeNext = db.YL_FLOW_FLOWNODE.SingleOrDefault(x => x.ID == toFlowNodeID);

            if (taskFlow == null)
            {
                err.IsError = true;
                err.Message = "程序节点不存在";
                return false;
            }

            if (taskFlow.IS_HANDLE == 0)
            {
                taskFlow.DEAL_STATUS = "超级管理员处理";
                taskFlow.HANDLE_USER_ID = gu.UserId;
                taskFlow.HANDLE_URL = "~/Task/SupHandle";

                taskFlow.SHOW_URL = "";
                //taskFlow.SHOW_URL = flowNode.SHOW_URL;
                taskFlow.IS_HANDLE = 1;
                taskFlow.NAME = "跳转流程至:" + flowNodeNext.NAME;

                taskFlow.YL_TASK_FLOW_HANDLE.Add(new YL_TASK_FLOW_HANDLE
                {
                    ID = Fun.GetSeqID<YL_TASK_FLOW_HANDLE>(),
                    CONTENT = reMark,
                    DEAL_TIME = DateTime.Now,
                    DEAL_USER_ID = gu.UserId,
                    DEAL_USER_NAME = gu.UserName,
                    TASK_FLOW_ID = taskFlow.ID
                });

            }


            taskFlow.YL_TASK.STATUS = "跳转流程至:" + flowNodeNext.NAME;
            taskFlow.YL_TASK.STATUS_TIME = DateTime.Now;

            if (flowNodeNext != null)
            {
                //如果不是结束节点则添加任务
                if (endFlowNodeID != flowNodeNext.ID)
                {
                    YL_TASK_FLOW taskFlowNext = new YL_TASK_FLOW();
                    taskFlowNext.TASK_ID = taskFlow.TASK_ID;
                    taskFlowNext.LEVEL_ID = taskFlow.LEVEL_ID + 1;
                    taskFlowNext.FLOWNODE_ID = flowNodeNext.ID;
                    taskFlowNext.HANDLE_URL = flowNodeNext.HANDLE_URL;
                    taskFlowNext.SHOW_URL = flowNodeNext.SHOW_URL;
                    taskFlowNext.IS_HANDLE = 0;
                    taskFlowNext.START_TIME = DateTime.Now;
                    taskFlowNext.NAME = flowNodeNext.NAME;
                    taskFlow.YL_TASK_FLOW1.Add(taskFlowNext);
                }
                //是结束节点
                else
                {
                    //if (endFlow.Count == 0)
                    //{
                    //    nowStatus = "完成";
                    //}
                    //else
                    //{
                    //    nowStatus = endFlow[0].STATUS;
                    //}
                }
            }

            return true;
        }


        /// <summary>
        /// 设置公共基本信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ent"></param>
        /// <param name="taskID"></param>
        /// <param name="flownodeId"></param>
        public static void SetFlowBaseInfo(DBEntities db, ProInterface.Models.TNode ent, YL_TASK_FLOW taskFlow)
        {
            SetFlowBaseInfo(db, ent, taskFlow.FLOWNODE_ID, taskFlow.YL_TASK.FLOW_ID, taskFlow.TASK_ID);

        }


        public static void SetFlowBaseInfo(DBEntities db, ProInterface.Models.TNode ent, int? flownodeId, int? flowId, int taskId = 0)
        {

            ent.AllStatus = new List<SelectListItem>();
            if (flownodeId != null && flowId != null)
            {
                foreach (var t in db.YL_FLOW_FLOWNODE_FLOW.Where(x => x.FROM_FLOWNODE_ID == flownodeId && x.FLOW_ID == flowId).ToList())
                {
                    ent.AllStatus.Add(new SelectListItem { Value = t.STATUS, Text = t.STATUS });
                    var allRole = t.YL_ROLE;
                    if (t.ASSIGNER == 1)
                    {
                        if (allRole.Count() == 0)
                        {
                            allRole = db.YL_ROLE.ToList();
                        }
                    }
                    ent.Assigner = t.ASSIGNER;
                    ent.AllRole = allRole.Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.NAME }).ToList();
                }
            }
            else
            {
                ent.AllStatus.Add(new SelectListItem { Text = "完成", Value = "完成" });
                ent.AllStatus.Add(new SelectListItem { Text = "转派", Value = "转派" });
            }
            var task = db.YL_TASK.SingleOrDefault(x => x.ID == taskId);
            if (task != null)
            {
                ent.TaskName = task.TASK_NAME;
            }
            var taskFlow = db.YL_TASK_FLOW.SingleOrDefault(x => x.ID == ent.ID);
            if (taskFlow != null)
            {
                var allFile = new List<FILES>();
                foreach (var t in taskFlow.YL_TASK_FLOW_HANDLE.ToList())
                {
                    foreach (var t0 in t.YL_FILES.ToList())
                    {
                        allFile.Add(Fun.ClassToCopy<YL_FILES, FILES>(t0));
                    }
                }
                ent.AllFilesStr = JSON.DecodeToStr(allFile);
            }
        }

    }
}
