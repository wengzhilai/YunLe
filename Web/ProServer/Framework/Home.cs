using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Expressions;
using LINQExtensions;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using ProInterface.Models;
using ProInterface;
using System.Collections;


namespace ProServer
{
    public partial class Service : IHome
    {

        /// <summary>
        /// 获取快速通道
        /// </summary>
        /// <returns></returns>
        public string GetSepeed(string loginKey, UrlHelper url)
        {
            try
            {
                GlobalUser gu = Global.GetUser(loginKey);
                using (DBEntities db = new DBEntities())
                {
                    List<YL_MODULE> list = db.YL_USER.SingleOrDefault(k => k.ID == gu.UserId).YL_MODULE.ToList();
                    string str = "";
                    foreach (var l in list)
                    {
                        l.LOCATION = l.LOCATION == null ? "" : url.Content(l.LOCATION);
                        str += "{\"id\":" + l.ID + ",\"text\":\"" + l.NAME + "\",\"url\":\"" + l.LOCATION + "\"},";
                    }
                    str = "[" + (str.Length > 0 ? (str.Substring(0, str.Length - 1)) : "") + "]";
                    return str;
                }
            }
            catch
            {
                return "[]";
            }
        }

        /// <summary>
        /// 获取有地址模块
        /// </summary>
        /// <returns></returns>
        public string GetModule(string loginKey, ref ErrorInfo err)
        {
            try
            {
                ProInterface.IModule ems = new ProServer.Service();
                var pt = ems.SysModuleWhere(loginKey, ref err, 1, 100, "k=>k.LOCATION!=null", "SHOW_ORDER", "asc").ToList();
                string str = "";
                foreach (var p in pt)
                {
                    str += "{\"id\": \"" + p.ID + "\", \"text\": \"" + p.NAME + "\"},";
                }
                str = "[" + (str.Length > 0 ? str.Substring(0, str.Length - 1) : "") + "]";
                return str;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 获取用户菜单
        /// </summary>
        /// <returns></returns>
        public string GetAllMenus(string loginKey, ref ErrorInfo err, UrlHelper url)
        {
            try
            {
                string menu = "";
                using (DBEntities db = new DBEntities())
                {

                    ProInterface.IModule ems = new ProServer.Service();
                    string whereLamba = "a=>a.PARENT_ID==null";
                    var pt = ems.SysModuleWhere(loginKey, ref err, 1, 100, whereLamba, "SHOW_ORDER", "asc");
                    pt = pt.Where(x => x.IS_HIDE == 0).ToList();
                    if (pt.Count > 0)
                    {
                        int index = 0;
                        foreach (var r in pt)
                        {
                            if (r.LOCATION.IsNullOrEmpty() == false)
                            {
                                r.LOCATION = url.Content(r.LOCATION);
                            }
                            menu += "<li style='width: 10px; background: url(../Styles/Images/ico.png) no-repeat 4px 2px;'></li><li >  <a href='#' onclick=\"OpenTab('" + r.LOCATION + "','" + r.NAME + "','" + r.ID + "')\"  class=\"easyui-menubutton\" data-options=\"menu:'#mm" + index + "'\"> " + r.NAME + "</a> ";
                            int num = 0;
                            List<MODULE> list = ems.SysModuleWhere(loginKey, ref err, 1, 100, "k => k.PARENT_ID.Value ==" + r.ID, "SHOW_ORDER", "asc").ToList().Where(k => k.IS_HIDE == 0).ToList();
                            string chrilmenu = "";
                            GetMenus(ref chrilmenu, r.ID, index.ToString(), ref num, list, loginKey, ref err, url);
                            menu += chrilmenu + "</li>";
                            index++;
                        }
                    }
                }
                return menu;
            }
            catch
            {
                return "";
            }
        }
        private void GetMenus(ref string menu, int parentid, string ids, ref int num, List<MODULE> querys, string loginKey, ref ErrorInfo err, UrlHelper url)
        {
            try
            {
                using (DBEntities db = new DBEntities())
                {
                    if (querys.Count > 0)
                    {
                        ProInterface.IModule ems = new ProServer.Service();
                        menu += "<div id=\"mm" + ids + "\">";
                        ids += "10";
                        foreach (var q in querys)
                        {
                            string whereLamba = "a=>a.PARENT_ID.Value==" + q.ID;
                            var query = ems.SysModuleWhere(loginKey, ref err, 1, 100, whereLamba, "SHOW_ORDER", "asc").Where(k => k.IS_HIDE == 0).ToList();
                            if (q.LOCATION.IsNullOrEmpty() == false)
                            {
                                q.LOCATION = url.Content(q.LOCATION);
                            }
                            menu += "<div  onclick=\"OpenTab('" + q.LOCATION + "','" + q.NAME + "','" + q.ID + "')\"  >" + q.NAME;
                            GetMenus(ref menu, q.ID, ids, ref num, query, loginKey, ref err, url);
                            menu += "</div>";
                        }
                        menu += "</div>";
                    }

                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 获取同级菜单
        /// </summary>
        /// <returns></returns>
        public string GetMenuItem(string loginKey, ref ErrorInfo err, int parentid, string statu, UrlHelper url)
        {
            try
            {
                using (DBEntities db = new DBEntities())
                {
                    List<MODULE> pt = null;
                    ProInterface.IModule ems = new ProServer.Service();
                    string whereLamba = "a=>a.PARENT_ID==null";
                    var menu = "";
                    if (parentid == 0)
                    {
                        pt = ems.SysModuleWhere(loginKey, ref err, 1, 100, whereLamba, "SHOW_ORDER", "asc").Where(k => k.IS_HIDE == 0).ToList();
                        menu += "{\"id\": \"0\", \"text\": \"主菜单\",\"attributes\":\"\"},";
                    }
                    else
                    {
                        var first = ems.SysModuleWhere(loginKey, ref err, 1, 100, "k=>k.ID==" + parentid, "SHOW_ORDER", "asc").Where(k => k.IS_HIDE == 0).ToList()[0];
                        //  var first = db.YL_MODULE.SingleOrDefault(k => k.ID == parentid);
                        MODULE ml = null;
                        if (statu == "Next")
                        {
                            whereLamba = "a=>a.PARENT_ID.Value==" + parentid;
                            pt = ems.SysModuleWhere(loginKey, ref err, 1, 100, whereLamba, "SHOW_ORDER", "asc").Where(k => k.IS_HIDE == 0).ToList();
                            ml = first;
                        }
                        else if (statu == "Pre")
                        {
                            if (first.PARENT_ID == null)
                            {
                                whereLamba = "a=>a.PARENT_ID==null";
                                pt = ems.SysModuleWhere(loginKey, ref err, 1, 100, whereLamba, "SHOW_ORDER", "asc").Where(k => k.IS_HIDE == 0).ToList();
                            }
                            else
                            {
                                whereLamba = "a=>a.PARENT_ID.Value==" + first.PARENT_ID;
                                pt = ems.SysModuleWhere(loginKey, ref err, 1, 100, whereLamba, "SHOW_ORDER", "asc").Where(k => k.IS_HIDE == 0).ToList();
                            }
                            string tj = first.PARENT_ID == null ? "k=>k.ID==0" : "k=>k.ID==" + first.PARENT_ID;

                            var qu = ems.SysModuleWhere(loginKey, ref err, 1, 100, tj, "SHOW_ORDER", "asc").Where(k => k.IS_HIDE == 0).ToList();
                            //var qu = db.YL_MODULE.Where(k => k.ID == first.PARENT_ID).ToList();
                            if (qu.Count > 0)
                            {
                                ml = qu[0];
                            }
                            else
                            {
                                ml = new MODULE();
                                ml.NAME = "主菜单";
                                ml.ID = 0;
                            }

                        }
                        if (ml.LOCATION.IsNullOrEmpty() == false)
                        {
                            ml.LOCATION = url.Content(ml.LOCATION);
                        }
                        menu += "{\"id\": \"" + ml.ID + "\", \"text\": \"" + ml.NAME + "\",\"attributes\":\"" + ml.LOCATION + "\"},";
                    }

                    if (pt.Count > 0)
                    {
                        foreach (var p in pt)
                        {
                            if (p.LOCATION.IsNullOrEmpty() == false)
                            {
                                p.LOCATION = url.Content(p.LOCATION);
                            }
                            menu += "{\"id\": \"" + p.ID + "\", \"text\": \"" + p.NAME + "\",\"attributes\":\"" + p.LOCATION + "\"},";
                        }
                    }
                    menu = menu.Length > 0 ? menu.Substring(0, menu.Length - 1) : "";
                    return "[" + menu + "]";
                }
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 查询模块代码
        /// </summary>
        /// <returns></returns>
        public string GetMenuCode(string loginKey, ref ErrorInfo err, string code)
        {
            try
            {
                using (DBEntities db = new DBEntities())
                {
                    ProInterface.IModule ems = new ProServer.Service();
                    ProInterface.Models.MODULE model = ems.ModuleSingleCode(loginKey, ref err, code);
                    string str = "";
                    if (model != null)
                    {
                        str = "{\"text\":\"" + model.NAME + "\",\"attributes\":\"" + model.LOCATION + "\"}";
                    }
                    return str;
                }
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 同步菜单
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetTongBu(string loginKey, ref ErrorInfo err, int id, UrlHelper url)
        {
            try
            {
                 GlobalUser gu = Global.GetUser(loginKey);
                 using (DBEntities db = new DBEntities())
                 {
                     ProInterface.IModule ems = new ProServer.Service();
                     var pt = db.YL_MODULE.SingleOrDefault(x=>x.ID==id);
                     string main = "";
                     string menu = "";
                     if (pt.PARENT_ID == null)
                     {

                         main = "{\"id\": \"0\", \"text\": \"主菜单\",\"attributes\":\"\"},";
                         var list = db.YL_MODULE.Where(k => k.PARENT_ID == null && k.IS_HIDE == 0 && k.YL_ROLE.Where(x => gu.RoleID.Contains(x.ID)).Count() > 0).OrderBy(t => t.SHOW_ORDER);
                         foreach (var l in list)
                         {
                             l.LOCATION = l.LOCATION == null ? "" : url.Content(l.LOCATION);
                             menu += "{\"id\": \"" + l.ID + "\", \"text\": \"" + l.NAME + "\",\"attributes\":\"" + l.LOCATION + "\"},";
                         }
                     }
                     else
                     {
                         var parent =  db.YL_MODULE.SingleOrDefault(x=>x.ID== pt.PARENT_ID.Value);
                         parent.LOCATION = parent.LOCATION == null ? "" : url.Content(parent.LOCATION);
                         main = "{\"id\": \"" + parent.ID + "\", \"text\": \"" + parent.NAME + "\",\"attributes\":\"" + parent.LOCATION + "\"},";
                         var list = db.YL_MODULE.Where(k => k.PARENT_ID == pt.PARENT_ID && k.IS_HIDE == 0 && k.YL_ROLE.Where(x => gu.RoleID.Contains(x.ID)).Count() > 0).OrderBy(t => t.SHOW_ORDER);
                         foreach (var l in list)
                         {
                             l.LOCATION = l.LOCATION == null ? "" : url.Content(l.LOCATION);
                             menu += "{\"id\": \"" + l.ID + "\", \"text\": \"" + l.NAME + "\",\"attributes\":\"" + l.LOCATION + "\"},";
                         }
                     }
                     string str = main + menu;
                     str = str.Length > 0 ? (str.Substring(0, str.Length - 1)) : "";
                     return "[" + str + "]";
                 }
            }
            catch(Exception ex)
            {
             
                err.IsError = true;
                err.Message = ex.ToString(); 
                return "[]";
            }
        }

        /// <summary>
        /// 获取用户菜单
        /// </summary>
        /// <returns></returns>
        public string[] HomeGetParentMenu(string loginKey, ref ErrorInfo err, UrlHelper url)
        {
            try
            {
                GlobalUser gu = Global.GetUser(loginKey);
                using (DBEntities db = new DBEntities())
                {
                    var allModule = db.YL_MODULE.Where(k => k.IS_HIDE == 0 && k.YL_ROLE.Where(x => gu.RoleID.Contains(x.ID)).Count() > 0).OrderBy(t => t.SHOW_ORDER).ToList();
                    string[] stratt = new string[2];
                    ProInterface.IModule ems = new ProServer.Service();
                    var pt = allModule.Where(k => k.PARENT_ID == null).OrderBy(t => t.SHOW_ORDER);
                    string menu = "";
                    string chrilmenu = "";
                    if (pt.Any())
                    {
                        int index = 0;
                        foreach (var r in pt)
                        {
                            if (r.LOCATION.IsNullOrEmpty() == false)
                            {
                                r.LOCATION = url.Content(r.LOCATION);
                            }
                            menu += "<a href='#' onclick=\"OpenTab('" + r.LOCATION + "','" + r.NAME + "','" + r.ID + "')\"  class=\"easyui-menubutton\" data-options=\"menu:'#mm" + index + "'\"> " + r.NAME + "</a>";
                            var list = allModule.Where(k => k.PARENT_ID == r.ID).OrderBy(k => k.SHOW_ORDER).ToList();
                            HomeGetChildMenu(ref err,  allModule, ref chrilmenu, index.ToString(), list, url);
                            index++;
                        }
                    }
                    stratt.SetValue(menu, 0);
                    stratt.SetValue(chrilmenu, 1);
                    return stratt;
                }

            }
            catch
            {
                return null;
            }
        }
        private void HomeGetChildMenu(ref ErrorInfo err, IList<YL_MODULE> allModule, ref string menu, string ids, List<YL_MODULE> list, UrlHelper url)
        {
            try
            {
                if (list.Count > 0)
                {
                    ProInterface.IModule ems = new ProServer.Service();
                    menu += "<div id=\"mm" + ids + "\">";
                    ids += "10";
                    foreach (var q in list)
                    {
                        var query = allModule.Where(k => k.PARENT_ID == q.ID).OrderBy(k => k.SHOW_ORDER).ToList();
                        if (q.LOCATION.IsNullOrEmpty() == false)
                        {
                            q.LOCATION = url.Content(q.LOCATION);
                        }
                        menu += "<div  onclick=\"OpenTab('" + q.LOCATION + "','" + q.NAME + "','" + q.ID + "')\"  >" + q.NAME;
                        HomeGetChildMenu(ref err, allModule, ref menu, ids, query, url);
                        menu += "</div>";
                    }
                    menu += "</div>";
                }
                else {
                    if (Convert.ToInt32(ids) < 30)
                    {
                        //menu += "<div id=\"mm" + ids + "\"></div>";
                    }
                }
            }
            catch
            {

            }
        }

    }
}
