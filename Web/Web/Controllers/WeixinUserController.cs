
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class WeixinUserController : Controller
    {
        public ActionResult GetProperDescription(string proName)
        {
            return Json(ProServer.Fun.GetClassProperDescription<ProInterface.Models.YL_WEIXIN_USER>(proName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(string id)
        {
            if (id != null)
            {
                ProInterface.IZ_WeixinUser db = new ProServer.Service();
                ProInterface.ErrorInfo error=new ProInterface.ErrorInfo();
                var _Ent = db.WeixinUser_SingleId(Fun.UserKey, ref error, id);
                return View(_Ent);
            }
            return View();
        }


        [HttpPost]
        public ActionResult Details(ProInterface.Models.YL_WEIXIN_USER ent)
        {

            ProInterface.IZ_WeixinUser db = new ProServer.Service();
            ProInterface.ErrorInfo error=new ProInterface.ErrorInfo();
            db.WeixinUser_Save(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());
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
                ProInterface.IZ_WeixinUser db = new ProServer.Service();
                for (int i = 0; i < idArr.Count(); i++)
                {
                    try
                    {
                        if (db.WeixinUser_Delete(Fun.UserKey, ref error, idArr[i]))
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
