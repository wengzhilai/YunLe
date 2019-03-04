using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data;
using System.Text.RegularExpressions;
using System.Data.Entity.Validation;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Configuration;
using ProInterface.Models;
using System.Runtime.Caching;
using System.Drawing;

namespace ProServer
{
    public class Fun
    {
        public static int GetSeqID<T>() where T : new()
        {
            using (DBEntities db = new DBEntities())
            {
                T tmp = new T();
                string sql = "SELECT   " + tmp.GetType().Name + "_SEQ.NEXTVAL   FROM   DUAL";
                string dbType = GetDataBaseType();
                switch (dbType)
                {
                    case "DB2":
                        sql = "SELECT   " + tmp.GetType().Name + "_SEQ.NEXTVAL   FROM   SYSIBM.DUAL";
                        break;
                    case "Oracle":
                        sql = "SELECT   " + tmp.GetType().Name + "_SEQ.NEXTVAL   FROM   DUAL";
                        break;
                    case "Sql":
                        return 0;
                }


                var conn = db.Database.Connection;
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                int reInt = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();
                return reInt;
            }
        }

        public static bool CheckPassword(ref ProInterface.ErrorInfo err,string pwdStr)
        {
            Regex r1 = new Regex(@"^(?=.*[a-z])");
            Regex r2 = new Regex(@"^(?=.*[A-Z])");
            Regex r3 = new Regex(@"^(?=.*[0-9])");
            Regex r4 = new Regex(@"^(?=([\x21-\x7e]+)[^a-zA-Z0-9])");
            if (pwdStr.Length < ProInterface.AppSet.PwdMinLength)
            {
                err.IsError = true;
                err.Message =string.Format("密码长度不够{0}位",ProInterface.AppSet.PwdMinLength);
                return false;
            }
            int reInt = 0;
            if (r1.IsMatch(pwdStr)) reInt++;
            if (r2.IsMatch(pwdStr)) reInt++;
            if (r3.IsMatch(pwdStr)) reInt++;
            if (r4.IsMatch(pwdStr)) reInt++;

            if (reInt<ProInterface.AppSet.PwdComplexity)
            {
                err.IsError = true;
                err.Message = string.Format("密码必须包括字母大写、字母小写、数字和特殊字符中的其中{0}种", ProInterface.AppSet.PwdComplexity);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取当前数据库类型
        /// </summary>
        /// <returns></returns>
        public static string GetDataBaseType()
        {
            using (DBEntities db = new DBEntities())
            {
                string dbType = db.Database.Connection.GetType().Name;
                dbType = dbType.Substring(0, dbType.IndexOf("C"));
                return dbType;
            }
        }

        /// <summary>
        /// 在实体验证失败时
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static string GetDbEntityErrMess(DbEntityValidationException error)
        {
            StringBuilder reStr = new StringBuilder();
            foreach (var t in error.EntityValidationErrors)
            {
                foreach (var t0 in t.ValidationErrors)
                {
                    reStr.Append(t0.ErrorMessage + ",");
                }
            }
            return reStr.ToString();
        }

        public static string GetExceptionMessage(Exception e)
        {
            IList<string> message = new List<string>();
            message.Add(e.Message);
            while (e.InnerException != null)
            {
                e = e.InnerException;
                message.Add(e.Message);
            }
            return message[message.Count - 1];
        }


        /// <summary>
        /// 将DataTable数据转换成实体类
        /// 本功能主要用于外导EXCEL
        /// </summary>
        /// <typeparam name="T">MVC的实体类</typeparam>
        /// <param name="dt">输入的DataTable</param>
        /// <returns>实体类的LIST</returns>
        public static IList<T> TableToClass<T>(DataTable dt) where T : new()
        {
            IList<T> outList = new List<T>();
            T tmpClass = new T();
            if (dt.Rows.Count == 0) return outList;
            PropertyInfo[] proInfoArr = tmpClass.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);//得到该类的所有公共属性
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Dictionary<string, string> dic_all = new Dictionary<string, string>();
            foreach (var t in proInfoArr)
            {
                var attrsPro = t.GetCustomAttributes(typeof(DisplayAttribute), true);
                if (attrsPro.Length > 0)
                {
                    DisplayAttribute pro = (DisplayAttribute)attrsPro[0];
                    dic_all.Add(pro.Name, t.Name);
                }
            }
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dic_all.Where(x => x.Key == dt.Columns[i].Caption).Count() != 0)
                {
                    dic.Add(dt.Columns[i].Caption, dic_all[dt.Columns[i].Caption]);
                }
                else if (dic_all.Where(x => x.Value == dt.Columns[i].Caption).Count() != 0)
                {
                    dic.Add(dt.Columns[i].Caption, dt.Columns[i].Caption);
                }
            }

            var rowTmp = dt.Rows[0];

            for (int a = 0; a < dt.Rows.Count; a++)
            {
                var row = dt.Rows[a];
                tmpClass = new T();
                proInfoArr = tmpClass.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);//得到该类的所有公共属性
                foreach (var t in dic)
                {
                    PropertyInfo outproInfo = tmpClass.GetType().GetProperty(t.Value);
                    if (outproInfo != null)
                    {
                        if (row[t.Key] == null || string.IsNullOrEmpty(row[t.Key].ToString()))
                        {
                            row[t.Key] = rowTmp[t.Key];
                        }
                        outproInfo.SetValue(tmpClass, Convert.ChangeType(row[t.Key], outproInfo.PropertyType, CultureInfo.CurrentCulture), null);
                    }
                }
                outList.Add(tmpClass);
                rowTmp = dt.Rows[a];
            }
            return outList;
        }

        /// <summary>
        /// 复制一个类里所有属性到别一个类
        /// </summary>
        /// <typeparam name="inT">传入的类型</typeparam>
        /// <typeparam name="outT">输出类型</typeparam>
        /// <param name="inClass">传入的类</param>
        /// <param name="outClass">输入的类</param>
        /// <returns>复制结果的类</returns>
        public static outT ClassToCopy<inT, outT>(inT inClass, outT outClass, IList<string> allPar = null)
        {
            if (inClass == null) return outClass;
            PropertyInfo[] proInfoArr = inClass.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);//得到该类的所有公共属性
            for (int a = 0; a < proInfoArr.Length; a++)
            {
                if (allPar != null && !allPar.Contains(proInfoArr[a].Name)) continue;
                PropertyInfo outproInfo = outClass.GetType().GetProperty(proInfoArr[a].Name);
                if (outproInfo != null)
                {
                    var type = outproInfo.PropertyType;
                    object objValue = proInfoArr[a].GetValue(inClass, null);
                    if (null != objValue)
                    {
                        if (!outproInfo.PropertyType.IsGenericType)
                        {
                            objValue = Convert.ChangeType(objValue, outproInfo.PropertyType);
                        }
                        else
                        {
                            Type genericTypeDefinition = outproInfo.PropertyType.GetGenericTypeDefinition();
                            if (genericTypeDefinition == typeof(Nullable<>))
                            {
                                objValue = Convert.ChangeType(objValue, Nullable.GetUnderlyingType(outproInfo.PropertyType));
                            }
                        }
                    }
                    outproInfo.SetValue(outClass, objValue, null);
                }
            }
            return outClass;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="inT"></typeparam>
        /// <typeparam name="outT"></typeparam>
        /// <param name="inClass"></param>
        /// <returns></returns>
        public static outT ClassToCopy<inT, outT>(inT inClass) where outT : new()
        {
            if (inClass == null) return default(outT);
            outT outClass = new outT();
            return ClassToCopy(inClass, outClass);
        }
        /// <summary>
        /// 转换IList内的所有属性
        /// </summary>
        /// <typeparam name="inT"></typeparam>
        /// <typeparam name="outT"></typeparam>
        /// <param name="inClass"></param>
        /// <returns></returns>
        public static IList<outT> ClassListToCopy<inT, outT>(IList<inT> inClass) where outT : new()
        {
            if (inClass == null) return default(IList<outT>);
            IList<outT> outClass = new List<outT>();
            for (int a = 0; a < inClass.Count; a++)
            {
                outClass.Add(ClassToCopy<inT, outT>(inClass[a]));
            }
            return outClass;
        }

        /// <summary>
        /// 把枚举转换成下接列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IList<SelectListItem> GetSelectList<T>(T obj = default(T))
        {
            Type _enumType = obj.GetType();
            var allAttr = _enumType.GetCustomAttributes(true);

            string[] nameArr = Enum.GetNames(_enumType);
            var valuesArr = Enum.GetValues(_enumType);
            IList<SelectListItem> re = new List<SelectListItem>();
            for (int i = 0; i < nameArr.Count(); i++)
            {
                string name = nameArr[i];
                int value = (int)valuesArr.GetValue(i);
                FieldInfo fi = _enumType.GetField(name);
                DescriptionAttribute dna = null;
                dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                re.Add(new SelectListItem() { Text = dna.Description, Value = value.ToString() });
            }

            return re;
        }
        /// <summary>
        /// 计算MD5
        /// </summary>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        public static string FilesMakeMd5(byte[] fileContent)
        {
            if (fileContent == null) return null;
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fileContent);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
                sb.Append(retVal[i].ToString("x2"));
            return sb.ToString();
        }

        /// <summary>
        /// 将Url路径转换成文件在服务器上的全路径
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlToAllPath(string url)
        {
            if (url == null) return null;
            url = url.Replace("\"", "");
            string urlPath = CFG.ConfigWebUrl;
            string WebRootPath = System.AppDomain.CurrentDomain.BaseDirectory;
            if (url.IndexOf("http") > -1)
            {
                url = url.Replace(urlPath, "");
            }
            url = WebRootPath + url.Replace("../", "/").Replace("~/", "/").Replace("/", "\\");
            url = url.Replace("\\\\", "\\");
            return url;
        }
        /// <summary>
        /// 虚拟路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string AllWebPath(string path)
        {
            string WebUrl = CFG.ConfigWebUrl;
            string WebRootPath = System.AppDomain.CurrentDomain.BaseDirectory;
            if (path.IndexOf(WebUrl) == 0)
            {
                path = path.Replace(WebUrl, "/");
            }
            if (path.IndexOf(WebRootPath) == 0)
            {
                path = path.Replace(WebRootPath, "/");
                path = path.Replace("\\", "/");
            }
            return path;
        }
        /// <summary>
        /// 将 URL 转换为在请求客户端可用的 URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ResolveUrl(string url)
        {
            if (url.IndexOf("http") == -1)
            {
                string urlPath = CFG.ConfigWebUrl;
                url = urlPath + url.Replace("../", "/").Replace("~/", "/");
            }
            return url;
        }

        /// <summary>
        /// 相对路径的虚拟目录
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string AbsoluteUrl(string url)
        {
            if (url.IndexOf("http") == -1)
            {
                string urlPath = CFG.ConfigAbsolute;
                url = urlPath + url.Replace("../", "").Replace("~/", "");
            }
            return url;
        }

        public static byte[] GetPicByteByUrl(string url)
        {
            try
            {
                url = ResolveUrl(url);

                string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\UpFile\\Tmp\\" + DateTime.Now.Ticks + url.Substring(url.LastIndexOf("."));
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                Stream reader = response.GetResponseStream();
                FileStream writer = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                byte[] buff = new byte[512];
                int c = 0; //实际读取的字节数
                while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                {
                    writer.Write(buff, 0, c);
                }
                writer.Close();
                writer.Dispose();
                reader.Close();
                reader.Dispose();
                response.Close();
                return File.ReadAllBytes(path);
            }
            catch
            {
                return null;
            }
        }


        public static string GetUpMoth(string yyyyMM, int numMoth)
        {
            DateTime dt = new DateTime(Convert.ToInt32(yyyyMM.Substring(0, 4)), Convert.ToInt32(yyyyMM.Substring(4, 2)), 1);
            return dt.AddMonths(numMoth).ToString("yyyyMM");
        }


        public static DataTable JsonToDataTable(string strJson)
        {
            //取出表名  
            Regex rg = new Regex(@"(?<={)[^:]+(?=:\[)", RegexOptions.IgnoreCase);
            string strName = rg.Match(strJson).Value;
            DataTable tb = null;
            //去除表名  
            strJson = strJson.Substring(strJson.IndexOf("[") + 1);
            strJson = strJson.Substring(0, strJson.IndexOf("]"));

            //获取数据  
            rg = new Regex(@"(?<={)[^}]+(?=})");
            MatchCollection mc = rg.Matches(strJson);
            for (int i = 0; i < mc.Count; i++)
            {
                string strRow = mc[i].Value;
                string[] strRows = strRow.Split(',');

                //创建表  
                if (tb == null)
                {
                    tb = new DataTable();
                    tb.TableName = strName;
                    foreach (string str in strRows)
                    {
                        DataColumn dc = new DataColumn();
                        string[] strCell = str.Split(':');
                        dc.ColumnName = strCell[0].ToString().Replace("\"", "");
                        tb.Columns.Add(dc);
                    }
                    tb.AcceptChanges();
                }

                //增加内容  
                DataRow dr = tb.NewRow();
                for (int r = 0; r < strRows.Length; r++)
                {
                    dr[r] = strRows[r].Split(':')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("\"", "");
                }
                tb.Rows.Add(dr);
                tb.AcceptChanges();
            }

            return tb;
        }
        public static string EvalExpression(string formula)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Result").Expression = formula;
            dt.Rows.Add(dt.NewRow());

            var result = dt.Rows[0]["Result"];
            return result.ToString();
        }
        /// <summary>
        /// 获取类的备注信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetClassDescription<T>()
        {

            object[] peroperties = typeof(T).GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (peroperties.Length > 0)
            {
                return ((DescriptionAttribute)peroperties[0]).Description;
            }
            return "";
        }
        /// <summary>
        /// 获取类的属性说明
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetClassProperDescription<T>(string properName)
        {
            PropertyInfo[] peroperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in peroperties)
            {
                if (property.Name == properName)
                {
                    object[] objs = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (objs.Length > 0)
                    {
                        return ((DescriptionAttribute)objs[0]).Description;
                    }
                }
            }
            return "";
        }

        public static string GetClassProperType<T>(string properName)
        {
            PropertyInfo[] peroperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in peroperties)
            {
                if (property.Name == properName)
                {
                    var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    return propertyType.Name;
                }
            }
            return "";
        }

        public static IEnumerable<T> GetListWhere<T>(IEnumerable<T> inList, string orderFiled, string orderType, string orderValue, ref ProInterface.ErrorInfo error)
        {
            if (string.IsNullOrEmpty(orderValue)) return inList;
            string whereLambda = "";
            orderValue = orderValue.Trim();
            if (orderValue.ToLower() == "null")
            {
                whereLambda = string.Format("x=>x.{0} {1} null", orderFiled, orderType);
            }
            else
            {
                if (orderValue.Substring(0, 1) == "\"")
                {
                    orderValue = orderValue.Substring(1);
                }
                if (orderValue.Substring(orderValue.Length - 1) == "\"")
                {
                    orderValue = orderValue.Substring(0, orderValue.Length - 1);
                }
                if (!orderValue.IsDouble())
                {
                    orderValue = "\"" + orderValue + "\"";
                }

                whereLambda = string.Format("x => x.{0} {1} {2}", orderFiled, orderType, orderValue);
                if (orderType != null && orderType.ToLower() == "like")
                {
                    whereLambda = string.Format("x=>x.{0} != null && x.{0}.IndexOf({1})>-1", orderFiled, orderValue);
                }
            }

            if (!string.IsNullOrEmpty(whereLambda))
            {
                try
                {
                    Func<T, bool> whereFunc = StringToLambda.LambdaParser.Compile<Func<T, bool>>(whereLambda);
                    inList = inList.Where(whereFunc);
                }
                catch
                {
                    error.IsError = true;
                    error.Message = "条件表态式[" + whereLambda + "]有误";
                }
            }
            return inList;
        }


        public static IEnumerable<T> GetListOrder<T>(IEnumerable<T> inList, string orderFiled, string orderType, ref ProInterface.ErrorInfo error)
        {
            if (!string.IsNullOrEmpty(orderFiled))
            {
                try
                {
                    var dateType = Fun.GetClassProperType<T>(orderFiled).ToLower();
                    switch (dateType)
                    {
                        case "decimal":
                            Func<T, decimal> whereFuncDecimal = StringToLambda.LambdaParser.Compile<Func<T, decimal>>("x=>x." + orderFiled);
                            if (orderType == "asc")
                            {
                                inList = inList.OrderBy(whereFuncDecimal);
                            }
                            else
                            {
                                inList = inList.OrderByDescending(whereFuncDecimal);
                            }
                            break;
                        case "int":
                            Func<T, int> whereFuncInt = StringToLambda.LambdaParser.Compile<Func<T, int>>("x=>x." + orderFiled);
                            if (orderType == "asc")
                            {
                                inList = inList.OrderBy(whereFuncInt);
                            }
                            else
                            {
                                inList = inList.OrderByDescending(whereFuncInt);
                            }
                            break;
                        case "int32":
                            Func<T, Int32> whereFuncInt32 = StringToLambda.LambdaParser.Compile<Func<T, Int32>>("x=>x." + orderFiled);
                            if (orderType == "asc")
                            {
                                inList = inList.OrderBy(whereFuncInt32);
                            }
                            else
                            {
                                inList = inList.OrderByDescending(whereFuncInt32);
                            }
                            break;
                        case "datetime":
                            Func<T, DateTime> whereFuncDatetime = StringToLambda.LambdaParser.Compile<Func<T, DateTime>>("x=>x." + orderFiled);
                            if (orderType == "asc")
                            {
                                inList = inList.OrderBy(whereFuncDatetime);
                            }
                            else
                            {
                                inList = inList.OrderByDescending(whereFuncDatetime);
                            }
                            break;
                       
                        default:
                            Func<T, string> whereFunc = StringToLambda.LambdaParser.Compile<Func<T, string>>("x=>x." + orderFiled);
                            if (orderType == "asc")
                            {
                                inList = inList.OrderBy(whereFunc);
                            }
                            else
                            {
                                inList = inList.OrderByDescending(whereFunc);
                            }
                            break;
                    }
                    return inList;

                }
                catch
                {
                    error.IsError = true;
                    error.Message = "条件表态式[" + orderFiled + "]有误";
                }
            }
            return inList;
        }

        public static DataTable ClassToDataTable(IList<ProInterface.Models.KTV> inList)
        {
            DataTable reDt = new DataTable();
            reDt.Columns.Add("渠道编号");
            reDt.Columns.Add("渠道名称");
            //列
            foreach (var t in inList.GroupBy(x => x.T).ToList())
            {
                reDt.Columns.Add(t.Key);
            }
            //行
            foreach (var t in inList.GroupBy(x => x.K).ToList())
            {
                DataRow dr = reDt.NewRow();
                if (t.Count() > 0)
                {
                    dr["渠道名称"] = t.ToList()[0].TClass.V;
                }
                dr["渠道编号"] = t.Key;
                foreach (var item in t.ToList())
                {
                    dr[item.T] = item.V;
                }
                reDt.Rows.Add(dr);
            }
            return reDt;
        }

        public static IList<ProInterface.Models.KTV> DataTableToClass(DataTable inData)
        {
            IList<ProInterface.Models.KTV> reList = new List<ProInterface.Models.KTV>();
            IList<string> allCol = new List<string>();
            for (int i = 0; i < inData.Columns.Count; i++)
            {
                allCol.Add(inData.Columns[i].Caption);
            }
            foreach (DataRow t in inData.Rows)
            {
                string name = t[0].ToString();
                for (int i = 2; i < allCol.Count; i++)
                {
                    reList.Add(new ProInterface.Models.KTV { K = name, T = allCol[i], V = t[i].ToString() });
                }
            }
            return reList;
        }

        /// <summary>
        /// 产生一组不重复的随机数
        /// </summary>
        public static IList<int> RandomIntList(int MinValue, int MaxValue, int Length)
        {
            if (MaxValue - MinValue + 1 < Length)
            {
                return null;
            }
            Random R = new Random();
            Int32 SuiJi = 0;
            IList<int> suijisuzu = new List<int>();
            int min = MinValue - 1;
            int max = MaxValue + 1;
            for (int i = 0; i < Length; i++)
            {
                suijisuzu.Add(min);
            }
            for (int i = 0; i < Length; i++)
            {
                while (true)
                {
                    SuiJi = R.Next(min, max);
                    if (!suijisuzu.Contains(SuiJi))
                    {
                        suijisuzu[i] = SuiJi;
                        break;
                    }
                }
            }
            return suijisuzu;
        }

        #region 通过两个点的经纬度计算距离

        private const double EARTH_RADIUS = 6378.137; //地球半径
        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }
        /// <summary>
        /// 通过两个点的经纬度计算距离(米)
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lng1"></param>
        /// <param name="lat2"></param>
        /// <param name="lng2"></param>
        /// <returns></returns>
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10;
            return s;
        }
        /// <summary>
        /// 通过两个点的经纬度计算距离(米)
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lng1"></param>
        /// <param name="lat2"></param>
        /// <param name="lng2"></param>
        /// <returns></returns>
        public static double GetDistance(string lat1, string lng1, string lat2, string lng2)
        {
            return Fun.GetDistance(Convert.ToDouble(lat1), Convert.ToDouble(lng1), Convert.ToDouble(lat2), Convert.ToDouble(lng2));
        }

        #endregion


        /// <summary>
        /// 替换@{day(0)}、@{month(0)}、@{years(0)},@{last_day()}
        /// </summary>
        /// <param name="inStr"></param>
        /// <param name="nowDt"></param>
        /// <returns></returns>
        public static string ReplaceDataTime(string inStr, DateTime nowDt, string loginKey=null)
        {
            if (!string.IsNullOrEmpty(loginKey))
            {
                GlobalUser gu = Global.GetUser(loginKey);
                if (gu != null)
                {
                    inStr = inStr.Replace("@{DISTRICT_ID}", gu.DistrictId.ToString());
                    inStr = inStr.Replace("@{DISTRICT_CODE}", gu.DistrictCode.ToString());
                    inStr = inStr.Replace("@{USER_ID}", gu.UserId.ToString());
                    inStr = inStr.Replace("@{REGION}", gu.Region);
                    inStr = inStr.Replace("@{ALL_ROLE}", gu.GetRoleAllStr());
                    inStr = inStr.Replace("@{ALL_REGION}", gu.GetRegionLeveStr());
                    inStr = inStr.Replace("@{NOW_LEVEL_ID}", gu.LevelId.ToString());
                }
            }

            inStr = inStr.Replace("@{day}", "@{day(0)}");
            inStr = inStr.Replace("@{month}", "@{month(0)}");
            inStr = inStr.Replace("@{years}", "@{year(0)}");
            inStr = inStr.Replace("@{years", "@{year");
            inStr = inStr.Replace("@{last_day}", "@{last_day(0)}");

            var sql = inStr;
            int nowPlace = 0;
            {
                int s = sql.IndexOf("@{day(");
                nowPlace = s;
                if (s > -1)
                {
                    int e = sql.IndexOf(")}", s);
                    while (e > s && s > -1)
                    {
                        s = s + 6;
                        int per = 0;
                        if (e > s)
                        {
                            per = Convert.ToInt32(sql.Substring(s, e - s));
                        }
                        sql = sql.Replace("@{day(" + per + ")}", nowDt.AddDays(per).ToString("yyyyMMdd"));
                        if (per == 0)
                        {
                            sql = sql.Replace("@{day()}", nowDt.AddDays(per).ToString("yyyyMMdd"));
                        }

                        s = sql.IndexOf("@{day(");
                        if (nowPlace == s)
                        {
                            return "";
                        }
                        nowPlace = s;
                        if (s > -1)
                        {
                            e = sql.IndexOf(")}", s);
                        }
                    }
                }
            }

            {
                int s = sql.IndexOf("@{month(");
                nowPlace = s;
                if (s > -1)
                {
                    int e = sql.IndexOf(")}", s);
                    while (e > s && s > -1)
                    {
                        s = s + 8;
                        int per = 0;
                        if (e > s)
                        {
                            per = Convert.ToInt32(sql.Substring(s, e - s));
                        }
                        sql = sql.Replace("@{month(" + per + ")}", nowDt.AddMonths(per).ToString("yyyyMM"));
                        if (per == 0)
                        {
                            sql = sql.Replace("@{month()}", nowDt.AddMonths(per).ToString("yyyyMM"));
                        }

                        s = sql.IndexOf("@{month(");
                        if (nowPlace == s)
                        {
                            return "";
                        }
                        nowPlace = s;
                        if (s > -1)
                        {
                            e = sql.IndexOf(")}", s);
                        }
                    }
                }
            }
            {
                int s = sql.IndexOf("@{year(");
                nowPlace = s;
                if (s > -1)
                {
                    int e = sql.IndexOf(")}", s);
                    while (e > s && s > -1)
                    {
                        s = s + 7;
                        int per = 0;
                        if (e > s)
                        {
                            per = Convert.ToInt32(sql.Substring(s, e - s));
                        }
                        sql = sql.Replace("@{year(" + per + ")}", nowDt.AddYears(per).ToString("yyyy"));
                        if (per == 0)
                        {
                            sql = sql.Replace("@{year()}", nowDt.AddYears(per).ToString("yyyy"));
                        }

                        s = sql.IndexOf("@{year(");
                        if (nowPlace == s)
                        {
                            return "";
                        }
                        nowPlace = s;
                        if (s > -1)
                        {
                            e = sql.IndexOf(")}", s);
                        }
                    }
                }
            }

            {
                int s = sql.IndexOf("@{last_day(");
                nowPlace = s;
                if (s > -1)
                {
                    int e = sql.IndexOf(")}", s);
                    while (e > s && s > -1)
                    {
                        s = s + 11;
                        int per = 0;
                        if (e > s)
                        {
                            per = Convert.ToInt32(sql.Substring(s, e - s));
                        }

                        DateTime temp = new DateTime(nowDt.Year, nowDt.Month, 1);
                        var tmpV = temp.AddMonths(per + 1).AddDays(-1).ToString("yyyyMMdd");

                        sql = sql.Replace("@{last_day(" + per + ")}", tmpV);
                        if (per == 0)
                        {
                            sql = sql.Replace("@{last_day()}", tmpV);
                        }

                        s = sql.IndexOf("@{last_day(");
                        if (nowPlace == s)
                        {
                            return "";
                        }
                        nowPlace = s;
                        if (s > -1)
                        {
                            e = sql.IndexOf(")}", s);
                        }
                    }
                }
            }

            return sql;
        }


        public static string GetSelectScript(string p)
        {
            // @[00-9]  -5   @[00-23,24-40]
            StringBuilder str = new StringBuilder();
            int begin = p.IndexOf("@");//0
            int end = p.IndexOf("]");//6

            //取得参数占位符中的参数
            string temp = p.Substring(begin + 2, end - begin - 2);

            string[] mc = temp.Split(',');

            foreach (var a in mc)
            {
                string[] c = a.Split('-');

                int i1 = 0, i2 = 0, size = 0;
                try
                {
                    i1 = Int32.Parse(c[0]);
                    i2 = Int32.Parse(c[1]);
                }
                catch (Exception e)
                {
                    //如果不是数字，返回空字符串
                    return str.ToString();
                }
                //是否不超过第二个数
                for (int i = i1; i <= i2; i++)
                {
                    size = c[0].Length;
                    size = size - i.ToString().Length;
                    if (size > 0)
                    {

                        //补齐占位符
                        switch (size)
                        {
                            case 1:
                                str.Append(p.Replace(p.Substring(begin, end - begin + 1), "0" + i.ToString()));
                                break;
                            case 2:
                                str.Append(p.Replace(p.Substring(begin, end - begin + 1), "00" + i.ToString()));
                                break;
                            case 3:
                                str.Append(p.Replace(p.Substring(begin, end - begin + 1), "000" + i.ToString()));
                                break;
                            default:
                                str.Append(p.Replace(p.Substring(begin, end - begin + 1), "0000" + i.ToString()));
                                break;

                        }

                    }
                    else
                    {
                        str.Append(p.Replace(p.Substring(begin, end - begin + 1), i.ToString()));
                    }
                    str.Append(";");
                }
            }
            return str.ToString();
        }

        /// <summary>
        /// 去除HTML标记
        /// </summary>
        /// <param name="NoHTML">包括HTML的源码 </param>
        /// <returns>已经去除后的文字</returns>
        public static string NoHTML(string Htmlstring)
        {
            if (string.IsNullOrEmpty(Htmlstring)) return Htmlstring;
            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            //Htmlstring=HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;

        }


        /// <summary>
        /// 用于绑定前端数据
        /// </summary>
        /// <typeparam name="inT"></typeparam>
        /// <typeparam name="outT"></typeparam>
        /// <param name="inClass"></param>
        /// <param name="outClass"></param>
        /// <param name="allPar"></param>
        /// <returns></returns>
        public static IList<DataGridColumns> ClassToJsonColumns<T>() where T : new()
        {
            T t = new T();
            PropertyInfo[] proInfoArr = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);//得到该类的所有公共属性
            IList<DataGridColumns> reList = new List<DataGridColumns>();
            for (int a = 0; a < proInfoArr.Length; a++)
            {
                PropertyInfo outproInfo = t.GetType().GetProperty(proInfoArr[a].Name);
                if (outproInfo != null)
                {
                    var CustomAttributes = outproInfo.GetCustomAttributes(true);
                    for (int i = 0; i < CustomAttributes.Length; i++)
                    {
                        if (CustomAttributes[i].GetType() == typeof(DisplayAttribute))
                        {
                            DisplayAttribute dna = CustomAttributes[i] as DisplayAttribute;
                            reList.Add(new DataGridColumns { field = proInfoArr[a].Name, title = dna.Name, width = 80, sortable = false, editor = new DataGridColumnsEditor { type = "text" } });
                            break;
                        }
                    }
                }
            }
            return reList;
        }


        public static object WriteFileObj = new object();
        public static void WriteAllText(string path, string contents)
        {
            lock (WriteFileObj)
            {
                System.IO.File.WriteAllText(path, ProInterface.JSON.DecodeToStr(contents));
            }
        }

        public static bool WriteModuleLog(string moduleName, int userId)
        {
            try
            {
                using (DBEntities db = new DBEntities())
                {
                    YL_LOG ent = new YL_LOG();
                    ent.ADD_TIME = DateTime.Now;
                    ent.MODULE_NAME = moduleName;
                    ent.USER_ID = userId;
                    db.YL_LOG.Add(ent);
                    db.SaveChanges();
                    return true;

                }
            }
            catch
            {
                return false;
            }
        }


        public static Expression<Func<T, bool>> LambdaAndAlso<T>(
                      Expression<Func<T, bool>> a,
                      Expression<Func<T, bool>> b)
        {

            if (a == null) return b;
            if (b == null) return a;

            var p = Expression.Parameter(typeof(T), "x");
            var bd = Expression.AndAlso(
                    Expression.Invoke(a, p),
                    Expression.Invoke(b, p));
            var ld = Expression.Lambda<Func<T, bool>>(bd, p);
            return ld;
        }

        /// <summary>
        /// 从内存缓存中读取配置。若缓存中不存在，则重新从文件中读取配置，存入缓存
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <returns>配置词典</returns>
        public static T GetObjectCache<T>(string cacheKey) where T : new()
        {
            T configs=new T();
            //1、获取内存缓存对象
            ObjectCache cache = MemoryCache.Default;
            //2、通过Key判断缓存中是否已有词典内容（Key在存入缓存时设置）
            if (MemoryCache.Default.Contains(cacheKey))
            {
                //3、直接从缓存中读取词典内容
                configs = (T)cache.GetCacheItem(cacheKey).Value;
                return configs;
            }

            return configs;
        }

        /// <summary>
        /// 从内存缓存中读取配置。若缓存中不存在，则重新从文件中读取配置，存入缓存
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <returns>配置词典</returns>
        public static bool SetObjectCache(string cacheKey, object Data)
        {


            ObjectCache cache = MemoryCache.Default;
            //4、新建一个CacheItemPolicy对象，该对象用于声明配置对象在缓存中的处理策略
            CacheItemPolicy policy = new CacheItemPolicy();

            //5、因为配置文件一直需要读取，所以在此设置缓存优先级为不应删除
            // 实际情况请酌情考虑，同时可以设置AbsoluteExpiration属性指定过期时间
            policy.Priority = CacheItemPriority.NotRemovable;
            policy.AbsoluteExpiration = DateTime.Now.AddMinutes(10);
            //6、将词典内容添加到缓存，传入 缓存Key、配置对象、对象策略
            // Set方法首先会检查Key是否在缓存中存在，如果存在，更新value，不存在则创建新的
            // 这里先加入缓存再加监视的原因是：在缓存加入时，也会触发监视事件，会导致出错。
            cache.Set(cacheKey, Data, policy);

            return true;

        }


        public static string ExecutePostJson(string server_addr, string postStr, CookieContainer cookieList=null)
        {
            string content = string.Empty;
            try
            {
                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                var cdt= (int)(DateTime.Now - startTime).TotalSeconds;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(server_addr);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Timestamp", Convert.ToString(cdt));
                request.ReadWriteTimeout = 30 * 1000;
                request.CookieContainer = cookieList;
                if (!string.IsNullOrEmpty(postStr))
                {
                    byte[] data = Encoding.UTF8.GetBytes(postStr);
                    request.ContentLength = data.Length;
                    Stream myRequestStream = request.GetRequestStream();
                    myRequestStream.Write(data, 0, data.Length);
                    myRequestStream.Close();
                }
                
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                content = myStreamReader.ReadToEnd();
                myStreamReader.Close();
            }
            catch (Exception ex)
            {
                return "{\"Message\":\"" + ex.Message + "\",\"IsError\":true}";
            }
            return content;
        }

        public static string ExecuteGetJson(string server_addr)
        {
            string content = string.Empty;
            try
            {
                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                var cdt = (int)(DateTime.Now - startTime).TotalSeconds;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(server_addr);
                request.Method = "get";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                content = myStreamReader.ReadToEnd();
                myStreamReader.Close();
            }
            catch (Exception ex)
            {
                return "{\"Message\":\"" + ex.Message + "\",\"IsError\":true}";
            }
            return content;
        }


        public static bool HttpPostEncoded(string Url, string postDataStr,ref string reStr)
        {
            byte[] dataArray = Encoding.UTF8.GetBytes(postDataStr);
            // Console.Write(Encoding.UTF8.GetString(dataArray));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = dataArray.Length;
            //request.CookieContainer = cookie;
            
            try
            {
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(dataArray, 0, dataArray.Length);
                dataStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                reStr = reader.ReadToEnd();
                reader.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static CookieContainer ExecuteGetCookieAndPic(string url,ref Bitmap sourcebm)
        {
            string content = string.Empty;
            try
            {
                CookieContainer myCookieContainer = new CookieContainer();
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string cookieStr = response.Headers["set-cookie"];
                Stream resStream = response.GetResponseStream();//得到验证码数据流
                sourcebm = new Bitmap(resStream);//初始化Bitmap图片
                CookieContainer reList = new CookieContainer();
                var cookieVal = Fun.Substring(cookieStr, "=", ";");
                reList.Add(new Cookie("PHPSESSID", cookieVal, "/", "panda.xmxing.net"));
                return reList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string Substring(string inStr,string startStr,string endStr)
        {
            if (string.IsNullOrEmpty(inStr)) return "";
            int s = inStr.IndexOf(startStr);
            if (s > 0)
            {
                var tmp = inStr.Substring(s + startStr.Length);
                int e = tmp.IndexOf(endStr);
                if (e > 0)
                {
                    return tmp.Substring(0, e);
                }
            }
            return "";
        }

        /// <summary>
        /// 下载
        /// </summary>
        public static bool DownLoadSoft(string DownloadPath, string url, string FileName)
        {
            string path = DownloadPath.Remove(DownloadPath.Length - 1);
            bool flag = false;
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                using (FileStream fs = new FileStream(DownloadPath + FileName, FileMode.Create, FileAccess.Write))
                {
                    //创建请求
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    //接收响应
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    //输出流
                    Stream responseStream = response.GetResponseStream();
                    byte[] bufferBytes = new byte[10000];//缓冲字节数组
                    int bytesRead = -1;
                    while ((bytesRead = responseStream.Read(bufferBytes, 0, bufferBytes.Length)) > 0)
                    {
                        fs.Write(bufferBytes, 0, bytesRead);
                    }
                    if (fs.Length > 0)
                    {
                        flag = true;
                    }
                    //关闭写入
                    fs.Flush();
                    fs.Close();
                }

            }
            catch (Exception exp)
            {
                //返回错误消息
            }
            return flag;
        }

    }

}
