using Newtonsoft.Json;
using ProInterface;
using ProInterface.Models;
using ProInterface.Models.Api;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.Open.OAuthAPIs;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Weixin;

namespace Web.Controllers
{
    public class WebApiController : Controller
    {
        protected ProServer.Api db = new ProServer.Api();



        public ActionResult CheckUpdate(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.CheckUpdate(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoginReg(ApiLogingBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiLogingBean>(inStr);

            ErrorInfo err = new ErrorInfo();
            var ent = db.LoginReg(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoginOut(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.LoginOut(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ClientLogin(ApiLogingBean inEnt, string inStr)
        {

            if (!string.IsNullOrEmpty(inEnt.openid) && string.IsNullOrEmpty(inEnt.loginName))
            {
                if (inEnt.openid.IndexOf("code|") == 0)
                {
                    string code = inEnt.openid.Replace("code|", "");
                    var codeArr = code.Split('|');

                    var openIdResult = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAccessToken(
                        WebConfigurationManager.AppSettings["WeixinAppId"], 
                        WebConfigurationManager.AppSettings["WeixinAppSecret"],
                        codeArr[0]);
                    inEnt.userId = codeArr[1];
                    inEnt.openid = openIdResult.openid;
                }
            }

            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiLogingBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.ClientLogin(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SalesmanLogin(ApiLogingBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inEnt.openid) && string.IsNullOrEmpty(inEnt.loginName))
            {
                if (inEnt.openid.IndexOf("code|") == 0)
                {
                    string code = inEnt.openid.Replace("code|", "");
                    var codeArr = code.Split('|');

                    var openIdResult = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAccessToken(
                        WebConfigurationManager.AppSettings["WeixinAppId"],
                        WebConfigurationManager.AppSettings["WeixinAppSecret"],
                        codeArr[0]);
                    inEnt.userId = codeArr[1];
                    inEnt.openid = openIdResult.openid;
                }
            }


            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiLogingBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.SalesmanLogin(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GarageUserLogin(ApiLogingBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiLogingBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.GarageUserLogin(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        [HttpPost]
        public ActionResult FileUp(int? userId, string authToken)
        {
            ErrorInfo err = new ErrorInfo();
            if (Request.Files.Count == 0)
            {
                return Json(new { code = 12006, message = "无文件内容" }, null, System.Text.Encoding.GetEncoding("gbk"));
            }
            string url = string.Format("~/File/Phone/{0}/", DateTime.Now.ToString("yyyyMMdd"));
            string path = Request.MapPath(url);
            string extName = "";
            if (Request.Files[0].FileName.LastIndexOf(".") > 0)
            {
                extName = Request.Files[0].FileName.Substring(Request.Files[0].FileName.LastIndexOf("."));
            }
            if (string.IsNullOrEmpty(extName))
            {
                extName = ".jpg";
            }
            var fileName = DateTime.Now.Ticks.ToString() + extName;
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            Request.Files[0].SaveAs(path + fileName);

            ProInterface.Models.FILES tmpFile = new ProInterface.Models.FILES();
            tmpFile.LENGTH = Request.Files[0].ContentLength;
            tmpFile.PATH = path + fileName;
            tmpFile.URL = url + fileName;
            tmpFile.UPLOAD_TIME = DateTime.Now;
            tmpFile.NAME = fileName;
            if (userId != null)
            {
                tmpFile.USER_ID = userId.Value;
            }
            ProInterface.IFiles db = new ProServer.Service();
            tmpFile = db.FilesAdd(authToken, ref err, tmpFile);
            if (tmpFile == null)
            {
                //return Json(err, null, System.Text.Encoding.GetEncoding("gbk"));
                return Json(err, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var reEnt = ProServer.Fun.ClassToCopy<ProInterface.Models.FILES, ApiFileBean>(tmpFile);
                return Content(JSON.DecodeToStr(reEnt));
                //return Json(reEnt, null, System.Text.Encoding.GetEncoding("gbk"));
            }
        }

        public ActionResult FileList(ApiRequesPageBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.FileList(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FileDel(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.FileDel(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SendCode(string loginKey,ApiKeyValueBean phone, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) phone = JsonConvert.DeserializeObject<ApiKeyValueBean>(inStr);
            if (Request["phone"] != null) phone = new ApiKeyValueBean { V = Request["phone"] };
            ErrorInfo err = new ErrorInfo();
            if (phone == null)
            {
                err.IsError = true;
                err.Message = "电话号码不能为空";
                return null;
            }
            var ent = db.SendCode(loginKey, ref err, phone.V);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ResetPassword(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.ResetPassword(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllDistrict()
        {
            ErrorInfo err = new ErrorInfo();
            var ent = db.GetAllDistrict(ref err);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SalesmanSingle(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.SalesmanSingle(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ClientSingle(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.ClientSingle(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GarageUserSingle(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.GarageUserSingle(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClientSave(ApiRequesSaveEntityBean<ApiClientBean> inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesSaveEntityBean<ApiClientBean>>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.ClientSave(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SalesmanSave(ApiRequesSaveEntityBean<ApiSalesmanBean> inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesSaveEntityBean<ApiSalesmanBean>>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.SalesmanSave(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserEditPwd(ApiRequesSaveEntityBean<string> inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesSaveEntityBean<string>>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.UserEditPwd(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClientSingleByPhone(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.ClientSingleByPhone(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SalesmanSingleByPhone(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.SalesmanSingleByPhone(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SalesmanJoinTeam(ApiRequesSaveEntityBean<string> inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesSaveEntityBean<string>>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.SalesmanJoinTeam(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ClientPeccancy(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.ClientPeccancy(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClientPeccancy1(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.ClientPeccancy1(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClientPeccancy2(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.ClientPeccancy2(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }



        public ActionResult CarSave(ApiRequesSaveEntityBean<ApiCarBean> inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesSaveEntityBean<ApiCarBean>>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.CarSave(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CarList(ApiRequesPageBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.CarList(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CarSetDefault(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.CarSetDefault(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CarDelete(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.CarDelete(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QueryCar(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.QueryCar(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult QueryInsure(ApiRequesSaveEntityBean<ApiCardBean> inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesSaveEntityBean<ApiCardBean>>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.QueryInsure(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QueryInsureProduct(ApiRequesPageBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.QueryInsureProduct(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }


        public ActionResult OrderInsureSave(ApiRequesSaveEntityBean<YlOrderInsure> inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesSaveEntityBean<YlOrderInsure>>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.OrderInsureSave(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrderInsureList(ApiRequesPageBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.OrderInsureList(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OrderRescueList(ApiRequesPageBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.OrderRescueList(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OrderInsureSingle(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.OrderInsureSingle(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GarageSingle(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.GarageSingle(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QueryInsurePrice(ApiRequesSaveEntityBean<ApiCardBean> inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesSaveEntityBean<ApiCardBean>>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.QueryInsurePrice(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RescueQuery(ApiRequesPageBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.RescueQuery(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RescueSave(ApiRequesSaveEntityBean<ApiOrderRescueBean> inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesSaveEntityBean<ApiOrderRescueBean>>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.RescueSave(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RescueSingle(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.RescueSingle(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }


        public ActionResult OrderSave(ApiRequesSaveEntityBean<YlOrder> inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesSaveEntityBean<YlOrder>>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.OrderSave(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrderSingle(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.OrderSingle(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrderList(ApiRequesPageBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.OrderList(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrderGrabList(ApiRequesPageBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.OrderGrabList(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrderGrab(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.OrderGrab(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrderSaveStatus(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.OrderSaveStatus(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OrderSaveVital(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.OrderSaveVital(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SalesmanClientAdd(ApiRequesSaveEntityBean<ApiClientBean> inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesSaveEntityBean<ApiClientBean>>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.SalesmanClientAdd(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SalesmanClientList(ApiRequesPageBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.SalesmanClientList(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SalesmanClientRestPwd(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.SalesmanClientRestPwd(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CardSave(ApiRequesSaveEntityBean<ApiCardBean> inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesSaveEntityBean<ApiCardBean>>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.CardSave(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CardList(ApiRequesPageBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.CardList(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CarSingle(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.CarSingle(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult WithdrawAdd(ApiRequesSaveEntityBean<ApiWithdrawBean> inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesSaveEntityBean<ApiWithdrawBean>>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.WithdrawAdd(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult WithdrawList(ApiRequesPageBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.WithdrawList(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CostList(ApiRequesPageBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.CostList(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MessageGetAll(ApiRequesPageBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.MessageGetAll(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MessageReply(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.MessageReply(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MessageSend(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.MessageSend(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MessageDelete(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.MessageDelete(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }



        public ActionResult AddressSave(ApiRequesSaveEntityBean<ApiAddressBean> inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesSaveEntityBean<ApiAddressBean>>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.AddressSave(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddressList(ApiRequesPageBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.AddressList(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddressSetDefault(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.AddressSetDefault(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddressDelete(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.AddressDelete(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddressSingle(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.AddressSingle(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PayAlipaySign(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var tmp = inEnt.para.SingleOrDefault(x => x.K == "orderInfo");
            if (tmp != null)
            {
                err.IsError = false;
                err.Message = Alipay.RSAFromPkcs8.sign(tmp.V, Alipay.Config.Private_key, Alipay.Config.Input_charset);
            }
            else
            {
                err.IsError = true;
            }
            return Json(err, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WeiXinJSSDKSign(ApiRequesEntityBean inEnt, string inStr)
        {

            string Token = ConfigurationManager.AppSettings["WeixinToken"];//与微信公众账号后台的Token设置保持一致，区分大小写。
            string EncodingAESKey = ConfigurationManager.AppSettings["WeixinEncodingAESKey"];//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
            string AppId = ConfigurationManager.AppSettings["WeixinAppId"];//与微信公众账号后台的AppId设置保持一致，区分大小写。
            string AppSecret = ConfigurationManager.AppSettings["WeixinAppSecret"];//与微信公众账号后台的AppId设置保持一致，区分大小写。
            ApiWeiXinJSSDKBean reEnt = new ApiWeiXinJSSDKBean();
            reEnt.appId = AppId;
            reEnt.nonceStr = Senparc.Weixin.MP.TenPayLib.TenPayUtil.GetNoncestr();
            reEnt.timestamp = Senparc.Weixin.MP.TenPayLib.TenPayUtil.GetTimestamp();
            string JsTicket = JsApiTicketContainer.TryGetJsApiTicket(AppId, AppSecret);

            string signature = "";
            string url = inEnt.authToken;//这里是当前页面的地址
            //url = HttpUtility.UrlDecode(url);
            Senparc.Weixin.MP.TenPayLib.RequestHandler paySignReqHandler = new Senparc.Weixin.MP.TenPayLib.RequestHandler(null);

            paySignReqHandler.SetParameter("jsapi_ticket", JsTicket);
            paySignReqHandler.SetParameter("noncestr", reEnt.nonceStr);
            paySignReqHandler.SetParameter("timestamp", reEnt.timestamp);
            paySignReqHandler.SetParameter("url", url);
            signature = paySignReqHandler.CreateSHA1Sign();
            reEnt.signature = signature;

            return Json(reEnt, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WeiXinFileUp(ApiRequesEntityBean inEnt, string inStr)
        {
            ErrorInfo err = new ErrorInfo();

            string AppId = ConfigurationManager.AppSettings["WeixinAppId"];//与微信公众账号后台的AppId设置保持一致，区分大小写。
            string AppSecret = ConfigurationManager.AppSettings["WeixinAppSecret"];//与微信公众账号后台的AppId设置保持一致，区分大小写。
            var accessToken = AccessTokenContainer.TryGetAccessToken(AppId, AppSecret);
            string mediaId = "";
            var tmpKey = inEnt.para.SingleOrDefault(x => x.K == "mediaId");
            if (tmpKey == null)
            {
                err.IsError = true;
                err.Message = "参数有误";
                return Json(err, JsonRequestBehavior.AllowGet);
            }
            mediaId = tmpKey.V;
            MemoryStream ms = new MemoryStream();
            Senparc.Weixin.MP.AdvancedAPIs.MediaApi.Get(accessToken, mediaId, ms);
            ms.Seek(0, SeekOrigin.Begin);

            


            string url = string.Format("~/File/Phone/{0}/", DateTime.Now.ToString("yyyyMMdd"));
            string path = Request.MapPath(url);
            string extName = ".jpg";
            var fileName = DateTime.Now.Ticks.ToString() + extName;
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            ProInterface.Models.FILES tmpFile = new ProInterface.Models.FILES();
            tmpFile.LENGTH = Convert.ToInt32(ms.Length);

            FileStream fs = new FileStream(path + fileName, FileMode.OpenOrCreate);
            BinaryWriter w = new BinaryWriter(fs);
            w.Write(ms.ToArray());
            fs.Close();
            ms.Close();

            tmpFile.PATH = path + fileName;
            tmpFile.URL = url + fileName;
            tmpFile.UPLOAD_TIME = DateTime.Now;
            tmpFile.NAME = fileName;

            ProServer.GlobalUser gu = ProServer.Global.GetUser(inEnt.authToken);
            if (gu != null)
            {
                tmpFile.USER_ID = gu.UserId;
            }
            ProInterface.IFiles db = new ProServer.Service();
            tmpFile = db.FilesAdd(inEnt.authToken, ref err, tmpFile);
            if (tmpFile == null)
            {
                return Json(err, null, System.Text.Encoding.GetEncoding("gbk"));
            }
            else
            {
                var reEnt = ProServer.Fun.ClassToCopy<ProInterface.Models.FILES, ApiFileBean>(tmpFile);
                return Json(reEnt, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult PayWeixinSign(ApiRequesEntityBean inEnt)
        {
            ErrorInfo err = new ErrorInfo();
            string order_no = "";
            var tmp = inEnt.para.FirstOrDefault(x => x.K == "order_no");
            if (tmp != null)
            {
                order_no = tmp.V;
            }
            try
            {
                ProServer.Service ser= new ProServer.Service();

                var nowOrderFlow = ser.OrderFlowGetWaitPay(inEnt.authToken, ref err,Convert.ToInt32(order_no));
                if (err.IsError)
                {
                    return Json(err, JsonRequestBehavior.AllowGet);
                }

                var codeMsg = db.WeixinGetOpenid(ref err, inEnt);
                if (!codeMsg.IsError && !string.IsNullOrEmpty(err.Message))
                {
                    var ent = new WeixinController().JSPay(codeMsg.Message, 
                        nowOrderFlow.ORDER_FLOW_NO, 
                        Convert.ToInt32(nowOrderFlow.COST*100),
                        string.Format("[{0}]:{1}", nowOrderFlow.SUBJECT, nowOrderFlow.BODY));
                    return Json(ent, JsonRequestBehavior.AllowGet);
                }
                else {
                    err.IsError = true;
                    err.Message = "获取微信用户失败";
                    return Json(err, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                err.IsError = true;
                err.Message = e.Message;
                return Json(err, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult WeixinGetUser(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.WeixinGetUser(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 发送消息给用户
        /// </summary>
        /// <param name="inEnt"></param>
        /// <param name="inStr"></param>
        /// <returns></returns>
        public ActionResult WeixinSendMsg(ApiRequesEntityBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesEntityBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.WeixinSendMsg(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TeamMyAll(ApiRequesPageBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.TeamMyAll(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TeamSingle(ApiRequesPageBean inEnt, string inStr)
        {
            if (!string.IsNullOrEmpty(inStr)) inEnt = JsonConvert.DeserializeObject<ApiRequesPageBean>(inStr);
            ErrorInfo err = new ErrorInfo();
            var ent = db.TeamSingle(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BbsList(ApiRequesPageBean inEnt, string inStr)
        {
            ErrorInfo err = new ErrorInfo();
            var ent = db.BbsList(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BbsSingle(ApiRequesEntityBean inEnt, string inStr)
        {
            ErrorInfo err = new ErrorInfo();
            var ent = db.BbsSingle(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BbsGood(ApiRequesEntityBean inEnt, string inStr)
        {
            ErrorInfo err = new ErrorInfo();
            var ent = db.BbsGood(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BbsDelete(ApiRequesEntityBean inEnt, string inStr)
        {
            ErrorInfo err = new ErrorInfo();
            var ent = db.BbsDelete(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BbsSave(ApiRequesSaveEntityBean<Ylbbs> inEnt, string inStr)
        {
            ErrorInfo err = new ErrorInfo();
            var ent = db.BbsSave(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MessageTypeAll(ApiRequesEntityBean inEnt, string inStr)
        {
            ErrorInfo err = new ErrorInfo();
            var ent = db.MessageTypeAll(ref err, inEnt);
            if (err.IsError) { return Json(err, JsonRequestBehavior.AllowGet); }
            return Json(ent, JsonRequestBehavior.AllowGet);
        }

    }
}
