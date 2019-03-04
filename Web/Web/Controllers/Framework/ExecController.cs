using ProInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class ExecController : Controller
    {
        //
        // GET: /Exec/

        public ActionResult Index(string sql,string dbId)
        {
            var gu = ProServer.Global.GetUser(Fun.UserKey);
            if (gu == null)
            {
                return Content("登录超时");
            }
            ViewData["NowDisId"] = gu.DistrictId;
            ProInterface.IDbServer db=new ProServer.Service();
            var allDb = db.DbServerGetAllDb();
            if (dbId != null)
            {
                var tmp = allDb.SingleOrDefault(x => x.Value == dbId);
                if (tmp != null) tmp.Selected = true;
            }
            ViewData["dbServerID"] = allDb;
            return View();
        }

        public ActionResult ExecSqlColumns(FormCollection collection)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            try
            {
                string sql = collection["sql"];
                if (!Fun.ProcessSqlStr(sql, ref error))
                {
                    return JavaScript(error.Message);
                }

                DateTime nowDt = DateTime.Now;
                string dtStr = Request["MONTHDAY"];
                if (dtStr != null && dtStr.Length == 8)
                {
                    try
                    {
                        dtStr = dtStr.Insert(6, "-");
                        dtStr = dtStr.Insert(4, "-");
                        nowDt = Convert.ToDateTime(dtStr);
                    }
                    catch { }
                }

                sql = sql.Replace("@{day}", nowDt.ToString("yyyyMMdd"));
                sql = sql.Replace("@{month}", Request["MONTH"]);


                sql = ProServer.Fun.ReplaceDataTime(sql, nowDt, Fun.UserKey);
                sql = sql.Replace("@(DISTRICT_ID)", Request["DISTRICT_ID"]);
                int dbServerID = Convert.ToInt32(collection["dbServerID"]);
                string type = collection["type"];

                string reStr = "";
                ProInterface.IDbServer db = new ProServer.Service();
                error = new ProInterface.ErrorInfo(); ;
                switch (type)
                {
                    case "command":
                        break;
                    default:

                        reStr = db.DbServerColumns(dbServerID, sql);
                        break;
                }
                if (error.IsError)
                {
                    return JavaScript(error.Message);
                }
                return JavaScript(reStr);
            }
            catch (Exception e)
            {
                return JavaScript(e.Message);
            }
        }
        public ActionResult ExecSql(FormCollection collection)
        {
            string sql = collection["sql"];
            if (!Fun.ProcessSqlStr(sql, ref Fun.Err))
            {
                return JavaScript(Fun.Err.Message);
            }
            int dbServerID = Convert.ToInt32(collection["dbServerID"]);
            string type = collection["type"];
            int pageIndex = Convert.ToInt32(collection["page"]);
            int pageSize = Convert.ToInt32(collection["rows"]);
            string sortField = collection["sort"];
            string sortOrder = collection["order"];
            string defaultOrder = collection["defaultOrder"];
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = defaultOrder;
            }
            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "ASC";
            }
            string orderStr = sortField + " " + sortOrder;
            string reStr = "";
            ProInterface.IDbServer db = new ProServer.Service();
            ErrorInfo error = new ProInterface.ErrorInfo();
            DateTime nowDt = DateTime.Now;
            string dtStr=Request["MONTHDAY"];

            if (dtStr!=null && dtStr.Length == 8)
            {
                try
                {
                    dtStr = dtStr.Insert(6, "-");
                    dtStr = dtStr.Insert(4, "-");
                    nowDt = Convert.ToDateTime(dtStr);
                }
                catch { }
            }
            sql = sql.Replace("@{day}", nowDt.ToString("yyyyMMdd"));
            sql = sql.Replace("@{month}", Request["MONTH"]);

            sql = ProServer.Fun.ReplaceDataTime(sql, nowDt,Fun.UserKey);
            sql = sql.Replace("@(DISTRICT_ID)", Request["DISTRICT_ID"]);
            switch (type)
            {
                case "command":
                    break;
                default:
                    reStr = db.DbServerSqlJson(Fun.UserKey, ref error, dbServerID, sql, pageIndex, pageSize, orderStr);
                    break;
            }
            if (error.IsError)
            {
                return JavaScript("alert('"+Fun.Err.Message+"')");
            }
            return JavaScript(reStr);
        }

        [HttpPost]
        public ActionResult ExportToExcel(FormCollection collection)
        {
            string sql = collection["SQL_INPUT"];
            ProInterface.IDbServer db = new ProServer.Service();
            ErrorInfo error = new ProInterface.ErrorInfo();
            if (!Fun.ProcessSqlStr(sql, ref Fun.Err))
            {
                return JavaScript(error.Message);
            }

            DateTime nowDt = DateTime.Now;
            string dtStr = Request["MONTHDAY"];
            if (dtStr != null && dtStr.Length == 8)
            {
                try
                {
                    dtStr = dtStr.Insert(6, "-");
                    dtStr = dtStr.Insert(4, "-");
                    nowDt = Convert.ToDateTime(dtStr);
                }
                catch { }
            }
            sql = sql.Replace("@{day}", nowDt.ToString("yyyyMMdd"));
            sql = sql.Replace("@{month}", Request["MONTH"]);

            sql = ProServer.Fun.ReplaceDataTime(sql, nowDt,Fun.UserKey);
            sql = sql.Replace("@(DISTRICT_ID)", Request["DISTRICT_ID"]);


            int dbServerID = Convert.ToInt32(collection["dbServerID"]);
            byte[] tmp = new byte[5];

            string fileName = "report.xls";

            switch (Request["DownType"].ToString())
            {
                case "EXCEL":
                    int allNum = 0;
                    tmp = db.DbServerXlsByte(dbServerID, sql, ref allNum);
                    fileName = "report.xls";
                    if (allNum > 10000)
                    {
                        fileName = "report.CSV";
                    }
                    break;
                case "CSV":
                    tmp = db.DbServerTxtByte(dbServerID, sql);
                    fileName = "report.CSV";
                    break;
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

    }
}
