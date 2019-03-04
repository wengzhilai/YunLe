/*----------------------------------------------------------------
    Copyright (C) 2015 Senparc
    
    文件名：WeixinController.cs
    文件功能描述：用于处理微信回调的信息
    
    
    创建标识：Senparc - 20150312
----------------------------------------------------------------*/

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.MvcExtension;
using Web.Weixin.MessageHandlers.CustomMessageHandler;
using System.Configuration;
using System.Web;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.TenPayLibV3;
using System.Xml.Linq;
using ProInterface.Models.Api;
using System.Collections.Generic;
using ProInterface.Models;
using ZXing.Common;
using ZXing;
using System.Drawing.Imaging;

namespace Web.Controllers
{
    public partial class WeixinController : Controller
    {
        public static readonly string Token = WebConfigurationManager.AppSettings["WeixinToken"];//与微信公众账号后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = WebConfigurationManager.AppSettings["WeixinEncodingAESKey"];//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string AppId = WebConfigurationManager.AppSettings["WeixinAppId"];//与微信公众账号后台的AppId设置保持一致，区分大小写。
        public static readonly string AppSecret = WebConfigurationManager.AppSettings["WeixinAppSecret"];//与微信公众账号后台的AppId设置保持一致，区分大小写。
        public static readonly string MchId = WebConfigurationManager.AppSettings["TenPayV3_MchId"];//
        public static readonly string appkey = WebConfigurationManager.AppSettings["TenPayV3_Key"];//

        public static readonly string NotifyUrl = "http://www.woolawyer.com/YL/Weixin/PayNotifyUrl";

        public WeixinController()
        {

        }

        public ActionResult Down(int type = 1)
        {
            string iosUrl = "", androidUrl = "", title="" ;
            switch (type)
            {
                case 1://表示用户
                    title = "云乐享车";
                    iosUrl = "https://itunes.apple.com/us/app/yun-le-xiang-che-yong-hu-duan/id1214196774?l=zh&ls=1&mt=8";
                    androidUrl = "https://www.pgyer.com/KA8V";
                    break;
                case 2://表示是服务商
                    title = "云乐享车-服务商";
                    iosUrl = "https://itunes.apple.com/us/app/yun-le-xiang-che-fu-wu-shang/id1214203114?l=zh&ls=1&mt=8";
                    androidUrl = "https://www.pgyer.com/F1qK";
                    break;
            }
            ViewData["title"] = title;
            ViewData["iosUrl"] = iosUrl;
            ViewData["androidUrl"] = androidUrl;
            return View();
        }

        public ActionResult Matrix(string type, int w)
        {
            var url = string.Format("http://{0}{1}/Weixin/Down?type={2}", Request.Url.Host, Request.ApplicationPath, type);
            BitMatrix bitMatrix;
            bitMatrix = new MultiFormatWriter().encode(url, BarcodeFormat.QR_CODE, w, w);
            ZXing.BarcodeWriter bw = new ZXing.BarcodeWriter();
            var ms = new MemoryStream();
            var bitmap = bw.Write(bitMatrix);
            bitmap.Save(ms, ImageFormat.Png);
            //return File(ms, "image/png");
            ms.WriteTo(Response.OutputStream);
            Response.ContentType = "image/png";
            return null;
        }

        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：http://weixin.senparc.com/weixin
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + Senparc.Weixin.MP.CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, Token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }

        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// PS：此方法为简化方法，效果与OldPost一致。
        /// v0.8之后的版本可以结合Senparc.Weixin.MP.MvcExtension扩展包，使用WeixinResult，见MiniPost方法。
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(PostModel postModel)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content("参数错误！");
            }

            postModel.Token = Token;
            postModel.EncodingAESKey = EncodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = AppId;//根据自己后台的设置保持一致

            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
            var maxRecordCount = 10;

            var logPath = Server.MapPath(string.Format("~/App_Data/MP/{0}/", DateTime.Now.ToString("yyyy-MM-dd")));
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new Web.Weixin.MessageHandlers.CustomMessageHandler.CustomMessageHandler(Request.InputStream, postModel, maxRecordCount);
            try
            {

                //测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。
                messageHandler.RequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request_{1}.txt", DateTime.Now.Ticks, messageHandler.RequestMessage.FromUserName)));
                if (messageHandler.UsingEcryptMessage)
                {
                    messageHandler.EcryptRequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request_Ecrypt_{1}.txt", DateTime.Now.Ticks, messageHandler.RequestMessage.FromUserName)));
                }

                /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
                 * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/
                messageHandler.OmitRepeatedMessage = true;


                //执行微信处理过程
                messageHandler.Execute();

                //测试时可开启，帮助跟踪数据

                //if (messageHandler.ResponseDocument == null)
                //{
                //    throw new Exception(messageHandler.RequestDocument.ToString());
                //}

                if (messageHandler.ResponseDocument != null)
                {
                    messageHandler.ResponseDocument.Save(Path.Combine(logPath, string.Format("{0}_Response_{1}.txt", DateTime.Now.Ticks, messageHandler.RequestMessage.FromUserName)));
                }

                if (messageHandler.UsingEcryptMessage)
                {
                    //记录加密后的响应信息
                    messageHandler.FinalResponseDocument.Save(Path.Combine(logPath, string.Format("{0}_Response_Final_{1}.txt", DateTime.Now.Ticks, messageHandler.RequestMessage.FromUserName)));
                }

                //return Content(messageHandler.ResponseDocument.ToString());//v0.7-
                return new FixWeixinBugWeixinResult(messageHandler);//为了解决官方微信5.0软件换行bug暂时添加的方法，平时用下面一个方法即可
                //return new WeixinResult(messageHandler);//v0.8+
            }
            catch (Exception ex)
            {
                using (TextWriter tw = new StreamWriter(Server.MapPath("~/App_Data/Error_" + DateTime.Now.Ticks + ".txt")))
                {
                    tw.WriteLine("ExecptionMessage:" + ex.Message);
                    tw.WriteLine(ex.Source);
                    tw.WriteLine(ex.StackTrace);
                    //tw.WriteLine("InnerExecptionMessage:" + ex.InnerException.Message);

                    if (messageHandler.ResponseDocument != null)
                    {
                        tw.WriteLine(messageHandler.ResponseDocument.ToString());
                    }

                    if (ex.InnerException != null)
                    {
                        tw.WriteLine("========= InnerException =========");
                        tw.WriteLine(ex.InnerException.Message);
                        tw.WriteLine(ex.InnerException.Source);
                        tw.WriteLine(ex.InnerException.StackTrace);
                    }

                    tw.Flush();
                    tw.Close();
                }
                return Content("");
            }
        }


        /// <summary>
        /// 最简化的处理流程（不加密）
        /// </summary>
        [HttpPost]
        [ActionName("MiniPost")]
        public ActionResult MiniPost(PostModel postModel)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                //return Content("参数错误！");//v0.7-
                return new WeixinResult("参数错误！");//v0.8+
            }

            postModel.Token = Token;
            postModel.EncodingAESKey = EncodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = AppId;//根据自己后台的设置保持一致

            var messageHandler = new CustomMessageHandler(Request.InputStream, postModel, 10);

            messageHandler.Execute();//执行微信处理过程

            //return Content(messageHandler.ResponseDocument.ToString());//v0.7-
            return new FixWeixinBugWeixinResult(messageHandler);//v0.8+
            return new WeixinResult(messageHandler);//v0.8+
        }

        /*
         * v0.3.0之前的原始Post方法见：WeixinController_OldPost.cs
         * 
         * 注意：虽然这里提倡使用CustomerMessageHandler的方法，但是MessageHandler基类最终还是基于OldPost的判断逻辑，
         * 因此如果需要深入了解Senparc.Weixin.MP内部处理消息的机制，可以查看WeixinController_OldPost.cs中的OldPost方法。
         * 目前为止OldPost依然有效，依然可用于生产。
         */

        /// <summary>
        /// 为测试并发性能而建
        /// </summary>
        /// <returns></returns>
        public ActionResult ForTest()
        {
            //异步并发测试（提供给单元测试使用）
            DateTime begin = DateTime.Now;
            int t1, t2, t3;
            System.Threading.ThreadPool.GetAvailableThreads(out t1, out t3);
            System.Threading.ThreadPool.GetMaxThreads(out t2, out t3);
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.5));
            DateTime end = DateTime.Now;
            var thread = System.Threading.Thread.CurrentThread;
            var result = string.Format("TId:{0}\tApp:{1}\tBegin:{2:mm:ss,ffff}\tEnd:{3:mm:ss,ffff}\tTPool：{4}",
                    thread.ManagedThreadId,
                    HttpContext.ApplicationInstance.GetHashCode(),
                    begin,
                    end,
                    t2 - t1
                    );
            return Content(result);
        }

        public ActionResult oauth2()
        {
            return Content(Request["code"]);
        }


        public ProInterface.Models.Api.ApiWeixinPayBean JSPay(string openid,string order_no, int total_fee,string body)
        {
            //var openIdResult = OAuthApi.GetAccessToken(AppId, AppSecret, code);

            //创建支付应答对象
            RequestHandler packageReqHandler = new RequestHandler(null);
            //初始化
            packageReqHandler.Init();

            var timeStamp = Senparc.Weixin.MP.TenPayLibV3.TenPayV3Util.GetTimestamp();
            var nonceStr = Senparc.Weixin.MP.TenPayLibV3.TenPayV3Util.GetNoncestr();

            //设置package订单参数
            packageReqHandler.SetParameter("appid", AppId);		  //公众账号ID
            packageReqHandler.SetParameter("mch_id", MchId);		  //商户号
            packageReqHandler.SetParameter("nonce_str", nonceStr);                    //随机字符串
            packageReqHandler.SetParameter("body", body);    //商品信息
            packageReqHandler.SetParameter("out_trade_no", order_no);		//商家订单号
            packageReqHandler.SetParameter("total_fee", total_fee.ToString());			        //商品金额,以分为单位(money * 100).ToString()
            packageReqHandler.SetParameter("spbill_create_ip", GetIP());   //用户的公网ip，不是商户服务器IP
            packageReqHandler.SetParameter("notify_url", NotifyUrl);		    //接收财付通通知的URL
            packageReqHandler.SetParameter("trade_type", TenPayV3Type.JSAPI.ToString());	                    //交易类型
            packageReqHandler.SetParameter("openid", openid);	                    //用户的openId
            
            string sign = packageReqHandler.CreateMd5Sign("key", appkey);
            packageReqHandler.SetParameter("sign", sign);	                    //签名

            string data = packageReqHandler.ParseXML();

            var result = TenPayV3.Unifiedorder(data);
            var res = XDocument.Parse(result);
            string result_code = res.Element("xml").Element("result_code").Value;
            if (result_code.Equals("FAIL"))
            {
                throw new Exception(res.Element("xml").Element("err_code_des").Value);
            }
            string prepayId = res.Element("xml").Element("prepay_id").Value;

            //设置支付参数
            RequestHandler paySignReqHandler = new RequestHandler(null);
            paySignReqHandler.SetParameter("appId", AppId);
            paySignReqHandler.SetParameter("timeStamp", timeStamp);
            paySignReqHandler.SetParameter("nonceStr", nonceStr);
            paySignReqHandler.SetParameter("package", string.Format("prepay_id={0}", prepayId));
            paySignReqHandler.SetParameter("signType", "MD5");
            var paySign = paySignReqHandler.CreateMd5Sign("key", appkey);

            ApiWeixinPayBean reBean = new ApiWeixinPayBean();
            reBean.appId = AppId;
            reBean.timeStamp = Convert.ToInt64( timeStamp);
            reBean.nonceStr = nonceStr;
            reBean.packageValue = string.Format("prepay_id={0}", prepayId);
            reBean.signType = "MD5";
            reBean.sign = paySign;
            return reBean;
        }


        private string GetIP()
        {
            string ip = string.Empty;
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
                ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            if (string.IsNullOrEmpty(ip))
                ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
            return ip;
        }

        public ActionResult PayNotifyUrl()
        {
                //["result_code"]: "SUCCESS"
                //["fee_type"]: "CNY"
                //["return_code"]: "SUCCESS"
                //["total_fee"]: "2"
                //["mch_id"]: "1354799602"
                //["cash_fee"]: "2"
                //["openid"]: "otuM4v9SrdHXuZYketltaOwVqvKA"
                //["transaction_id"]: "4008412001201610176912647416"
                //["sign"]: "A52AD2B85E1B38A85D76E0E5606B672E"
                //["bank_type"]: "CFT"
                //["appid"]: "wx3b8877cd39306fe6"
                //["time_end"]: "20161017110813"
                //["trade_type"]: "JSAPI"
                //["nonce_str"]: "8E98D81F8217304975CCB23337BB5761"
                //["is_subscribe"]: "Y"
                //["out_trade_no"]: "20161007095802004"

            ResponseHandler resHandler = new ResponseHandler(null);

            string return_code = resHandler.GetParameter("return_code");
            string return_msg = resHandler.GetParameter("return_msg");

            string res = null;

            resHandler.SetKey(appkey);
            //验证请求是否从微信发过来（安全）
            if (resHandler.IsTenpaySign())
            {
                res = "success";
                string openid = resHandler.GetParameter("openid");
                string cash_fee = resHandler.GetParameter("cash_fee");
                string transaction_id = resHandler.GetParameter("transaction_id");
                string out_trade_no = resHandler.GetParameter("out_trade_no");
                string time_end = resHandler.GetParameter("time_end");
                ProServer.Service ser = new ProServer.Service();
                ProInterface.ErrorInfo err = new ProInterface.ErrorInfo();
                ser.OrderFlowSavePay(null, ref err, out_trade_no, cash_fee, transaction_id, openid);
                //正确的订单处理
            }
            else
            {
                res = "wrong";

                //错误的订单处理
            }

            var fileStream = System.IO.File.OpenWrite(Server.MapPath("~/1.txt"));
            fileStream.Write(Encoding.Default.GetBytes(res), 0, Encoding.Default.GetByteCount(res));
            fileStream.Close();

            string xml = string.Format(@"<xml>
   <return_code><![CDATA[{0}]]></return_code>
   <return_msg><![CDATA[{1}]]></return_msg>
</xml>", return_code, return_msg);

            return Content(xml, "text/xml");
        }
    }
}
