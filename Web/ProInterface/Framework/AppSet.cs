using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ProInterface
{
    /// <summary>
    /// 读取web.config的appSettings方法
    /// </summary>
    public class AppSet
    {
        #region 默认用户密码
        private static string _DefaultPwd;
        /// <summary>
        /// 默认用户密码
        /// </summary>
        public static string DefaultPwd
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DefaultPwd"]))
                {
                    AppSet._DefaultPwd = ConfigurationManager.AppSettings["DefaultPwd"];
                }
                return AppSet._DefaultPwd;
            }
        } 
        #endregion

        #region 允许重复登录
        private static bool _RepeatUser;
        /// <summary>
        /// 允许重复登录
        /// </summary>
        public static bool RepeatUser
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["RepeatUser"]))
                {
                    AppSet._RepeatUser = Convert.ToBoolean(ConfigurationManager.AppSettings["RepeatUser"]);
                }
                return AppSet._RepeatUser;
            }
        } 
        #endregion

        #region 允许写登录日志
        private static bool _WiteLoginLog;
        /// <summary>
        /// 允许写登录日志
        /// </summary>
        public static bool WiteLoginLog
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["WiteLoginLog"]))
                {
                    AppSet._WiteLoginLog = Convert.ToBoolean(ConfigurationManager.AppSettings["WiteLoginLog"]);
                }
                return AppSet._WiteLoginLog;
            }
        } 
        #endregion

        #region 关闭浏览器超时时间(分钟)
        private static int _TimeOut = 120;
        /// <summary>
        /// 关闭浏览器超时时间(分钟)
        /// </summary>
        public static int TimeOut
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["TimeOut"]))
                {
                    AppSet._TimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["TimeOut"]);
                }
                return AppSet._TimeOut;
            }
        } 
        #endregion

        #region 系统名称
        private static string _SysName;
        /// <summary>
        /// 系统名称
        /// </summary>
        public static string SysName
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SysName"]))
                {
                    AppSet._SysName = ConfigurationManager.AppSettings["SysName"];
                }
                return AppSet._SysName;
            }
        } 
        #endregion

        #region 技术支持
        private static string _Support;
        /// <summary>
        /// 技术支持
        /// </summary>
        public static string Support
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Support"]))
                {
                    AppSet._Support = ConfigurationManager.AppSettings["Support"];
                }
                return AppSet._Support;
            }
        } 
        #endregion

        #region 码表参数
        private static string _ConfigCode;
        /// <summary>
        /// 码表参数
        /// </summary>
        public static string ConfigCode
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ConfigCode"]))
                {
                    AppSet._ConfigCode = ConfigurationManager.AppSettings["ConfigCode"];
                }
                return AppSet._ConfigCode;
            }
        } 
        #endregion

        #region 码表参数
        private static int _CityId;
        /// <summary>
        /// 码表参数
        /// </summary>
        public static int CityId
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CityId"]))
                {
                    AppSet._CityId = Convert.ToInt32(ConfigurationManager.AppSettings["CityId"]);
                }
                return AppSet._CityId;
            }
        } 
        #endregion

        #region 可拜访的最大距离（米）
        private static double _VisitMaxDis;
        /// <summary>
        /// 可拜访的最大距离（米）
        /// </summary>
        public static double VisitMaxDis
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["VisitMaxDis"]))
                {
                    AppSet._VisitMaxDis = Convert.ToDouble(ConfigurationManager.AppSettings["VisitMaxDis"]);
                }
                return AppSet._VisitMaxDis;
            }
        } 
        #endregion

        #region 拜访时签到和离开的最大时间
        private static int _VisitMaxMin;
        /// <summary>
        /// 拜访时签到和离开的最大时间
        /// </summary>
        public static int VisitMaxMin
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["VisitMaxMin"]))
                {
                    AppSet._VisitMaxMin = Convert.ToInt32(ConfigurationManager.AppSettings["VisitMaxMin"]);
                }
                return AppSet._VisitMaxMin;
            }
        } 
        #endregion

        #region 片区名称
        private static string _RegionName;
        /// <summary>
        /// 片区名称
        /// </summary>
        public static string RegionName
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["RegionName"]))
                {
                    AppSet._RegionName = ConfigurationManager.AppSettings["RegionName"];
                }
                return AppSet._RegionName;
            }
        } 
        #endregion

        #region 乡镇名称
        private static string _ThorpeName;
        /// <summary>
        /// 乡镇名称
        /// </summary>
        public static string ThorpeName
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ThorpeName"]))
                {
                    AppSet._ThorpeName = ConfigurationManager.AppSettings["ThorpeName"];
                }
                return AppSet._ThorpeName;
            }
        } 
        #endregion

        #region 拜访重写坐标
        private static bool _VisitUpdatePlace;
        /// <summary>
        /// 拜访重写坐标
        /// </summary>
        public static bool VisitUpdatePlace
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["VisitUpdatePlace"]))
                {
                    AppSet._VisitUpdatePlace = Convert.ToBoolean(ConfigurationManager.AppSettings["VisitUpdatePlace"]);
                }
                return AppSet._VisitUpdatePlace;
            }
        } 
        #endregion

        #region 启用验证码
        private static bool _VerifyCode;
        /// <summary>
        /// 启用验证码
        /// </summary>
        public static bool VerifyCode
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["VerifyCode"]))
                {
                    AppSet._VerifyCode = Convert.ToBoolean(ConfigurationManager.AppSettings["VerifyCode"]);
                }
                return AppSet._VerifyCode;
            }
        } 
        #endregion

        #region 密码最小长度
        private static int _PwdMinLength;
        public static int PwdMinLength {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PwdMinLength"]))
                {
                    AppSet._PwdMinLength = Convert.ToInt32(ConfigurationManager.AppSettings["PwdMinLength"]);
                }
                return AppSet._PwdMinLength;
            }
        }
        #endregion
        #region 密码复杂度
        private static int _PwdComplexity;
        /// <summary>
        /// 密码复杂度
        /// </summary>
        public static int PwdComplexity
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PwdComplexity"]))
                {
                    AppSet._PwdComplexity = Convert.ToInt32(ConfigurationManager.AppSettings["PwdComplexity"]);
                }
                return AppSet._PwdComplexity;
            }
        } 
        #endregion

        #region Mas用户名
        private static string _MasUid;
        /// <summary>
        /// Mas用户名
        /// </summary>
        public static string MasUid
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["MasUid"]))
                {
                    AppSet._MasUid = ConfigurationManager.AppSettings["MasUid"];
                }
                return AppSet._MasUid;
            }
        } 
        #endregion

        #region Mas用户密码
        private static string _MasPwd;
        /// <summary>
        /// Mas用户密码
        /// </summary>
        public static string MasPwd
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["MasPwd"]))
                {
                    AppSet._MasPwd = ConfigurationManager.AppSettings["MasPwd"];
                }
                return AppSet._MasPwd;
            }
        } 
        #endregion

        #region Mas用户DB
        private static string _MasDb;
        /// <summary>
        /// Mas用户DB
        /// </summary>
        public static string MasDb
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["MasDb"]))
                {
                    AppSet._MasDb = ConfigurationManager.AppSettings["MasDb"];
                }
                return AppSet._MasDb;
            }
        } 
        #endregion

        #region 网站备案号
        private static string _Miitbeian;
        /// <summary>
        /// 网站备案号
        /// </summary>
        public static string Miitbeian
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Miitbeian"]))
                {
                    AppSet._Miitbeian = ConfigurationManager.AppSettings["Miitbeian"];
                }
                return AppSet._Miitbeian;
            }
        } 
        #endregion

        #region 脚本服务器:可同时运行数量
        private static int _ScriptRunMaxNum;
        /// <summary>
        /// 脚本服务器:可同时运行数量
        /// </summary>
        public static int ScriptRunMaxNum
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ScriptRunMaxNum"]))
                {
                    AppSet._ScriptRunMaxNum = Convert.ToInt32(ConfigurationManager.AppSettings["ScriptRunMaxNum"]);
                }
                return AppSet._ScriptRunMaxNum;
            }
        }
        #endregion

        #region 启用是APP
        private static bool _NoApp;
        /// <summary>
        /// 启用验证码
        /// </summary>
        public static bool NoApp
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["NoApp"]))
                {
                    AppSet._NoApp = Convert.ToBoolean(ConfigurationManager.AppSettings["NoApp"]);
                }
                return AppSet._NoApp;
            }
        }
        #endregion


        #region 短信验证
        private static bool _VerifyCodeSms;
        public static bool VerifyCodeSms
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["VerifyCodeSms"]))
                {
                    AppSet._VerifyCodeSms = Convert.ToBoolean(ConfigurationManager.AppSettings["VerifyCodeSms"]);
                }
                return AppSet._VerifyCodeSms;
            }
        }
        #endregion


        #region 微信AppId
        private static string _AppId;
        /// <summary>
        /// 网站备案号
        /// </summary>
        public static string AppId
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["WeixinAppId"]))
                {
                    AppSet._AppId = ConfigurationManager.AppSettings["WeixinAppId"];
                }
                return AppSet._AppId;
            }
        }
        #endregion

        #region 微信AppSecret
        private static string _AppSecret;
        /// <summary>
        /// 网站备案号
        /// </summary>
        public static string AppSecret
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["WeixinAppSecret"]))
                {
                    AppSet._AppSecret = ConfigurationManager.AppSettings["WeixinAppSecret"];
                }
                return AppSet._AppSecret;
            }
        }
        #endregion
    }
}
