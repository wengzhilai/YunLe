
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
    public partial class Service : IScriptTask
    {

        public bool ScriptTaskReStart(string loginKey, ref ErrorInfo err, int keyId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return false;
            }
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_SCRIPT_TASK.SingleOrDefault(x => x.ID == keyId);
                ent.RUN_STATE = "等待";
                ent.DSL_TYPE = "手动重启";
                db.SaveChanges();
                return true;
            }
        }

        public bool ScriptTaskCancel(string loginKey, ref ErrorInfo err, int keyId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return false;
            }
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_SCRIPT_TASK.SingleOrDefault(x => x.ID == keyId);
                ent.RUN_STATE = "手动停止";
                db.SaveChanges();
                return true;
            }
        }

        public string ScriptTaskLookLog(string loginKey, ref ErrorInfo err, int keyId)
        {
            using (DBEntities db = new DBEntities())
            {
                var allLog = db.YL_SCRIPT_TASK_LOG.Where(x => x.SCRIPT_TASK_ID == keyId).OrderByDescending(x => x.ID).ToList();
                return string.Join("\r\n", allLog.Select(x => x.LOG_TIME.ToString() + "  " + x.MESSAGE).ToList());
            }
        }

        public string ScriptTaskLookScript(string loginKey, ref ErrorInfo err, int keyId)
        {
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_SCRIPT_TASK.SingleOrDefault(x => x.ID == keyId);
                return ent.BODY_TEXT;
            }
        }


        public bool ScriptTaskAdd(string loginKey, ref ErrorInfo err, int scriptId,int? groupId=null)
        {
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_SCRIPT.SingleOrDefault(x => x.ID == scriptId);

                if (ent.IS_GROUP == 0)
                {
                    var nowRunTaskNum = db.YL_SCRIPT_TASK.Where(x => x.SCRIPT_ID == ent.ID && x.RUN_STATE.Equals("等待")).Count();
                    if (nowRunTaskNum > 0)
                    {
                        err.IsError = true;
                        err.Message = string.Format("当前已经有等待任务【{0}】个", nowRunTaskNum);
                        return false;
                    }
                    var task = Fun.ClassToCopy<YL_SCRIPT, YL_SCRIPT_TASK>(ent);
                    task.ID = 0;
                    task.SCRIPT_ID = ent.ID;
                    task.RUN_STATE = "等待";
                    task.DSL_TYPE = "手工添加";
                    task.GROUP_ID = groupId;
                    db.YL_SCRIPT_TASK.Add(task);
                    db.SaveChanges();
                }
                else
                {
                    var allList = ent.YL_SCRIPT_GROUP_LIST.OrderBy(x => x.ORDER_INDEX).ToList();
                    if (allList.Count() > 0)
                    {
                        return ScriptTaskAdd(loginKey, ref err, allList[0].SCRIPT_ID, scriptId);
                    }
                }
                return true;
            }
        }
    }
}
