
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
    /// 维护站内容
    /// </summary>
    public interface IGarage : IZ_Garage
    {
        /// <summary>
        /// 修改维护站
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改维护站</returns>
        bool GarageSave(string loginKey, ref ErrorInfo err, YlGarage inEnt, IList<string> allPar);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">主键</param>
        /// <returns></returns>
        YlGarage GarageSingleId(string loginKey, ref ErrorInfo err, int keyId);


        YlSalesman GarageUserSingleId(string loginKey, ref ErrorInfo err, int keyId);

        YlSalesman GarageUserSave(string loginKey, ref ErrorInfo err, YlSalesman inEnt, IList<string> allPar);

        IList<YlGarage> GarageFindAllMy(string loginKey, ref ErrorInfo err);

        /// <summary>
        /// 删除维护站用户
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">删除维护站</param>
        /// <returns></returns>
        bool GarageUserDelete(string loginKey, ref ErrorInfo err, int keyId);
    }
}
