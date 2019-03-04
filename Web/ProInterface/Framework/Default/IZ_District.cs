
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProInterface.Models;

namespace ProInterface
{
    /// <summary>
    /// 组织结构内容
    /// </summary>
    public interface IZ_District
    {
        #region 默认接口

        /// <summary>
        /// 查找所有
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <returns></returns>
        IList<DISTRICT> District_FindAll(string loginKey, ref ErrorInfo err);

        /// <summary>
        /// 添加组织结构
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <returns>添加组织结构</returns>
        object District_Add(string loginKey, ref ErrorInfo err, DISTRICT inEnt);
        /// <summary>
        /// 修改组织结构
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改组织结构</returns>
        bool District_Edit(string loginKey, ref ErrorInfo err, DISTRICT inEnt, IList<string> allPar);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="Id">条件lambda表达表</param>
        /// <returns></returns>
        DISTRICT District_SingleId(string loginKey, ref ErrorInfo err, int Id);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="whereLambda">条件lambda表达表</param>
        /// <returns></returns>
        DISTRICT District_Single(string loginKey, ref ErrorInfo err, string whereLambda);

        /// <summary>
        /// 删除组织结构
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="id">删除组织结构</param>
        /// <returns></returns>
        bool District_Delete(string loginKey, ref ErrorInfo err, int id);
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="whereLambda">条件lambda表达表</param>
        /// <param name="orderField">排序</param>
        /// <param name="orderBy">排序方式</param>
        /// <returns>返回满足条件的泛型</returns>
        IList<DISTRICT> District_Where(string loginKey, ref ErrorInfo err, int pageIndex, int pageSize, string whereLambda, string orderField, string orderBy);

        /// <summary>
        /// 满足条件记录数
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="whereLambda">条件lambda表达表</param>
        /// <returns></returns>
        int District_Count(string loginKey, ref ErrorInfo err, string whereLambda);

        #endregion
    }
}
