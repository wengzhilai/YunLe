using ProInterface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ProServer
{

    public class SMS
    {
        /// <summary>
        /// 用于初始化终端与串口的连接
        /// </summary>
        /// <param name="CopyRight"></param>
        /// <param name="Com_Port">串口号（0为红外接口，1,2,3,...为串口）</param>
        /// <param name="Com_BaudRate">波特率</param>
        /// <param name="Mobile_Type">返回终端型号</param>
        /// <param name="CopyRightToCOM"></param>
        /// <returns>返回值(0：连接终端失败；1：连接终端成功)</returns>
        [DllImport("sms.dll", EntryPoint = "Sms_Connection")]
        public static extern uint Sms_Connection(string CopyRight, uint Com_Port, uint Com_BaudRate, out string Mobile_Type, out string CopyRightToCOM);

        /// <summary>
        /// 断开终端与串口的连接
        /// </summary>
        /// <returns></returns>
        [DllImport("sms.dll", EntryPoint = "Sms_Disconnection")]
        public static extern uint Sms_Disconnection();

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="Sms_TelNum">发送给的终端号码</param>
        /// <param name="Sms_Text">发送的短信内容</param>
        /// <returns>返回值(0：发送短信失败；1：发送短信成功)</returns>
        [DllImport("sms.dll", EntryPoint = "Sms_Send")]
        public static extern uint Sms_Send(string Sms_TelNum, string Sms_Text);

        /// <summary>
        /// 接收指定类型的短信
        /// </summary>
        /// <param name="Sms_Type">短信类型(0：未读短信；1：已读短信；2：待发短信；3：已发短信；4：全部短信)</param>
        /// <param name="Sms_Text">返回指定类型的短信内容字符串(短信内容字符串说明：短信与短信之前用"|"符号作为分隔符,每条短信中间的各字段用"#"符号作为分隔符)</param>
        /// <returns></returns>
        [DllImport("sms.dll", EntryPoint = "Sms_Receive")]
        public static extern uint Sms_Receive(string Sms_Type, out string Sms_Text);

        [DllImport("sms.dll", EntryPoint = "Sms_Delete")]
        public static extern uint Sms_Delete(string Sms_Index);

        [DllImport("sms.dll", EntryPoint = "Sms_AutoFlag")]
        public static extern uint Sms_AutoFlag();

        [DllImport("sms.dll", EntryPoint = "Sms_NewFlag")]
        public static extern uint Sms_NewFlag();

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        public void SmsConnection(string loginKey, ref ErrorInfo err)
        {
            uint port = Convert.ToUInt32(ConfigurationManager.AppSettings["smsPort"]);


            String TypeStr = "";
            String CopyRightToCOM = "";
            String CopyRightStr = "//上海迅赛信息技术有限公司,网址www.xunsai.com//";

            if (Sms_Connection(CopyRightStr, port, 9600, out TypeStr, out CopyRightToCOM) == 1) ///5为串口号，0为红外接口，1,2,3,...为串口
            {
                err.IsError = false;
                err.Message = TypeStr;
            }
            else
            {
                err.IsError = true;
                err.Message= "连接失败！";

            }
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        public void SmsDisconnection(string loginKey, ref ErrorInfo err)
        {
            try
            {
                Sms_Disconnection();
            }
            catch (AccessViolationException e)
            {
                err.IsError = true;
                err.Message = e.Message;
            }
        }

        /// <summary>
        /// 发送短信,每次发送前连接设备，发送完后，断开设备
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="telNum"></param>
        /// <param name="sendSms"></param>
        /// <returns>成功条数</returns>
        public int SmsSend(string loginKey, ref ErrorInfo err, IList<string> telNum, string sendSms, ref IList<string> errorTelNum)
        {
            SmsConnection(loginKey, ref err);
            int succNum = 0;
            errorTelNum = new List<string>();
            foreach (var t in telNum)
            {
                if (Sms_Send(t, sendSms) == 1)
                {
                    succNum++;
                }
                else
                {
                    errorTelNum.Add(t);
                }
            }
            SmsDisconnection(loginKey, ref err);
            return succNum;
        }

        public bool SmsSendOne(string loginKey, ref ErrorInfo err, string telNum, string sendSms)
        {
            try
            {
                SmsConnection(loginKey, ref err);
                if (Sms_Send(telNum, sendSms) == 1)
                {
                    SmsDisconnection(loginKey, ref err);
                    return true;
                }
                else
                {
                    SmsDisconnection(loginKey, ref err);
                    return false;
                }
            }
            catch {
                return false;
            }
        }

        public bool SmsSend(string loginKey, ref ErrorInfo err, string telNum, string sendSms)
        {
            try
            {
                if (Sms_Send(telNum, sendSms) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch {
                return false;
            }
        }


        /// <summary>
        /// 接收短信
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public string SmsReceive(string loginKey, ref ErrorInfo err)
        {
            String ReceiveSmsStr = "";
            //短信类型(0：未读短信；1：已读短信；2：待发短信；3：已发短信；4：全部短信)
            if (Sms_Receive("4", out ReceiveSmsStr) == 1)
            {
                return ReceiveSmsStr;
            }
            else
            {
                err.IsError = true;
                err.Message = "读取短信失败";
            }
            return null;
        }

        /// <summary>
        /// 删除短信
        /// </summary>
        /// <param name="smsIndex"></param>
        public void SmsDelete(string smsIndex)
        {
            Sms_Delete(smsIndex);
        }
        /// <summary>
        /// 检测设备是否正常
        /// </summary>
        /// <returns></returns>
        public bool SmsNewFlag()
        {
            if (Sms_NewFlag() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
