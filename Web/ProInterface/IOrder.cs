
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
    /// 车辆内容
    /// </summary>
    public interface IOrder: IZ_Order
    {
        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">主键</param>
        /// <returns></returns>
        YlOrder OrderSingleId(string loginKey, ref ErrorInfo err, int keyId);

        /// <summary>
        /// 删除车辆
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">删除车辆</param>
        /// <returns></returns>
        bool OrderDelete(string loginKey, ref ErrorInfo err, int keyId);
    }
}
