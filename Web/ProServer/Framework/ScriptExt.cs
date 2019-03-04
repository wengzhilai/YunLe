
//using IBM.Data.DB2;
using Oracle.ManagedDataAccess.Client;
using ProInterface;
using ProInterface.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ProServer
{

    public class ScriptExt
    {
        #region 定义变量
        public DateTime runData = DateTime.Now;

        public string _Error = "";

        public string _dbType = "DB2";
        public string _schema = "";
        public string _connStr = "";
        public object _conn;
        public string _dbNickName;
        public bool _isRun = false;
        /// <summary>
        /// 任务ID
        /// </summary>
        public Int64 _taskId = 0;
        public Int64 _scriptId = 0;
        /// <summary>
        /// 下载文件的临时文件夹
        /// </summary>
        public string _downDbPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\UpFiles\\DownDB\\";

        public static readonly string SqlSelectIsTableExists = "SELECT 1 A FROM ALL_TABLES WHERE TABLE_NAME = {1}p_table";
        /// <summary>
        /// 判断表是否存在:{0}schema,{1}表名
        /// </summary>
        public static readonly string SqlSelectIsTableWithOwnerExists = "SELECT 1 A FROM ALL_TABLES WHERE TABLE_NAME = '{1}' AND OWNER = '{0}'";

        public static readonly string SqlSelectIsTableExistsDB2 = "SELECT 1 A FROM SYSCAT.TABLES WHERE TABNAME = {1}p_table WITH UR";
        /// <summary>
        /// 判断表是否存在:{0}schema,{1}表名
        /// </summary>
        public static readonly string SqlSelectIsTableWithOwnerExistsDB2 = "SELECT 1 A FROM SYSCAT.TABLES WHERE TABNAME = '{1}' AND TABSCHEMA = '{0}' WITH UR";

        public static readonly string SqlSelectIsIndexExists = "SELECT 1 A FROM ALL_INDEXES WHERE INDEX_NAME = {1}p_index";

        public static readonly string SqlSelectIsIndexWithOwnerExists = "SELECT 1 FROM ALL_INDEXES WHERE INDEX_NAME = {1}p_index AND OWNER = {1}p_owner";

        public static readonly string SqlSelectIsIndexExistsDB2 = "SELECT 1 FROM SYSCAT.INDEXES WHERE INDNAME = {1}p_index";

        public static readonly string SqlSelectIsIndexWithOwnerExistsDB2 = "SELECT 1 FROM SYSCAT.INDEXES WHERE INDNAME = {1}p_index AND INDSCHEMA = {1}p_owner";

        #endregion


        #region 添加一个任务脚本
        /// <summary>
        /// 添加一个任务脚本
        /// </summary>
        /// <param name="scriptId"></param>
        public void AddScriptTask(int scriptId, int? groupId, ref long taskId)
        {
            var scriptEnt = FunSqlToClass.ClassSingle<SCRIPT>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                "where ID = " + scriptId,
                ConfigurationManager.AppSettings["dbPrefix"]
                );

            SCRIPT_TASK ent = new SCRIPT_TASK();
            ent.ID = FunSqlToClass.GetSeqID<SCRIPT_TASK>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                ConfigurationManager.AppSettings["dbPrefix"]
                );
            ent.SCRIPT_ID = scriptId;
            ent.GROUP_ID = groupId;
            ent.BODY_TEXT = scriptEnt.BODY_TEXT;
            ent.BODY_HASH = scriptEnt.BODY_HASH;
            ent.RUN_WHEN = scriptEnt.RUN_WHEN;
            ent.RUN_ARGS = scriptEnt.RUN_ARGS;
            ent.RUN_DATA = scriptEnt.RUN_DATA;
            ent.RUN_STATE = "等待";
            ent.DSL_TYPE = "自动添加";
            ent.START_TIME = DateTime.Now;
            ent.SERVICE_FLAG = scriptEnt.SERVICE_FLAG;
            FunSqlToClass.Save<SCRIPT_TASK>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                ent,
                ConfigurationManager.AppSettings["dbPrefix"]
                );

            taskId = ent.ID;
        }
        #endregion


        #region 运行脚本任务
        /// <summary>
        /// 运行脚本任务
        /// </summary>
        /// <param name="scriptId"></param>
        public void ScriptTaskRun(Int64 scriptTaskId)
        {

            var entTask = FunSqlToClass.ClassSingle<SCRIPT_TASK>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                "where ID = " + scriptTaskId,
                ConfigurationManager.AppSettings["dbPrefix"]
                );

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("RUN_STATE", "运行");
            dic.Add("START_TIME", DateTime.Now);
            FunSqlToClass.UpData<SCRIPT_TASK>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                dic,
                string.Format("where ID={0} ", _taskId),
                ConfigurationManager.AppSettings["dbPrefix"]);

            _taskId = entTask.ID;
            _scriptId = entTask.SCRIPT_ID;
            runData = new Service().AnalysisRunDate(entTask.RUN_DATA);

        }
        #endregion




        public bool IsRun()
        {
            return ScriptTaskIsRun();
        }

        public string GetError()
        {
            return _Error;
        }

        #region 结束任务
        /// <summary>
        /// 结束任务
        /// </summary>
        public void CompleteScriptTask()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("RUN_STATE", "停止");
            dic.Add("END_TIME", DateTime.Now);
            if (IsRun())
            {
                dic.Add("RETURN_CODE", "成功");
            }
            else
            {
                dic.Add("RETURN_CODE", "失败");
            }
            FunSqlToClass.UpData<SCRIPT_TASK>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                dic,
                string.Format("where ID={0} ", _taskId),
                ConfigurationManager.AppSettings["dbPrefix"]);


        }
        #endregion

        /// <summary>
        /// 检测任务是否已经在运行
        /// </summary>
        /// <returns></returns>
        public bool ScriptTaskIsRun()
        {
            var ent = FunSqlToClass.ClassSingle<SCRIPT_TASK>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                string.Format("where ID={0} ", _taskId),
                ConfigurationManager.AppSettings["dbPrefix"]
                );
            if (ent == null || ent.RUN_STATE.IndexOf("停止") > -1)
            {
                //log(string.Format("状态异常【{0}】", ent.RUN_STATE));
                _isRun = false;
                return false;
            }
            _isRun = true;
            return true;
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        public void ScriptTaskCancel()
        {
            log(string.Format("取消任务【{0}】", _taskId));
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("RUN_STATE", "停止");
            dic.Add("RETURN_CODE", "取消");
            FunSqlToClass.UpData<SCRIPT_TASK>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                dic,
                string.Format("where ID={0} ", _taskId),
                ConfigurationManager.AppSettings["dbPrefix"]);

        }


        /// <summary>
        /// 运行出错日志
        /// </summary>
        /// <param name="err"></param>
        public void ErrorScriptTask(Exception err)
        {
            var message = err.Message;
            if (message.Length > 40)
            {
                message = message.Substring(0, 40);
            }
            _Error = err.Message;

            log(err.Message, err.ToString(), 1);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("RUN_STATE", "停止");
            dic.Add("RETURN_CODE", "失败");
            dic.Add("END_TIME", DateTime.Now);
            dic.Add("DISABLE_DATE", DateTime.Now);
            dic.Add("DISABLE_REASON", message);
            FunSqlToClass.UpData<SCRIPT_TASK>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                dic,
                string.Format("where ID={0} ", _taskId),
                ConfigurationManager.AppSettings["dbPrefix"]);
            _isRun = false;
        }

        public string GetSchemaTableName(string tableName, string schema)
        {
            string reStr = "";
            string[] temp = tableName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length > 1)
            {
                schema = temp[temp.Length - 2];
                reStr = string.Format("{0}.{1}", temp[temp.Length - 2], temp[temp.Length - 1]);
            }
            else if (!string.IsNullOrEmpty(schema))
            {
                reStr = string.Format("{0}.{1}", schema, tableName);
            }
            return reStr;
        }

        public string GetTableName(string tableName, out string schema)
        {
            string reStr = "";
            string[] temp = tableName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            schema = _schema;
            if (temp.Length > 1)
            {
                schema = temp[temp.Length - 2];
                reStr = temp[temp.Length - 1];
            }
            else if (!string.IsNullOrEmpty(_schema))
            {
                reStr = tableName;
            }
            return reStr;
        }

        /// <summary>
        /// 释放连接资源
        /// </summary>
        public void Dispose()
        {
            if (_conn != null)
            {
                switch (_dbType)
                {
                    case "ORACLE":
                        var nowOracleConn = _conn as OracleConnection;
                        if (nowOracleConn.State != ConnectionState.Closed)
                        {
                            nowOracleConn.Dispose();
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 设置当前数据库
        /// </summary>
        public void setnowdb(string dbNickName = null)
        {

            if (string.IsNullOrEmpty(dbNickName)) dbNickName = "中心库";
            var ent = FunSqlToClass.ClassSingle<DB_SERVER>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                string.Format("where NICKNAME='{0}' ", dbNickName),
                ConfigurationManager.AppSettings["dbPrefix"]
                );
            if (ent != null)
            {
                _connStr = GetDbConnStr(ent);
                if (string.IsNullOrEmpty(ent.TYPE))
                {
                    ErrorScriptTask(new Exception("数据类型不能为空"));
                    return;
                }
                switch (ent.TYPE.ToUpper())
                {
                    case "ORACLE":
                        _conn = new OracleConnection(_connStr);
                        break;
                }
                _schema = ent.UID;
                _dbType = ent.TYPE.ToUpper();
                _dbNickName = dbNickName;
                log(string.Format("【{0}】数据库切换成功", dbNickName));
            }
            else
            {
                log(string.Format("【{0}】数据库不存在", dbNickName));
                return;
            }

        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        private string GetDbConnStr(DB_SERVER ent)
        {
            switch (ent.TYPE.ToUpper())
            {
                case "DB2":
                    return string.Format("Server={0}:{1};Database={2};UID={3};PWD={4};Connection Timeout =3600", ent.IP, ent.PORT, ent.DBNAME, ent.UID, ent.PASSWORD);
                case "ORACLE":
                    return string.Format("Data Source={0}:{1}/{2};User Id={3};Password={4};Connection Timeout =3600", ent.IP, ent.PORT, ent.DBNAME, ent.UID, ent.PASSWORD);
            }
            return "";
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sql_text"></param>
        /// <param name="logType"></param>
        public void log(string message, string sql_text = "", Int16 logType = 0)
        {
            try
            {
                if (!IsRun())
                {
                    return;
                }
                if (_taskId == 0)
                {
                    return;
                }
                ProInterface.Models.SCRIPT_TASK_LOG ent = new SCRIPT_TASK_LOG();
                ent.SCRIPT_TASK_ID = _taskId;
                ent.LOG_TIME = DateTime.Now;
                ent.LOG_TYPE = logType;
                ent.MESSAGE = message;
                ent.SQL_TEXT = sql_text;
                FunSqlToClass.Save<ProInterface.Models.SCRIPT_TASK_LOG>(ConfigurationManager.AppSettings["dbType"], ConfigurationManager.AppSettings["dbConnSt"], ent, ConfigurationManager.AppSettings["dbPrefix"]);
            }
            catch { }

        }



        /// <summary>
        /// 执行SQL命令
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int execute(string sql)
        {
            if (!IsRun())
            {
                return 0;
            }
            int reObj = 0;
            try
            {
                log(string.Format("在【{1}】执行语句【{0}】", sql, _dbNickName));
                switch (_dbType)
                {
                    case "ORACLE":
                        reObj = DbHelper.OracleHelper.ExecuteNonQuery(_conn as OracleConnection, CommandType.Text, sql);
                        break;
                }
                log(string.Format("执行成功", sql, _dbNickName));
            }
            catch (Exception e)
            {
                log(string.Format("在【{1}】执行语句失败【{0}】", sql, _dbNickName));
                log(string.Format("错误原因【{0}】", e.Message));
                ErrorScriptTask(e);
            }
            return reObj;
        }

        public int execute(string sql, DB_SERVER ent)
        {
            if (!IsRun())
            {
                return 0;
            }
            int reObj = 0;
            try
            {
                log(string.Format("在【{1}】执行语句【{0}】", sql, _dbNickName));
                string connStr = GetDbConnStr(ent);
                switch (ent.TYPE.ToUpper())
                {
                    case "ORACLE":
                        reObj = DbHelper.OracleHelper.ExecuteNonQuery(connStr, CommandType.Text, sql);
                        break;
                }
                log(string.Format("执行成功", sql, _dbNickName));
            }
            catch (Exception e)
            {
                ErrorScriptTask(e);
                log(string.Format("在【{1}】执行语句失败【{0}】", sql, _dbNickName));
                log(string.Format("错误原因【{0}】", e.Message));
            }
            return reObj;
        }

        public int exec(string sql)
        {
            return execute(sql);
        }

        /// <summary>
        /// 返回执行返回值
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object execute_scalar(string sql)
        {
            if (!IsRun())
            {
                return null;
            }
            object reObj = 0;
            try
            {
                switch (_dbType)
                {
                    case "ORACLE":
                        reObj = DbHelper.OracleHelper.ExecuteScalar(_conn as OracleConnection, CommandType.Text, sql);
                        break;
                }
                log(string.Format("执行操作语句成功【{0}】", sql));
            }
            catch (Exception e)
            {
                ErrorScriptTask(e);
                log(string.Format("执行操作语句失败【{0}】", sql));
                log(string.Format("错误原因【{0}】", e.Message));
            }
            return reObj;
        }

        private object execute_scalar(string sql, DB_SERVER ent)
        {
            if (!IsRun())
            {
                return 0;
            }
            object reObj = 0;
            string connStr = GetDbConnStr(ent);
            try
            {
                switch (ent.TYPE.ToUpper())
                {
                    case "DB2":
                        reObj = DbHelper.DB2Helper.ExecuteScalar(connStr, CommandType.Text, sql);
                        break;
                    case "ORACLE":
                        reObj = DbHelper.OracleHelper.ExecuteScalar(connStr, CommandType.Text, sql);
                        break;
                }
                log(string.Format("执行操作语句成功【{0}】在【{1}】", sql, ent.NICKNAME));
            }
            catch (Exception e)
            {
                ErrorScriptTask(e);
                log(string.Format("执行操作语句失败【{0}】在【{1}】", sql, ent.NICKNAME));
                log(string.Format("错误原因【{0}】", e.Message));
            }
            return reObj;
        }

        /// <summary>
        /// 获取当前日期格式"yyyyMMdd"
        /// </summary>
        /// <param name="value">增加的天数，可为负数</param>
        /// <returns></returns>
        public string day(int value = 0)
        {
            return runData.AddDays(value).ToString("yyyyMMdd");
        }
        /// <summary>
        /// 获取当前月份格式"yyyyMM"
        /// </summary>
        /// <param name="value">增加的月份，可为负数</param>
        /// <returns></returns>
        public string month(int value = 0)
        {
            return runData.AddMonths(value).ToString("yyyyMM");
        }
        /// <summary>
        /// 获取当前年份格式"yyyy"
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string years(int value = 0)
        {
            return runData.AddYears(value).ToString("yyyy");
        }

        /// <summary>
        /// 月份最后一天
        /// </summary>
        /// <param name="value">当前月增加月份</param>
        /// <returns></returns>
        public string last_day(int value = 0)
        {
            DateTime temp = runData;
            temp = new DateTime(temp.Year, temp.Month, 1);
            return temp.AddMonths(value + 1).AddDays(-1).ToString("yyyyMMdd");
        }


        public string MakeCreateTableSql(List<TableFiled> filedList, string tableName, string dbType)
        {
            string reStr = @"
create Table {0}
(
{1}
)
            ";
            IList<string> filed = new List<string>();
            string tmpFormat = "{0} {1}";
            foreach (var t in filedList)
            {
                switch (t.CSharpType)
                {
                    case "Int16":
                        switch (dbType)
                        {
                            case "DB2":
                                filed.Add(string.Format(tmpFormat, t.Code, "SMALLINT"));
                                break;
                            case "ORACLE":
                                filed.Add(string.Format(tmpFormat, t.Code, "NUMBER(4)"));
                                break;
                            default:
                                filed.Add(string.Format(tmpFormat, t.Code, "NUMBER(10)"));
                                break;
                        }
                        break;
                    case "Int32":
                        switch (dbType)
                        {
                            case "DB2":
                                filed.Add(string.Format(tmpFormat, t.Code, "INT"));
                                break;
                            case "ORACLE":
                                filed.Add(string.Format(tmpFormat, t.Code, "NUMBER(10)"));
                                break;
                            default:
                                filed.Add(string.Format(tmpFormat, t.Code, "INT"));
                                break;
                        }
                        break;
                    case "Int64":
                        switch (dbType)
                        {
                            case "DB2":
                                filed.Add(string.Format(tmpFormat, t.Code, "BIGINT"));
                                break;
                            case "ORACLE":
                                filed.Add(string.Format(tmpFormat, t.Code, "NUMBER(20)"));
                                break;
                            default:
                                filed.Add(string.Format(tmpFormat, t.Code, "INT"));
                                break;
                        }
                        break;
                    case "DateTime":
                        switch (dbType)
                        {
                            case "DB2":
                                filed.Add(string.Format(tmpFormat, t.Code, "TIMESTAMP"));
                                break;
                            case "ORACLE":
                                filed.Add(string.Format(tmpFormat, t.Code, "TIMESTAMP"));
                                break;
                            default:
                                filed.Add(string.Format(tmpFormat, t.Code, "datetime"));
                                break;
                        }
                        break;
                    default:
                        if (t.Length == 0) t.Length = 100;
                        if (t.Length < 3000)
                        {
                            switch (dbType)
                            {
                                case "DB2":
                                    filed.Add(string.Format(tmpFormat, t.Code, string.Format("VARCHAR({0})", t.Length * 2)));
                                    break;
                                case "ORACLE":
                                    filed.Add(string.Format(tmpFormat, t.Code, string.Format("VARCHAR2({0})", t.Length * 2)));
                                    break;
                                default:
                                    filed.Add(string.Format(tmpFormat, t.Code, string.Format("VARCHAR({0})", t.Length * 2)));
                                    break;
                            }
                        }
                        else
                        {
                            switch (dbType)
                            {
                                case "DB2":
                                    filed.Add(string.Format(tmpFormat, t.Code, "CLOB"));
                                    break;
                                case "ORACLE":
                                    filed.Add(string.Format(tmpFormat, t.Code, "NCLOB"));
                                    break;
                                default:
                                    filed.Add(string.Format(tmpFormat, t.Code, "TEXT"));
                                    break;
                            }
                        }
                        break;
                }
            }
            reStr = string.Format(reStr, tableName, string.Join(",\r\n", filed));
            return reStr;
        }

        /// <summary>
        /// 导出表，生成的文件路径：~/UpFiles/DownDB/{tableName}.zip
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="tableName">导入的文件名</param>
        /// <returns>文件路径</returns>
        public string down_db_to_file(string sql, string tableName)
        {
            if (!IsRun())
            {
                return null;
            }
            var path = down_db(sql, tableName);
            var zipFile = _downDbPath + tableName + ".zip";
            try
            {
                ProServer.Helper.ZipHelper.ZipFileMain(path, zipFile);
                log(string.Format("生成ZIP文件成功路径为【{0}】", zipFile));
            }
            catch (Exception e)
            {
                ErrorScriptTask(e);
                log(string.Format("压缩文件失败，路径为【{0}】", path));
                log(string.Format("错误原因【{0}】", e.Message));
            }

            return zipFile;
        }

        private string down_db(string sql, string tableName, int pageSize = 1000000)
        {
            if (!IsRun())
            {
                return null;
            }
            DbDataReader dr = null;
            object conn = new object();
            try
            {
                switch (_dbType)
                {
                    case "ORACLE":
                        conn = new OracleConnection(_connStr);
                        dr = DbHelper.OracleHelper.ExecuteReader((OracleConnection)conn, CommandType.Text, sql);
                        break;
                }

                log(string.Format("执行查询语句成功【{0}】", sql));
            }
            catch (Exception e)
            {
                ErrorScriptTask(e);
                log(string.Format("执行查询语句失败【{0}】", sql));
                log(string.Format("错误原因【{0}】", e.Message));
                return "";
            }

            var nowPath = _downDbPath + tableName + "\\";
            if (!Directory.Exists(nowPath))
            {
                Directory.CreateDirectory(nowPath);
            }
            foreach (var t in Directory.GetFiles(nowPath))
            {
                File.Delete(t);
            }

            var tableFiled = new List<TableFiled>();
            for (int i = 0; i < dr.FieldCount; i++)
            {
                tableFiled.Add(new TableFiled { Code = dr.GetName(i), DataType = dr.GetDataTypeName(i), CSharpType = dr.GetFieldType(i).Name });
            }

            Fun.WriteAllText(nowPath + tableName + "_Create.txt", MakeCreateTableSql(tableFiled, tableName, _dbType));
            Fun.WriteAllText(nowPath + tableName + "_TableFiled.txt", JSON.DecodeToStr(tableFiled));

            IList<string> allData = new List<string>();
            int rowNum = 0;
            int pageNum = 1;
            log(string.Format("开始下载数据"));
            while (dr.Read())
            {
                rowNum++;
                IList<string> rowData = new List<string>();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (dr[i] == null || dr[i] == DBNull.Value)
                    {
                        rowData.Add(null);
                    }
                    else
                    {
                        rowData.Add(dr[i].ToString().Replace(",", "，，"));
                    }
                }
                allData.Add(string.Join(",", rowData));
                if (rowNum == pageSize)
                {
                    File.WriteAllLines(nowPath + tableName + "_" + pageNum + ".txt", allData);
                    log(string.Format("下载完成第【{0}】页数据", pageNum));
                    allData = new List<string>();
                    rowNum = 0;
                    pageNum++;
                }
            }
            log(string.Format("结束下载，共【{0}】页，【{1}】条数据", pageNum, (pageNum - 1) * pageSize + rowNum));
            if (rowNum != 0)
            {
                File.WriteAllLines(nowPath + tableName + "_" + pageNum + ".txt", allData);
                allData = new List<string>();
            }
            dr.Close();
            dr.Dispose();
            switch (_dbType)
            {
                case "ORACLE":
                    var oralceConn = (OracleConnection)conn;
                    oralceConn.Close();
                    oralceConn.Dispose();
                    break;
            }
            return nowPath;
        }

        /// <summary>
        /// 下载表到数据库
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="tableName">生成的表名</param>
        /// <param name="dbNickName">导入的数据库</param>
        /// <param name="isCreat">0(默认值)表示不创建表;1:表示自动创建表(在导数据之前，要删除已经存在表);</param>
        /// <param name="pageSize">设置每页导出的大小。值越大速度越快(对服务器压力大，内容要足够大) 默认值：1000000</param>
        public void down_db_to_db(string sql, string tableName, string dbNickName, int isCreatTable = 1, int pageSize = 1000000)
        {
            if (!IsRun())
            {
                return;
            }

            DbDataReader dr = null;
            object conn = new object();

            #region 读取数据
            try
            {
                switch (_dbType)
                {
                    case "ORACLE":
                        conn = new OracleConnection(_connStr);
                        dr = DbHelper.OracleHelper.ExecuteReader((OracleConnection)conn, CommandType.Text, sql);
                        break;
                }

                log(string.Format("数据库【{1}】执行查询语句成功【{0}】", sql, _dbNickName));
            }
            catch (Exception e)
            {
                ErrorScriptTask(e);
                log(string.Format("数据库【{1}】执行查询语句失败【{0}】", sql, _dbNickName));
                log(string.Format("错误原因【{0}】", e.Message));
                return;
            }
            #endregion

            var allFiled = new List<TableFiled>();
            for (int i = 0; i < dr.FieldCount; i++)
            {
                allFiled.Add(new TableFiled { Code = dr.GetName(i), DataType = dr.GetDataTypeName(i), CSharpType = dr.GetFieldType(i).Name });
            }

            var ent = FunSqlToClass.ClassSingle<DB_SERVER>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                string.Format(" where NICKNAME='{0}' ", dbNickName),
                ConfigurationManager.AppSettings["dbPrefix"]
                );
            if (ent == null)
            {
                log(string.Format("【{0}】数据库不存在", dbNickName));
                return;
            }


            IList<IList<object>> allRows = new List<IList<object>>();
            int rowNum = 0;
            int pageNum = 1;
            log(string.Format("开始下载数据"));
            log(string.Format("开始下载第【{0}】页数据", pageNum));
            var isRight = true;
            while (dr.Read())
            {
                if (!_isRun)
                {
                    break;
                }

                rowNum++;
                //行数据
                IList<object> rowData = new List<object>();
                #region 读取行数据
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (dr[i] == null || dr[i] == DBNull.Value)
                    {
                        rowData.Add(null);
                    }
                    else
                    {
                        rowData.Add(dr[i]);
                    }
                }
                allRows.Add(rowData);
                #endregion

                //如果行数据等于页数，则开始导入
                if (rowNum == pageSize)
                {
                    int parNum = allFiled.Count;
                    //转成可插入的格式
                    IList<IList<object>> allData = new List<IList<object>>();
                    #region 行转列
                    foreach (var x in allFiled)
                    {
                        allData.Add(new object[allRows.Count]);
                    }
                    object[] allPar = new object[parNum];
                    for (int a = 0; a < allRows.Count(); a++)
                    {
                        for (int i = 0; i < parNum; i++)
                        {
                            allData[i][a] = allRows[a][i];
                        }
                    }
                    #endregion

                    #region 如果是第一页则创建表
                    if (pageNum == 1)
                    {
                        for (var i = 0; i < allFiled.Count; i++)
                        {
                            if (allFiled[i].CSharpType == "String")
                            {

                                string maxSqlStr = string.Format("SELECT  MAX(LENGTH({0})) FROM ({1})", allFiled[i].Code, sql);
                                object maxLength = 0;
                                switch (_dbType)
                                {
                                    case "DB2":
                                        maxLength = DbHelper.DB2Helper.ExecuteScalar(_connStr, CommandType.Text, maxSqlStr);
                                        break;
                                    case "ORACLE":
                                        maxLength = DbHelper.OracleHelper.ExecuteScalar(_connStr, CommandType.Text, maxSqlStr);
                                        break;
                                }
                                if (maxLength == null || maxLength == DBNull.Value) maxLength = 50;
                                allFiled[i].Length = Convert.ToInt32(maxLength);
                                //var allUseCol = allData[i].Where(x => x != null && x != DBNull.Value);
                                //if (allUseCol.Count() > 0)
                                //{
                                //    allFiled[i].Length = allUseCol.Max(x => x.ToString().Length);
                                //}
                                //else
                                //{
                                //    allFiled[i].Length = 500;
                                //}
                            }
                        }
                        if (isCreatTable == 1)
                        {
                            var createScript = MakeCreateTableSql(allFiled, tableName, ent.TYPE.ToUpper());
                            drop_table(tableName, ent);
                            log(string.Format("在数据【{1}】上创建表【{0}】", tableName, dbNickName));
                            execute(createScript, ent);
                        }
                    }
                    #endregion

                    log(string.Format("完成下载第【{0}】页数据", pageNum));
                    if (!IsRun())
                    {
                        break;
                    }
                    isRight = InsertInto(tableName, dbNickName, allFiled, allData, allRows.Count, pageNum);
                    allData.Clear();
                    allRows.Clear();
                    rowNum = 0;


                    if (!isRight || !ScriptTaskIsRun())
                    {
                        break;
                    }
                    pageNum++;
                    log(string.Format("开始下载第【{0}】页数据", pageNum));
                }
            }

            if (!IsRun())
            {
                return;
            }
            if (rowNum != 0)
            {

                int parNum = allFiled.Count;
                IList<IList<object>> allData = new List<IList<object>>();
                foreach (var x in allFiled)
                {
                    allData.Add(new object[allRows.Count]);
                }
                object[] allPar = new object[parNum];
                for (int a = 0; a < allRows.Count(); a++)
                {
                    for (int i = 0; i < parNum; i++)
                    {
                        allData[i][a] = allRows[a][i];
                    }
                }

                if (pageNum == 1)
                {
                    for (var i = 0; i < allFiled.Count; i++)
                    {
                        if (allFiled[i].CSharpType == "String")
                        {
                            var allUseCol = allData[i].Where(x => x != null && x != DBNull.Value);
                            if (allUseCol.Count() > 0)
                            {
                                allFiled[i].Length = allUseCol.Max(x => x.ToString().Length);
                            }
                            else
                            {
                                allFiled[i].Length = 500;
                            }
                        }
                    }
                    if (isCreatTable == 1)
                    {
                        var createScript = MakeCreateTableSql(allFiled, tableName, ent.TYPE.ToUpper());
                        drop_table(tableName, ent);
                        log(string.Format("在数据【{1}】上创建表【{0}】", tableName, dbNickName));
                        execute(createScript, ent);
                    }
                }
                isRight = InsertInto(tableName, dbNickName, allFiled, allData, allRows.Count, pageNum);
                allData.Clear();
            }
            if (isRight)
            {
                log(string.Format("结束下载，共【{0}】页，【{1}】条数据", pageNum, (pageNum - 1) * pageSize + rowNum));
            }
            else
            {
                log(string.Format("下载失败，共【{0}】页，【{1}】条数据", pageNum, (pageNum - 1) * pageSize + rowNum));
            }
            dr.Close();
            dr.Dispose();
            switch (_dbType)
            {
                case "ORACLE":
                    var oralceConn = (OracleConnection)conn;
                    oralceConn.Close();
                    oralceConn.Dispose();
                    break;
            }
        }

        public bool InsertInto(string tableName, string dbNickName, IList<TableFiled> allFiled, IList<IList<object>> allData, int rowsNum, int nowPage)
        {

            log(string.Format("开始导第【{0}】页数据", nowPage));
            var ent = FunSqlToClass.ClassSingle<DB_SERVER>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                string.Format("where NICKNAME='{0}' ", dbNickName),
                ConfigurationManager.AppSettings["dbPrefix"]
                );
            string connStr = GetDbConnStr(ent);
            try
            {
                object oldNum = 0, newNum = 0;
                switch (ent.TYPE.ToUpper())
                {
                    case "ORACLE":
                        IList<OracleParameter> OracleP = new List<OracleParameter>();
                        for (int i = 0; i < allFiled.Count; i++)
                        {
                            var t = allFiled[i];
                            OracleParameter deptNoParam = new OracleParameter(t.Code, DbHelper.OracleHelper.GetDbType(t.CSharpType));
                            deptNoParam.Direction = ParameterDirection.Input;
                            deptNoParam.Value = allData[i];
                            OracleP.Add(deptNoParam);
                        }
                        var insertSql = string.Format("insert into {0} ({1}) values({2})", tableName, string.Join(",", allFiled.Select(x => x.Code)), ":" + string.Join(",:", allFiled.Select(x => x.Code)));
                        oldNum = DbHelper.OracleHelper.ExecuteScalar(connStr, CommandType.Text, string.Format("select count(1) T from {0}", tableName));
                        DbHelper.OracleHelper.Import(rowsNum, connStr, insertSql, OracleP);
                        newNum = DbHelper.OracleHelper.ExecuteScalar(connStr, CommandType.Text, string.Format("select count(1) T from {0}", tableName));

                        break;
                }

                if (Convert.ToDecimal(newNum) != Convert.ToDecimal(oldNum) + rowsNum)
                {
                    throw new Exception(string.Format("导入数据失败，已导入{0}条;导入第{1}页{2}条数据失败", oldNum, nowPage, rowsNum));
                }
                log(string.Format("完成导入第【{0}】页,数据【{1}】行", nowPage, rowsNum));
                allData.Clear();
                return true;
            }
            catch (Exception e)
            {
                ErrorScriptTask(e);
                log(string.Format("导入第【{0}】页数据失败", nowPage));
                log(string.Format("失败原因：{0}", e.Message));
                return false;
            }
        }


        /// <summary>
        /// 快速删除表
        /// </summary>
        /// <param name="tables">可以是多表，以逗号分开</param>
        public void drop_table(string tables)
        {
            truncate_table(tables);
            foreach (string table in SplitString(tables))
            {
                string schema = _schema;
                string scTable = GetSchemaTableName(table, schema);
                if (is_table_exists(scTable))
                    execute("DROP TABLE " + scTable);
            }
        }

        public void drop_table(string tables, DB_SERVER ent)
        {
            log(string.Format("开始删除表【{0}】在【{1}】", tables, ent.NICKNAME));
            truncate_table(tables, ent);
            foreach (string table in SplitString(tables))
            {
                string schema = ent.UID;
                string scTable = table;
                if (is_table_exists(scTable, ent))
                    execute("DROP TABLE " + scTable, ent);
            }
            log(string.Format("结束删除表【{0}】在【{1}】", tables, ent.NICKNAME));
        }

        /// <summary>
        /// 快速清空资料表
        /// </summary>
        /// <param name="tables">可以是多表，以逗号分开</param>
        public void truncate_table(string tables)
        {

            foreach (string table in SplitString(tables))
            {
                string schema = _schema;
                string scTable = GetSchemaTableName(table, schema);
                if (is_table_exists(scTable))
                {
                    try
                    {
                        switch (_dbType)
                        {
                            case "DB2":
                                execute("ALTER TABLE " + scTable + " ACTIVATE NOT LOGGED INITIALLY WITH EMPTY TABLE");
                                break;
                            case "Sql":
                                break;
                            default:
                                execute("TRUNCATE TABLE " + scTable);
                                break;
                        }
                        log(string.Format("清除表资料{0}成功", scTable));

                    }
                    catch (Exception e)
                    {
                        _isRun = false;
                        log(string.Format("清除表资料{0}失败", scTable));
                        log(string.Format("失败原因：{0}", e.Message));
                    }
                }
            }
        }

        public void truncate_table(string tables, DB_SERVER ent)
        {
            foreach (string table in SplitString(tables))
            {
                string schema = ent.UID;
                string scTable = GetSchemaTableName(table, schema);
                if (is_table_exists(tables, ent))
                {
                    try
                    {
                        switch (ent.TYPE.ToUpper())
                        {
                            case "DB2":
                                execute("ALTER TABLE " + scTable + " ACTIVATE NOT LOGGED INITIALLY WITH EMPTY TABLE", ent);
                                break;
                            case "Sql":
                                break;
                            default:
                                execute("TRUNCATE TABLE " + scTable, ent);
                                break;
                        }
                        log(string.Format("清除表资料{0}成功", scTable));
                    }
                    catch (Exception e)
                    {
                        _isRun = false;
                        log(string.Format("清除表资料{0}失败", scTable));
                        log(string.Format("失败原因：{0}", e.Message));
                    }
                }
            }
        }

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="tables">可以是多表，以逗号分开</param>
        /// <returns></returns>
        public bool is_table_exists(string tables)
        {
            // Oracle系统表存放的是大写的表名，查找时须转换为大写
            foreach (string table in SplitString(tables.ToUpper()))
            {
                string schema = _schema;
                string scTable = GetSchemaTableName(table, schema);
                if (execute_scalar(GetTableExistsCommand(scTable)) == null)
                    return false;
            }
            return true;
        }
        public bool is_table_exists(string tables, DB_SERVER ent)
        {
            // Oracle系统表存放的是大写的表名，查找时须转换为大写
            foreach (string table in SplitString(tables.ToUpper()))
            {
                string schema = ent.UID;
                string scTable = table;
                if (execute_scalar(GetTableExistsCommand(scTable, ent), ent) == null)
                    return false;
            }
            return true;
        }


        public int GetSeqID(string tableName)
        {
            string sql = "SELECT   " + tableName + "_SEQ.NEXTVAL   FROM   DUAL";
            switch (_dbType)
            {
                case "DB2":
                    sql = "SELECT   " + tableName + "_SEQ.NEXTVAL   FROM   SYSIBM.DUAL";
                    break;
                case "Oracle":
                    sql = "SELECT   " + tableName + "_SEQ.NEXTVAL   FROM   DUAL";
                    break;
                case "Sql":
                    return 0;
            }
            object obj = execute_scalar(sql);
            return Convert.ToInt32(obj);
        }


        private string GetTableExistsCommand(string table, DB_SERVER ent)
        {
            string result;
            string schema = ent.UID;
            string tableName = table;
            switch (ent.TYPE.ToUpper())
            {
                case "DB2":
                    result = string.Format(SqlSelectIsTableWithOwnerExistsDB2, schema, tableName);
                    break;
                default:
                    result = string.Format(SqlSelectIsTableWithOwnerExists, schema, tableName);
                    break;
            }
            return result;
        }

        private string GetTableExistsCommand(string table)
        {
            string result;
            string schema = "";
            string tableName = GetTableName(table, out schema);

            switch (_dbType)
            {
                case "DB2":
                    result = string.Format(SqlSelectIsTableWithOwnerExistsDB2, schema, tableName);
                    break;
                default:
                    result = string.Format(SqlSelectIsTableWithOwnerExists, schema, tableName);
                    break;
            }
            return result;
        }

        protected string[] SplitString(string input)
        {
            List<string> result = new List<string>();

            if (input != null)
            {
                foreach (string s in input.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    result.Add(s.Trim());
                }
            }
            return result.ToArray();
        }


        /// <summary>
        /// 添加等待发送的内容
        /// </summary>
        /// <param name="phoneNo"></param>
        /// <param name="content"></param>
        /// <returns>主键</returns>
        public string SmsAdd(string phoneNo, string content)
        {

            ProInterface.Models.SMS_SEND ent = new SMS_SEND();
            ent.KEY = Guid.NewGuid().ToString().Replace("-", "");
            ent.PHONE_NO = phoneNo;
            ent.ADD_TIME = DateTime.Now;
            ent.CONTENT = content;
            ent.STAUTS = "等待";
            FunSqlToClass.Save<ProInterface.Models.SMS_SEND>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                ent,
                ConfigurationManager.AppSettings["dbPrefix"]);
            return ent.KEY;
        }
    }
}
