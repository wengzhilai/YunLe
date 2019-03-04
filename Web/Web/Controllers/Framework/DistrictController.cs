
using ProInterface.Models;
using ProServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class DistrictController : Controller
    {

        public ActionResult GetAsyn(string id, string type = "ID")
        {
            ProInterface.IDistrict ems = new ProServer.Service();
            return Json(ems.DistrictGetAsyn(id, type), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAll(int id=0,int levelId=5)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ProInterface.IDistrict ems = new ProServer.Service();
            return Json(ems.DistrictGetAll(Fun.UserKey, ref error, id, levelId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetJsonByCompanyId(int companyId)
        {
            ProServer.Service ems = new ProServer.Service();
            var ent = ems.DistrictTreeDataGetAllByCompId(0, companyId);
            return Json(ent,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int? id)
        {
            ProServer.Service ems = new ProServer.Service();

            GlobalUser gu = Global.GetUser(Fun.UserKey);

            var allTreeEnt = ems.DistrictTreeDataGetAll(0,Fun.UserKey);
            allTreeEnt.Insert(0, new ProInterface.Models.TreeData() {  id=null,text=""});
            ViewData["DisJsonObj"] = ProInterface.JSON.DecodeToStr(allTreeEnt);
            if (id != null)
            {
                var _Ent = ems.District_SingleId(Fun.UserKey, ref Fun.Err, id.Value);
                return View(_Ent);
            }
            return View(new DISTRICT { IN_USE=1});
        }


        [HttpPost]
        public ActionResult Details(ProInterface.Models.DISTRICT ent)
        {

            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ProInterface.IDistrict ems = new ProServer.Service();
            if (ent.ID==0)
            {
                var id = ems.DistrictAdd(Fun.UserKey, ref error, ent);
            }
            else
            {
                ems.DistrictEdit(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());
            }
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

        public string UpLevelIdpath()
        {
            ProInterface.IDistrict db = new ProServer.Service();
            db.DistrictUpLevelIdpath();
            return "";
        }

        public ActionResult Delete(string id)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            if (id != null)
            {
                int fail = 0, succ = 0;
                string[] idArr = id.Split(',');
                for (int i = 0; i < idArr.Count(); i++)
                {
                    int _t = 0;
                    try
                    {
                        _t = Convert.ToInt32(idArr[i]);
                        Fun.Err = new ProInterface.ErrorInfo();
                        ProInterface.IDistrict ems = new ProServer.Service();
                        if (ems.District_Delete(Fun.UserKey, ref error, _t))
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
