
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
    /// 微信粉丝内容
    /// </summary>
    public interface IZ_WeixinUser
    {
        #region 默认接口

        /// <summary>
        /// 查找所有
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <returns></returns>
        IList<YL_WEIXIN_USER> WeixinUser_FindAll(string loginKey, ref ErrorInfo err);

        /// <summary>
        /// 修改微信粉丝
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改微信粉丝</returns>
        bool WeixinUser_Save(string loginKey, ref ErrorInfo err, YL_WEIXIN_USER inEnt, IList<string> allPar);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">主键</param>
        /// <returns></returns>
        YL_WEIXIN_USER WeixinUser_SingleId(string loginKey, ref ErrorInfo err, string keyId);

        /// <summary>
        /// 删除微信粉丝
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">删除微信粉丝</param>
        /// <returns></returns>
        bool WeixinUser_Delete(string loginKey, ref ErrorInfo err, string keyId);
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="whereLambda">条件lambda表达表</param>
        /// <param name="orderField">排序</param>
        /// <param name="orderBy">排序方式</param>
        /// <returns>返回满足条件的泛型</returns>
        IList<YL_WEIXIN_USER> WeixinUser_Where(string loginKey, ref ErrorInfo err, int pageIndex, int pageSize, string whereLambda, string orderField, string orderBy);

        /// <summary>
        /// 满足条件记录数
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="whereLambda">条件lambda表达表</param>
        /// <returns></returns>
        int WeixinUser_Count(string loginKey, ref ErrorInfo err, string whereLambda);

        #endregion
    }
}
