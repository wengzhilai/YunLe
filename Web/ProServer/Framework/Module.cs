using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LINQExtensions;
using System.Linq.Expressions;
using System.IO;
using ProInterface;


namespace ProServer
{
    public partial class Service :IModule
    {

        /// <summary>
        /// 满足条件记录数
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="whereLambda">条件lambda表达表</param>
        /// <returns></returns>
        public int SysModuleCount(string loginKey, ref ErrorInfo err, string whereLambda)
        {

            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return 0;
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return 0;
            }
            using (DBEntities db = new DBEntities())
            {
                IList<YL_MODULE> content = new List<YL_MODULE>();
                Expression<Func<YL_MODULE, bool>> whereFunc;
                try
                {
                    if (whereLambda == null || whereLambda.Trim() == "")
                    {
                        return db.YL_MODULE.Where(x=>x.YL_ROLE.Where(y=>gu.RoleID.Contains(y.ID)).Count()>0).Count();
                    }
                    whereFunc = StringToLambda.LambdaParser.Parse<Func<YL_MODULE, bool>>(whereLambda);
                    return db.YL_MODULE.Where(x => x.YL_ROLE.Where(y => gu.RoleID.Contains(y.ID)).Count() > 0).Where(whereFunc).Count();
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
        public IList<ProInterface.Models.MODULE> SysModuleWhere(string loginKey, ref ProInterface.ErrorInfo err, int pageIndex, int pageSize, string whereLambda, string orderField, string orderBy)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }

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
                var allList = db.YL_MODULE.Where(x => x.YL_ROLE.Where(y => gu.RoleID.Contains(y.ID)).Count() > 0).AsQueryable();
                if (whereLambda != null && whereLambda != "")
                {
                    try
                    {
                        Expression<Func<YL_MODULE, bool>> whereFunc = StringToLambda.LambdaParser.Parse<Func<YL_MODULE, bool>>(whereLambda);
                        allList = allList.Where(whereFunc);
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
                return Fun.ClassListToCopy<YL_MODULE, ProInterface.Models.MODULE>(content);
            }
        }


        public bool ModuleDelete(string loginKey, ref ErrorInfo err, int id)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    GlobalUser gu = Global.GetUser(loginKey);
                    string region = gu.Region;
                    var ent = db.YL_MODULE.SingleOrDefault(a => a.ID == id);
                    ent.YL_ROLE.Clear();
                    ent.YL_USER.Clear();
                    db.YL_MODULE.Remove(ent);
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

        public ProInterface.Models.MODULE ModuleSingleCode(string loginKey, ref ProInterface.ErrorInfo err, string code)
        {

            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_MODULE.SingleOrDefault(x => x.CODE == code);
                var reEnt = new ProInterface.Models.MODULE();
                if (ent != null)
                {
                    reEnt = Fun.ClassToCopy<YL_MODULE, ProInterface.Models.MODULE>(ent);
                }
                return reEnt;
            }
        }

        /// <summary>
        /// 修改模块
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改模块</returns>
        public bool ModuleSave(string loginKey, ref ProInterface.ErrorInfo err, ProInterface.Models.TModule inEnt, IList<string> allPar)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_MODULE.SingleOrDefault(a => a.ID == inEnt.ID);
                    bool isAdd = false;
                    if (ent == null)
                    {
                        isAdd = true;
                        ent = Fun.ClassToCopy<ProInterface.Models.MODULE, YL_MODULE>(inEnt);
                        ent.ID = Fun.GetSeqID<YL_MODULE>();
                    }
                    else
                    {
                        ent = Fun.ClassToCopy<ProInterface.Models.MODULE, YL_MODULE>(inEnt, ent, allPar);
                    }
                    var allRoleId=inEnt.AllRoleIdArrStr.Split(',').Select(x=>Convert.ToInt32(x)).ToList();
                    ent.YL_ROLE.Clear();
                    ent.YL_ROLE = db.YL_ROLE.Where(x => allRoleId.Contains(x.ID)).ToList();

                    if (isAdd)
                    {
                        db.YL_MODULE.Add(ent);
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
        public ProInterface.Models.TModule ModuleSingleId(string loginKey, ref ProInterface.ErrorInfo err, int id)
        {

            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_MODULE.SingleOrDefault(x => x.ID == id);
                var reEnt = new ProInterface.Models.TModule();
                if (ent != null)
                {
                    reEnt = Fun.ClassToCopy<YL_MODULE, ProInterface.Models.TModule>(ent);
                    reEnt.AllRoleIdArrStr = string.Join(",", ent.YL_ROLE.Select(x => x.ID));
                }
                return reEnt;
            }
        }


        public IList<ProInterface.Models.IdText> ModuleByNameOrCode(string loginKey, ref ErrorInfo err, string key)
        {
             if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }

            using (DBEntities db = new DBEntities())
            {

                var allList = db.YL_MODULE.Where(x =>x.YL_ROLE.Where(y => gu.RoleID.Contains(y.ID)).Count() > 0 ).OrderBy(x=>x.YL_MODULE2.SHOW_ORDER).ToList();
                if (!string.IsNullOrEmpty(key))
                {
                    allList = allList.Where(x => x.NAME.IndexOf(key) > -1 || ( x.CODE!=null && x.CODE.IndexOf(key) > -1)).ToList();
                }
                IList<ProInterface.Models.IdText> reList = new List<ProInterface.Models.IdText>();
                foreach (var t in allList)
                {
                    ProInterface.Models.IdText tmp = new ProInterface.Models.IdText();
                    if (!string.IsNullOrEmpty(t.LOCATION))
                    {
                        tmp.id = string.Format("OpenTab('{0}','{1}','{2}')", t.LOCATION, t.NAME, t.ID);
                    }
                    tmp.text = string.Format("{1}", t.CODE, t.NAME);
                    if (t.YL_MODULE2 != null)
                    {
                        tmp.target = t.YL_MODULE2.NAME;
                    }
                    reList.Add(tmp);
                }
                return reList;
            }
        }


        public bool ModuleUserAuthority(string loginKey, ref ErrorInfo err, string url)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return false;
            }

            using (DBEntities db = new DBEntities())
            {
                url = string.Format("~{0}", url.Trim('~').Trim('&').Trim('?'));
                var allModule = db.YL_MODULE.Where(x => x.LOCATION.Equals(url)).ToList();
                if (allModule.Count() > 0)
                {
                    if (allModule.Where(x => x.YL_ROLE.Where(y => gu.RoleID.Contains(y.ID)).Count() > 0).Count()>0)
                    {
                        return true;
                    }
                    else {
                        //return AppMeunUserAuthority(loginKey, ref err, url);
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
