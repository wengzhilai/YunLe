using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Linq;
using System.Reflection;
using System.Text;
using ProInterface.Models;
using ProInterface;
using System.Linq.Expressions;
using LINQExtensions;
using System.IO;
using System.Data;
using System.Data.Common;
namespace ProServer
{
    public partial class Service
    {
        public int ExecuteCount(string sql, int dbId = 1)
        {
            return ExecuteCount(dbId, sql);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteCount(int dbId,string sql)
        {
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_DB_SERVER.SingleOrDefault(x => x.ID == dbId);
                if (ent != null)
                {
                    switch (ent.TYPE.ToUpper())
                    {
                        case "SQL":
                            var orderPlace = sql.ToLower().Replace("\r\n", "  ").IndexOf(" order ");
                            if (orderPlace > 1)
                            {
                                sql = sql.Substring(0, orderPlace);
                            }
                            break;
                    }
                }

                string withStr = "";
                string WithStartStr = "-----WithStart-----";
                string WithEndStr = "-----WithEnd-----";
                int WithStartP = sql.IndexOf(WithStartStr);
                int WithEndP = sql.IndexOf(WithEndStr);
                if (WithStartP > -1 && WithEndP > -1 && WithEndP > WithStartP)
                {
                    withStr = sql.Substring(WithStartStr.Length, WithEndP - WithStartP - WithStartStr.Length);
                    sql = sql.Substring(WithEndP + WithEndStr.Length);
                }


                string sqlStr = withStr + "SELECT COUNT(1) FROM (" + sql + " ) T";

                return Convert.ToInt32(DbServerScalar(dbId, sqlStr));

                //using (DbConnection conn = db.Database.Connection)
                //{
                //    using (var cmd = conn.CreateCommand())
                //    {
                //        cmd.CommandText = sqlStr;
                //        conn.Open();
                //        int reInt = Convert.ToInt32(cmd.ExecuteScalar());
                //        conn.Close();
                //        return reInt;
                //    }
                //}
            }
        }

        public object ExecuteObject(int dbId,string sqlStr)
        {
            return DbServerScalar(dbId, sqlStr);
            //using (DBEntities db = new DBEntities())
            //{

            //    using (DbConnection conn = db.Database.Connection)
            //    {
            //        using (var cmd = conn.CreateCommand())
            //        {
            //            cmd.CommandText = sqlStr;
            //            conn.Open();
            //            object reInt = cmd.ExecuteScalar();
            //            conn.Close();
            //            return reInt;
            //        }
            //    }
            //}
        }

        public int ExecuteNonQuery(int dbId,string sqlStr)
        {
            return DbServerNonQuery(dbId, sqlStr);
            //using (DBEntities db = new DBEntities())
            //{

            //    using (DbConnection conn = db.Database.Connection)
            //    {
            //        using (var cmd = conn.CreateCommand())
            //        {
            //            cmd.CommandText = sqlStr;
            //            conn.Open();
            //            int reInt = cmd.ExecuteNonQuery();
            //            conn.Close();
            //            return reInt;
            //        }
            //    }
            //}
        }

        public DataTable ExecuteSql(string sql, int pageIndex, int pageSize, string orderStr, string whereStr, int dbId, IList<string> fieldList = null)
        {
            if (!ProcessSqlStr(whereStr) || !ProcessSqlStr(orderStr))
            {
                return new DataTable();
            }
            if (pageIndex < 0) pageIndex = 1;
            if (pageSize < 0) pageSize = 10;
            if (!string.IsNullOrEmpty(whereStr) && !whereStr.Trim().ToLower().StartsWith("where")) whereStr = "where " + whereStr;

            var dbType = DbServerGetDbType(dbId);

            if (dbType == "Sql")
            {
                var orderPlace = sql.ToLower().Replace("\r\n", "  ").IndexOf(" order ");
                if (orderPlace > 1)
                {
                    sql = sql.Substring(0, orderPlace);
                }
            }

            if (string.IsNullOrEmpty(orderStr))
            {
                orderStr = "ROWNUM ";
                switch (dbType)
                {
                    case "DB2":
                        orderStr = string.Format(" ROW_NUMBER() OVER( PARTITION BY 1 ) ");
                        break;
                    case "Oracle":
                        orderStr = "ROWNUM ";
                        break;
                    case "Sql":
                        DataTable dt = ExecuteGetNullTable(dbId, sql);
                        orderStr = dt.Columns[0].Caption;
                        orderStr = string.Format("ROW_NUMBER() OVER (ORDER BY {0} DESC)", orderStr);
                        break;
                }
            }
            else
            {
                switch (dbType)
                {
                    case "DB2":
                        orderStr = string.Format(" ROW_NUMBER() OVER( PARTITION BY 1 ORDER BY {0}) ", orderStr);
                        break;
                    case "Oracle":
                        orderStr = string.Format("ROW_NUMBER() OVER (ORDER BY {0})", orderStr);
                        break;
                    case "Sql":
                        orderStr = string.Format("ROW_NUMBER() OVER (ORDER BY {0})", orderStr);
                        break;
                }
            }



            string withStr = "";
            string WithStartStr = "-----WithStart-----";
            string WithEndStr = "-----WithEnd-----";
            int WithStartP = sql.IndexOf(WithStartStr);
            int WithEndP = sql.IndexOf(WithEndStr);
            if (WithStartP > -1 && WithEndP > -1 && WithEndP > WithStartP)
            {
                withStr = sql.Substring(WithStartStr.Length, WithEndP - WithStartP - WithStartStr.Length);
                sql = sql.Substring(WithEndP + WithEndStr.Length);
            }


            string t1File = "*";
            string tFile = "*";
            if (fieldList != null)
            {
                t1File = string.Join(",T1.", fieldList);
                tFile = string.Join(",T.", fieldList);
            }
            string sqlStr = @"
{5}
SELECT T1.{7} FROM 
	(
		SELECT {1} N, T.{6} FROM 
			( 
				{0}
			) T {4}
	)  T1 WHERE T1.N>{2} AND T1.N<={3}
";
            int startNum = (pageIndex - 1) * pageSize;
            sqlStr = string.Format(sqlStr, sql, orderStr, startNum, startNum + pageSize, whereStr, withStr, tFile, t1File);

            return ExecuteSqlToTable(dbId, sqlStr);
        }

        public DataTable ExecuteSqlToTable(int dbId,string sqlStr)
        {
            return DbServerDataTable(dbId, sqlStr);
        }
 
        public static DbDataAdapter CreateDataAdapter(DbConnection connection)
        {
            DbDataAdapter adapter; //we can't construct an adapter directly
            //So let's run around the block 3 times, before potentially crashing

            if (connection is System.Data.SqlClient.SqlConnection)
                adapter = new System.Data.SqlClient.SqlDataAdapter();
            else if (connection is System.Data.OleDb.OleDbConnection)
                adapter = new System.Data.OleDb.OleDbDataAdapter();
            else if (connection is System.Data.Odbc.OdbcConnection)
                adapter = new System.Data.Odbc.OdbcDataAdapter();
            //TODO: Add more DbConnection kinds as they become invented
            else
            {
                string fullName = connection.GetType().FullName;
                adapter = DbProviderFactories.GetFactory(fullName.Substring(0, fullName.LastIndexOf("."))).CreateDataAdapter();
            }
            return adapter;
        }

        public DataTable ExecuteSqlAll(int dbId,string sql, string orderStr, string whereStr,ref string sqlStr)
        {
            if (!string.IsNullOrEmpty(orderStr))
            {
                orderStr = string.Format(" ORDER BY {0} ", orderStr);
            }
            if (!string.IsNullOrEmpty(whereStr) && !whereStr.Trim().ToLower().StartsWith("where")) whereStr = "where " + whereStr;

            string withStr = "";
            string WithStartStr = "-----WithStart-----";
            string WithEndStr = "-----WithEnd-----";
            int WithStartP = sql.IndexOf(WithStartStr);
            int WithEndP = sql.IndexOf(WithEndStr);
            if (WithStartP > -1 && WithEndP > -1 && WithEndP > WithStartP)
            {
                withStr = sql.Substring(WithStartStr.Length, WithEndP - WithStartP - WithStartStr.Length);
                sql = sql.Substring(WithEndP + WithEndStr.Length);
            }



            sqlStr = @"
        {3}
		SELECT * FROM 
			( 
				{0}
			) T {2} {1}
";
            sqlStr = string.Format(sqlStr, sql, orderStr, whereStr, withStr);

            return ExecuteSqlToTable(dbId, sqlStr);

        }



        public DataTable ExecuteGetNullTable(int dbId, string sql)
        {

            string dbType = Fun.GetDataBaseType();
            switch (dbType)
            {
                case "Sql":
                    var orderPlace = sql.ToLower().Replace("\r\n", "  ").IndexOf(" order ");
                    if (orderPlace > 1)
                    {
                        sql = sql.Substring(0, orderPlace);
                    }
                    break;
            }

            string withStr = "";
            string WithStartStr = "-----WithStart-----";
            string WithEndStr = "-----WithEnd-----";
            int WithStartP = sql.IndexOf(WithStartStr);
            int WithEndP = sql.IndexOf(WithEndStr);
            if (WithStartP > -1 && WithEndP > -1 && WithEndP > WithStartP)
            {
                withStr = sql.Substring(WithStartStr.Length, WithEndP - WithStartP - WithStartStr.Length);
                sql = sql.Substring(WithEndP + WithEndStr.Length);
            }


            string sqlStr = withStr + "SELECT * FROM ( " + sql + " ) T WHERE 1<>1";
            return ExecuteSqlToTable(dbId, sqlStr);

        }
        /// <summary>
        /// 检测危险数据
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        private bool ProcessSqlStr(string Str)
        {
            bool ReturnValue = true;
            if (string.IsNullOrEmpty(Str))
            {
                return true;
            }
            Str = Str.ToLower();
            try
            {
                if (Str != "")
                {
                    //                    string SqlStr = @" select * |and '|or '| insert into | delete from | alter table | update | create table | createview | drop view|cr
                    //eateindex|dropindex|createprocedure|dropprocedure|createtrigger|droptrigger|createschema|dro
                    //pschema|createdomain|alterdomain|dropdomain|);|select@|declare@|print@|char(| select ";
                    string SqlStr = " insert | create | update | alter | drop | print | merge ";
                    string[] anySqlStr = SqlStr.Split('|');
                    foreach (string ss in anySqlStr)
                    {
                        if (Str.IndexOf(ss) >= 0)
                        {
                            ReturnValue = false;
                        }
                    }
                }
            }
            catch
            {
                ReturnValue = false;
            }
            return ReturnValue;
        }
        public static string Convert8859P1ToGB2312(string s)
        {
            //return System.Text.Encoding.GetEncoding("iso-8859-1").GetString(System.Text.Encoding.Default.GetBytes(s));
            return s;
        }
    }
}
