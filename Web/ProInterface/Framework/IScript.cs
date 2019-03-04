using ProInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface
{
    public interface IScript:IZ_Script
    {

        /// <summary>
        /// 修改口径脚本
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改口径脚本</returns>
        bool ScriptSave(string loginKey, ref ErrorInfo err, TScript inEnt, IList<string> allPar);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="Id">条件lambda表达表</param>
        /// <returns></returns>
        TScript ScriptSingleId(string loginKey, ref ErrorInfo err, int Id);


        /// <summary>
        /// 删除口径脚本
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="id">删除口径脚本</param>
        /// <returns></returns>
        bool ScriptDelete(string loginKey, ref ErrorInfo err, int id);

        /// <summary>
        /// 启动口径
        /// </summary>
        /// <returns></returns>
        //bool ScriptStart(ref ErrorInfo err, ref Dictionary<int, object> runStatus, int scriptID, string defaultDb, string csharpCode);
        bool ScriptTaskStart(ref ErrorInfo error, int scriptTaskID);

        /// <summary>
        /// 取消执行
        /// </summary>
        /// <returns></returns>
        bool ScriptCancel(ref ErrorInfo err,  int scriptID);

        /// <summary>
        /// 检测语法
        /// </summary>
        /// <param name="csharpCode"></param>
        /// <returns></returns>
        string ScriptTest(string csharpCode);

        /// <summary>
        /// 获取最近一次的执行日志
        /// </summary>
        /// <param name="scriptId"></param>
        /// <returns></returns>
        string ScriptRunLog(int scriptId);
    }
}
