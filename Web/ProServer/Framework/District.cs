
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using ProInterface.Models;
using ProInterface;
using System.Linq.Expressions;
using LINQExtensions;
using System.Data;
using System.Data.SqlClient;
using System.Data.Objects;
namespace ProServer
{
    public partial class Service : IDistrict
    {

        public IList<IdText> DistrictGetAsyn(string key,string type="ID")
        {
            IList<YL_DISTRICT> reEntList = new List<YL_DISTRICT>();
            using (DBEntities db = new DBEntities())
            {
                if (string.IsNullOrEmpty(key) || key=="0")
                {
                    reEntList = db.YL_DISTRICT.Where(x => x.LEVEL_ID == 1 && x.IN_USE == 1).OrderBy(x => x.ID).ToList();
                }
                else
                {
                    if (!key.IsInt32())
                    {
                        type = "CODE";
                    }
                    switch (type)
                    {
                        case "CODE":
                            reEntList = db.YL_DISTRICT.Where(x => x.YL_DISTRICT2.CODE == key && x.IN_USE == 1).OrderBy(x => x.ID).ToList();
                            break;
                        default:
                            var id = Convert.ToInt32(key);
                            reEntList = db.YL_DISTRICT.Where(x => x.PARENT_ID == id && x.IN_USE == 1).OrderBy(x => x.ID).ToList();
                            break;
                    }
                }
                
                return reEntList.Select(x => (x.YL_DISTRICT1.Count() > 0) ? new IdText { text = string.Format("{0}({1})", x.NAME, x.CODE), id = x.CODE, state = "closed" } : new IdText { text = string.Format("{0}({1})", x.NAME, x.CODE), id = x.CODE }).ToList();
            }
        }

        public IList<TreeClass> DistrictGetAll(string loginKey, ref ErrorInfo err, int id, int levelId)
        {
            IList<YL_DISTRICT> reEntList = new List<YL_DISTRICT>();
            using (DBEntities db = new DBEntities())
            {
                var idInt = Convert.ToInt32(id);
                if (idInt == 0)
                {
                    var user=Global.GetUser(loginKey);
                    if (user == null)
                    {
                        err.IsError = true;
                        err.Message = "登录超时";
                        return null;
                    }
                    idInt = Convert.ToInt32(user.Region);
                }

                var ent = db.YL_DISTRICT.Single(x => x.ID == idInt);
                var idPath = ent.ID_PATH + ent.ID + ".";
                reEntList = db.YL_DISTRICT.Where(x => (x.ID == idInt || x.ID_PATH.IndexOf(idPath) > -1) && x.IN_USE == 1).OrderBy(x => x.ID).ToList();

                reEntList = reEntList.Where(x => x.LEVEL_ID <= levelId).ToList();

                return reEntList.Select(x => new TreeClass { name = x.NAME, id = x.ID.ToString(), parentId = (x.PARENT_ID == null) ? "" : Convert.ToString(x.PARENT_ID) }).ToList();
            }
        }

        public IList<TreeData> DistrictTreeDataGetAll(int id, string loginKey)
        {
            IList<YL_DISTRICT> reEntList = new List<YL_DISTRICT>();
            GlobalUser gu = Global.GetUser(loginKey);
            using (DBEntities db = new DBEntities())
            {
                if (id == 0 )
                {
                    reEntList = db.YL_DISTRICT.Where(x => x.IN_USE == 1).OrderBy(x => x.ID).ToList();
                }
                else
                {
                    var tmpId =Convert.ToInt32(id);
                    var ent = db.YL_DISTRICT.Single(x => x.ID == tmpId);
                    var idPath = ent.ID_PATH + ent.ID + ".";
                    reEntList = db.YL_DISTRICT.Where(x => (x.ID == tmpId || x.ID_PATH.IndexOf(idPath) == 0) && x.IN_USE == 1).OrderBy(x => x.ID).ToList();
                }
                return DistrictTreeDataGetAll(id, reEntList);
            }
        }


        public IList<TreeData> DistrictTreeDataGetAllByCompId(int id, int regionInt)
        {
            IList<YL_DISTRICT> reEntList = new List<YL_DISTRICT>();
            var region = regionInt.ToString();
            using (DBEntities db = new DBEntities())
            {
                if (id == 0 )
                {
                    reEntList = db.YL_DISTRICT.Where(x => x.IN_USE == 1 && x.REGION == region).OrderBy(x => x.ID).ToList();
                }
                else
                {
                    var tmpId = Convert.ToInt32(id);
                    var ent = db.YL_DISTRICT.Single(x => x.ID == tmpId);
                    var idPath = ent.ID_PATH + ent.ID + ".";
                    reEntList = db.YL_DISTRICT.Where(x => (x.ID == tmpId || x.ID_PATH.IndexOf(idPath) == 0) && x.REGION == region && x.IN_USE == 1).OrderBy(x => x.ID).ToList();
                }
                return DistrictTreeDataGetAll(id, reEntList);
            }
        }

        public IList<TreeData> DistrictTreeDataGetAll(int parent_id, IList<YL_DISTRICT> AllData)
        {
            IList<TreeData> now = new List<TreeData>();
            IList<YL_DISTRICT> reEntList = new List<YL_DISTRICT>();
            if (parent_id==0)
            {
                reEntList = AllData.Where(x => x.IN_USE == 1 && x.PARENT_ID == null).OrderBy(x => x.ID).ToList();
            }
            else
            {
                var tmpId = parent_id;
                reEntList = AllData.Where(x => x.IN_USE == 1 && x.PARENT_ID == tmpId).OrderBy(x => x.ID).ToList();
            }
            foreach (var t in reEntList)
            {
                TreeData tmp = new TreeData();
                tmp.id =Convert.ToInt32(t.ID);
                tmp.text = t.NAME;
                tmp.children = DistrictTreeDataGetAll(t.ID, AllData);
                if (tmp.children.Count() > 0)
                {
                    tmp.state = "closed";
                }
                now.Add(tmp);
            }
            return now;

        }





        public DISTRICT DistrictAdd(string loginKey, ref ErrorInfo err, DISTRICT inEnt)
        {
            using (DBEntities db = new DBEntities())
            {
                YL_DISTRICT reEnt = Fun.ClassToCopy<ProInterface.Models.DISTRICT, YL_DISTRICT>(inEnt);
                #region 计算ID_PATH和Level_ID
                if (reEnt.PARENT_ID == null)
                {
                    reEnt.LEVEL_ID = 1;
                    reEnt.ID_PATH = ".";
                }
                else
                {
                    var parent = db.YL_DISTRICT.Single(x => x.ID == reEnt.PARENT_ID);
                    if (parent == null)
                    {
                        reEnt.LEVEL_ID = 1;
                        reEnt.ID_PATH = ".";
                    }
                    else
                    {
                        reEnt.ID_PATH = parent.ID_PATH + reEnt.PARENT_ID + ".";
                        reEnt.LEVEL_ID = parent.LEVEL_ID + 1;
                    }
                }
                #endregion
                reEnt.ID = Fun.GetSeqID<YL_DISTRICT>();
                #region 计算REGION

                GlobalUser gu = Global.GetUser(loginKey);
                if (gu == null)
                {
                    reEnt.REGION = reEnt.ID.ToString();
                }
                else
                {
                    reEnt.REGION = gu.Region;
                } 
                #endregion
                reEnt.CODE = (string.IsNullOrEmpty(inEnt.CODE)) ? reEnt.ID.ToString() : inEnt.CODE;
                reEnt.IN_USE = 1;
                reEnt = db.YL_DISTRICT.Add(reEnt);
                try
                {
                    db.SaveChanges();
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Add);
                    return Fun.ClassToCopy<YL_DISTRICT, DISTRICT>(reEnt);
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = e.Message;
                    err.Excep = e;
                    return null;
                }
            }
        }

        public bool DistrictUpLevelIdpath()
        {
            using (DBEntities db = new DBEntities())
            {

                foreach (var t in db.YL_DISTRICT.Where(x => x.PARENT_ID == null))
                {
                    t.LEVEL_ID = 1;
                    t.ID_PATH = ".";
                    DistrictSetLevelId(t.ID, 1, t.ID_PATH, db);
                }

                db.SaveChanges();
                return true;
            }
        }

        public void DistrictSetLevelId(int? parentId, int levelId,string idPath, DBEntities db)
        {
            
            foreach (var t in db.YL_DISTRICT.Where(x => x.PARENT_ID == parentId))
            {
                t.LEVEL_ID = levelId + 1;
                t.ID_PATH =idPath+parentId+ ".";
                DistrictSetLevelId(t.ID, t.LEVEL_ID, t.ID_PATH, db);
            }
        }

        public bool DistrictEdit(string loginKey, ref ErrorInfo err, DISTRICT inEnt, IList<string> allPar)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var reEnt = db.YL_DISTRICT.SingleOrDefault(a => a.ID == inEnt.ID);
                    if (reEnt.PARENT_ID != inEnt.PARENT_ID)
                    {
                        var parentOld = db.YL_DISTRICT.SingleOrDefault(x => x.ID == reEnt.PARENT_ID);
                        var pathOld = (parentOld == null) ? "." : parentOld.ID_PATH + parentOld.ID + ".";
                        var parentNew = db.YL_DISTRICT.SingleOrDefault(x => x.ID == inEnt.PARENT_ID);
                        var pathNew = (parentNew == null) ? "." : parentNew.ID_PATH + parentNew.ID + ".";
                        if (parentOld == null) parentOld = new YL_DISTRICT();
                        if (parentNew == null) parentNew = new YL_DISTRICT();
                        int levelChange = parentNew.LEVEL_ID - parentOld.LEVEL_ID;

                        reEnt.LEVEL_ID = parentNew.LEVEL_ID + 1;
                        reEnt.ID_PATH = pathNew;
                        DistrictSetLevelId(reEnt.ID, reEnt.LEVEL_ID, reEnt.ID_PATH, db);
                        //var t = db.YL_DISTRICT.Where(x => x.ID_PATH.IndexOf(pathOld) == 0).Update(x => new YL_DISTRICT { ID_PATH = x.ID_PATH.Replace(pathOld, pathNew), LEVEL_ID = x.LEVEL_ID + levelChange });
                    }
                    else
                    {
                        var parentNew = db.YL_DISTRICT.SingleOrDefault(x => x.ID == inEnt.PARENT_ID);
                        if (parentNew == null)
                        {
                            reEnt.LEVEL_ID = 1;
                            reEnt.ID_PATH = ".";
                        }
                        else
                        {
                            reEnt.LEVEL_ID = parentNew.LEVEL_ID + 1;
                            reEnt.ID_PATH = parentNew.ID_PATH + parentNew.ID + ".";
                        }
                    }
                    reEnt = Fun.ClassToCopy<ProInterface.Models.DISTRICT, YL_DISTRICT>(inEnt, reEnt, allPar);

                    db.SaveChanges();
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Edit);
                    return true;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = e.Message;
                    err.Excep = e;
                    return false;
                }
            }
        }


        public string DistrictGetLevelStr(int disId)
        {
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_DISTRICT.SingleOrDefault(x => x.ID == disId);
                IList<string> idArr = new List<string>();
                while (ent != null)
                {
                    idArr.Add(ent.NAME);
                    ent = ent.YL_DISTRICT2;
                }
                idArr = idArr.Reverse().ToList();
                return string.Join(".", idArr);
            }
        }


        public IList<System.Web.Mvc.SelectListItem> DistrictGetUserLevel(string loginKey, ref ErrorInfo err, int levelId, string districtId=null)
        {
            
            IList<System.Web.Mvc.SelectListItem> reList = new List<System.Web.Mvc.SelectListItem>();
            var user = Global.GetUser(loginKey);
            if (user == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return reList;
            }
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_DISTRICT.Single(x => x.ID == user.DistrictId);
                if (ent.LEVEL_ID > levelId)
                {
                    var tmp=ent.YL_DISTRICT2;
                    while (tmp.LEVEL_ID > 1)
                    {
                        tmp = tmp.YL_DISTRICT2;
                    }
                    reList.Add(new System.Web.Mvc.SelectListItem { Value=tmp.ID.ToString(),Text=tmp.NAME });
                    return reList;
                }
                else if (ent.LEVEL_ID == levelId)
                { 
                    reList.Add(new System.Web.Mvc.SelectListItem { Value=ent.ID.ToString(),Text=ent.NAME });
                    return reList;
                }
                else if (ent.LEVEL_ID == levelId-1)
                {
                    reList.Add(new System.Web.Mvc.SelectListItem { Value = "", Text = "所有" });
                    var tmpent = db.YL_DISTRICT.Where(x => x.PARENT_ID == ent.ID && x.IN_USE == 1).OrderBy(x => x.ID).ToList();
                    foreach (var t in tmpent)
                    {

                        reList.Add(new System.Web.Mvc.SelectListItem { Value = t.ID.ToString(), Text = t.NAME, Selected = (t.ID.ToString() == districtId) ? true : false });
                    }
                    return reList;
                }
                else
                {
                    reList.Add(new System.Web.Mvc.SelectListItem { Value = "", Text = "所有" });
                    return reList;
                }
            }
        }


        public DISTRICT DistrictGetByUser(string loginKey, ref ErrorInfo err)
        {
            using (DBEntities db = new DBEntities())
            {
                GlobalUser gu = Global.GetUser(loginKey);
                var reEnt = db.YL_DISTRICT.SingleOrDefault(x => x.ID == gu.DistrictId);
                return Fun.ClassToCopy<YL_DISTRICT, DISTRICT>(reEnt);
            }
        }
    }
}
