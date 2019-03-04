using ProInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using ProInterface.Models.Api;
using System.Data.Entity.Validation;
using ProInterface.Models;
using System.Net;
using System.Drawing;
using System.Web.Configuration;
using Senparc.Weixin.MP.Containers;

namespace ProServer
{
    public class Api : IApi
    {
        public ProInterface.Models.YL_APP_VERSION CheckUpdate(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            ProInterface.Models.YL_APP_VERSION reEnt = new ProInterface.Models.YL_APP_VERSION();
            using (DBEntities db = new DBEntities())
            {
                if (inEnt.para == null) inEnt.para = new List<ApiKeyValueBean>();
                var type = "";
                var tempKey = inEnt.para.SingleOrDefault(x => x.K == "type");
                if (tempKey != null)
                {
                    type = tempKey.V;
                }
                if (string.IsNullOrEmpty(type))
                {
                    err.IsError = true;
                    err.Message = "app版本类型不能为空";
                    return null;
                }
                var v = db.YL_APP_VERSION.Where(x => x.TYPE == type && x.IS_NEW == 1).ToList();
                if (v.Count() == 0)
                {
                    err.IsError = true;
                    err.Message = "没找到版本信息";
                    return null;
                }
                reEnt = Fun.ClassToCopy<YL_APP_VERSION, ProInterface.Models.YL_APP_VERSION>(v[0]);
                reEnt.UPDATE_URL = Fun.ResolveUrl(reEnt.UPDATE_URL);
                //err.Message = string.Format("itms-services://?action=download-manifest&url=https://raw.githubusercontent.com/easyman-github/COSS/master/{0}.plist");
                //err.IsError = false;
                return reEnt;
            }
        }

        public ErrorInfo LoginReg(ref ErrorInfo err, ApiLogingBean inEnt)
        {
            using (DBEntities db = new DBEntities())
            {
                if (ProInterface.AppSet.VerifyCode)
                {
                    var nowDate = DateTime.Now.AddMinutes(-30);
                    var codeNum = db.YL_SMS_SEND.Where(x => x.ADD_TIME > nowDate && x.PHONE_NO == inEnt.loginName && x.CONTENT == inEnt.code).Count();
                    if (codeNum == 0)
                    {
                        err.IsError = true;
                        err.Message = "验证码无效";
                        return err;
                    }
                }
                var userList = db.YL_USER.Where(x => x.LOGIN_NAME == inEnt.loginName).ToList();
                var loginList = db.YL_LOGIN.Where(x => x.LOGIN_NAME == inEnt.loginName).ToList();
                if (userList.Count() > 0)
                {
                    err.IsError = true;
                    err.Message = "用户名已经存在";
                    return err;
                }

                if (string.IsNullOrEmpty(inEnt.loginName))
                {
                    err.IsError = true;
                    err.Message = "电话号码不能为空";
                    return err;
                }

                if (!inEnt.loginName.IsOnlyNumber() || inEnt.loginName.Length != 11)
                {
                    err.IsError = true;
                    err.Message = "电话号码格式不正确";
                    return err;
                }

                if (!Fun.CheckPassword(ref err, inEnt.password))
                {
                    err.Message = string.Format("密码复杂度不够：{0}", err.Message);
                    err.IsError = true;
                    return err;
                }

                switch (inEnt.type)
                {
                    case 0://普通用户

                        var saleMan = db.YL_SALESMAN.SingleOrDefault(x => x.REQUEST_CODE == inEnt.pollCode);
                        if (saleMan == null)
                        {
                            err.IsError = true;
                            err.Message = "推荐码有误";
                            return err;
                        }

                        var userId = new Service().UserAdd(null, ref err, new TUser
                        {
                            DISTRICT_ID = saleMan.YL_USER.DISTRICT_ID,
                            PHONE_NO = inEnt.loginName,
                            RoleAllID = "2",
                            LOGIN_NAME = inEnt.loginName,
                            IS_LOCKED = 0,
                            NAME = (string.IsNullOrEmpty(inEnt.userName)) ? inEnt.loginName : inEnt.userName,
                            PassWord = inEnt.password
                        });
                        if (err.IsError)
                        {
                            return err;
                        }
                        db.YL_CLIENT.Add(new YL_CLIENT
                        {
                            ID = (Int32)userId,
                            STATUS = "注册",
                            STATUS_TIME = DateTime.Now,
                            SALESMAN_ID = saleMan.ID
                        });
                        break;
                    case 1:
                        var team = db.YL_TEAM.SingleOrDefault(x => x.REQUEST_CODE == inEnt.pollCode);
                        if (team == null)
                        {
                            err.IsError = true;
                            err.Message = "推荐码有误";
                            return err;
                        }
                        Service ser = new Service();

                        //增加业务员组织
                        var dis = ser.DistrictAdd(null, ref err, new DISTRICT
                        {
                            NAME = inEnt.userName,
                            PARENT_ID = team.ID
                        });

                        userId = new Service().UserAdd(null, ref err, new TUser
                        {
                            DISTRICT_ID = dis.ID,
                            PHONE_NO = inEnt.loginName,
                            RoleAllID = "3",
                            LOGIN_NAME = inEnt.loginName,
                            IS_LOCKED = 0,
                            NAME = (string.IsNullOrEmpty(inEnt.userName)) ? inEnt.loginName : inEnt.userName,
                            PassWord = inEnt.password
                        });
                        if (err.IsError)
                        {
                            return err;
                        }
                        db.YL_SALESMAN.Add(new YL_SALESMAN
                        {
                            ID = (Int32)userId,
                            TEAM_ID = team.ID,
                            STATUS = "注册",
                            STATUS_TIME = DateTime.Now,
                            REQUEST_CODE = "A" + userId
                        });
                        db.SaveChanges();
                        Fun.ExecuteGetJson("http://139.129.194.140/YL/Salesman/MakeMatrix?salesmanId=" + userId);
                        break;
                    default:
                        err.IsError = true;
                        err.Message = string.Format("类型错误【{0}】", inEnt.type);
                        return err;
                }
                try
                {
                    db.SaveChanges();

                    err.IsError = false;
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return err;
                }
                return err;
            }
        }

        public ErrorInfo LoginOut(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            Global.Del(inEnt.authToken);
            return new ErrorInfo();
        }
        public ApiClientBean ClientLogin(ref ErrorInfo err, ApiLogingBean inEnt)
        {
            ApiClientBean reEnt = new ApiClientBean();
            using (DBEntities db = new DBEntities())
            {
                YL_USER user = new YL_USER();
                GlobalUser gu = new GlobalUser();
                YL_LOGIN login = new YL_LOGIN();
                if (!string.IsNullOrEmpty(inEnt.openid) && string.IsNullOrEmpty(inEnt.loginName))
                {

                    var weixinUser = db.YL_WEIXIN_USER.SingleOrDefault(x => x.OPENID == inEnt.openid);
                    if (weixinUser != null)
                    {
                        if (weixinUser.USER_ID != null)
                        {
                            user = db.YL_USER.SingleOrDefault(x => x.ID == weixinUser.USER_ID);
                            if (user == null)
                            {
                                return new ApiClientBean { OpenId = inEnt.openid };
                            }
                            login = db.YL_LOGIN.SingleOrDefault(x => x.LOGIN_NAME == user.LOGIN_NAME);
                            gu = new Service().UserLogin(ref err, login.LOGIN_NAME, inEnt.imei) as GlobalUser;
                            if (db.YL_CLIENT.SingleOrDefault(x => x.ID == gu.UserId) == null)
                            {
                                return new ApiClientBean { OpenId = inEnt.openid };
                            }
                        }
                        else
                        {
                            return new ApiClientBean { OpenId = inEnt.openid };
                        }
                    }
                    else
                    {
                        err.IsError = true;
                        err.Message = "该用户还没有注册";
                        return null;
                    }

                }
                else
                {
                    gu = new Service().UserLogin(ref err, inEnt.loginName, inEnt.password, inEnt.imei) as GlobalUser;
                    if (err.IsError)
                    {
                        return reEnt;
                    }
                    user = db.YL_USER.SingleOrDefault(x => x.ID == gu.UserId);
                    login = db.YL_LOGIN.SingleOrDefault(x => x.LOGIN_NAME == gu.LoginName);

                    if (user.YL_CLIENT == null)
                    {
                        err.IsError = true;
                        err.Message = "该用户资料不正确，请确定该号码是否为客户";
                        return null;
                    }
                    if (!string.IsNullOrEmpty(inEnt.openid))
                    {
                        var weixinUser = db.YL_WEIXIN_USER.SingleOrDefault(x => x.OPENID == inEnt.openid);
                        if (weixinUser != null)
                        {
                            weixinUser.USER_ID = user.ID;
                        }
                    }
                    db.SaveChanges();
                }

                reEnt = ClientSingle(ref err, new ApiRequesEntityBean { authToken = gu.Guid, id = gu.UserId });
                reEnt.authToken = gu.Guid;
            }

            return reEnt;
        }
        public ApiSalesmanBean SalesmanLogin(ref ErrorInfo err, ApiLogingBean inEnt)
        {
            ApiSalesmanBean reEnt = new ApiSalesmanBean();
            using (DBEntities db = new DBEntities())
            {

                YL_USER user = new YL_USER();
                GlobalUser gu = new GlobalUser();
                YL_LOGIN login = new YL_LOGIN();
                if (!string.IsNullOrEmpty(inEnt.openid) && string.IsNullOrEmpty(inEnt.loginName))
                {

                    var weixinUser = db.YL_WEIXIN_USER.SingleOrDefault(x => x.OPENID == inEnt.openid);
                    if (weixinUser != null)
                    {
                        if (weixinUser.USER_ID != null)
                        {
                            user = db.YL_USER.SingleOrDefault(x => x.ID == weixinUser.USER_ID);
                            if (user == null)
                            {
                                return new ApiSalesmanBean { OpenId = inEnt.openid };
                            }
                            login = db.YL_LOGIN.SingleOrDefault(x => x.LOGIN_NAME == user.LOGIN_NAME);
                            gu = new Service().UserLogin(ref err, login.LOGIN_NAME, inEnt.imei) as GlobalUser;
                            if (db.YL_SALESMAN.SingleOrDefault(x => x.ID == gu.UserId) == null)
                            {
                                return new ApiSalesmanBean { OpenId = inEnt.openid };
                            }
                        }
                        else
                        {
                            return new ApiSalesmanBean { OpenId = inEnt.openid };
                        }
                    }
                }
                else
                {
                    gu = new Service().UserLogin(ref err, inEnt.loginName, inEnt.password, inEnt.imei) as GlobalUser;
                    if (err.IsError)
                    {
                        return reEnt;
                    }
                    user = db.YL_USER.SingleOrDefault(x => x.ID == gu.UserId);
                    login = db.YL_LOGIN.SingleOrDefault(x => x.LOGIN_NAME == gu.LoginName);

                    if (user.YL_SALESMAN == null)
                    {
                        err.IsError = true;
                        err.Message = "该用户不是业务员";
                        return null;
                    }
                    if (!string.IsNullOrEmpty(inEnt.openid))
                    {
                        var weixinUser = db.YL_WEIXIN_USER.SingleOrDefault(x => x.OPENID == inEnt.openid);
                        if (weixinUser != null)
                        {
                            weixinUser.USER_ID = user.ID;
                        }
                    }
                    db.SaveChanges();
                }

                reEnt = SalesmanSingle(ref err, new ApiRequesEntityBean { authToken = gu.Guid, id = gu.UserId });
                if (err.IsError)
                {
                    return reEnt;
                }

            }
            return reEnt;
        }
        public YlSalesman GarageUserLogin(ref ErrorInfo err, ApiLogingBean inEnt)
        {
            YlSalesman reEnt = new YlSalesman();
            using (DBEntities db = new DBEntities())
            {

                GlobalUser gu = new Service().UserLogin(ref err, inEnt.loginName, inEnt.password, inEnt.imei) as GlobalUser;

                if (err.IsError)
                {
                    return reEnt;
                }
                var user = db.YL_USER.SingleOrDefault(x => x.ID == gu.UserId);
                if (user.YL_GARAGE.Count() == 0)
                {
                    err.IsError = true;
                    err.Message = "该用户不是服务商";
                    return null;
                }
                reEnt = GarageUserSingle(ref err, new ApiRequesEntityBean { id = user.ID });
                reEnt.garage = Fun.ClassToCopy<YL_GARAGE, YlGarage>(user.YL_GARAGE.ToList()[0]);
                if (err.IsError)
                {
                    return reEnt;
                }
                reEnt.authToken = gu.Guid;
            }

            return reEnt;
        }

        public ApiPagingDataBean FileList(ref ErrorInfo err, ApiRequesPageBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            if (inEnt.pageSize == 0) inEnt.pageSize = 10;
            if (inEnt == null)
            {
                err.IsError = true;
                err.Message = "参数有误";
                return null;
            }
            ApiPagingDataBean reEnt = new ApiPagingDataBean();

            int skip = 0;
            if (inEnt.currentPage > 1)
            {
                skip = (inEnt.currentPage - 1) * inEnt.pageSize;
            }
            using (DBEntities db = new DBEntities())
            {
                var allData = db.YL_FILES.Where(x => x.USER_ID == gu.UserId).OrderByDescending(x=>x.ID).AsEnumerable();

                #region 过虑条件
                if (inEnt.searchKey != null)
                {
                    foreach (var filter in inEnt.searchKey)
                    {
                        allData = Fun.GetListWhere(allData, filter.K, filter.T, filter.V, ref err);
                    }
                }
                #endregion

                #region 排序

                if (allData == null)
                {
                    err.IsError = true;
                    return null;
                }
                foreach (var filter in inEnt.orderBy)
                {
                    allData = Fun.GetListOrder(allData, filter.K, filter.V, ref err);
                }
                #endregion

                var allList = allData.Skip(skip).Take(inEnt.pageSize).ToList();

                reEnt.currentPage = inEnt.currentPage;
                reEnt.pageSize = inEnt.pageSize;
                reEnt.totalCount = allData.Count();
                reEnt.totalPage = reEnt.totalCount / reEnt.pageSize;
                if (reEnt.totalCount % reEnt.pageSize != 0) reEnt.totalPage++;
                IList<FILES> reList = new List<FILES>();
                foreach (var t in allList)
                {
                    var single = Fun.ClassToCopy<YL_FILES, FILES>(t);
                    reList.Add(single);
                }
                reEnt.data = reList;
            }
            return reEnt;
        }
        public ApiFileBean FileUp(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            throw new NotImplementedException();
        }
        public ErrorInfo FileDel(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            ErrorInfo reEnt = new ErrorInfo();
            using (DBEntities db = new DBEntities())
            {
                var file = db.YL_FILES.SingleOrDefault(x => x.ID == inEnt.id);
                if (file == null)
                {
                    err.IsError = true;
                    err.Message = "文件对象不存在";
                    return null;
                }
                var allPath = Fun.UrlToAllPath(file.URL);
                if (System.IO.File.Exists(allPath))
                {
                    System.IO.File.Delete(allPath);
                }
                db.YL_FILES.Remove(file);
                db.SaveChanges();
                reEnt.IsError = false;
            }
            return reEnt;
        }
        public ErrorInfo SendCode(string loginKey, ref ErrorInfo err, string phone)
        {
            ErrorInfo reEnt = new ErrorInfo();
            if (string.IsNullOrEmpty(phone))
            {
                err.IsError = true;
                err.Message = "电话号码不能为空";
                return err;
            }

            if (!phone.IsOnlyNumber() || phone.Length != 11)
            {
                err.IsError = true;
                err.Message = "电话号码格式不正确";
                return err;
            }

            using (DBEntities db = new DBEntities())
            {
                var code = PicFun.ValidateMake(4);

                var login = db.YL_LOGIN.SingleOrDefault(x => x.LOGIN_NAME == phone);
                if (login != null)
                {
                    login.VERIFY_CODE = code;
                }

                YL_SMS_SEND ent = new YL_SMS_SEND()
                {
                    KEY = Guid.NewGuid().ToString().Replace("-", ""),
                    ADD_TIME = DateTime.Now,
                    CONTENT = code,
                    STAUTS = "成功",
                    PHONE_NO = phone
                };
                if (new ProServer.Service().SmsSendCode(loginKey,phone, code))
                {
                    reEnt.Message = "发送成功";
                }
                else {
                    reEnt.IsError = true;
                    reEnt.Message = "短信服务已欠费，请联系管理员";
                }
                db.YL_SMS_SEND.Add(ent);
                db.SaveChanges();
                return reEnt;
            }
        }
        public ErrorInfo ResetPassword(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            ErrorInfo reEnt = new ErrorInfo();
            string VerifyCode = "";
            string LoginName = "";
            string NewPwd = "";
            var tmp = inEnt.para.SingleOrDefault(x => x.K == "VerifyCode");
            if (tmp != null)
            {
                VerifyCode = tmp.V;
            }
            tmp = inEnt.para.SingleOrDefault(x => x.K == "LoginName");
            if (tmp != null)
            {
                LoginName = tmp.V;
            }
            tmp = inEnt.para.SingleOrDefault(x => x.K == "NewPwd");
            if (tmp != null)
            {
                NewPwd = tmp.V;
            }

            if (string.IsNullOrEmpty(VerifyCode) || string.IsNullOrEmpty(LoginName) || string.IsNullOrEmpty(NewPwd))
            {
                err.IsError = true;
                err.Message = "参数不正确";
                return err;
            }
            using (DBEntities db = new DBEntities())
            {
                var login = db.YL_LOGIN.SingleOrDefault(x => x.LOGIN_NAME == LoginName);
                if (login == null)
                {
                    err.IsError = true;
                    err.Message = "登录名不存在";
                    return err;
                }
                if (login.VERIFY_CODE != VerifyCode)
                {
                    err.IsError = true;
                    err.Message = "验证码不正确";
                    return err;
                }
                //检测密码复杂度
                if (!Fun.CheckPassword(ref err, NewPwd))
                {
                    err.IsError = true;
                    err.Message = string.Format("密码复杂度不够：{0}", err.Message);
                    return err;
                }
                login.PASSWORD = NewPwd.Md5();
                db.SaveChanges();
                return reEnt;
            }
        }
        public IList<ApiKeyValueBean> GetAllDistrict(ref ErrorInfo err)
        {
            using (DBEntities db = new DBEntities())
            {
                return db.YL_DISTRICT.ToList().Select(x => new ApiKeyValueBean { K = x.ID.ToString(), V = x.NAME, T = x.PARENT_ID.ToString() }).ToList();
            }
        }

        public ApiSalesmanBean SalesmanSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_SALESMAN.SingleOrDefault(x => x.ID == inEnt.id);
                if (ent == null)
                {
                    err.IsError = true;
                    err.Message = "您不是服务商，无法登录";
                    return null;
                }
                ApiSalesmanBean reEnt = Fun.ClassToCopy<YL_USER, ApiSalesmanBean>(ent.YL_USER);
                reEnt = Fun.ClassToCopy<YL_SALESMAN, ApiSalesmanBean>(ent, reEnt);
                reEnt.authToken = inEnt.authToken;
                reEnt.distictName = ent.YL_USER.YL_DISTRICT.NAME;
                reEnt.RoleAllID = string.Join(",", gu.RoleID);
                reEnt.DistrictName = gu.Region;

                if (ent.YL_USER.ICON_FILES_ID != null)
                {
                    var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.YL_USER.ICON_FILES_ID);
                    if (image != null) reEnt.iconURL = image.URL;
                }
                if (ent.ID_NO_PIC != null)
                {
                    var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.ID_NO_PIC);
                    if (image != null) reEnt.idNoUrl = image.URL;
                }
                reEnt.phone = ent.YL_USER.LOGIN_NAME;
                var login = db.YL_LOGIN.SingleOrDefault(x => x.LOGIN_NAME == reEnt.LOGIN_NAME);
                if (login != null)
                {
                    reEnt.email = login.EMAIL_ADDR;
                }
                reEnt.allCost = ent.YL_USER.YL_COSTLIST.Sum(x => x.COST).ToString();

                reEnt.clientNum = ent.YL_CLIENT.Count();

                var allOrder = db.YL_ORDER.Where(x => x.YL_CLIENT.SALESMAN_ID == gu.UserId).ToList();

                reEnt.expireInsureNum = allOrder.Where(x => x.YL_ORDER_INSURE!=null && x.YL_ORDER_INSURE.DATE_END!=null && x.YL_ORDER_INSURE.DATE_END.Value.AddMonths(3)>= DateTime.Now && x.YL_ORDER_INSURE.DATE_END.Value < DateTime.Now && x.ORDER_TYPE == "投保" && x.VITAL == 1).Count();
                reEnt.orderNum = allOrder.Count();
                reEnt.orderNumSucc = allOrder.Where(x => x.STATUS == "完成").Count();
                reEnt.qrcode = string.Format("~/File/QrCode/salesman_{0}.jpg|~/File/QrCode/salesman_weixin_{0}.jpg", inEnt.id);

                if (ent.YL_USER.YL_GARAGE.Count() > 0)
                {
                    reEnt.garage = Fun.ClassToCopy<YL_GARAGE, YlGarage>(ent.YL_USER.YL_GARAGE.ToList()[0]);
                }

                return reEnt;
            }
        }

        public ApiClientBean ClientSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_CLIENT.SingleOrDefault(x => x.ID == inEnt.id);
                if (ent == null)
                {
                    err.IsError = true;
                    err.Message = "客户不存在";
                    return null;
                }
                var tmp = Fun.ClassToCopy<YL_USER, ApiClientBean>(ent.YL_USER);
                tmp = Fun.ClassToCopy<YL_CLIENT, ApiClientBean>(ent, tmp);
                if (ent.YL_CAR.Count() > 0)
                {
                    var allCar = ent.YL_CAR.ToList();

                    foreach (var t in allCar)
                    {
                        tmp.AllCar.Add(CarSingle(ref err, new ApiRequesEntityBean { id = t.ID }));
                    }
                    var defaultCar = allCar.FirstOrDefault(x => x.IS_DEFAULT == 1);
                    if (defaultCar != null || allCar.Count() > 0)
                    {
                        if (defaultCar == null)
                        {
                            defaultCar = allCar[0];
                        }
                        tmp.NowCar = CarSingle(ref err, new ApiRequesEntityBean { id = defaultCar.ID });
                    }
                }
                if (ent.YL_USER.ICON_FILES_ID != null)
                {
                    var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.YL_USER.ICON_FILES_ID);
                    if (image != null) tmp.iconURL = image.URL;
                }
                if (ent.ID_NO_PIC_ID != null)
                {
                    var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.ID_NO_PIC_ID);
                    if (image != null) tmp.idNoUrl = image.URL;
                }
                if (ent.ID_NO_PIC_ID1 != null)
                {
                    var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.ID_NO_PIC_ID1);
                    if (image != null) tmp.idNoUrl1 = image.URL;
                }
                if (ent.DRIVER_PIC_ID != null)
                {
                    var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.DRIVER_PIC_ID);
                    if (image != null) tmp.driverPicUrl = image.URL;
                }
                if (ent.DRIVER_PIC_ID1 != null)
                {
                    var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.DRIVER_PIC_ID1);
                    if (image != null) tmp.driverPicUrl1 = image.URL;
                }
                var login = db.YL_LOGIN.SingleOrDefault(x => x.LOGIN_NAME == ent.YL_USER.LOGIN_NAME);
                if (login != null)
                {
                    tmp.email = login.EMAIL_ADDR;
                }
                tmp.allCost = ent.YL_USER.YL_COSTLIST.Sum(x => x.COST).ToString();


                tmp.distictName = ent.YL_USER.YL_DISTRICT.NAME;

                tmp.phone = ent.YL_USER.LOGIN_NAME;

                var salesmanBean = Fun.ClassToCopy<YL_SALESMAN, ApiSalesmanBean>(ent.YL_SALESMAN);
                salesmanBean = Fun.ClassToCopy<YL_USER, ApiSalesmanBean>(ent.YL_SALESMAN.YL_USER, salesmanBean);
                tmp.SalesmanBean = salesmanBean;
                tmp.authToken = inEnt.authToken;
                tmp.RoleAllID = string.Join(",", ent.YL_USER.YL_ROLE.Select(x => x.ID));
                return tmp;
            }
        }
        public YlSalesman GarageUserSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_USER.SingleOrDefault(x => x.ID == inEnt.id);
                YlSalesman reEnt = Fun.ClassToCopy<YL_USER, YlSalesman>(ent);
                if (ent.YL_GARAGE.Count() > 0)
                {
                    reEnt.garage = GarageSingle(ref err, new ApiRequesEntityBean { authToken = inEnt.authToken, id = ent.YL_GARAGE.ToList()[0].ID });
                }

                reEnt.distictName = ent.YL_DISTRICT.NAME;
                if (ent.ICON_FILES_ID != null)
                {
                    var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.ICON_FILES_ID);
                    if (image != null) reEnt.iconURL = image.URL;
                }
                reEnt.phone = ent.LOGIN_NAME;
                var login = db.YL_LOGIN.SingleOrDefault(x => x.LOGIN_NAME == reEnt.LOGIN_NAME);
                if (login != null)
                {
                    reEnt.email = login.EMAIL_ADDR;
                }
                return reEnt;
            }
        }

        public ApiClientBean ClientSave(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiClientBean> inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            if (string.IsNullOrEmpty(inEnt.saveKeys))
            {
                err.IsError = true;
                err.Message = "saveKeys不能为空";
                return null;
            }
            ErrorInfo reEnt = new ErrorInfo() { IsError = true };
            var inBean = inEnt.entity;
            using (DBEntities db = new DBEntities())
            {
                YL_CLIENT ent = db.YL_CLIENT.SingleOrDefault(x => x.ID == inBean.ID);
                YL_LOGIN login = db.YL_LOGIN.SingleOrDefault(x => x.LOGIN_NAME == ent.YL_USER.LOGIN_NAME);
                if (ent == null || login == null)
                {
                    err.IsError = true;
                    err.Message = "用户不存在";
                    return null;
                }
                else
                {
                    if (!ent.YL_USER.LOGIN_NAME.Equals(inBean.phone))
                    {
                        if (db.YL_LOGIN.Where(x => x.LOGIN_NAME == inBean.phone && x.ID != login.ID).Count() > 0)
                        {
                            err.IsError = true;
                            err.Message = "电话号已经存在";
                            return null;
                        }
                        login.LOGIN_NAME = inBean.phone;
                        login.PHONE_NO = inBean.phone;
                        ent.YL_USER.LOGIN_NAME = inBean.phone;
                    }
                    login.EMAIL_ADDR = inBean.email;
                    ent = Fun.ClassToCopy(inBean, ent, inEnt.saveKeys.Split(',').ToList());
                    ent.YL_USER = Fun.ClassToCopy(inBean, ent.YL_USER, inEnt.saveKeys.Split(',').ToList());
                }

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return null;
                }
            }
            reEnt.IsError = false;
            return ClientSingle(ref err, new ApiRequesEntityBean { id = inBean.ID, authToken = inEnt.authToken });
        }

        public ApiSalesmanBean SalesmanSave(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiSalesmanBean> inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            if (string.IsNullOrEmpty(inEnt.saveKeys))
            {
                err.IsError = true;
                err.Message = "saveKeys不能为空";
                return null;
            }
            ErrorInfo reEnt = new ErrorInfo() { IsError = true };
            var inBean = inEnt.entity;
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_SALESMAN.SingleOrDefault(x => x.ID == inBean.ID);
                if (ent == null)
                {
                    err.IsError = true;
                    err.Message = "业务员不存在";
                    return null;
                }
                else
                {
                    ent = Fun.ClassToCopy(inBean, ent, inEnt.saveKeys.Split(',').ToList());
                    ent.YL_USER = Fun.ClassToCopy(inBean, ent.YL_USER, inEnt.saveKeys.Split(',').ToList());
                }

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return null;
                }
            }
            reEnt.IsError = false;
            return SalesmanSingle(ref err, new ApiRequesEntityBean { id = inBean.ID, authToken = inEnt.authToken });
        }

        public ErrorInfo UserEditPwd(ref ErrorInfo err, ApiRequesSaveEntityBean<string> inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            ErrorInfo inBean = new ErrorInfo();
            using (DBEntities db = new DBEntities())
            {
                var user = db.YL_USER.SingleOrDefault(x => x.ID == gu.UserId);
                var login = db.YL_LOGIN.SingleOrDefault(x => x.LOGIN_NAME == user.LOGIN_NAME);
                var tmp = inEnt.para.SingleOrDefault(x => x.K == "oldPwd");
                if (tmp == null)
                {
                    err.IsError = true;
                    err.Message = string.Format("旧密码不能为空");
                    return null;
                }
                var pws = tmp.V;
                if (login.PASSWORD != pws.Md5())
                {
                    err.IsError = true;
                    err.Message = string.Format("原密码不正确");
                    return null;
                }
                login.PASSWORD = inEnt.entity.ToString().Trim().Md5();
                inBean.IsError = false;
                db.SaveChanges();
            }
            return inBean;
        }

        public ApiSalesmanBean ClientSingleByPhone(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            using (DBEntities db = new DBEntities())
            {
                var tmp = inEnt.para.SingleOrDefault(x => x.K == "phone");
                if (tmp == null)
                {
                    err.IsError = true;
                    err.Message = string.Format("参数[phone]为空");
                    return null;
                }
                var ent = db.YL_SALESMAN.SingleOrDefault(x => x.YL_USER.LOGIN_NAME == tmp.V);
                if (ent == null)
                {
                    err.IsError = true;
                    err.Message = "业务员不存在,请确认电话是否正确";
                    return null;
                }
                ApiSalesmanBean reEnt = Fun.ClassToCopy<YL_SALESMAN, ApiSalesmanBean>(ent);
                reEnt = Fun.ClassToCopy(ent.YL_USER, reEnt);

                reEnt.distictName = ent.YL_USER.YL_DISTRICT.NAME;
                if (ent.YL_USER.ICON_FILES_ID != null)
                {
                    var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.YL_USER.ICON_FILES_ID);
                    if (image != null) reEnt.iconURL = image.URL;
                }
                return reEnt;
            }
        }

        public ApiClientBean SalesmanSingleByPhone(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            using (DBEntities db = new DBEntities())
            {
                var tmp = inEnt.para.SingleOrDefault(x => x.K == "phone");
                if (tmp == null)
                {
                    err.IsError = true;
                    err.Message = string.Format("参数[phone]为空");
                    return null;
                }
                var ent = db.YL_CLIENT.SingleOrDefault(x => x.YL_USER.LOGIN_NAME == tmp.V);
                if (ent == null)
                {
                    err.IsError = true;
                    err.Message = "业务员不存在,请确认电话是否正确";
                    return null;
                }
                ApiClientBean reEnt = Fun.ClassToCopy<YL_CLIENT, ApiClientBean>(ent);
                reEnt = Fun.ClassToCopy(ent.YL_USER, reEnt);

                reEnt.distictName = ent.YL_USER.YL_DISTRICT.NAME;
                if (ent.YL_USER.ICON_FILES_ID != null)
                {
                    var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.YL_USER.ICON_FILES_ID);
                    if (image != null) reEnt.iconURL = image.URL;
                }

                return reEnt;
            }
        }

        public ErrorInfo SalesmanJoinTeam(ref ErrorInfo err, ApiRequesSaveEntityBean<string> inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            ErrorInfo inBean = new ErrorInfo();
            using (DBEntities db = new DBEntities())
            {
                var salesman = db.YL_SALESMAN.SingleOrDefault(x => x.ID == gu.UserId);
                if (salesman == null)
                {
                    err.IsError = true;
                    err.Message = "业务不存在";
                    return null;
                }
                if (salesman.TEAM_ID != null)
                {
                    err.IsError = true;
                    err.Message = "该业务员已有团队，请联系管理员";
                    return null;
                }
                var team = db.YL_TEAM.SingleOrDefault(x => x.REQUEST_CODE == inEnt.entity);
                if (team == null)
                {
                    err.IsError = true;
                    err.Message = "团队推荐码有误，请联系管理员";
                    return null;
                }
                salesman.TEAM_ID = team.ID;
                inBean.IsError = false;
                db.SaveChanges();
            }
            return inBean;
        }
        public ErrorInfo ClientPeccancy(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            var idNo = inEnt.para.SingleOrDefault(x => x.K == "IdNo");
            if (idNo == null || string.IsNullOrEmpty(idNo.V))
            {
                err.IsError = true;
                err.Message = "参数【IdNo】不存在";
                return null;
            }

            var t = Fun.ExecutePostJson("https://panda.xmxing.net/panda/zzzf/step2.php?sfz_lr=" + idNo.V, null, StaticVar.CookieList);
            if (t.IndexOf("超时") > 0)
            {
                Bitmap bmp = null;
                StaticVar.CookieList = Fun.ExecuteGetCookieAndPic("https://panda.xmxing.net/panda/include/checkcode.php", ref bmp);
                var path = Fun.UrlToAllPath("~/Upfiles/Tmp/");
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                string fileName = DateTime.Now.Ticks + ".png";
                bmp.Save(path + fileName);
                err.IsError = true;
                err.Message = "验证码过期";
                err.Params = "/YL/Upfiles/Tmp/" + fileName;
                return err;
            }
            var t0 = Fun.ExecutePostJson("https://panda.xmxing.net/panda/zzzf/yzmhq.php?sjhm=0&djkey=111", null, StaticVar.CookieList);

            return new ErrorInfo { Message = Fun.Substring(t, "<option value=\"0\">", "</option>").Trim() };
        }

        public ApiPagingDataBean<ApiPeccancyBean> ClientPeccancy1(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            ApiPagingDataBean<ApiPeccancyBean> reEnt = new ApiPagingDataBean<ApiPeccancyBean>();

            var Code = inEnt.para.SingleOrDefault(x => x.K == "Code");
            if (Code == null || string.IsNullOrEmpty(Code.V))
            {
                err.IsError = true;
                err.Message = "参数【Code】不存在";
                return null;
            }
            //
            var t = Fun.ExecutePostJson(string.Format("https://panda.xmxing.net/panda/zzzf/yzmyz.php?yzm={0}&djkey=111", Code.V), null, StaticVar.CookieList);
            //{"state":0,"msg":"\u77ed\u4fe1\u5df2\u6210\u529f\u53d1\u9001\u5230151****9851\u624b\u673a!"}

            if (t.IndexOf("超时") > 0)
            {
                Bitmap bmp = null;
                StaticVar.CookieList = Fun.ExecuteGetCookieAndPic("https://panda.xmxing.net/panda/include/checkcode.php", ref bmp);
                var path = Fun.UrlToAllPath("~/Upfiles/Tmp/");
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                string fileName = DateTime.Now.Ticks + ".png";
                bmp.Save(path + fileName);
                err.IsError = true;
                err.Message = "验证码过期";
                err.Params = "/YL/Upfiles/Tmp/" + fileName;
                return null;
            }
            var t0 = Fun.ExecutePostJson(string.Format("https://panda.xmxing.net/panda/zzzf/step3.php?sjhm=0&yzm_lr={0}&", Code.V), null, StaticVar.CookieList);

            //var t0 = System.IO.File.ReadAllText(@"F:\work\YunLe\Code\Web\Web\UpFiles\list.txt");
            //t0 = t0.Replace("\r", "").Replace("\t", "").Replace("\n", "");

            var str = Fun.Substring(t0, "<div id=\"wf_body\" class=\"wf_body_css\">", "</div>");
            var allRow = str.Split(new string[] { "</ul>" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            reEnt.currentPage = 1;
            reEnt.pageSize = 10;
            reEnt.totalCount = allRow.Count();
            reEnt.totalPage = 1;
            reEnt.data = new List<ApiPeccancyBean>();
            foreach (var row in allRow)
            {
                var tmp = row.Replace("<ul>", "");
                var itemArr = tmp.Split(new string[] { "</li>" }, StringSplitOptions.RemoveEmptyEntries);
                if (itemArr.Length > 5)
                {
                    ApiPeccancyBean pec = new ApiPeccancyBean();
                    pec.PlateNumber = Fun.NoHTML(itemArr[0]);
                    pec.HappenTime = Fun.NoHTML(itemArr[1]);
                    pec.Action = Fun.NoHTML(itemArr[2]);
                    pec.Score = Fun.NoHTML(itemArr[3]);
                    pec.Money = Fun.NoHTML(itemArr[4]);
                    pec.Id = Fun.Substring(itemArr[5], "gourljf(", ")");
                    reEnt.data.Add(pec);
                }
            }

            return reEnt;
        }
        public ErrorInfo ClientPeccancy2(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            var PicCode = inEnt.para.SingleOrDefault(x => x.K == "PicCode");
            if (PicCode == null || string.IsNullOrEmpty(PicCode.V))
            {
                err.IsError = true;
                err.Message = "参数【PicCode】不存在";
                return null;
            }
            var postData = "{\"username\":\"13679080666\",\"password\":\"xinyu8511171\",\"password\":\"scwidth\",\"code\":\"" + PicCode.V + "\"}";
            postData = "scwidth=1350&username=13679080666&password=xinyu851117&code=" + PicCode.V;
            var t = Fun.ExecutePostJson("https://panda.xmxing.net/panda/login_post.php", postData, StaticVar.CookieList);
            if (t.IndexOf("<frameset") > 0)
            {
                return ClientPeccancy(ref err, inEnt);
            }
            else
            {
                err.IsError = true;
                err.Message = Fun.Substring(t, "alert(\"", "\"");
                return err;
            }

        }


        public ApiCarBean CarSave(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiCarBean> inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            var inBean = inEnt.entity;
            if (inEnt.saveKeys != null && inEnt.saveKeys.IndexOf("PLATE_NUMBER") > -1)
            {
                if ( string.IsNullOrEmpty(inBean.PLATE_NUMBER))
                {
                    if (inBean.BILL_PIC_ID == null || inBean.CERTIFICATE_PIC_ID == null)
                    {
                        err.IsError = true;
                        err.Message = "车牌号不能为空";
                        return null;
                    }
                    else
                    {
                        inBean.PLATE_NUMBER = "新车";
                    }
                }
                else if (inBean.PLATE_NUMBER.Length != 7)
                {
                    err.IsError = true;
                    err.Message = "车牌号位数不正确";
                    return null;
                }
            }


            using (DBEntities db = new DBEntities())
            {
                var client = db.YL_CLIENT.Single(x => x.ID == gu.UserId);

                YL_CAR ent = db.YL_CAR.SingleOrDefault(x => x.PLATE_NUMBER == inBean.PLATE_NUMBER);
                if (ent == null)
                {
                    ent = Fun.ClassToCopy<ApiCarBean, YL_CAR>(inBean);
                    ent.ID = Fun.GetSeqID<YL_CAR>();
                    if (string.IsNullOrEmpty(ent.PLATE_NUMBER) || ent.PLATE_NUMBER == "新车") ent.PLATE_NUMBER =string.Format("新车{0}", ent.ID);
                    ent.YL_CLIENT.Add(client);
                    inBean.ID = ent.ID;
                    db.YL_CAR.Add(ent);
                }
                else
                {
                    if (ent.ID == inBean.ID)
                    {
                        ent = Fun.ClassToCopy(inBean, ent, inEnt.saveKeys.Split(','));
                    }
                    else
                    {
                        client.YL_CAR.Add(ent);
                    }
                }
                foreach (var t in client.YL_CAR.ToList())
                {
                    if (t.PLATE_NUMBER == inBean.PLATE_NUMBER)
                    {
                        t.IS_DEFAULT = 1;
                    }
                    else
                    {
                        t.IS_DEFAULT = 0;
                    }
                }
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return null;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return null;
                }
            }
            return inBean;
        }

        public ApiPagingDataBean CarList(ref ErrorInfo err, ApiRequesPageBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            if (inEnt.pageSize == 0) inEnt.pageSize = 10;
            if (inEnt == null)
            {
                err.IsError = true;
                err.Message = "参数有误";
                return null;
            }
            ApiPagingDataBean reEnt = new ApiPagingDataBean();

            int skip = 0;
            if (inEnt.currentPage > 1)
            {
                skip = (inEnt.currentPage - 1) * inEnt.pageSize;
            }
            using (DBEntities db = new DBEntities())
            {
                var allData = db.YL_CLIENT.Single(x => x.ID == gu.UserId).YL_CAR.AsEnumerable();

                #region 过虑条件
                if (inEnt.searchKey != null)
                {
                    foreach (var filter in inEnt.searchKey)
                    {
                        allData = Fun.GetListWhere(allData, filter.K, filter.T, filter.V, ref err);
                    }
                }
                #endregion

                #region 排序

                if (allData == null)
                {
                    err.IsError = true;
                    return null;
                }
                foreach (var filter in inEnt.orderBy)
                {
                    allData = Fun.GetListOrder(allData, filter.K, filter.V, ref err);
                }
                #endregion

                var allList = allData.Skip(skip).Take(inEnt.pageSize).ToList();

                reEnt.currentPage = inEnt.currentPage;
                reEnt.pageSize = inEnt.pageSize;
                reEnt.totalCount = allData.Count();
                reEnt.totalPage = reEnt.totalCount / reEnt.pageSize;
                if (reEnt.totalCount % reEnt.pageSize != 0) reEnt.totalPage++;
                IList<ApiCarBean> reList = new List<ApiCarBean>();
                foreach (var t in allList)
                {
                    var single = CarSingle(ref err, new ApiRequesEntityBean { id = t.ID });
                    reList.Add(single);
                }
                reEnt.data = reList;
            }
            return reEnt;
        }

        public ApiCarBean CarSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_CAR.SingleOrDefault(x => x.ID == inEnt.id);
                ApiCarBean reEnt = Fun.ClassToCopy<YL_CAR, ApiCarBean>(ent);

                if (ent.DRIVING_PIC_ID != null)
                {
                    var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.DRIVING_PIC_ID);
                    if (image != null) reEnt.DrivingPicUrl = image.URL;
                }
                if (ent.DRIVING_PIC_ID1 != null)
                {
                    var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.DRIVING_PIC_ID1);
                    if (image != null) reEnt.DrivingPicUrl1 = image.URL;
                }

                if (ent.ID_NO_PIC_ID != null)
                {
                    var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.ID_NO_PIC_ID);
                    if (image != null) reEnt.idNoUrl = image.URL;
                }
                if (ent.ID_NO_PIC_ID1 != null)
                {
                    var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.ID_NO_PIC_ID1);
                    if (image != null) reEnt.idNoUrl1 = image.URL;
                }
                if (ent.BILL_PIC_ID != null)
                {
                    var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.BILL_PIC_ID);
                    if (image != null) reEnt.billUrl = image.URL;
                }
                if (ent.CERTIFICATE_PIC_ID != null)
                {
                    var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.CERTIFICATE_PIC_ID);
                    if (image != null) reEnt.certificatePicUrl = image.URL;
                }
                return reEnt;
            }
        }
        public ApiCarBean CarSetDefault(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            using (DBEntities db = new DBEntities())
            {
                var user = db.YL_CLIENT.Single(x => x.ID == gu.UserId);
                ApiCarBean reEnt = new ApiCarBean();
                foreach (var t in user.YL_CAR.ToList())
                {
                    if (t.ID == inEnt.id)
                    {
                        reEnt = Fun.ClassToCopy<YL_CAR, ApiCarBean>(t);
                        if (t.DRIVING_PIC_ID != null)
                        {
                            var image = db.YL_FILES.SingleOrDefault(x => x.ID == t.DRIVING_PIC_ID);
                            if (image != null) reEnt.DrivingPicUrl = image.URL;
                        }
                        t.IS_DEFAULT = 1;
                    }
                    else
                    {
                        t.IS_DEFAULT = 0;
                    }
                }
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return null;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return null;
                }
                return reEnt;
            }
        }

        public ErrorInfo CarDelete(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            using (DBEntities db = new DBEntities())
            {
                var user = db.YL_CLIENT.Single(x => x.ID == gu.UserId);

                var car = user.YL_CAR.SingleOrDefault(x => x.ID == inEnt.id);
                if (car != null)
                {
                    user.YL_CAR.Remove(car);
                }
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return null;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return null;
                }
                return new ErrorInfo();
            }
        }

        public ApiCarBean QueryCar(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            string lateNumber = "", userName = "";
            var tmpPara = inEnt.para.SingleOrDefault(x => x.K == "lateNumber");
            if (tmpPara != null)
            {
                lateNumber = tmpPara.V;
            }
            tmpPara = inEnt.para.SingleOrDefault(x => x.K == "userName");
            if (tmpPara != null)
            {
                userName = tmpPara.V;
            }

            using (DBEntities db = new DBEntities())
            {
                string packetStr = "<Packet><vehicleModel><vehicleName>宝马BMW 528i轿车</vehicleName></vehicleModel></Packet>";
                string postStr = @"
<xml encoding=""utf-8"">
	<appId>2015072100000001</appId>
	<method>V01</method>
	<timestamp>{1}</timestamp>
	<sign>{2}</sign>
	<bizContent>
		<![CDATA[
			{0}
		]]>
	</bizContent>
</xml>";
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmm");
                string stringA = "appId=2015072100000001&bizContent=" + packetStr + "&method=V01&timestamp=" + timestamp;
                string stringSignTemp = stringA + "&appKey=1234567890ABCDEF";
                string sign = stringSignTemp.Md5().ToLower();
                postStr = string.Format(postStr, packetStr, timestamp, sign);
                var str = Fun.ExecutePostJson("http://api.uat.ejintai.com/cps_apihub/auto/gateway", postStr);


                var ent = db.YL_CAR.SingleOrDefault(x => x.ENGINE_NUMBER == lateNumber);
                return Fun.ClassToCopy<YL_CAR, ApiCarBean>(ent);
            }
        }

        public IList<ApiInsureBean> QueryInsure(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiCardBean> inEnt)
        {
            using (DBEntities db = new DBEntities())
            {
                var reList = Fun.ClassListToCopy<YL_INSURER, ApiInsureBean>(db.YL_INSURER.ToList());
                IList<YlInsureProduct> allPrdouct = new List<YlInsureProduct>();

                foreach (var prdouct in db.YL_INSURER_PRODUCT.Where(x => x.PARENT_ID == null).ToList())
                {
                    var nowprdouct = Fun.ClassToCopy<YL_INSURER_PRODUCT, YlInsureProduct>(prdouct);
                    if (prdouct.YL_INSURER_PRODUCT1.Count() > 0)
                    {
                        foreach(var child in prdouct.YL_INSURER_PRODUCT1.ToList())
                        {
                            var tmpChild = Fun.ClassToCopy<YL_INSURER_PRODUCT, YlInsureProduct>(child);
                            tmpChild.maxPay = child.DEFAULT_VAL;
                            nowprdouct.ChildItem.Add(tmpChild);
                        }
                    }
                    nowprdouct.maxPay = nowprdouct.DEFAULT_VAL;
                    allPrdouct.Add(nowprdouct);
                }

                foreach (var t in reList)
                {
                    t.AllProductPrice = allPrdouct;
                }
                return reList;
            }
        }

        public ApiPagingDataBean<ApiInsureProductBean> QueryInsureProduct(ref ErrorInfo err, ApiRequesPageBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            if (inEnt == null)
            {
                err.IsError = true;
                err.Message = "参数有误";
                return null;
            }
            if (inEnt.pageSize == 0) inEnt.pageSize = 10;


            ApiPagingDataBean<ApiInsureProductBean> reEnt = new ApiPagingDataBean<ApiInsureProductBean>();

            int skip = 0;
            if (inEnt.currentPage > 1)
            {
                skip = (inEnt.currentPage - 1) * inEnt.pageSize;
            }
            using (DBEntities db = new DBEntities())
            {
                var allData = db.YL_INSURER_PRODUCT.ToList();
                #region 过虑条件
                if (inEnt.searchKey != null)
                {
                    foreach (var filter in inEnt.searchKey)
                    {
                        allData = Fun.GetListWhere(allData, filter.K, filter.T, filter.V, ref err).ToList();
                    }
                }
                #endregion

                #region 排序

                if (allData == null)
                {
                    err.IsError = true;
                    return null;
                }
                foreach (var filter in inEnt.orderBy)
                {
                    allData = Fun.GetListOrder(allData, filter.K, filter.V, ref err).ToList();
                }
                #endregion

                var allList = allData.Skip(skip).Take(inEnt.pageSize).ToList();

                reEnt.currentPage = inEnt.currentPage;
                reEnt.pageSize = inEnt.pageSize;
                reEnt.totalCount = allData.Count();
                reEnt.totalPage = reEnt.totalCount / reEnt.pageSize;
                if (reEnt.totalCount % reEnt.pageSize != 0) reEnt.totalPage++;
                IList<ApiInsureProductBean> reList = new List<ApiInsureProductBean>();
                foreach (var t in allList)
                {
                    ApiInsureProductBean tmp = Fun.ClassToCopy<YL_INSURER_PRODUCT, ApiInsureProductBean>(t);
                    reList.Add(tmp);
                }
                reEnt.data = reList;
            }
            return reEnt;
        }

        /// <summary>
        /// 计算保单
        /// </summary>
        /// <param name="err"></param>
        /// <param name="inEnt"></param>
        /// <returns></returns>
        public YlOrderInsure OrderInsureSave(ref ErrorInfo err, ApiRequesSaveEntityBean<YlOrderInsure> inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            var inBean = inEnt.entity;
            #region 检测
            if (inBean.INSURER_ID == null || inBean.INSURER_ID == 0)
            {
                err.IsError = true;
                err.Message = "保险公司ID不能为空";
                return null;
            }
            if (string.IsNullOrEmpty(inBean.DELIVERY))
            {
                err.IsError = true;
                err.Message = "配送信息不能为空";
                return null;
            }
            if (inBean.SaveProductId == null || inBean.SaveProductId.Count() == 0)
            {
                err.IsError = true;
                err.Message = "险种不能为空";
                return null;
            } 
            #endregion

            inBean.ORDER_TYPE = "投保";

            if (string.IsNullOrEmpty(inEnt.saveKeys)) inEnt.saveKeys = "";

            var postBean = new ApiRequesSaveEntityBean<YlOrder>();
            postBean.entity = inEnt.entity;
            postBean.saveKeys = inEnt.saveKeys;
            postBean.authToken = inEnt.authToken;

            //生成定单
            var order = OrderSave(ref err, postBean);

            if (err.IsError)
            {
                return null;
            }
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_ORDER.SingleOrDefault(x => x.ID == order.ID);
                try
                {
                    var allSaveKeys = inEnt.saveKeys.Split(',').ToList();
                    if (ent.YL_ORDER_INSURE == null)
                    {
                        #region 添加订单
                        var rescue = Fun.ClassToCopy<ProInterface.Models.YL_ORDER_INSURE, YL_ORDER_INSURE>(inBean);
                        rescue.ID = ent.ID;
                        var client = ent.YL_CLIENT;

                        //rescue.CAR_USERNAME = client.YL_USER.NAME;

                        rescue.ID_NO_PIC_ID = ent.YL_CAR.ID_NO_PIC_ID;
                        rescue.ID_NO_PIC_ID1 = ent.YL_CAR.ID_NO_PIC_ID1;
                        rescue.DRIVING_PIC_ID = ent.YL_CAR.DRIVING_PIC_ID;
                        rescue.DRIVING_PIC_ID1 = ent.YL_CAR.DRIVING_PIC_ID1;
                        foreach (var t in inBean.SaveProductId)
                        {
                            YL_ORDER_INSURE_PRODUCT tmp = new YL_ORDER_INSURE_PRODUCT();
                            tmp.INSURER_ID = inBean.INSURER_ID.Value;
                            tmp.PRODUCT_ID = t.PRODUCT_ID;
                            tmp.MAX_PAY = t.MAX_PAY;
                            tmp.INSURE_ID = rescue.ID;
                            db.YL_ORDER_INSURE_PRODUCT.Add(tmp);
                        }
                        ent.STATUS = "新生成";
                        ent.STATUS_TIME = DateTime.Now;
                        db.YL_ORDER_INSURE.Add(rescue); 
                        #endregion
                    }
                    else
                    {
                        #region 修改投保信息

                        ent.YL_ORDER_INSURE = Fun.ClassToCopy(inBean, ent.YL_ORDER_INSURE, inEnt.saveKeys.Split(','));
                        var nowAllProduce = ent.YL_ORDER_INSURE.YL_ORDER_INSURE_PRODUCT.Where(x => x.INSURER_ID == inBean.INSURER_ID).ToList();

                        ent.YL_ORDER_INSURE.ID_NO_PIC_ID = ent.YL_CAR.ID_NO_PIC_ID;
                        ent.YL_ORDER_INSURE.ID_NO_PIC_ID1 = ent.YL_CAR.ID_NO_PIC_ID1;
                        ent.YL_ORDER_INSURE.DRIVING_PIC_ID = ent.YL_CAR.DRIVING_PIC_ID;
                        ent.YL_ORDER_INSURE.DRIVING_PIC_ID1 = ent.YL_CAR.DRIVING_PIC_ID1;

                        foreach (var t in nowAllProduce)
                        {
                            if (inBean.SaveProductId.SingleOrDefault(x => x.PRODUCT_ID == t.PRODUCT_ID) == null)
                            {
                                db.YL_ORDER_INSURE_PRODUCT.Remove(t);
                            }
                        }
                        foreach (var t in inBean.SaveProductId)
                        {
                            var tmp = nowAllProduce.SingleOrDefault(x => x.PRODUCT_ID == t.PRODUCT_ID);
                            if (tmp == null)
                            {
                                tmp = new YL_ORDER_INSURE_PRODUCT();
                                tmp.INSURER_ID = inBean.INSURER_ID.Value;
                                tmp.PRODUCT_ID = t.PRODUCT_ID;
                                tmp.MAX_PAY = t.MAX_PAY;
                                tmp.INSURE_ID = ent.ID;
                                db.YL_ORDER_INSURE_PRODUCT.Add(tmp);
                            }
                            else {
                                tmp.MAX_PAY = t.MAX_PAY;
                            }
                        }
                        #endregion

                        #region 修改文件
                        var taskHandle = db.YL_TASK_FLOW_HANDLE.FirstOrDefault(x => x.TASK_FLOW_ID == inBean.ID);
                        taskHandle.YL_FILES.Clear();
                        var allFileIdList = inBean.AllFiles.Select(x => x.ID).ToList();
                        taskHandle.YL_FILES = db.YL_FILES.Where(x => allFileIdList.Contains(x.ID)).ToList(); 
                        #endregion
                    }

                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return null;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return null;
                }

            }
            return inBean;
        }

        public YlOrderInsure OrderInsureSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            return new Service().OrderInsureSingleId(inEnt.authToken, ref err, inEnt.id);
        }

        public ApiPagingDataBean<YlOrderInsure> OrderInsureList(ref ErrorInfo err, ApiRequesPageBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            if (inEnt.pageSize == 0) inEnt.pageSize = 10;
            if (inEnt == null)
            {
                err.IsError = true;
                err.Message = "参数有误";
                return null;
            }
            ApiPagingDataBean<YlOrderInsure> reEnt = new ApiPagingDataBean<YlOrderInsure>();

            int skip = 0;
            if (inEnt.currentPage > 1)
            {
                skip = (inEnt.currentPage - 1) * inEnt.pageSize;
            }
            using (DBEntities db = new DBEntities())
            {

                var user = db.YL_USER.SingleOrDefault(x => x.ID == gu.UserId);

                var allData = db.YL_ORDER.OrderByDescending(x => x.ID).AsEnumerable();
                if (user.YL_SALESMAN != null)
                {
                    allData = allData.Where(x => x.CLIENT_ID == gu.UserId || x.YL_CLIENT.SALESMAN_ID == gu.UserId).AsEnumerable();
                }
                else {
                    allData = allData.Where(x => x.CLIENT_ID == gu.UserId).AsEnumerable();
                }
                allData = allData.Where(x => x.ORDER_TYPE == "投保" && x.YL_ORDER_INSURE != null).ToList();

                var IsWarm = inEnt.searchKey.SingleOrDefault(x => x.K == "IsWarm");
                if (IsWarm != null && IsWarm.V.Equals("1"))
                {
                    allData= allData.Where(x => x.YL_ORDER_INSURE != null && x.YL_ORDER_INSURE.DATE_END != null && x.YL_ORDER_INSURE.DATE_END.Value.AddMonths(3) >= DateTime.Now && x.YL_ORDER_INSURE.DATE_END.Value < DateTime.Now && x.ORDER_TYPE == "投保" && x.VITAL == 1);
                    inEnt.searchKey.Remove(IsWarm);
                }

                #region 过虑条件
                if (inEnt.searchKey != null)
                {
                    foreach (var filter in inEnt.searchKey)
                    {
                        allData = Fun.GetListWhere(allData, filter.K, filter.T, filter.V, ref err);
                    }
                }
                #endregion

                #region 排序

                if (allData == null)
                {
                    err.IsError = true;
                    return null;
                }
                foreach (var filter in inEnt.orderBy)
                {
                    allData = Fun.GetListOrder(allData, filter.K, filter.V, ref err);
                }
                #endregion

                var allList = allData.Skip(skip).Take(inEnt.pageSize).ToList();

                reEnt.currentPage = inEnt.currentPage;
                reEnt.pageSize = inEnt.pageSize;
                reEnt.totalCount = allData.Count();
                reEnt.totalPage = reEnt.totalCount / reEnt.pageSize;
                if (reEnt.totalCount % reEnt.pageSize != 0) reEnt.totalPage++;
                IList<YlOrderInsure> reList = new List<YlOrderInsure>();
                foreach (var t in allList)
                {
                    var tmp = OrderInsureSingle(ref err, new ApiRequesEntityBean { authToken=inEnt.authToken, id = t.ID });
                    reList.Add(tmp);
                }
                reEnt.data = reList;
            }
            return reEnt;
        }

        public ApiPagingDataBean<ApiOrderRescueBean> OrderRescueList(ref ErrorInfo err, ApiRequesPageBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            if (inEnt.pageSize == 0) inEnt.pageSize = 10;
            if (inEnt == null)
            {
                err.IsError = true;
                err.Message = "参数有误";
                return null;
            }
            ApiPagingDataBean<ApiOrderRescueBean> reEnt = new ApiPagingDataBean<ApiOrderRescueBean>();

            int skip = 0;
            if (inEnt.currentPage > 1)
            {
                skip = (inEnt.currentPage - 1) * inEnt.pageSize;
            }
            using (DBEntities db = new DBEntities())
            {

                var user = db.YL_USER.SingleOrDefault(x => x.ID == gu.UserId);

                var allData = db.YL_ORDER.OrderByDescending(x => x.ID).AsEnumerable();
                if (user.YL_SALESMAN != null)
                {
                    allData = allData.Where(x => x.CLIENT_ID == gu.UserId || x.YL_CLIENT.SALESMAN_ID == gu.UserId).AsEnumerable();
                }
                else {
                    allData = allData.Where(x => x.CLIENT_ID == gu.UserId).AsEnumerable();
                }
                allData = allData.Where(x => x.YL_ORDER_RESCUE != null).ToList();

                var OrderType = inEnt.searchKey.SingleOrDefault(x => x.K == "ORDER_TYPE");
                if (OrderType != null && OrderType.V.Equals("维保"))
                {
                    string value = OrderType.V;
                    allData = allData.Where(x => x.ORDER_TYPE == "维护" || x.ORDER_TYPE == "保养").ToList();
                    inEnt.searchKey.Remove(OrderType);
                }

                #region 过虑条件
                if (inEnt.searchKey != null)
                {
                    foreach (var filter in inEnt.searchKey)
                    {
                        allData = Fun.GetListWhere(allData, filter.K, filter.T, filter.V, ref err);
                    }
                }
                #endregion

                #region 排序

                if (allData == null)
                {
                    err.IsError = true;
                    return null;
                }
                foreach (var filter in inEnt.orderBy)
                {
                    allData = Fun.GetListOrder(allData, filter.K, filter.V, ref err);
                }
                #endregion

                var allList = allData.Skip(skip).Take(inEnt.pageSize).ToList();

                reEnt.currentPage = inEnt.currentPage;
                reEnt.pageSize = inEnt.pageSize;
                reEnt.totalCount = allData.Count();
                reEnt.totalPage = reEnt.totalCount / reEnt.pageSize;
                if (reEnt.totalCount % reEnt.pageSize != 0) reEnt.totalPage++;
                IList<ApiOrderRescueBean> reList = new List<ApiOrderRescueBean>();
                foreach (var t in allList)
                {
                    var tmp = RescueSingle(ref err, new ApiRequesEntityBean { id = t.ID, authToken=inEnt.authToken });
                    reList.Add(tmp);
                }
                reEnt.data = reList;
            }
            return reEnt;
        }

        public ApiInsuranceExpenseBean QueryInsurePrice(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiCardBean> inEnt)
        {
            return new ApiInsuranceExpenseBean();
        }

        public ApiPagingDataBean<ApiGarageBean> RescueQuery(ref ErrorInfo err, ApiRequesPageBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            if (inEnt.pageSize == 0) inEnt.pageSize = 10;
            if (inEnt == null)
            {
                err.IsError = true;
                err.Message = "参数有误";
                return null;
            }

            double lat = 0, lang = 0;
            var tmpPara = inEnt.searchKey.SingleOrDefault(x => x.K == "lat");
            if (tmpPara != null)
            {
                lat = Convert.ToDouble(tmpPara.V);
                inEnt.searchKey.Remove(tmpPara);
            }
            tmpPara = inEnt.searchKey.SingleOrDefault(x => x.K == "lang");
            if (tmpPara != null)
            {
                lang = Convert.ToDouble(tmpPara.V);
                inEnt.searchKey.Remove(tmpPara);
            }


            ApiPagingDataBean<ApiGarageBean> reEnt = new ApiPagingDataBean<ApiGarageBean>();

            int skip = 0;
            if (inEnt.currentPage > 1)
            {
                skip = (inEnt.currentPage - 1) * inEnt.pageSize;
            }
            using (DBEntities db = new DBEntities())
            {
                var allData = db.YL_GARAGE.ToList();
                if (lat != 0 && lang != 0)
                {
                    allData = allData.Where(x => !string.IsNullOrEmpty(x.LAT) && !string.IsNullOrEmpty(x.LANG)).OrderBy(x => System.Math.Abs(lat - Convert.ToDouble(x.LAT)) + System.Math.Abs(lang - Convert.ToDouble(x.LANG))).ToList();
                }

                #region 过虑条件
                if (inEnt.searchKey != null)
                {
                    foreach (var filter in inEnt.searchKey)
                    {
                        allData = Fun.GetListWhere(allData, filter.K, filter.T, filter.V, ref err).ToList();
                    }
                }
                #endregion

                #region 排序

                if (allData == null)
                {
                    err.IsError = true;
                    return null;
                }
                foreach (var filter in inEnt.orderBy)
                {
                    allData = Fun.GetListOrder(allData, filter.K, filter.V, ref err).ToList();
                }
                #endregion

                var allList = allData.Skip(skip).Take(inEnt.pageSize).ToList();

                reEnt.currentPage = inEnt.currentPage;
                reEnt.pageSize = inEnt.pageSize;
                reEnt.totalCount = allData.Count();
                reEnt.totalPage = reEnt.totalCount / reEnt.pageSize;
                if (reEnt.totalCount % reEnt.pageSize != 0) reEnt.totalPage++;
                IList<ApiGarageBean> reList = new List<ApiGarageBean>();
                foreach (var t in allList)
                {
                    ApiGarageBean tmp = Fun.ClassToCopy<YL_GARAGE, ApiGarageBean>(t);
                    reList.Add(tmp);
                }
                reEnt.data = reList;
            }
            return reEnt;
        }

        public ApiOrderRescueBean RescueSave(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiOrderRescueBean> inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            var inBean = inEnt.entity;
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }


            var postBean = new ApiRequesSaveEntityBean<YlOrder>();
            postBean.entity = inEnt.entity;
            postBean.saveKeys = inEnt.saveKeys;
            postBean.authToken = inEnt.authToken;

            if (postBean.entity.Car == null)
            {
                postBean.entity.Car = new YlCar() { PLATE_NUMBER = inBean.PLATE_NUMBER };
            }
            if (postBean.entity.Client == null)
            {
                postBean.entity.Client = new YlClient() { LOGIN_NAME = inBean.CLIENT_PHONE, NAME = inBean.CLIENT_NAME };
            }

            if (!inBean.CLIENT_PHONE.IsOnlyNumber() || inBean.CLIENT_PHONE.Length != 11)
            {
                err.IsError = true;
                err.Message = "电话号码格式不正确";
                return null;
            }
            

            using (DBEntities db = new DBEntities())
            {

                var garageList = new List<YL_GARAGE>();
                if (inBean.GARAGE_ID == null)
                {
                    garageList = db.YL_GARAGE.ToList().Where(x => x.LANG.IsFloat() && x.LAT.IsFloat()).OrderBy(x => Math.Abs(Convert.ToDouble(x.LAT) - Convert.ToDouble(inBean.LAT)) + Math.Abs(Convert.ToDouble(x.LANG) - Convert.ToDouble(inBean.LANG))).ToList();
                    garageList = garageList.Take(30).ToList();
                }
                else {
                    garageList = db.YL_GARAGE.Where(x => x.ID == inBean.GARAGE_ID).ToList();
                }
                //指定维修站后，选择维修站人员
                IList<int> userIdList = new List<int>();
                foreach(var garage in garageList)
                {
                    userIdList = userIdList.Union(garage.YL_USER.Select(x => x.ID).ToList()).ToList();
                }
                if (userIdList==null || userIdList.Count() == 0)
                {
                    err.IsError = true;
                    err.Message = "维修站没有配置用户，请与管理员联系";
                    return null;
                }
                postBean.entity.UserIdArrStr = string.Join(",", userIdList);

                var order = OrderSave(ref err, postBean);
                if (err.IsError)
                {
                    return null;
                }

                var ent = db.YL_ORDER.SingleOrDefault(x => x.ID == order.ID);
                try
                {

                    if (ent.YL_ORDER_RESCUE == null)
                    {
                        var rescue = Fun.ClassToCopy<ProInterface.Models.YL_ORDER_RESCUE, YL_ORDER_RESCUE>(inBean);
                        rescue.ID = ent.ID;
                        ent.STATUS = "新建";
                        ent.STATUS_TIME = DateTime.Now;
                        ent.YL_ORDER_RESCUE = rescue;
                        db.YL_ORDER_RESCUE.Add(rescue);

                        for (var i = 0; i < garageList.Count(); i++)
                        {
                            db.YL_ORDER_RESCUE_SEND.Add(new YL_ORDER_RESCUE_SEND
                            {
                                GARAGE_ID = garageList[i].ID,
                                ORDER_ID = ent.ID,
                                INDEX_NO = i,
                                ADD_TIME = DateTime.Now
                            });
                        }

                    }
                    else
                    {
                        ent.YL_ORDER_RESCUE = Fun.ClassToCopy(inBean, ent.YL_ORDER_RESCUE, inEnt.saveKeys.Split(','));

                        #region 修改文件
                        var taskHandle = db.YL_TASK_FLOW_HANDLE.FirstOrDefault(x => x.TASK_FLOW_ID == inBean.ID);
                        taskHandle.YL_FILES.Clear();
                        var allFileIdList = inBean.AllFiles.Select(x => x.ID).ToList();
                        taskHandle.YL_FILES = db.YL_FILES.Where(x => allFileIdList.Contains(x.ID)).ToList();
                        #endregion
                    }

                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return null;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return null;
                }

            }
            return inBean;
        }

        public ApiOrderRescueBean RescueSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            using (DBEntities db = new DBEntities())
            {

                var order = OrderSingle(ref err, inEnt);
                ApiOrderRescueBean reEnt = Fun.ClassToCopy<YlOrder, ApiOrderRescueBean>(order);
                var ent = db.YL_ORDER.SingleOrDefault(x => x.ID == inEnt.id);
                var taskFlow = FunTask.TaskSingle(inEnt.authToken, ref err, 0, ent.ID);
                if (ent.YL_ORDER_RESCUE != null)
                {
                    reEnt = Fun.ClassToCopy<YL_ORDER_RESCUE, ApiOrderRescueBean>(ent.YL_ORDER_RESCUE, reEnt);
                    if (ent.YL_ORDER_RESCUE.YL_GARAGE != null)
                    {
                        reEnt.GarageName = ent.YL_ORDER_RESCUE.YL_GARAGE.NAME;
                        reEnt.Garage = GarageSingle(ref err, new ApiRequesEntityBean { id = ent.YL_ORDER_RESCUE.GARAGE_ID.Value });
                    }
                }

                reEnt.AllFlow = Fun.ClassListToCopy<TTaskFlow, ProInterface.Models.YL_ORDER_FLOW>(taskFlow.AllFlow);

                reEnt.COST = ent.YL_ORDER_FLOW.Sum(x => x.COST);
                reEnt.AllFiles = Fun.ClassListToCopy<YL_FILES, ProInterface.Models.FILES>(db.YL_FILES.Where(x => x.YL_TASK_FLOW_HANDLE.Where(y => y.YL_TASK_FLOW.TASK_ID== taskFlow.ID).Count() > 0).ToList());
                reEnt.ClientName = ent.YL_CLIENT.YL_USER.NAME;
                reEnt.ClientPhone = ent.YL_CLIENT.YL_USER.LOGIN_NAME;
                return reEnt;
            }
        }

        public ApiGarageBean GarageSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_GARAGE.SingleOrDefault(x => x.ID == inEnt.id);

                ApiGarageBean reEnt = new ApiGarageBean();
                if (ent != null)
                {
                    reEnt = Fun.ClassToCopy<YL_GARAGE, ApiGarageBean>(ent);
                    reEnt.OrderNum = ent.YL_ORDER_RESCUE.Count().ToString();
                    reEnt.StartNum = "0";
                    if (ent.PIC_ID != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.PIC_ID);
                        if (image != null) reEnt.picIdUrl = image.URL;
                    }
                }
                return reEnt;
            }
        }


        public YlOrder OrderSave(ref ErrorInfo err, ApiRequesSaveEntityBean<YlOrder> inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            var inBean = inEnt.entity;
            if (inBean.CLIENT_ID == 0)
            {
                if (string.IsNullOrEmpty(inBean.Client.LOGIN_NAME) || string.IsNullOrEmpty(inBean.Client.LOGIN_NAME))
                {
                    err.IsError = true;
                    err.Message = "用户姓名，电话不能为空";
                    return null;
                }

                if (inBean.Client.LOGIN_NAME.Length != 11)
                {
                    err.IsError = true;
                    err.Message = "用户电话号码无效";
                    return null;
                }
            }
            var allOrderTypeList = new List<string>() { "救援", "维修", "保养", "投保", "审车" };
            if (!allOrderTypeList.Contains(inBean.ORDER_TYPE))
            {
                err.IsError = true;
                err.Message = "分类不存在";
                return null;
            }

            using (DBEntities db = new DBEntities())
            {
                Service ser = new Service();

                var user = new TUser();
                #region 添加用户
                if (inBean.CLIENT_ID == 0)
                {
                    user = new TUser
                    {
                        DISTRICT_ID = gu.DistrictId,
                        PHONE_NO = inBean.Client.LOGIN_NAME,
                        RoleAllID = "2",
                        LOGIN_NAME = inBean.Client.LOGIN_NAME,
                        IS_LOCKED = 0,
                        NAME = inBean.Client.NAME,
                        PassWord = inBean.Client.LOGIN_NAME.Substring(inBean.Client.LOGIN_NAME.Length - 6)
                    };
                    user = ser.UserGetAndSave(inEnt.authToken, ref err, user, null);
                }
                else
                {
                    user = Fun.ClassToCopy<YL_USER, TUser>(db.YL_USER.SingleOrDefault(x => x.ID == inBean.CLIENT_ID));
                }
                if (user == null)
                {
                    return null;
                } 
                #endregion

                var client = db.YL_CLIENT.SingleOrDefault(x => x.ID == user.ID);
                #region 添加客户
                if (client == null)
                {
                    client = new YL_CLIENT();
                    client.ID = user.ID;
                    client.STATUS = "注册";
                    client.STATUS_TIME = DateTime.Now;
                    client.SALESMAN_ID = gu.UserId;
                    db.YL_CLIENT.Add(client);
                }
                if (client == null)
                {
                    return null;
                } 
                #endregion
                inBean.CLIENT_ID = user.ID;
                var car = new ProInterface.Models.YL_CAR();
                #region 添加车辆
                car = ser.CarGetAndSave(inEnt.authToken, ref err, inBean.Car, inBean.CLIENT_ID);

                if (car == null)
                {
                    return null;
                } 
                #endregion
                inBean.CAR_ID = car.ID;

                if (string.IsNullOrEmpty(inEnt.saveKeys)) inEnt.saveKeys = "";
                //添加流程并启动
                var ent = ser.OrderSaveFun(gu, ref err, db, inBean, inEnt.saveKeys.Split(','));
                if (err.IsError)
                {
                    return null;
                }
                try
                {
                    db.SaveChanges();
                    inBean.ID = ent.ID;
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return null;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return null;
                }
            }
            return inBean;
        }


        public YlOrder OrderSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            return new Service().OrderSingleId(inEnt.authToken, ref err, inEnt.id);
        }

        public ApiPagingDataBean<ApiOrderBean> OrderList(ref ErrorInfo err, ApiRequesPageBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            if (inEnt.pageSize == 0) inEnt.pageSize = 10;
            if (inEnt == null)
            {
                err.IsError = true;
                err.Message = "参数有误";
                return null;
            }
            ApiPagingDataBean<ApiOrderBean> reEnt = new ApiPagingDataBean<ApiOrderBean>();

            int skip = 0;
            if (inEnt.currentPage > 1)
            {
                skip = (inEnt.currentPage - 1) * inEnt.pageSize;
            }
            using (DBEntities db = new DBEntities())
            {

                var user = db.YL_USER.SingleOrDefault(x => x.ID == gu.UserId);

                var allData = db.YL_ORDER.OrderByDescending(x => x.ID).ToList();
                if (user.YL_SALESMAN != null)
                {
                    var allGrageIdList = user.YL_SALESMAN.YL_USER.YL_GARAGE.Select(x => x.ID).ToList();
                    if (allGrageIdList.Count() > 0)
                    {
                        allData = allData.Where(x => (x.YL_ORDER_RESCUE != null && x.YL_ORDER_RESCUE.GARAGE_ID != null && allGrageIdList.Contains(x.YL_ORDER_RESCUE.GARAGE_ID.Value)) || x.CLIENT_ID == gu.UserId || x.YL_CLIENT.SALESMAN_ID == gu.UserId).ToList();
                    }
                    else {
                        allData = allData.Where(x => x.CLIENT_ID == gu.UserId || x.YL_CLIENT.SALESMAN_ID == gu.UserId).ToList();
                    }
                }
                else {
                    allData = allData.Where(x => x.CLIENT_ID == gu.UserId).ToList();
                }

                var OrderType = inEnt.searchKey.SingleOrDefault(x => x.K == "ORDER_TYPE");
                if (OrderType != null && OrderType.V.Equals("维保"))
                {
                    string value = OrderType.V;
                    allData = allData.Where(x => x.ORDER_TYPE == "维修" || x.ORDER_TYPE == "保养").ToList();
                    inEnt.searchKey.Remove(OrderType);
                }

                #region 过虑条件
                if (inEnt.searchKey != null)
                {
                    foreach (var filter in inEnt.searchKey)
                    {
                        try
                        {
                            allData = Fun.GetListWhere(allData, filter.K, filter.T, filter.V, ref err).ToList();
                        }
                        catch
                        {

                        }
                    }
                }
                #endregion

                #region 排序

                if (allData == null)
                {
                    err.IsError = true;
                    return null;
                }
                foreach (var filter in inEnt.orderBy)
                {
                    allData = Fun.GetListOrder(allData, filter.K, filter.V, ref err).ToList();
                }
                #endregion

                if (err.IsError)
                {
                    return null;
                }
                allData = allData.ToList();
                var allList = allData.Skip(skip).Take(inEnt.pageSize).ToList();

                reEnt.currentPage = inEnt.currentPage;
                reEnt.pageSize = inEnt.pageSize;
                reEnt.totalCount = allData.Count();
                reEnt.totalPage = reEnt.totalCount / reEnt.pageSize;
                if (reEnt.totalCount % reEnt.pageSize != 0) reEnt.totalPage++;
                IList<ApiOrderBean> reList = new List<ApiOrderBean>();
                foreach (var t in allList)
                {
                    var tmp = Fun.ClassToCopy<YL_ORDER, ApiOrderBean>(t);
                    tmp.ClientName = t.YL_CLIENT.YL_USER.NAME;
                    tmp.ClientPhone = t.YL_CLIENT.YL_USER.LOGIN_NAME;
                    if (string.IsNullOrEmpty(t.ORDER_NO))
                    {
                        string idStr = "0000" + t.ID;
                        idStr = idStr.Substring(idStr.Length - 4);
                        tmp.ORDER_NO = t.CREATE_TIME.ToString("yyyyMMddhhmm") + idStr;
                    }
                    tmp.LastStatus = "新建";
                    var allFlow = t.YL_ORDER_FLOW.OrderByDescending(x => x.LEVEL_ID).ToList();
                    if (allFlow.Count() > 0)
                    {
                        if(tmp.COST==null || tmp.COST==0)
                        {
                            tmp.COST = allFlow.Sum(x => x.COST);
                        }
                        tmp.LastStatus = allFlow[0].SUBJECT;
                    }
                    if (t.YL_CAR != null)
                    {
                        tmp.CarPlateNumber = t.YL_CAR.PLATE_NUMBER;
                    }

                    if (t.YL_ORDER_RESCUE != null)
                    {
                        tmp.AddressStr = t.YL_ORDER_RESCUE.ADDRESS;
                    }

                    var taskFlow = db.YL_TASK_FLOW.SingleOrDefault(x => x.ID == tmp.ID);
                    if (taskFlow != null && taskFlow.YL_TASK!=null)
                    {
                        tmp.STATUS = taskFlow.YL_TASK.STATUS;
                        if (taskFlow.YL_TASK.STATUS_TIME != null)
                        {
                            tmp.STATUS_TIME = taskFlow.YL_TASK.STATUS_TIME.Value;
                        }
                    }
                    reList.Add(tmp);
                }
                reEnt.data = reList;
            }
            return reEnt;
        }

        public ApiPagingDataBean<ApiOrderBean> OrderGrabList(ref ErrorInfo err, ApiRequesPageBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            if (inEnt.pageSize == 0) inEnt.pageSize = 10;
            if (inEnt == null)
            {
                err.IsError = true;
                err.Message = "参数有误";
                return null;
            }
            ApiPagingDataBean<ApiOrderBean> reEnt = new ApiPagingDataBean<ApiOrderBean>();

            int skip = 0;
            if (inEnt.currentPage > 1)
            {
                skip = (inEnt.currentPage - 1) * inEnt.pageSize;
            }
            using (DBEntities db = new DBEntities())
            {

                var user = db.YL_USER.SingleOrDefault(x => x.ID == gu.UserId);

                var allData = db.YL_ORDER.OrderByDescending(x => x.ID).AsEnumerable();
                //if (user.YL_SALESMAN != null)
                //{
                //    allData = allData.Where(x => x.CLIENT_ID == gu.UserId || x.YL_CLIENT.SALESMAN_ID == gu.UserId).AsEnumerable();
                //}
                //else {
                //    allData = allData.Where(x => x.CLIENT_ID == gu.UserId).AsEnumerable();
                //}

                var OrderType = inEnt.searchKey.SingleOrDefault(x => x.K == "ORDER_TYPE");
                if (OrderType != null && OrderType.V.Equals("维保"))
                {
                    string value = OrderType.V;
                    allData = allData.Where(x => x.ORDER_TYPE == "维修" || x.ORDER_TYPE == "保养").ToList();
                    inEnt.searchKey.Remove(OrderType);
                }
                allData = allData.Where(x => 
                x.ORDER_TYPE == "救援" || 
                x.ORDER_TYPE == "维修" || 
                x.ORDER_TYPE == "保养"
                ).ToList();


                #region 过虑条件
                if (inEnt.searchKey != null)
                {
                    foreach (var filter in inEnt.searchKey)
                    {
                        allData = Fun.GetListWhere(allData, filter.K, filter.T, filter.V, ref err);
                    }
                }
                #endregion

                #region 排序

                if (allData == null)
                {
                    err.IsError = true;
                    return null;
                }
                foreach (var filter in inEnt.orderBy)
                {
                    allData = Fun.GetListOrder(allData, filter.K, filter.V, ref err);
                }
                #endregion

                //指定维修站的就不用抢了
                allData = allData.Where(x => x.YL_ORDER_RESCUE != null && x.YL_ORDER_RESCUE.GARAGE_ID == null);
                var allList = allData.ToList();
                var mygarage = db.YL_USER.SingleOrDefault(x => x.ID == gu.UserId).YL_GARAGE.Select(x => x.ID).ToList();

                var tempSend = db.YL_ORDER_RESCUE_SEND.Where(x => !mygarage.Contains(x.GARAGE_ID)).ToList().Where(x => x.ADD_TIME.AddMinutes(x.INDEX_NO - 1) > DateTime.Now).Select(x => x.ORDER_ID).ToList();
                allList = allList.Where(x => !tempSend.Contains(x.ID)).ToList();
                allList = allData.Skip(skip).Take(inEnt.pageSize).ToList();

                reEnt.currentPage = inEnt.currentPage;
                reEnt.pageSize = inEnt.pageSize;
                reEnt.totalCount = allData.Count();
                reEnt.totalPage = reEnt.totalCount / reEnt.pageSize;
                if (reEnt.totalCount % reEnt.pageSize != 0) reEnt.totalPage++;
                IList<ApiOrderBean> reList = new List<ApiOrderBean>();
                foreach (var t in allList)
                {
                    var tmp = Fun.ClassToCopy<YL_ORDER, ApiOrderBean>(t);
                    tmp.ClientName = t.YL_CLIENT.YL_USER.NAME;
                    tmp.ClientPhone = t.YL_CLIENT.YL_USER.LOGIN_NAME;
                    if (string.IsNullOrEmpty(t.ORDER_NO))
                    {
                        string idStr = "0000" + t.ID;
                        idStr = idStr.Substring(idStr.Length - 4);
                        tmp.ORDER_NO = t.CREATE_TIME.ToString("yyyyMMddhhmm") + idStr;
                    }
                    if (t.YL_ORDER_RESCUE != null)
                    {
                        tmp.AddressStr = t.YL_ORDER_RESCUE.ADDRESS;
                    }
                    if(t.YL_CAR!=null)
                    {
                        tmp.CarPlateNumber = t.YL_CAR.PLATE_NUMBER;
                    }
                    reList.Add(tmp);
                }
                reEnt.data = reList;
            }
            return reEnt;
        }


        public ApiOrderRescueBean OrderGrab(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }

            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_ORDER_RESCUE.SingleOrDefault(x => x.ID == inEnt.id);
                var mygarage = db.YL_USER.SingleOrDefault(x => x.ID == gu.UserId).YL_GARAGE.Select(x => x.ID).ToList();
                if (mygarage.Count == 0)
                {
                    err.IsError = true;
                    err.Message = "您没有配置维修站";
                    return null;
                }
                //所有发送
                var allSend = ent.YL_ORDER_RESCUE_SEND.Where(x => mygarage.Contains(x.GARAGE_ID)).ToList();
                if (allSend.Count() == 0)
                {
                    ent.GARAGE_ID = mygarage[0];
                }
                else
                {
                    foreach (var t in allSend)
                    {
                        t.ACCEPT_TIME = DateTime.Now;
                        t.USER_ID = gu.UserId;
                        ent.GARAGE_ID = t.GARAGE_ID;
                    }
                }

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return null;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return null;
                }
                return null;
            }
        }

        public ErrorInfo OrderSaveStatus(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            string stauts = "", body = null, fileIdStr = "";
            int orderId = 0, taskID = 0, taskFlowId = 0;
            decimal cost = 0;
            #region 读取参数
            var tmpPar = inEnt.para.SingleOrDefault(x => x.K == "stauts");
            if (tmpPar != null)
            {
                stauts = tmpPar.V;
            }
            tmpPar = inEnt.para.SingleOrDefault(x => x.K == "body");
            if (tmpPar != null)
            {
                body = tmpPar.V;
            }
            tmpPar = inEnt.para.SingleOrDefault(x => x.K == "cost");
            if (tmpPar != null && tmpPar.V.IsDouble())
            {
                cost = Convert.ToDecimal(tmpPar.V);
            }
            tmpPar = inEnt.para.SingleOrDefault(x => x.K == "fileIdStr");
            if (tmpPar != null)
            {
                fileIdStr = tmpPar.V;
            }
            tmpPar = inEnt.para.SingleOrDefault(x => x.K == "orderId");
            if (tmpPar != null && tmpPar.V.IsInt32())
            {
                orderId = Convert.ToInt32(tmpPar.V);
            }
            tmpPar = inEnt.para.SingleOrDefault(x => x.K == "taskID");
            if (tmpPar != null && tmpPar.V.IsInt32())
            {
                taskID = Convert.ToInt32(tmpPar.V);
            }
            tmpPar = inEnt.para.SingleOrDefault(x => x.K == "taskFlowId");
            if (tmpPar != null && tmpPar.V.IsInt32())
            {
                taskFlowId = Convert.ToInt32(tmpPar.V);
            }

            #endregion

            if (stauts == "抢单")
            {
                OrderGrab(ref err, new ApiRequesEntityBean { id = orderId, authToken = inEnt.authToken });
                if (err.IsError)
                {
                    return null;
                }
            }


            using (DBEntities db = new DBEntities())
            {

                ProInterface.Models.YL_ORDER_FLOW flow = new ProInterface.Models.YL_ORDER_FLOW();
                flow.ID = taskFlowId;
                flow.ORDER_ID = orderId;
                flow.COST = cost;
                flow.SUBJECT = stauts;
                flow.BODY = body;
                flow.AllFilesStr = fileIdStr;
                flow.STATUS = (flow.COST == null || flow.COST == 0) ? "已支付" : "待支付";
                var serr = new Service();
                FunTask.FlowSubmit(db, ref err, gu, taskFlowId, body, fileIdStr, null, null, 0, stauts);
                if (err.IsError)
                {
                    return err;
                }
                db.SaveChanges();
                if (serr.OrderFlowSave(inEnt.authToken, ref err, flow, null))
                {
                    return new ErrorInfo();
                }
                else
                {

                    return err;
                }
            }
        }

        public ErrorInfo OrderSaveVital(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            int VITAL = 0;
            #region 读取参数
            var tmpPar = inEnt.para.SingleOrDefault(x => x.K == "VITAL");
            if (tmpPar != null && tmpPar.V.IsInt32())
            {
                VITAL = Convert.ToInt32(tmpPar.V);
            }
            #endregion
            using (DBEntities db = new DBEntities())
            {
                var order = db.YL_ORDER.SingleOrDefault(x => x.ID == inEnt.id);
                if (order != null)
                {
                    order.VITAL = VITAL;
                    db.SaveChanges();
                    err.Message = "设置成功";
                }
                else
                {
                    err.Message = "设置失败";
                }
                return err;
            }
        }

        public ApiClientBean SalesmanClientAdd(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiClientBean> inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            var inBean = inEnt.entity;
            using (DBEntities db = new DBEntities())
            {

                #region 验证


                if (string.IsNullOrEmpty(inBean.NAME))
                {
                    err.IsError = true;
                    err.Message = "姓名不能为空";
                    return null;
                }



                if (string.IsNullOrEmpty(inBean.LOGIN_NAME))
                {
                    inBean.ID = Fun.GetSeqID<YL_USER>();
                    inBean.LOGIN_NAME = inBean.NAME + inBean.ID.ToString();
                }
                else if (!inBean.LOGIN_NAME.IsOnlyNumber() || inBean.LOGIN_NAME.Length != 11)
                {
                    err.IsError = true;
                    err.Message = "电话号码格式不正确";
                    return null;
                }
                #endregion

                var saleMan = db.YL_SALESMAN.SingleOrDefault(x => x.ID == gu.UserId);
                if (saleMan == null)
                {
                    err.IsError = true;
                    err.Message = "您不是业务员";
                    return null;
                }

                var tuser = new Service().UserSave(inEnt.authToken, ref err, new TUser
                {
                    ID = inBean.ID,
                    DISTRICT_ID = saleMan.YL_USER.DISTRICT_ID,
                    RoleAllID = "2",
                    PHONE_NO = inBean.LOGIN_NAME,
                    LOGIN_NAME = inBean.LOGIN_NAME,
                    IS_LOCKED = 0,
                    PassWord = inBean.LOGIN_NAME.Md5(),
                    ICON_FILES_ID=inBean.ICON_FILES_ID,
                    NAME = (string.IsNullOrEmpty(inBean.NAME)) ? inBean.LOGIN_NAME : inBean.NAME,
                    REMARK = inBean.REMARK
                }, new List<string> { "PHONE_NO", "LOGIN_NAME", "NAME", "REMARK" });

                if (err.IsError)
                {
                    return null;
                }
                var client = db.YL_CLIENT.SingleOrDefault(x => x.ID == tuser.ID);

                if (client == null)//表示修改用户信息
                {
                    #region 添加
                    if (err.IsError)
                    {
                        return null;
                    }
                    client = Fun.ClassToCopy<ApiClientBean, YL_CLIENT>(inBean);
                    client.ID = tuser.ID;
                    client.STATUS = "注册";
                    client.STATUS_TIME = DateTime.Now;
                    client.SALESMAN_ID = saleMan.ID;
                    db.YL_CLIENT.Add(client);
                    #endregion

                }
                else
                {

                    #region 修改
                    client = Fun.ClassToCopy(inBean, client, new List<string> { "SEX", "ADDRESS", "REMARK", "ID_NO" });
                    client.YL_USER.ICON_FILES_ID = inBean.ICON_FILES_ID;
                    client.DRIVER_PIC_ID = inBean.DRIVER_PIC_ID;
                    client.DRIVER_PIC_ID1 = inBean.DRIVER_PIC_ID1;
                    client.ID_NO_PIC_ID = inBean.ID_NO_PIC_ID;
                    client.ID_NO_PIC_ID1 = inBean.ID_NO_PIC_ID1;
 
                    #endregion
                }
                var allClientCar = client.YL_CAR.ToList();
                var inAllClientCar = inBean.AllCar.ToList();
                #region 更新车辆数据

                foreach (var car in allClientCar)
                {
                    var tmpCar = inAllClientCar.SingleOrDefault(x => x.PLATE_NUMBER == car.PLATE_NUMBER);
                    if (tmpCar == null)
                    {
                        client.YL_CAR.Remove(car);
                    }
                    else
                    {
                        car.DRIVING_PIC_ID = tmpCar.DRIVING_PIC_ID;
                        car.DRIVING_PIC_ID1 = tmpCar.DRIVING_PIC_ID1;
                    }
                }
                foreach (var car in inAllClientCar)
                {
                    var tmpCar = allClientCar.SingleOrDefault(x => x.PLATE_NUMBER == car.PLATE_NUMBER);
                    if (tmpCar == null)
                    {
                        var tmpAdd = db.YL_CAR.SingleOrDefault(x => x.PLATE_NUMBER == car.PLATE_NUMBER);
                        if (tmpAdd == null)
                        {
                            tmpAdd = new YL_CAR();
                            tmpAdd.ID = Fun.GetSeqID<YL_CAR>();
                            tmpAdd.PLATE_NUMBER = car.PLATE_NUMBER;
                            tmpAdd.DRIVING_PIC_ID = car.DRIVING_PIC_ID;
                            tmpAdd.DRIVING_PIC_ID1 = car.DRIVING_PIC_ID1;
                        }
                        client.YL_CAR.Add(tmpAdd);
                    }

                } 
                #endregion
                db.SaveChanges();
                return Fun.ClassToCopy<YL_CLIENT, ApiClientBean>(client);

            }
        }

        public ApiPagingDataBean<ApiClientBean> SalesmanClientList(ref ErrorInfo err, ApiRequesPageBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            if (inEnt.pageSize == 0) inEnt.pageSize = 10;
            if (inEnt == null)
            {
                err.IsError = true;
                err.Message = "参数有误";
                return null;
            }
            ApiPagingDataBean<ApiClientBean> reEnt = new ApiPagingDataBean<ApiClientBean>();

            int skip = 0;
            if (inEnt.currentPage > 1)
            {
                skip = (inEnt.currentPage - 1) * inEnt.pageSize;
            }
            using (DBEntities db = new DBEntities())
            {
                var allData = db.YL_CLIENT.Where(x => x.SALESMAN_ID == gu.UserId).AsEnumerable();

                var serarch = inEnt.searchKey.SingleOrDefault(x => x.K == "searchName");
                if (serarch != null)
                {
                    var k = serarch.V;
                    if (!string.IsNullOrEmpty(k))
                    {
                        allData = allData.Where(x =>
                        (x.ADDRESS != null && x.ADDRESS.IndexOf(k) > -1)
                        || (x.SEX != null && x.SEX.IndexOf(k) > -1)
                        || (x.ID_NO != null && x.ID_NO.IndexOf(k) > -1)
                        || (x.STATUS != null && x.STATUS.IndexOf(k) > -1)
                        || (x.REMARK != null && x.REMARK.IndexOf(k) > -1)
                        || (x.YL_USER != null && x.YL_USER.NAME.IndexOf(k) > -1)
                        || (x.YL_USER != null && x.YL_USER.LOGIN_NAME.IndexOf(k) > -1)
                        ).ToList();
                    }
                    inEnt.searchKey.Remove(serarch);
                }
                var OrderType = inEnt.searchKey.SingleOrDefault(x => x.K == "OrderType");
                if (OrderType != null)
                {
                    string value = OrderType.V;
                    allData = allData.Where(x => x.YL_ORDER.Where(y => y.PAY_STATUS == value).Count() > 0).ToList();
                    inEnt.searchKey.Remove(OrderType);
                }


                #region 过虑条件
                if (inEnt.searchKey != null)
                {
                    foreach (var filter in inEnt.searchKey)
                    {
                        allData = Fun.GetListWhere(allData, filter.K, filter.T, filter.V, ref err);
                    }
                }
                #endregion
                #region 排序

                if (allData == null)
                {
                    err.IsError = true;
                    return null;
                }
                foreach (var filter in inEnt.orderBy)
                {
                    allData = Fun.GetListOrder(allData, filter.K, filter.V, ref err);
                }
                #endregion
                var allList = allData.Skip(skip).Take(inEnt.pageSize).ToList();
                reEnt.currentPage = inEnt.currentPage;
                reEnt.pageSize = inEnt.pageSize;
                reEnt.totalCount = allData.Count();
                reEnt.totalPage = reEnt.totalCount / reEnt.pageSize;
                if (reEnt.totalCount % reEnt.pageSize != 0) reEnt.totalPage++;
                IList<ApiClientBean> reList = new List<ApiClientBean>();
                foreach (var t in allList)
                {
                    var tmp = ClientSingle(ref err, new ApiRequesEntityBean { id = t.ID,authToken=inEnt.authToken });
                    reList.Add(tmp);
                }
                reEnt.data = reList;
            }
            return reEnt;
        }


        public ErrorInfo SalesmanClientRestPwd(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            ErrorInfo inBean = new ErrorInfo();
            using (DBEntities db = new DBEntities())
            {
                var user = db.YL_CLIENT.SingleOrDefault(x => x.ID == inEnt.id && x.SALESMAN_ID==gu.UserId);
                if (user == null)
                {
                    err.IsError = true;
                    err.Message = "用户不存在";
                    return err;
                }
                var login = db.YL_LOGIN.SingleOrDefault(x => x.LOGIN_NAME == user.YL_USER.LOGIN_NAME);
                login.PASSWORD = ProInterface.AppSet.DefaultPwd.Md5();
                inBean.IsError = false;
                inBean.Message = "用户密码，已经设置为:" + ProInterface.AppSet.DefaultPwd;
                db.SaveChanges();
            }
            return inBean;
        }

        public ApiCardBean CardSave(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiCardBean> inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            var inBean = inEnt.entity;
            if (string.IsNullOrEmpty(inBean.BANK_NAME) || string.IsNullOrEmpty(inBean.USER_NAME) || string.IsNullOrEmpty(inBean.CARD_NUMBER))
            {
                err.IsError = true;
                err.Message = "银行卡内容不能为空";
                return null;
            }
            using (DBEntities db = new DBEntities())
            {
                YL_USER_CARD ent = db.YL_USER_CARD.SingleOrDefault(x => x.ID == inBean.ID);
                bool isAdd = false;
                if (ent == null)
                {
                    ent = Fun.ClassToCopy<ApiCardBean, YL_USER_CARD>(inBean);
                    ent.ID = Fun.GetSeqID<YL_CAR>();
                    ent.STATUS = "正常";
                    ent.STATUS_TIME = DateTime.Now;
                    inBean.ID = ent.ID;
                    isAdd = true;
                }
                else
                {
                    ent = Fun.ClassToCopy(inBean, ent, inEnt.saveKeys.Split(','));
                }
                if (isAdd)
                {
                    db.YL_USER_CARD.Add(ent);
                }
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return null;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return null;
                }
            }
            return inBean;
        }

        public ApiPagingDataBean CardList(ref ErrorInfo err, ApiRequesPageBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            if (inEnt.pageSize == 0) inEnt.pageSize = 10;
            if (inEnt == null)
            {
                err.IsError = true;
                err.Message = "参数有误";
                return null;
            }
            ApiPagingDataBean reEnt = new ApiPagingDataBean();

            int skip = 0;
            if (inEnt.currentPage > 1)
            {
                skip = (inEnt.currentPage - 1) * inEnt.pageSize;
            }
            using (DBEntities db = new DBEntities())
            {
                var allData = db.YL_USER.Single(x => x.ID == gu.UserId).YL_USER_CARD.AsEnumerable();

                #region 过虑条件
                if (inEnt.searchKey != null)
                {
                    foreach (var filter in inEnt.searchKey)
                    {
                        allData = Fun.GetListWhere(allData, filter.K, filter.T, filter.V, ref err);
                    }
                }
                #endregion
                #region 排序

                if (allData == null)
                {
                    err.IsError = true;
                    return null;
                }
                foreach (var filter in inEnt.orderBy)
                {
                    allData = Fun.GetListOrder(allData, filter.K, filter.V, ref err);
                }
                #endregion
                var allList = allData.Skip(skip).Take(inEnt.pageSize).ToList();
                reEnt.currentPage = inEnt.currentPage;
                reEnt.pageSize = inEnt.pageSize;
                reEnt.totalCount = allData.Count();
                reEnt.totalPage = reEnt.totalCount / reEnt.pageSize;
                if (reEnt.totalCount % reEnt.pageSize != 0) reEnt.totalPage++;
                IList<ApiCardBean> reList = new List<ApiCardBean>();
                foreach (var t in allList)
                {
                    ApiCardBean tmp = Fun.ClassToCopy<YL_USER_CARD, ApiCardBean>(t);
                    reList.Add(tmp);
                }
                reEnt.data = reList;
            }
            return reEnt;
        }

        public ApiWithdrawBean WithdrawAdd(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiWithdrawBean> inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            var inBean = inEnt.entity;
            if (inBean.MONEY == 0 || string.IsNullOrEmpty(inBean.REMARK))
            {
                err.IsError = true;
                err.Message = "银行卡内容不能为空";
                return null;
            }
            using (DBEntities db = new DBEntities())
            {
                YL_USER_WITHDRAW ent = db.YL_USER_WITHDRAW.SingleOrDefault(x => x.ID == inBean.ID);
                bool isAdd = false;
                if (ent == null)
                {
                    ent = Fun.ClassToCopy<ApiWithdrawBean, YL_USER_WITHDRAW>(inBean);
                    ent.ID = Fun.GetSeqID<YL_USER_WITHDRAW>();
                    ent.STATUS = "申请";
                    ent.STATUS_TIME = DateTime.Now;
                    ent.CARETE_TIEM = DateTime.Now;
                    inBean.ID = ent.ID;
                    isAdd = true;
                }
                else
                {
                    ent = Fun.ClassToCopy(inBean, ent, inEnt.saveKeys.Split(','));
                }
                if (isAdd)
                {
                    db.YL_USER_WITHDRAW.Add(ent);
                }
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return null;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return null;
                }
            }
            return inBean;
        }

        public ApiPagingDataBean WithdrawList(ref ErrorInfo err, ApiRequesPageBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            if (inEnt.pageSize == 0) inEnt.pageSize = 10;
            if (inEnt == null)
            {
                err.IsError = true;
                err.Message = "参数有误";
                return null;
            }
            ApiPagingDataBean reEnt = new ApiPagingDataBean();

            int skip = 0;
            if (inEnt.currentPage > 1)
            {
                skip = (inEnt.currentPage - 1) * inEnt.pageSize;
            }
            using (DBEntities db = new DBEntities())
            {
                var allData = db.YL_USER.Single(x => x.ID == gu.UserId).YL_USER_WITHDRAW.AsEnumerable();

                #region 过虑条件
                if (inEnt.searchKey != null)
                {
                    foreach (var filter in inEnt.searchKey)
                    {
                        allData = Fun.GetListWhere(allData, filter.K, filter.T, filter.V, ref err);
                    }
                }
                #endregion
                #region 排序

                if (allData == null)
                {
                    err.IsError = true;
                    return null;
                }
                foreach (var filter in inEnt.orderBy)
                {
                    allData = Fun.GetListOrder(allData, filter.K, filter.V, ref err);
                }
                #endregion
                var allList = allData.Skip(skip).Take(inEnt.pageSize).ToList();
                reEnt.currentPage = inEnt.currentPage;
                reEnt.pageSize = inEnt.pageSize;
                reEnt.totalCount = allData.Count();
                reEnt.totalPage = reEnt.totalCount / reEnt.pageSize;
                if (reEnt.totalCount % reEnt.pageSize != 0) reEnt.totalPage++;
                IList<ApiWithdrawBean> reList = new List<ApiWithdrawBean>();
                foreach (var t in allList)
                {
                    var tmp = Fun.ClassToCopy<YL_USER_WITHDRAW, ApiWithdrawBean>(t);
                    reList.Add(tmp);
                }
                reEnt.data = reList;
            }
            return reEnt;
        }

        public ApiPagingDataBean CostList(ref ErrorInfo err, ApiRequesPageBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            if (inEnt.pageSize == 0) inEnt.pageSize = 10;
            if (inEnt == null)
            {
                err.IsError = true;
                err.Message = "参数有误";
                return null;
            }
            ApiPagingDataBean reEnt = new ApiPagingDataBean();

            int skip = 0;
            if (inEnt.currentPage > 1)
            {
                skip = (inEnt.currentPage - 1) * inEnt.pageSize;
            }
            using (DBEntities db = new DBEntities())
            {
                var allData = db.YL_COSTLIST.Where(x => x.USER_ID == gu.UserId).OrderByDescending(x => x.ID).AsEnumerable();


                #region 过虑条件
                if (inEnt.searchKey != null)
                {
                    foreach (var filter in inEnt.searchKey)
                    {
                        allData = Fun.GetListWhere(allData, filter.K, filter.T, filter.V, ref err);
                    }
                }
                #endregion

                #region 排序

                if (allData == null)
                {
                    err.IsError = true;
                    return null;
                }
                foreach (var filter in inEnt.orderBy)
                {
                    allData = Fun.GetListOrder(allData, filter.K, filter.V, ref err);
                }
                #endregion

                var allList = allData.Skip(skip).Take(inEnt.pageSize).ToList();

                reEnt.currentPage = inEnt.currentPage;
                reEnt.pageSize = inEnt.pageSize;
                reEnt.totalCount = allData.Count();
                reEnt.totalPage = reEnt.totalCount / reEnt.pageSize;
                if (reEnt.totalCount % reEnt.pageSize != 0) reEnt.totalPage++;
                IList<ApiCostListBean> reList = new List<ApiCostListBean>();
                foreach (var t in allList)
                {
                    var tmp = Fun.ClassToCopy<YL_COSTLIST, ApiCostListBean>(t);
                    if (t.CREATE_USER_ID != null)
                    {
                        tmp.CreateUserName = db.YL_USER.Single(x => x.ID == t.CREATE_USER_ID).NAME;
                    }
                    reList.Add(tmp);
                }
                reEnt.data = reList;
            }
            return reEnt;
        }

        public ApiPagingDataBean MessageGetAll(ref ErrorInfo err, ApiRequesPageBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            ApiPagingDataBean reEnt = new ApiPagingDataBean();

            int skip = 0;
            if (inEnt.currentPage > 1)
            {
                skip = (inEnt.currentPage - 1) * inEnt.pageSize;
            }
            if (inEnt.pageSize == 0) inEnt.pageSize = 10;


            using (DBEntities db = new DBEntities())
            {
                ErrorInfo error = new ErrorInfo();
                var allData = db.YL_USER_MESSAGE.Where(x => x.USER_ID == gu.UserId).OrderByDescending(x => x.STATUS_TIME).AsEnumerable();

                #region 过虑条件
                foreach (var filter in inEnt.searchKey)
                {
                    allData = Fun.GetListWhere(allData, filter.K, filter.T, filter.V, ref error);
                }
                #endregion


                #region 排序

                if (allData == null)
                {
                    err.IsError = true;
                    return null;
                }
                foreach (var filter in inEnt.orderBy)
                {
                    allData = Fun.GetListOrder(allData, filter.K, filter.V, ref err);
                }
                #endregion

                var allList = allData.Skip(skip).Take(inEnt.pageSize).ToList();

                reEnt.currentPage = inEnt.currentPage;
                reEnt.pageSize = inEnt.pageSize;
                reEnt.totalCount = allData.Count();
                reEnt.totalPage = reEnt.totalCount / reEnt.pageSize;
                if (reEnt.totalCount % reEnt.pageSize != 0) reEnt.totalPage++;

                IList<ApiSendMessageBean> reList = new List<ApiSendMessageBean>();
                foreach (var t in allData.ToList())
                {
                    ApiSendMessageBean tmp = Fun.ClassToCopy<YL_USER_MESSAGE, ApiSendMessageBean>(t);
                    tmp = Fun.ClassToCopy<YL_MESSAGE, ApiSendMessageBean>(t.YL_MESSAGE, tmp);
                    tmp.TypeName = t.YL_MESSAGE.YL_MESSAGE_TYPE.NAME;
                    reList.Add(tmp);
                    t.STATUS = "完成";
                    //t.PUSH_TYPE = "APP";
                }
                reEnt.data = reList;
                db.SaveChanges();
            }
            return reEnt;
        }

        public ErrorInfo MessageReply(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            ErrorInfo reEnt = new ErrorInfo();
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_USER_MESSAGE.SingleOrDefault(x => x.MESSAGE_ID == inEnt.id && x.USER_ID == gu.UserId);
                if (ent == null)
                {
                    err.IsError = true;
                    err.Message = "消息不存在";
                    return null;
                }
                ent.STATUS = "已阅";
                ent.STATUS_TIME = DateTime.Now;
                db.SaveChanges();

                reEnt.IsError = false;
                reEnt.Message = "回复成功";
            }
            return reEnt;
        }

        public ErrorInfo MessageSend(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            ProServer.Service s = new Service();
            ErrorInfo error = new ErrorInfo();

            ProInterface.Models.MESSAGE sendMsg = new ProInterface.Models.MESSAGE();

            var tmp = inEnt.para.SingleOrDefault(x => x.K == "TITLE");
            if (tmp != null)
            {
                sendMsg.TITLE = tmp.V;
            }

            tmp = inEnt.para.SingleOrDefault(x => x.K == "CONTENT");
            if (tmp != null)
            {
                sendMsg.CONTENT = tmp.V;
            }

            tmp = inEnt.para.SingleOrDefault(x => x.K == "RoleIdStr");
            if (tmp != null)
            {
                sendMsg.ALL_ROLE_ID = tmp.V;
            }

            tmp = inEnt.para.SingleOrDefault(x => x.K == "DistrictId");
            if (tmp != null)
            {

                sendMsg.DISTRICT_ID = Convert.ToInt32(tmp.V);
            }

            string allUserId = "";
            tmp = inEnt.para.SingleOrDefault(x => x.K == "UserIDStr");
            if (tmp != null)
            {
                allUserId = tmp.V;
            }
            s.UserMessageSave(inEnt.authToken, ref error, sendMsg, null, allUserId);
            return error;
        }

        public ErrorInfo MessageDelete(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            ErrorInfo reEnt = new ErrorInfo();
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_MESSAGE.SingleOrDefault(x => x.ID == inEnt.id);
                if (ent == null)
                {
                    err.IsError = true;
                    err.Message = "消息不存在";
                    return null;
                }
                foreach (var t in ent.YL_USER_MESSAGE.ToList())
                {
                    db.YL_USER_MESSAGE.Remove(t);
                }
                db.YL_MESSAGE.Remove(ent);
                db.SaveChanges();

                reEnt.IsError = false;
                reEnt.Message = "删除成功";
            }
            return reEnt;
        }


        public ErrorInfo PayAlipaySign(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            throw new NotImplementedException();
        }


        public ApiAddressBean AddressSave(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiAddressBean> inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            var inBean = inEnt.entity;
            if (inEnt.saveKeys != null && string.IsNullOrEmpty(inEnt.entity.ADDRESS))
            {
                err.IsError = true;
                err.Message = "详细地址不能为空";
                return null;
            }
            using (DBEntities db = new DBEntities())
            {
                YL_USER_ADDRESS ent = db.YL_USER_ADDRESS.SingleOrDefault(x => x.ID == inBean.ID);
                var client = db.YL_USER.Single(x => x.ID == gu.UserId);
                foreach (var t in client.YL_USER_ADDRESS.ToList())
                {
                    t.IS_DEFAULT = 0;
                }
                bool isAdd = false;
                if (ent == null)
                {
                    ent = Fun.ClassToCopy<ApiAddressBean, YL_USER_ADDRESS>(inBean);
                    ent.ID = Fun.GetSeqID<YL_USER_ADDRESS>();
                    ent.USER_ID = gu.UserId;
                    ent.IS_DEFAULT = 1;
                    inBean.ID = ent.ID;
                    isAdd = true;
                }
                else
                {
                    ent = Fun.ClassToCopy(inBean, ent, inEnt.saveKeys.Split(','));
                }
                if (isAdd)
                {
                    db.YL_USER_ADDRESS.Add(ent);
                }
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return null;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return null;
                }
            }
            return inBean;
        }

        public ApiPagingDataBean AddressList(ref ErrorInfo err, ApiRequesPageBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            if (inEnt.pageSize == 0) inEnt.pageSize = 10;
            if (inEnt == null)
            {
                err.IsError = true;
                err.Message = "参数有误";
                return null;
            }
            ApiPagingDataBean reEnt = new ApiPagingDataBean();

            int skip = 0;
            if (inEnt.currentPage > 1)
            {
                skip = (inEnt.currentPage - 1) * inEnt.pageSize;
            }
            using (DBEntities db = new DBEntities())
            {
                var allData = db.YL_USER.Single(x => x.ID == gu.UserId).YL_USER_ADDRESS.AsEnumerable();

                #region 过虑条件
                if (inEnt.searchKey != null)
                {
                    foreach (var filter in inEnt.searchKey)
                    {
                        allData = Fun.GetListWhere(allData, filter.K, filter.T, filter.V, ref err);
                    }
                }
                #endregion

                #region 排序

                if (allData == null)
                {
                    err.IsError = true;
                    return null;
                }
                foreach (var filter in inEnt.orderBy)
                {
                    allData = Fun.GetListOrder(allData, filter.K, filter.V, ref err);
                }
                #endregion

                var allList = allData.Skip(skip).Take(inEnt.pageSize).ToList();

                reEnt.currentPage = inEnt.currentPage;
                reEnt.pageSize = inEnt.pageSize;
                reEnt.totalCount = allData.Count();
                reEnt.totalPage = reEnt.totalCount / reEnt.pageSize;
                if (reEnt.totalCount % reEnt.pageSize != 0) reEnt.totalPage++;
                IList<ApiAddressBean> reList = new List<ApiAddressBean>();
                foreach (var t in allList)
                {
                    ApiAddressBean tmp = Fun.ClassToCopy<YL_USER_ADDRESS, ApiAddressBean>(t);
                    reList.Add(tmp);
                }
                reEnt.data = reList;
            }
            return reEnt;
        }

        public ErrorInfo AddressSetDefault(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            using (DBEntities db = new DBEntities())
            {
                var user = db.YL_USER.Single(x => x.ID == gu.UserId);
                foreach (var t in user.YL_USER_ADDRESS.ToList())
                {
                    if (t.ID == inEnt.id)
                    {
                        t.IS_DEFAULT = 1;
                    }
                    else
                    {
                        t.IS_DEFAULT = 0;
                    }
                }
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return null;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return null;
                }
                return new ErrorInfo();
            }
        }

        public ErrorInfo AddressDelete(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            using (DBEntities db = new DBEntities())
            {
                var user = db.YL_USER.Single(x => x.ID == gu.UserId);

                var car = user.YL_USER_ADDRESS.SingleOrDefault(x => x.ID == inEnt.id);
                if (car != null)
                {
                    user.YL_USER_ADDRESS.Remove(car);
                }
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return null;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return null;
                }
                return new ErrorInfo();
            }
        }

        public ApiAddressBean AddressSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_USER_ADDRESS.SingleOrDefault(x => x.ID == inEnt.id);
                ApiAddressBean reEnt = Fun.ClassToCopy<YL_USER_ADDRESS, ApiAddressBean>(ent);
                return reEnt;
            }
        }

        public ApiWeiXinJSSDKBean WenXinJSSDKSign(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            throw new NotImplementedException();
        }

        public ErrorInfo WeixinGetOpenid(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            using (DBEntities db = new DBEntities())
            {
                GlobalUser gu = Global.GetUser(inEnt.authToken);
                if (gu == null)
                {
                    err.IsError = true;
                    err.Message = "登录超时";
                    return null;
                }
                var ent = db.YL_WEIXIN_USER.SingleOrDefault(x => x.USER_ID == gu.UserId);
                if (ent != null)
                {
                    err.Message = ent.OPENID;
                }
                return err;
            }
        }

        public ErrorInfo WeixinSendMsg(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {

            string EncodingAESKey = WebConfigurationManager.AppSettings["WeixinEncodingAESKey"];//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
            string AppId = WebConfigurationManager.AppSettings["WeixinAppId"];//与微信公众账号后台的AppId设置保持一致，区分大小写。
            string AppSecret = WebConfigurationManager.AppSettings["WeixinAppSecret"];//与微信公众账号后台的AppId设置保持一致，区分大小写。

            if (!AccessTokenContainer.CheckRegistered(AppId))
            {
                AccessTokenContainer.Register(AppId, AppSecret);
            }
            var result = AccessTokenContainer.GetAccessTokenResult(AppId); //CommonAPIs.CommonApi.GetToken(appId, appSecret);

            var codeMsg = WeixinGetOpenid(ref err, inEnt);
            var tmp = inEnt.para.FirstOrDefault(x => x.K == "Msg");
            string msg = "";
            if (tmp != null)
            {
                msg = tmp.V;
            }
            var wxJsonResult = Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(result.access_token, codeMsg.Message, msg);
            if (wxJsonResult.errcode != 0)
            {
                err.IsError = true;
                err.Message = wxJsonResult.errmsg;
            }
            else
            {
                err.IsError = false;
            }
            return err;
        }

        public ProInterface.Models.YL_WEIXIN_USER WeixinGetUser(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_WEIXIN_USER.SingleOrDefault(x => x.OPENID == inEnt.authToken);
                if (ent != null)
                {
                    return Fun.ClassToCopy<YL_WEIXIN_USER, ProInterface.Models.YL_WEIXIN_USER>(ent);
                }
                err.IsError = true;
                err.Message = "参数有误";
                return null;
            }
        }

        public ApiPagingDataBean<YlTeam> TeamMyAll(ref ErrorInfo err, ApiRequesPageBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            if (inEnt.pageSize == 0) inEnt.pageSize = 10;
            if (inEnt == null)
            {
                err.IsError = true;
                err.Message = "参数有误";
                return null;
            }
            ApiPagingDataBean<YlTeam> reEnt = new ApiPagingDataBean<YlTeam>();

            int skip = 0;
            if (inEnt.currentPage > 1)
            {
                skip = (inEnt.currentPage - 1) * inEnt.pageSize;
            }
            using (DBEntities db = new DBEntities())
            {
                var allData = db.YL_TEAM.ToList().Where(x => x.LEAD_ID_STR!=null && x.LEAD_ID_STR.Split(',').ToList().Contains(gu.UserId.ToString())).AsEnumerable();

                var allList = allData.Skip(skip).Take(inEnt.pageSize).ToList();
                reEnt.currentPage = inEnt.currentPage;
                reEnt.pageSize = inEnt.pageSize;
                reEnt.totalCount = allData.Count();
                reEnt.totalPage = reEnt.totalCount / reEnt.pageSize;
                if (reEnt.totalCount % reEnt.pageSize != 0) reEnt.totalPage++;
                IList<YlTeam> reList = new List<YlTeam>();
                foreach (var t in allList)
                {
                    inEnt.id = t.ID;
                    var tmp = TeamSingle(ref err, inEnt);
                    reList.Add(tmp);
                }
                reEnt.data = reList;
            }
            return reEnt;
        }

        public YlTeam TeamSingle(ref ErrorInfo err, ApiRequesPageBean inEnt)
        {
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_TEAM.SingleOrDefault(x => x.ID == inEnt.id);

                YlTeam reEnt = Fun.ClassToCopy<YL_TEAM, YlTeam>(ent);

                var allSalesman = ent.YL_SALESMAN.ToList();
                foreach (var t in allSalesman)
                {
                    reEnt.allSalesman.Add(SalesmanSingle(ref err, new ApiRequesEntityBean { authToken = inEnt.authToken, id = t.ID }));
                }

                var allSalesmanId = allSalesman.Select(x => x.ID).ToList();
                var allClient = db.YL_CLIENT.Where(x => allSalesmanId.Contains(x.SALESMAN_ID));
                var allClientIdList = allClient.Select(x => x.ID).ToList();
                var allOrder = db.YL_ORDER.Where(x => allClientIdList.Contains(x.CLIENT_ID)).AsEnumerable();

                DateTime timeStart = new DateTime();
                DateTime timeEnd = new DateTime();
                var tmpKey = inEnt.searchKey.SingleOrDefault(x => x.K == "timeStart");
                if (tmpKey != null && !string.IsNullOrEmpty(tmpKey.V) && tmpKey.V.IsDateTime())
                {
                    timeStart = Convert.ToDateTime(tmpKey.V);
                    allOrder = allOrder.Where(x => x.CREATE_TIME > timeStart);
                }
                tmpKey = inEnt.searchKey.SingleOrDefault(x => x.K == "timeEnd");
                if (tmpKey != null && !string.IsNullOrEmpty(tmpKey.V) && tmpKey.V.IsDateTime())
                {
                    timeEnd = Convert.ToDateTime(tmpKey.V);
                    allOrder = allOrder.Where(x => x.CREATE_TIME < timeEnd);
                }

                int skip = 0;
                if (inEnt.currentPage > 1)
                {
                    skip = (inEnt.currentPage - 1) * inEnt.pageSize;
                }
                var allList = allOrder.Skip(skip).Take(inEnt.pageSize).ToList();
                foreach (var t in allList)
                {
                    var tmp = Fun.ClassToCopy<YL_ORDER, ApiOrderBean>(t);
                    tmp.ClientName = t.YL_CLIENT.YL_USER.NAME;
                    tmp.ClientPhone = t.YL_CLIENT.YL_USER.LOGIN_NAME;
                    if (string.IsNullOrEmpty(t.ORDER_NO))
                    {
                        string idStr = "0000" + t.ID;
                        idStr = idStr.Substring(idStr.Length - 4);
                        tmp.ORDER_NO = t.CREATE_TIME.ToString("yyyyMMddhhmm") + idStr;
                    }
                    tmp.LastStatus = "新建";
                    var allFlow = t.YL_ORDER_FLOW.OrderByDescending(x => x.LEVEL_ID).ToList();
                    if (allFlow.Count() > 0)
                    {
                        if (tmp.COST == null || tmp.COST == 0)
                        {
                            tmp.COST = allFlow.Sum(x => x.COST);
                        }
                        tmp.LastStatus = allFlow[0].SUBJECT;
                    }
                    if (t.YL_CAR != null)
                    {
                        tmp.CarPlateNumber = t.YL_CAR.PLATE_NUMBER;
                    }

                    if (t.YL_ORDER_RESCUE != null)
                    {
                        tmp.AddressStr = t.YL_ORDER_RESCUE.ADDRESS;
                    }
                    var taskFlow = db.YL_TASK_FLOW.SingleOrDefault(x => x.ID == tmp.ID);
                    if (taskFlow != null && taskFlow.YL_TASK != null)
                    {
                        tmp.STATUS = taskFlow.YL_TASK.STATUS;
                        if (taskFlow.YL_TASK.STATUS_TIME != null)
                        {
                            tmp.STATUS_TIME = taskFlow.YL_TASK.STATUS_TIME.Value;
                        }
                    }
                    reEnt.allOrder.Add(tmp);
                }
                reEnt.orderNum = reEnt.allOrder.Count;
                reEnt.orderNumSucc = reEnt.allOrder.Where(x => x.PAY_STATUS == "完成").Count();
                reEnt.qrcode = string.Format("~/File/QrCode/team_{0}.jpg|~/File/QrCode/team_weixin_{0}.jpg", inEnt.id);
                return reEnt;
            }
        }


        public ErrorInfo BbsDelete(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            ErrorInfo reEnt = new ErrorInfo();
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_BBS.SingleOrDefault(x => x.ID == inEnt.id);
                if (ent == null)
                {
                    err.IsError = true;
                    err.Message = "帖子不存在";
                    return null;
                }
                if (ent.USER_ID != gu.UserId)
                {
                    err.IsError = true;
                    err.Message = string.Format("删除失败：该帖子是由【{0}】发起的", ent.YL_USER.NAME);
                    return null;
                }

                //var nowEnt = ent.YL_BBS1.ToList();
                //if (nowEnt.Count() > 0)
                //{
                //    ent.CONTENT = "该内容已经被删除";
                //    reEnt.IsError = false;
                //    reEnt.Message = "清除内容成功";
                //}
                //else
                //{

                foreach (var t in ent.YL_BBS1.ToList())
                {
                    t.YL_USER1.Clear();
                    t.YL_FILES.Clear();
                    db.YL_BBS.Remove(t);
                }
                ent.YL_USER1.Clear();
                ent.YL_FILES.Clear();
                db.YL_BBS.Remove(ent);
                reEnt.IsError = false;
                reEnt.Message = "删除成功";
                //}
                db.SaveChanges();
            }
            return reEnt;
        }
        public ErrorInfo BbsGood(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            ErrorInfo reEnt = new ErrorInfo();
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_BBS.SingleOrDefault(x => x.ID == inEnt.id);
                if (ent == null)
                {
                    err.IsError = true;
                    err.Message = "帖子不存在";
                    return null;
                }
                var nowEnt = ent.YL_USER1.SingleOrDefault(x => x.ID == gu.UserId);
                if (ent != null && nowEnt == null)
                {
                    ent.YL_USER1.Add(db.YL_USER.Single(x => x.ID == gu.UserId));
                    reEnt.IsError = false;
                    reEnt.Message = "赞成功";


                    var rootBbs = ent;
                    while (rootBbs.YL_BBS2 != null)
                    {
                        rootBbs = rootBbs.YL_BBS2;
                    }

                    ProInterface.Models.MESSAGE sendMsg = new ProInterface.Models.MESSAGE();
                    sendMsg.MESSAGE_TYPE_ID = 2;
                    sendMsg.TITLE = ent.TYPE;
                    sendMsg.KEY = rootBbs.ID;
                    sendMsg.CONTENT = string.Format("【{0}】赞了你【{1}】", gu.UserName, sendMsg.TITLE);
                    sendMsg.PUSH_TYPE = "智能推送";
                    new Service().UserMessageSave(inEnt.authToken, ref err, sendMsg, null, ent.USER_ID.ToString());
                }
                else
                {
                    ent.YL_USER1.Remove(nowEnt);
                    reEnt.IsError = false;
                    reEnt.Message = "取消赞";
                }
                db.SaveChanges();
            }
            return reEnt;
        }
        public ApiPagingDataBean BbsList(ref ErrorInfo err, ApiRequesPageBean inEnt)
        {
            if (inEnt.pageSize == 0) inEnt.pageSize = 10;

            if (inEnt == null)
            {
                err.IsError = true;
                err.Message = "参数有误";
                return null;
            }
            ApiPagingDataBean reEnt = new ApiPagingDataBean();


            int skip = 0;
            if (inEnt.currentPage > 1)
            {
                skip = (inEnt.currentPage - 1) * inEnt.pageSize;
            }
            using (DBEntities db = new DBEntities())
            {
                var allData = db.YL_BBS.OrderByDescending(x => x.ID).ToList();

                if (inEnt.id == 0)
                {
                    allData = allData.Where(x => x.PARENT_ID == null).ToList();
                }
                else
                {
                    allData = allData.Where(x => x.PARENT_ID == inEnt.id).ToList();
                }

                ErrorInfo error = new ErrorInfo();

                var serarch = inEnt.searchKey.SingleOrDefault(x => x.K == "searchName");
                if (serarch != null)
                {
                    var k = serarch.V;
                    if (!string.IsNullOrEmpty(k))
                    {
                        allData = allData.Where(x =>
                        (x.NAME != null && x.NAME.IndexOf(k) > -1)
                        || (x.STATUS != null && x.STATUS.IndexOf(k) > -1)
                        || (x.CONTENT != null && x.CONTENT.IndexOf(k) > -1)
                        || (x.YL_MESSAGE_TYPE != null && x.YL_MESSAGE_TYPE.NAME.IndexOf(k) > -1)
                        || (x.YL_MESSAGE_TYPE != null && x.YL_MESSAGE_TYPE.REMARK != null && x.YL_MESSAGE_TYPE.REMARK.IndexOf(k) > -1)
                        ).ToList();
                    }
                    inEnt.searchKey.Remove(serarch);
                }



                var pareType = inEnt.para.SingleOrDefault(x => x.K == "type");
                if (pareType != null)
                {
                    GlobalUser gu = Global.GetUser(inEnt.authToken);
                    if (gu == null)
                    {
                        err.IsError = true;
                        err.Message = "登录超时";
                        return null;
                    }

                    //1获取我参与的评论或我发起的;
                    //2获取我发起的
                    switch (pareType.T)
                    {
                        case "1":
                            allData = allData.Where(x => x.USER_ID == gu.UserId || x.YL_BBS1.Where(y => y.USER_ID == gu.UserId).Count() > 0).ToList();
                            break;
                        case "2":
                            allData = allData.Where(x => x.USER_ID == gu.UserId).ToList();
                            break;
                    }
                    inEnt.para.Remove(pareType);
                }

                var isReplyPara = inEnt.searchKey.SingleOrDefault(x => x.K == "isReply");
                if (isReplyPara != null)
                {
                    GlobalUser gu = Global.GetUser(inEnt.authToken);
                    if (gu == null)
                    {
                        err.IsError = true;
                        err.Message = "登录超时";
                        return null;
                    }
                    inEnt.searchKey.Remove(isReplyPara);
                    switch (isReplyPara.V)
                    {
                        case "1"://已经回复
                            allData = allData.Where(x => x.YL_BBS1.Where(y => y.USER_ID == gu.UserId).Count() > 0).ToList();
                            break;
                        case "2":
                            allData = allData.Where(x => x.YL_BBS1.Where(y => y.USER_ID == gu.UserId).Count() == 0).ToList();
                            break;
                    }

                }

                #region 过虑条件
                if (inEnt.searchKey != null)
                {
                    foreach (var filter in inEnt.searchKey)
                    {
                        allData = Fun.GetListWhere<YL_BBS>(allData, filter.K, filter.T, filter.V, ref error).ToList();
                    }
                }
                #endregion


                #region 排序

                if (allData == null)
                {
                    err.IsError = true;
                    return null;
                }
                foreach (var filter in inEnt.orderBy)
                {
                    allData = Fun.GetListOrder<YL_BBS>(allData, filter.K, filter.V, ref error).ToList();
                }

                #endregion

                var allList = allData.Skip(skip).Take(inEnt.pageSize).ToList();

                reEnt.currentPage = inEnt.currentPage;
                reEnt.pageSize = inEnt.pageSize;
                reEnt.totalCount = allData.Count();
                reEnt.totalPage = reEnt.totalCount / reEnt.pageSize;
                if (reEnt.totalCount % reEnt.pageSize != 0) reEnt.totalPage++;
                IList<Ylbbs> reList = new List<Ylbbs>();
                foreach (var t in allList)
                {
                    Ylbbs tmp = Fun.ClassToCopy<YL_BBS, Ylbbs>(t);
                    tmp.addUserName = t.YL_USER.NAME;
                    if (t.YL_MESSAGE_TYPE != null)
                    {
                        tmp.CaseTypeName = t.YL_MESSAGE_TYPE.NAME;
                    }
                    tmp.goodNum = t.YL_USER1.Count();
                    tmp.goodUserName = t.YL_USER1.Select(x => x.NAME).ToArray();
                    tmp.reviewNum = t.YL_BBS1.Count();
                    tmp.userPhone = t.YL_USER.LOGIN_NAME;
                    if (t.YL_USER.ICON_FILES_ID != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == t.YL_USER.ICON_FILES_ID);
                        if (image != null) tmp.iconURL = image.URL;
                    }
                    tmp.AllFiles = Fun.ClassListToCopy<YL_FILES, FILES>(t.YL_FILES.ToList());
                    foreach (var t0 in t.YL_BBS1.ToList().OrderBy(x => x.ID).ToList())
                    {
                        Ylbbs childTmp = Fun.ClassToCopy<YL_BBS, Ylbbs>(t0);
                        childTmp.addUserName = t0.YL_USER.NAME;
                        tmp.AllChildrenItem.Add(childTmp);
                    }
                    if (t.YL_DISTRICT != null)
                    {
                        tmp.distictName = t.YL_DISTRICT.NAME;
                    }
                    reList.Add(tmp);
                }
                reEnt.data = reList;
            }
            return reEnt;
        }
        public Ylbbs BbsSave<T>(ref ErrorInfo err, ApiRequesSaveEntityBean<T> inEnt)
        {
            GlobalUser gu = Global.GetUser(inEnt.authToken);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return null;
            }
            Ylbbs inBean = inEnt.entity as Ylbbs;

            if (inEnt.saveKeys.IndexOf("CONTENT") > -1 && string.IsNullOrEmpty(inBean.CONTENT))
            {
                err.IsError = true;
                err.Message = "内容不能为空";
                return null;
            }

            if (inBean.DISTRICT_ID == 0)
            {
                inBean.DISTRICT_ID = null;
            }



            using (DBEntities db = new DBEntities())
            {
                YL_BBS ent = db.YL_BBS.SingleOrDefault(x => x.ID == inBean.ID);
                bool isAdd = false;
                if (ent == null)
                {
                    inBean.USER_ID = gu.UserId;
                    inBean.STATUS = "正常";
                    inBean.STATUS_TIME = DateTime.Now;
                    inBean.ADD_TIME = DateTime.Now;
                    ent = Fun.ClassToCopy<Ylbbs, YL_BBS>(inBean);
                    ent.ID = Fun.GetSeqID<YL_BBS>();
                    if (ent.PARENT_ID != null && ent.PARENT_ID != 0)
                    {
                        var parentBbs = db.YL_BBS.SingleOrDefault(x => x.ID == ent.PARENT_ID);
                        ent.ID = Fun.GetSeqID<YL_BBS>();

                        var rootBbs = parentBbs;
                        while (rootBbs.YL_BBS2 != null)
                        {
                            rootBbs = rootBbs.YL_BBS2;
                        }

                        ent.MESSAGE_TYPE_ID = parentBbs.MESSAGE_TYPE_ID;
                        ent.TYPE = parentBbs.TYPE;
                        if (gu.UserId != parentBbs.USER_ID)
                        {
                            ProInterface.Models.MESSAGE sendMsg = new ProInterface.Models.MESSAGE();
                            switch (inBean.TYPE)
                            {
                                case "经验交流":
                                    sendMsg.MESSAGE_TYPE_ID = 3;
                                    break;
                                case "BUG提交":
                                    sendMsg.MESSAGE_TYPE_ID = 2;
                                    break;
                                case "问题咨询":
                                    sendMsg.MESSAGE_TYPE_ID = 1;
                                    break;
                                default:
                                    err.IsError = true;
                                    err.Message = "类型不正解";
                                    return null;
                            }
                            sendMsg.TITLE = parentBbs.TYPE;
                            sendMsg.KEY = rootBbs.ID;
                            sendMsg.CONTENT = string.Format("【{0}】回复了你", gu.UserName);
                            sendMsg.PUSH_TYPE = "智能推送";
                            new Service().UserMessageSave(inEnt.authToken, ref err, sendMsg, null, parentBbs.USER_ID.ToString());
                        }
                    }

                    if (ent.MESSAGE_TYPE_ID == 0) ent.MESSAGE_TYPE_ID = 1;

                    inBean.ID = ent.ID;
                    isAdd = true;
                }
                else
                {
                    ent = Fun.ClassToCopy(inBean, ent, inEnt.saveKeys.Split(','));
                }

                ent.YL_FILES.Clear();
                var allFileId = inBean.AllFiles.Select(x => x.ID).ToList();
                ent.YL_FILES = db.YL_FILES.Where(x => allFileId.Contains(x.ID)).ToList();


                if (isAdd)
                {
                    db.YL_BBS.Add(ent);
                }

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return null;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return null;
                }
            }
            return inBean;
        }
        public Ylbbs BbsSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            //GlobalUser gu = Global.GetUser(inEnt.authToken);
            //if (gu == null)
            //{
            //    err.IsError = true;
            //    err.Message = "登录超时";
            //    return null;
            //}
            Ylbbs reEnt = new Ylbbs();
            using (DBEntities db = new DBEntities())
            {
                var t = db.YL_BBS.SingleOrDefault(x => x.ID == inEnt.id);
                if (t == null)
                {
                    err.IsError = true;
                    err.Message = "登录超时";
                    return null;
                }
                reEnt = Fun.ClassToCopy<YL_BBS, Ylbbs>(t);
                reEnt.addUserName = t.YL_USER.NAME;
                reEnt.CaseTypeName = t.YL_MESSAGE_TYPE.NAME;
                reEnt.goodNum = t.YL_USER1.Count();
                reEnt.goodUserName = t.YL_USER1.Select(x => x.NAME).ToArray();
                reEnt.reviewNum = t.YL_BBS1.Count();

                reEnt.userRole = "业务员";
                if (t.YL_USER.YL_SALESMAN == null)
                {
                    reEnt.userRole = "用户";
                }


                var login = db.YL_LOGIN.SingleOrDefault(x => x.LOGIN_NAME == t.YL_USER.LOGIN_NAME);
                if (login != null)
                {
                    reEnt.userPhone = login.PHONE_NO;
                }

                if (t.YL_USER.ICON_FILES_ID != null)
                {
                    var image = db.YL_FILES.SingleOrDefault(x => x.ID == t.YL_USER.ICON_FILES_ID);
                    if (image != null) reEnt.iconURL = image.URL;
                }

                reEnt.AllFiles = Fun.ClassListToCopy<YL_FILES, FILES>(t.YL_FILES.ToList());
                foreach (var child in t.YL_BBS1.ToList().OrderBy(x => x.ADD_TIME).ToList())
                {
                    Ylbbs tmp = new Ylbbs();

                    tmp = Fun.ClassToCopy<YL_BBS, Ylbbs>(child);
                    tmp.addUserName = child.YL_USER.NAME;
                    tmp.CaseTypeName = child.YL_MESSAGE_TYPE.NAME;
                    tmp.goodNum = child.YL_USER1.Count();
                    tmp.goodUserName = child.YL_USER1.Select(x => x.NAME).ToArray();
                    reEnt.userRole = "业务员";
                    if (t.YL_USER.YL_SALESMAN == null)
                    {
                        reEnt.userRole = "用户";
                    }
                    tmp.reviewNum = child.YL_BBS1.Count();

                    if (child.YL_USER.ICON_FILES_ID != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == child.YL_USER.ICON_FILES_ID);
                        if (image != null) tmp.iconURL = image.URL;
                    }
                    login = db.YL_LOGIN.SingleOrDefault(x => x.LOGIN_NAME == child.YL_USER.LOGIN_NAME);
                    if (login != null)
                    {
                        tmp.userPhone = login.PHONE_NO;
                    }

                    foreach (var child_1 in child.YL_BBS1.OrderByDescending(x => x.ADD_TIME).ToList())
                    {
                        Ylbbs tmp_1 = new Ylbbs();

                        tmp_1 = Fun.ClassToCopy<YL_BBS, Ylbbs>(child_1);
                        tmp_1.addUserName = child_1.YL_USER.NAME;
                        tmp_1.CaseTypeName = child_1.YL_MESSAGE_TYPE.NAME;
                        tmp_1.goodNum = child_1.YL_USER1.Count();
                        tmp_1.goodUserName = child_1.YL_USER1.Select(x => x.NAME).ToArray();
                        reEnt.userRole = "业务员";
                        if (t.YL_USER.YL_SALESMAN == null)
                        {
                            reEnt.userRole = "用户";
                        }
                        tmp_1.reviewNum = child_1.YL_BBS1.Count();

                        if (child_1.YL_USER.ICON_FILES_ID != null)
                        {
                            var image = db.YL_FILES.SingleOrDefault(x => x.ID == child_1.YL_USER.ICON_FILES_ID);
                            if (image != null) tmp_1.iconURL = image.URL;
                        }
                        login = db.YL_LOGIN.SingleOrDefault(x => x.LOGIN_NAME == child_1.YL_USER.LOGIN_NAME);
                        if (login != null)
                        {
                            tmp_1.userPhone = login.PHONE_NO;
                        }

                        tmp.AllChildrenItem.Add(tmp_1);
                    }


                    reEnt.AllChildrenItem.Add(tmp);
                }
            }
            return reEnt;
        }

        public IList<ProInterface.Models.YL_MESSAGE_TYPE> MessageTypeAll(ref ErrorInfo err, ApiRequesEntityBean inEnt)
        {
            using (DBEntities db = new DBEntities())
            {
                return Fun.ClassListToCopy<YL_MESSAGE_TYPE, ProInterface.Models.YL_MESSAGE_TYPE>(db.YL_MESSAGE_TYPE.ToList());
            }
        }


    }
}
