
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
    /// 救援和保养内容
    /// </summary>
    public interface IOrderRescue: IZ_OrderRescue
    {
        /// <summary>
        /// 修改救援和保养
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改救援和保养</returns>
        bool OrderRescueSave(string loginKey, ref ErrorInfo err, YlOrderRescue inEnt, IList<string> allPar);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">主键</param>
        /// <returns></returns>
        YlOrderRescue OrderRescueSingleId(string loginKey, ref ErrorInfo err, int keyId);

        /// <summary>
        /// 删除救援和保养
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">删除救援和保养</param>
        /// <returns></returns>
        bool OrderRescueDelete(string loginKey, ref ErrorInfo err, int keyId);
    }
}
