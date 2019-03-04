﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using System.Collections;
using System.Data;

namespace DbHelper
{
    public partial class OracleHelper
    {
        public static bool Import(int recc, string connectStr, string sql, IList<OracleParameter> allOraclePara)
        {
            //设置一个数据库的连接串
            using (OracleConnection conn = new OracleConnection(connectStr))
            {
                conn.Open();
                OracleTransaction transaction = conn.BeginTransaction();
                try
                {
                    OracleCommand command = new OracleCommand();
                    command.Connection = conn;
                    command.CommandType = CommandType.Text;
                    command.Transaction = transaction;
                    //到此为止，还都是我们熟悉的代码，下面就要开始喽
                    //这个参数需要指定每次批插入的记录数
                    command.ArrayBindCount = recc;
                    //在这个命令行中,用到了参数,参数我们很熟悉,但是这个参数在传值的时候
                    //用到的是数组,而不是单个的值,这就是它独特的地方
                    command.CommandText = sql;
                    //下面定义几个数组,分别表示三个字段,数组的长度由参数直接给出
                    foreach (var t in allOraclePara)
                    {
                        command.Parameters.Add(t);
                    }
                    //这个调用将把参数数组传进SQL,同时写入数据库
                    command.ExecuteNonQuery();
                    transaction.Commit();//提交事务
                }
                catch (Exception e)
                {
                    transaction.Rollback();//事务回滚
                    throw e;
                }
            }
            return true;
        }

        public static string ExportTxt(string connectionString, string commandText)
        {
            using (OracleConnection cn = new OracleConnection(connectionString))
            {
                try
                {
                    cn.Open();
                    var dr = ExecuteReader(cn, CommandType.Text, commandText);
                    StringBuilder sb = new StringBuilder();
                    IList<string> allColumns = new List<string>();
                    for (var i = 0; i < dr.FieldCount; i++) allColumns.Add(dr.GetName(i)); ;
                    sb.AppendLine(string.Join(",", allColumns));
                    while (dr.Read())
                    {
                        IList<string> row = new List<string>();
                        foreach (var t in allColumns)
                        {
                            if (dr[t] == null || dr[t] == DBNull.Value)
                            {
                                row.Add("");
                            }
                            else
                            {
                                row.Add(dr[t].ToString());
                            }
                        }
                        sb.AppendLine(string.Join(",", row));
                    }
                    dr.Dispose();
                    cn.Close();
                    cn.Dispose();
                    return sb.ToString();
                }
                catch (Exception err)
                {
                    cn.Close();
                    cn.Dispose();
                    throw err;
                }
            }
        }

        /// <summary>
        /// 类型全名
        /// </summary>
        /// <param name="dbTypeName"></param>
        /// <returns></returns>
        public static OracleDbType GetDbType(string dbTypeFullName)
        {
            if (dbTypeFullName.IndexOf("System.Nullable`1[[") == 0)
            {
                dbTypeFullName = dbTypeFullName.Replace("System.Nullable`1[[", "");
                dbTypeFullName = dbTypeFullName.Substring(0, dbTypeFullName.IndexOf(","));
            }
            string dbTypeName = dbTypeFullName.Substring(dbTypeFullName.IndexOf(".")+1);
            switch (dbTypeName)
            {
                case "Int16":
                    return OracleDbType.Int16;
                case "Int32":
                    return OracleDbType.Int32;
                case "Int64":
                    return OracleDbType.Int64;
                case "DateTime":
                    return OracleDbType.TimeStamp;
                default:
                    return OracleDbType.Varchar2;
            }
        }
    }
}
