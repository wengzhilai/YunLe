using ProInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface
{
    public interface ITask
    {
        bool TaskHandle(string loginKey, ref ErrorInfo err, TTask inEnt);

        TFlow TaskGetTaskFlow(string loginKey, ref ErrorInfo err, int taskId);

        TTask TaskSingle(string loginKey, ref ErrorInfo err,int id);

        TTaskFlow TaskFlowSingle(int id);

        TNode TaskNodeSingle(string loginKey, ref ErrorInfo err, int id);

        TNodeFlow TaskNodeFlowSingle(string loginKey, ref ErrorInfo err, int id);

        bool TaskNodeSave(string loginKey, ref ErrorInfo err, TNode inEnt);

        bool SupTaskNodeSave(string loginKey, ref ErrorInfo err, TNode inEnt);
        /// <summary>
        /// 获取当前流程节点中，最近的一个指定节点的ID
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        string TaskGetLastTaskFlowShowUrl(int flowID, int nodeID);

        /// <summary>
        /// 获取任务所有节点
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="taskId">任务ID</param>
        /// <returns></returns>
        IList<TTaskFlow> TaskGetTreeFlow(string loginKey, ref ErrorInfo err, int taskId);

        #region 默认接口

        /// <summary>
        /// 修改任务
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改任务</returns>
        bool TaskSave(string loginKey, ref ErrorInfo err, TTask inEnt, IList<string> allPar);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">主键</param>
        /// <returns></returns>
        TASK TaskSingleId(string loginKey, ref ErrorInfo err, int keyId);

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">删除任务</param>
        /// <returns></returns>
        bool TaskDelete(string loginKey, ref ErrorInfo err, int keyId);

        #endregion

    }
}
