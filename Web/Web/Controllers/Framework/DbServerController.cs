
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class DbServerController : Controller
    {

        public ActionResult Details(int? id)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ProServer.Service db = new ProServer.Service();
            ViewData["dbType"] = db.DbServerType_Where(Fun.UserKey, ref error, 1, 100, null, "ID", null).Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.NAME }).ToList();
            if (id != null)
            {
                var _Ent = db.DbServer_SingleId(Fun.UserKey, ref error, id.Value);
                return View(_Ent);
            }
            return View();
        }


        [HttpPost]
        public ActionResult Details(ProInterface.Models.DB_SERVER ent)
        {

            ProInterface.IZ_DbServer db = new ProServer.Service();
            ProInterface.ErrorInfo error=new ProInterface.ErrorInfo();
            db.DbServer_Save(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());
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
                ProInterface.IZ_DbServer db = new ProServer.Service();
                for (int i = 0; i < idArr.Count(); i++)
                {
                    int _t = 0;
                    try
                    {
                        _t = Convert.ToInt32(idArr[i]);
                        if (db.DbServer_Delete(Fun.UserKey, ref error, _t))
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
