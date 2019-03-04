
using ProInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProServer;
using ProInterface.Models.Api;
using System.Text;

namespace Web.Controllers
{
    public class BulletinController : Controller
    {

        public ActionResult DownContent(string Title, int? id)
        {
            ProInterface.ErrorInfo error = new ErrorInfo();
            ProInterface.IBulletin ems = new ProServer.Service();
            var _Ent = new ProInterface.Models.TBulletin();
            if (id != null)
            {
                _Ent = ems.BulletinSingle(Fun.UserKey, ref error, id.Value);
            }
            else
            {
                _Ent = ems.BulletinSingleByTitle(Fun.UserKey, ref error, Title);
            }
            //System.IO.File.WriteAllText(Server.MapPath(string.Format("~/File/Bulletin_{0}.html", _Ent.ID)), _Ent.CONTENT,Encoding.UTF8);
            //ProServer.WordOperate.HtmlToWord(Server.MapPath(string.Format("~/File/Bulletin_{0}.html", _Ent.ID)), Server.MapPath(string.Format("~/File/Bulletin_{0}.doc", _Ent.ID)));
            //byte[] tmp = System.IO.File.ReadAllBytes(Server.MapPath(string.Format("~/File/Bulletin_{0}.doc", _Ent.ID)));

            byte[] tmp = new byte[5];
            tmp = Encoding.UTF8.GetBytes(_Ent.CONTENT);
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Response.ContentType = "application/vnd.ms-word";

            Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(_Ent.TITLE+".doc"));
            Response.BinaryWrite(tmp);
            Response.Flush();
            Response.End();
            return new EmptyResult();

        }
        public ActionResult More()
        {
            ProInterface.Models.TBulletin _Ent = new ProInterface.Models.TBulletin();
            ProInterface.ErrorInfo error = new ErrorInfo();
            ProServer.Service db = new ProServer.Service();
            ViewData["TYPE_CODE"] = db.BulletinType();
            return View();
        }
        public ActionResult QueryList()
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            int pageIndex = Convert.ToInt32(Request["page"]);
            //pageIndex = pageIndex + 1;
            int pageSize = Convert.ToInt32(Request["rows"]);
            string sortField = Request["sort"];
            string sortOrder = Request["order"];
            IList<ProInterface.Models.QueryPara> AllPara = ProInterface.JSON.EncodeToEntity<List<ProInterface.Models.QueryPara>>(Request["AllParaStr"]);
            if (AllPara == null) AllPara = new List<ProInterface.Models.QueryPara>();
            //string code = Request["Code"];
            //if (code == null)
            //{
            //    return Json("[1]");
            //}
            string WhereStr = Request["WhereStr"];
            string TypeCode = Request["TypeCode"];
            string PublishTimeSrart = Request["PublishTimeStart"];
            string PublishTimeEnd = Request["PublishTimeEnd"];
            string Publisher = Request["Publisher"];
            string Title = Request["Title"];
            ProInterface.IQuery ems = new ProServer.Service();

            ProInterface.IBulletin ser = new ProServer.Service();
            ErrorInfo apiError = new ErrorInfo();
            var bulletList = ser.BulletinList(ref apiError, new ApiRequesPageBean
            {
                authToken = Fun.UserKey,
                currentPage = pageIndex,
                pageSize = pageSize,
                searchKey = new[] {
                    new ApiKeyValueBean { K = "TYPE_CODE", V = TypeCode, T = "==" },
                    new ApiKeyValueBean { K = "CREATE_TIME", V = PublishTimeSrart, T = ">=" },
                    new ApiKeyValueBean { K = "CREATE_TIME", V = PublishTimeEnd, T = "<=" },
                    new ApiKeyValueBean { K = "PUBLISHER", V = Publisher, T = "like" },
                    new ApiKeyValueBean { K = "TITLE", V = Title, T = "like" } }
            });
            var t1 = JSON.DecodeToStr(bulletList.totalCount, bulletList.data);
            return Content(t1);
        }

        public ActionResult LookByTitle(string Title, int? id)
        {
            ProInterface.ErrorInfo error = new ErrorInfo();
            ProInterface.IBulletin ems = new ProServer.Service();
            var _Ent = new ProInterface.Models.TBulletin();
            if (id != null)
            {
                _Ent = ems.BulletinSingle(Fun.UserKey, ref error, id.Value);
            }
            else
            {
                _Ent = ems.BulletinSingleByTitle(Fun.UserKey, ref error, Title);
            }
            return View(_Ent);
        }

        public ActionResult EditByTitle(string Title)
        {
            ProInterface.IBulletin ems = new ProServer.Service();

            ProInterface.ErrorInfo error = new ErrorInfo();
            var _Ent = new ProInterface.Models.TBulletin();
            _Ent = ems.BulletinSingleByTitle(Fun.UserKey, ref error, Title);
            return View(_Ent);
        }

        public ActionResult Details(int? id, string authToken)
        {
            string userKey = Fun.UserKey;
            if (!string.IsNullOrEmpty(authToken))
            {
                Fun.UserKey = authToken;
                userKey = authToken;
            }

            ProInterface.Models.TBulletin _Ent = new ProInterface.Models.TBulletin();
            ProInterface.ErrorInfo error = new ErrorInfo();
            ProServer.Service db = new ProServer.Service();
            _Ent = db.BulletinSingle(userKey, ref error, id);
            if (_Ent != null) {
                ViewData["TYPE_CODE"] = db.BulletinType();
                ViewData["AllRole"] = db.Role_Where(userKey, ref error, 1, 100, null, "ID", null).Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.NAME }).ToList();
                ViewData["AllRole1"] = "ghkjkj";
            }
            return View(_Ent);
        }

        public ActionResult Look(int id, string authToken)
        {
            string userKey = Fun.UserKey;
            if (!string.IsNullOrEmpty(authToken))
            {
                Fun.UserKey = authToken;
                userKey = authToken;
            }

            ProInterface.ErrorInfo error = new ErrorInfo();
            ProInterface.IBulletin ems = new ProServer.Service();
            var _Ent = ems.BulletinSingle(userKey, ref error, id);
            return View(_Ent);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Details(ProInterface.Models.TBulletin ent)
        {
            ProInterface.IBulletin db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            db.BulletinSave(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());
            if (error.IsError)
            {
                return Json(error);
            }
            else
            {
                error.Message = "保存成功";
                return Json(error);
            }
        }

        public ActionResult AddFile(string name, int size, string savePath)
        {
            ProInterface.ErrorInfo error = new ErrorInfo();
            ProInterface.IFiles db = new ProServer.Service();
            ProInterface.Models.FILES ent = new ProInterface.Models.FILES();
            ent.LENGTH = size;
            ent.FILE_TYPE = name.Substring(name.LastIndexOf(".") + 1);
            if (string.IsNullOrEmpty(name))
            {
                ent.NAME = savePath.Substring(savePath.LastIndexOf("/"));
            }
            else {
                ent.NAME = name;
            }
            ent.PATH = savePath;
            ent.REMARK = "公告附件";
            ent.UPLOAD_TIME = DateTime.Now;
            ent.URL = savePath;
            ent = db.FilesAdd(Fun.UserKey, ref error, ent);
            string reStr = JSON.DecodeToStr(ent);
            return Content(reStr);
        }

        public bool DelFile(int fileID)
        {
            ProInterface.ErrorInfo error = new ErrorInfo();
            ProInterface.IFiles db = new ProServer.Service();
            Fun.Err = new ErrorInfo();
            var ent = db.FilesSingle(Fun.UserKey, ref error, fileID);
            if(ent!=null){
            if (db.Files_Delete(Fun.UserKey, ref error, fileID))
            {
                try
                {
                    System.IO.File.Delete(Server.MapPath(ent.URL));
                }
                catch { }
                return true;
            }
            }
            return false;
        }

        public ActionResult Delete(string id)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            if (id != null)
            {
                int fail = 0, succ = 0;
                string[] idArr = id.Split(',');
                ProInterface.IBulletin db = new ProServer.Service();
                for (int i = 0; i < idArr.Count(); i++)
                {
                    int _t = 0;
                    try
                    {
                        _t = Convert.ToInt32(idArr[i]);
                        if (db.BulletinDelete(Fun.UserKey, ref error, _t))
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
            return Json(error, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 置顶
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string SetTop(string id)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ProInterface.IBulletin ems = new ProServer.Service();
            int myid = Convert.ToInt32(id);

            bool bl = ems.SetTop(Fun.UserKey, ref error, myid, true);
            return bl.ToString();
        }
        /// <summary>
        ///  取消置顶
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string SetTopCancel(string id)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ProInterface.IBulletin ems = new ProServer.Service();
            int myid = Convert.ToInt32(id);
            bool bl = ems.SetTop(Fun.UserKey, ref error, myid, false);
            return bl.ToString();
        }
        /// <summary>
        /// 重要
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string SetImport(string id)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ProInterface.IBulletin ems = new ProServer.Service();
            int myid = Convert.ToInt32(id);
            bool bl = ems.SetImport(Fun.UserKey, ref error, myid, true);
            return bl.ToString();
        }
        /// <summary>
        /// 取消重要
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string SetImportCancel(string id)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            ProInterface.IBulletin ems = new ProServer.Service();
            int myid = Convert.ToInt32(id);
            bool bl = ems.SetImport(Fun.UserKey, ref error, myid, false);
            return bl.ToString();
        }
    }
}
