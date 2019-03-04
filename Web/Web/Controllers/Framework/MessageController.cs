
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class MessageController : Controller
    {


        public ActionResult GetProperDescription(string proName)
        {
            return Json(ProServer.Fun.GetClassProperDescription<ProInterface.Models.MESSAGE>(proName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int? id, string authToken)
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                Fun.UserKey = authToken;
            }
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ProServer.Service db = new ProServer.Service();
            

            ViewData["AllRole"] = db.Role_Where(Fun.UserKey, ref error, 1, 100, null, "ID", null).Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.NAME }).ToList();
            ProInterface.Models.MESSAGE _Ent = new ProInterface.Models.MESSAGE();
            if (id != null)
            {
                _Ent = db.Message_SingleId(Fun.UserKey, ref error, id.Value);
            }
            else
            {
                if (Request["MESSAGE_TYPE_ID"] != null)
                {
                    _Ent.MESSAGE_TYPE_ID =Convert.ToInt32(Request["MESSAGE_TYPE_ID"]);
                }
                if (Request["KEY"] != null)
                {
                    _Ent.KEY = Convert.ToInt32(Request["KEY"]);
                }
                if (Request["CONTENT"] != null || Request["PUBLISHER"]!=null)
                {
                    _Ent.CONTENT = string.Format("有新信息：{0}【{1}】", Request["CONTENT"], Request["PUBLISHER"]);
                }

                var user= ProServer.Global.GetUser(Fun.UserKey);
                if (user != null)
                {
                    _Ent.DISTRICT_ID = user.DistrictId;
                }
            }
            return View(_Ent);
        }


        [HttpPost]
        public ActionResult Details(ProInterface.Models.MESSAGE ent)
        {

            ProInterface.IUserMessage db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            db.UserMessageSave(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());
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
                ProInterface.IZ_Message db = new ProServer.Service();
                for (int i = 0; i < idArr.Count(); i++)
                {
                    int _t = 0;
                    try
                    {
                        _t = Convert.ToInt32(idArr[i]);
                        if (db.Message_Delete(Fun.UserKey, ref error, _t))
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
