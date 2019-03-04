
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class ScriptController : Controller
    {

        public ActionResult GroupDetails(int? id)
        {
            ProServer.Service db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            if (id != null)
            {
                var _Ent = db.ScriptSingleId(Fun.UserKey, ref error, id.Value);
                return View(_Ent);
            }
            else {
                id = 0;
                var _Ent = db.ScriptSingleId(Fun.UserKey, ref error, id.Value);
                return View(_Ent);
            }
            
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GroupDetails(ProInterface.Models.TScript ent)
        {

            ProInterface.IScript db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ent.IS_GROUP = 1;
            ent.BODY_TEXT = "无";
            ent.BODY_HASH = ent.BODY_TEXT.Md5();
            db.ScriptSave(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());
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

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ScriptTest(string code)
        {
            ProInterface.IScript db = new ProServer.Service();
            var msg = db.ScriptTest(code);
            if (string.IsNullOrEmpty(msg)) msg = "语法通过";
            return Content(msg);
        }


        public ActionResult ScriptStart(int id, string database)
        {

            ProInterface.IScriptTask db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            var isPass = db.ScriptTaskAdd(Fun.UserKey, ref error, id);
            if (error.IsError)
            {
                error.Message = "启动失败";
            }
            else
            {
                error.Message = "启动成功";
            }
            return Json(error, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ScriptCancel(int id)
        {

            ProInterface.IScript db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            var isTure = db.ScriptCancel(ref error, id);

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


        public ActionResult Operate(int id)
        {
            ViewData["reMark"] = ProServer.Fun.GetClassDescription<ProServer.ScriptExt>();

            ProServer.Service db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ViewData["db"] = db.DbServer_Where(Fun.UserKey, ref error, 1, 100, null, "ID", null).Select(x => new SelectListItem { Value = x.NICKNAME, Text = x.NICKNAME }).ToList();
            var _Ent = db.Script_SingleId(Fun.UserKey, ref error, id);
            return View(_Ent);
        }

        public ActionResult GetProperDescription(string proName)
        {
            return Json(ProServer.Fun.GetClassProperDescription<ProInterface.Models.SCRIPT>(proName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LookLog(int scriptId)
        {
            ProInterface.IScript db = new ProServer.Service();
            ViewData["log"] = db.ScriptRunLog(scriptId);
            return View();
        }

        public ActionResult Details(int? id)
        {
            ProServer.Service db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ViewData["db"] = db.DbServer_Where(Fun.UserKey, ref error, 1, 100, null, "ID", null).Select(x => new SelectListItem { Value = x.NICKNAME, Text = x.NICKNAME }).ToList();
            if (id != null)
            {
                var _Ent = db.ScriptSingleId(Fun.UserKey, ref error, id.Value);
                return View(_Ent);
            }
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Details(ProInterface.Models.TScript ent)
        {

            ProInterface.IScript db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ent.BODY_HASH = ent.BODY_TEXT.Md5();
            db.ScriptSave(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());
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
                ProInterface.IScript db = new ProServer.Service();
                for (int i = 0; i < idArr.Count(); i++)
                {
                    int _t = 0;
                    try
                    {
                        _t = Convert.ToInt32(idArr[i]);
                        if (db.ScriptDelete(Fun.UserKey, ref error, _t))
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
