using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProServer
{
    public class CFG
    {
        private static string _ConfigWebUrl = null;
        /// <summary>
        /// 网页地址
        /// </summary>
        public static string ConfigWebUrl
        {
            get
            {
                if (_ConfigWebUrl == null)
                {
                    _ConfigWebUrl = ConfigGetValue("WebUrl");
                }
                return _ConfigWebUrl;
            }
            set
            {
                ConfigSetValue("WebUrl", value, "网页地址");
            }
        }

        /// <summary>
        /// 网页虚拟目录
        /// </summary>
        public static string ConfigAbsolute
        {
            get
            {
                if (_ConfigWebUrl == null)
                {
                    _ConfigWebUrl = ConfigGetValue("Absolute");
                }
                return _ConfigWebUrl;
            }
            set
            {
                ConfigSetValue("Absolute", value, "网页虚拟目录");
            }
        }

        private static string _ConfigRolePara = null;
        public static string ConfigRolePara
        {
            get
            {
                if (_ConfigRolePara == null)
                {
                    _ConfigRolePara = ConfigGetValue("RolePara");
                }
                return _ConfigRolePara;
            }
        }


        private static bool? _ConfigMultiUser;
        public static bool ConfigMultiUser
        {
            get
            {
                if (_ConfigMultiUser == null)
                {
                    var val = ConfigGetValue("MultiUser");
                    if (val != null)
                    {
                        if (val == "1")
                        {
                            _ConfigMultiUser = true;
                        }
                    }
                    _ConfigMultiUser = false;
                }
                return _ConfigMultiUser.Value;
            }
        }

        public static string ConfigGetValue(string code)
        {
            using (DBEntities db = new DBEntities())
            {
                var tmp = db.YL_CONFIG.SingleOrDefault(x => x.CODE == code);
                if (tmp != null)
                {
                    return tmp.VALUE;
                }
                return null;
            }
        }
        public static bool ConfigSetValue(string code,string value,string msg)
        {
            using (DBEntities db = new DBEntities())
            {
                var tmp = db.YL_CONFIG.SingleOrDefault(x => x.CODE == code);
                if (tmp != null)
                {
                    tmp.VALUE = value;
                }
                else
                {
                    db.YL_CONFIG.Add(new YL_CONFIG() { VALUE = value, CODE = code, NAME = msg, REGION="0" });
                }
                db.SaveChanges();
                return true;
            }
        }

    }
}
