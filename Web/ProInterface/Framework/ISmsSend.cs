using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface
{
    public interface ISmsSend : IZ_SmsSend
    {
        /// <summary>
        /// 添加发送内容到手机
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="phone"></param>
        /// <param name="conten"></param>
        /// <returns></returns>
        bool SmsSendAdd(string loginKey, ref ErrorInfo err, string phone, string conten, int? messageId);
    }
}
