
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
    public partial class Service : IUserMessage
    {

        public IList<TUserMessage> UserMessageGetTypeNew(string loginKey, ref ErrorInfo err, int typeId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            GlobalUser gu=Global.GetUser(loginKey);
            using (DBEntities db = new DBEntities())
            {
                var type = db.YL_MESSAGE_TYPE.SingleOrDefault(x => x.ID == typeId);
                if (type == null)
                {
                    err.IsError = true;
                    err.Message = "消息类型不存在";
                    return null;
                }
                var allEnt = db.YL_USER_MESSAGE.Where(x => x.STATUS == "等待" && x.USER_ID==gu.UserId && x.YL_MESSAGE.MESSAGE_TYPE_ID == typeId).OrderByDescending(x=>x.MESSAGE_ID).ToList();

                IList<TUserMessage> allReList = new List<TUserMessage>();
                foreach (var t in allEnt)
                {
                    var tmp = Fun.ClassToCopy<YL_USER_MESSAGE, TUserMessage>(t);
                    tmp.TypeName = t.YL_MESSAGE.YL_MESSAGE_TYPE.NAME;
                    allReList.Add(tmp);
                }
                return allReList;
            }
        }



        public bool UserMessageSetTypeFinish(string loginKey, ref ErrorInfo err, int typeId, int keyId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            GlobalUser gu = Global.GetUser(loginKey);
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_USER_MESSAGE.SingleOrDefault(x => x.YL_MESSAGE.MESSAGE_TYPE_ID == typeId && x.YL_MESSAGE.KEY == keyId && x.USER_ID == gu.UserId);
                if (ent != null)
                {
                    ent.STATUS = "已阅";
                    ent.STATUS_TIME = DateTime.Now;
                    db.SaveChanges();
                }
                return true;
            }
        }

        public bool UserMessageSetFinish(string loginKey, ref ErrorInfo err, int messageId)
        {
           if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            GlobalUser gu = Global.GetUser(loginKey);
            using (DBEntities db = new DBEntities())
            {
                var message = db.YL_USER_MESSAGE.SingleOrDefault(x => x.MESSAGE_ID == messageId && x.USER_ID == gu.UserId);
                if (message == null) return false;
                message.STATUS = "已阅";
                message.STATUS_TIME = DateTime.Now;
                db.SaveChanges();
                return true;
            }
        }


        public bool UserMessageSave(string loginKey, ref ErrorInfo err, ProInterface.Models.MESSAGE inEnt, IList<string> allPar, string allUserIdStr = null)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            GlobalUser gu = Global.GetUser(loginKey);
            return UserMessageSaveByGlobalUser(gu, ref err, inEnt, allPar, allUserIdStr);
        }

        public bool UserMessageSaveAndSend(string loginKey, ref ErrorInfo err, string content, int messageIdTypeId, IList<int> userIdList,List<KV> allPar=null)
        {
            GlobalUser gu = Global.GetUser(loginKey);

            using (DBEntities db = new DBEntities())
            {
                YL_MESSAGE message = new YL_MESSAGE();
                message.ID = Fun.GetSeqID<YL_MESSAGE>();
                message.MESSAGE_TYPE_ID = messageIdTypeId;
                message.CREATE_TIME = DateTime.Now;
                message.CONTENT = content;
                message.CREATE_USERNAME = gu.UserName;
                message.CREATE_USERID = gu.UserId;
                var allUser = db.YL_USER.Where(x => userIdList.Contains(x.ID)).ToList();
                foreach (var t in allUser)
                {
                    var login = db.YL_LOGIN.SingleOrDefault(x => x.LOGIN_NAME == t.LOGIN_NAME);

                    YL_USER_MESSAGE tmp = new YL_USER_MESSAGE();
                    tmp.MESSAGE_ID = message.ID;
                    tmp.USER_ID = t.ID;
                    tmp.STATUS = "等待";
                    tmp.STATUS_TIME = DateTime.Now;

                    tmp.PUSH_TYPE = message.PUSH_TYPE;

                    if (login != null)
                    {
                        tmp.PHONE_NO = login.PHONE_NO;
                        if (message.MESSAGE_TYPE_ID == 4)
                        {
                            //SmsSendAdd(db, login.PHONE_NO, message.CONTENT, message.ID);
                            SmsSendWeiXin(ref err, message.CONTENT, tmp.USER_ID, allPar);
                            if (!err.IsError)
                            {
                                tmp.STATUS = "已推送";
                            }
                            //不发送短信了
                            #region 不发送短信了
                            err.IsError = false; 
                            #endregion
                        }
                    }

                    message.YL_USER_MESSAGE.Add(tmp);
                }
                db.YL_MESSAGE.Add(message);
                db.SaveChanges();
                return true;
            }
        }


        public bool UserMessageSaveByGlobalUser(GlobalUser gu, ref ErrorInfo err, ProInterface.Models.MESSAGE inEnt, IList<string> allPar, string allUserIdStr = null)
        {
            using (DBEntities db = new DBEntities())
            {
                var disId = string.Format(".{0}.", inEnt.DISTRICT_ID);
                IList<int> allUserId = new List<int>();
                IList<int> allRole = new List<int>();
                IList<int> allUser = new List<int>();
                if (!string.IsNullOrEmpty(inEnt.ALL_ROLE_ID))
                {
                    allRole = inEnt.ALL_ROLE_ID.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                }
                if (!string.IsNullOrEmpty(allUserIdStr))
                {
                    allUser = allUserIdStr.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                }
                if (allUser.Count() > 0)
                {
                    allUserId = db.YL_USER.Where(x => allUser.Contains(x.ID) && (x.YL_DISTRICT.ID_PATH.IndexOf(disId) > -1 || x.DISTRICT_ID == inEnt.DISTRICT_ID)).Select(x => x.ID).ToList();
                }
                else if (allRole.Count() == 0)
                {
                    allUserId = db.YL_USER.Where(x => x.YL_DISTRICT.ID_PATH.IndexOf(disId) > -1).Select(x => x.ID).ToList();
                }
                else
                {
                    allUserId = db.YL_USER.Where(x => x.YL_ROLE.Where(y => allRole.Contains(y.ID)).Count() > 0 && (x.DISTRICT_ID == inEnt.DISTRICT_ID || x.YL_DISTRICT.ID_PATH.IndexOf(disId) > -1)).Select(x => x.ID).ToList();
                }

                YL_MESSAGE message = new YL_MESSAGE();
                bool isAdd = false;
                if (inEnt.ID == 0)
                {
                    isAdd = true;
                    message = Fun.ClassToCopy<ProInterface.Models.MESSAGE, YL_MESSAGE>(inEnt);
                    if (message.MESSAGE_TYPE_ID == null)
                    {
                        message.MESSAGE_TYPE_ID = 1;
                    }
                    message.ID = Fun.GetSeqID<YL_MESSAGE>();
                    message.CREATE_TIME = DateTime.Now;
                    message.CREATE_USERNAME = gu.UserName;
                    message.CREATE_USERID = gu.UserId;
                    var allUserList = db.YL_USER.Where(x => allUserId.Contains(x.ID)).ToList();
                    var tmpLoginName = allUserList.Select(x => x.LOGIN_NAME).ToList();
                    var allLogin = db.YL_LOGIN.Where(x => tmpLoginName.Contains(x.LOGIN_NAME)).ToList();
                    foreach (var t in allUserList)
                    {
                        var login = allLogin.SingleOrDefault(x => x.LOGIN_NAME == t.LOGIN_NAME);
                        if (login != null && login.PHONE_NO != null)
                        {


                            YL_USER_MESSAGE tmp = new YL_USER_MESSAGE();
                            tmp.MESSAGE_ID = message.ID;
                            tmp.USER_ID = t.ID;
                            tmp.PHONE_NO = login.PHONE_NO;
                            tmp.STATUS = "等待";
                            tmp.STATUS_TIME = DateTime.Now;


                            if (message.PUSH_TYPE == "短信推送")
                            {
                                SmsSendAdd(db, login.PHONE_NO, message.CONTENT, message.ID);
                                tmp.STATUS = "已推送";
                            }
                            tmp.PUSH_TYPE = message.PUSH_TYPE;
                            message.YL_USER_MESSAGE.Add(tmp);
                        }
                    }
                }
                else
                {
                    message = db.YL_MESSAGE.Single(x => x.ID == inEnt.ID);
                    message = Fun.ClassToCopy<ProInterface.Models.MESSAGE, YL_MESSAGE>(inEnt, message, allPar);
                }

                if (isAdd)
                {
                    db.YL_MESSAGE.Add(message);
                }
                db.SaveChanges();
            }
            return true;
        }

        public bool UserMessageSave(string loginKey, ref ErrorInfo err, string message, string allUserIdStr, int typeId)
        {
            ProInterface.Models.MESSAGE msg=new ProInterface.Models.MESSAGE();
            msg.CONTENT=message;
            msg.MESSAGE_TYPE_ID= typeId;
            msg.DISTRICT_ID = AppSet.CityId;
            msg.PUSH_TYPE = "智能推送";
            return UserMessageSave(loginKey, ref err, msg, null, allUserIdStr);
        }


        public bool UserMessageSaveByGlobalUser(GlobalUser gu, ref ErrorInfo err, string message, string allUserIdStr, int typeId)
        {
            ProInterface.Models.MESSAGE msg = new ProInterface.Models.MESSAGE();
            msg.CONTENT = message;
            msg.MESSAGE_TYPE_ID = typeId;
            msg.DISTRICT_ID = AppSet.CityId;
            msg.PUSH_TYPE = "智能推送";
            return UserMessageSaveByGlobalUser(gu, ref err, msg, null, allUserIdStr);
        }


        public int UserMessageGetNewCount(string loginKey, ref ErrorInfo err)
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
                var allEnt = db.YL_USER_MESSAGE.Where(x => x.STATUS == "等待" && x.USER_ID == gu.UserId).ToList();
                return allEnt.Count();
            }
        }

        public IList<TUserMessage> UserMessageGetNew(string loginKey, ref ErrorInfo err)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            GlobalUser gu = Global.GetUser(loginKey);
            using (DBEntities db = new DBEntities())
            {
                var allEnt = db.YL_USER_MESSAGE.Where(x => x.STATUS == "等待" && x.USER_ID == gu.UserId).OrderByDescending(x=>x.MESSAGE_ID).ToList();
                IList<TUserMessage> allReList = new List<TUserMessage>();
                foreach (var t in allEnt)
                {
                    var tmp = Fun.ClassToCopy<YL_USER_MESSAGE, TUserMessage>(t);
                    tmp = Fun.ClassToCopy<YL_MESSAGE, TUserMessage>(t.YL_MESSAGE, tmp);
                    tmp.TypeName = t.YL_MESSAGE.YL_MESSAGE_TYPE.NAME;
                    allReList.Add(tmp);
                }
                return allReList;
            }
        }

        public IList<TUserMessage> UserMessageGetOld(string loginKey, ref ErrorInfo err)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            GlobalUser gu = Global.GetUser(loginKey);
            using (DBEntities db = new DBEntities())
            {
                var allEnt = db.YL_USER_MESSAGE.Where(x => x.STATUS != "等待" && x.USER_ID == gu.UserId).OrderByDescending(x => x.MESSAGE_ID).ToList();
                IList<TUserMessage> allReList = new List<TUserMessage>();
                foreach (var t in allEnt)
                {
                    var tmp = Fun.ClassToCopy<YL_USER_MESSAGE, TUserMessage>(t);
                    tmp = Fun.ClassToCopy<YL_MESSAGE, TUserMessage>(t.YL_MESSAGE, tmp);
                    tmp.TypeName = t.YL_MESSAGE.YL_MESSAGE_TYPE.NAME;
                    allReList.Add(tmp);
                }
                return allReList;
            }
        }

    }
}
