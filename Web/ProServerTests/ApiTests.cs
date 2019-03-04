using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProInterface;
using ProInterface.Models;
using ProInterface.Models.Api;
using ProServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProServer.Tests
{
    [TestClass()]
    public class ApiTests
    {
        const string url = "http://127.0.0.1/YL/WebApi/";
        string loginKey = "8ac76a18fc99414280159de00036f899";
        int startNum = 1;
        Api api = new Api();

        public ApiTests()
        {
            startNum = Convert.ToInt32(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\startNum.txt"));
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\startNum.txt", Convert.ToString(startNum + 1));

            loginKey =File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\loginKey.txt");
            if (string.IsNullOrEmpty(loginKey))
            {
                ErrorInfo err = new ErrorInfo();
                loginKey = SalesmanLogin("18180770313", "123456", ref err);
            }
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\loginKey.txt", loginKey);
        }

        [TestMethod()]
        public void SalesmanClientAddTest()
        {
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SalesmanClientListTest()
        {
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void LoginRegTest()
        {
            ApiLogingBean postBean = new ApiLogingBean();
            postBean.loginName = "test" + startNum;
            postBean.password = "ss";
            string postStr = JSON.DecodeToStr(postBean);
            Console.Write(postStr);
            var jsonStr = ProServer.Fun.ExecutePostJson(url + "LoginReg", JSON.DecodeToStr(postBean), null);
            if (jsonStr.IndexOf("{\"IsError\":") == 0)
            {
                var errObj = JSON.EncodeToEntity<ProInterface.ErrorInfo>(jsonStr);
                Assert.IsFalse(errObj.IsError, errObj.Message);
            }
            Console.Write(jsonStr);
            using (DBEntities db = new DBEntities())
            {
                var user = db.YL_USER.SingleOrDefault(x => x.LOGIN_NAME == postBean.loginName);
                Assert.IsTrue(user != null);
            }
            Assert.IsTrue(false);
        }

        [TestMethod()]
        public void OrderSaveTest()
        {
            ApiRequesSaveEntityBean<YlOrder> postBean = new ApiRequesSaveEntityBean<YlOrder>();
            postBean.authToken = loginKey;
            postBean.entity = new YlOrder
            {
                Client = new YlClient { LOGIN_NAME = "18180770313", NAME = "翁志来测试" },
                ORDER_TYPE = "审车",
                Car = new YlCar { PLATE_NUMBER = "川A427HW" }
            };
            string postStr = JSON.DecodeToStr(postBean);
            Console.Write(postStr);
            var jsonStr = ProServer.Fun.ExecutePostJson(url + "OrderSave", JSON.DecodeToStr(postBean), null);
            if (jsonStr.IndexOf("{\"IsError\":") == 0)
            {
                var errObj = JSON.EncodeToEntity<ProInterface.ErrorInfo>(jsonStr);
                Assert.IsFalse(errObj.IsError, errObj.Message);
            }
            Assert.IsTrue(false);
        }

        [TestMethod()]
        public void OrderInsureSaveTest()
        {
            ApiRequesSaveEntityBean<YlOrderInsure> postBean = new ApiRequesSaveEntityBean<YlOrderInsure>();
            ErrorInfo err = new ErrorInfo();
            postBean.authToken = loginKey;
            postBean.entity = new YlOrderInsure
            {
                Client = new YlClient { LOGIN_NAME = "18180770313", NAME = "翁志来测试" },
                ORDER_TYPE = "投保",
                Car = new YlCar { PLATE_NUMBER = "川A427HW" },
                INSURER_ID = 1,
                DELIVERY = "配送地址",
                SaveProductId = new List<YlOrderInsureProduct> { new YlOrderInsureProduct { INSURER_ID = 1, PRODUCT_ID = 4, MAX_PAY = "30万" } }
            };

            var reBean = api.OrderInsureSave(ref err, postBean);
            if (err.IsError)
            {
                Assert.IsFalse(err.IsError, "出错原因：" + err.Message);
            }
            else {
                Assert.IsTrue(true);
            }
        }

        public string SalesmanLogin(string loginName,string password,ref ErrorInfo err)
        {
            ApiLogingBean postBean = new ApiLogingBean();
            postBean.loginName = loginName;
            postBean.password = password;
            var reBean= api.SalesmanLogin(ref err, postBean);
            if (err.IsError)
            {
                return null;
            }
            else
            {
                return reBean.authToken;
            }
        }

        [TestMethod()]
        public void SalesmanLoginTest()
        {
            ApiLogingBean postBean = new ApiLogingBean();
            postBean.loginName = "18180770313";
            postBean.password = "123456";
            Api ser = new Api();
            ErrorInfo err = new ErrorInfo();
            ser.SalesmanLogin(ref err, postBean);
            if (err.IsError)
            {
                Assert.IsFalse(err.IsError, "出错原因："+err.Message);
            }
            else
            {
                Assert.IsTrue(true);
            }

        }
    }
}