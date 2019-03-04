using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects;
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
    public partial class Service:IDbServer
    {
        public string DbServerGetConnStr(YL_DB_SERVER ent)
        {
            switch (ent.TYPE.ToUpper())
            {
                case "DB2":
                    return string.Format("Server={0}:{1};Database={2};UID={3};PWD={4};Connect Timeout =200", ent.IP, ent.PORT, ent.DBNAME, ent.DBUID, ent.PASSWORD);
                case "ORACLE":
                    return string.Format("Data Source={0}:{1}/{2};User Id={3};Password={4};Connect Timeout =200", ent.IP, ent.PORT, ent.DBNAME, ent.DBUID, ent.PASSWORD);
                case "SQL":
                    return string.Format("data source={0};initial catalog={2};user id={3};password={4}", ent.IP, ent.PORT, ent.DBNAME, ent.DBUID, ent.PASSWORD);
            }
            return "";
        }



        public object DbServerScalar(int id, string sql)
        {
            using (DBEntities db = new DBEntities())
            {
                if (id == 1)
                {
                    var conn = db.Database.Connection;
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    var reOjb = cmd.ExecuteScalar();
                    conn.Close();
                    return reOjb;
                }

                var ent = db.YL_DB_SERVER.SingleOrDefault(x => x.ID == id);
                if (ent == null) return null;
                switch (ent.TYPE.ToUpper())
                {
                    case "DB2":
                        return DbHelper.DB2Helper.ExecuteScalar(DbServerGetConnStr(ent), CommandType.Text, sql);
                    case "ORACLE":
                        return DbHelper.OracleHelper.ExecuteScalar(DbServerGetConnStr(ent), CommandType.Text, sql);
                    case "SQL":
                        return DbHelper.SqlHelper.ExecuteScalar(DbServerGetConnStr(ent), CommandType.Text, sql);
                }
                return null;
            }
        }

        public int DbServerNonQuery(int id, string sql)
        {
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_DB_SERVER.SingleOrDefault(x => x.ID == id);
                if (ent == null) return 0;
                return DbServerNonQuery(ent.TYPE, DbServerGetConnStr(ent), sql);
            }
        }

        public int DbServerNonQuery(string dbType, string ConnStr,string sql)
        {
            switch (dbType.ToUpper())
            {
                case "DB2":
                    return DbHelper.DB2Helper.ExecuteNonQuery(ConnStr, CommandType.Text, sql);
                case "ORACLE":
                    return DbHelper.OracleHelper.ExecuteNonQuery(ConnStr, CommandType.Text, sql);
                case "SQL":
                    return DbHelper.SqlHelper.ExecuteNonQuery(ConnStr, CommandType.Text, sql);
                default:
                    return 0;
            }
        }

        public DataTable DbServerDataTable(int id, string sql)
        {
            using (DBEntities db = new DBEntities())
            {
                if (id == 1)
                {
                    var conn = db.Database.Connection;
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    DataTable table = new DataTable();
                    try
                    {
                        table.Load(cmd.ExecuteReader());
                        conn.Close();
                    }
                    catch(Exception e) {
                        conn.Close();
                        throw e;
                    }
                    return table;
                }

                var ent = db.YL_DB_SERVER.SingleOrDefault(x => x.ID == id);
                if (ent == null) return null;
                switch (ent.TYPE.ToUpper())
                {
                    case "DB2":
                        return DbHelper.DB2Helper.ExecuteDataset(DbServerGetConnStr(ent), CommandType.Text, sql).Tables[0];
                    case "ORACLE":
                        return DbHelper.OracleHelper.ExecuteDataset(DbServerGetConnStr(ent), CommandType.Text, sql).Tables[0];
                    case "SQL":
                        return DbHelper.SqlHelper.ExecuteDataset(DbServerGetConnStr(ent), CommandType.Text, sql).Tables[0];
                }
                return null;
            }
        }

        public DataTable DbServerGetColumns(int id, string sql)
        {
            using (DBEntities db = new DBEntities())
            {


                var ent = db.YL_DB_SERVER.SingleOrDefault(x => x.ID == id);
                if (ent == null) return null;
                switch (ent.TYPE.ToUpper())
                {
                    case "DB2":
                        sql = string.Format("SELECT * FROM ( {0} ) FETCH FIRST 1 ROW ONLY ", sql);
                        break;
                    case "ORACLE":
                        sql = string.Format("SELECT * FROM({0}) T WHERE ROWNUM<2", sql);
                        break;
                    case "SQL":
                        sql = string.Format("SELECT TOP 0 * FROM ({0}) T", sql);
                        break;
                }

                if (id == 1)
                {
                    var conn = db.Database.Connection;
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    DataTable table = new DataTable();
                    table.Load(cmd.ExecuteReader());
                    conn.Close();
                    return table;
                }

                switch (ent.TYPE.ToUpper())
                {
                    case "DB2":
                        return DbHelper.DB2Helper.ExecuteDataset(DbServerGetConnStr(ent), CommandType.Text, sql).Tables[0];
                    case "ORACLE":
                        return DbHelper.OracleHelper.ExecuteDataset(DbServerGetConnStr(ent), CommandType.Text, sql).Tables[0];
                    case "SQL":
                        return DbHelper.SqlHelper.ExecuteDataset(DbServerGetConnStr(ent), CommandType.Text, sql).Tables[0];
                }

                return null;
            }
        }

        public int DbServerGetSqlCount(int id, string sql)
        {
            string sqlStr = "SELECT COUNT(1) FROM (" + sql + " ) T";
            sqlStr = Convert8859P1ToGB2312(sqlStr);
            using (DBEntities db = new DBEntities())
            {
                if (id == 1)
                {
                    var conn = db.Database.Connection;
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    var tmp= cmd.ExecuteScalar();
                    conn.Close();
                    return Convert.ToInt32(tmp);
                }

                var ent = db.YL_DB_SERVER.SingleOrDefault(x => x.ID == id);
                if (ent == null) return 0;
                object obj = 0;
                switch (ent.TYPE.ToUpper())
                {
                    case "DB2":
                        obj = DbHelper.DB2Helper.ExecuteScalar(DbServerGetConnStr(ent), CommandType.Text, sqlStr);
                        break;
                    case "ORACLE":
                        obj = DbHelper.OracleHelper.ExecuteScalar(DbServerGetConnStr(ent), CommandType.Text, sqlStr);
                        break;
                    case "SQL":
                        obj = DbHelper.SqlHelper.ExecuteScalar(DbServerGetConnStr(ent), CommandType.Text, sqlStr);
                        break;
                }
                return Convert.ToInt32(obj);
            }
        }


        public string DbServerGetInserIntoSql(string tableName, DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sb.Append("@" + dt.Columns[i].Caption + ",");
            }
            if (dt.Columns.Count > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return string.Format("insert into {0} values({1})", tableName, sb.ToString());
        }

        public string DbServerColumns(int dbId, string sql)
        {

            DataTable dt = ExecuteGetNullTable(dbId, sql);
            StringBuilder columns = new StringBuilder("[\r\n");
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                columns.Append(string.Format("{{\"field\": \"{0}\", \"title\": \"{0}\",\"sortable\":true}},\r\n", dt.Columns[i].Caption));
            }
            if (dt.Columns.Count > 0) columns.Remove(columns.Length - 3, 1);
            columns.Append("]\r\n");
            return columns.ToString();
        }

        public string DbServerSqlJson(string loginKey, ref ErrorInfo err,int dbId, string sql, int pageIndex, int pageSize, string orderStr)
        {
            if (pageIndex < 0) pageIndex = 1;
            if (pageSize < 0) pageSize = 10;
            if (string.IsNullOrEmpty(orderStr)) orderStr = "ID DESC";
            string sqlStr = @"
SELECT * FROM 
	(
		SELECT ROW_NUMBER() OVER (ORDER BY {1}) N, T.* FROM 
			( 
				{0}
			) T 
	)  T1 WHERE N>{2} AND N<={3}
";
            int startNum = (pageIndex - 1) * pageSize;
            sqlStr = string.Format(sqlStr, sql, orderStr, startNum, startNum + pageSize);
            DataTable dt = new DataTable();
            try
            {
                dt = ExecuteSqlToTable(dbId,sqlStr);
            }
            catch
            {
                dt = ExecuteSqlToTable(dbId,sql);
            }
            int allNum = ExecuteCount(dbId, sql);
            return JSON.DecodeToStr(allNum, dt);
        }

        public IList<System.Web.Mvc.SelectListItem> DbServerGetAllDb()
        {
            using (DBEntities db = new DBEntities())
            {
                return db.YL_DB_SERVER.OrderBy(x => x.ID).ToList().Select(x => new System.Web.Mvc.SelectListItem { Value=x.ID.ToString(),Text=x.NICKNAME }).ToList() ;
            }
        }

        public byte[] DbServerXlsByte(int db, string sql,ref int allNum)
        {
            DataTable dt = ExecuteSqlToTable(db, sql);
            allNum = dt.Rows.Count;
            return ExcelHelper.ExportDTtoByte(dt, "数据");
        }


        public byte[] DbServerTxtByte(int id, string sql)
        {
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_DB_SERVER.SingleOrDefault(x => x.ID == id);
                if (ent == null) return null;
                string reStr = "";
                switch (ent.TYPE.ToUpper())
                {
                    case "DB2":
                        reStr = DbHelper.DB2Helper.ExportTxt(DbServerGetConnStr(ent), sql);
                        break;
                    case "ORACLE":
                        reStr = DbHelper.OracleHelper.ExportTxt(DbServerGetConnStr(ent), sql);
                        break;
                }
                return Encoding.Default.GetBytes(reStr);
            }
        }


        public string DbServerGetDbType(int dbId)
        {
            using (DBEntities db = new DBEntities())
            {
                if (dbId == 1)
                {
                    var dbName = db.Database.Connection.GetType().Name;
                    return dbName.Replace("Connection", "");
                }
                else
                {
                    return db.YL_DB_SERVER.Single(x => x.ID == dbId).TYPE;
                }
            }
        }
    }
}
