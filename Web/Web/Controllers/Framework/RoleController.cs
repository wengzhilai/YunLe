using ProInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class RoleController : Controller
    {
        public string AjaxAllModule(int roleID)
        {
            ProInterface.IModule ems = new ProServer.Service();
            IList<MODULE> allModule = ems.Module_Where(Fun.UserKey, ref Fun.Err, 1, 1000, "", "SHOW_ORDER", "asc");
            IList<TreeClass> reEnt = new List<TreeClass>();
            foreach (var ent in allModule)
            {
                reEnt.Add(new TreeClass() { id = ent.ID.ToString(), name = ent.NAME, parentId = (ent.PARENT_ID == null) ? "" : ent.PARENT_ID.ToString() });
            }
            return ProInterface.JSON.DecodeToStr(reEnt);

        }

        public ActionResult Authority(int roleId,int queryId)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ProServer.Service db = new ProServer.Service();
            ViewData["queryId"] = db.QueryWhere(Fun.UserKey, ref error, 1, 300, null, "ID", null).Select(x => new SelectListItem { Value=x.ID.ToString(), Text=x.NAME }).ToList();

            ViewData["AuthArr"] = db.RoleGetNoAuthority(roleId, queryId);
            return Details(roleId);
        }

        [HttpPost]
        public ActionResult Authority(int roleId, int queryId, string AuthArr)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ProInterface.IRole ems = new ProServer.Service();
            ems.RoleSaveNoAuthority(Fun.UserKey, ref Fun.Err,roleId,queryId ,AuthArr);
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

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                id = 0;
            }
            ProInterface.IRole ems = new ProServer.Service();
            var _Ent = ems.RoleSingle(Fun.UserKey, ref Fun.Err, id.Value);
            return View(_Ent);
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="ent">模块类</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Details(ProInterface.Models.TRole ent)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ProInterface.IRole ems = new ProServer.Service();
            ems.RoleSave(Fun.UserKey, ref Fun.Err, ent, Request.Form.AllKeys.ToList());
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
                ProInterface.IRole db = new ProServer.Service();
                for (int i = 0; i < idArr.Count(); i++)
                {
                    int _t = 0;
                    try
                    {
                        _t = Convert.ToInt32(idArr[i]);
                        if (db.RoleDelete(Fun.UserKey, ref error, _t))
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
            return Json(error,JsonRequestBehavior.AllowGet);
        }
    }
}
