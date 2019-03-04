using ProInterface;
using ProInterface.Models;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ProServer
{

    public class SmsSendQuartz : IJob
    {
        private static readonly Common.Logging.ILog logger = Common.Logging.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string statusLogPath = AppDomain.CurrentDomain.BaseDirectory + "/QuartzRunStatus.log";
        public void Execute(IJobExecutionContext context)
        {

            #region 检测运算实例是否存在，并把结束了的线程终止

            QuartzRunStatus nowQrs = new QuartzRunStatus();
            IList<QuartzRunStatus> qrs = new List<QuartzRunStatus>();

            try
            {
                qrs = ProInterface.JSON.EncodeToEntity<IList<QuartzRunStatus>>(System.IO.File.ReadAllText(statusLogPath));
            }
            catch { }
            foreach (var t in qrs.Where(x => x.StatusTime.AddHours(2) < DateTime.Now).ToList())
            {
                qrs.Remove(t);
            }
            if (qrs == null) qrs = new List<QuartzRunStatus>();
            nowQrs = qrs.SingleOrDefault(x => x.JobName == context.JobDetail.Key.Name);
            if (nowQrs == null)
            {
                nowQrs = new QuartzRunStatus();
            }
            if (nowQrs.IsRun)
            {
                return ;
            }
            nowQrs.IsRun = true;
            nowQrs.JobName = context.JobDetail.Key.Name;
            nowQrs.StatusTime = DateTime.Now;
            if (qrs.SingleOrDefault(x => x.JobName == context.JobDetail.Key.Name) == null)
            {
                qrs.Add(nowQrs);
            }
            Fun.WriteAllText(statusLogPath, ProInterface.JSON.DecodeToStr(qrs));

            #endregion



            
            try
            {
                #region 添加发送的短信
                //添加数据
                string sql = @"
INSERT INTO YL_SMS_SEND(
   KEY,
   MESSAGE_ID,
   PHONE_NO,
   ADD_TIME,
   CONTENT,
   STAUTS)
SELECT 
   SYS_GUID() KEY,
   A.MESSAGE_ID,
   A.PHONE_NO,
   sysdate ADD_TIME,
   B.CONTENT,
   '等待' STAUTS
 FROM YL_USER_MESSAGE A,YL_MESSAGE B WHERE A.MESSAGE_ID=B.ID AND A.STATUS='等待' AND PHONE_NO IS NOT NULL
AND ((CEIL(((SYSDATE -CAST(A.STATUS_TIME AS DATE) )) * 24 * 60)>30 AND B.PUSH_TYPE='智能推送') OR B.PUSH_TYPE='短信推送')
";
                //更新状态
                string sqlUpdate = "UPDATE YL_USER_MESSAGE SET  STATUS='已推送',PUSH_TYPE='短信推送' WHERE STATUS='等待' AND PHONE_NO IS NOT NULL AND ((CEIL(((SYSDATE -CAST(STATUS_TIME AS DATE) )) * 24 * 60)>30 AND PUSH_TYPE='智能推送') OR PUSH_TYPE='短信推送')";

                FunSqlToClass.NonQuery(sql,
                    ConfigurationManager.AppSettings["dbType"],
                    ConfigurationManager.AppSettings["dbConnSt"],
                    ConfigurationManager.AppSettings["dbPrefix"]);

                FunSqlToClass.NonQuery(sqlUpdate,
                    ConfigurationManager.AppSettings["dbType"],
                    ConfigurationManager.AppSettings["dbConnSt"],
                    ConfigurationManager.AppSettings["dbPrefix"]);
                
                #endregion

                
                IList<ProInterface.Models.SMS_SEND> reList = new List<ProInterface.Models.SMS_SEND>();
                reList = FunSqlToClass.SqlToList<ProInterface.Models.SMS_SEND>("SELECT * FROM YL_SMS_SEND WHERE STAUTS='等待' OR (STAUTS='失败' AND TRY_NUM<5)", ConfigurationManager.AppSettings["dbType"], ConfigurationManager.AppSettings["dbConnSt"]);
                if (reList != null && reList.Count() > 0)
                {
                    reList = reList.OrderBy(x => x.ADD_TIME).ToList();
                    ProInterface.ErrorInfo error = new ErrorInfo();
                    int succNum = 0;

                    FunSqlToClass.NonQuery("UPDATE YL_SMS_SEND SET TRY_NUM=TRY_NUM+1 WHERE STAUTS='等待' OR (STAUTS='失败' AND TRY_NUM<5)",
                        ConfigurationManager.AppSettings["dbType"],
                        ConfigurationManager.AppSettings["dbConnSt"],
                        ConfigurationManager.AppSettings["dbPrefix"]);

                    foreach (var t in reList.GroupBy(x => x.CONTENT).ToList())
                    {
                        var conten = t.Key;
                        var phoneList = t.Select(x => x.PHONE_NO).ToArray();
                        
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("SEND_TIME", DateTime.Now);
                        dic.Add("STAUTS", "成功");

                        var allPage = t.Count() / 1000;
                        if (t.Count() % 1000 != 0)
                        {
                            allPage++;
                        }
                        for (var i = 0; i < allPage; i++)
                        {
                            FunSqlToClass.UpData<SMS_SEND>(
                            ConfigurationManager.AppSettings["dbType"],
                            ConfigurationManager.AppSettings["dbConnSt"],
                            dic,
                            string.Format(" where KEY IN ('{0}') ", string.Join("','", t.Skip(i*1000).Take(1000).Select(x => x.KEY))),
                            ConfigurationManager.AppSettings["dbPrefix"]);
                        }

                        foreach (var phone in phoneList)
                        {
                            DbHelper.MasHelper.Send(phone, conten);
                        }
                        succNum += phoneList.Count();
                    }


                    //sms.SmsConnection("", ref error);
                    //foreach (var t in reList)
                    //{
                    //    Dictionary<string, object> dic = new Dictionary<string, object>();
                    //    dic.Add("SEND_TIME", DateTime.Now);
                    //    string[] listPhoneNo = new string[] { t.PHONE_NO };
                    //    if (sms.SmsSend("", ref error, t.PHONE_NO, t.CONTENT))
                    //    //if (SMSHandle.sendSms(listPhoneNo, t.CONTENT) == "0")
                    //    {
                    //        succNum++;
                    //        dic.Add("STAUTS", "成功");
                    //    }
                    //    else
                    //    {
                    //        dic.Add("STAUTS", "失败");
                    //    }
                    //    FunSqlToClass.UpData<SMS_SEND>(
                    //        ConfigurationManager.AppSettings["dbType"],
                    //        ConfigurationManager.AppSettings["dbConnSt"],
                    //        dic,
                    //        string.Format(" where KEY='{0}' ", t.KEY),
                    //        ConfigurationManager.AppSettings["dbPrefix"]);
                    //}
                    //sms.SmsDisconnection("", ref error);
                    //if (error.IsError)
                    //{
                    //    logger.Info(string.Format("断开短信猫失败【{0}】", error.Message));
                    //}


                    logger.Info(string.Format("成功发送【{0}】条短信", succNum));
                }
                qrs.Remove(nowQrs);
                Fun.WriteAllText(statusLogPath, ProInterface.JSON.DecodeToStr(qrs));
            }
            catch (Exception ex)
            {
                qrs.Remove(nowQrs);
                Fun.WriteAllText(statusLogPath, ProInterface.JSON.DecodeToStr(qrs));
                logger.Error("发送短信 运行异常", ex);
            }
        }
    }
}