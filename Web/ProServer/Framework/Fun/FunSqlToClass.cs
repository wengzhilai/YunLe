//using IBM.Data.DB2;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ProServer
{
    public class FunSqlToClass
    {
        public static object NonQuery(string sql, string dbType, string connStr, string prefix = "")
        {
            int reObj = 0;
            switch (dbType.ToUpper())
            {
                case "DB2":
                    reObj = DbHelper.DB2Helper.ExecuteNonQuery(connStr, CommandType.Text, sql);
                    break;
                case "ORACLE":
                    reObj = DbHelper.OracleHelper.ExecuteNonQuery(connStr, CommandType.Text, sql);
                    break;
            }
            return reObj;
        }

        public static int GetSeqID<T>(string dbType, string connStr, string prefix = "") where T : new()
        {
            T tmp = new T();
            string sql = "SELECT   " + prefix + tmp.GetType().Name + "_SEQ.NEXTVAL   FROM   DUAL";
            object obj = null;
            switch (dbType.ToUpper())
            {
                case "DB2":
                    sql = "SELECT   " + prefix + tmp.GetType().Name + "_SEQ.NEXTVAL   FROM   SYSIBM.DUAL";
                    obj = DbHelper.DB2Helper.ExecuteScalar(connStr, CommandType.Text, sql);
                    break;
                case "ORACLE":
                    sql = "SELECT   " + prefix + tmp.GetType().Name + "_SEQ.NEXTVAL   FROM   DUAL";
                    obj = DbHelper.OracleHelper.ExecuteScalar(connStr, CommandType.Text, sql);
                    break;
                case "Sql":
                    return 0;
            }
            int reInt = Convert.ToInt32(obj);
            return reInt;
        }

        public static bool Save<T>(string dbType, string connStr, T inEnt, string prefix = "",string saveKeys="")
        {
            IList<string> allFiledList = new List<string>();
            PropertyInfo[] proInfoArr = inEnt.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);//得到该类的所有公共属性
            foreach (var t in proInfoArr)
            {
                if (t.PropertyType.Name != "ICollection`1")
                {
                    if (string.IsNullOrEmpty(saveKeys))
                    {
                        allFiledList.Add(t.Name);
                    }
                    else if (Convert.ToString(","+saveKeys+",").IndexOf(","+t.Name+",")>-1)
                    {
                        allFiledList.Add(t.Name);
                    }
                }
            }
            string tableName = string.Format("{0}{1}", prefix, inEnt.GetType().Name);
            string insertSql = "";
            switch (dbType.ToUpper())
            {
                //case "DB2":
                //    IList<DB2Parameter> DB2P = new List<DB2Parameter>();
                //    for (int i = 0; i < allFiledList.Count; i++)
                //    {
                //        var t = allFiledList[i];
                //        PropertyInfo outproInfo = inEnt.GetType().GetProperty(t);

                //        DB2Parameter deptNoParam = new DB2Parameter(t, DbHelper.DB2Helper.GetDbType(outproInfo.PropertyType.FullName));
                //        deptNoParam.Direction = ParameterDirection.Input;
                //        object val = outproInfo.GetValue(inEnt, null);
                //        if (val != null && val != DBNull.Value && val.ToString().Length > 2000)
                //        {
                //            deptNoParam.DB2Type = DB2Type.Clob;
                //        }
                //        deptNoParam.Value = val;
                //        DB2P.Add(deptNoParam);
                //    }
                //    var insertSql = string.Format("INSERT INTO {0} ({1}) VALUES({2})", tableName, string.Join(",", allFiledList), "@" + string.Join(",@", allFiledList));
                //    DbHelper.DB2Helper.ExecuteNonQuery(connStr,CommandType.Text, insertSql, DB2P.ToArray());
                //    break;
                case "ORACLE":
                    IList<OracleParameter> OracleP = new List<OracleParameter>();
                    for (int i = 0; i < allFiledList.Count; i++)
                    {
                        var t = allFiledList[i];
                        PropertyInfo outproInfo = inEnt.GetType().GetProperty(t);
                        OracleParameter deptNoParam = new OracleParameter(t, DbHelper.OracleHelper.GetDbType(outproInfo.PropertyType.FullName));
                        deptNoParam.Direction = ParameterDirection.Input;
                        object val = outproInfo.GetValue(inEnt, null);
                        if (val != null && val != DBNull.Value && val.ToString().Length > 2000)
                        {
                            deptNoParam.OracleDbType = OracleDbType.NClob;
                        }
                        deptNoParam.Value = val;
                        OracleP.Add(deptNoParam);
                    }
                    insertSql = string.Format("INSERT INTO {0} ({1}) VALUES({2})", tableName, string.Join(",", allFiledList), ":" + string.Join(",:", allFiledList));
                    DbHelper.OracleHelper.ExecuteNonQuery(connStr,CommandType.Text, insertSql, OracleP.ToArray());
                    break;
            }

            return true;
        }


        public static bool UpData<T>(string dbType, string connStr, Dictionary<string, object> upData, string whereStr, string prefix = "") where T : new()
        {
            T tmp = new T();

            string tableName = string.Format("{0}{1}", prefix, tmp.GetType().Name);
            var insertSql = "";
            switch (dbType.ToUpper())
            {
                //case "DB2":
                //    IList<DB2Parameter> DB2P = new List<DB2Parameter>();
                //    foreach(var t in upData)
                //    {
                //        PropertyInfo outproInfo = tmp.GetType().GetProperty(t.Key);
                //        DB2Parameter deptNoParam = new DB2Parameter(t.Key, DbHelper.DB2Helper.GetDbType(outproInfo.PropertyType.FullName));
                //        deptNoParam.Direction = ParameterDirection.Input;
                //        deptNoParam.Value = t.Value;
                //        DB2P.Add(deptNoParam);
                //    }
                //    var insertSql = string.Format("UPDATA {0} SET {1} {2}", tableName, string.Join(",", upData.Keys.Select(x => string.Format("{0}=@{0}", x)), whereStr));
                //    DbHelper.DB2Helper.ExecuteNonQuery(connStr,CommandType.Text, insertSql, DB2P.ToArray());
                //    break;
                case "ORACLE":
                    IList<OracleParameter> OracleP = new List<OracleParameter>();
                    foreach (var t in upData)
                    {
                        PropertyInfo outproInfo = tmp.GetType().GetProperty(t.Key);
                        OracleParameter deptNoParam = new OracleParameter(t.Key, DbHelper.OracleHelper.GetDbType(outproInfo.PropertyType.FullName));
                        deptNoParam.Direction = ParameterDirection.Input;
                        deptNoParam.Value = t.Value;
                        OracleP.Add(deptNoParam);
                    }
                    var idArr=string.Join(",", upData.Keys.Select(x=>string.Format("{0}=:{0}",x)).ToList());
                    insertSql = string.Format("UPDATE {0} SET {1} {2}", tableName, idArr, whereStr);
                    DbHelper.OracleHelper.ExecuteNonQuery(connStr,CommandType.Text, insertSql, OracleP.ToArray());
                    break;
            }

            return true;
        }


        /// <summary>
        /// 将SQL语句。转成实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static IList<T> SqlToList<T>(string sql, string dbType, string connStr) where T : new()
        {
            DbDataReader dr = null;
            //DB2Connection connDb2 = new DB2Connection();
            OracleConnection connOracle = new OracleConnection();
            switch (dbType)
            {
                //case "DB2":
                //    DB2Connection connDb2 = new DB2Connection();
                //    connDb2 = new DB2Connection(connStr);
                //    dr = DbHelper.DB2Helper.ExecuteReader(connDb2, CommandType.Text, sql);
                //    break;
                case "ORACLE":
                    connOracle = new OracleConnection(connStr);
                    dr = DbHelper.OracleHelper.ExecuteReader(connOracle, CommandType.Text, sql);
                    break;
            }
            IList<T> reList = new List<T>();
            if (dr == null) return reList;
            while (dr.Read())
            {
                T tmp = new T();
                PropertyInfo[] proInfoArr = tmp.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);//得到该类的所有公共属性
                for (int a = 0; a < proInfoArr.Length; a++)
                {
                    try
                    {
                        object obj = dr[proInfoArr[a].Name];
                        PropertyInfo outproInfo = tmp.GetType().GetProperty(proInfoArr[a].Name);
                        if (outproInfo != null && obj != null && obj != DBNull.Value)
                        {
                            try
                            {
                                outproInfo.SetValue(tmp, obj, null);
                            }
                            catch
                            {
                                try
                                {
                                    outproInfo.SetValue(tmp, Convert.ChangeType(dr[proInfoArr[a].Name], outproInfo.PropertyType, CultureInfo.CurrentCulture), null);
                                }
                                catch
                                {
                                    outproInfo.SetValue(tmp, Convert.ChangeType(obj, outproInfo.PropertyType, CultureInfo.CurrentCulture), null);
                                }
                            }
                        }
                    }catch{}

                }
                reList.Add(tmp);
            }

            dr.Close();
            dr.Dispose();
            switch (dbType)
            {
                //case "DB2":
                //    connDb2.Close();
                //    connDb2.Dispose();
                //    break;
                case "ORACLE":
                    connOracle.Close();
                    connOracle.Dispose();
                    break;
            }
            return reList;
        }

        /// <summary>
        /// 生成SQL语句，返回满足条件的第一条
        /// </summary>
        /// <typeparam name="T">要返回的类型</typeparam>
        /// <param name="dbType">数据库类型:DB2、ORACLE</param>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="whereStr">带where的条件语句</param>
        /// <param name="prefix">表的前缀</param>
        /// <returns>实体对象</returns>
        public static T ClassSingle<T>(string dbType, string connStr, string whereStr, string prefix = "") where T : new()
        {
            DbDataReader dr = null;
            //DB2Connection db2Conn = null;
            OracleConnection oralceConn = null;

            T reEnt = new T();
            IList<string> reFiledList = new List<string>();
            PropertyInfo[] proInfoArr = reEnt.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);//得到该类的所有公共属性
            foreach (var t in proInfoArr)
            {
                if (t.PropertyType.Name != "ICollection`1")
                {
                    reFiledList.Add(t.Name);
                }
            }

            string sql = string.Format("SELECT {0} FROM {3}{1} {2}", string.Join(",", reFiledList.Select(x => "\"" + x + "\"").ToArray()), reEnt.GetType().Name, whereStr, prefix);

            object conn = new object();

            switch (dbType.ToUpper())
            {
                    //case "DB2":
                    //    db2Conn = new DB2Connection(connStr);
                    //    dr = DbHelper.DB2Helper.ExecuteReader(db2Conn, CommandType.Text, sql);
                    //    break;
                case "ORACLE":
                    oralceConn = new OracleConnection(connStr);
                    dr = DbHelper.OracleHelper.ExecuteReader(oralceConn, CommandType.Text, sql);
                    break;
            }
            while (dr.Read())
            {
                foreach (var t in reFiledList)
                {
                    object obj = dr[t];
                    PropertyInfo outproInfo = reEnt.GetType().GetProperty(t);
                    if (outproInfo != null && obj != null && obj != DBNull.Value)
                    {
                        if (obj.GetType().Name == "Int64" && outproInfo.PropertyType.Name == "Int32")
                        {
                            obj = Convert.ToInt32(obj.ToString());
                        }
                        try
                        {
                            outproInfo.SetValue(reEnt, obj, null);
                        }
                        catch (Exception e)
                        {
                            var ttt = e.Message;
                        }
                    }
                }
                break;
            }
            dr.Close();
            dr.Dispose();

            switch (dbType.ToUpper())
            {
                //case "DB2":
                //    db2Conn.Close();
                //    db2Conn.Dispose();
                //    break;
                case "ORACLE":
                    oralceConn.Close();
                    oralceConn.Dispose();
                    break;
            }


            return reEnt;
        }
    }
}
