using ProInterface;
using ProInterface.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class FunController : Controller
    {


        /// <summary>
        /// 上传文件,返回文件名
        /// </summary>
        /// <param name="nameType">0表示随机文件名，1表示使用原文件名</param>
        /// <param name="fileType">文件类型:image、flash、media、file、*、扩展名</param>
        /// <returns>返回文件名</returns>
        [HttpPost]
        public ActionResult FileUp(string dirPath,string nameType ,string fileType)
        {
            if (string.IsNullOrEmpty(dirPath))
            {
                dirPath = "~/UpFiles/Tmp/";
            }
            if (string.IsNullOrEmpty(nameType))
            {
                nameType = "0";
            }
            if (string.IsNullOrEmpty(fileType))
            {
                fileType = "image";
            }
            fileType = fileType.ToLower();

            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            //定义允许上传的文件扩展名
            Dictionary<string, string> extTable = new Dictionary<string, string>();
            extTable.Add("image", "gif,jpg,jpeg,png,bmp");
            extTable.Add("flash", "swf,flv");
            extTable.Add("media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb");
            extTable.Add("file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2,apk,app");

            //文件名称
            if (Request.Files.Count == 0)
            {
                error.IsError = true;
                error.Message = "上传失败，请重新选择文件，建议使用google浏览器";
                return Json(error, "text/html", System.Text.Encoding.GetEncoding("gbk"));
            }
            var fileName = Request.Files[0].FileName;
            if (fileName.LastIndexOf(".") == -1)
            {
                error.IsError = true;
                error.Message = "上传失败，文件无扩展名";
                return Json(error, "text/html", System.Text.Encoding.GetEncoding("gbk"));
            }
            string fileExtName = fileName.Substring(fileName.LastIndexOf(".")+1).ToLower();
            if (extTable.ContainsKey(fileType))
            {
                if (!extTable[fileType].Split(',').Contains(fileExtName))
                {
                    error.IsError = true;
                    error.Message = "上传失败，扩展名非法";
                    return Json(error, "text/html", System.Text.Encoding.GetEncoding("gbk"));
                }
            }
            else if (fileType=="*")
            {
                var allValue = string.Join(",", extTable.Values.ToList()).Split(',');
                if (!allValue.Contains(fileExtName))
                {
                    error.IsError = true;
                    error.Message = "上传失败，扩展名非法";
                    return Json(error, "text/html", System.Text.Encoding.GetEncoding("gbk"));
                }
            }
            else if (!string.IsNullOrEmpty(fileType))
            {
                var allValue = fileType.Split(',');
                if (!allValue.Contains(fileExtName))
                {
                    error.IsError = true;
                    error.Message = "上传失败，上传的文件类型不支持";
                    return Json(error, "text/html", System.Text.Encoding.GetEncoding("gbk"));
                }
            }
            else
            {
                error.IsError = true;
                error.Message = "上传失败，上传的文件类型不支持";
                return Json(error, "text/html", System.Text.Encoding.GetEncoding("gbk"));
            }


            if (Request.Files.Count == 0)
            {
                error.IsError = true;
                error.Message = "无文件内容";
                return Json(error, "text/html", System.Text.Encoding.GetEncoding("gbk"));
            }
            string path = Request.MapPath(dirPath);
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            if (nameType == "0")
            {
                fileName = DateTime.Now.Ticks.ToString() + "." + fileExtName;
            }
            try
            {
                if (System.IO.File.Exists(path + fileName))
                {
                    System.IO.File.Delete(path + fileName);
                }
                Request.Files[0].SaveAs(path + fileName);
            }
            catch { }
            error.Message = dirPath + fileName;
            error.Params = Request.Files[0].ContentLength + "|" + Request.Files[0].FileName;
            return Json(error, "text/html", System.Text.Encoding.GetEncoding("gbk"));
        }

        public ActionResult AddFile(string name, int size, string savePath,string remark)
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
            else
            {
                ent.NAME = name;
            }
            ent.PATH = savePath;
            ent.REMARK = remark;
            ent.UPLOAD_TIME = DateTime.Now;
            ent.URL = savePath;
            ent = db.FilesAdd(Fun.UserKey, ref error, ent);
            return Json(ent,JsonRequestBehavior.AllowGet);
        }

        public bool DelFile(int fileID)
        {
            ProInterface.ErrorInfo error = new ErrorInfo();
            ProInterface.IFiles db = new ProServer.Service();
            Fun.Err = new ErrorInfo();
            var ent = db.FilesSingle(Fun.UserKey, ref error, fileID);
            if (ent != null)
            {
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

        public ActionResult GetFile(int fileId)
        {
            ProInterface.ErrorInfo error = new ErrorInfo();
            ProInterface.IFiles db = new ProServer.Service();
            ProInterface.Models.FILES ent = db.FilesSingle(Fun.UserKey, ref error, fileId);
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowFile(int fileId)
        {
            ProInterface.ErrorInfo error = new ErrorInfo();
            ProInterface.IFiles db = new ProServer.Service();
            ProInterface.Models.FILES ent = db.FilesSingle(Fun.UserKey, ref error, fileId);
            return View(ent);
        }

        public ActionResult ReplaceDataTime(string content, DateTime nowDt)
        {
            string str = ProServer.Fun.ReplaceDataTime(content, nowDt,Fun.UserKey);
            return Content(str);
        }

        public ActionResult ExcelToJson(string urlPath)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            string path = Request.MapPath(urlPath);
            DataTable dt = ProServer.ExcelHelper.ImportExcel2007toDt(path);
            string reStr = JSON.DecodeToStr(dt);
            return Content(reStr);
        }

        /// <summary>
        /// 获取用户可用的层级
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDistrictType()
        {
            IList<ProInterface.Models.IdText> reList = new List<ProInterface.Models.IdText>();
            reList.Add(new IdText { id = "2", text = "区县" });
            reList.Add(new IdText { id = "3", text = ProInterface.AppSet.RegionName });
            reList.Add(new IdText { id = "4", text = ProInterface.AppSet.ThorpeName });

            ProInterface.IDistrict db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            var dis= db.DistrictGetByUser(Fun.UserKey, ref error);
            reList = reList.Where(x => Convert.ToInt32(x.id) >= dis.LEVEL_ID).ToList();
            if (reList.Count()>0)
            {
                reList[0].selected = true;
            }
            if (reList.Count() == 0)
            {
                reList.Add(new IdText { id = "4", text = ProInterface.AppSet.ThorpeName });
            }
            return Json(reList);
        }

        public ActionResult GetDistrictType1()
        {
            IList<ProInterface.Models.IdText> reList = new List<ProInterface.Models.IdText>();
            reList.Add(new IdText { id = "2", text = "区县" });
            reList.Add(new IdText { id = "3", text = ProInterface.AppSet.RegionName  });
            reList.Add(new IdText { id = "4", text = ProInterface.AppSet.ThorpeName });
            reList.Add(new IdText { id = "5", text = "渠道" });

            ProInterface.IDistrict db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            var dis = db.DistrictGetByUser(Fun.UserKey, ref error);
            reList = reList.Where(x => Convert.ToInt32(x.id) >= dis.LEVEL_ID).ToList();
            if (reList.Count() > 0)
            {
                reList[0].selected = true;
            }
            if (reList.Count() == 0)
            {
                reList.Add(new IdText { id = "4", text = ProInterface.AppSet.ThorpeName });
            }
            return Json(reList);
        }


        public ActionResult GetDistrictType2()
        {
            IList<ProInterface.Models.IdText> reList = new List<ProInterface.Models.IdText>();
            reList.Add(new IdText { id = "3", text = ProInterface.AppSet.RegionName  });
            reList.Add(new IdText { id = "4", text = ProInterface.AppSet.ThorpeName });
            reList.Add(new IdText { id = "5", text = "渠道" });

            ProInterface.IDistrict db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            var dis = db.DistrictGetByUser(Fun.UserKey, ref error);
            reList = reList.Where(x => Convert.ToInt32(x.id) >= dis.LEVEL_ID).ToList();
            if (reList.Count() > 0)
            {
                reList[0].selected = true;
            }
            if (reList.Count() == 0)
            {
                reList.Add(new IdText { id = "4", text = ProInterface.AppSet.ThorpeName });
            }
            return Json(reList);
        }
    }
}
