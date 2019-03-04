using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ProInterface
{
    public interface IExecute
    {
        string ExecuteGetConStr();
        string ExecuteGetDbType();
        int ExecuteCount(string sql);
        object ExecuteObject(string sqlStr);
        object ExecuteNonQuery(string sqlStr);
        string ExecuteMakePageSql(string sql, int pageIndex, int pageSize, string orderStr, string whereStr, string dbType);
        DataTable ExecuteSql(string sql, int pageIndex, int pageSize, string orderStr, string whereStr);
        DataTable ExecuteSqlToTable(string sqlStr);
        DataTable ExecuteSqlAll(string sql, string orderStr = null, string whereStr = null);
        DataTable ExecuteGetNullTable(string sql);
    }
}
