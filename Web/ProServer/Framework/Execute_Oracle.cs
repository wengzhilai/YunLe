//using Oracle.ManagedDataAccess.Client;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.IO;
//using System.Linq;
//using System.Text;

//namespace ProServer
//{
//    public class Execute_Oracle
//    {
//        public static bool Import(int recc, string connectStr, string sql, OracleParameter[] allOraclePara)
//        {
//            //设置一个数据库的连接串
//            OracleConnection conn = new OracleConnection(connectStr);
//            OracleCommand command = new OracleCommand();
//            command.Connection = conn;
//            //到此为止，还都是我们熟悉的代码，下面就要开始喽
//            //这个参数需要指定每次批插入的记录数
//            command.ArrayBindCount = recc;
//            //在这个命令行中,用到了参数,参数我们很熟悉,但是这个参数在传值的时候
//            //用到的是数组,而不是单个的值,这就是它独特的地方
//            command.CommandText = sql;
//            conn.Open();
//            //下面定义几个数组,分别表示三个字段,数组的长度由参数直接给出
//            foreach (var t in allOraclePara)
//            {
//                command.Parameters.Add(t);
//            }
//            //这个调用将把参数数组传进SQL,同时写入数据库
//            command.ExecuteNonQuery();
//            return true;
//        }

//        public static string GetInserIntoSql(string tableName, DataTable dt)
//        {
//            StringBuilder sb = new StringBuilder();
//            for (int i = 0; i < dt.Columns.Count; i++)
//            {
//                sb.Append("@" + dt.Columns[i].Caption + ",");
//            }
//            if (dt.Columns.Count > 0)
//            {
//                sb.Remove(sb.Length - 1, 1);
//            }
//            return string.Format("insert into {0} values({1})", tableName, sb.ToString());
//        }

        
//        public static OracleParameter[] GetAllOraclePara(DataTable dt)
//        {
//            int rowNum = dt.Rows.Count;
//            int colNum = dt.Columns.Count;

//            OracleParameter[] rePara = new OracleParameter[colNum];

//            IList<object[]> valueArr = new List<object[]>();
//            for (int i = 0; i < colNum; i++)
//            {
//                object[] obj = new object[rowNum];
//                for (int x = 0; x < rowNum; x++)
//                {
//                    obj[x] = dt.Rows[x][i];
//                }
//                valueArr.Add(obj);
//            }

//            for (int i = 0; i < colNum; i++)
//            {
//                OracleParameter deptNoParam = new OracleParameter(dt.Columns[i].Caption, GetOracleType(dt.Columns[i].DataType));
//                deptNoParam.Direction = ParameterDirection.Input;
//                deptNoParam.Value = valueArr[i];
//                rePara[i] = deptNoParam;
//            }
//            return rePara;
//        }
//        public static OracleDbType GetOracleType(Type type)
//        {
//            switch (type.Name)
//            {
//                case "System.Int32":
//                    return OracleDbType.Int32;
//                default:
//                    return OracleDbType.Varchar2;

//            }
//        }


//        public static int ExecuteStoreCommand(string sql)
//        {
//            using (DBEntities db = new DBEntities())
//            {
//                string sqlStr = "SELECT COUNT(1) FROM (" + sql + " ) T";
//                using (OracleConnection sqlCon = new OracleConnection(db.Database.Connection.ConnectionString))
//                {
//                    using (OracleCommand cmd = new OracleCommand(sqlStr, sqlCon))
//                    {
//                        sqlCon.Open();
//                        int reInt = Convert.ToInt32(cmd.ExecuteScalar());
//                        sqlCon.Close();
//                        return reInt;
//                    }
//                }
//            }
//        }

//        public static DataTable ExecuteSql(string sql, int pageIndex, int pageSize, string orderStr, string whereStr)
//        {

//            if (pageIndex < 0) pageIndex = 1;
//            if (pageSize < 0) pageSize = 10;
//            if (string.IsNullOrEmpty(orderStr)) orderStr = "ID DESC";
//            if (!string.IsNullOrEmpty(whereStr) && !whereStr.Trim().ToLower().StartsWith("where")) whereStr = "where " + whereStr;
//            string sqlStr = @"
//SELECT * FROM 
//	(
//		SELECT ROW_NUMBER() OVER (ORDER BY {1}) N, T.* FROM 
//			( 
//				{0}
//			) T {4}
//	)  T1 WHERE N>{2} AND N<={3}
//";
//            int startNum = (pageIndex - 1) * pageSize;
//            sqlStr = string.Format(sqlStr, sql, orderStr, startNum, startNum + pageSize, whereStr);

//            using (DBEntities db = new DBEntities())
//            {
//                using (OracleConnection sqlCon = new OracleConnection(db.Database.Connection.ConnectionString))
//                {
//                    using (OracleCommand cmd = new OracleCommand(sqlStr, sqlCon))
//                    {
//                        using (OracleDataAdapter dataAdapter = new OracleDataAdapter(cmd))
//                        {
//                            DataTable dataTable = new DataTable("Table1");
//                            dataAdapter.Fill(dataTable);
//                            return dataTable;
//                        }
//                    }
//                }
//            }
//        }
//        public static DataTable ExecuteSqlAll(string sql, string orderStr, string whereStr)
//        {
//            if (!string.IsNullOrEmpty(whereStr) && !whereStr.Trim().ToLower().StartsWith("where")) whereStr = "where " + whereStr;
//            string sqlStr = @"
//		SELECT * FROM 
//			( 
//				{0}
//			) T {2} ORDER BY {1}
//";
//            sqlStr = string.Format(sqlStr, sql, orderStr, whereStr);

//            using (DBEntities db = new DBEntities())
//            {
//                using (OracleConnection sqlCon = new OracleConnection(db.Database.Connection.ConnectionString))
//                {
//                    using (OracleCommand cmd = new OracleCommand(sqlStr, sqlCon))
//                    {
//                        using (OracleDataAdapter dataAdapter = new OracleDataAdapter(cmd))
//                        {
//                            DataTable dataTable = new DataTable("Table1");
//                            dataAdapter.Fill(dataTable);
//                            return dataTable;
//                        }
//                    }
//                }
//            }
//        }



//        public static DataTable ExecuteGetNullTable(string sql)
//        {
//            string sqlStr = "SELECT * FROM ( " + sql + " ) T WHERE 1<>1";
//            using (DBEntities db = new DBEntities())
//            {
//                using (OracleConnection sqlCon = new OracleConnection(db.Database.Connection.ConnectionString))
//                {
//                    using (OracleCommand cmd = new OracleCommand(sqlStr, sqlCon))
//                    {
//                        using (OracleDataAdapter dataAdapter = new OracleDataAdapter(cmd))
//                        {
//                            DataTable dataTable = new DataTable("Table1");
//                            dataAdapter.Fill(dataTable);
//                            return dataTable;
//                        }
//                    }
//                }
//            }
//        }

//        public static string GetOracleDataType(string CSharpType)
//        {
//            CSharpType = CSharpType.Replace("System.", "");
//            switch (CSharpType)
//            {
//                case "Int32":
//                    return "INTEGER";
//                case "Int16":
//                    return "SMALLINT";
//                case "Int64":
//                    return "INTEGER";
//                case "DateTime":
//                    return "TIMESTAMP";
//                case "Decimal":
//                    return "DECIMAL";
//                case "Double":
//                    return "DOUBLE(10)";
//                default:
//                    return "VARCHAR(50)";
//            }
//        }
//    }
//}
