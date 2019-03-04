using System;
using System.Collections.Generic;
using System.Web;
using System.Collections;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.Data;

namespace ProInterface
{
    /// <summary>
    /// json操作
    /// </summary>
    public static class JSON
    {
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static T EncodeToEntity<T>(string jsonStr)
        {
            if (jsonStr == null) return default(T);
            JavaScriptSerializer jss = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
            var ent = jss.Deserialize<T>(jsonStr);
            return ent;
        }
        /// <summary>
        /// 对象转换成字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string DecodeToStr<T>(T entity)
        {
            if (entity == null) return null;
            try
            {
                if (entity == null) return null;
                if ((entity.GetType() == typeof(String) || entity.GetType() == typeof(string)))
                {
                    return entity.ToString();
                }
                string DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
                IsoDateTimeConverter dt = new IsoDateTimeConverter();
                dt.DateTimeFormat = DateTimeFormat;
                var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                jSetting.Converters.Add(dt);
                return JsonConvert.SerializeObject(entity, jSetting);
            }
            catch
            {
                JavaScriptSerializer jss = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
                jss.MaxJsonLength = int.MaxValue;
                var ent = jss.Serialize(entity);
                return ent;
            }
        }


        public static string DecodeToStr(int allNum, object o)
        {
            string json = DecodeToStr(o);
            if (json == null || json == "") json = "[]";
            return "{\"total\":" + allNum + ",\"rows\":" + json + "}";
        }

        public static DataTable ToDataTable(this string json)
        {
            DataTable dataTable = new DataTable();  //实例化
            DataTable result;
            try
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
                ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);
                if (arrayList.Count > 0)
                {
                    foreach (Dictionary<string, object> dictionary in arrayList)
                    {
                        if (dictionary.Keys.Count == 0)
                        {
                            result = dataTable;
                            return result;
                        }
                        if (dataTable.Columns.Count == 0)
                        {
                            foreach (string current in dictionary.Keys)
                            {
                                dataTable.Columns.Add(current, dictionary[current].GetType());
                            }
                        }
                        DataRow dataRow = dataTable.NewRow();
                        foreach (string current in dictionary.Keys)
                        {
                            dataRow[current] = dictionary[current];
                        }
                        dataTable.Rows.Add(dataRow); //循环添加行到DataTable中
                    }
                }
            }
            catch
            {
            }
            result = dataTable;
            return result;

        }

    }
}