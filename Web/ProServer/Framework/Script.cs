using Microsoft.CSharp;
using ProInterface;
using ProInterface.Models;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ProServer
{
    /// <summary>  
    /// 代码执行类  
    /// </summary>  
    public partial class Service : IScript
    {
        Assembly objAssembly = null;

        private string statusLogPath = AppDomain.CurrentDomain.BaseDirectory + "/QuartzRunStatus.log";

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="csharpCode"></param>
        /// <param name="scriptTaskID"></param>
        /// <param name="defaultDb"></param>
        /// <returns>程序集的实例</returns>
        public bool ScriptTaskStart(ref ErrorInfo error, int scriptTaskID)
        {

            var scriptEnt = FunSqlToClass.ClassSingle<SCRIPT_TASK>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                "where ID = " + scriptTaskID,
                ConfigurationManager.AppSettings["dbPrefix"]
                );
            if (scriptEnt == null)
            {
                error.IsError = true;
                error.Message = "脚本不存在";
                return false;
            }

            string pathInterface = AppDomain.CurrentDomain.BaseDirectory + "ProInterface.dll";
            string pathServer = AppDomain.CurrentDomain.BaseDirectory + "ProServer.dll";

            #region 判断类库是否存在
            if (!System.IO.File.Exists(pathInterface))
            {
                pathInterface = AppDomain.CurrentDomain.BaseDirectory + "Bin\\ProInterface.dll";
            }
            if (!System.IO.File.Exists(pathServer))
            {
                pathServer = AppDomain.CurrentDomain.BaseDirectory + "Bin\\ProServer.dll";
            }

            if (!System.IO.File.Exists(pathInterface))
            {
                error.Message = string.Format("类库【{0}】不存在", pathInterface);
                return false;
            }
            if (!System.IO.File.Exists(pathServer))
            {
                error.Message = string.Format("类库【{0}】不存在", pathInterface);
                return false;
            }
            #endregion

            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();
            ICodeCompiler objICodeCompiler = objCSharpCodePrivoder.CreateCompiler();
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            objCompilerParameters.ReferencedAssemblies.Add(pathInterface);
            objCompilerParameters.ReferencedAssemblies.Add(pathServer);
            objCompilerParameters.GenerateExecutable = false;
            objCompilerParameters.GenerateInMemory = true;

            string allCode = "";
            try
            {
                allCode = GenerateCode(scriptEnt.BODY_TEXT, AnalysisRunDate(scriptEnt.RUN_DATA));
            }
            catch
            {
                ErrorScriptTask(error.Message, Convert.ToInt32(scriptEnt.ID));
                return false;
            }
            CompilerResults cr = objICodeCompiler.CompileAssemblyFromSource(objCompilerParameters, allCode);
            if (cr.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder("编译错误：");
                foreach (CompilerError err in cr.Errors)
                {
                    sb.AppendLine(string.Format("行{0}列{1}：{2} <br />", err.Line - 12, err.Column, err.ErrorText));
                }
                error.IsError = true;
                error.Message = sb.ToString();

                ErrorScriptTask(error.Message, Convert.ToInt32(scriptEnt.ID));

                return false;
            }
            else
            {
                // 通过反射，调用HelloWorld的实例
                objAssembly = cr.CompiledAssembly;
                object objScripRun = objAssembly.CreateInstance("ProServer.ScripRun");

                //创建任务
                var bodyText = objScripRun.GetType().GetMethod("ScriptTaskRun").Invoke(objScripRun, new object[] { scriptEnt.ID });
                var setnowdb = objScripRun.GetType().GetMethod("setnowdb").Invoke(objScripRun, new object[] { scriptEnt.SERVICE_FLAG });
                //运行脚本
                var run = objScripRun.GetType().GetMethod("Run").Invoke(objScripRun, null);
                var errorMsg = objScripRun.GetType().GetMethod("GetError").Invoke(objScripRun, null);

                IList<ProInterface.Models.SCRIPT_GROUP_LIST> reList = FunSqlToClass.SqlToList<ProInterface.Models.SCRIPT_GROUP_LIST>(
                    string.Format("SELECT * FROM YL_SCRIPT_GROUP_LIST WHERE GROUP_ID={0} ORDER BY ORDER_INDEX", (scriptEnt.GROUP_ID == null) ? 0 : scriptEnt.GROUP_ID.Value),
                    ConfigurationManager.AppSettings["dbType"],
                    ConfigurationManager.AppSettings["dbConnSt"]);
                if (reList.Count() > 0 && (bool)run && string.IsNullOrEmpty(errorMsg.ToString()))
                {
                    var nowPlace = reList.SingleOrDefault(x => x.SCRIPT_ID == scriptEnt.SCRIPT_ID);
                    var last = reList.Where(x => x.ORDER_INDEX > nowPlace.ORDER_INDEX).ToList();
                    if (last.Count() > 0)
                    {
                        long taskId = 0;
                        ScriptExt ext = new ScriptExt();
                        ext.AddScriptTask(last[0].SCRIPT_ID, last[0].GROUP_ID, ref taskId);
                    }
                }
                return string.IsNullOrEmpty(errorMsg.ToString());
            }
        }

        public bool ScriptCancel(ref ErrorInfo err, int scriptID)
        {

            try
            {
                var qrs = ProInterface.JSON.EncodeToEntity<IList<QuartzRunStatus>>(System.IO.File.ReadAllText(statusLogPath));
                var nowQrs = qrs.SingleOrDefault(x => x.JobName == "ScriptID_" + scriptID);
                qrs.Remove(nowQrs);
                Fun.WriteAllText(statusLogPath, ProInterface.JSON.DecodeToStr(qrs));
                return true;
            }
            catch (Exception e)
            {
                err.IsError = true;
                err.Message = e.Message;
                return false;
            }

        }

        public string GenerateCode(string csharpCode, DateTime nowDt)
        {

            csharpCode = Fun.ReplaceDataTime(csharpCode, nowDt);

            string code = @"
            using System;
            using ProServer;
            namespace ProServer
            {
                  public class ScripRun:ScriptExt
                  {
                        public bool Run()
                        {
                            try
                            {
                                _isRun=true;
                                @(csharpCode)
                                Dispose();
                                CompleteScriptTask();
                                _isRun=false;
                                return true;

                            }
                            catch(Exception err)
                            {
                                Dispose();
                                ErrorScriptTask(err);
                                _isRun=false;
                                return false;
                            }
                        }
                  }
            }";
            code = code.Replace("@(csharpCode)", csharpCode);
            return code;
        }

        public string ScriptTest(string csharpCode)
        {
            StringBuilder sb = new StringBuilder();
            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();
            // 2.ICodeComplier
            ICodeCompiler objICodeCompiler = objCSharpCodePrivoder.CreateCompiler();
            // 3.CompilerParameters
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            objCompilerParameters.ReferencedAssemblies.Add(AppDomain.CurrentDomain.BaseDirectory + "Bin\\ProServer.dll");
            objCompilerParameters.GenerateExecutable = false;
            objCompilerParameters.GenerateInMemory = true;
            // 4.CompilerResults
            var allCode = GenerateCode(csharpCode, DateTime.Now);
            CompilerResults cr = objICodeCompiler.CompileAssemblyFromSource(objCompilerParameters, allCode);
            if (cr.Errors.HasErrors)
            {
                foreach (CompilerError err in cr.Errors)
                {
                    sb.AppendLine(string.Format("行{0}列{1}：{2} <br />", err.Line - 12, err.Column, err.ErrorText));
                }
            }
            return sb.ToString();
        }




        public string ScriptRunLog(int scriptId)
        {
            using (DBEntities db = new DBEntities())
            {
                var tmp = db.YL_SCRIPT_TASK.Where(x => x.SCRIPT_ID == scriptId);
                if (tmp.Count() == 0)
                {
                    return "还没有执行";
                }
                var maxTaskId = tmp.Max(x => x.ID);
                var allLog = db.YL_SCRIPT_TASK_LOG.Where(x => x.SCRIPT_TASK_ID == maxTaskId).OrderByDescending(x => x.ID).ToList();
                return string.Join("\r\n", allLog.Select(x => x.LOG_TIME.ToString() + "  " + x.MESSAGE).ToList());
            }
        }


        public void ErrorScriptTask(string message, int taskId)
        {
            if (message.Length > 40)
            {
                message = message.Substring(0, 40);
            }
            log(message, taskId);
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("RUN_STATE", "停止");
            dic.Add("RETURN_CODE", "失败");
            dic.Add("END_TIME", DateTime.Now);
            dic.Add("DISABLE_DATE", DateTime.Now);
            dic.Add("DISABLE_REASON", message);
            FunSqlToClass.UpData<SCRIPT_TASK>(
                ConfigurationManager.AppSettings["dbType"],
                ConfigurationManager.AppSettings["dbConnSt"],
                dic,
                string.Format("where ID={0} ", taskId),
                ConfigurationManager.AppSettings["dbPrefix"]);
        }

        public void log(string message, int taskId)
        {
            if (taskId == 0)
            {
                return;
            }
            ProInterface.Models.SCRIPT_TASK_LOG ent = new SCRIPT_TASK_LOG();
            ent.SCRIPT_TASK_ID = taskId;
            ent.LOG_TIME = DateTime.Now;
            ent.MESSAGE = message;
            FunSqlToClass.Save<ProInterface.Models.SCRIPT_TASK_LOG>(ConfigurationManager.AppSettings["dbType"], ConfigurationManager.AppSettings["dbConnSt"], ent, ConfigurationManager.AppSettings["dbPrefix"]);

        }

        public bool ScriptSave(string loginKey, ref ErrorInfo err, TScript inEnt, IList<string> allPar)
        {
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return false;
            }

            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_SCRIPT.SingleOrDefault(a => a.ID == inEnt.ID);
                    bool isAdd = false;
                    if (ent == null)
                    {
                        isAdd = true;
                        ent = Fun.ClassToCopy<ProInterface.Models.TScript, YL_SCRIPT>(inEnt);
                        ent.ID = Fun.GetSeqID<YL_SCRIPT>();
                    }
                    else
                    {
                        if (!ent.BODY_TEXT.Equals(inEnt.BODY_TEXT))
                        {
                            inEnt.BODY_TEXT = string.Format("//{0}于{1}修改\r\n{2}", gu.UserName, DateTime.Now.ToString(), inEnt.BODY_TEXT);
                        }

                        ent = Fun.ClassToCopy<ProInterface.Models.TScript, YL_SCRIPT>(inEnt, ent, allPar);
                    }
                    if (!string.IsNullOrEmpty(inEnt.ScriptGroupListJosn))
                    {
                        inEnt.ScriptGroupList = JSON.EncodeToEntity<IList<SCRIPT_GROUP_LIST>>(inEnt.ScriptGroupListJosn);
                    }
                    foreach (var t in inEnt.ScriptGroupList)
                    {
                        var single = ent.YL_SCRIPT_GROUP_LIST.SingleOrDefault(x => x.SCRIPT_ID == t.SCRIPT_ID);
                        if (single == null)
                        {
                            single = Fun.ClassToCopy<ProInterface.Models.SCRIPT_GROUP_LIST, YL_SCRIPT_GROUP_LIST>(t);
                            single.GROUP_ID = ent.ID;
                            ent.YL_SCRIPT_GROUP_LIST.Add(single);
                        }
                        else
                        {
                            single.ORDER_INDEX = t.ORDER_INDEX;
                        }
                    }

                    foreach (var t in ent.YL_SCRIPT_GROUP_LIST.ToList())
                    {
                        var single = inEnt.ScriptGroupList.SingleOrDefault(x => x.SCRIPT_ID == t.SCRIPT_ID);
                        if (single == null)
                        {
                            db.YL_SCRIPT_GROUP_LIST.Remove(t);
                        }
                    }

                    if (isAdd)
                    {
                        db.YL_SCRIPT.Add(ent);
                    }
                    db.SaveChanges();
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Edit);
                    return true;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = e.Message;
                    return false;
                }
            }
        }

        public TScript ScriptSingleId(string loginKey, ref ErrorInfo err, int Id)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_SCRIPT.SingleOrDefault(x => x.ID == Id);
                var reEnt = new ProInterface.Models.TScript();
                if (ent != null)
                {
                    reEnt = Fun.ClassToCopy<YL_SCRIPT, ProInterface.Models.TScript>(ent);
                    reEnt.ScriptGroupList = Fun.ClassListToCopy<YL_SCRIPT_GROUP_LIST, ProInterface.Models.SCRIPT_GROUP_LIST>(ent.YL_SCRIPT_GROUP_LIST.OrderBy(x=>x.ORDER_INDEX).ToList());
                    foreach (var t in reEnt.ScriptGroupList.ToList())
                    {
                        var cn = db.YL_SCRIPT.SingleOrDefault(x => x.ID == t.SCRIPT_ID);
                        t.CODE = cn.CODE;
                        t.NAME = cn.NAME;
                    }
                    reEnt.ScriptGroupListJosn = JSON.DecodeToStr(reEnt.ScriptGroupList);
                }
                
                return reEnt;
            }
        }


        public bool ScriptDelete(string loginKey, ref ErrorInfo err, int id)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_SCRIPT.SingleOrDefault(a => a.ID == id);

                    foreach (var t in ent.YL_SCRIPT_GROUP_LIST.ToList())
                    {
                        db.YL_SCRIPT_GROUP_LIST.Remove(t);
                    }

                    foreach (var t in ent.YL_SCRIPT_TASK.ToList())
                    {
                        foreach (var t0 in t.YL_SCRIPT_TASK_LOG.ToList())
                        {
                            db.YL_SCRIPT_TASK_LOG.Remove(t0);
                        }
                        db.YL_SCRIPT_TASK.Remove(t);
                    }

                    db.YL_SCRIPT.Remove(ent);

                    db.SaveChanges();
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Delete);
                    return true;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = e.Message;
                    return false;
                }
            }
        }
    }
}
