using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using ProInterface;
using System.Configuration;

namespace Web
{
    public static class Fun
    {

        public static Dictionary<string, string> QueryDt = new Dictionary<string, string>();
        /// <summary>
        /// 错误信息
        /// </summary>
        public static ErrorInfo Err = new ErrorInfo();
        /// <summary>
        /// 脚本运行状态
        /// </summary>
        public static Dictionary<Int64, object> ScriptRunStatus = new Dictionary<Int64, object>();

        public static IList<SelectListItem> GetSelectList<T>(T obj = default(T))
        {
            Type _enumType = obj.GetType();
            var allAttr = _enumType.GetCustomAttributes(true);

            string[] nameArr = Enum.GetNames(_enumType);
            var valuesArr = Enum.GetValues(_enumType);
            IList<SelectListItem> re = new List<SelectListItem>();
            for(int i=0;i<nameArr.Count();i++)
            {
                string name = nameArr[i];
                int value=(int)valuesArr.GetValue(i);
                FieldInfo fi = _enumType.GetField(name);
                DescriptionAttribute dna = null;
                dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                re.Add(new SelectListItem() { Text = dna.Description, Value = value.ToString() });
            }
            
            return re;
        }


        public static string UserKey
        {
            get
            {
                string reStr = Cookie.getCookie("USER_KEY");
                if (reStr == null || reStr == "")
                {
                    return null;
                }
                try
                {
                    return Cookie.getCookie("USER_KEY");
                }
                catch {
                    return null;
                }
            }
            set
            {
                Cookie.setCookie("USER_KEY", value, 20);
            }
        }
        public static byte[] GetFileByte(string path)
        {
            return File.ReadAllBytes(path);
        }

        /// <summary>
        /// 检测SQL语句危险数据
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static bool ProcessSqlStr(string Str, ref ProInterface.ErrorInfo err)
        {
            bool ReturnValue = true;
            if (string.IsNullOrEmpty(Str))
            {
                return true;
            }
            Str = Str.ToLower();
            try
            {
                if (Str != "")
                {
                    //                    string SqlStr = @" select * |and '|or '| insert into | delete from | alter table | update | create table | createview | drop view|cr
                    //eateindex|dropindex|createprocedure|dropprocedure|createtrigger|droptrigger|createschema|dro
                    //pschema|createdomain|alterdomain|dropdomain|);|select@|declare@|print@|char(| select ";
                    string SqlStr = " insert | create | update | alter | drop | print | merge ";
                    string[] anySqlStr = SqlStr.Split('|');
                    foreach (string ss in anySqlStr)
                    {
                        if (Str.IndexOf(ss) >= 0)
                        {
                            ReturnValue = false;
                            err.IsError = true;
                            err.Message = "包含：" + ss;
                            break;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                err.IsError = true;
                err.Message = e.Message;
                ReturnValue = false;
            }
            return ReturnValue;
        }

        public static void WriteLog(string msg)
        {
            //File.AppendAllLines(AppDomain.CurrentDomain.BaseDirectory + "/log.txt",new string[]{msg});
        }

        public static ProServer.GlobalUser GetNowUser()
        {
            ProServer.GlobalUser gu = ProServer.Global.GetUser(Fun.UserKey);
            return gu;
        }
    }
}