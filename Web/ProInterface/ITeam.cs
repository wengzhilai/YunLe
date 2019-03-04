
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
    /// 团队内容
    /// </summary>
    public interface ITeam : IZ_Team
    {
        /// <summary>
        /// 修改团队
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改团队</returns>
        bool TeamSave(string loginKey, ref ErrorInfo err, YL_TEAM inEnt, IList<string> allPar);
        /// <summary>
        /// 删除团队
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">删除团队</param>
        /// <returns></returns>
        bool TeamDelete(string loginKey, ref ErrorInfo err, int keyId);
    }
}
