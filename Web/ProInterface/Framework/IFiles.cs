using ProInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface
{
    public interface IFiles:IZ_Files
    {
        /// <summary>
        /// 添加文件表
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <returns>添加文件表</returns>
        FILES FilesAdd(string loginKey, ref ErrorInfo err, FILES inEnt);

        /// <summary>
        /// 删除文件表,和物理文件
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="id">删除文件表</param>
        /// <returns></returns>
        bool FilesDelete(string loginKey, ref ErrorInfo err, int id);

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="id">条件lambda表达表</param>
        /// <returns></returns>
        FILES FilesSingle(string loginKey, ref ErrorInfo err, int id);
    }
}
