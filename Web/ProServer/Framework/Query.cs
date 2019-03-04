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
using System.Data;
using System.Data.SqlClient;
using ProServer.Helper;

namespace ProServer
{
    public partial class Service : IQuery
    {

        public bool QuerySave(string loginKey, ref ErrorInfo err, QUERY inEnt, IList<string> allPar)
        {
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return false;
            }

            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;


            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_QUERY.SingleOrDefault(a => a.ID == inEnt.ID);
                    bool isAdd = false;
                    if (ent == null)
                    {
                        isAdd = true;
                        ent = Fun.ClassToCopy<ProInterface.Models.QUERY, YL_QUERY>(inEnt);

                    }
                    else
                    {
                        //if (!inEnt.QUERY_CONF.Equals(ent.QUERY_CONF))
                        //{
                        //    inEnt.QUERY_CONF = string.Format("--{0}于{1}修改\r\n{2}",gu.UserName,DateTime.Now.ToString(),inEnt.QUERY_CONF);
                        //}

                        ent = Fun.ClassToCopy<ProInterface.Models.QUERY, YL_QUERY>(inEnt, ent, allPar);
                    }

                    if (isAdd)
                    {
                        db.YL_QUERY.Add(ent);
                    }
                    db.SaveChanges();
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Edit);
                    return true;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return false;
                }
            }
        }

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="id">条件lambda表达表</param>
        /// <returns>查询一条</returns>
        public ProInterface.Models.TQuery QuerySingleId(string loginKey, ref ProInterface.ErrorInfo err, int id)
        {

            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_QUERY.SingleOrDefault(x => x.ID == id);
                var reEnt = new ProInterface.Models.TQuery();
                if (ent != null)
                {
                    reEnt = Fun.ClassToCopy<YL_QUERY, ProInterface.Models.TQuery>(ent);
                }
                return reEnt;
            }
        }

        #region 默认方法
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="whereLambda">条件lambda表达表</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderBy">排序方式</param>
        /// <returns>返回满足条件的泛型</returns>
        public IList<ProInterface.Models.TQuery> QueryWhere(string loginKey, ref ProInterface.ErrorInfo err, int pageIndex, int pageSize, string whereLambda, string orderField, string orderBy)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 1;
            int skipCount = (pageIndex - 1) * pageSize;
            if (orderField == null || orderField == "")
            {
                err.IsError = true;
                err.Message = "排序表态式不能为空";
                return null;
            }
            using (DBEntities db = new DBEntities())
            {
                var allList = db.YL_QUERY.AsQueryable();
                if (whereLambda != null && whereLambda != "")
                {
                    try
                    {
                        Expression<Func<YL_QUERY, bool>> whereFunc = StringToLambda.LambdaParser.Parse<Func<YL_QUERY, bool>>(whereLambda);
                        allList = db.YL_QUERY.Where(whereFunc);
                    }
                    catch
                    {
                        err.IsError = true;
                        err.Message = "条件表态式有误";
                        return null;
                    }
                }

                if (orderBy == "asc")
                {
                    allList = StringFieldNameSortingSupport.OrderBy(allList, orderField);
                }
                else
                {
                    allList = StringFieldNameSortingSupport.OrderByDescending(allList, orderField);
                }

                var content = allList.Skip(skipCount).Take(pageSize).ToList();
                return Fun.ClassListToCopy<YL_QUERY, ProInterface.Models.TQuery>(content);
            }
        }

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="whereLambda">条件lambda表达表</param>
        /// <returns>查询一条</returns>
        public ProInterface.Models.TQuery QuerySingle(string loginKey, ref ProInterface.ErrorInfo err, string whereLambda)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                IList<YL_QUERY> content = new List<YL_QUERY>();
                Expression<Func<YL_QUERY, bool>> whereFunc;
                try
                {
                    whereFunc = StringToLambda.LambdaParser.Parse<Func<YL_QUERY, bool>>(whereLambda);
                }
                catch
                {
                    err.IsError = true;
                    err.Message = "条件表态式有误";
                    return null;
                }
                var queryEnt = db.YL_QUERY.Where(whereFunc).ToList();
                if (queryEnt.Count > 0)
                {
                    var reEnt = Fun.ClassToCopy<YL_QUERY, ProInterface.Models.TQuery>(queryEnt[0]);
                    GlobalUser gu = Global.GetUser(loginKey);
                    if (gu == null)
                    {
                        err.IsError = true;
                        err.Message = "登录超时";
                        return null;
                    }
                    var allAuth = queryEnt[0].YL_ROLE_QUERY_AUTHORITY.Where(x => gu.RoleID.Contains(x.ROLE_ID)).ToList();
                    foreach (var t in allAuth)
                    {
                        if (reEnt.NoAuthority == null)
                        {
                            if (t.NO_AUTHORITY == null)
                            {
                                reEnt.NoAuthority = "";
                            }
                            else
                            {
                                reEnt.NoAuthority = t.NO_AUTHORITY;
                            }

                        }
                        else
                        {
                            if (reEnt.NoAuthority == null) reEnt.NoAuthority = "";
                            if (t.NO_AUTHORITY == null) t.NO_AUTHORITY = "";
                            var allNowAuth = reEnt.NoAuthority.Split(',').ToList();
                            var thisAuth = t.NO_AUTHORITY.Split(',');
                            foreach (var t0 in reEnt.NoAuthority.Split(',').ToList())
                            {
                                if (!thisAuth.Contains(t0))
                                {
                                    allNowAuth.Remove(t0);
                                }
                            }
                            reEnt.NoAuthority = string.Join(",", allNowAuth);
                        }
                    }
                    return reEnt;
                }
                return null;
            }
        }
        /// <summary>
        /// 查询权限检查
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="code"></param>
        /// <param name="methodBase"></param>
        /// <returns></returns>
        public bool QueryCheckFunctioAuthority(string loginKey, ref ErrorInfo err, string code, MethodBase methodBase = null)
        {

            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时，请重新登录";
                return false;
            }
            using (DBEntities db = new DBEntities())
            {
                #region 角色模板权限控制
                var user = db.YL_USER.SingleOrDefault(x => x.ID == gu.UserId);
                var roles = db.YL_ROLE.Where(x => gu.RoleID.Contains<int>(x.ID)).ToList();
                string str = string.Format("~/Query/Query?code={0}", code);
                var moduleCount = user.YL_MODULE.Where(x => x.LOCATION == str).Count();
                if (moduleCount == 0)
                {
                    foreach (var i in roles)
                    {
                        moduleCount = i.YL_MODULE.Where(x => x.LOCATION == str).Count();
                        if (moduleCount != 0)
                        {
                            break;
                        }
                    }
                    if (moduleCount == 0) {
                        err.IsError = true;
                        err.Message = "您没有此模块的权限";
                        return false;
                    }
                }
                #endregion

                #region 按键权限控制
                var queryEnt = db.YL_QUERY.Where(x => x.CODE == code).ToList();
                IList<string> allNowAuth = null;
                if (queryEnt.Count > 0)
                {
                    var allAuth = queryEnt[0].YL_ROLE_QUERY_AUTHORITY.Where(x => gu.RoleID.Contains(x.ROLE_ID)).ToList();
                    var noAuthority = string.Empty;
                    foreach (var t in allAuth)
                    {
                        if (t.NO_AUTHORITY == null)
                        {
                            return true;
                        }
                        else
                        {
                            noAuthority = t.NO_AUTHORITY;
                        }

                        allNowAuth = noAuthority.Split(',').ToList();
                        var thisAuth = t.NO_AUTHORITY.Split(',');
                        foreach (var t0 in noAuthority.Split(',').ToList())
                        {
                            if (!thisAuth.Contains(t0))
                            {
                                allNowAuth.Remove(t0);
                            }
                        }

                    }
                    if (noAuthority.IsNullOrEmpty()) {
                        return true;
                    }
                    if (methodBase != null)
                    {
                        if (methodBase.Name == "DownFile")
                        {
                            if (allNowAuth.Contains<string>("导出"))
                            {
                                err.IsError = true;
                                err.Message = "您没有此查询的导出权限";
                                return false;
                            }
                        }

                    }
                }
                #endregion
            }
            return true;
        }

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="code">代码</param>
        /// <returns>查询一条</returns>
        public ProInterface.Models.TQuery QuerySingleByCode(string loginKey, ref ProInterface.ErrorInfo err, string code)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                var queryEnt = db.YL_QUERY.Where(x => x.CODE == code).ToList();

                if (queryEnt.Count > 0)
                {
                    var reEnt = Fun.ClassToCopy<YL_QUERY, ProInterface.Models.TQuery>(queryEnt[0]);
                    GlobalUser gu = Global.GetUser(loginKey);
                    if (gu == null)
                    {
                        err.IsError = true;
                        err.Message = "登录超时";
                        return null;
                    }



                    var allAuth = queryEnt[0].YL_ROLE_QUERY_AUTHORITY.Where(x => gu.RoleID.Contains(x.ROLE_ID)).ToList();
                    foreach (var t in allAuth)
                    {
                        if (reEnt.NoAuthority == null)
                        {
                            if (t.NO_AUTHORITY == null)
                            {
                                reEnt.NoAuthority = "";
                            }
                            else
                            {
                                reEnt.NoAuthority = t.NO_AUTHORITY;
                            }

                        }
                        else
                        {
                            if (reEnt.NoAuthority == null) reEnt.NoAuthority = "";
                            if (t.NO_AUTHORITY == null) t.NO_AUTHORITY = "";
                            var allNowAuth = reEnt.NoAuthority.Split(',').ToList();
                            var thisAuth = t.NO_AUTHORITY.Split(',');
                            foreach (var t0 in reEnt.NoAuthority.Split(',').ToList())
                            {
                                if (!thisAuth.Contains(t0))
                                {
                                    allNowAuth.Remove(t0);
                                }
                            }
                            reEnt.NoAuthority = string.Join(",", allNowAuth);
                        }
                    }
                    return reEnt;
                }
                return null;
            }
        }

        #endregion

        /// <summary>
        /// 替换默认参数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="gu"></param>
        /// <returns></returns>
        public string ReplacePer(string sql, GlobalUser gu, IList<QueryPara> paraList)
        {
            try
            {
                sql = sql.Replace("@{DISTRICT_ID}", (paraList.SingleOrDefault(x => x.ParaName == "DISTRICT_ID") == null) ? gu.DistrictId.ToString() : paraList.SingleOrDefault(x => x.ParaName == "DISTRICT_ID").Value);
                sql = sql.Replace("@{DISTRICT_CODE}", (paraList.SingleOrDefault(x => x.ParaName == "DISTRICT_CODE") == null) ? gu.DistrictCode.ToString() : paraList.SingleOrDefault(x => x.ParaName == "DISTRICT_CODE").Value);
                sql = sql.Replace("@{USER_ID}", (paraList.SingleOrDefault(x => x.ParaName == "USER_ID") == null) ? gu.UserId.ToString() : paraList.SingleOrDefault(x => x.ParaName == "USER_ID").Value);
                sql = sql.Replace("@{REGION}", (paraList.SingleOrDefault(x => x.ParaName == "REGION") == null) ? gu.Region : paraList.SingleOrDefault(x => x.ParaName == "REGION").Value);
                sql = sql.Replace("@{ALL_ROLE}", (paraList.SingleOrDefault(x => x.ParaName == "ALL_ROLE") == null) ? gu.GetRoleAllStr() : paraList.SingleOrDefault(x => x.ParaName == "ALL_ROLE").Value);
                sql = sql.Replace("@{ALL_REGION}", (paraList.SingleOrDefault(x => x.ParaName == "ALL_REGION") == null) ? gu.GetRegionLeveStr() : paraList.SingleOrDefault(x => x.ParaName == "ALL_REGION").Value);
                sql = sql.Replace("@{RULE_REGION}", (paraList.SingleOrDefault(x => x.ParaName == "RULE_REGION") == null) ? gu.RuleRegionStr : paraList.SingleOrDefault(x => x.ParaName == "RULE_REGION").Value);
                sql = sql.Replace("@{NOW_LEVEL_ID}", (paraList.SingleOrDefault(x => x.ParaName == "NOW_LEVEL_ID") == null) ? gu.LevelId.ToString() : paraList.SingleOrDefault(x => x.ParaName == "NOW_LEVEL_ID").Value);
            }
            catch { }
            var nowDt = DateTime.Now;
            sql = Fun.ReplaceDataTime(sql, nowDt);

            return sql;
        }

        public DataTable QueryExecute(string loginKey, ref ErrorInfo err, string queryCode, int pageIndex, int pageSize, string orderStr, string whereJsonStr, IList<QueryPara> paraList, ref string sqlStr)
        {

            DataTable reDt = new DataTable();
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "用户【" + loginKey + "】登录超时，请重新登录";
                return reDt;
            }
            using (DBEntities db = new DBEntities())
            {
                var query = db.YL_QUERY.SingleOrDefault(x => x.CODE == queryCode);
                TQuery ent = Fun.ClassToCopy<YL_QUERY, TQuery>(query);
                IList<QueryCfg> cfg = JSON.EncodeToEntity<IList<QueryCfg>>(query.QUERY_CFG_JSON);
                string whereStr = "";
                ent.QUERY_CONF = MakeSql(gu, ent.QUERY_CONF, orderStr, whereJsonStr, paraList, ref whereStr, Fun.GetDataBaseType());
                try
                {
                    sqlStr = ent.QUERY_CONF;
                    reDt = ExecuteSql(ent.QUERY_CONF, pageIndex, pageSize, orderStr, whereStr, query.DB_SERVER_ID.Value, cfg.Select(x => x.FieldName).ToList());
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = e.Message;
                    return reDt;
                }
            }
            reDt.TableName = "tables1";
            return reDt;
        }

        private string MakeSql(GlobalUser gu, string queryCconf, string orderStr, string whereJsonStr, IList<QueryPara> paraList, ref string whereStr, string dbType)
        {
            IList<QueryRowBtnShowCondition> whereList = new List<QueryRowBtnShowCondition>();
            if (!string.IsNullOrEmpty(whereJsonStr))
            {
                whereList = JSON.EncodeToEntity<IList<QueryRowBtnShowCondition>>(whereJsonStr);
            }

            foreach (var tmp in whereList.Where(x => x.OpType == null))
            {
                queryCconf = queryCconf.Replace("@(" + tmp.ObjFiled + ")", tmp.Value);
            }
            if (paraList != null)
            {
                foreach (var tmp in paraList)
                {
                    if (tmp.Value == "@(NOWDATA)")
                    {
                        tmp.Value = DateTime.Today.ToString("yyyy-MM-dd");
                    }
                    queryCconf = queryCconf.Replace("@(" + tmp.ParaName + ")", tmp.Value);
                }
            }


            StringBuilder whereSb = new StringBuilder();
            foreach (var tmp in whereList.Where(x => x.OpType != null))
            {
                var nowType = tmp.FieldType.ToLower();
                int subIndex = tmp.FieldType.IndexOf(".");
                if (subIndex > -1)
                {
                    nowType = nowType.Substring(subIndex + 1);
                }
                switch (nowType)
                {
                    case "string":
                        switch (tmp.OpType)
                        {
                            case "in":
                                whereSb.Append(string.Format(" {0} {1} ('{2}') and ", tmp.ObjFiled, tmp.OpType, tmp.Value.Replace(",", "','")));
                                break;
                            default:
                                if (tmp.OpType == "like") tmp.Value = "%" + tmp.Value + "%";
                                whereSb.Append(string.Format(" {0} {1} '{2}' and ", tmp.ObjFiled, tmp.OpType, tmp.Value));
                                break;
                        }
                        break;
                    case "datetime":
                        whereSb.Append(string.Format(" {0} and ", DbFun.WhereData(dbType, tmp.ObjFiled, tmp.OpType, tmp.Value)));
                        break;
                    default:
                        if (tmp.OpType == "in")
                        {
                            whereSb.Append(string.Format(" {0} {1} ('{2}') and ", tmp.ObjFiled, tmp.OpType, tmp.Value.Replace(",", "','")));
                        }
                        else
                        {
                            whereSb.Append(string.Format(" {0} {1} {2} and ", tmp.ObjFiled, tmp.OpType, tmp.Value));
                        }
                        break;
                }
            }
            if (whereSb.Length > 4)
                whereSb = whereSb.Remove(whereSb.Length - 4, 4);
            whereStr = whereSb.ToString();
            queryCconf = ReplacePer(queryCconf, gu, paraList);
            return queryCconf;
        }


        public int QueryCount(string loginKey, ref ErrorInfo err, string queryCode, string whereJsonStr, IList<QueryPara> paraList, ref string sqlStr)
        {
            using (DBEntities db = new DBEntities())
            {

                var query = db.YL_QUERY.SingleOrDefault(x => x.CODE == queryCode);
                TQuery ent = Fun.ClassToCopy<YL_QUERY, TQuery>(query);
                GlobalUser gu = Global.GetUser(loginKey);
                string whereStr = "";
                ent.QUERY_CONF = MakeSql(gu, ent.QUERY_CONF, null, whereJsonStr, paraList, ref whereStr, Fun.GetDataBaseType());

                string withStr = "";
                string WithStartStr = "-----WithStart-----";
                string WithEndStr = "-----WithEnd-----";
                int WithStartP = ent.QUERY_CONF.IndexOf(WithStartStr);
                int WithEndP = ent.QUERY_CONF.IndexOf(WithEndStr);
                if (WithStartP > -1 && WithEndP > -1 && WithEndP > WithStartP)
                {
                    withStr = ent.QUERY_CONF.Substring(WithStartStr.Length, WithEndP - WithStartP - WithStartStr.Length);
                    ent.QUERY_CONF = ent.QUERY_CONF.Substring(WithEndP + WithEndStr.Length);
                }



                if (!string.IsNullOrEmpty(whereStr))
                {
                    ent.QUERY_CONF = withStr + "SELECT * FROM ( " + ent.QUERY_CONF + " ) TMP where " + whereStr;
                }
                else
                {
                    ent.QUERY_CONF = withStr + "SELECT * FROM ( " + ent.QUERY_CONF + " ) TMP";
                }
                var reInt = 0;
                try
                {
                    sqlStr = ent.QUERY_CONF;
                    reInt = ExecuteCount(query.DB_SERVER_ID.Value, ent.QUERY_CONF);
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = e.Message;
                }
                return reInt;
            }
        }

        public IList<QueryCfg> QueryGetCfg(string loginKey, ref ErrorInfo err, string sql, string queryCode)
        {

            using (DBEntities db = new DBEntities())
            {
                var query = db.YL_QUERY.SingleOrDefault(x => x.CODE == queryCode);

                IList<QueryCfg> nowCfgList = new List<QueryCfg>();
                GlobalUser gu = Global.GetUser(loginKey);
                if (gu == null)
                {
                    err.IsError = true;
                    err.Message = "用户登录超时，请重新登录";
                    return nowCfgList;

                }

                if (sql.IndexOf("<sql>") > -1)
                {
                    var xml = XmlHelper.Document.Load(sql);
                }




                sql = ReplacePer(sql, gu, null);


                DataTable dt = new DataTable();
                try
                {
                    dt = ExecuteGetNullTable(query.DB_SERVER_ID.Value, sql);
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return nowCfgList;
                }
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var t = dt.Columns[i];
                    var tmp = t.DataType.FullName.ToLower().Substring(t.DataType.FullName.IndexOf(".") + 1);
                    IList<string> numberList = new[] { "int", "decimal", "double", "int64", "int16" };
                    if (numberList.Contains(tmp))
                    {
                        tmp = "int";
                    }
                    string searchType = "";
                    string searchScript = null;
                    switch (tmp)
                    {
                        case "int":
                            searchScript = "$('{@this}').numberbox({min:0,precision:0});";
                            searchType = "numberbox";
                            break;
                        case "datetime":
                            searchScript = "$('{@this}').datetimebox({showSeconds: false,required: false});";
                            searchType = "datetimebox";
                            break;
                        default:
                            searchType = "text";
                            break;
                    }

                    nowCfgList.Add(new QueryCfg()
                    {
                        FieldName = t.ColumnName,
                        Show = true,
                        FieldType = t.DataType.FullName,
                        Width = 0,
                        CanSearch = true,
                        SearchType = searchType,
                        SearchScript = searchScript,
                        Sortable = true,
                        Alias = t.Caption
                    });
                }
                #region 获取当前状态
                {
                    var queryEnt = db.YL_QUERY.SingleOrDefault(x => x.CODE == queryCode);
                    if (queryEnt != null)
                    {
                        IList<QueryCfg> old = JSON.EncodeToEntity<IList<QueryCfg>>(queryEnt.QUERY_CFG_JSON);
                        if (old != null)
                        {
                            for (int i = 0; i < nowCfgList.Count; i++)
                            {
                                var t0 = old.SingleOrDefault(x => x.FieldName == nowCfgList[i].FieldName);
                                if (t0 != null) nowCfgList[i] = t0;
                            }
                        }
                    }
                }
                #endregion
                return nowCfgList;
            }
        }

        public byte[] QueryExportExcel(string loginKey, ref ErrorInfo err, string queryCode, string orderStr, string whereStr, IList<QueryPara> paraList, string allShow, ref string sqlStr, ref int allNum)
        {
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "用户【" + loginKey + "】登录超时，请重新登录";
                return null;
            }

            DataTable reDt = QueryDataTable(loginKey, ref err, queryCode, orderStr, whereStr, paraList, ref sqlStr);
            if (!string.IsNullOrEmpty(allShow))
            {
                var allShowList = allShow.Split(',').ToList();
                var nowColList = new List<string>();
                for (var i = 0; i < reDt.Columns.Count; i++)
                {
                    nowColList.Add(reDt.Columns[i].ColumnName);
                }

                var z1 = nowColList.Except(allShowList);
                foreach (var i in z1)
                {
                    reDt.Columns.Remove(i);
                }
            }

            allNum = reDt.Rows.Count;


            using (DBEntities db = new DBEntities())
            {
                YL_EXPORT_LOG ent = new YL_EXPORT_LOG();
                ent.USER_ID = gu.UserId;
                ent.LOGIN_NAME = gu.UserName;
                ent.NAME = queryCode;
                ent.SQL = sqlStr;
                ent.EXPORT_TIME = DateTime.Now;
                db.YL_EXPORT_LOG.Add(ent);
                db.SaveChanges();
            }

            return ExcelHelper.ExportDTtoByte(reDt, "数据");
        }



        public DataTable QueryDataTable(string loginKey, ref ErrorInfo err, string queryCode, string orderStr, string whereStr, IList<QueryPara> paraList, ref string sqlStr)
        {
            DataTable reDt = new DataTable();
            using (DBEntities db = new DBEntities())
            {
                var query = db.YL_QUERY.SingleOrDefault(x => x.CODE == queryCode);
                TQuery ent = Fun.ClassToCopy<YL_QUERY, TQuery>(query);

                GlobalUser gu = Global.GetUser(loginKey);

                ent.QUERY_CONF = MakeSql(gu, ent.QUERY_CONF, orderStr, whereStr, paraList, ref whereStr, Fun.GetDataBaseType());

                reDt = ExecuteSqlAll(query.DB_SERVER_ID.Value, ent.QUERY_CONF, orderStr, whereStr, ref sqlStr);
                reDt.TableName = "tables1";
                if (ent.QueryCfg != null)
                {
                    foreach (var par in ent.QueryCfg)
                    {
                        if (reDt.Columns[par.FieldName] != null)
                        {
                            if (par.Show)
                            {
                                reDt.Columns[par.FieldName].Caption = par.Alias;
                                //reDt.Columns[par.FieldName].ColumnName = par.Alias;
                            }
                            else
                            {
                                reDt.Columns.Remove(par.FieldName);
                            }
                        }
                    }
                }
                return reDt;
            }
        }
        public string MoudlePathByID(int? query_id)
        {
            string result = "";
            using (DBEntities db = new DBEntities())
            {
                var nowQuery = db.YL_QUERY.SingleOrDefault(p => p.ID == query_id);
                var local = "~/Query/Query?code=" + nowQuery.CODE;
                var nowMoudel = db.YL_MODULE.Where(p => p.LOCATION == local).ToList()[0];
                IList<string> strList = new List<string>();
                strList.Add(nowMoudel.NAME);
                GetMod(db, nowMoudel, ref strList);
                if (strList != null && strList.Count > 0)
                {
                    strList = strList.Reverse().ToList();
                    foreach (var item in strList)
                    {
                        result += item + ",";
                    }
                }
                if (result != "")
                {
                    result = result.Substring(0, result.LastIndexOf(","));
                }
            }
            return result;
        }
        public string MoudlePathByUrl(string url)
        {
            string result = "";
            using (DBEntities db = new DBEntities())
            {
                var nowMoudelList = db.YL_MODULE.Where(p => url.Contains(p.LOCATION.Substring(1))).ToList();
                if (nowMoudelList != null && nowMoudelList.Count > 0)
                {
                    var nowMoudel = nowMoudelList[0];
                    //var nowQuery = db.YL_QUERY.SingleOrDefault(p => p.ID == query_id);
                    //var local = "~/Query/Query?code=" + nowQuery.CODE;
                    //var nowMoudel = db.YL_MODULE.Where(p => p.LOCATION == url).ToList()[0];
                    IList<string> strList = new List<string>();
                    strList.Add(nowMoudel.NAME);
                    GetMod(db, nowMoudel, ref strList);
                    if (strList != null && strList.Count > 0)
                    {
                        strList = strList.Reverse().ToList();
                        foreach (var item in strList)
                        {
                            result += item + ",";
                        }
                    }
                    if (result != "")
                    {
                        result = result.Substring(0, result.LastIndexOf(","));
                    }
                }
            }
            return result;
        }
        //递归获取父级
        public void GetMod(DBEntities db, YL_MODULE mod, ref IList<string> strList)
        {

            if (mod.PARENT_ID != null)
            {
                var currMod = db.YL_MODULE.SingleOrDefault(p => p.ID == mod.PARENT_ID);
                strList.Add(currMod.NAME);
                GetMod(db, currMod, ref strList);
            }
            else
            {
                return;
            }
        }



        public bool QueryDelete(string loginKey, ref ErrorInfo err, int id)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_QUERY.SingleOrDefault(a => a.ID == id);
                    foreach (var t in ent.YL_ROLE_QUERY_AUTHORITY.ToList())
                    {
                        db.YL_ROLE_QUERY_AUTHORITY.Remove(t);
                    }
                    db.YL_QUERY.Remove(ent);

                    db.SaveChanges();
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Delete);
                    return true;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = e.Message;
                    err.Excep = e;
                    return false;
                }
            }
        }


    }
}
