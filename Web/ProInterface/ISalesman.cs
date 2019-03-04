
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
    /// 业务员内容
    /// </summary>
    public interface ISalesman: IZ_Salesman
    {
        /// <summary>
        /// 修改业务员
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改业务员</returns>
        bool SalesmanSave(string loginKey, ref ErrorInfo err, YlSalesman inEnt, IList<string> allPar);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">主键</param>
        /// <returns></returns>
        YlSalesman SalesmanSingleId(string loginKey, ref ErrorInfo err, int keyId);

        /// <summary>
        /// 删除业务员
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">删除业务员</param>
        /// <returns></returns>
        bool SalesmanDelete(string loginKey, ref ErrorInfo err, int keyId);
    }
}
