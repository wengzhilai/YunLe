
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
    /// 提醒内容
    /// </summary>
    public interface IUserMessage
    {
        /// <summary>
        /// 获取指定类型的新消息
        /// 1、选去类型的表查找是否有新有数据，如果有，测添加提醒消息
        /// 2、返回所有该类型的消息
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="typeId">消息类型ID</param>
        /// <returns>新的消息列表</returns>
        IList<TUserMessage> UserMessageGetTypeNew(string loginKey, ref ErrorInfo err, int typeId);

        /// <summary>
        /// 标记完成指定类型
        /// 如果指定类型的主键不存在，则添加并标注为完成
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="typeId">类型ID</param>
        /// <param name="keyId">主键</param>
        /// <returns></returns>
        bool UserMessageSetTypeFinish(string loginKey, ref ErrorInfo err, int typeId, int keyId);


        /// <summary>
        /// 自动生成发送消息
        ///如果allUser不为空，则allRole的值无效
        ///如果allUser的用户ID不在districtId内则不发送
        ///如果allUser和allRole都为空则表示给districtId区域下的所有角色，所有用户推送信息
        /// </summary>
        /// <returns></returns>
        bool UserMessageSave(string loginKey, ref ErrorInfo err, ProInterface.Models.MESSAGE inEnt, IList<string> allPar, string allUserIdStr = null);

        bool UserMessageSave(string loginKey, ref ErrorInfo err, string message, string allUserIdStr, int typeId);
        /// <summary>
        /// 获取用户新消息数据
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        int UserMessageGetNewCount(string loginKey, ref ErrorInfo err);

        /// <summary>
        /// 获取所有的新消息
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <returns>新的消息列表</returns>
        IList<TUserMessage> UserMessageGetNew(string loginKey, ref ErrorInfo err);


        /// <summary>
        /// 获取所有的已阅消息
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <returns>已阅消息列表</returns>
        IList<TUserMessage> UserMessageGetOld(string loginKey, ref ErrorInfo err);

        /// <summary>
        /// 标记完成指定消息
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="messageId">消息ID</param>
        /// <returns></returns>
        bool UserMessageSetFinish(string loginKey, ref ErrorInfo err, int messageId);
    }
}
