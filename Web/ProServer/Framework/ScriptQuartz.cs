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
    public class ScriptQuartz : IJob
    {
        private static readonly Common.Logging.ILog logger = Common.Logging.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string statusLogPath = AppDomain.CurrentDomain.BaseDirectory + "/QuartzRunStatus.log";
        public void Execute(IJobExecutionContext context)
        {
            #region 添加脚本任务
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = schedulerFactory.GetScheduler();

            string jobGroupName = "ScriptJobGroup";
            string triGroupName = "ScriptTriGroup";
            string jobNamePex = "ScriptJob_";
            string triNamePex = "ScriptTri_";

            //所有需要运行的脚本
            var allScript = AllScript();
            var triKeyArr = scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals("ScriptTriGroup"));
            //删除触发器，删除这个触发器没有在运行的脚本里
            foreach (var t in triKeyArr)
            {
                var trigger = scheduler.GetTrigger(t);
                IJobDetail job = scheduler.GetJobDetail(trigger.JobKey);
                var tmp = allScript.SingleOrDefault(x => t.Name == triNamePex + x.ID.ToString());
                if (tmp == null)
                {
                    StopTask(Convert.ToInt32(t.Name.Replace(triNamePex, "")));
                    scheduler.DeleteJob(trigger.JobKey);
                    logger.InfoFormat("脚本服务 移除触发器ID{0}", t.Name);
                }
            }

            foreach (var t in allScript)
            {
                try
                {
                    //新任务
                    if (triKeyArr.SingleOrDefault(x => x.Name == triNamePex + t.ID.ToString()) == null)
                    {
                        IJobDetail job = JobBuilder.Create<ScriptTaskAddQuartz>()
                                        .WithIdentity(new JobKey(jobNamePex + t.ID.ToString(), jobGroupName))
                                        .StoreDurably()
                                        .Build();

                        ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                            .WithIdentity(new TriggerKey(triNamePex + t.ID.ToString(), triGroupName))
                                            .ForJob(job)
                                            .StartNow().WithCronSchedule(t.RUN_WHEN)
                                            .Build();
                        logger.InfoFormat("脚本服务 添加脚本触发器ID{0}", trigger.Key.Name);
                        scheduler.ScheduleJob(job, trigger);
                    }
                    else
                    {

                        ICronTrigger trigger = (ICronTrigger)scheduler.GetTrigger(new TriggerKey(triNamePex + t.ID.ToString(), triGroupName));
                        IJobDetail job = scheduler.GetJobDetail(trigger.JobKey);
                        if (trigger.CronExpressionString != t.RUN_WHEN)
                        {
                            logger.InfoFormat("脚本服务 修改触发器【{0}】的时间表达式【{1}】为【{2}】", trigger.Key.Name, trigger.CronExpressionString, t.RUN_WHEN);
                            trigger.CronExpressionString = t.RUN_WHEN;
                            scheduler.DeleteJob(trigger.JobKey);
                            scheduler.ScheduleJob(job, trigger);
                        }
                    }
                }
                catch(Exception e) {
                    Dictionary<string, object> dicStart = new Dictionary<string, object>();
                    dicStart.Add("STATUS", "禁用");
                    FunSqlToClass.UpData<SCRIPT>(
                        ConfigurationManager.AppSettings["dbType"],
                        ConfigurationManager.AppSettings["dbConnSt"],
                        dicStart,
                        string.Format("where ID={0} ", t.ID),
                        ConfigurationManager.AppSettings["dbPrefix"]);

                    logger.InfoFormat("脚本服务 添加脚本触发器任务【{0}】，失败【{1}】",t.CODE, e.Message);
                }

            }
            #endregion

            #region 运行脚本任务


            //获取正在等待的任务
            foreach (var task in AllScriptTask())
            {
                try
                {
                    #region 检测运算实例是否存在，并把结束了的线程终止
                    QuartzRunStatus nowQrs = new QuartzRunStatus();
                    IList<QuartzRunStatus> qrs = new List<QuartzRunStatus>();

                    try
                    {
                        qrs = ProInterface.JSON.EncodeToEntity<IList<QuartzRunStatus>>(System.IO.File.ReadAllText(statusLogPath));
                    }
                    catch { }
                    //清理2小时还没有远行完的口径
                    foreach (var t in qrs.Where(x => x.StatusTime.AddHours(2) < DateTime.Now).ToList())
                    {
                        qrs.Remove(t);
                    }
                    if (qrs == null) qrs = new List<QuartzRunStatus>();
                    nowQrs = qrs.SingleOrDefault(x => x.JobName == "ScriptID_" + task.SCRIPT_ID);
                    if (nowQrs == null) nowQrs = new QuartzRunStatus();
                    //表示该脚本正在运行，则退出
                    if (nowQrs.IsRun)
                    {
                        continue;
                    }
                    if (qrs.Count > ProInterface.AppSet.ScriptRunMaxNum)
                    {

                        logger.InfoFormat("执行脚本数【{0}】已经超过最大任务数【{1}】了", qrs.Count, ProInterface.AppSet.ScriptRunMaxNum);
                        return;
                    }
                    nowQrs.IsRun = true;
                    nowQrs.JobName = "ScriptID_" + task.SCRIPT_ID;
                    nowQrs.StatusTime = DateTime.Now;
                    if (qrs.SingleOrDefault(x => x.JobName == "ScriptID_" + task.SCRIPT_ID) == null)
                    {
                        qrs.Add(nowQrs);
                    }
                    Fun.WriteAllText(statusLogPath, ProInterface.JSON.DecodeToStr(qrs));

                    #endregion

                    logger.InfoFormat("执行脚本 开始脚本【{0}】，任务ID【{1}】", task.SCRIPT_ID, task.ID);
                    Dictionary<string, object> dicStart = new Dictionary<string, object>();
                    dicStart.Add("RUN_STATE", "运行");
                    dicStart.Add("START_TIME", DateTime.Now);
                    dicStart.Add("RETURN_CODE", null);
                    dicStart.Add("END_TIME", null);
                    dicStart.Add("DISABLE_DATE", null);
                    dicStart.Add("DISABLE_REASON", null);
                    FunSqlToClass.UpData<SCRIPT_TASK>(
                        ConfigurationManager.AppSettings["dbType"],
                        ConfigurationManager.AppSettings["dbConnSt"],
                        dicStart,
                        string.Format("where ID={0} ", task.ID),
                        ConfigurationManager.AppSettings["dbPrefix"]);
                    FunSqlToClass.NonQuery("DELETE FROM YL_SCRIPT_TASK_LOG where SCRIPT_TASK_ID=" + task.ID,
                        ConfigurationManager.AppSettings["dbType"],
                        ConfigurationManager.AppSettings["dbConnSt"],
                        ConfigurationManager.AppSettings["dbPrefix"]);

                    ProServer.Service db = new Service();
                    ProInterface.ErrorInfo error = new ProInterface.ErrorInfo();
                    object obj = db.ScriptTaskStart(ref error, Convert.ToInt32(task.ID));
                    if (error.IsError)
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("RUN_STATE", "停止");
                        dic.Add("RETURN_CODE", "失败");
                        dic.Add("END_TIME", DateTime.Now);
                        dic.Add("DISABLE_DATE", DateTime.Now);
                        if (error.Message != null)
                        {
                            if (error.Message.Length > 25)
                            {
                                dic.Add("DISABLE_REASON", error.Message.Substring(0, 25));
                            }
                            else
                            {
                                dic.Add("DISABLE_REASON", error.Message);
                            }
                        }
                        FunSqlToClass.UpData<SCRIPT_TASK>(
                            ConfigurationManager.AppSettings["dbType"],
                            ConfigurationManager.AppSettings["dbConnSt"],
                            dic,
                            string.Format("where ID={0} ", task.ID),
                            ConfigurationManager.AppSettings["dbPrefix"]);
                        logger.InfoFormat("执行脚本 脚本【{0}】，任务ID【{1}】 出错:{2}", task.SCRIPT_ID, task.ID, error.Message);
                    }
                    else
                    {
                        logger.InfoFormat("执行脚本 结束脚本【{0}】，任务ID【{1}】", task.SCRIPT_ID, task.ID);
                    }
                }
                catch (Exception e)
                {
                    logger.InfoFormat("分析 结束脚本【{0}】，任务ID【{1}】,出错:{2}", task.SCRIPT_ID, task.ID,e.ToString());
                }

                var qrsEnd = ProInterface.JSON.EncodeToEntity<IList<QuartzRunStatus>>(System.IO.File.ReadAllText(statusLogPath));
                var nowQrsEnd = qrsEnd.SingleOrDefault(x => x.JobName == "ScriptID_" + task.SCRIPT_ID);
                if (nowQrsEnd != null)
                {
                    qrsEnd.Remove(nowQrsEnd);
                    Fun.WriteAllText(statusLogPath, ProInterface.JSON.DecodeToStr(qrsEnd));
                }
            }
            #endregion
        }

        /// <summary>
        /// 获取所有脚本
        /// </summary>
        /// <returns></returns>
        public IList<ProInterface.Models.SCRIPT> AllScript()
        {
            IList<ProInterface.Models.SCRIPT> reList = new List<ProInterface.Models.SCRIPT>();
            reList = FunSqlToClass.SqlToList<ProInterface.Models.SCRIPT>("select * from YL_SCRIPT where STATUS='正常' and RUN_WHEN is not null", ConfigurationManager.AppSettings["dbType"], ConfigurationManager.AppSettings["dbConnSt"]);
            reList = reList.OrderBy(x => x.ID).ToList();
            return reList;
        }


        /// <summary>
        /// 获取所有等待脚本任务
        /// </summary>
        /// <returns></returns>
        public IList<ProInterface.Models.SCRIPT_TASK> AllScriptTask()
        {
            IList<ProInterface.Models.SCRIPT_TASK> reList = new List<ProInterface.Models.SCRIPT_TASK>();
            reList = FunSqlToClass.SqlToList<ProInterface.Models.SCRIPT_TASK>("select * from YL_SCRIPT_TASK where RUN_STATE='等待'", ConfigurationManager.AppSettings["dbType"], ConfigurationManager.AppSettings["dbConnSt"]);
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


    /// <summary>
    /// 添加一个脚本任务
    /// </summary>
    public class ScriptTaskAddQuartz : IJob
    {
        private static readonly Common.Logging.ILog logger = Common.Logging.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Execute(IJobExecutionContext context)
        {


            var scriptId = Convert.ToInt32(context.JobDetail.Key.Name.Replace("ScriptJob_", ""));
            var taskEnt = FunSqlToClass.ClassSingle<SCRIPT_TASK>(
    ConfigurationManager.AppSettings["dbType"],
    ConfigurationManager.AppSettings["dbConnSt"],
    "where RUN_STATE='运行' AND SCRIPT_ID = " + scriptId,
    ConfigurationManager.AppSettings["dbPrefix"]
    );
            //如果有任务没有结束，则不添加任务
            if (taskEnt != null && taskEnt.ID != 0)
            {
                if (taskEnt.START_TIME != null && taskEnt.START_TIME.Value.AddHours(2) < DateTime.Now)
                {
                    Dictionary<string, object> dicStart = new Dictionary<string, object>();
                    dicStart.Add("RUN_STATE", "失败");
                    dicStart.Add("DISABLE_REASON", "运行超时");
                    FunSqlToClass.UpData<SCRIPT_TASK>(
                        ConfigurationManager.AppSettings["dbType"],
                        ConfigurationManager.AppSettings["dbConnSt"],
                        dicStart,
                        string.Format("where ID={0} ", scriptId),
                        ConfigurationManager.AppSettings["dbPrefix"]);
                }
                else
                {
                    return;
                }
            }


            var scriptEnt = FunSqlToClass.ClassSingle<SCRIPT>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                "where ID = " + scriptId,
                ConfigurationManager.AppSettings["dbPrefix"]
                );


            ScriptExt ext = new ScriptExt();
            if (scriptEnt.IS_GROUP == (short)1)//表示是任务组
            {
                IList<ProInterface.Models.SCRIPT_GROUP_LIST> reList = new List<ProInterface.Models.SCRIPT_GROUP_LIST>();
                reList = FunSqlToClass.SqlToList<ProInterface.Models.SCRIPT_GROUP_LIST>(string.Format("SELECT * FROM YL_SCRIPT_GROUP_LIST WHERE GROUP_ID={0} ORDER BY ORDER_INDEX", scriptId), ConfigurationManager.AppSettings["dbType"], ConfigurationManager.AppSettings["dbConnSt"]);
                if (reList.Count() > 0)
                {
                    long taskId = 0;
                    ext.AddScriptTask(reList[0].SCRIPT_ID, scriptId, ref taskId);
                    logger.InfoFormat("执行脚本 添加脚本【{0}】任务ID【{1}】", scriptId, taskId);
                }
            }
            else//普通脚本
            {
                long taskId = 0;
                ext.AddScriptTask(scriptId, null, ref taskId);
                logger.InfoFormat("执行脚本 添加脚本【{0}】任务ID【{1}】", scriptId, taskId);
            }
        }

    }
}
