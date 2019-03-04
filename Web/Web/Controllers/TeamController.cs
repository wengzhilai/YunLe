

using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Containers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;

namespace Web.Controllers
{
    public class TeamController : Controller
    {

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="type">0用户，1业务员</param>
        /// <param name="w"></param>
        /// <returns></returns>
        public ActionResult Matrix(string pollCode, string type)
        {
            int w = 300;
            var url = string.Format("http://www.woolawyer.com/staffCar/index.html#/register/{0}", pollCode);
            IDictionary<EncodeHintType, object> hint = new Dictionary<EncodeHintType, object>();
            hint.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            hint.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
            BitMatrix bitMatrix;
            bitMatrix = new MultiFormatWriter().encode(url, BarcodeFormat.QR_CODE, w, w, hint);
            ZXing.BarcodeWriter bw = new ZXing.BarcodeWriter();

            //构造二维码写码器
            MultiFormatWriter mutiWriter = new MultiFormatWriter();
            //生成二维码
            Bitmap bitmap = bw.Write(bitMatrix);

            //要插入到二维码中的图片
            Image middlImg = Image.FromFile(Server.MapPath("~/Content/images/yunle.png"));

            //获取二维码实际尺寸（去掉二维码两边空白后的实际尺寸）
            //计算插入图片的大小和位置
            int middleImgW = Math.Min((int)(200 / 3.5), middlImg.Width);
            int middleImgH = Math.Min((int)(200 / 3.5), middlImg.Height);
            int middleImgL = (300 - middleImgW) / 2;
            int middleImgT = (300 - middleImgH) / 2;

            //将img转换成bmp格式，否则后面无法创建 Graphics对象
            Bitmap bmpimg = new Bitmap(300, 300, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmpimg))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.DrawImage(bitmap, 0, 0);
            }

            //在二维码中插入图片
            System.Drawing.Graphics MyGraphic = System.Drawing.Graphics.FromImage(bmpimg);
            //白底
            MyGraphic.FillRectangle(Brushes.White, middleImgL, middleImgT, middleImgW, middleImgH);
            MyGraphic.DrawImage(middlImg, middleImgL, middleImgT, middleImgW, middleImgH);

            var ms = new MemoryStream();
            bmpimg.Save(ms, ImageFormat.Png);
            ms.WriteTo(Response.OutputStream);
            Response.ContentType = "image/png";
            return null;
        }

        public ActionResult MakeWeiXinMatrix(int teamId)
        {
            if (!AccessTokenContainer.CheckRegistered(ProInterface.AppSet.AppId))
            {
                AccessTokenContainer.Register(ProInterface.AppSet.AppId, ProInterface.AppSet.AppSecret);
            }

            ProInterface.IZ_Team db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            var _Ent = db.Team_SingleId(Fun.UserKey, ref error, teamId);
            if (_Ent != null)
            {
                var result = AccessTokenContainer.GetAccessTokenResult(ProInterface.AppSet.AppId); //CommonAPIs.CommonApi.GetToken(appId, appSecret);
                var reCoe = Senparc.Weixin.MP.AdvancedAPIs.QrCodeApi.Create(
                    result.access_token,
                    0,
                    teamId,
                    QrCode_ActionName.QR_LIMIT_STR_SCENE, "team_" + _Ent.REQUEST_CODE);
                var url = Senparc.Weixin.MP.AdvancedAPIs.QrCodeApi.GetShowQrCodeUrl(reCoe.ticket);

                ProServer.Fun.DownLoadSoft(Server.MapPath("~/File/QrCode/"), url, string.Format("team_weixin_{0}.jpg", teamId));

                url = "http://www.27580sc.com/YL/Team/Matrix?pollCode=" + _Ent.REQUEST_CODE;
                ProServer.Fun.DownLoadSoft(Server.MapPath("~/File/QrCode/"), url, string.Format("team_{0}.jpg", teamId));
            }

            if (error.IsError)
            {
                return Json(error);
            }
            else
            {
                error.Message = "生成成功";
                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetProperDescription(string proName)
        {
            return Json(ProServer.Fun.GetClassProperDescription<ProInterface.Models.YL_TEAM>(proName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int? id)
        {
            ProServer.Service db = new ProServer.Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            if (id != null)
            {
                ViewData["LEAD_ID_STR"] = db.UserGetBylRole(Fun.UserKey, ref error, "4").Select(x=>new SelectListItem { Text=x.name, Value=x.id }).ToList();
                var _Ent = db.Team_SingleId(Fun.UserKey, ref error, id.Value);
                return View(_Ent);
            }
            return View();
        }


        [HttpPost]
        public ActionResult Details(ProInterface.Models.YL_TEAM ent)
        {

            ProInterface.ITeam db = new ProServer.Service();
            ProInterface.ErrorInfo error=new ProInterface.ErrorInfo();
            db.TeamSave(Fun.UserKey, ref error, ent, Request.Form.AllKeys.ToList());
            if (error.IsError)
            {
                return Json(error);
            }
            else
            {
                MakeWeiXinMatrix(Convert.ToInt32(error.Message));
                error.Message = "保存成功";
                return Json(error);
            }
        }

        public ActionResult Delete(string id)
        {
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            if (id != null)
            {
                int fail = 0, succ = 0;
                string[] idArr = id.Split(',');
                ProInterface.ITeam db = new ProServer.Service();
                for (int i = 0; i < idArr.Count(); i++)
                {
                    int _t = 0;
                    try
                    {
                        _t = Convert.ToInt32(idArr[i]);
                        if (db.TeamDelete(Fun.UserKey, ref error, _t))
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
                error.Message= "删除成功[" + succ + "]个\\r\\n删除失败[" + fail + "]个";
            }
            else {
                error.Message = "删除失败";
            }
            return Json(error,JsonRequestBehavior.AllowGet);
        }

    }
}
