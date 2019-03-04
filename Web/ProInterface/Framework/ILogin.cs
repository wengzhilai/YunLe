using ProInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface
{
    public interface ILogin:IZ_Login
    {
        /// <summary>
        /// 密码难证
        /// </summary>
        /// <param name="err"></param>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool LoginByPassWord(ref ErrorInfo err, string loginName, string password);

        /// <summary>
        /// 获取所有权限
        /// </summary>
        /// <returns></returns>
        IList<System.Web.Mvc.SelectListItem> LoginAllOauth();

        /// <summary>
        /// 添加登录工号
        /// </summary>
        bool LoginAdd(int appId, LOGIN inEnt);

        /// <summary>
        /// 用户授权登录
        /// </summary>
        /// <param name="err"></param>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <param name="loginIP"></param>
        /// <returns></returns>
        IList<OAUTH> LoginOauth(ref ErrorInfo err, string loginName, string password, string loginIP);
    }
}
