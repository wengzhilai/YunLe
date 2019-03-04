using ProInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface
{
    public interface IDistrict:IZ_District
    {
        /// <summary>
        /// 获取异步
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IList<IdText> DistrictGetAsyn(string id, string type = "ID");
        /// <summary>
        /// 获取所有
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IList<TreeClass> DistrictGetAll(string loginKey, ref ErrorInfo err, int id, int levelId);


        /// <summary>
        /// 获取可以直接绑定到树上的对象
        /// </summary>
        /// <param name="parent_id"></param>
        /// <returns></returns>
        IList<TreeData> DistrictTreeDataGetAll(int parent_id, string loginKey);


        /// <summary>
        /// 添加组织结构
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <returns>添加组织结构</returns>
        DISTRICT DistrictAdd(string loginKey, ref ErrorInfo err, DISTRICT inEnt);
        /// <summary>
        /// 修改组织结构
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改组织结构</returns>
        bool DistrictEdit(string loginKey, ref ErrorInfo err, DISTRICT inEnt, IList<string> allPar);

        bool DistrictUpLevelIdpath();

        /// <summary>
        /// 获取节点的所有上级节点
        /// </summary>
        /// <param name="disId"></param>
        /// <returns></returns>
        string DistrictGetLevelStr(int disId);

        /// <summary>
        /// 获取用户的层级数据
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="levelId">层级</param>
        /// <returns></returns>
        IList<System.Web.Mvc.SelectListItem> DistrictGetUserLevel(string loginKey, ref ErrorInfo err, int levelId, string districtId);

        DISTRICT DistrictGetByUser(string loginKey, ref ErrorInfo err);
    }
}
