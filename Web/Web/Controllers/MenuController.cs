using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Menu;
using ProInterface;
using Senparc.Weixin.MP;
using System.Web.Configuration;
using Senparc.Weixin.MP.Containers;

namespace Web.Controllers
{
    public class MenuController : Controller
    {
        public static readonly string Token = WebConfigurationManager.AppSettings["WeixinToken"];//与微信公众账号后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = WebConfigurationManager.AppSettings["WeixinEncodingAESKey"];//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string AppId = WebConfigurationManager.AppSettings["WeixinAppId"];//与微信公众账号后台的AppId设置保持一致，区分大小写。
        public static readonly string AppSecret = WebConfigurationManager.AppSettings["WeixinAppSecret"];//与微信公众账号后台的AppId设置保持一致，区分大小写。


        public ActionResult Index()
        {
            try
            {

                if (!AccessTokenContainer.CheckRegistered(AppId))
                {
                    AccessTokenContainer.Register(AppId, AppSecret);
                }
                var result = AccessTokenContainer.GetAccessTokenResult(AppId); //CommonAPIs.CommonApi.GetToken(appId, appSecret);

                var Menu = CommonApi.GetMenu(result.access_token);

                if (Menu == null)
                {
                    return CreateMenu1(AppId, @"
{
    ""menu"": {
        ""button"": [
            {
                ""sub_button"": [
                    {
                        ""key"": ""http://139.129.194.140/staffCar/"",
                        ""type"": ""view"",
                        ""name"": ""业务员登录""
                    },
                    {
                        ""key"": ""http://139.129.194.140/UserCar/"",
                        ""type"": ""view"",
                        ""name"": ""会员登录""
                    }
                ],
                ""name"": ""用户登录""
            },
            {
                ""sub_button"": [
                    {
                        ""key"": ""SubClickRoot_Text"",
                        ""type"": ""click"",
                        ""name"": ""返回文本""
                    },
                    {
                        ""key"": ""SubClickRoot_News"",
                        ""type"": ""click"",
                        ""name"": ""返回图文""
                    }
                ],
                ""name"": ""公司介绍""
            },
            {
                ""sub_button"": [
                    {
                        ""key"": ""OAuth"",
                        ""type"": ""click"",
                        ""name"": ""OAuth授权""
                    }
                ],
                ""name"": ""车友会""
            }
        ]
    }
}
");
                }
                var t = JSON.DecodeToStr(Menu); ;
                ViewData["token"] = AppId;
                ViewData["menuJson"] = t;
                return View();
            }
            catch (Exception)
            {
                //TODO:为简化代码，这里不处理异常（如Token过期）
                return Json(new { error = "执行过程发生错误！" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetToken(string appId, string appSecret)
        {
            try
            {
                if (!AccessTokenContainer.CheckRegistered(appId))
                {
                    AccessTokenContainer.Register(appId, appSecret);
                }
                var result = AccessTokenContainer.GetAccessTokenResult(appId); //CommonAPIs.CommonApi.GetToken(appId, appSecret);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                //TODO:为简化代码，这里不处理异常（如Token过期）
                return Json(new { error = "执行过程发生错误！" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CreateMenu1(string token, string menuJson)
        {
            try
            {
                var resultFull = JSON.EncodeToEntity<GetMenuResultFull>(menuJson);
                //重新整理按钮信息
                var bg = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetMenuFromJsonResult(resultFull, new ConditionalButtonGroup()).menu;
                var result = Senparc.Weixin.MP.CommonAPIs.CommonApi.CreateMenu(token, bg);
                return JavaScript("alert('保存：" + result.errmsg + "');");
            }
            catch (Exception ex)
            {
                return JavaScript("alert('失败：" + ex.Message.Replace("\r\n","") + "');");
            }
        }

        public ActionResult GetMenu(string token)
        {
            var result = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetMenu(token);
            if (result == null)
            {
                return Json(new { error = "菜单不存在或验证失败！" }, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteMenu(string token)
        {
            try
            {
                var result = Senparc.Weixin.MP.CommonAPIs.CommonApi.DeleteMenu(token);
                var json = new
                               {
                                   Success = result.errmsg == "ok",
                                   Message = result.errmsg
                               };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new { Success = false, Message = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
