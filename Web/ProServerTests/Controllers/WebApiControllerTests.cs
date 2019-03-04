using Microsoft.VisualStudio.TestTools.UnitTesting;
using Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProInterface.Models.Api;
using ProInterface;
using ProServer;
using System.Web.Mvc;

namespace Web.Controllers.Tests
{
    [TestClass()]
    public class WebApiControllerTests
    {
        const string url = "http://127.0.0.1/YL/WebApi/";
        string loginKey = "8ac76a18fc99414280159de00036f899";
        int startNum = 1;
        Api api = new Api();

        public string SalesmanLogin(string loginName, string password, ref ErrorInfo err)
        {
            ApiLogingBean postBean = new ApiLogingBean();
            postBean.loginName = loginName;
            postBean.password = password;
            var reBean = api.SalesmanLogin(ref err, postBean);
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
        public void WeixinSendMsgTest()
        {
            ErrorInfo err = new ErrorInfo();
            var loginKey = SalesmanLogin("18180770313", "123456", ref err);

            ApiRequesEntityBean postBean = new ApiRequesEntityBean();
            postBean.authToken = loginKey;
            postBean.userId = 162;
            postBean.para.Add(new ApiKeyValueBean { V = "翁志来测试", K = "Msg" });
            WebApiController webApi = new WebApiController();
            JsonResult res = (JsonResult)(webApi.WeixinSendMsg(postBean, null));

            ErrorInfo jsonObj = (ErrorInfo)res.Data;
            if (jsonObj.IsError)
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(true);
        }
    }
}