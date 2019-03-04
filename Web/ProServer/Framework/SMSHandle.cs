using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using ProServer.SMSServices;

namespace ProServer
{
    public class SMSHandle
    {
        public static SubmitServiceClient gyclient = null;
        public static ReqHeader header = null;
        public static SmsSubmit smssubmit = null;

        private static SubmitServiceClient GetStubClient()
        {
            if (((gyclient == null) || (smssubmit == null)) || (header == null))
            {
                try
                {
                    gyclient = new SubmitServiceClient();
                    smssubmit = new SmsSubmit();
                    header = new ReqHeader();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            return gyclient;
        }

        public static string sendSms(IList<string> moblist, string content)
        {
            int maxSend=1000;
            string result = "";
            if (moblist.Count() <= maxSend)
            {
                result = EasyManSmsSend(moblist.ToArray(), content);
            }
            else
            {
                int page = moblist.Count() / maxSend;
                for (int i = 0; i < page; i++)
                {
                    var thisSend = moblist.Skip(maxSend * i).Take(maxSend).ToArray();
                    result = EasyManSmsSend(thisSend, content);
                    Thread.Sleep(50);
                }
                if (moblist.Count() % maxSend != 0)
                {
                    var thisSend = moblist.Skip(maxSend * page).Take(maxSend).ToArray();
                    result = EasyManSmsSend(thisSend, content);
                }

            }

            return result;
        }

        static string EasyManSmsSend(string[] moblist, string content)
        {
            try
            {
                GetStubClient();
                header.sysid = "85152301809";
                string strPassWord = "qewerwrffsetxd";
                string strMagic = "dafewerqrewqe";
                string authCode1 = CreateMD5(strPassWord + strMagic);
                header.authCode = "7DE8047D9A79069C9266BD71AEFD8D44";//md5
                header.reqno = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                smssubmit.content = content;
                smssubmit.param = null;
                smssubmit.sourceAddr = "1065733372257";
                smssubmit.dest = getmobarry(moblist);
                RespHeader k = gyclient.smsSubmit(header, smssubmit, null);
                string aa = k.resultCode;
                if (string.IsNullOrEmpty(k.resultCode))
                {
                    return "";
                }
                //LogWriteGuiYangSms(k, moblist.Length);//入本地数据
                return aa;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static string CreateMD5(String input)
        {
            Byte[] clearBytes = new UnicodeEncoding().GetBytes(input);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);
            string tt = BitConverter.ToString(hashedBytes).Replace("-", "");
            return tt;
        }

        protected static void LogWriteEasyManSms(RespHeader rinfo, int lenNum)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("本次提交号码：" + lenNum + "个\r\n");
            sb.Append("返回执行码：" + rinfo.resultCode + "\r\n");
            string CurrentPathSms = "E:\\";
            string pathMms = CurrentPathSms + "Log\\SmsLog.log";
            FileStream fs = new FileStream(pathMms, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(sb.ToString() + "\r\n--------------------------------------------------------执行时间：" + DateTime.Now.ToString());
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        private static ArrayOfString getmobarry(string[] mob)
        {
            ArrayOfString refof = new ArrayOfString();
            for (int i = 0; i < mob.Length; i++)
            {
                refof.Add(mob[i]);
            }
            return refof;
        }
    }
}
