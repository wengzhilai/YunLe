using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LINQExtensions;
using System.Linq.Expressions;
using System.IO;
using ProInterface;
using ProInterface.Models;
using System.Data.Entity.Validation;
using System.Web.Mvc;
using ProInterface.Models.Api;


namespace ProServer
{
    public partial class Service : IBulletin
    {

        public object BulletinSave(string loginKey, ref ErrorInfo err, ProInterface.Models.TBulletin inEnt, IList<string> allPar)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                GlobalUser gu = Global.GetUser(loginKey);
                inEnt.AllFiles = JSON.EncodeToEntity<IList<FILES>>(inEnt.AllFilesStr == null ? "[]" : inEnt.AllFilesStr);
                IList<int> fileIdList = inEnt.AllFiles.Select(x => x.ID).ToList();
                if (string.IsNullOrEmpty(inEnt.AllRoleId))
                {
                    err.IsError = true;
                    err.Message = string.Format("保存失败，没有选择可查看的角色", inEnt.AllRoleId);
                    return null;
                }
                IList<int> AllRoleId = inEnt.AllRoleId.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                YL_BULLETIN reEnt = new YL_BULLETIN();
                if (inEnt.ID == 0)
                {
                    reEnt = Fun.ClassToCopy<ProInterface.Models.BULLETIN, YL_BULLETIN>(inEnt);
                    reEnt.ID = Fun.GetSeqID<YL_BULLETIN>();
                    reEnt.YL_FILES = db.YL_FILES.Where(x => fileIdList.Contains(x.ID)).ToList();
                    reEnt.YL_ROLE = db.YL_ROLE.Where(x => AllRoleId.Contains(x.ID)).ToList();
                    reEnt.PUBLISHER = gu.UserName;
                    reEnt.CREATE_TIME = DateTime.Now;
                    reEnt.UPDATE_TIME = DateTime.Now;
                    reEnt.REGION = gu.Region;
                    reEnt.USER_ID = gu.UserId;
                    reEnt = db.YL_BULLETIN.Add(reEnt);
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Add);
                }
                else
                {
                    reEnt = db.YL_BULLETIN.SingleOrDefault(x => x.ID == inEnt.ID);

                    if (reEnt.USER_ID != gu.UserId)
                    {
                        err.IsError = true;
                        err.Message = string.Format("该公告是【{0}】添加的，不能修改", reEnt.PUBLISHER);
                        return null;
                    }



                    reEnt = Fun.ClassToCopy<ProInterface.Models.BULLETIN, YL_BULLETIN>(inEnt, reEnt, allPar);
                    var allNowFiles = db.YL_FILES.Where(x => fileIdList.Contains(x.ID)).ToList();
                    foreach (var t in reEnt.YL_FILES.ToList())
                    {
                        if (allNowFiles.SingleOrDefault(x => x.ID == t.ID) == null)
                        {
                            reEnt.YL_FILES.Remove(t);
                            db.YL_FILES.Remove(t);
                        }
                    }

                    reEnt.YL_FILES.Clear();
                    reEnt.YL_ROLE.Clear();
                    reEnt.YL_ROLE = db.YL_ROLE.Where(x => AllRoleId.Contains(x.ID)).ToList();
                    reEnt.YL_FILES = allNowFiles;
                    reEnt.PUBLISHER = gu.UserName;
                    reEnt.UPDATE_TIME = DateTime.Now;
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Edit);
                }
                try
                {
                    //FunTask.StartTask(db, ref err, 1, "阅读公告信息", gu);
                    db.SaveChanges();
                    return reEnt.ID;
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    err.Excep = e;
                    return null;
                }
            }
        }

        public ProInterface.Models.TBulletin BulletinSingle(string loginKey, ref ErrorInfo err, int? bullID)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                TBulletin reEnt = new TBulletin();

                YL_BULLETIN ent = db.YL_BULLETIN.SingleOrDefault(x => x.ID == bullID);
                GlobalUser gu = Global.GetUser(loginKey);
                if (ent != null)
                {
                    db.YL_BULLETIN_LOG.Add(new YL_BULLETIN_LOG
                    {
                        BULLETIN_ID = ent.ID,
                        LOOK_TIME = DateTime.Now,
                        USER_ID = gu.UserId
                    });
                    db.SaveChanges();
                    reEnt = Fun.ClassToCopy<YL_BULLETIN, TBulletin>(ent);
                    reEnt.AllFiles = Fun.ClassListToCopy<YL_FILES, FILES>(ent.YL_FILES.ToArray());
                    reEnt.AllFilesStr = JSON.DecodeToStr(reEnt.AllFiles);
                    reEnt.AllRoleId = string.Join(",", ent.YL_ROLE.Select(x => x.ID).ToList());
                    var user=db.YL_USER.SingleOrDefault(x=>x.ID==ent.USER_ID);
                    if(user!=null)
                    {
                        reEnt.DistrictName = user.YL_DISTRICT.NAME;
                    }

                    foreach (var t in ent.YL_BULLETIN_REVIEW.OrderBy(x => x.ADD_TIME).ToList())
                    {
                        var tmp = Fun.ClassToCopy<YL_BULLETIN_REVIEW, ProInterface.Models.BulletinReview>(t);
                        var userTmp = db.YL_USER.SingleOrDefault(x => x.ID == t.USER_ID);
                        if (userTmp != null)
                        {
                            tmp.UserName = userTmp.NAME;
                            tmp.DistictName = userTmp.YL_DISTRICT.NAME;
                            tmp.UserPhone = userTmp.LOGIN_NAME;
                            tmp.UserRole = string.Join(",", userTmp.YL_ROLE.Select(x => x.NAME).ToList());
                        }
                        reEnt.AllChildrenItem.Add(tmp);
                    }
                }
                else
                {
                    reEnt.IS_URGENT = 1;
                    reEnt.IS_IMPORT = 1;
                    reEnt.IS_SHOW = 1;
                    reEnt.REGION = gu.DistrictId.ToString();
                }
                return reEnt;
            }
        }


        /// <summary>
        /// 删除公告
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="keyId">删除公告</param>
        /// <returns>删除公告</returns>
        public bool BulletinDelete(string loginKey, ref ProInterface.ErrorInfo err, int keyId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_BULLETIN.SingleOrDefault(a => a.ID == keyId);
                    ent.YL_FILES.Clear();
                    ent.YL_ROLE.Clear();

                    foreach (var t in ent.YL_BULLETIN_REVIEW.ToList())
                    {
                        db.YL_BULLETIN_REVIEW.Remove(t);
                    }

                    foreach (var t in ent.YL_BULLETIN_LOG.ToList())
                    {
                        db.YL_BULLETIN_LOG.Remove(t);
                    }

                    db.YL_BULLETIN.Remove(ent);

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


        public IList<TBulletin> BulletinGetNew(string loginKey, ref ErrorInfo err)
        {
            using (DBEntities db = new DBEntities())
            {
                var tmpEnt = db.YL_BULLETIN.AsEnumerable();
                GlobalUser gu = Global.GetUser(loginKey);
                if (gu != null)
                {
                    var dis = db.YL_USER.SingleOrDefault(x => x.ID == gu.UserId).YL_DISTRICT;
                    var id_path = dis.ID_PATH + dis.ID + ".";
                    tmpEnt = tmpEnt.Where(x => id_path.IndexOf("." + x.REGION + ".") > -1).AsEnumerable();
                }

                var entList = tmpEnt.Where(x => x.IS_SHOW == 1 && x.ISSUE_DATE < DateTime.Now).OrderByDescending(x => x.IS_URGENT).ThenByDescending(x => x.ISSUE_DATE).ToList();
                IList<TBulletin> reEntList = new List<TBulletin>();
                foreach (var t in entList)
                {
                    var tmp = Fun.ClassToCopy<YL_BULLETIN, TBulletin>(t);
                    tmp.AllFiles = Fun.ClassListToCopy<YL_FILES, FILES>(t.YL_FILES.ToList());
                    reEntList.Add(tmp);
                }
                return reEntList.ToList();
            }
        }

        /// <summary>
        /// 置顶
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="id"></param>
        /// <returns></returns>

        public bool SetTop(string loginKey, ref ErrorInfo err, int id, bool istop)
        {
            try
            {
                using (DBEntities db = new DBEntities())
                {
                    var bu = db.YL_BULLETIN.Where(k => k.ID == id);
                    if (bu.Any())
                    {
                        foreach (var b in bu)
                        {
                            b.IS_URGENT = istop == true ? Convert.ToInt16(1) : Convert.ToInt16(0);
                            b.UPDATE_TIME = DateTime.Now;
                        }
                    }
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }



        /// <summary>
        /// 重要
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="err"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool SetImport(string loginKey, ref ErrorInfo err, int id, bool isimport)
        {
            try
            {
                using (DBEntities db = new DBEntities())
                {
                    var bu = db.YL_BULLETIN.Where(k => k.ID == id);
                    if (bu.Any())
                    {
                        foreach (var b in bu)
                        {
                            b.IS_IMPORT = isimport == true ? Convert.ToInt16(1) : Convert.ToInt16(0);
                            b.UPDATE_TIME = DateTime.Now;
                        }
                    }
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public TBulletin BulletinSingleByTitle(string loginKey, ref ErrorInfo err, string title)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                TBulletin reEnt = new TBulletin();
                YL_BULLETIN ent = db.YL_BULLETIN.FirstOrDefault(x => x.TITLE == title);
                if (ent == null)
                {
                    reEnt.TITLE = title;
                    reEnt.ISSUE_DATE = DateTime.Now;
                    reEnt.IS_SHOW = 0;
                    reEnt.IS_URGENT = 0;
                }
                else
                {
                    reEnt = Fun.ClassToCopy<YL_BULLETIN, TBulletin>(ent);
                    reEnt.AllFiles = Fun.ClassListToCopy<YL_FILES, FILES>(ent.YL_FILES.ToArray());
                    reEnt.AllFilesStr = JSON.DecodeToStr(reEnt.AllFiles);
                }
                return reEnt;
            }
        }


        public IList<System.Web.Mvc.SelectListItem> BulletinType()
        {
            using (DBEntities db = new DBEntities())
            {
                List<SelectListItem> bulletionTypes = new List<SelectListItem>();
                bulletionTypes.Add(new SelectListItem { Text = "全部", Value = "全部" });
                foreach (var t in db.YL_BULLETIN_TYPE.ToList())
                {
                    
                    bulletionTypes.Add(new SelectListItem { Text = t.NAME, Value = t.NAME });
                    
                }
                return bulletionTypes;
              // return db.YL_BULLETIN_TYPE.ToList().Select(x => new SelectListItem { Value = x.NAME, Text = x.NAME }).ToList();
            }
        }


        public ProInterface.Models.Api.ApiPagingDataBean BulletinList(ref ErrorInfo err, ProInterface.Models.Api.ApiRequesPageBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            ApiPagingDataBean reEnt = new ApiPagingDataBean();
            int skip = 0;
            if (inEnt.currentPage > 1)
            {
                skip = (inEnt.currentPage - 1) * inEnt.pageSize;
            }
            using (DBEntities db = new DBEntities())
            {

                var tmpEnt = db.YL_BULLETIN.AsEnumerable();
                var allData = tmpEnt.Where(x => x.IS_SHOW == 1 && x.ISSUE_DATE < DateTime.Now).OrderByDescending(x => x.IS_URGENT).ThenByDescending(x => x.ISSUE_DATE).AsEnumerable();
                allData = allData.Where(x => x.YL_ROLE.Where(y => gu.RoleID.Contains(y.ID)).Count() > 0).AsEnumerable();

                ErrorInfo error = new ErrorInfo();
                #region 过虑条件
                foreach (var filter in inEnt.searchKey)
                {
                    if (filter.K == "CREATE_TIME" && filter.T==">=" && !string.IsNullOrEmpty(filter.V))
                    {
                        DateTime dt1 = DateTime.ParseExact(filter.V, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
                        allData = allData.Where(x => x.CREATE_TIME >= dt1).AsEnumerable();
                    }
                    else if (filter.K == "CREATE_TIME" && filter.T == "<=" && !string.IsNullOrEmpty(filter.V))
                    {
                        DateTime dt2 = DateTime.ParseExact(filter.V, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
                        allData = allData.Where(x => x.CREATE_TIME <= dt2).AsEnumerable();
                    }else if(filter.V == "全部")
                    {
                         allData = tmpEnt.Where(x => x.IS_SHOW == 1 && x.ISSUE_DATE < DateTime.Now).OrderByDescending(x => x.IS_URGENT).ThenByDescending(x => x.ISSUE_DATE).AsEnumerable();
                        
                    }
                    else
                    {
                        allData = Fun.GetListWhere<YL_BULLETIN>(allData, filter.K, filter.T, filter.V, ref error);
                    }
                }
                #endregion
                var allList = allData.Skip(skip).Take(inEnt.pageSize).ToList();
                reEnt.currentPage = inEnt.currentPage;
                reEnt.pageSize = inEnt.pageSize;
                reEnt.totalCount = allData.Count();
                reEnt.totalPage = reEnt.totalCount / reEnt.pageSize;
                if (reEnt.totalCount % reEnt.pageSize != 0) reEnt.totalPage++;
                IList<TBulletin> reList = new List<TBulletin>();
                foreach (var t in allList)
                {
                    TBulletin tmp = Fun.ClassToCopy<YL_BULLETIN, TBulletin>(t);
                    tmp.CONTENT = Fun.NoHTML(tmp.CONTENT);
                    reList.Add(tmp);
                }
                reEnt.data = reList;
            }
            return reEnt;
        }


        public IList<SelectListItem> BulletinByTypeCode(string loginKey, ref ErrorInfo err, string typeCode)
        {
            using (DBEntities db = new DBEntities())
            {
                List<SelectListItem> bulletionTypes = new List<SelectListItem>();
                foreach (var t in db.YL_BULLETIN.Where(x => x.TYPE_CODE == typeCode).ToList())
                {
                    bulletionTypes.Add(new SelectListItem { Text = t.TITLE, Value = t.ID.ToString() });

                }
                return bulletionTypes;
                // return db.YL_BULLETIN_TYPE.ToList().Select(x => new SelectListItem { Value = x.NAME, Text = x.NAME }).ToList();
            }
        }
    }
}
