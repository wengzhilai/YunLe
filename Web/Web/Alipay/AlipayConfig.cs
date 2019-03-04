using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;

namespace Alipay
{
    /// <summary>
    /// 类名：Config
    /// 功能：基础配置类
    /// 详细：设置帐户有关信息及返回路径
    /// 版本：3.3
    /// 日期：2012-07-05
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    /// 
    /// 如何获取安全校验码和合作身份者ID
    /// 1.用您的签约支付宝账号登录支付宝网站(www.alipay.com)
    /// 2.点击“商家服务”(https://b.alipay.com/order/myOrder.htm)
    /// 3.点击“查询合作者身份(PID)”、“查询安全校验码(Key)”
    /// </summary>
    public class Config
    {
        #region 字段
        private static string partner = "";
        private static string private_key = "";
        private static string public_key = "";
        private static string input_charset = "";
        private static string sign_type = "";
        #endregion

        static Config()
        {
            //↓↓↓↓↓↓↓↓↓↓请在这里配置您的基本信息↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓

            //合作身份者ID，以2088开头由16位纯数字组成的字符串
            partner = "2088221519733898";

            //商户的私钥
            private_key = @"MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBAM85pWy6LuIq+UakByGfblDDRQtNBszu2ZZPTbdmUY2YDM5SIz6Www/IJhr8YbLxZM1fZIno+LCMyXZmiPbM83SmKBoDBaLK24DFK4Y+cyfyKchZsTaACWapZc9MGdO9teq9NzYJ2KT8c+FmcaboZF4DCzt+W/JP6OnapVdjs9BjAgMBAAECgYAPpGdTTL6LPfkxFuKe7Bz0pbjJgJf50jHEgdn49RVE3exhipu0dsbkoxQVR2XMjyIvynqZWmejVA1FDbpa/t+FlNj9M8lWzCJhE7bWBOhBjR4V2PEMuIVEDEo0KGD/A9YoJ7Oi5727eev2WGUKHZ0RSD2t9J0kjt06ipX6RJsXyQJBAOvtzVjpkWovkyyW0E6wPpf2Su8scLXH4o0fuUZwnYEOmaBh3ltxXdtPNPDA/ha6Yb0ACcGW0Ja7b7kp8Xd4LMcCQQDg2rej7G3T6AyJbE8S4mcU+vt5YDQxIfjGTnI1Nb2RaKZZi5HGfDmJmdGLgg8yD+I6Zvxr1Is8/gCvw33M6AuFAkEAoqb5kTjFtc1Cy9TVm8pJ/P4hvy6GLey2NdEPLI7rJ1RneXi/kY9gw5ehyfFju0uXonNqRnqxJ5nldmSCQnkpdQJAFveAJmgx16EuFqNPeFhTuxrfsMgYzKPxqx+8Hp88m6uawi61Vxa9McbaVmuUbcKdkuWOBt2Q00wGee133gQdmQJBAMEJVm3N9mLvVzHw2AOZU+F6j/ZJiR7sndi7Qr5xoTtzNlHYbL8h9epA0K1TUZW4K2b/pK7pKvDX5t/hrBoBsU0=";
            //支付宝的公钥，无需修改该值
            public_key = @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCnxj/9qwVfgoUh/y2W89L6BkRAFljhNhgPdyPuBV64bfQNN1PjbCzkIM6qRdKBoLPXmKKMiFYnkd6rAoprih3/PrQEB/VsW8OoM8fxn67UDYuyBTqA23MML9q1+ilIZwBC2AQ2UBVOrFXfFl75p6/B5KsiNG9zpgmLCUYuLkxpLQIDAQAB";

            //↑↑↑↑↑↑↑↑↑↑请在这里配置您的基本信息↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑



            //字符编码格式 目前支持 gbk 或 utf-8
            input_charset = "utf-8";

            //签名方式，选择项：RSA、DSA、MD5
            sign_type = "RSA";
        }

        #region 属性
        /// <summary>
        /// 获取或设置合作者身份ID
        /// </summary>
        public static string Partner
        {
            get { return partner; }
            set { partner = value; }
        }

        /// <summary>
        /// 获取或设置商户的私钥
        /// </summary>
        public static string Private_key
        {
            get { return private_key; }
            set { private_key = value; }
        }

        /// <summary>
        /// 获取或设置支付宝的公钥
        /// </summary>
        public static string Public_key
        {
            get { return public_key; }
            set { public_key = value; }
        }

        /// <summary>
        /// 获取字符编码格式
        /// </summary>
        public static string Input_charset
        {
            get { return input_charset; }
        }

        /// <summary>
        /// 获取签名方式
        /// </summary>
        public static string Sign_type
        {
            get { return sign_type; }
        }
        #endregion
    }
}