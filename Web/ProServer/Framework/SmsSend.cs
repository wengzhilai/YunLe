
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
using System.Data.Entity.Validation;
using System.Web;
using System.Web.Configuration;
using Senparc.Weixin.MP.Containers;

namespace ProServer
{
    public partial class Service : ISmsSend
    {

        public bool SmsSendAdd(string loginKey, ref ErrorInfo err, string phone, string conten,int? messageId)
        {
            using (DBEntities db = new DBEntities())
            {
                YL_SMS_SEND ent=new YL_SMS_SEND();
                ent.KEY = Guid.NewGuid().ToString().Replace("-", "");
                ent.MESSAGE_ID = messageId;
                ent.PHONE_NO = phone;
                ent.ADD_TIME = DateTime.Now;
                ent.CONTENT = conten;
                ent.STAUTS = "等待";
                db.YL_SMS_SEND.Add(ent);
                try
                {
                    db.SaveChanges();
                }
                catch(DbEntityValidationException e) {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return false;
                }
                return true;
            }
        }

        public bool SmsSendAdd(DBEntities db, string phone, string conten, int? messageId)
        {
            YL_SMS_SEND ent = new YL_SMS_SEND();
            ent.KEY = Guid.NewGuid().ToString().Replace("-", "");
            ent.MESSAGE_ID = messageId;
            ent.PHONE_NO = phone;
            ent.ADD_TIME = DateTime.Now;
            ent.CONTENT = conten;
            ent.STAUTS = "等待";               
            db.YL_SMS_SEND.Add(ent);
            return true;
        }

        public bool SmsSendCode(string loginKey,string mobile, string code)
        {
            string tpl_value = HttpUtility.UrlEncode(
           HttpUtility.UrlEncode("#code#", Encoding.UTF8) + "=" +
           HttpUtility.UrlEncode(code, Encoding.UTF8), Encoding.UTF8);
            string data_tpl_sms = "apikey=51f88df9eedd2e9565f5f3a9417c45df&mobile=" + mobile + "&tpl_id=1323633&tpl_value=" + tpl_value;
            string questStr = "";
            if (Fun.HttpPostEncoded("https://sms.yunpian.com/v2/sms/tpl_single_send.json", data_tpl_sms, ref questStr))
            {
                return true;
            }
            return false;
        }
        public bool SmsSendOrder(ref ErrorInfo err, string mobile, string time,string content)
        {
            if(!mobile.IsInt64() || mobile.Length!=11)
            {
                return false;
            }

            string tpl_value = "#time#={0}&#content#={1}";
            tpl_value = string.Format(tpl_value, time, content);
            tpl_value = HttpUtility.UrlEncode(tpl_value);

            string data_tpl_sms = "apikey=51f88df9eedd2e9565f5f3a9417c45df&mobile=" + mobile + "&tpl_id=1540992&tpl_value=" + tpl_value;
            string questStr = "";
            if (Fun.HttpPostEncoded("https://sms.yunpian.com/v2/sms/tpl_single_send.json", data_tpl_sms, ref questStr))
            {
                return true;
            }

            return false;
        }
        public bool SmsSendWeiXin(ref ErrorInfo err, string content, int userId, List<KV> allPar = null)
        {

            var url = "http://www.27580sc.com/staffcar/index.html{0}#/OrderListPage/";
            if (allPar != null && allPar.Count()>0)
            {
                url += allPar[0].V;
            }
            using (DBEntities db = new DBEntities())
            {
                var user = db.YL_USER.SingleOrDefault(x => x.ID == userId);
                if (user != null)
                {
                    if (string.Join(",", user.YL_ROLE.Select(x => x.ID).ToList()) == "2")
                    {
                        url = url.Replace("staffcar", "userCar");
                    }
                }
                if (WeixinSendMsg(userId, content, url, ref err))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool WeixinSendMsg(int userId, string content,string url,ref ErrorInfo err)
        {
            string openId = "";
            using (DBEntities db = new DBEntities())
            {
                var entList = db.YL_WEIXIN_USER.Where(x => x.USER_ID == userId).OrderByDescending(x => x.TAKE_TIME).ToList();
                if (entList.Count() == 0)
                {
                    return false;
                }
                var ent = entList[0];
                if (ent == null || string.IsNullOrEmpty(ent.OPENID))
                {
                    err.IsError = true;
                    err.Message = "该用户不是微信用户";
                    return false;
                }
                openId = ent.OPENID;
            }
            string EncodingAESKey = WebConfigurationManager.AppSettings["WeixinEncodingAESKey"];//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
            string AppId = WebConfigurationManager.AppSettings["WeixinAppId"];//与微信公众账号后台的AppId设置保持一致，区分大小写。
            string AppSecret = WebConfigurationManager.AppSettings["WeixinAppSecret"];//与微信公众账号后台的AppId设置保持一致，区分大小写。

            if (!AccessTokenContainer.CheckRegistered(AppId))
            {
                AccessTokenContainer.Register(AppId, AppSecret);
            }
            var result = AccessTokenContainer.GetAccessTokenResult(AppId); //CommonAPIs.CommonApi.GetToken(appId, appSecret);

            try
            {
                var allAtricle = new List<Senparc.Weixin.MP.Entities.Article>();
                allAtricle.Add(new Senparc.Weixin.MP.Entities.Article() {
                    Title = content,
                    Url = string.Format(url, "?openid=" + openId)
                });
                var wxJsonResult = Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendNews(result.access_token, openId, allAtricle);
                if (wxJsonResult.errcode != 0)
                {
                    err.IsError = true;
                    err.Message = wxJsonResult.errmsg;
                }
                else
                {
                    err.IsError = false;
                }
            }
            catch(Exception e) {
                err.IsError = true;
                err.Message = e.Message;
            }
            return !err.IsError;
        }

    }
}
