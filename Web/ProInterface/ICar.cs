
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
    public interface ICar: IZ_Car
    {
        /// <summary>
        /// 修改车辆
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <returns>修改车辆</returns>
        YL_CAR CarGetAndSave(string loginKey, ref ErrorInfo err, YL_CAR inEnt, int userId);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">主键</param>
        /// <returns></returns>
        YlCar CarSingleId(string loginKey, ref ErrorInfo err, int keyId);
    }
}
