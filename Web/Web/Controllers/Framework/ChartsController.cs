using ProInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers.Framework
{
    public class ChartsController : Controller
    {
        //
        // GET: /FusionCharts/

        public ActionResult QueryList()
        {

            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            IList<ProInterface.Models.QueryPara> AllPara = ProInterface.JSON.EncodeToEntity<List<ProInterface.Models.QueryPara>>(Request["AllParaStr"]);
            if (AllPara == null) AllPara = new List<ProInterface.Models.QueryPara>();
            string chartsType = Request["chartsType"];
            string code = Request["Code"];
            if (code == null)
            {
                return Json("[1]", JsonRequestBehavior.AllowGet);
            }
            string WhereStr = Request["WhereStr"];
            ProInterface.IChart db = new ProServer.Service();

            string reSql = "";
            string logStr = "";
            if (chartsType == "MultiSeries")
            {
                var Ilist = db.ChartGetByQueryCodeMulti(Fun.UserKey, ref error, code, AllPara.ToArray(), ref reSql);
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
                    return Json(error, JsonRequestBehavior.AllowGet);
                }
                logStr = string.Format("参数:{2}\r\n执行语句：{0}\r\n{1}", error.Message, reSql, Request["AllParaStr"]);
                if (!Fun.QueryDt.Keys.Contains(code))
                {
                    Fun.QueryDt.Add(code, logStr);
                }
                else
                {
                    Fun.QueryDt[code] = logStr;
                }
                return Json(Ilist, JsonRequestBehavior.AllowGet); ;
            }
            else
            {
                var Ilist = db.ChartGetByQueryCodeSingle(Fun.UserKey, ref error, code, AllPara.ToArray(), ref reSql);
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
                    return Json(error, JsonRequestBehavior.AllowGet);
                }
                logStr = string.Format("参数:{2}\r\n执行语句：{0}\r\n{1}", error.Message, reSql, Request["AllParaStr"]);
                if (!Fun.QueryDt.Keys.Contains(code))
                {
                    try
                    {
                        Fun.QueryDt.Add(code, logStr);
                    }
                    catch {
                        Fun.QueryDt.Clear();
                        Fun.QueryDt.Add(code, logStr);
                    }
                }
                else
                {
                    Fun.QueryDt[code] = logStr;
                }
                return Json(Ilist, JsonRequestBehavior.AllowGet); ;
            }
        }

        public ActionResult GetDubug(string code)
        {
            var reStr = "";
            if (Fun.QueryDt.Keys.Contains(code))
            {
                reStr = Fun.QueryDt[code];
            }

            reStr = reStr.Replace("%0a", "\r\n");
            reStr = reStr.Replace("%0d", " ");

            return Content(reStr);
        }


        public ActionResult Charts(string code, string authToken)
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                Fun.UserKey = authToken;
            }
            if (string.IsNullOrEmpty(Fun.UserKey))
            {
                return Redirect("~/Login/Login");
            }

            IList<ProInterface.Models.QueryPara> para = new List<ProInterface.Models.QueryPara>();
            foreach (var t in Request.QueryString.Keys)
            {
                if (t != null)
                {
                    para.Add(new ProInterface.Models.QueryPara() { ParaName = t.ToString(), Value = Request.QueryString[t.ToString()] });
                }
            }
            ViewBag.AllPara = ProInterface.JSON.DecodeToStr(para);
            ViewBag.Code = code;

            ProServer.Service db = new ProServer.Service();
            var ent = db.QuerySingle(Fun.UserKey, ref Fun.Err, "x=>x.CODE==\"" + code + "\"");
            if (ent == null)
            {
                return Content("查询代码[" + code + "]不存在");
            }
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            IList<SelectListItem> allType = System.IO.Directory.GetFiles(Server.MapPath("~/FusionChart/" + ent.CHARTS_TYPE + "/")).Where(x => x.Substring(x.LastIndexOf(".") + 1) == "swf").Select(x => new SelectListItem { Value = x.Substring(0, x.LastIndexOf(".")), Text = x.Substring(0, x.LastIndexOf(".")) }).ToList();
            allType = allType.Select(x => new SelectListItem { Text = x.Text.Substring(x.Text.LastIndexOf("\\") + 1), Value = x.Value.Substring(x.Value.LastIndexOf("\\") + 1) }).ToList();
            ViewData["AllType"] = allType;
            return View(ent);
        }


        public ActionResult Hcharts(string code, string authToken)
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                Fun.UserKey = authToken;
            }
            if (string.IsNullOrEmpty(Fun.UserKey))
            {
                return Redirect("~/Login/Login");
            }

            IList<ProInterface.Models.QueryPara> AllPara = new List<ProInterface.Models.QueryPara>();
            foreach (var t in Request.QueryString.Keys)
            {
                if (t != null)
                {
                    AllPara.Add(new ProInterface.Models.QueryPara() { ParaName = t.ToString(), Value = Request.QueryString[t.ToString()] });
                }
            }
            ViewBag.AllPara = ProInterface.JSON.DecodeToStr(AllPara);
            ViewBag.Code = code;

            ProServer.Service db = new ProServer.Service();
            var ent = db.QuerySingle(Fun.UserKey, ref Fun.Err, "x=>x.CODE==\"" + code + "\"");
            if (ent == null)
            {
                return Content("查询代码[" + code + "]不存在");
            }
            IList<SelectListItem> allType = System.IO.Directory.GetFiles(Server.MapPath("~/FusionChart/" + ent.CHARTS_TYPE + "/")).Where(x => x.Substring(x.LastIndexOf(".") + 1) == "swf").Select(x => new SelectListItem { Value = x.Substring(0, x.LastIndexOf(".")), Text = x.Substring(0, x.LastIndexOf(".")) }).ToList();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            string reSql="";
            var Ilist = db.QueryExecute(Fun.UserKey, ref error, code, 1, 100, null, null, AllPara.ToArray(), ref reSql);
            ViewData["DT"] = Ilist;
            return View(ent);
        }

    }
}
