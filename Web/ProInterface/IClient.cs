
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
    /// 客户内容
    /// </summary>
    public interface IClient: IZ_Client
    {
        /// <summary>
        /// 修改客户
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改客户</returns>
        bool ClientSave(string loginKey, ref ErrorInfo err, YlClient inEnt, IList<string> allPar);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">主键</param>
        /// <returns></returns>
        YlClient ClientSingleId(string loginKey, ref ErrorInfo err, int keyId);

        /// <summary>
        /// 删除客户
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">删除客户</param>
        /// <returns></returns>
        bool ClientDelete(string loginKey, ref ErrorInfo err, int keyId);
    }
}
