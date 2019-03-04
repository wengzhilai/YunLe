
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
    /// 费用明细内容
    /// </summary>
    public interface ICostlist: IZ_Costlist
    {
        /// <summary>
        /// 修改费用明细
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改费用明细</returns>
        bool CostlistSave(string loginKey, ref ErrorInfo err, YL_COSTLIST inEnt, IList<string> allPar);

        bool CostlistSaveByPay(ref ErrorInfo err, YL_COSTLIST inEnt);

    }
}
