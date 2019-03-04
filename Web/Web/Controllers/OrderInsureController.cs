
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class OrderInsureController : Controller
    {
        public ActionResult GetProperDescription(string proName)
        {
            return Json(ProServer.Fun.GetClassProperDescription<ProInterface.Models.YL_ORDER_INSURE>(proName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Import(string proName)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Import(int FILE_ID)
        {

            ProInterface.IOrderInsure db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            db.OrderInsureAddFromExcel(Fun.UserKey, ref error, FILE_ID);
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


        public ActionResult Single(int? id)
        {
            ProServer.Service db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            var insurerAll = db.Insurer_FindAll(Fun.UserKey, ref error).Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.NAME }).ToList();
            ViewData["insurerAll"] = insurerAll;

            if (id != null)
            {

                var _Ent = db.OrderInsureSingleId(Fun.UserKey, ref error, id.Value);
                return View(_Ent);
            }
            return View();
        }

        public ActionResult Details(int? id)
        {
            ProServer.Service db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            var insurerAll= db.Insurer_FindAll(Fun.UserKey, ref error).Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.NAME }).ToList();
            ViewData["insurerAll"] = insurerAll;

            if (id != null)
            {
                
                var _Ent = db.OrderInsureSingleId(Fun.UserKey, ref error, id.Value);
                return View(_Ent);
            }
            return View();
        }


        [HttpPost]
        public ActionResult Details(ProInterface.Models.YlOrderInsure ent)
        {

            ProInterface.IOrderInsure db = new ProServer.Service();
            ProInterface.ErrorInfo error=new ProInterface.ErrorInfo();
            db.OrderInsureSave(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());
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
                error.Message = "删除成功[" + succ + "]个\\r\\n删除失败[" + fail + "]个";
            }
            else {
                error.Message = "删除失败";
            }
            return Json(error, JsonRequestBehavior.AllowGet);
        }

    }
}
