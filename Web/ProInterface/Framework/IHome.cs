using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ProInterface
{
    public interface IHome
    {
        /// <summary>
        /// 获取用户菜单
        /// </summary>
        /// <returns></returns>
        string GetAllMenus(string loginKey, ref ErrorInfo err, UrlHelper url);
        /// <summary>
        /// 获取同级菜单
        /// </summary>
        /// <returns></returns>
        string GetMenuItem(string loginKey, ref ErrorInfo err, int parentid, string statu, UrlHelper url);
        /// <summary>
        /// 查询模块代码
        /// </summary>
        /// <returns></returns>  
        string GetMenuCode(string loginKey, ref ErrorInfo err, string code);

        /// <summary>
        /// 获取有地址模块
        /// </summary>
        /// <returns></returns>
        string GetModule(string loginKey, ref ErrorInfo err);
        /// <summary>
        /// 获取快速通道
        /// </summary>
        /// <returns></returns>
        string GetSepeed(string loginKey, UrlHelper url);
        /// <summary>
        /// 同步菜单
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        string GetTongBu(string loginKey, ref ErrorInfo err, int id, UrlHelper url);
        /// <summary>
        /// 获取用户菜单
        /// </summary>
        /// <returns></returns>
        string[] HomeGetParentMenu(string loginKey, ref ErrorInfo err, UrlHelper url);
    }
}
