
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
    /// 口径脚本内容
    /// </summary>
    public interface IZ_Script
    {
        #region 默认接口

        /// <summary>
        /// 修改口径脚本
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改口径脚本</returns>
        bool Script_Save(string loginKey, ref ErrorInfo err, SCRIPT inEnt, IList<string> allPar);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="Id">条件lambda表达表</param>
        /// <returns></returns>
        SCRIPT Script_SingleId(string loginKey, ref ErrorInfo err, int Id);

        /// <summary>
        /// 删除口径脚本
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="id">删除口径脚本</param>
        /// <returns></returns>
        bool Script_Delete(string loginKey, ref ErrorInfo err, int id);
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
        IList<SCRIPT> Script_Where(string loginKey, ref ErrorInfo err, int pageIndex, int pageSize, string whereLambda, string orderField, string orderBy);

        /// <summary>
        /// 满足条件记录数
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="whereLambda">条件lambda表达表</param>
        /// <returns></returns>
        int Script_Count(string loginKey, ref ErrorInfo err, string whereLambda);

        #endregion
    }
}
