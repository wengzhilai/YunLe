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
    /// <summary>
    /// 添加和移除任务
    /// </summary>
    public class ReflashDownData : IJob
    {
        private static readonly Common.Logging.ILog logger = Common.Logging.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Execute(IJobExecutionContext context)
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = schedulerFactory.GetScheduler();

            string jobGroupName = "DownDataJobGroup";
            string triGroupName = "DownDataTriGroup";
            string jobNamePex = "DownDataJob_";
            string triNamePex = "DownDataTri_";
            List<string> rescheduledJobs = new List<string>();

            //获取所有下载的任务
            var allTask = AllDownData(); ;
            var triKeyArr = scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(triGroupName));
            //删除触发器
            foreach (var t in triKeyArr)
            {
                var trigger = scheduler.GetTrigger(t);
                IJobDetail job = scheduler.GetJobDetail(trigger.JobKey);
                var tmp = allTask.SingleOrDefault(x => triNamePex + x.ID.ToString() == t.Name);
                if (tmp == null)
                {
                    StopTask(Convert.ToInt32(t.Name.Replace(triNamePex, "")));
                    scheduler.DeleteJob(trigger.JobKey);
                    logger.InfoFormat("脚本服务 移除下表触发器ID【{0}】", t.Name);
                }
            }

            foreach (var t in allTask)
            {
                //新任务
                if (triKeyArr.SingleOrDefault(x => x.Name == triNamePex + t.ID.ToString()) == null)
                {
                    IJobDetail job = JobBuilder.Create<DataDownQuartz>()
                                    .WithIdentity(new JobKey(jobNamePex + t.ID.ToString(), jobGroupName))
                                    .StoreDurably()
                                    .Build();

                    ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                        .WithIdentity(new TriggerKey(triNamePex + t.ID.ToString(), triGroupName))
                                        .ForJob(job)
                                        .StartNow().WithCronSchedule(t.CRON_EXPRESSION)
                                        .Build();
                    logger.InfoFormat("脚本服务 添加下表触发器ID【{0}】", trigger.Key.Name);
                    scheduler.ScheduleJob(job, trigger);
                }
                else
                {

                    ICronTrigger trigger = (ICronTrigger)scheduler.GetTrigger(new TriggerKey(triNamePex + t.ID.ToString(), triGroupName));
                    IJobDetail job = scheduler.GetJobDetail(trigger.JobKey);
                    if (trigger.CronExpressionString != t.CRON_EXPRESSION)
                    {
                        logger.InfoFormat("脚本服务 修改触发器【{0}】的时间表达式【{1}】为【{2}】", trigger.Key.Name, trigger.CronExpressionString, t.CRON_EXPRESSION);
                        trigger.CronExpressionString = t.CRON_EXPRESSION;
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有任务
        /// </summary>
        /// <returns></returns>
        public IList<ProInterface.Models.DATA_DOWN> AllDownData()
        {
            IList<ProInterface.Models.DATA_DOWN> reList = new List<ProInterface.Models.DATA_DOWN>();
            reList = FunSqlToClass.SqlToList<ProInterface.Models.DATA_DOWN>("select * from YL_DATA_DOWN where STATUS='正常' and CRON_EXPRESSION is not null", ConfigurationManager.AppSettings["dbType"], ConfigurationManager.AppSettings["dbConnSt"]);
            return reList;
        }

        /// <summary>
        /// 强行停止一个任务
        /// </summary>
        /// <param name="scriptId"></param>
        /// <returns></returns>
        public bool StopTask(int scriptId)
        {
            ProServer.Service db = new Service();
            ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
            return db.ScriptCancel(ref error, scriptId);
        }
    }



    public class DataDownQuartz : IJob
    {
        private static readonly Common.Logging.ILog logger = Common.Logging.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                logger.Info("启动自动下载数据 开始");
                var downID = Convert.ToInt32(context.JobDetail.Key.Name.Replace("DownDataJob_", ""));
                logger.Info(string.Format("启动自动下载数据 数据ID{0}", downID));


                var down = FunSqlToClass.ClassSingle<DATA_DOWN>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                "where ID = " + downID,
                ConfigurationManager.AppSettings["dbPrefix"]
                );

                string tableName = ProServer.Fun.ReplaceDataTime(down.CREATE_TABLE_NAME, DateTime.Now);
                var eventEnt = new DATA_DOWN_EVENT();
                eventEnt.ID = FunSqlToClass.GetSeqID<DATA_DOWN_EVENT>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                ConfigurationManager.AppSettings["dbPrefix"]
                );
                eventEnt.TARGET_NAME = tableName;
                eventEnt.ALL_NUM = 0;
                eventEnt.LAST_MONTH_NUM = 0;
                eventEnt.PATH = down.TO_PATH;
                eventEnt.DATA_DOWN_ID = downID;
                eventEnt.START_TIME = DateTime.Now;

                FunSqlToClass.Save<DATA_DOWN_EVENT>(
                    ConfigurationManager.AppSettings["dbType"],
                    ConfigurationManager.AppSettings["dbConnSt"],
                    eventEnt,
                    ConfigurationManager.AppSettings["dbPrefix"]
                );

                string createScript = "";
                if (!string.IsNullOrEmpty(down.CREATE_SCRIPT))
                {
                    createScript = down.CREATE_SCRIPT.Replace("{@TABLE_NAME}", eventEnt.TARGET_NAME);
                }

                var allDataDownTo = FunSqlToClass.SqlToList<ProInterface.Models.DATA_DOWN_TO>(string.Format("select * from YL_DATA_DOWN_TO where DATA_DOWN_ID='{0}'", downID), ConfigurationManager.AppSettings["dbType"], ConfigurationManager.AppSettings["dbConnSt"]);

                foreach (var to in allDataDownTo)
                {
                    var toServer = FunSqlToClass.ClassSingle<DB_SERVER>(
                        ConfigurationManager.AppSettings["dbType"],
                        ConfigurationManager.AppSettings["dbConnSt"],
                        "where ID = " + to.DB_SERVER_ID,
                        ConfigurationManager.AppSettings["dbPrefix"]
                        );

                    #region 在目标服务器上创建表
                    var db = new ProServer.ScriptExt();
                    try
                    {
                        try
                        {
                            db.execute("drop table " + eventEnt.TARGET_NAME, toServer);
                        }
                        catch (Exception e)
                        {

                        }
                        db.execute(createScript, toServer);
                    }
                    catch (Exception e)
                    {
                        logger.Info(string.Format("在服务器【{0}】上建表失败：\r\nSQL:{1}\r\n{2}", toServer.NICKNAME, createScript, e.Message));
                    }
                    #endregion


                    var allDataDownForm = FunSqlToClass.SqlToList<ProInterface.Models.DB_SERVER>(string.Format("select a.* from YL_DB_SERVER a,YL_DATA_DOWN_FORM b where a.ID=b.DB_SERVER_ID and b.DATA_DOWN_ID='{0}'", downID), ConfigurationManager.AppSettings["dbType"], ConfigurationManager.AppSettings["dbConnSt"]);
                    foreach (var from in allDataDownForm)
                    {

                        DATA_DOWN_TASK task = new DATA_DOWN_TASK();
                        task.ID = FunSqlToClass.GetSeqID<DATA_DOWN_EVENT>(
                            ConfigurationManager.AppSettings["dbType"],
                            ConfigurationManager.AppSettings["dbConnSt"],
                            ConfigurationManager.AppSettings["dbPrefix"]
                            );
                        task.NAME = eventEnt.TARGET_NAME;
                        task.SELECT_SCRIPT = down.SELECT_SCRIPT;
                        task.EVENT_ID = eventEnt.ID;
                        #region 替换SELECT_SCRIPT
                        foreach (var replace in JSON.EncodeToEntity<IList<KV>>(to.REPLACE_STR))
                        {
                            task.SELECT_SCRIPT = task.SELECT_SCRIPT.Replace(replace.K, replace.V);
                        }
                        task.SELECT_SCRIPT = ProServer.Fun.ReplaceDataTime(task.SELECT_SCRIPT, DateTime.Now);
                        #endregion

                        #region 生成@[00-99]
                        if (task.SELECT_SCRIPT.IndexOf("@") != -1)
                        {
                            task.SELECT_SCRIPT = Fun.GetSelectScript(task.SELECT_SCRIPT);
                        }
                        #endregion

                        //设置存放路径
                        task.TO_PATH = ProServer.Fun.ReplaceDataTime(down.TO_PATH, DateTime.Now);

                        int thisAllNum = 0;

                        task.ALL_NUM = thisAllNum;
                        int upMonthNum = 0;

                        task.LAST_MONTH_NUM = upMonthNum;
                        task.SELECT_DB_SERVER = from.ID;
                        task.SELECT_SERVER = string.Format("(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVER=DEDICATED)(SID={2})))", from.IP, from.PORT, from.DBNAME);
                        task.SELECT_UID = from.UID;
                        task.SELECT_PWD = from.PASSWORD;
                        task.EVENT_TYPE = 1;
                        if (down.SUCC_SCRIPT != null)
                        {
                            task.SUCC_SCRIPT = down.SUCC_SCRIPT.Replace("{@TABLE_NAME}", eventEnt.TARGET_NAME);
                        }
                        task.TO_DB_SERVER = toServer.ID;
                        task.TO_SERVER = toServer.DBNAME;
                        task.TO_UID = toServer.UID;
                        task.TO_PWD = toServer.PASSWORD;
                        task.CREATE_SCRIPT = createScript;
                        task.ERROR_NUM = 0;
                        task.STATUS = "等待";
                        task.PAGE_SIZE = down.PAGE_SIZE;
                        task.SPLIT_STR = down.SPLIT_STR;
                        task.IS_CANCEL = 0;
                        task.ORDER_NUM = task.ID;

                        FunSqlToClass.Save<DATA_DOWN_TASK>(
                            ConfigurationManager.AppSettings["dbType"],
                            ConfigurationManager.AppSettings["dbConnSt"],
                            task,
                            ConfigurationManager.AppSettings["dbPrefix"]
                        );
                    }
                }

                logger.Info("启动自动下载数据 结束");
            }
            catch (Exception ex)
            {
                logger.Error("启动自动下载数据 运行异常", ex);
            }
        }
    }
}