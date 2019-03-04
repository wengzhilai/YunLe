using ProInterface.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ProInterface
{
    /// <summary>
    /// 查询
    /// </summary>
    public interface IQuery:IZ_Query
    {

        /// <summary>
        /// 修改查询
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改查询</returns>
        bool QuerySave(string loginKey, ref ErrorInfo err, QUERY inEnt, IList<string> allPar);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="Id">条件lambda表达表</param>
        /// <returns></returns>
        TQuery QuerySingleId(string loginKey, ref ErrorInfo err, int Id);

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
        IList<TQuery> QueryWhere(string loginKey, ref ErrorInfo err, int pageIndex, int pageSize, string whereLambda, string orderField, string orderBy);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="whereLambda">条件lambda表达表</param>
        /// <returns></returns>
        TQuery QuerySingle(string loginKey, ref ErrorInfo err, string whereLambda);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="code">代码</param>
        /// <returns></returns>
        TQuery QuerySingleByCode(string loginKey, ref ErrorInfo err, string code);

        /// <summary>
        /// 查询QUERY
        /// </summary>
        /// <param name="queryCode"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderStr"></param>
        /// <param name="whereStr"></param>
        /// <param name="paraList"></param>
        /// <returns></returns>
        DataTable QueryExecute(string loginKey, ref ErrorInfo err, string queryCode, int pageIndex, int pageSize, string orderStr, string whereStr, IList<QueryPara> paraList,ref string sql);

        /// <summary>
        /// 查询总数
        /// </summary>
        /// <param name="queryCode"></param>
        /// <param name="whereStr"></param>
        /// <param name="paraList"></param>
        /// <returns></returns>
        int QueryCount(string loginKey, ref ErrorInfo err, string queryCode, string whereStr, IList<QueryPara> paraList, ref string sql);

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="queryCode"></param>
        /// <param name="orderStr"></param>
        /// <param name="whereStr"></param>
        /// <param name="paraList"></param>
        /// <returns></returns>
        byte[] QueryExportExcel(string loginKey, ref ErrorInfo err, string queryCode, string orderStr, string whereStr, IList<QueryPara> paraList, string allShow, ref string sqlStr, ref int allNum);

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="queryCode"></param>
        /// <param name="orderStr"></param>
        /// <param name="whereStr"></param>
        /// <param name="paraList"></param>
        /// <returns></returns>
        DataTable QueryDataTable(string loginKey, ref ErrorInfo err, string queryCode, string orderStr, string whereStr, IList<QueryPara> paraList, ref string sqlStr);

        /// <summary>
        /// 根据sql返回字段集合
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="queryCode"></param>
        /// <returns></returns>
        IList<QueryCfg> QueryGetCfg(string loginKey, ref ErrorInfo err, string sql, string queryCode);

        string MoudlePathByID(int? query_id);

        string MoudlePathByUrl(string url);

        /// <summary>
        /// 删除查询
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="id">删除查询</param>
        /// <returns></returns>
        bool QueryDelete(string loginKey, ref ErrorInfo err, int id);


    }
}
