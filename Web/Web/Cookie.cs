using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web
{
    public class Cookie
    {
        /// <summary>
        /// Cookies赋值
        /// </summary>
        /// <param name="strName">主键</param>
        /// <param name="strValue">键值</param>
        /// <param name="strDay">有效天数</param>
        /// <returns></returns>
        public static bool setCookie(string strName, string strValue, int strDay)
        {
            try
            {
                HttpCookie Cookie = new HttpCookie(strName);
                //Cookie.Domain = ".xxx.com";//当要跨域名访问的时候,给cookie指定域名即可,格式为.xxx.com
                Cookie.Expires = DateTime.Now.AddDays(strDay);
                Cookie.Value = strValue;
                System.Web.HttpContext.Current.Response.Cookies.Add(Cookie);
                System.Threading.Thread.Sleep(10);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 读取Cookies
        /// </summary>
        /// <param name="strName">主键</param>
        /// <returns></returns>
        public static string getCookie(string strName)
        {
            HttpCookie Cookie = System.Web.HttpContext.Current.Request.Cookies[strName];
            if (Cookie != null && Cookie.Value!=null)
            {

                return Cookie.Value.ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 删除Cookies
        /// </summary>
        /// <param name="strName">主键</param>
        /// <returns></returns>
        public static bool DelCookie(string strName)
        {
            try
            {

                HttpCookie Cookie = new HttpCookie(strName);
                //Cookie.Domain = ".xxx.com";//当要跨域名访问的时候,给cookie指定域名即可,格式为.xxx.com
                Cookie.Expires = DateTime.Now.AddDays(-1);
                System.Web.HttpContext.Current.Response.Cookies.Add(Cookie);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool ClearCookie()
        {
            var obj = System.Web.HttpContext.Current.Response.Cookies;
            obj.Clear();
            return true;
        }
    } 
}