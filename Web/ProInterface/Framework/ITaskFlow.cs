using ProInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ProInterface
{
    public interface ITaskFlow
    {

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">主键</param>
        /// <returns></returns>
        TTaskFlow TaskFlowSingleId(string loginKey, ref ErrorInfo err, int keyId);

        /// <summary>
        /// 获取该用户层级下所有角色
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        IList<SelectListItem> TaskFlowMyAllRole(string loginKey, ref ErrorInfo err);

        /// <summary>
        /// 保存任务工单
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt"></param>
        /// <param name="allPar"></param>
        /// <returns></returns>
        bool TaskFlowSave(string loginKey, ref ProInterface.ErrorInfo err, ProInterface.Models.TTaskFlow inEnt, IList<string> allPar);

        /// <summary>
        /// 受理任务
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="taskFlowId"></param>
        /// <returns></returns>
        int TaskFlowAccept(string loginKey, ref ProInterface.ErrorInfo err, string taskFlowIdStr);
    
    }
}
