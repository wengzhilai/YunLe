
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class ConfigController : Controller
    {

        public ActionResult Details(int? id)
        {
            IList<ProInterface.Models.KV> configCode=ProInterface.JSON.EncodeToEntity<IList<ProInterface.Models.KV>>( ProInterface.AppSet.ConfigCode);

            ViewData["CODE"] = configCode.Select(x => new SelectListItem { Value=x.K, Text=x.V });

            ProInterface.Models.CONFIG _Ent = new ProInterface.Models.CONFIG();
            if (id != null)
            {
                ProInterface.IZ_Config db = new ProServer.Service();
                ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
                _Ent = db.Config_SingleId(Fun.UserKey, ref error, id.Value);
            }
            else
            {
                _Ent.TYPE = "码表";
            }
            return View(_Ent);
        }


        [HttpPost]
        public ActionResult Details(ProInterface.Models.CONFIG ent)
        {
            ProInterface.IZ_Config db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            db.Config_Save(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());
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
                ProInterface.IZ_Config db = new ProServer.Service();
                for (int i = 0; i < idArr.Count(); i++)
                {
                    int _t = 0;
                    try
                    {
                        _t = Convert.ToInt32(idArr[i]);
                        if (db.Config_Delete(Fun.UserKey, ref error, _t))
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
