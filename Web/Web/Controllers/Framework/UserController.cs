using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProInterface.Models;
using ProInterface;

namespace Web.Controllers
{
    public class UserController : Controller
    {
        public ActionResult AllOnlineUser()
        {
            return View(ProServer.Global.OnLines);
        }
        public ActionResult DeleteOnlineUser(string userKey)
        {
            if (!string.IsNullOrEmpty(Fun.UserKey) && ProServer.Global.GetUser(userKey) != null)
            {
                ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
                if (!ProServer.Global.Del(userKey))
                {
                    error.IsError = true;
                    error.Message = "删除失败";
                }
                else
                {
                    error.Message = "删除成功";
                }
                return Json(error,JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Redirect("~/Login/Login");
            }
        }

        public ActionResult CheckUser()
        {
            var db=new ProServer.Service();
            ErrorInfo error=new ErrorInfo();
            var allRole=db.Role_FindAll(Fun.UserKey, ref error).Select(x => new SelectListItem { Value=x.ID.ToString(),Text=x.NAME }).ToList();

            if (!string.IsNullOrEmpty(Request["roleIdStr"]))
            { 
                var allRoleList=Request["roleIdStr"].Split(',').ToList();
                allRole = allRole.Where(x => allRoleList.Contains(x.Value)).ToList();
                foreach (var t in allRole)
                {
                    t.Selected = true;
                }
            }

            ViewData["AllRole"] = allRole;

            
            
            return View();
        }
        public ActionResult GetBylUserId(string userIdStr)
        {
            ProInterface.IUser db = new ProServer.Service();
            ErrorInfo error = new ErrorInfo();
            var allList = db.UserGetBylUserId(Fun.UserKey, ref error, userIdStr);
            return Json(allList, JsonRequestBehavior.AllowGet);
        }

        

        /// <summary>
        /// 根据角色找到所有用户的树弄结构
        /// </summary>
        /// <param name="roleIdStr"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public ActionResult AjaxAllUser(string roleIdStr , int level=4)
        {
            ProInterface.IUser db = new ProServer.Service();
            ErrorInfo error = new ErrorInfo();
            var allList = db.UserGetTree(Fun.UserKey, ref error, roleIdStr, level);
            return Json(allList,JsonRequestBehavior.AllowGet);
        }



        public ActionResult UserGetBylRoleDistrict(string roleIdStr, string districtId)
        {
            ProInterface.IUser db = new ProServer.Service();
            ErrorInfo error = new ErrorInfo();
            var allList = db.UserGetBylRoleDistrict(Fun.UserKey, ref error, roleIdStr, districtId);
            return Json(allList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxAllBylRole(string roleIdStr)
        {
            ProInterface.IUser db = new ProServer.Service();
            ErrorInfo error = new ErrorInfo();
            var allList = db.UserGetBylRole(Fun.UserKey, ref error, roleIdStr);
            return Json(allList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserResetPwdFrom()
        {
            return View();
        }
        [HttpGet]
        public JavaScriptResult UserResetPwd(string loginName,string oldPwd,string newPwd)
        {
            ProInterface.IUser ems = new ProServer.Service();
            if (string.IsNullOrEmpty(newPwd))
            {
                newPwd =ProInterface.AppSet.DefaultPwd;
            }
            if (ems.UserResetPwd(ref Fun.Err, Fun.UserKey, loginName, oldPwd, newPwd))
            {
                return JavaScript("alert('修改【" + loginName + "】密码为【" + newPwd + "】成功')");
            }
            else
            {
                return JavaScript("alert('修改【" + loginName + "】密码失败\\r\\n" + Fun.Err.Message + "')");
            }
        }
        public IList<SelectListItem> AjaxAllRole()
        {
            ProInterface.IRole ems = new ProServer.Service();
            IList<ROLE> allModule = ems.Role_Where(Fun.UserKey, ref Fun.Err, 1, 1000, "", "ID", "asc");
            IList<SelectListItem> reEnt = new List<SelectListItem>();
            foreach (var ent in allModule)
            {
                reEnt.Add(new SelectListItem { Text = ent.NAME, Value = ent.ID.ToString(),Selected=false });
            }
            return reEnt;
        }
        public ActionResult Details(int? id)
        {
            if (id != null)
            {
                ProInterface.IUser ems = new ProServer.Service();
                var _Ent = ems.UserSingleId(Fun.UserKey, ref Fun.Err, id.Value);
                ViewData["list"] = AjaxAllRole();
                return View(_Ent);
            }
            else
            {
                ViewData["list"] = AjaxAllRole();
            }
            return View();
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="ent">模块类</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Details(ProInterface.Models.TUser ent)
        {
            ProInterface.IUser ems = new ProServer.Service();
            ErrorInfo error = new ErrorInfo();
            //ent.UserDistrict = Request["UserDistrict"];
            //ErrorInfo error = new ErrorInfo();
            //db.UserSave(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());

            if (ent.ID == 0)
            {
                var id = ems.UserAdd(Fun.UserKey, ref error, ent);
            }
            else
            {
                ems.UserEdit(Fun.UserKey, ref error, ent);
            }

            if (error.IsError)
            {
                return JavaScript("alert('保存失败：" + error.Message + "');");
            }
            return JavaScript("alert('保存成功');");
        }

        public ActionResult Delete(string id)
        {
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
                        ProInterface.IUser ems = new ProServer.Service();
                        if (ems.UserDelete(Fun.UserKey, ref Fun.Err, _t))
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
                return JavaScript("alert('删除成功[" + succ + "]个\\r\\n删除失败[" + fail + "]个');");
            }
            return JavaScript("alert('删除失败');");
        }
        public string GetAllDistrictJson(string id)
        {
            ProInterface.IDistrict ems = new ProServer.Service();
            return JSON.DecodeToStr(ems.DistrictGetAsyn(id));
        }
        public ActionResult UserMessageGetNew()
        {
            IList<TUserMessage> allReList = new List<TUserMessage>();
            ErrorInfo err = new ErrorInfo();
            ProInterface.IUserMessage usermsg = new ProServer.Service();
            allReList=usermsg.UserMessageGetNew(Fun.UserKey, ref err);
            ViewData["allReList"] = allReList;
            return View();
        }
        public ActionResult UserMessageGetOld()
        {
            IList<TUserMessage> allReList = new List<TUserMessage>();
            ErrorInfo err = new ErrorInfo();
            ProInterface.IUserMessage usermsg = new ProServer.Service();
            allReList = usermsg.UserMessageGetOld(Fun.UserKey, ref err);
            ViewData["allReList"] = allReList;
            return View();
        }
       [HttpPost]
        public ActionResult UserMessageSetFinish(string messageid)
        {
            ErrorInfo err = new ErrorInfo();
            ProInterface.IUserMessage usermsg = new ProServer.Service();
            bool del=usermsg.UserMessageSetFinish(Fun.UserKey, ref err,int.Parse(messageid));
            return Json(del);
        }

       public ActionResult UpdateLocked(string userIdStr, short lockVal)
       {
           ErrorInfo err = new ErrorInfo();
           ProInterface.IUser usermsg = new ProServer.Service();
           
           var del = usermsg.UserUpdateLocked(Fun.UserKey, ref err, userIdStr, lockVal);
           return Json(del,JsonRequestBehavior.AllowGet);
       }
    }

}

