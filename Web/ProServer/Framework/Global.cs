using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ProServer
{
    public static class Global
    {

        private static string _logPath = AppDomain.CurrentDomain.BaseDirectory + "OnLine.txt";
        private static bool MultiUser = CFG.ConfigMultiUser;
        private static IList<GlobalUser> _OnLines;
        /// <summary>
        /// 在线用户
        /// </summary>
        public static IList<GlobalUser> OnLines
        {
            get
            {
                if (_OnLines == null)
                {
                    _OnLines = GetOnlines();
                    ClearTimeOutUser();
                }
                return _OnLines;
            }
            set
            {
                _OnLines = value;
            }
        }
        private static IList<GlobalUser> GetOnlines()
        {
            string jsonStr = "[]";
            if (File.Exists(_logPath))
            {
                for (int i = 0; i < 100; i++)
                {
                    try
                    {
                        jsonStr = File.ReadAllText(_logPath, Encoding.UTF8);
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(100);
                    }
                }

                IList<GlobalUser> re = ProInterface.JSON.EncodeToEntity<IList<GlobalUser>>(jsonStr);
                if (re == null) re = new List<GlobalUser>();
                return re;
            }
            return new List<GlobalUser>();
        }
        /// <summary>
        /// 保存到文件
        /// </summary>
        private static void Save()
        {
            if (_OnLines == null)
            {
                return;
            }
            string content = ProInterface.JSON.DecodeToStr(_OnLines);

            try
            {
                File.WriteAllText(_logPath, content, Encoding.UTF8);
                return;
            }
            catch
            {
            }
        }

        /// <summary>
        /// 清除超时用户
        /// </summary>
        /// <returns></returns>
        public static int ClearTimeOutUser()
        {
            try
            {
                var timeOut = OnLines.Where(a => Convert.ToDateTime(a.LastOpTime).AddMinutes(ProInterface.AppSet.TimeOut) < DateTime.Now).ToList();
                int reInt = timeOut.Count();
                for (int a = 0; a < reInt; a++)
                {
                    Remove(timeOut[a]);
                }

                string filePath = string.Format("{0}\\UpFiles\\", AppDomain.CurrentDomain.BaseDirectory);
                var allLoginPic = Directory.GetFiles(filePath, "*.jpg");
                foreach (var t in allLoginPic)
                {
                    var name = t.Substring(t.LastIndexOf("\\") + 1);
                    if (name.Length == 36)
                    {
                        var key = name.Substring(0, 32);
                        if (_OnLines.Where(x => x.Guid == key).Count() == 0)
                        {
                            File.Delete(t);
                        }
                    }
                }

                return reInt;
            }
            catch {
                return 0;
            }
        }
        /// <summary>
        /// 删除登录用户
        /// </summary>
        /// <param name="gu"></param>
        public static void Remove(GlobalUser gu)
        {

            string filePath = string.Format("{0}/UpFiles/{1}.jpg", AppDomain.CurrentDomain.BaseDirectory, gu.Guid);
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch { }
            }
            _OnLines.Remove(gu);
        }

        /// <summary>
        /// 检测用户是否超时
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <returns></returns>
        public static GlobalUser CheckLoginKey(string loginKey, ref ProInterface.ErrorInfo err)
        {
            GlobalUser re = GetUser(loginKey);
            if (re == null)
            {
                err.IsError = true;
                err.Message = "登录凭证过期或超时";
            }
            return re;
        }

        public static GlobalUser GetUser(int useid)
        {
            //return new GlobalUser { UserId = 1, RoleID = new List<int> { 1 }, UserName = "admin" , DistrictId=5};
            ClearTimeOutUser();
            GlobalUser re = new GlobalUser();
            try
            {
                re = OnLines.SingleOrDefault(a => a.UserId == useid);
            }
            catch
            {
                _OnLines = null;
                re = OnLines.SingleOrDefault(a => a.UserId == useid);
            }
            if (re == null)
            {
                return null;
            }
            UpdateTime(re.Guid);
            return re;
        }
        public static GlobalUser GetUser(string loginKey)
        {
            //return new GlobalUser { UserId = 1, RoleID = new List<int> { 1 }, UserName = "admin" , DistrictId=5};
            ClearTimeOutUser();
            GlobalUser re = new GlobalUser();
            try
            {
                re = OnLines.SingleOrDefault(a => a.Guid == loginKey);
            }
            catch {
                _OnLines = null;
                re = OnLines.SingleOrDefault(a => a.Guid == loginKey);
            }
            if (re == null)
            {
                return null;
            }
            UpdateTime(re.Guid);
            return re;
        }



        public static bool UpdateTime(string loginKey)
        {
            var ent = OnLines.SingleOrDefault(a => a.Guid == loginKey);
            if (ent != null)
            {
                ent.LastOpTime = DateTime.Now;
                //UserUpLastTime(ent.UserId);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool UpdateRegion(string loginKey, string Region)
        {
            var ent = OnLines.SingleOrDefault(a => a.Guid == loginKey);
            if (ent != null)
            {
                using (DBEntities db = new DBEntities())
                {
                    ent.DistrictId=Convert.ToInt32( Region);
                    var dis = db.YL_DISTRICT.Single(x => x.ID == ent.DistrictId);
                    ent.Region = Region;
                    ent.LevelId = dis.LEVEL_ID;

                    IList<string> idArr = new List<string>();
                    while (dis != null)
                    {
                        idArr.Add(dis.ID.ToString());
                        dis = dis.YL_DISTRICT2;
                    }
                    ent.RegionList = idArr;
                }
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool Del(string loginKey)
        {
            var ent = OnLines.SingleOrDefault(a => a.Guid == loginKey);
            if (ent == null) return false;
            using (DBEntities db = new DBEntities())
            {
                int userID = ent.UserId;
                #region 记录登出历史
                var tmp_user = db.YL_USER.SingleOrDefault(x => x.ID == userID);
                if (tmp_user != null)
                {
                    tmp_user.LAST_LOGOUT_TIME = DateTime.Now;
                }
                if (ProInterface.AppSet.WiteLoginLog)
                {
                    var tmp = db.YL_LOGIN_HISTORY.SingleOrDefault(x => x.ID == ent.loginHistoryId);
                    if (tmp != null)
                    {
                        tmp.LOGOUT_TIME = DateTime.Now;
                    }
                }
                #endregion

                Remove(ent);
                db.SaveChanges();
            }
            Save();
            return true;
        }
        private static object symObj = new object();
        public static GlobalUser Add(int userId, string loginIP)
        {
            GlobalUser reEnt = new GlobalUser();
            var entArr = OnLines.Where(a => a.UserId == userId && a.LoginIP == loginIP).ToList();
            if (entArr.Count() > 0)
            {
                reEnt = entArr[0];
                reEnt.LastOpTime = DateTime.Now;
                return reEnt;
            }

            lock (symObj)
            {
                using (DBEntities db = new DBEntities())
                {
                    var user = db.YL_USER.SingleOrDefault(x => x.ID == userId);

                    string loginKey = Guid.NewGuid().ToString().Replace("-", "");
                    ClearTimeOutUser();
                    

                    IList<string> idArr = new List<string>();
                    var dis = user.YL_DISTRICT;
                    var tmp = dis;
                    while (tmp != null)
                    {
                        idArr.Add(tmp.ID.ToString());
                        tmp = tmp.YL_DISTRICT2;
                    }
                    string tmpRuleRegionList = "";
                    if (user.YL_DISTRICT1.Count() > 0)
                    {
                        tmpRuleRegionList = string.Format("'{0}'", string.Join("','", user.YL_DISTRICT1.Select(x => x.CODE).ToList()));
                    }
                    else
                    {
                        tmpRuleRegionList = string.Format("'{0}'", user.DISTRICT_ID);
                    }


                    reEnt = new GlobalUser
                    {
                        Guid = loginKey,
                        UserId = userId,
                        DistrictId = user.DISTRICT_ID,
                        LastOpTime = DateTime.Now,
                        RoleID = user.YL_ROLE.Select(y => y.ID).ToList(),
                        UserName = user.NAME,
                        LoginIP = loginIP,
                        Region = user.DISTRICT_ID.ToString(),
                        RegionList = idArr,
                        RuleRegionStr = tmpRuleRegionList,
                        LevelId = user.YL_DISTRICT.LEVEL_ID,
                        LoginName = user.LOGIN_NAME,
                        DistrictCode=user.YL_DISTRICT.CODE
                    };
                    OnLines.Add(reEnt);

                    Save();

                    #region 记录登录历史
                    var tmp_user = db.YL_USER.SingleOrDefault(x => x.ID == userId);
                    if (tmp_user.LOGIN_COUNT == null)
                    {
                        tmp_user.LOGIN_COUNT = 0;
                    }
                    tmp_user.LOGIN_COUNT++;
                    tmp_user.LAST_LOGIN_TIME = DateTime.Now;
                    if (ProInterface.AppSet.WiteLoginLog)
                    {
                        YL_LOGIN_HISTORY hist = new YL_LOGIN_HISTORY();
                        hist.ID = Fun.GetSeqID<YL_LOGIN_HISTORY>();
                        hist.USER_ID = userId;
                        hist.LOGIN_TIME = DateTime.Now;
                        hist.LOGIN_HOST = loginIP;
                        db.YL_LOGIN_HISTORY.Add(hist);
                        GetUser(loginKey).loginHistoryId = hist.ID;
                    }
                    db.SaveChanges();
                    #endregion

                    var Watermark = PicFun.CreateWatermark(ProInterface.AppSet.SysName, reEnt.UserName);
                    try
                    {
                        File.WriteAllBytes(string.Format("{0}/UpFiles/{1}.jpg", AppDomain.CurrentDomain.BaseDirectory, reEnt.Guid), Watermark);
                    }
                    catch { }
                    return reEnt;
                }
            }
        }
    }
    public class GlobalUser
    {
        public int loginHistoryId { get; set; }

        /// <summary>
        /// 登录凭证
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 当前节点ID
        /// </summary>
        public int DistrictId { get; set; }
        /// <summary>
        /// 当前节点代码
        /// </summary>
        public string DistrictCode { get; set; }
        /// <summary>
        /// 用户层级
        /// </summary>
        public int LevelId { get; set; }
        /// <summary>
        /// 当前区域
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 用户权限ID
        /// </summary>
        public IList<int> RoleID { get; set; }

        /// <summary>
        /// 区域层级
        /// </summary>
        public IList<string> RegionList { get; set; }

        /// <summary>
        /// 用户管辖区域
        /// </summary>
        public string RuleRegionStr{ get; set; }

        /// <summary>
        /// 最后操作时间
        /// </summary>
        public DateTime LastOpTime { get; set; }

        /// <summary>
        /// 登录IP
        /// </summary>
        public string LoginIP { get; set; }

        /// <summary>
        /// 区域层级
        /// </summary>
        public string GetRegionLeveStr()
        {
            return string.Join(",", RegionList.Select(x => "'" + x + "'"));
        }

        /// <summary>
        /// 所有角色字符串
        /// </summary>
        public string GetRoleAllStr()
        {
            return string.Join(",", RoleID);
        }
    }
}
