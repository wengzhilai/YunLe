using ProInterface;
using ProInterface.Models;
using ProServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class TaskController : Controller
    {

        public ActionResult GetTreeFlow(int id)
        {
            ProInterface.ITask db = new ProServer.Service();
            var ent = db.TaskGetTreeFlow(Fun.UserKey, ref Fun.Err, id);
            var reStr = JSON.DecodeToStr(ent);
            return Content(reStr);
        }


        public ActionResult WaitTask()
        {
            return View();
        }

        public ActionResult Find(int id)
        {
            ErrorInfo error = new ErrorInfo();
            var ttask = FunTask.TaskSingle(Fun.UserKey, ref error, id);
            return View(ttask);
        }

        public ActionResult HandleNode(int id)
        {
            ProInterface.ITask db = new ProServer.Service();
            var ent = db.TaskFlowSingle(id);
            if (ent.HANDLE_URL.IndexOf("?") > 0)
            {
                ent.HANDLE_URL += "&id=" + ent.ID;
            }
            else
            {
                ent.HANDLE_URL += "?id=" + ent.ID;
            }
            return View(ent);
        }

        public ActionResult LogReport(int id)
        {
            var ent = new ProInterface.Models.TTaskFlow();
            ent.TASK_ID = id;
            return View(ent);
        }


        public ActionResult LookFlow(int id, int? nodeid = null)
        {
            ProInterface.ITask db = new ProServer.Service();
            var ent = db.TaskGetTaskFlow(Fun.UserKey, ref Fun.Err, id);
            ViewData["FlownodeAll"] = ent.AllFlownode;
            ErrorInfo error = new ErrorInfo();
            var task = FunTask.TaskSingle(Fun.UserKey, ref error, id);
            ViewData["UseNode"] = JSON.DecodeToStr(task.AllFlow.Select(x => new
            {
                level = x.LEVEL_ID,
                nodeid = x.FLOWNODE_ID,
                ishandle = x.IS_HANDLE,
                url = x.SHOW_URL,
                name = x.NAME,
                flowId = x.ID
            }).ToList());

            return View(ent);
        }

        public ActionResult LookNowFlow(int id)
        {
            ErrorInfo error = new ErrorInfo();
            var task = FunTask.TaskSingle(Fun.UserKey, ref error, id);
            var allUseFlow = task.AllFlow.Where(x => x.EQUAL_ID == null).ToList();
            ViewData["UseNode"] = allUseFlow;

            var normalFlow = allUseFlow.Where(x=>x.PARENT_ID!=null).Select(x => new ProInterface.Models.FLOW_FLOWNODE_FLOW()
            {
                FROM_FLOWNODE_ID = x.PARENT_ID.Value,
                TO_FLOWNODE_ID = x.ID,
                STATUS = "转派"
            }).ToList();

            var rePlayFlow = task.AllFlow.Where(x => x.PARENT_ID != null && x.FLOWNODE_ID == null && x.IS_HANDLE == 1 && x.DEAL_STATUS.Equals("回复")).Select(x => new ProInterface.Models.FLOW_FLOWNODE_FLOW()
            {
                FROM_FLOWNODE_ID = (x.EQUAL_ID==null)? x.ID:x.EQUAL_ID.Value,
                TO_FLOWNODE_ID = x.PARENT_ID.Value,
                STATUS ="回复"
            }).ToList();
            foreach (var t in rePlayFlow) normalFlow.Add(t);
            ViewData["FlowListStr"] = JSON.DecodeToStr(normalFlow);
            return View(task);
        }

        public ActionResult Handle(int id, int? nodeid = null)
        {
            ErrorInfo error = new ErrorInfo();
            var ttask = FunTask.TaskSingle(Fun.UserKey, ref error, id);
            if (error.IsError)
            {
                return null;
            }
            if(ttask.AllFlow.Count()>0)
            {
                var last = ttask.AllFlow[ttask.AllFlow.Count() - 1];
                ViewData["AllRole"] = last.RoleList.Select(x => new SelectListItem() {  Value=x.K,Text=x.V,Selected=true}).ToList();
            }
            return View(ttask);
        }

        [HttpPost]
        public ActionResult Handle(ProInterface.Models.TTask ent)
        {
            ProInterface.ITask db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            db.TaskHandle(Fun.UserKey, ref error, ent);
            if (error.IsError)
            {
                return Json(error);
            }
            else
            {
                error.Message = "保存成功";
                return Json(error);
            }
        }

        public ActionResult JudeHandle(int id, int flow_id)
        {
            ProInterface.ITask db = new ProServer.Service();
            var ent = new ProInterface.Models.TNodeFlow();
            if (flow_id != 0)
            {
                ent = db.TaskNodeFlowSingle(Fun.UserKey, ref Fun.Err, id);
            }
            else
            {
                ent.ID = id;
                ent.FlowID = 0;
            }
            return View(ent);
        }
        //超级管理员 流程退回
        public ActionResult SupHandleNode(int id)
        {
            ProInterface.ITask db = new ProServer.Service();
            var ent = db.TaskFlowSingle(id);
            if (ent.HANDLE_URL.IndexOf("?") > 0)
            {
                ent.HANDLE_URL = "~/Task/SupHandle&id=" + ent.ID;
            }
            else
            {
                ent.HANDLE_URL = "~/Task/SupHandle?id=" + ent.ID;
            }
            ent.NAME = "流程调整";
            return View(ent);
        }
        //超级管理员 退回页
        public ActionResult SupHandle(int id)
        {
            ProInterface.ITask db = new ProServer.Service();

            var ent = db.TaskNodeSingle(Fun.UserKey, ref Fun.Err, id);
            return View(ent);
        }
        [HttpPost]
        public ActionResult SupHandle(ProInterface.Models.TNode ent)
        {
            ProInterface.ITask db = new ProServer.Service();
            Fun.Err = new ProInterface.ErrorInfo();
            if (db.SupTaskNodeSave(Fun.UserKey, ref Fun.Err, ent))
            {
                return JavaScript("alert('保存成功')");
            }
            else
            {
                return JavaScript("alert('失败\\r\\n" + Fun.Err.Message + "')");
            }
        }
        public ActionResult GetProperDescription(string proName)
        {
            return Json(ProServer.Fun.GetClassProperDescription<ProInterface.Models.TASK>(proName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int? id,int? flowId)
        {
            ProServer.Service db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            IList<SelectListItem> allFlow = new List<SelectListItem>();
            if (flowId != null)
            {
                allFlow = db.FlowAllMy(Fun.UserKey, ref error);
                allFlow = allFlow.Where(x => x.Value == flowId.ToString()).ToList();
                if (allFlow.Count() == 0)
                {
                    return Content("您没有限制创建该工单");
                }
            }
            else
            {
                allFlow.Add(new SelectListItem { Value = "0", Text = "任务工单" });
            }
            ViewData["FLOW_ID"] = allFlow;
            ViewData["AllRole"] = db.TaskFlowMyAllRole(Fun.UserKey, ref error);
            if (!string.IsNullOrEmpty(Request["Type"]))
            {
                var data = db.DbServerDataTable(1, string.Format("SELECT ID,NAME FROM {0}", Request["Type"]));
                IList<System.Web.Mvc.SelectListItem> allKey = new List<System.Web.Mvc.SelectListItem>();
                for (var i = 0; i < data.Rows.Count; i++)
                {
                    allKey.Add(new SelectListItem { Value = data.Rows[i]["ID"].ToString(), Text = data.Rows[i]["NAME"].ToString() });
                }
                ViewData["KEY"] = allKey;
            }
            TTask _Ent = new TTask();
            if (id != null)
            {
                _Ent = FunTask.TaskSingle(Fun.UserKey, ref error, id.Value);
            }
            else if(flowId!=null)
            {
                
                _Ent = FunTask.TaskGetCreateSingle(Fun.UserKey, ref error, flowId.Value);
            }
            return View(_Ent);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Details(ProInterface.Models.TTask ent)
        {

            ProInterface.ITask db = new ProServer.Service();

            ent.UserIdArrStr = Request["UserIdArrStr"];
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            db.TaskSave(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());
            if (error.IsError)
            {
                return Json(error);
            }
            else
            {
                error.Message = "保存成功";
                return Json(error);
            }
        }

        public ActionResult Delete(string id)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            if (id != null)
            {
                int fail = 0, succ = 0;
                string[] idArr = id.Split(',');
                ProInterface.ITask db = new ProServer.Service();
                for (int i = 0; i < idArr.Count(); i++)
                {
                    int _t = 0;
                    try
                    {
                        _t = Convert.ToInt32(idArr[i]);
                        if (db.TaskDelete(Fun.UserKey, ref error, _t))
                        {
                            succ++;
                        }
                        else
                        {
                            fail++;
                        }
                    }
                    catch { continue; }
                }
                error.Message = "删除成功[" + succ + "]个\\r\\n删除失败[" + fail + "]个";
            }
            else
            {
                error.Message = "删除失败";
            }
            return Json(error, JsonRequestBehavior.AllowGet);
        }

    }
}
