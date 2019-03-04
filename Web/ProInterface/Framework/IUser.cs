using ProInterface.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ProInterface
{
    /// <summary>
    /// 用户
    /// </summary>
    public interface IUser : IZ_User
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="err"></param>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <param name="loginIP"></param>
        /// <returns></returns>
        object UserLogin(ref ErrorInfo err, string loginName, string password, string loginIP);
      
        /// <summary>
        /// 加密方式用户登录
        /// </summary>
        /// <param name="err"></param>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <param name="loginIP"></param>
        /// <returns></returns>
        object UserLogin(ref ErrorInfo err, TLogin ent, string loginIP);

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="err"></param>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <param name="loginIP"></param>
        /// <returns></returns>
        object UserLogin(ref ErrorInfo err, string loginName, string loginIP);

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="err"></param>
        /// <param name="loginKey"></param>
        /// <param name="loginName"></param>
        /// <param name="oldPwd"></param>
        /// <param name="newPwd"></param>
        /// <returns></returns>
        bool UserResetPwd(ref ErrorInfo err, string loginKey, string loginName, string oldPwd, string newPwd);

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        bool UserLoginOut(string loginKey, ref ErrorInfo err);

        /// <summary>
        /// 保存用户
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="ent"></param>
        /// <returns></returns>
        object UserAdd(string loginKey, ref ErrorInfo err, TUser ent);

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <returns>修改用户</returns>
        bool UserEdit(string loginKey, ref ErrorInfo err, TUser inEnt);

        /// <summary>
        /// 修改系统用户
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改系统用户</returns>
        TUser UserSave(string loginKey, ref ErrorInfo err, TUser inEnt, IList<string> allPar);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">主键</param>
        /// <returns></returns>
        bool UserDelete(string loginKey, ref ErrorInfo err, int keyId);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">主键</param>
        /// <returns></returns>
        TUser UserSingleId(string loginKey, ref ErrorInfo err, int keyId);

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        TUser UserGetNow(string loginKey, ref ErrorInfo err);


        /// <summary>
        /// 设置用户快速通道模块
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="allModuleList">设置模块</param>
        /// <returns></returns>
        bool UserSetUserModule(string loginKey, ref ErrorInfo err, IList<int> allModuleList);

        /// <summary>
        /// 获取用户快速通道模块
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        IList<MODULE> UserGetUserModule(string loginKey, ref ErrorInfo err);

        /// <summary>
        /// 获取当前用户角色
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        TRole UserGetRole(string loginKey, ref ErrorInfo err);

        /// <summary>
        /// 根据登录名，获取电话号码
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="err">错误信息</param>
        /// <returns></returns>
        string UserGetPhone(ref ErrorInfo err, string loginName);
        /// <summary>
        /// 用户名和密码验证
        /// </summary>
        /// <param name="err"></param>
        /// <param name="entlogin"></param>
        /// <returns></returns>
        string UserGetPhone(ref ErrorInfo err, TLogin entlogin);
        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="err">错误信息</param>
        /// <returns></returns>
        bool UserSendVerifyCode(ref ErrorInfo err, string loginName);
        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="err"></param>
        /// <param name="entlogin"></param>
        /// <returns></returns>
        bool UserSendVerifyCode(ref ErrorInfo err, TLogin entlogin);
        /// <summary>
        /// 根据
        /// </summary>
        /// <param name="err">错误信息</param>
        /// <param name="loginName">登录名</param>
        /// <param name="VerifyCode">验证码</param>
        /// <param name="newPwd">新密码</param>
        /// <returns></returns>
        bool UserResetPwdByVerifyCode(ref ErrorInfo err, string loginName, string VerifyCode, string newPwd);

        /// <summary>
        /// 获取用户树，用于绑定easyui tree上
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="roleIdStr">角色</param>
        /// <returns></returns>
        IList<TreeClass> UserGetTree(string loginKey, ref ErrorInfo err, string roleIdStr, int level = 1);
        /// <summary>
        /// 本用户下 获取角色下所有用户
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="roleIdStr"></param>
        /// <returns></returns>
        IList<TreeClass> UserGetBylRole(string loginKey, ref ErrorInfo err, string roleIdStr);

        /// <summary>
        /// 批量更新用户锁定状态
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="userIdStr"></param>
        /// <param name="lockVal"></param>
        /// <returns></returns>
        int UserUpdateLocked(string loginKey, ref ErrorInfo err, string userIdStr, short lockVal);

        /// <summary>
        /// 本用户下 获取角色下所有用户
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="roleIdStr">角色</param>
        /// <param name="districtId">区域</param>
        /// <returns></returns>
        IList<TreeClass> UserGetBylRoleDistrict(string loginKey, ref ErrorInfo err, string roleIdStr, string districtId);

        /// <summary>
        /// 根据userId获取用户
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="userIdStr"></param>
        /// <returns></returns>
        IList<TreeClass> UserGetBylUserId(string loginKey, ref ErrorInfo err, string userIdStr);

        /// <summary>
        /// 获取用户所有上级用户ID
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        IList<int> UserGetMasterUser(string loginKey, ref ErrorInfo err, List<int> allRoleIdList);




        
    }
}
