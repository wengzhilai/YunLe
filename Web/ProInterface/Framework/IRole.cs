using ProInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface
{
    public interface IRole:IZ_Role
    {
        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <returns>修改角色</returns>
        bool RoleSave(string loginKey, ref ErrorInfo err, TRole inEnt, IList<string> allPar);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="id">条件lambda表达表</param>
        /// <returns></returns>
        TRole RoleSingle(string loginKey, ref ErrorInfo err, int id);

        /// <summary>
        /// 删除系统角色
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="id">删除系统角色</param>
        /// <returns></returns>
        bool RoleDelete(string loginKey, ref ErrorInfo err, int id);

        /// <summary>
        /// 获取角色不能使用的查询权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="queryId"></param>
        /// <returns></returns>
        IList<System.Web.Mvc.SelectListItem> RoleGetNoAuthority(int roleId, int queryId);

        /// <summary>
        /// 修改角色不能使用的查询权限
        /// </summary>
        bool RoleSaveNoAuthority(string loginKey, ref ErrorInfo err, int roleId, int queryId, string AuthArr);
    }
}
