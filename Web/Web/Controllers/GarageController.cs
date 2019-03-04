
using ProInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class GarageController : Controller
    {

        public ActionResult GarageUser(int? id)
        {
            ProServer.Service db = new ProServer.Service();
            ErrorInfo error = new ErrorInfo();
            var allGarage = db.GarageFindAllMy(Fun.UserKey, ref error).Select(x => new SelectListItem { Text = x.NAME, Value = x.ID.ToString(), Selected = false }).ToList();
            ViewData["list"] = allGarage;
            if (id != null)
            {
                var _Ent = db.SalesmanSingleId(Fun.UserKey, ref error, id.Value);
                return View(_Ent);
            }
            return View();
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="ent">模块类</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GarageUser(ProInterface.Models.YlSalesman ent)
        {
            ProServer.Service db = new ProServer.Service();
            ErrorInfo error = new ErrorInfo();
            var reEnt= db.GarageUserSave(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());

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

        public ActionResult GetProperDescription(string proName)
        {
            return Json(ProServer.Fun.GetClassProperDescription<ProInterface.Models.YL_GARAGE>(proName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int? id)
        {
            if (id != null)
            {
                ProInterface.IGarage db = new ProServer.Service();
                ProInterface.ErrorInfo error=new ProInterface.ErrorInfo();
                var _Ent = db.GarageSingleId(Fun.UserKey, ref error, id.Value);
                return View(_Ent);
            }
            return View();
        }


        [HttpPost]
        public ActionResult Details(ProInterface.Models.YlGarage ent)
        {

            ProInterface.IGarage db = new ProServer.Service();
            ProInterface.ErrorInfo error=new ProInterface.ErrorInfo();
            db.GarageSave(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());
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
                ProInterface.IZ_Garage db = new ProServer.Service();
                for (int i = 0; i < idArr.Count(); i++)
                {
                    int _t = 0;
                    try
                    {
                        _t = Convert.ToInt32(idArr[i]);
                        if (db.Garage_Delete(Fun.UserKey, ref error, _t))
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

        public ActionResult GarageUserDelete(string id)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            if (id != null)
            {
                int fail = 0, succ = 0;
                string[] idArr = id.Split(',');
                ProInterface.IGarage db = new ProServer.Service();
                for (int i = 0; i < idArr.Count(); i++)
                {
                    int _t = 0;
                    try
                    {
                        _t = Convert.ToInt32(idArr[i]);
                        if (db.GarageUserDelete(Fun.UserKey, ref error, _t))
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
