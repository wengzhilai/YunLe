using ProInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface
{
    public interface IDataDown : IZ_DataDown
    {
        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">主键</param>
        /// <returns></returns>
        DataDown DataDownSingleId(string loginKey, ref ErrorInfo err, int keyId);

        /// <summary>
        /// 获取SQL语句的字段
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        IList<TableFiled> DataDowGetSqlFilde(ref ErrorInfo err, string sql, int serverID);

        /// <summary>
        /// 修改数据下载
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改数据下载</returns>
        bool DataDownSave(string loginKey, ref ErrorInfo err, DataDown inEnt, IList<string> allPar);


        /// <summary>
        /// 启动下载
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="downID"></param>
        /// <returns></returns>
        bool DataDownEventStart(string loginKey, ref ErrorInfo err, int downID);

    }
}
