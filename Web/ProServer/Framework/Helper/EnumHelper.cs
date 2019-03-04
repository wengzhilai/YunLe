using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace ProServer
{
    /// <summary>
    /// 枚举助手
    /// </summary>
    public static class EnumHelper
    {


        ///<summary>
        /// 返回 Dic<枚举项，描述>
        ///</summary>
        ///<param name="enumType"></param>
        ///<returns>Dic<枚举项，描述></returns>
        public static Dictionary<string, string> GetEnumDic(Type enumType)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            FieldInfo[] fieldinfos = enumType.GetFields();
            foreach (FieldInfo field in fieldinfos)
            {
                if (field.FieldType.IsEnum)
                {
                    Object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    dic.Add(field.Name, ((DescriptionAttribute)objs[0]).Description);
                }

            }

            return dic;
        }

        ///<summary>
        /// 返回 IList<SelectListItem>
        ///</summary>
        ///<param name="enumType"></param>
        ///<returns></returns>
        public static IList<SelectListItem> GetEnumSelectListItem(Type enumType)
        {
            IList<SelectListItem> reEnt = new List<SelectListItem>();
            FieldInfo[] fieldinfos = enumType.GetFields();

            foreach (FieldInfo field in fieldinfos)
            {
                if (field.FieldType.IsEnum)
                {
                    Object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    var strValue = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
                    reEnt.Add(new SelectListItem() { Value = strValue, Text = ((DescriptionAttribute)objs[0]).Description });
                  
                }

            }

            return reEnt;
        }
        ///<summary>
        /// 从枚举类型和它的特性读出并返回一个键值对
        ///</summary>
        ///<param name="enumType">Type,该参数的格式为typeof(需要读的枚举类型)</param>
        ///<returns>键值对</returns>
        public static NameValueCollection GetNVCFromEnumValue(Type enumType)
        {
            NameValueCollection nvc = new NameValueCollection();
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            string strValue = string.Empty;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    strValue = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        DescriptionAttribute aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = field.Name;
                    }
                    nvc.Add(strValue, strText);
                }
            }
            return nvc;
        }
    }
}
