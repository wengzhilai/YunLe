using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ProInterface;
using System.Data;
using ProInterface.Models;
using System.Reflection;

namespace Web.Controllers
{
    public class QueryController : Controller
    {
        #region 默认方法
        [HttpPost]
        public string AjaxList(FormCollection collection)
        {
            int pageIndex = Convert.ToInt32(collection["page"]);
            int pageSize = Convert.ToInt32(collection["rows"]);
            string sortField = collection["sort"];
            string sortOrder = collection["order"];
            string key = collection["key"];
            string keyValue = collection["keyValue"];
            string lambdaStr = null;
            string lambdaInt = null;

            if (sortField == null || sortField == "")
            {
                sortField = "ID";
            }


            if (keyValue != null && keyValue.Trim() != "")
            {
                lambdaStr = "a => a." + key + ".Contains(\"" + keyValue + "\")";
                lambdaInt = "a=>a." + key + "==" + keyValue + "";
            }
            ProInterface.IQuery ems = new ProServer.Service();
            Fun.Err = new ProInterface.ErrorInfo();
            var Ilist = ems.QueryWhere(Fun.UserKey, ref Fun.Err, pageIndex, pageSize, lambdaStr, sortField, sortOrder);
            int all = 0;
            if (Fun.Err.IsError)
            {
                Ilist = ems.QueryWhere(Fun.UserKey, ref Fun.Err, pageIndex, pageSize, lambdaInt, sortField, sortOrder);
                all = ems.Query_Count(Fun.UserKey, ref Fun.Err, lambdaInt);
            }
            else
            {
                all = ems.Query_Count(Fun.UserKey, ref Fun.Err, lambdaStr);
            }
            var t = JSON.DecodeToStr(all, Ilist);
            return t;
        }


        public ActionResult Index(string code)
        {

            ProInterface.IQuery ems = new ProServer.Service();
            var ent = ems.QuerySingle(Fun.UserKey, ref Fun.Err, "x=>x.CODE='" + code + "'");
            return View(ent);
        }


        /// <summary>
        /// 编辑模块
        /// </summary>
        /// <param name="id">模块ID</param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            ProInterface.ErrorInfo error = new ErrorInfo();
            ProServer.Service db = new ProServer.Service();
            ViewData["db"] = db.DbServer_Where(Fun.UserKey, ref error, 1, 100, null, "ID", "asc").Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.NICKNAME }).ToList();
            var _Ent = new ProInterface.Models.TQuery();
            if (id != null)
            {
                _Ent = db.QuerySingleId(Fun.UserKey, ref error, id.Value);
            }
            else
            {
                _Ent.HEARD_BTN = "";
                _Ent.PAGE_SIZE = 100;
                _Ent.AUTO_LOAD = 1;
                _Ent.SHOW_CHECKBOX = 1;
                _Ent.IS_DEBUG = 1;
                _Ent.ROWS_BTN = "[]";
                _Ent.HEARD_BTN = "[]";
            }
            return View(_Ent);
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="ent">模块类</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Details(ProInterface.Models.QUERY ent)
        {

            ProInterface.IQuery ems = new ProServer.Service();

            IList<ProInterface.Models.QueryRowBtn> allRowBtn = JSON.EncodeToEntity<List<ProInterface.Models.QueryRowBtn>>(ent.ROWS_BTN);
            //ent.ROWS_BTN = JSON.DecodeToStr(allRowBtn);

            ProInterface.ErrorInfo err = new ErrorInfo();
            ems.QuerySave(Fun.UserKey, ref err, ent, Request.Form.AllKeys.ToList());
            if (!err.IsError)
            {
                err.Message = "保存成功";
            }
            return Json(err); ;
        }

        public ActionResult Delete(string id)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            if (id != null)
            {
                int fail = 0, succ = 0;
                string[] idArr = id.Split(',');
                ProInterface.IQuery db = new ProServer.Service();
                for (int i = 0; i < idArr.Count(); i++)
                {
                    int _t = 0;
                    try
                    {
                        _t = Convert.ToInt32(idArr[i]);
                        if (db.QueryDelete(Fun.UserKey, ref error, _t))
                        {
                            succ++;
                        }
                        else
                        {
                            fail++;
                        }
                    }
                    catch { continue; }
                }
                error.Message = "删除成功[" + succ + "]个\\r\\n删除失败[" + fail + "]个";
            }
            else
            {
                error.Message = "删除失败";
            }
            return Json(error,JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult GetProperDescription(string proName)
        {
            return Json(ProServer.Fun.GetClassProperDescription<ProInterface.Models.TQuery>(proName), JsonRequestBehavior.AllowGet);
        }


        public ActionResult QueryArr(string code, string authToken)
        {
            string userKey = Fun.UserKey;
            if (!string.IsNullOrEmpty(authToken))
            {
                Fun.UserKey = authToken;
                userKey = authToken;
            }
            if (string.IsNullOrEmpty(Fun.UserKey))
            {
                return Redirect("~/Login/Login");
            }
            ErrorInfo error = new ErrorInfo();
            return View();
        }

        public ActionResult QueryList()
        {
            string errorStr = "{{\"msgtitle\":\"{0}\",\"msgtype\":\"error\",\"msginfo\":\"{1}\",\"msgsta\":2,\"total\":0,\"rows\":[]}}";
            ProServer.Service db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            string code = Request["Code"];
            if (string.IsNullOrEmpty(code))
            {
                return Content(string.Format(errorStr, "参数错误", "代码不存在"));
            }

            var url =string.Format("~/Query/Query?code={0}",code);
            if (!db.ModuleUserAuthority(Fun.UserKey, ref error, url))
            {
                return Content(string.Format(errorStr, "非法操作","无权限操作"));
            }


            int pageIndex = Convert.ToInt32(Request["page"]);
            //pageIndex = pageIndex + 1;
            int pageSize = Convert.ToInt32(Request["rows"]);
            string sortField = Request["sort"];
            string sortOrder = Request["order"];
            IList<ProInterface.Models.QueryPara> AllPara=new List<ProInterface.Models.QueryPara>();
            if (Request["AllParaStr"] != null)
            {
                AllPara = ProInterface.JSON.EncodeToEntity<List<ProInterface.Models.QueryPara>>(Request["AllParaStr"]);
            }
            if (AllPara!=null)
            {
            foreach (var t in AllPara)
            {
                t.Value = Server.UrlDecode(t.Value);
            }
            }
            if (AllPara == null) AllPara = new List<ProInterface.Models.QueryPara>();
            string WhereStr = Request["WhereStr"];

            IList<QueryRowBtnShowCondition> whereList = new List<QueryRowBtnShowCondition>();
            if (!string.IsNullOrEmpty(WhereStr))
            {
                whereList = JSON.EncodeToEntity<IList<QueryRowBtnShowCondition>>(WhereStr);
            }
            foreach (var t in whereList)
            {
                t.Value = Server.UrlDecode(t.Value);
            }
            WhereStr = JSON.DecodeToStr(whereList);


            if (sortField == null || sortField == "")
            {
                var ent = db.QuerySingleByCode(Fun.UserKey, ref error, code);
                if (error.IsError)
                {
                    return Content(string.Format(errorStr, "获取出错", error.IsError));
                }
                sortField = ent.QueryCfg[0].FieldName;
            }
            string orderStr = null;
            if (!string.IsNullOrEmpty(sortOrder))
            {
                orderStr = sortField + " " + sortOrder;
            }
            string reSql = "";
            var Ilist = db.QueryExecute(Fun.UserKey, ref error, code, pageIndex, pageSize, orderStr, WhereStr, AllPara.ToArray(), ref reSql);
            string logStr = "";
            if (error.IsError)
            {
                logStr = string.Format("参数:{2}\r\n执行查询语句出错：{0}\r\n{1}", error.Message, reSql, Request["AllParaStr"]);
                if (!Fun.QueryDt.Keys.Contains(code))
                {
                    Fun.QueryDt.Add(code, logStr);
                }
                else
                {
                    Fun.QueryDt[code] = logStr;
                }
                Fun.WriteLog(logStr);
                return Content(string.Format(errorStr, "数据库错误", "请联系管理员"));
            }
            int all = 0;
            all = db.QueryCount(Fun.UserKey, ref error, code, WhereStr, AllPara.ToArray(), ref reSql);
            if (error.IsError)
            {
                logStr = string.Format("参数:{2}\r\n执行统计语句出错：{0}\r\n{1}", error.Message, reSql, Request["AllParaStr"]);
                if (!Fun.QueryDt.Keys.Contains(code))
                {
                    Fun.QueryDt.Add(code, logStr);
                }
                else
                {
                    Fun.QueryDt[code] = logStr;
                }
                Fun.WriteLog(logStr);
                return Content(string.Format(errorStr, "数据库错误", "请联系管理员"));
            }
            logStr = string.Format("参数:{2}\r\n执行语句：{0}\r\n{1}", error.Message, reSql, Request["AllParaStr"]);
            Fun.WriteLog(logStr);

            if (!Fun.QueryDt.Keys.Contains(code))
            {
                Fun.QueryDt.Add(code, logStr);
            }
            else
            {
                Fun.QueryDt[code] = logStr;
            }
            ProInterface.Models.DataGridDataJson reEnt = new ProInterface.Models.DataGridDataJson();
            reEnt.total = all;
            reEnt.rows = Ilist;
            var t1 = JSON.DecodeToStr(reEnt);
            return Content(t1);
        }

        [ValidateInput(false)]
        public ActionResult ReloadCfg(string sql, string code)
        {
            ProInterface.IQuery ems = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            var reObj = ems.QueryGetCfg(Fun.UserKey, ref error, sql, code);
            if (error.IsError)
            {
                return Json(error, JsonRequestBehavior.AllowGet);
            }
            return Json(reObj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DownRDLC(int ID)
        {

            ProInterface.IQuery ems = new ProServer.Service();
            byte[] tmp = new byte[5];
            string fileName = "报表.rdlc";
            var ent = ems.QuerySingle(Fun.UserKey, ref Fun.Err, "x=>x.ID==" + ID + "");
            if (ent != null)
            {
                if (string.IsNullOrEmpty(ent.REPORT_SCRIPT))
                {
                    ent.REPORT_SCRIPT = MakeRdlc(ent.QueryCfg, ent.CODE);
                }
                tmp = Encoding.UTF8.GetBytes(ent.REPORT_SCRIPT);
                fileName = ent.NAME + DateTime.Now.ToString() + ".rdlc";
            }
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Response.ContentType = "application/octet-stream";

            Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(fileName));
            Response.BinaryWrite(tmp);
            Response.Flush();
            Response.End();
            return new EmptyResult();
        }

        public string MakeRdlc(IList<ProInterface.Models.QueryCfg> AllPara, string code)
        {
            string reStr = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Report xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition"" xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"">
  <Width>6.5in</Width>
  <Body>
    <Height>2in</Height>
  </Body>
  <rd:ReportTemplate>true</rd:ReportTemplate>
  <Page>
  </Page>
{0}
</Report>
";
            string dataStr = @"
<DataSources>
    <DataSource Name=""{1}"">
      <ConnectionProperties>
        <DataProvider>System.Data.DataSet</DataProvider>
        <ConnectString>/* Local Connection */</ConnectString>
      </ConnectionProperties>
      <rd:DataSourceID>{2}</rd:DataSourceID>
    </DataSource>
  </DataSources>
  <DataSets>
    <DataSet Name=""DataSet1"">
      <Query>
        <DataSourceName>{1}</DataSourceName>
        <CommandText>/* Local Query */</CommandText>
      </Query>
      <Fields>
{0}
      </Fields>
      <rd:DataSetInfo>
        <rd:DataSetName>{1}</rd:DataSetName>
        <rd:SchemaPath></rd:SchemaPath>
        <rd:TableName>DataTable1</rd:TableName>
        <rd:TableAdapterFillMethod />
        <rd:TableAdapterGetDataMethod />
        <rd:TableAdapterName />
      </rd:DataSetInfo>
    </DataSet>
  </DataSets>
";
            var fieldStr = @"
        <Field Name=""{0}"">
          <DataField>{1}</DataField>
          <rd:TypeName>{2}</rd:TypeName>
        </Field>
";
            var allFieldStr = "";
            foreach (var t in AllPara)
            {
                allFieldStr += string.Format(fieldStr, t.Alias, t.FieldName, t.FieldType);
            }
            dataStr = string.Format(dataStr, allFieldStr, code, new Guid().ToString());
            reStr = string.Format(reStr, dataStr);
            return reStr;
        }

        public ActionResult DownFile(string code, string sortName, string sortOrder, string WhereStr, string AllParaStr, string AllShow, string authToken = null)
        {
            ErrorInfo error = new ErrorInfo();

            if (!string.IsNullOrEmpty(sortName) && sortName.Split(' ').Count() > 1)
            {
                return Content("参数用误");
            }

            if (!string.IsNullOrEmpty(authToken))
            {
                Fun.UserKey = authToken;
            }
            if (string.IsNullOrEmpty(Fun.UserKey))
            {
                return Content("Login");
            }
            ProServer.Service ems = new ProServer.Service();

            //是否越权
            if (!ems.QueryCheckFunctioAuthority(Fun.UserKey, ref error,code,MethodBase.GetCurrentMethod()))
            {
                return Content(error.Message);
            }
            //sortField = collection["sort"];
            //sortOrder = collection["order"];
            //code = collection["Code"];
            //WhereStr = collection["WhereStr"];
            IList<ProInterface.Models.QueryPara> AllPara = ProInterface.JSON.EncodeToEntity<List<ProInterface.Models.QueryPara>>(AllParaStr);
            if (AllPara == null) AllPara = new List<ProInterface.Models.QueryPara>();
            if (code == null) return null;
            
            var queryEnt = new ProInterface.Models.TQuery();
            queryEnt = ems.QuerySingleByCode(Fun.UserKey, ref error, code);
            if (string.IsNullOrEmpty(sortName))
            {
                sortName = queryEnt.QueryCfg[0].FieldName;
            }
            if (sortOrder == null || sortOrder == "")
            {
                sortOrder = "asc";
            }

            string orderStr = sortName + " " + sortOrder;
            string sqlStr = "";
            byte[] tmp = new byte[5];
            int allNum = 0;
            tmp = ems.QueryExportExcel(Fun.UserKey, ref Fun.Err,code, orderStr, WhereStr, AllPara.ToArray(),AllShow,ref sqlStr,ref allNum);

            string fileName = string.Format("{0}.xls", queryEnt.NAME);
            string absoluFilePath = string.Format("~/UpFiles/{0}.xls", queryEnt.NAME);
            if (allNum > 10000)
            {
                absoluFilePath = string.Format("~/UpFiles/{0}.csv", queryEnt.NAME);
                fileName = string.Format("{0}.csv", queryEnt.NAME);
            }
            //System.IO.File.WriteAllBytes(Server.MapPath(absoluFilePath), tmp);

            //return Content(Url.Content(absoluFilePath));
            return File(tmp, "application/octet-stream", Server.UrlEncode(fileName));

            //Response.Charset = "UTF-8";
            //Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            //Response.ContentType = "application/octet-stream";
            //Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(fileName));
            //Response.BinaryWrite(tmp);
            //Response.Flush();
            //Response.End();
            //return new EmptyResult();
        }

        public ActionResult Query(string code, string authToken)
        {
            string userKey = Fun.UserKey;
            if (!string.IsNullOrEmpty(authToken))
            {
                Fun.UserKey = authToken;
                userKey = authToken;
            }
            if (string.IsNullOrEmpty(Fun.UserKey))
            {
                return Redirect("~/Login/Login");
            }
            
            ErrorInfo error=new ErrorInfo();
            ProServer.Service db = new ProServer.Service();

            var url = Request.Url.PathAndQuery;
            url = url.Trim('&');
            if (url.IndexOf("&_") > 0)
            {
                url = url.Substring(0, url.LastIndexOf("&_"));
            }
            if (!db.ModuleUserAuthority(Fun.UserKey, ref error, url))
            {
                return Content("无操作权限");
            }

            ViewData["Dis1"] = db.DistrictGetUserLevel(userKey, ref error, 1, Request["DISTRICT_ID"]);
            ViewData["Dis2"] = db.DistrictGetUserLevel(userKey, ref error, 2, Request["DISTRICT_ID"]);
            ViewData["Dis3"] = db.DistrictGetUserLevel(userKey, ref error, 3, Request["DISTRICT_ID"]);
            ViewData["Dis4"] = db.DistrictGetUserLevel(userKey, ref error, 4, Request["DISTRICT_ID"]);

            ProServer.GlobalUser gu = ProServer.Global.GetUser(userKey);
            try
            {
                ViewData["Gu"] = gu;
                ViewData["mydis"] = gu.DistrictId;
            }
            catch { }
            IList<ProInterface.Models.QueryPara> para = new List<ProInterface.Models.QueryPara>();
            foreach (var t in Request.QueryString.Keys)
            {
                if (t != null)
                {
                    para.Add(new ProInterface.Models.QueryPara() { ParaName = t.ToString(), Value = Request.QueryString[t.ToString()] });
                }
            }
            ViewData["para"] = para;
            ViewBag.AllPara = ProInterface.JSON.DecodeToStr(para);
            ViewBag.Code = code;

            var ent = db.QuerySingleByCode(userKey, ref error, code);
            if (error.IsError || ent==null)
            {
                return Content(string.Format("查询出错代码[{0}]\r\n{1}\r\n{2}", code, error.Message, Request.Url.AbsoluteUri));
                
                //return Content("");
            }
            //写日志
            var isTrue = ProServer.Fun.WriteModuleLog("后台-"+ent.NAME, gu.UserId);

            return View(ent);
        }

        public ActionResult Query2(string code, string authToken)
        {
            return Query(code, authToken);
        }

        public ActionResult GetDubug(string code)
        {
            var reStr= "";

            if (Fun.QueryDt.Keys.Contains(code))
            {
                reStr = Fun.QueryDt[code];
            }

            reStr = reStr.Replace("%0a", "\r\n");
            reStr = reStr.Replace("%0d", " ");
            
            return Content(reStr);
        }

        public ActionResult ExecSql(string sql,int dbId=1)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();

            if (!Fun.ProcessSqlStr(sql, ref error))
            {
                return JavaScript(error.Message);
            }
            DataTable dt = new DataTable();
            string reStr = "";
            try
            {
                dt = ExecSqlDt(sql, dbId);
                reStr = JSON.DecodeToStr(dt);
            }
            catch (Exception e)
            {
                error.IsError = true;
                error.Message = e.Message;
                reStr = JSON.DecodeToStr(error);
            }
            return Content(reStr);
        }

        public DataTable ExecSqlDt(string sql, int? dbId = 1)
        {
            ProServer.Service db = new ProServer.Service();
            return db.ExecuteSqlToTable(dbId.Value, sql);
        }


        //根据queryID获取路径
        public string GetPathByID(int? query_id)
        {
            string resutl = "";
            ProInterface.IQuery ems = new ProServer.Service();
            resutl = ems.MoudlePathByID(query_id);
            return resutl;
        }

        //根据URL获取路径
        public string GetPathByUrl(string url)
        {
            string resutl = "";
            ProInterface.IQuery ems = new ProServer.Service();
            resutl = ems.MoudlePathByUrl(url);
            return resutl;
        }
    }
}
