using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ProInterface
{
    public interface IDbServer
    {

        /// <summary>
        /// 返回SQL总记录数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        int DbServerGetSqlCount(int id, string sql);

        /// <summary>
        /// 返回第一个值的对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        object DbServerScalar(int id, string sql);
        /// <summary>
        /// 用户执行SQL
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        int DbServerNonQuery(int id, string sql);
        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        DataTable DbServerDataTable(int id, string sql);

        /// <summary>
        /// 导出文本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        byte[] DbServerTxtByte(int id, string sql);
        /// <summary>
        /// 获取所有数据库列表
        /// </summary>
        /// <returns></returns>
        IList<System.Web.Mvc.SelectListItem> DbServerGetAllDb();
        /// <summary>
        /// 获取语句的列名
        /// </summary>
        /// <param name="dbId"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        string DbServerColumns(int dbId, string sql);

        string DbServerSqlJson(string loginKey, ref ErrorInfo err, int dbId, string sql, int pageIndex, int pageSize, string orderStr);

        /// <summary>
        /// 生成excel数据
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="allNum"></param>
        /// <returns></returns>
        byte[] DbServerXlsByte(int db, string sql, ref int allNum);

        /// <summary>
        /// 获取数据库类型
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        string DbServerGetDbType(int dbId);
    }
}
