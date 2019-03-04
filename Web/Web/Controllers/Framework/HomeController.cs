using ProInterface;
using ProInterface.Models.Api;
using ProServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProInterface.Models;
using System.Net;
namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult ChangeRegion(string region)
        {
            Global.UpdateRegion(Fun.UserKey, region);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 获取快速通道
        /// </summary>
        /// <returns></returns>
        public string GetSepeed()
        {
            ProInterface.IHome ser = new ProServer.Service();
            string str = ser.GetSepeed(Fun.UserKey, Url);
            return str;
        }



        /// <summary>
        /// 获取有地址模块
        /// </summary>
        /// <returns></returns>
        public string GetModule()
        {
            ProInterface.IHome ser = new ProServer.Service();
            string str = ser.GetModule(Fun.UserKey, ref Fun.Err);
            return str;
        }

        /// <summary>
        /// 获取用户菜单
        /// </summary>
        /// <returns></returns>
        public string GetAllMenus()
        {


            ProInterface.IHome ser = new ProServer.Service();
            return ser.GetAllMenus(Fun.UserKey, ref Fun.Err, Url);
        }
        /// <summary>
        /// 获取用户菜单
        /// </summary>
        /// <returns></returns>
        public string GetItemMenu(string id, string statu)
        {
            int parentid = Convert.ToInt32(id);
            ProInterface.IHome ser = new ProServer.Service();
            return ser.GetMenuItem(Fun.UserKey, ref Fun.Err, parentid, statu, Url);
        }

        public string GetMenusName(string name)
        {
            ProInterface.IModule ems = new ProServer.Service();
            List<ProInterface.Models.MODULE> list = ems.SysModuleWhere(Fun.UserKey, ref Fun.Err, 1, 100, "k=>k.LOCATION!=null", "SHOW_ORDER", "asc").ToList();
            string str = "";
            foreach (var l in list)
            {
                str += "{\"name\":\"" + l.NAME + "\",\"url\":\"" + l.LOCATION + "\"},";
            }
            if (str == "")
            {
                return "[]";
            }
            else
            {
                str = "[" + str.Substring(0, str.Length - 1) + "]";
            }
            return str;
        }

        public string GetAllModules(string id)
        {
            string loginKey = Fun.UserKey;
            ProInterface.IModule ems = new ProServer.Service();

            string whereLamba = null;
            if (id != null)
            {
                whereLamba = "a=>a.PARENT_ID.Value==" + id;
            }
            else
            {
                whereLamba = "a=>a.PARENT_ID==null";
            }
            var allList = ems.SysModuleWhere(loginKey, ref Fun.Err, 1, 100, whereLamba, "SHOW_ORDER", "asc");
            allList = allList.Where(x => x.IS_HIDE == 0).ToList();
            if (null == allList)
                return "[]";

            IList<IdText> allRest = new List<IdText>();
            foreach (var t in allList)
            {
                if (t.LOCATION != null)
                {
                    t.LOCATION = Url.Content(t.LOCATION);
                    t.LOCATION = string.Format("OpenTab('{0}','{1}','{2}')", t.LOCATION, t.NAME, t.ID); 
                }
                IdText tmp = new IdText();
                tmp.id = t.ID.ToString();
                tmp.text = t.NAME;
                tmp.attributes = new List<IdTextAttr>();
                tmp.attributes.Add(new IdTextAttr { url = t.LOCATION });
                if (ems.SysModuleCount(loginKey, ref Fun.Err, "a=>a.PARENT_ID.Value==" + t.ID) > 0)
                {
                    tmp.state = "closed";
                }
                allRest.Add(tmp);
            }
            string reStr = ProInterface.JSON.DecodeToStr(allRest);
            return reStr;

        }

        public ActionResult Iframe(string url, string id)
        {

            string tmpUrl = Request.Url.Query.Substring(Request.Url.Query.IndexOf(url));
            if (tmpUrl.Replace(url, "").Substring(0, 1) == "&" && url.IndexOf('?') < 0)
            {
                tmpUrl = url + "?" + tmpUrl.Replace(url, "").Substring(1);
                
            }

            if (tmpUrl.IndexOf("127.0.0.1") > -1)
            {
                tmpUrl = tmpUrl.Replace("127.0.0.1", Request.Url.Host);
            }


            if (tmpUrl.IndexOf("http") > -1)
            {
                if (tmpUrl.IndexOf("?") > -1)
                {
                    tmpUrl = tmpUrl + "&authToken=" + Fun.UserKey;
                }
                else
                {
                    tmpUrl = tmpUrl + "?authToken=" + Fun.UserKey;
                }
            }

            
            ViewBag.Url = tmpUrl;
            ViewBag.ID = id;
            return View();
        }

        public ActionResult Home()
        {
            if (string.IsNullOrEmpty(Fun.UserKey))
            {
                return Redirect("~/Login/Login");
            }
            ProServer.Service db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            var user = db.UserGetRole(Fun.UserKey, ref error);
            if (user == null)
            {
                return Redirect("~/Login/Login");
            }

            ViewData["HomeUrl"] = "~/Query/Query?code=kpi022";
            var bullList = db.BulletinGetNew(Fun.UserKey, ref error);
            ViewData["BbsList"] = bullList;
            ViewData["BulletinList"] = bullList;
            ViewData["BulletinQueryList"] = db.BulletinByTypeCode(Fun.UserKey, ref error, "便捷查询");

            return View(user);

        }

        public string SetThemes(string themes)
        {
            if (themes != null)
            {
                Session["themes"] = themes;
            }
            return "0";
        }

        public ActionResult Indexold()
        {
            ErrorInfo err = new ErrorInfo();
            if (string.IsNullOrEmpty(Fun.UserKey))
            {
                return Redirect("~/Login/Login");
            }
            ProServer.Service db = new ProServer.Service();

            var user = db.UserGetNow(Fun.UserKey, ref Fun.Err);
            if (user == null)
            {
                return Redirect("~/Login/Login");
            }
            string menu = db.GetAllMenus(Fun.UserKey, ref Fun.Err, Url);

            ViewData["menu"] = menu;
            ViewData["region"] = ProServer.Global.GetUser(Fun.UserKey).Region;
            return View(user);
        }
        [HttpPost]
        public ActionResult Index1(string region)
        {
            ProServer.Global.UpdateRegion(Fun.UserKey, region);
            return RedirectToAction("Index");

        }
        /// <summary>
        /// 查询模块代码
        /// </summary>
        /// <returns></returns>
        public string GetMenuCode(string code)
        {
            ProInterface.IHome ems = new ProServer.Service();
            string str = ems.GetMenuCode(Fun.UserKey, ref Fun.Err, code);
            return str;
        }

        /// <summary>
        /// 保存用户快捷通道
        /// </summary>
        public void SetSaveMoudel(string json)
        {

            if (json.IsNullOrEmpty() == true)
            {
                return;
            }

            string[] arr = json.Split(',');
            IList<int> list = new List<int>();
            foreach (var a in arr)
            {
                list.Add(Convert.ToInt32(a));
            }
            if (list.Count > 6)
            {
                for (int i = list.Count - 1; i >= 6; i--)
                {
                    list.Remove(list[i]);
                }
            }
            ProInterface.IUser user = new ProServer.Service();
            bool b = user.UserSetUserModule(Fun.UserKey, ref Fun.Err, list);
        }

        /// <summary>
        /// 同步菜单
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetTongBu(string id)
        {
            int myid = Convert.ToInt32(id);
            ProInterface.IHome home = new ProServer.Service();
            string str = home.GetTongBu(Fun.UserKey, ref Fun.Err, myid, Url);
            return str;
        }
        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIp()
        {

            //获取客户端ip
            string userHostAddress =System.Web.HttpContext.Current.Request.UserHostAddress;
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress =System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (!string.IsNullOrEmpty(userHostAddress) && IsIP(userHostAddress))
            {
                return userHostAddress;
            }
            else {
                //获得本机局域网IP地址
                System.Net.IPAddress addr;
                addr = new System.Net.IPAddress(System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0].Address);
                return addr.ToString();
            }
                


        }
        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        public ActionResult Index()
        {
            ErrorInfo err = new ErrorInfo();
            if (string.IsNullOrEmpty(Fun.UserKey))
            {
                return Redirect("~/Login/Login");
            }
            ProInterface.IHome ser = new ProServer.Service();
            ProServer.Service db = new Service();
            ProInterface.IUser ems = new ProServer.Service();
            var user = ems.UserGetNow(Fun.UserKey, ref Fun.Err);
            if (user == null)
            {
                return Redirect("~/Login/Login");
            }
            string[] menu = ser.HomeGetParentMenu(Fun.UserKey, ref Fun.Err, Url);
            user.ParentMenu = menu[0];
            user.ChirldMenu = menu[1];
            user.EMAIL_ADDR = GetClientIp();
            ViewData["region"] = ProServer.Global.GetUser(Fun.UserKey).Region;
            return View(user);
        }
        [HttpPost]
        public ActionResult UserMessageGetNewCount()
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ProInterface.IUserMessage msg = new ProServer.Service();
            int count = msg.UserMessageGetNewCount(Fun.UserKey, ref error);
            return Json(count);
        }

        public ActionResult ModuleFilter(string key)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ProInterface.IModule db = new ProServer.Service();
            var obj = db.ModuleByNameOrCode(Fun.UserKey, ref error, key);
            return Json(obj,JsonRequestBehavior.AllowGet);
        }
    }
}
