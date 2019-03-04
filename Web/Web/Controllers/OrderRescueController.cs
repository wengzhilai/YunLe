
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class OrderRescueController : Controller
    {
        public ActionResult GetProperDescription(string proName)
        {
            return Json(ProServer.Fun.GetClassProperDescription<ProInterface.Models.YL_ORDER_RESCUE>(proName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int? id)
        {
            ProServer.Service db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ViewData["AllGarage"] = db.Garage_FindAll(Fun.UserKey, ref error).Select(x => new SelectListItem { Value=x.ID.ToString(),Text=x.NAME }).ToList();
            if (id != null)
            {
                var _Ent = db.OrderRescueSingleId(Fun.UserKey, ref error, id.Value);
                return View(_Ent);
            }
            return View();
        }

        public ActionResult Single(int? id)
        {
            ProServer.Service db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ViewData["AllGarage"] = db.Garage_FindAll(Fun.UserKey, ref error).Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.NAME }).ToList();
            if (id != null)
            {
                var _Ent = db.OrderRescueSingleId(Fun.UserKey, ref error, id.Value);
                return View(_Ent);
            }
            return View();
        }

        [HttpPost]
        public ActionResult Details(ProInterface.Models.YlOrderRescue ent)
        {

            ProInterface.IOrderRescue db = new ProServer.Service();
            ProInterface.ErrorInfo error=new ProInterface.ErrorInfo();
            db.OrderRescueSave(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());
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
                ProInterface.IOrder db = new ProServer.Service();
                for (int i = 0; i < idArr.Count(); i++)
                {
                    int _t = 0;
                    try
                    {
                        _t = Convert.ToInt32(idArr[i]);
                        if (db.OrderDelete(Fun.UserKey, ref error, _t))
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
