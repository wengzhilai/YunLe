
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
    public partial class Service : IZ_ScriptTask
    {
        #region 默认方法

        /// <summary>
        /// 修改任务
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改任务</returns>
        public bool ScriptTask_Save(string loginKey, ref ProInterface.ErrorInfo err, ProInterface.Models.SCRIPT_TASK inEnt, IList<string> allPar)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_SCRIPT_TASK.SingleOrDefault(a => a.ID == inEnt.ID);
                    bool isAdd = false;
                    if (ent == null)
                    {
                        isAdd = true;
                        ent = Fun.ClassToCopy<ProInterface.Models.SCRIPT_TASK, YL_SCRIPT_TASK>(inEnt);
                    }
                    else
                    {
                        ent = Fun.ClassToCopy<ProInterface.Models.SCRIPT_TASK, YL_SCRIPT_TASK>(inEnt, ent, allPar);
                    }

                    if (isAdd)
                    {
                        db.YL_SCRIPT_TASK.Add(ent);
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
        public ProInterface.Models.SCRIPT_TASK ScriptTask_SingleId(string loginKey, ref ProInterface.ErrorInfo err, int id)
        {

            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                var ent=db.YL_SCRIPT_TASK.SingleOrDefault(x => x.ID == id);
                var reEnt = new ProInterface.Models.SCRIPT_TASK();
                if (ent != null)
                {
                    reEnt = Fun.ClassToCopy<YL_SCRIPT_TASK, ProInterface.Models.SCRIPT_TASK>(ent);
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
        public ProInterface.Models.SCRIPT_TASK ScriptTask_Single(string loginKey, ref ProInterface.ErrorInfo err, string whereLambda)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                IList<YL_SCRIPT_TASK> content = new List<YL_SCRIPT_TASK>();
                Expression<Func<YL_SCRIPT_TASK, bool>> whereFunc;
                try
                {
                    whereFunc = StringToLambda.LambdaParser.Parse<Func<YL_SCRIPT_TASK, bool>>(whereLambda);
                }
                catch
                {
                    err.IsError = true;
                    err.Message = "条件表态式有误";
                    return null;
                }
                var reEnt = db.YL_SCRIPT_TASK.Where(whereFunc).ToList();
                if (reEnt.Count>0)
                {
                    return Fun.ClassToCopy<YL_SCRIPT_TASK, ProInterface.Models.SCRIPT_TASK>(reEnt[0]);
                }
                return null;
            }
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="roleId">删除任务</param>
        /// <returns>删除任务</returns>
        public bool ScriptTask_Delete(string loginKey, ref ProInterface.ErrorInfo err, int entId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_SCRIPT_TASK.SingleOrDefault(a => a.ID == entId);
                    db.YL_SCRIPT_TASK.Remove(ent);

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
        public int ScriptTask_Count(string loginKey, ref ProInterface.ErrorInfo err, string whereLambda)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return 0;
            using (DBEntities db = new DBEntities())
            {
                IList<YL_SCRIPT_TASK> content = new List<YL_SCRIPT_TASK>();
                Expression<Func<YL_SCRIPT_TASK, bool>> whereFunc;
                try
                {
                    if (whereLambda == null || whereLambda.Trim() == "")
                    {
                        return db.YL_SCRIPT_TASK.Count();
                    }
                    whereFunc = StringToLambda.LambdaParser.Parse<Func<YL_SCRIPT_TASK, bool>>(whereLambda);
                    return db.YL_SCRIPT_TASK.Where(whereFunc).Count();
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
        public IList<ProInterface.Models.SCRIPT_TASK> ScriptTask_Where(string loginKey, ref ProInterface.ErrorInfo err, int pageIndex, int pageSize, string whereLambda, string orderField, string orderBy)
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
                var allList = db.YL_SCRIPT_TASK.AsQueryable();
                if (whereLambda != null && whereLambda != "")
                {
                    try
                    {
                        Expression<Func<YL_SCRIPT_TASK, bool>> whereFunc = StringToLambda.LambdaParser.Parse<Func<YL_SCRIPT_TASK, bool>>(whereLambda);
                        allList = db.YL_SCRIPT_TASK.Where(whereFunc);
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
                return Fun.ClassListToCopy<YL_SCRIPT_TASK, ProInterface.Models.SCRIPT_TASK>(content);
            }           
        }
        

        #endregion

    }
}
