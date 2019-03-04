
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class ScriptTaskController : Controller
    {
        public ActionResult Cancel(int id)
        {
            ProInterface.IScriptTask db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            var isTure = db.ScriptTaskCancel(Fun.UserKey, ref error, id);

            if (error.IsError)
            {
                return Json(error, JsonRequestBehavior.AllowGet);
            }
            else
            {
                error.Message = "成功";
                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ReStart(int id)
        {
            ProInterface.IScriptTask db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            var isTure = db.ScriptTaskReStart(Fun.UserKey, ref error, id);

            if (error.IsError)
            {
                return Json(error, JsonRequestBehavior.AllowGet);
            }
            else
            {
                error.Message = "成功";
                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LookLog(int id)
        {
            ProInterface.IScriptTask db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            return ShowText(db.ScriptTaskLookLog(Fun.UserKey, ref error, id));
        }

        public ActionResult LookScript(int id)
        {
            ProInterface.IScriptTask db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            return ShowText(db.ScriptTaskLookScript(Fun.UserKey, ref error, id));
        }

        public ActionResult ShowText(string content)
        {
            ViewData["log"] = content;
            return View();
        }




        public ActionResult GetProperDescription(string proName)
        {
            return Json(ProServer.Fun.GetClassProperDescription<ProInterface.Models.SCRIPT_TASK>(proName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int? id)
        {
            if (id != null)
            {
                ProInterface.IZ_ScriptTask db = new ProServer.Service();
                ProInterface.ErrorInfo error=new ProInterface.ErrorInfo();
                var _Ent = db.ScriptTask_SingleId(Fun.UserKey, ref error, id.Value);
                return View(_Ent);
            }
            return View();
        }


        [HttpPost]
        public ActionResult Details(ProInterface.Models.SCRIPT_TASK ent)
        {

            ProInterface.IZ_ScriptTask db = new ProServer.Service();
            ProInterface.ErrorInfo error=new ProInterface.ErrorInfo();
            db.ScriptTask_Save(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());
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
                ProInterface.IZ_ScriptTask db = new ProServer.Service();
                for (int i = 0; i < idArr.Count(); i++)
                {
                    int _t = 0;
                    try
                    {
                        _t = Convert.ToInt32(idArr[i]);
                        if (db.ScriptTask_Delete(Fun.UserKey, ref error, _t))
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
                error.Message= "删除成功[" + succ + "]个\\r\\n删除失败[" + fail + "]个";
            }
            else {
                error.Message = "删除失败";
            }
            return Json(error,JsonRequestBehavior.AllowGet);
        }

    }
}
