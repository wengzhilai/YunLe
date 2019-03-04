using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class FlowController : Controller
    {
        //
        // GET: /Flow/

        public ActionResult MakeFlow(int? id=0)
        {
            ProInterface.IFlow db = new ProServer.Service();
            ViewData["FlownodeAll"] = db.FlowAllFlownode();
            ViewData["AllRole"] = db.FlowAllRole(Fun.UserKey, ref Fun.Err);
            var ent = db.FlowSingle(Fun.UserKey, ref Fun.Err, id);
            return View(ent);
        }

        [HttpPost]
        public ActionResult MakeFlow(ProInterface.Models.TFlow ent)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ProInterface.IFlow db = new ProServer.Service();
            var reBool = db.FlowSave(Fun.UserKey, ref Fun.Err, ent);
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

        public ActionResult GetFristNode(int flowId, string status)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ProServer.Service db = new ProServer.Service();
            var ent = db.FlowFristNode(Fun.UserKey, ref error, flowId, status);
            if (error.IsError)
            {
                return Json(error,JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(ent, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetNodeFlow(int flowId, int fromFlownodeId, string statusName)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ProInterface.IFlow db = new ProServer.Service();
            var ent = db.FlowGetNodeFlow(Fun.UserKey, ref error, flowId, fromFlownodeId, statusName);
            if (error.IsError)
            {
                return Json(error, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(ent, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
