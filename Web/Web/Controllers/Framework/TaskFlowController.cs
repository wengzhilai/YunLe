using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class TaskFlowController : Controller
    {

        public ActionResult Accept(string id)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            if (!string.IsNullOrEmpty(id))
            {
                int fail = 0, succ = 0;
                ProInterface.ITaskFlow db = new ProServer.Service();
                succ=db.TaskFlowAccept(Fun.UserKey,ref error,id);

                error.Message = "受理成功[" + succ + "]个\\r\\n受理失败[" + fail + "]个";
            }
            else {
                error.Message = "受理失败";
            }
            return Json(error,JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /TaskFlow/
        public ActionResult Handle(int id)
        {
            ProServer.Service db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            var allFlow = db.FlowAllMy(Fun.UserKey, ref error);
            allFlow.Insert(0, new SelectListItem { Value = "", Text = "任务工单" });
            ViewData["FLOW_ID"] = allFlow;
            ViewData["AllRole"] = db.TaskFlowMyAllRole(Fun.UserKey, ref error);
            var ent = db.TaskFlowSingleId(Fun.UserKey, ref Fun.Err, id);
            return View(ent);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Handle(ProInterface.Models.TTaskFlow ent)
        {

            ProInterface.ITaskFlow db = new ProServer.Service();

            ent.UserIdArrStr = Request["UserIdArrStr"];

            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            db.TaskFlowSave(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());
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


        public ActionResult Single(int id)
        {
            ProServer.Service db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            var _Ent = db.TaskSingleId(Fun.UserKey, ref error, id);
            if (error.IsError)
            {
                return Content(error.Message);
            }
            return View(_Ent);
        }

    }
}
