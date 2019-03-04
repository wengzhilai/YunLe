
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects;
using System.Linq;
using System.Reflection;
using System.Text;
using ProInterface.Models;
using ProInterface;
using System.Linq.Expressions;
using LINQExtensions;

namespace ProServer
{
    public partial class Service : IZ_DbServerType
    {
        #region 默认方法

        /// <summary>
        /// 修改服务器分类
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改服务器分类</returns>
        public bool DbServerType_Save(string loginKey, ref ProInterface.ErrorInfo err, ProInterface.Models.DB_SERVER_TYPE inEnt, IList<string> allPar)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_DB_SERVER_TYPE.SingleOrDefault(a => a.ID == inEnt.ID);
                    bool isAdd = false;
                    if (ent == null)
                    {
                        isAdd = true;
                        ent = Fun.ClassToCopy<ProInterface.Models.DB_SERVER_TYPE, YL_DB_SERVER_TYPE>(inEnt);
                    }
                    else
                    {
                        ent = Fun.ClassToCopy<ProInterface.Models.DB_SERVER_TYPE, YL_DB_SERVER_TYPE>(inEnt, ent, allPar);
                    }

                    if (isAdd)
                    {
                        db.YL_DB_SERVER_TYPE.Add(ent);
                    }
                    db.SaveChanges();
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Edit);
                    return true;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = e.Message;
                    return false;
                }
            }
        }

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="id">条件lambda表达表</param>
        /// <returns>查询一条</returns>
        public ProInterface.Models.DB_SERVER_TYPE DbServerType_SingleId(string loginKey, ref ProInterface.ErrorInfo err, int id)
        {

            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                var ent=db.YL_DB_SERVER_TYPE.SingleOrDefault(x => x.ID == id);
                var reEnt = new ProInterface.Models.DB_SERVER_TYPE();
                if (ent != null)
                {
                    reEnt = Fun.ClassToCopy<YL_DB_SERVER_TYPE, ProInterface.Models.DB_SERVER_TYPE>(ent);
                }
                return reEnt;
            }
        }

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="whereLambda">条件lambda表达表</param>
        /// <returns>查询一条</returns>
        public ProInterface.Models.DB_SERVER_TYPE DbServerType_Single(string loginKey, ref ProInterface.ErrorInfo err, string whereLambda)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                IList<YL_DB_SERVER_TYPE> content = new List<YL_DB_SERVER_TYPE>();
                Expression<Func<YL_DB_SERVER_TYPE, bool>> whereFunc;
                try
                {
                    whereFunc = StringToLambda.LambdaParser.Parse<Func<YL_DB_SERVER_TYPE, bool>>(whereLambda);
                }
                catch
                {
                    err.IsError = true;
                    err.Message = "条件表态式有误";
                    return null;
                }
                var reEnt = db.YL_DB_SERVER_TYPE.Where(whereFunc).ToList();
                if (reEnt.Count>0)
                {
                    return Fun.ClassToCopy<YL_DB_SERVER_TYPE, ProInterface.Models.DB_SERVER_TYPE>(reEnt[0]);
                }
                return null;
            }
        }

        /// <summary>
        /// 删除服务器分类
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="roleId">删除服务器分类</param>
        /// <returns>删除服务器分类</returns>
        public bool DbServerType_Delete(string loginKey, ref ProInterface.ErrorInfo err, int entId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_DB_SERVER_TYPE.SingleOrDefault(a => a.ID == entId);
                    db.YL_DB_SERVER_TYPE.Remove(ent);

                    db.SaveChanges();
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Delete);
                    return true;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = e.Message;
                    return false;
                }
            }
        }
        /// <summary>
        /// 满足条件记录数
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="whereLambda">条件lambda表达表</param>
        /// <returns></returns>
        public int DbServerType_Count(string loginKey, ref ProInterface.ErrorInfo err, string whereLambda)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return 0;
            using (DBEntities db = new DBEntities())
            {
                IList<YL_DB_SERVER_TYPE> content = new List<YL_DB_SERVER_TYPE>();
                Expression<Func<YL_DB_SERVER_TYPE, bool>> whereFunc;
                try
                {
                    if (whereLambda == null || whereLambda.Trim() == "")
                    {
                        return db.YL_DB_SERVER_TYPE.Count();
                    }
                    whereFunc = StringToLambda.LambdaParser.Parse<Func<YL_DB_SERVER_TYPE, bool>>(whereLambda);
                    return db.YL_DB_SERVER_TYPE.Where(whereFunc).Count();
                }
                catch
                {
                    err.IsError = true;
                    err.Message = "条件表态式有误";
                    return 0;
                }
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="whereLambda">条件lambda表达表</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderBy">排序方式</param>
        /// <returns>返回满足条件的泛型</returns>
        public IList<ProInterface.Models.DB_SERVER_TYPE> DbServerType_Where(string loginKey, ref ProInterface.ErrorInfo err, int pageIndex, int pageSize, string whereLambda, string orderField, string orderBy)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 1;
            int skipCount = (pageIndex - 1) * pageSize;
            if (orderField == null || orderField == "")
            {
                err.IsError = true;
                err.Message = "排序表态式不能为空";
                return null;
            }
            using (DBEntities db = new DBEntities())
            {
                var allList = db.YL_DB_SERVER_TYPE.AsQueryable();
                if (whereLambda != null && whereLambda != "")
                {
                    try
                    {
                        Expression<Func<YL_DB_SERVER_TYPE, bool>> whereFunc = StringToLambda.LambdaParser.Parse<Func<YL_DB_SERVER_TYPE, bool>>(whereLambda);
                        allList = db.YL_DB_SERVER_TYPE.Where(whereFunc);
                    }
                    catch
                    {
                        err.IsError = true;
                        err.Message = "条件表态式有误";
                        return null;
                    }
                }

                if (orderBy == "asc")
                {
                    allList = StringFieldNameSortingSupport.OrderBy(allList, orderField);
                }
                else
                {
                    allList = StringFieldNameSortingSupport.OrderByDescending(allList, orderField);
                }

                var content = allList.Skip(skipCount).Take(pageSize).ToList();
                return Fun.ClassListToCopy<YL_DB_SERVER_TYPE, ProInterface.Models.DB_SERVER_TYPE>(content);
            }           
        }
        

        #endregion

    }
}
