
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
    /// 投保内容
    /// </summary>
    public interface IOrderInsure: IZ_OrderInsure
    {
        /// <summary>
        /// 修改投保
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改投保</returns>
        bool OrderInsureSave(string loginKey, ref ErrorInfo err, YlOrderInsure inEnt, IList<string> allPar);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">主键</param>
        /// <returns></returns>
        YlOrderInsure OrderInsureSingleId(string loginKey, ref ErrorInfo err, int keyId);

        /// <summary>
        /// 从Excle里导入保单资料
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="excelId"></param>
        /// <returns></returns>
        int OrderInsureAddFromExcel(string loginKey, ref ErrorInfo err, int excelId);

    }
}
