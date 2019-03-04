using ProServer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZXing.Common;
using ZXing;

namespace Web.Controllers
{
    public class LoginController : Controller
    {

        public ActionResult Matrix(string type,int w)
        {

            var url = "http://"+Request.Url.Host+ Request.ApplicationPath+ " / File/Apk/staffCar.apk";

            BitMatrix bitMatrix;
            bitMatrix = new MultiFormatWriter().encode(url, BarcodeFormat.QR_CODE, w, w);
            ZXing.BarcodeWriter bw = new ZXing.BarcodeWriter();

            var ms = new MemoryStream();
            var bitmap = bw.Write(bitMatrix);
            bitmap.Save(ms, ImageFormat.Png);
            //return File(ms, "image/png");
            ms.WriteTo(Response.OutputStream);
            Response.ContentType = "image/png";
            return null;
        }
        

        public ActionResult CheckCode()
        {
            Session["ValidateCode"] = ProServer.PicFun.ValidateMake(4);
            //创建验证码的图片
            byte[] bytes = ProServer.PicFun.CreateValidateGraphic(Session["ValidateCode"].ToString());
            //最后将验证码返回
            return File(bytes, @"image/jpeg");
        }

        //
        // GET: /Login/ 
        public ActionResult Repwd()
        { 
            return View();
        }
        public ActionResult UserGetPhone(string name, string phone)
        {
            ProInterface.IUser ems = new ProServer.Service();
            ProInterface.Models.TLogin entlogin = new ProInterface.Models.TLogin();
            ProInterface.ErrorInfo err = new ProInterface.ErrorInfo();
            entlogin.AseLoginName = name;
            entlogin.AsePhoneNo = phone;
            entlogin.PHONE_NO = ems.UserGetPhone(ref err, entlogin);
            if (entlogin.PHONE_NO == null)
            {
                return Json(err);
            }
            else {
                if (!ems.UserSendVerifyCode(ref err, entlogin))
                {
                    return Json(err);
                }
                else {
                   return RedirectToAction("Repwd");
                }
            }
           
        }
        [HttpPost]
        public ActionResult UserSendVerifyCode(string username)
        {
            ProInterface.IUser ems = new ProServer.Service();
            ProInterface.ErrorInfo err = new ProInterface.ErrorInfo();
            if (!ems.UserSendVerifyCode(ref err, username))
            {
                return Json(err);
            }
            return RedirectToAction("Repwd");
        }
        [HttpPost]
        public ActionResult UserResetPwdByVerifyCode(string username, string code, string newpwd)
        {
            ProInterface.ErrorInfo err = new ProInterface.ErrorInfo(); 

            var i=0;
            if (Session["TryNum"] != null)
            {
                i = Convert.ToInt32(Session["TryNum"]);
            }
            i++;
            if (i > 5)
            {
                err.IsError = true;
                err.Message = "每天最多重试5次";
                return Json(err);
            }

            string userName = username; 
            string Code = code;
            string newPwd = newpwd;
            ProInterface.IUser ems = new ProServer.Service();
            if (!ems.UserResetPwdByVerifyCode(ref err, userName, Code, newPwd))
            {
                return Json(err);
            }
            return RedirectToAction("Repwd");
        }
        
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult LoginOut()
        {
            ProInterface.IUser ems = new ProServer.Service();
            if (ems.UserLoginOut(Fun.UserKey, ref Fun.Err))
            {
                Fun.UserKey = null;
                return Redirect("~/Login/Login");
            }
            else
            {
                return Content("<script>alert('退出失败');</script>");
            }
        }
        bool isLogin = false;
        [HttpPost]
        public ActionResult Login(ProInterface.Models.TLogin ent)
        {
            ProInterface.ErrorInfo err = new ProInterface.ErrorInfo();
            if (ProInterface.AppSet.VerifyCode)
            {
                if (ent.VERIFY_CODE==null || Session["ValidateCode"] == null || Session["ValidateCode"].ToString().ToLower() != ent.VERIFY_CODE.ToLower())
                {
                    err.IsError = true;
                    err.Message = "验证码错误";
                    return Json(err);
                }
            }
            if (isLogin) return View();
            isLogin = true;
            ProInterface.IUser ems = new ProServer.Service();
            string ip = "";
            if (HttpContext.Request.ServerVariables["REMOTE_ADDR"] != null) // using proxy
            {
                ip = HttpContext.Request.ServerVariables["REMOTE_ADDR"].ToString(); //While it can't get the Client IP, it will return proxy IP.
            }

            GlobalUser gu = ems.UserLogin(ref err, ent, ip) as GlobalUser;
            if (err.IsError)
            {
                return Json(err);
            }

            isLogin = false;
            if (!string.IsNullOrEmpty(gu.Guid))
            {
                //CFG.ConfigWebUrl = Request.Url.AbsoluteUri.Replace("Login/Login", "");
                //CFG.ConfigAbsolute = Request.Url.AbsolutePath.Replace("Login/Login", "");
                Fun.UserKey = gu.Guid;
                return Json(gu);
            }
            else
            {
                return Json(err);
            }
          
        }


        public int LoginByNamePwd(string loginName, string pwd)
        {
            string ip = "";
            if (HttpContext.Request.ServerVariables["HTTP_VIA"] != null) // using proxy
            {
                ip = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();  // Return real client IP.
            }
            else// not using proxy or can't get the Client IP
            {
                ip = HttpContext.Request.ServerVariables["REMOTE_ADDR"].ToString(); //While it can't get the Client IP, it will return proxy IP.
            }
            ProInterface.IUser ems = new ProServer.Service();
            GlobalUser gu = ems.UserLogin(ref Fun.Err, loginName, pwd, ip) as GlobalUser;
            return gu.UserId;
        }

        #region 系统生成

        public ActionResult GetProperDescription(string proName)
        {
            return Json(ProServer.Fun.GetClassProperDescription<ProInterface.Models.LOGIN>(proName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int? id)
        {
            ProServer.Service db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ViewData["allOauth"] = db.LoginAllOauth();

            if (id != null)
            {

                var _Ent = db.Login_SingleId(Fun.UserKey, ref error, id.Value);
                return View(_Ent);
            }
            return View();
        }


        [HttpPost]
        public ActionResult Details(ProInterface.Models.LOGIN ent)
        {

            ProInterface.IZ_Login db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            db.Login_Save(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());
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
                ProInterface.IZ_Login db = new ProServer.Service();
                for (int i = 0; i < idArr.Count(); i++)
                {
                    int _t = 0;
                    try
                    {
                        _t = Convert.ToInt32(idArr[i]);
                        if (db.Login_Delete(Fun.UserKey, ref error, _t))
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
        
        #endregion


        [HttpPost]
        public ActionResult OauthLogin(string openId,string state , string access_token)
        {
            ProInterface.ErrorInfo err = new ProInterface.ErrorInfo();
            string app_id = "28948761";
            string key = string.Format("{0}{1}{2}", app_id, openId, state).Md5();
            if (key == access_token) //认证成功
            {
                ProServer.Service db = new Service();
                string ip = "";
                if (HttpContext.Request.ServerVariables["REMOTE_ADDR"] != null) // using proxy
                {
                    ip = HttpContext.Request.ServerVariables["REMOTE_ADDR"].ToString(); //While it can't get the Client IP, it will return proxy IP.
                }
                GlobalUser gu = db.UserLogin(ref err, openId, ip) as GlobalUser;
                if (!string.IsNullOrEmpty(gu.Guid))
                {
                    CFG.ConfigWebUrl = Request.Url.AbsoluteUri.Replace("Login/Login", "");
                    CFG.ConfigAbsolute = Request.Url.AbsolutePath.Replace("Login/Login", "");
                    Fun.UserKey = gu.Guid;
                    return Content("<script>window.location = '../Home/Index';</script>");
                }
            }
            else
            {
                err.IsError = true;
                err.Message = "非法请求";
            }
            return Content(err.Message);
        }


        [HttpPost]
        public ActionResult OauthLoginByPwd(string loginName, string pwd, string access_token)
        {
            ProInterface.ErrorInfo err = new ProInterface.ErrorInfo();
            string app_id = "28948761";
            string key = string.Format("{0}{1}{2}", app_id, loginName, pwd).Md5();
            if (key == access_token) //认证成功
            {
                ProInterface.ILogin db = new Service();
                string ip = "";
                if (HttpContext.Request.ServerVariables["REMOTE_ADDR"] != null) // using proxy
                {
                    ip = HttpContext.Request.ServerVariables["REMOTE_ADDR"].ToString(); //While it can't get the Client IP, it will return proxy IP.
                }
                var isTrue = db.LoginByPassWord(ref err, loginName, pwd);
                if (isTrue)
                {
                    err.Message = "成功";
                    return Json(err);
                }
            }
            else
            {
                err.IsError = true;
                err.Message = "非法请求";
            }
            return Json(err);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="inEnt"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public ActionResult OauthLoginAdd(int appId, ProInterface.Models.LOGIN inEnt, string access_token)
        {
            ProInterface.ErrorInfo err = new ProInterface.ErrorInfo();
            string key = string.Format("{0}{1}", appId, inEnt.LOGIN_NAME).Md5();
            if (key == access_token) //认证成功
            {
                ProServer.Service db = new Service();
                var isSucc = db.LoginAdd(appId, inEnt);
                err.Message = "成功";
                return Json(err, JsonRequestBehavior.AllowGet);
            }
            else
            {
                err.IsError = true;
                err.Message = "非法请求";
            }
            return Json(err, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统一平台登录
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginOauth()
        {
            return View();
        }

        /// <summary>
        /// 统一平台登录
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoginOauth(ProInterface.Models.LOGIN ent)
        {
            ProInterface.ErrorInfo err = new ProInterface.ErrorInfo();
            if (ProInterface.AppSet.VerifyCode)
            {
                if (Session["ValidateCode"] == null || Session["ValidateCode"].ToString().ToLower() != ent.VERIFY_CODE.ToLower())
                {
                    err.IsError = true;
                    err.Message = "验证码错误";
                    return Json(err);
                }
            }

            string ip = "";
            if (HttpContext.Request.ServerVariables["REMOTE_ADDR"] != null) // using proxy
            {
                ip = HttpContext.Request.ServerVariables["REMOTE_ADDR"].ToString(); //While it can't get the Client IP, it will return proxy IP.
            }

            ProServer.Service db=new Service();
            var allOauth = db.LoginOauth(ref err, ent.LOGIN_NAME, ent.PASSWORD, ip);
            return Json(allOauth);
        }
    }
}