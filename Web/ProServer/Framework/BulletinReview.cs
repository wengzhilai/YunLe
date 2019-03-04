
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
    public partial class Service : IBulletinReview
    {

        public bool BulletinReviewSave(string loginKey, ref ErrorInfo err, BulletinReview inEnt, IList<string> allPar)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;

            var gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return false;
            }
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_BULLETIN_REVIEW.SingleOrDefault(a => a.ID == inEnt.ID);
                    bool isAdd = false;
                    if (ent == null)
                    {
                        isAdd = true;
                        ent = Fun.ClassToCopy<ProInterface.Models.BulletinReview, YL_BULLETIN_REVIEW>(inEnt);
                        ent.STATUS = "正常";
                        ent.STATUS_TIME = DateTime.Now;
                        ent.USER_ID = gu.UserId;
                        ent.ADD_TIME = DateTime.Now;
                    }
                    else
                    {
                        ent = Fun.ClassToCopy<ProInterface.Models.BulletinReview, YL_BULLETIN_REVIEW>(inEnt, ent, allPar);
                    }

                    if (isAdd)
                    {
                        db.YL_BULLETIN_REVIEW.Add(ent);
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
    }
}
