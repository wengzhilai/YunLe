using System;
using System.Collections.Generic;
using System.Text;
using ProInterface.Models;

namespace ProInterface
{
    /// <summary>
    /// 模块
    /// </summary>
    public interface IModule : IZ_Module
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="whereLambda">条件lambda表达表</param>
        /// <param name="orderLambda">排序</param>
        /// <param name="orderBy">排序方式</param>
        /// <returns>返回满足条件的泛型</returns>
        IList<MODULE> SysModuleWhere(string loginKey, ref ErrorInfo err, int pageIndex, int pageSize, string whereLambda, string orderLambda, string orderBy);

        /// <summary>
        /// 满足条件记录数
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="whereLambda">条件lambda表达表</param>
        /// <returns></returns>
        int SysModuleCount(string loginKey, ref ErrorInfo err, string whereLambda);


        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="id">删除模块</param>
        /// <returns></returns>
        bool ModuleDelete(string loginKey, ref ErrorInfo err, int id);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="code">代码</param>
        /// <returns></returns>
        MODULE ModuleSingleCode(string loginKey, ref ErrorInfo err, string code);



        /// <summary>
        /// 修改模块
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改模块</returns>
        bool ModuleSave(string loginKey, ref ErrorInfo err, TModule inEnt, IList<string> allPar);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="Id">条件lambda表达表</param>
        /// <returns></returns>
        TModule ModuleSingleId(string loginKey, ref ErrorInfo err, int Id);

        /// <summary>
        /// 获取用户的模块
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        IList<IdText> ModuleByNameOrCode(string loginKey, ref ErrorInfo err, string key);

        /// <summary>
        /// 判断用户是否有权限
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="url">地址</param>
        /// <returns></returns>
        bool ModuleUserAuthority(string loginKey, ref ErrorInfo err, string url);
    }
}
