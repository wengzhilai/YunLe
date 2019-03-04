
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
    /// 订单流程内容
    /// </summary>
    public interface IOrderFlow: IZ_OrderFlow
    {
        /// <summary>
        /// 修改订单流程
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改订单流程</returns>
        bool OrderFlowSave(string loginKey, ref ErrorInfo err, YL_ORDER_FLOW inEnt, IList<string> allPar);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">主键</param>
        /// <returns></returns>
        YL_ORDER_FLOW OrderFlowSingleId(string loginKey, ref ErrorInfo err, string keyId, int orderId);
        /// <summary>
        /// 获取定单待支付费用
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        YL_ORDER_FLOW OrderFlowGetWaitPay(string loginKey, ref ErrorInfo err, int orderId);


        /// <summary>
        /// 获取定单待支付费用
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        YL_ORDER_FLOW OrderFlowSavePay(string loginKey, ref ErrorInfo err, string out_trade_no,string cash_fee,string transaction_id,string openid);
    }
}
