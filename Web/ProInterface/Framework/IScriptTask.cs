
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProInterface.Models;

namespace ProInterface
{
    /// <summary>
    /// 任务内容
    /// </summary>
    public interface IScriptTask : IZ_ScriptTask
    {
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">删除任务</param>
        /// <returns></returns>
        bool ScriptTaskReStart(string loginKey, ref ErrorInfo err, int keyId);
        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="keyId"></param>
        /// <returns></returns>
        bool ScriptTaskCancel(string loginKey, ref ErrorInfo err, int keyId);
        /// <summary>
        /// 查看日志
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="keyId"></param>
        /// <returns></returns>
        string ScriptTaskLookLog(string loginKey, ref ErrorInfo err, int keyId);
        /// <summary>
        /// 查看脚本
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="keyId"></param>
        /// <returns></returns>
        string ScriptTaskLookScript(string loginKey, ref ErrorInfo err, int keyId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="scriptId"></param>
        /// <returns></returns>
        bool ScriptTaskAdd(string loginKey, ref ErrorInfo err, int scriptId, int? groupId = null);

    }
}
