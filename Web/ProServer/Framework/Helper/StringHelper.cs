using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace ProServer.Helper
{
    /// <summary>
    /// 字符串助手
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// 创建不带分割符的Guid，长度为32位
        /// 结果如：38a019e51f45495f81cdc360e5d38a5c
        /// </summary>
        public static string NewGuid()
        {
            return Guid.NewGuid().ToString("N").ToUpperInvariant();
        }

        ///// <summary>
        ///// 获取客户端MAC地址
        ///// </summary>
        //public static string GetMacAddress()
        //{
        //    string MAC = string.Empty;
        //    var mc = new System.Management.ManagementClass("Win32_NetworkAdapterConfiguration");
        //    System.Management.ManagementObjectCollection mocs = mc.GetInstances();
        //    foreach (System.Management.ManagementObject moc in mocs)
        //        if (moc["IPEnabled"].ToString() == "True")
        //        {
        //            MAC = moc["MacAddress"].ToString();
        //            MAC = MAC.Replace(':', '-');
        //            break;
        //        }
        //    return MAC ?? string.Empty;
        //}

        /// <summary>
        /// 加码(Microsoft.JScript.dll)
        /// </summary>
        public static string JSEscape(object @string)
        {
            string text1 = Convert.ToString(@string);
            string text2 = "0123456789ABCDEF";
            int num1 = text1.Length;
            StringBuilder builder1 = new StringBuilder(num1 * 2);
            int num3 = -1;
            while (++num3 < num1)
            {
                char ch1 = text1[num3];
                int num2 = ch1;
                if ((((0x41 > num2) || (num2 > 90)) && ((0x61 > num2) || (num2 > 0x7a))) && ((0x30 > num2) || (num2 > 0x39)))
                {
                    switch (ch1)
                    {
                        case '@':
                        case '*':
                        case '_':
                        case '+':
                        case '-':
                        case '.':
                        case '/':
                            goto Label_0125;
                    }
                    builder1.Append('%');
                    if (num2 < 0x100)
                    {
                        builder1.Append(text2[num2 / 0x10]);
                        ch1 = text2[num2 % 0x10];
                    }
                    else
                    {
                        builder1.Append('u');
                        builder1.Append(text2[(num2 >> 12) % 0x10]);
                        builder1.Append(text2[(num2 >> 8) % 0x10]);
                        builder1.Append(text2[(num2 >> 4) % 0x10]);
                        ch1 = text2[num2 % 0x10];
                    }
                }
            Label_0125:
                builder1.Append(ch1);
            }
            return builder1.ToString();
        }

        /// <summary>
        /// 解码(Microsoft.JScript.dll)
        /// </summary>
        public static string JSUnescape(object @string)
        {
            string text1 = Convert.ToString(@string);
            int num1 = text1.Length;
            StringBuilder builder1 = new StringBuilder(num1);
            int num6 = -1;
            while (++num6 < num1)
            {
                char ch1 = text1[num6];
                if (ch1 == '%')
                {
                    int num2;
                    int num3;
                    int num4;
                    int num5;
                    if (((((num6 + 5) < num1) && (text1[num6 + 1] == 'u')) && (((num2 = HexDigit(text1[num6 + 2])) != -1) &&
                        ((num3 = HexDigit(text1[num6 + 3])) != -1))) && (((num4 = HexDigit(text1[num6 + 4])) != -1) &&
                        ((num5 = HexDigit(text1[num6 + 5])) != -1)))
                    {
                        ch1 = (char)((ushort)((((num2 << 12) + (num3 << 8)) + (num4 << 4)) + num5));
                        num6 += 5;
                    }
                    else if ((((num6 + 2) < num1) && ((num2 = HexDigit(text1[num6 + 1])) != -1)) && ((num3 = HexDigit(text1[num6 + 2])) != -1))
                    {
                        ch1 = (char)((ushort)((num2 << 4) + num3));
                        num6 += 2;
                    }
                }
                builder1.Append(ch1);
            }
            return builder1.ToString();
        }

        internal static int HexDigit(char c)
        {
            if ((c >= '0') && (c <= '9'))
            {
                return (c - '0');
            }
            if ((c >= 'A') && (c <= 'F'))
            {
                return (('\n' + c) - 'A');
            }
            if ((c >= 'a') && (c <= 'f'))
            {
                return (('\n' + c) - 'a');
            }
            return -1;
        }

        /// <summary>
        /// 字符是否是字母
        /// </summary>
        public static bool CharIsLetter(char c)
        {
            return char.IsLower(c) || char.IsUpper(c);
        }

        /// <summary>
        /// 生成引用字符串
        /// </summary>
        /// <example>abc --> "abc"</example>
        /// <param name="source">源字符串</param>
        /// <param name="quoteChar">使用什么作为引用字符</param>
        /// <returns>string</returns>
        public static string ToQuote(string source, char quoteChar)
        {
            if (string.IsNullOrEmpty(source))
                return quoteChar.ToString() + quoteChar;
            if (source.IndexOf(quoteChar) == -1)
                return quoteChar + source + quoteChar;
            int vQuoteCharAscii = quoteChar;
            StringBuilder sb = new StringBuilder();
            sb.Append(quoteChar);
            foreach (char c in source)
            {
                if (c == vQuoteCharAscii)
                    sb.Append(c);
                sb.Append(c);
            }
            sb.Append(quoteChar);
            return sb.ToString();
        }
        /// <summary>
        /// 生成引用字符串
        /// </summary>
        /// <example>abc --> "abc"</example>
        /// <param name="source">源字符串</param>
        /// <param name="quoteChar">使用什么作为引用字符</param>
        /// <returns>string</returns>
        public static string ToQuote(string source, string quoteChar)
        {
            return ToQuote(source, quoteChar[0]);
        }
        /// <summary>
        /// 生成引用字符串，使用<c>"</c>作为引用字符
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>string</returns>
        public static string ToQuote(string source)
        {
            return ToQuote(source, '"');
        }
        /// <summary>
        /// 生成引用字符串，使用<c>'</c>作为引用字符
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>string</returns>
        public static string ToQuoteSingle(string source)
        {
            return ToQuote(source, '\'');
        }
        /// <summary>
        /// 生成引用字符串
        /// </summary>
        public static string ToQuote(string separator, string[] source, char quoteChar)
        {
            if (source == null || source.Length == 0)
                return quoteChar.ToString() + quoteChar;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < source.Length; i++)
            {
                if (i > 0)
                    sb.Append(separator);
                sb.Append(ToQuote(source[i], quoteChar));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 生成引用字符串，使用<c>'</c>作为引用字符
        /// </summary>
        public static string ToQuoteSingle(string separator, string[] source)
        {
            return ToQuote(separator, source, '\'');
        }
        /// <summary>
        /// 解析引用字符串
        /// </summary>
        /// <example>"abc" --> abc</example>
        /// <param name="source">源字符串</param>
        /// <param name="quoteChar">使用什么作为引用字符</param>
        /// <param name="refIndex">设置开始位置，返回结束位置</param>
        /// <returns>string</returns>
        public static string FromQuote(string source, char quoteChar, ref int refIndex)
        {
            if (string.IsNullOrEmpty(source))
            {
                refIndex = -1;
                return "";
            }
            if (refIndex < 0 || refIndex >= source.Length)
            {
                refIndex = -1;
                return "";
            }
            if (source[refIndex] != quoteChar)
            {
                int index = refIndex;
                refIndex = -1;
                return source.Substring(index, source.Length - index);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                int index = refIndex + 1;
                while (index < source.Length)
                {
                    if (source[index] == quoteChar)
                    {
                        index++;
                        if (index == source.Length)
                            break;
                        if (source[index] == quoteChar)
                        {
                            sb.Append(source[index]);
                            index++;
                        }
                        else
                            break;
                    }
                    else
                    {
                        sb.Append(source[index]);
                        index++;
                    }
                }
                if (index < source.Length)
                    refIndex = index;
                else
                    refIndex = -1;
                return sb.ToString();
            }
        }

        /// <summary>
        /// 解析引用字符串
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="quoteChar">使用什么作为引用字符</param>
        /// <returns>string</returns>
        public static string FromQuote(string source, char quoteChar)
        {
            int index = 0;
            return FromQuote(source, quoteChar, ref index);
        }
        /// <summary>
        /// 解析引用字符串，使用<c>"</c>作为引用字符
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>string</returns>
        public static string FromQuote(string source)
        {
            return FromQuote(source, '"');
        }
        /// <summary>
        /// 解析引用字符串，使用<c>'</c>作为引用字符
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>string</returns>
        public static string FromQuoteSingle(string source)
        {
            return FromQuote(source, '\'');
        }
        /// <summary>
        /// 是否全部由0-9数字的数字组成
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>bool</returns>
        public static bool IsDigit(string source)
        {
            foreach (char c in source)
                if (!char.IsDigit(c))
                    return false;
            return true;
        }
        /// <summary>
        /// 是否为有效的整数
        /// </summary>
        /// <param name="source">源字符串</param>
        public static bool IsInt(string source)
        {
            int o;
            return int.TryParse(source, out o);
        }
        /// <summary>
        /// 是否为有效的浮点数
        /// </summary>
        /// <param name="source">源字符串</param>
        public static bool IsFloat(string source)
        {
            float f;
            return float.TryParse(source, out f);
        }
        /// <summary>
        /// 是否为有效的浮点数
        /// </summary>
        /// <param name="source">源字符串</param>
        public static bool IsDouble(string source)
        {
            double f;
            return double.TryParse(source, out f);
        }
        /// <summary>
        /// 是否为有效的日期
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static bool IsDateTime(string source)
        {
            DateTime dt;
            return DateTime.TryParse(source, out dt);
        }
        /// <summary>
        /// 源字符串是分全部由 charArray 的字符组成
        /// </summary>
        public static bool IsCustomChars(string source, char[] charArray)
        {
            if (string.IsNullOrEmpty(source))
                return true;
            foreach (char c in source)
            {
                if (Array.IndexOf(charArray, c) == -1)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 源字符串是分全部由 strValue 中的字符组成
        /// </summary>
        public static bool IsCustomChars(string source, string strValue)
        {
            return IsCustomChars(source, strValue.ToCharArray());
        }
        /// <summary>
        /// 字符串仅包含字符
        /// </summary>
        public static bool IsLetter(string source)
        {
            if (string.IsNullOrEmpty(source))
                return true;
            foreach (char c in source)
                if (!CharIsLetter(c))
                    return false;
            return true;
        }
        /// <summary>
        /// 字符串只包含字符数字
        /// </summary>
        public static bool IsLetterOrDigit(string source)
        {
            if (string.IsNullOrEmpty(source))
                return true;
            foreach (char c in source)
                if (!(char.IsDigit(c) || CharIsLetter(c)))
                    return false;
            return true;
        }
        /// <summary>
        /// 字符串只包含字符数字下划线
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsLetterOrDigitOrLine(string source)
        {
            if (string.IsNullOrEmpty(source))
                return true;
            foreach (char c in source)
                if (!(char.IsDigit(c) || CharIsLetter(c) || c == '_'))
                    return false;
            return true;
        }
        /// <summary>
        /// 以字符或下划线打头
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsStartByLetterOrLine(string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;
            return CharIsLetter(source[0]) || (source[0] == '_');
        }

        /// <summary>
        /// 是否含有中文字符
        /// </summary>
        public static bool IsIncUnicodeChar(string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;
            foreach (char ch in source)
            {
                if (ch > 255)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 删除中文字符
        /// </summary>
        public static string RemoveUnicodeChar(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (char ch in source)
            {
                if (ch <= 255)
                    sb.Append(ch);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 比较2个字符串是否相同，忽略大小写
        /// </summary>
        public static bool IsSame(string s, string s2)
        {
            return IsSame(s, s2, true);
        }
        /// <summary>
        /// 比较2个字符串是否相同
        /// </summary>
        public static bool IsSame(string s, string s2, bool ignoreCase)
        {
            return string.Compare(s, s2, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture) == 0;
        }
        /// <summary>
        /// 比较字符串开始部分是否相同，忽略大小写
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="subStr">比较的子串</param>
        /// <returns></returns>
        public static bool IsSameStart(string source, string subStr)
        {
            return IsSameStart(source, subStr, true);
        }
        /// <summary>
        /// 比较字符串开始部分是否相同
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="subStr">比较的子串</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static bool IsSameStart(string source, string subStr, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(subStr))
                return false;

            if (subStr.Length > source.Length)
                return false;
            return (0 == string.Compare(source, 0, subStr, 0, subStr.Length, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture));
        }
        /// <summary>
        /// 比较字符串结束部分是否相同
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="subStr">比较的子串</param>
        /// <returns></returns>
        public static bool IsSameEnd(string source, string subStr)
        {
            return IsSameEnd(source, subStr, true);
        }
        /// <summary>
        /// 比较字符串结束部分是否相同
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="subStr">比较的子串</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static bool IsSameEnd(string source, string subStr, bool ignoreCase)
        {
            int num1 = source.Length - subStr.Length;
            if (num1 < 0)
                return false;
            return (0 == string.Compare(source, num1, subStr, 0, subStr.Length, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture));
        }
        /// <summary>
        /// 比较从index位置开始的字符串是否相同，忽略大小写
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="subStr">比较的子串</param>
        /// <param name="index">开始位置</param>
        /// <returns></returns>
        public static bool IsSameRange(string source, string subStr, int index)
        {
            return IsSameRange(source, subStr, index, true);
        }
        /// <summary>
        /// 比较从index位置开始的字符串是否相同
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="subStr">比较的子串</param>
        /// <param name="index">开始位置</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static bool IsSameRange(string source, string subStr, int index, bool ignoreCase)
        {
            if (null == subStr)
                throw new ArgumentNullException("subStr");
            if (source.Length < subStr.Length || index < 0 || index >= source.Length ||
                source.Length - index < subStr.Length)
                return false;
            return IsSame(source.Substring(index, subStr.Length), subStr, ignoreCase);
        }
        /// <summary>
        /// 比较2个字符串数组是否相同
        /// </summary>
        public static bool IsSameStrings(string[] arr, string[] arr2, bool ignoreCase)
        {
            if (arr == null && arr2 == null)
                return true;
            if (arr == null || arr2 == null)
                return false;
            if (arr.Length != arr2.Length)
                return false;
            for (int i = 0; i < arr.Length; i++)
            {
                string s1 = arr[i], s2 = arr2[i];
                if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
                    continue;
                if (!IsSame(arr[i], arr2[i], ignoreCase))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 字符串是否长度为0  和 string.IsNullOrEmpty 相同
        /// </summary>
        /// <param name="source">字符串</param>
        /// <returns></returns>
        public static bool IsLengthZero(string source)
        {
            return string.IsNullOrEmpty(source);
        }
        /// <summary>
        /// 字符串是否在一个字符串数组中
        /// </summary>
        public static bool IsOneOf(string source, string[] strArr, bool ignoreCase)
        {
            if (strArr == null || strArr.Length == 0)
                return false;
            foreach (string s in strArr)
            {
                if (IsSame(s, source, ignoreCase))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 报告指定的 String 在此实例中的第一个匹配项的索引。搜索从指定字符位置开始，并检查指定数量的字符位置。
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="str">查找的字符串</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="count">要检查的字符位置数</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns>如果找到该字符串，则为 value 的索引位置；否则如果未找到，则为 -1</returns>
        public static int IndexOf(string source, string str, int startIndex, int count, bool ignoreCase)
        {
            if (ignoreCase)
                return source.ToLower().IndexOf(str.ToLower(), startIndex, count);
            return source.IndexOf(str, startIndex, count);
        }

        /// <summary>
        /// 报告指定的 String 在此实例中的第一个匹配项的索引。搜索从指定字符位置开始，并检查指定数量的字符位置。
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="str">查找的字符串</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns>如果找到该字符串，则为 value 的索引位置；否则如果未找到，则为 -1</returns>
        public static int IndexOf(string source, string str, int startIndex, bool ignoreCase)
        {
            return IndexOf(source, str, startIndex, source.Length - startIndex, ignoreCase);
        }
        /// <summary>
        /// 报告指定的 String 在此实例中的第一个匹配项的索引。搜索从指定字符位置开始，并检查指定数量的字符位置。
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="str">查找的字符串</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns>如果找到该字符串，则为 value 的索引位置；否则如果未找到，则为 -1</returns>
        public static int IndexOf(string source, string str, bool ignoreCase)
        {
            return IndexOf(source, str, 0, ignoreCase);
        }
        /// <summary>
        /// 报告指定字符在此实例中的第一个匹配项的索引。搜索从指定字符位置开始，并检查指定数量的字符位置
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="ch">字符</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="count">要检查的字符位置数</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns>如果找到该字符，则为 value 的索引位置；否则如果未找到，则为 -1</returns>
        public static int IndexOf(string source, char ch, int startIndex, int count, bool ignoreCase)
        {
            if (ignoreCase)
                return source.ToLower().IndexOf(Char.ToLower(ch), startIndex, count);
            return source.IndexOf(ch, startIndex, count);
        }

        /// <summary>
        /// 报告指定字符在此实例中的第一个匹配项的索引。搜索从指定字符位置开始，并检查指定数量的字符位置
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="ch">字符</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns>如果找到该字符，则为 value 的索引位置；否则如果未找到，则为 -1</returns>
        public static int IndexOf(string source, char ch, int startIndex, bool ignoreCase)
        {
            return IndexOf(source, ch, startIndex, source.Length - startIndex, ignoreCase);
        }
        /// <summary>
        /// 报告指定字符在此实例中的第一个匹配项的索引。搜索从指定字符位置开始，并检查指定数量的字符位置
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="ch">字符</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns>如果找到该字符，则为 value 的索引位置；否则如果未找到，则为 -1</returns>
        public static int IndexOf(string source, char ch, bool ignoreCase)
        {
            return IndexOf(source, ch, 0, ignoreCase);
        }
        /// <summary>
        /// 统计 find  在 source 中的个数
        /// </summary>
        public static int IndexOfCount(string source, string find, bool ignoreCase)
        {
            int count = 0;
            int index = IndexOf(source, find, ignoreCase);
            while (index >= 0)
            {
                count++;
                index = IndexOf(source, find, index + find.Length, ignoreCase);
            }
            return count;
        }
        /// <summary>
        /// 从字符串数组中查找
        /// </summary>
        public static int IndexOf(string[] sourceArray, string find, bool ignoreCase)
        {
            if (sourceArray == null || sourceArray.Length == 0)
                return -1;
            for (int i = 0; i < sourceArray.Length; i++)
                if (IsSame(sourceArray[i], find, ignoreCase))
                    return i;
            return -1;
        }
        /// <summary>
        /// 复制字符串
        /// </summary>
        public static string Copy(string source, int timers)
        {
            if (timers <= 0)
                return string.Empty;
            if (timers == 1)
                return source;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < timers; i++)
                sb.Append(source);
            return sb.ToString();
        }
        /// <summary>
        /// 替换字符
        /// </summary>
        /// <param name="source"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static string Replace(string source, string oldValue, string newValue, bool ignoreCase)
        {
            return Replace(source, oldValue, newValue, 0, source.Length, ignoreCase);
        }
        /// <summary>
        /// 替换字符
        /// </summary>
        /// <param name="source"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static string Replace(string source, string oldValue, string newValue, int startIndex, bool ignoreCase)
        {
            int i = startIndex;
            if (i < 0)
                i = 0;
            return Replace(source, oldValue, newValue, i, source.Length - i, ignoreCase);
        }
        /// <summary>
        /// 替换字符
        /// </summary>
        /// <param name="source"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="count">替换字符个数</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static string Replace(string source, string oldValue, string newValue, int startIndex, int count, bool ignoreCase)
        {
            if (startIndex < 0)
                startIndex = 0;
            if (ignoreCase)
            {
                StringBuilder sb = new StringBuilder(10);
                if (startIndex > 0)
                    sb.Append(source.Substring(0, startIndex));
                int i = startIndex;
                int j = count;
                int findIndex = IndexOf(source, oldValue, i, j, true);
                while (findIndex != -1)
                {
                    if (findIndex > 0)
                        sb.Append(source.Substring(i, findIndex - i));
                    sb.Append(newValue);
                    i = findIndex + oldValue.Length;
                    j = count - i + startIndex;
                    if (j <= 0)
                        break;
                    findIndex = IndexOf(source, oldValue, i, j, true);
                }
                if (i < source.Length)
                    sb.Append(source.Substring(i, source.Length - i));
                return sb.ToString();
            }
            else
            {
                StringBuilder sb = new StringBuilder(source);
                return sb.Replace(oldValue, newValue, startIndex, count).ToString();
            }
        }

        /// <summary>
        /// 删除无效字符
        /// </summary>
        public static string RemoveChars(string source, char[] chars)
        {
            if (string.IsNullOrEmpty(source) || chars == null || chars.Length == 0)
                return source;
            List<char> lc = new List<char>(chars);
            lc.Sort();
            StringBuilder sb = new StringBuilder();
            foreach (char c in source)
            {
                if (lc.BinarySearch(c) < 0)
                    sb.Append(c);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 通用的比较器，取最大值 
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="item1">参数</param>
        /// <param name="items">参数数组</param>    
        public static T Max<T>(T item1, params T[] items) where T : IComparable
        {
            T t = item1;
            for (int i = 0; i < items.Length; i++)
                if (items[i].CompareTo(t) > 0)
                    t = items[i];
            return t;
        }
        /// <summary>
        /// 通用的比较器，取最小值 
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="item1">参数</param>
        /// <param name="items">参数数组</param>
        public static T Min<T>(T item1, params T[] items) where T : IComparable
        {
            T t = item1;
            for (int i = 0; i < items.Length; i++)
                if (items[i].CompareTo(t) < 0)
                    t = items[i];
            return t;
        }
        /// <summary>
        /// 通用的交换器 
        /// </summary>
        public static void Swap<T>(ref T item1, ref T item2)
        {
            T t = item1;
            item1 = item2;
            item2 = t;
        }
        /// <summary>
        /// 把一个列表保存到一个字符串中
        /// 按 ， 号分隔开
        /// </summary>
        /// <param name="list">源列表</param>
        /// <returns>结果字符串</returns>
        public static string GetListDelimitedText(IEnumerable list)
        {
            if (list == null)
                return string.Empty;
            const int cSpaceCharAscii = (int)' ';
            const char cCharDelimiter = ',';
            const char cCharQuoteChar = '"';

            StringBuilder sb = new StringBuilder();
            foreach (object o in list)
            {
                bool vIsNeedQuote = false;
                string s = o.ToString();
                foreach (char c in s)
                {
                    int vCharAscii = c;
                    if (vCharAscii >= 0 && vCharAscii <= cSpaceCharAscii ||
                        c == cCharQuoteChar || c == cCharDelimiter)
                    {
                        vIsNeedQuote = true;
                        break;
                    }
                }
                if (vIsNeedQuote)
                    sb.Append(ToQuote(s, cCharQuoteChar)).Append(cCharDelimiter);
                else
                    sb.Append(s).Append(cCharDelimiter);
            }
            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);
            return sb.Length == 0 ? string.Empty : sb.ToString();
        }
        /// <summary>
        /// 把按 ， 号分隔开字符串，拆开保存到一个IList中
        /// </summary>
        /// <param name="list">目标列表</param>
        /// <param name="delimitedText">按 ， 号分隔开字符串</param>
        /// <param name="isAddMode">是否保留原来的元素</param>
        public static void SetListDelimitedText(IList list, string delimitedText, bool isAddMode)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            const char cCharDelimiter = ',';
            const char cCharQuoteChar = '"';
            int index = 0;
            int len = delimitedText.Length;

            if (!isAddMode)
                list.Clear();
            while (index < len && index > -1)
            {
                string s;
                if (delimitedText[index] == cCharQuoteChar)
                {
                    s = FromQuote(delimitedText, cCharQuoteChar, ref index);
                }
                else
                {
                    int p = delimitedText.IndexOf(cCharDelimiter, index, len - index);
                    if (p != -1)
                    {
                        s = delimitedText.Substring(index, p - index);
                        p++;
                    }
                    else
                        s = delimitedText.Substring(index, len - index);
                    index = p;
                }
                list.Add(s);
                if (index > -1 && index < len && delimitedText[index] == cCharDelimiter)
                    index++;
            }
        }
        /// <summary>
        /// 把一个列表保存到一个字符串中
        /// 按 回车换行 号分隔开
        /// </summary>
        /// <param name="aList">源列表</param>
        /// <returns>结果字符串</returns>
        public static string GetListCRLFText(IEnumerable aList)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object o in aList)
                sb.AppendLine(o.ToString());
            if (sb.Length > 0)
                sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }
        /// <summary>
        /// 把按 回车换行 号分隔开字符串，拆开保存到一个IList中
        /// </summary>
        /// <param name="aList">目标列表</param>
        /// <param name="aCRLFText">按 回车换行 号分隔开字符串</param>
        /// <param name="aIsAddMode">是否保留原来的元素</param>
        public static void SetListCRLFText(IList aList, string aCRLFText, bool aIsAddMode)
        {
            string s = aCRLFText;
            int i = 0;
            int l = s.Length;
            if (!aIsAddMode)
                aList.Clear();
            while (i < l)
            {
                int start = i;
                char c = s[i];
                while (i < l && c != '\r' && c != '\n')
                {
                    i++;
                    if (i < l)
                        c = s[i];
                }
                aList.Add(s.Substring(start, i - start));
                if (i < l && s[i] == '\r')
                    i++;
                if (i < l && s[i] == '\n')
                    i++;
            }
        }
        /// <summary>
        /// 数字按Step的步进递增
        /// 0==>9  9==>0
        /// A==>Z  Z==>A
        /// a==>Z  z==>A
        /// 其他字符跳过
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="step">步进</param>
        /// <returns>转化后的字符串</returns>
        public static string IncLetterOrDigit(string source, int step)
        {
            if (IsLengthZero(source) || (step <= 0))
                return source;
            StringBuilder result = new StringBuilder(source);
            for (int i = source.Length - 1; i >= 0; i--)
            {
                char charCurr = source[i];
                char charFirst;
                char charLast;
                if (char.IsDigit(charCurr))
                {
                    charFirst = '0';
                    charLast = '9';
                }
                else if (char.IsLetter(charCurr))
                {
                    charCurr = char.ToUpper(charCurr);
                    charFirst = 'A';
                    charLast = 'Z';
                }
                else
                    continue;
                int j = charLast - charCurr;
                if (j < step)
                {
                    result[i] = (char)(charFirst + step - j - 1);
                    step = 1;
                }
                else
                {
                    result[i] = (char)(charCurr + step);
                    break;
                }
            }
            return result.ToString();
        }
        /// <summary>
        /// 数字按Step的步进递增
        /// 0==>9  9==>A  A==>Z  Z==>0
        /// a==>Z  
        /// z==>0
        /// 其他字符跳过
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="step">步进</param>
        /// <returns>转化后的字符串</returns>
        public static string IncLetterAndDigit(string source, int step)
        {
            if (IsLengthZero(source) || (step <= 0))
                return source;
            StringBuilder result = new StringBuilder(source);
            for (int i = source.Length - 1; i >= 0; i--)
            {
                char charCurr = source[i];
                char charLast;
                if (char.IsDigit(charCurr))
                {
                    charLast = '9';
                }
                else if (char.IsLetter(charCurr))
                {
                    charCurr = char.ToUpper(charCurr);
                    charLast = 'Z';
                }
                else
                    continue;
                int j = charLast - charCurr;
                if (j < step)
                {
                    if (char.IsDigit(charCurr))
                    {
                        result[i] = (char)('A' + step - j - 1);
                        break;
                    }
                    if (char.IsLetter(charCurr))
                    {
                        result[i] = (char)('0' + step - j - 1);
                        step = 1;
                    }
                }
                else
                {
                    result[i] = (char)(charCurr + step);
                    break;
                }
            }
            return result.ToString();
        }

        /// <summary>
        ///  把数字转换为 26 进制
        ///  如： 1 --> A,  27 --> AA 
        /// </summary>
        public static string IntegerToLetter(int value)
        {
            return InternalIntegerToLetter(value - 1);
        }
        private static string InternalIntegerToLetter(int value)
        {
            string result = string.Empty;
            if (value >= 26)
                result += InternalIntegerToLetter(value / 26 - 1);
            result += (char)('A' + value % 26);
            return result;
        }
        /// <summary>
        ///  把26 进制转换为数字
        ///  如： A --> 1, AA --> 27
        /// </summary>
        public static int LetterToInteger(string letter)
        {
            int result = 0;
            for (int i = letter.Length - 1; i >= 0; i--)
                result += ((int)Math.Pow(26, letter.Length - i - 1)) * (letter[i] - 'A' + 1);
            return result;
        }
        /// <summary>
        /// 对List进行节点交换
        /// </summary>
        public static void ListExchange<T>(List<T> list, int from, int to)
        {
            T temp = list[from];
            list[from] = list[to];
            list[to] = temp;
        }

        private static string ObjectToString(object obj, int level)
        {
            if (obj == null)
                return string.Empty;
            if (level > 10)
                return obj.ToString();
            Type t = obj.GetType();
            if (t == typeof(string))
                return obj.ToString();
            if (t.IsEnum)
                return t.Name + "." + obj;
            if (t.IsValueType)
            {
                int index = t.FullName.IndexOf('.');
                if (t.FullName.IndexOf('.', index + 1) < 0)
                    return obj.ToString();
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(t.Name);
            if (t.IsArray)
            {
                Array arr = (Array)obj;
                sb.Append(" <").Append(arr.Length).Append(">").AppendLine();
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i == 10)
                    {
                        sb.Append('\t', level + 1).Append("更多...");
                        break;
                    }
                    sb.Append('\t', level + 1).AppendFormat("[{0}]: {1}", i, ObjectToString(arr.GetValue(i), level + 1));
                    if (i < arr.Length - 1)
                        sb.AppendLine();
                }
            }
            else
            {
                IEnumerable ea = obj as IEnumerable;
                if (ea != null)
                {
                    sb.AppendLine();
                    int i = 0;
                    foreach (object o in ea)
                    {
                        if (i == 50)
                        {
                            sb.Append('\t', level + 1).Append("更多...");
                            break;
                        }
                        sb.Append('\t', level + 1).AppendFormat("[{0}]: {1}", i, ObjectToString(o, level + 1)).AppendLine();
                        i++;
                    }
                    sb.Length -= 2;
                    if (i == 0)
                        sb.Append(" <0>");
                }
                else
                {
                    PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance);
                    if (pi != null && pi.Length > 0)
                    {
                        sb.AppendLine();
                        foreach (PropertyInfo info in pi)
                        {
                            if (info.Name == "Item" || info.Name == "AsString" || info.Name == "AsObject" || info.Name == "AsCryptoString")
                                continue;
                            sb.Append('\t', level + 1).Append(info.Name).Append(": ");
                            try
                            {
                                sb.Append(ObjectToString(info.GetValue(obj, null), level + 1));
                            }
                            catch (Exception e)
                            {
                                sb.Append(e.Message);
                            }
                            sb.AppendLine();
                        }
                        sb.Length -= 2;
                    }
                    else
                        sb.Append("\t").Append(obj.ToString());
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 对象转换为字符串
        /// </summary>
        public static string ObjectToString(object obj)
        {
            return ObjectToString(obj, 0);
        }
        /// <summary>
        /// 取指定长度的随机字符串
        /// </summary>
        public static string BuildRandomString(int length)
        {
            const string randStr = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return BuildRandomString(randStr, length);
        }

        /// <summary>
        /// 取指定长度的随机字符串
        /// </summary>
        public static string BuildRandomString(string randomSource, int length)
        {
            return BuildRandomString(randomSource, length, new Random());
        }
        /// <summary>
        /// 取指定长度的随机字符串
        /// </summary>
        public static string BuildRandomString(string randomSource, int length, Random random)
        {
            string randStr = randomSource;
            StringBuilder sb = new StringBuilder(length);
            Random r = random;
            for (int i = 0; i < length; i++)
                sb.Append(randStr[r.Next(randStr.Length)]);
            return sb.ToString();
        }
        /// <summary>
        /// 字符串转换为日期
        /// </summary>
        public static bool TryStrToDate(string source, out DateTime date)
        {
            if (string.IsNullOrEmpty(source))
            {
                date = default(DateTime);
                return false;
            }
            if (DateTime.TryParse(source, out date))
                return true;
            string[] dateFormatList = { "yyyy-mm", "yyyymm", "yyyymmdd", "yyyy-mm-dd" };
            string s = source;
            foreach (var dateFormat in dateFormatList)
            {
                if (s.Length != dateFormat.Length)
                    continue;
                char[] year = { '1', '9', '0', '0' }, month = { '0', '1' }, day = { '0', '1' };
                int yearLen = 3, monthLen = 1, dayLen = 1;
                for (int i = dateFormat.Length - 1; i >= 0; i--)
                    switch (dateFormat[i])
                    {
                        case 'y':
                        case 'Y':
                            if (yearLen >= 0)
                                year[yearLen--] = s[i];
                            break;
                        case 'm':
                        case 'M':
                            if (monthLen >= 0)
                                month[monthLen--] = s[i];
                            break;
                        case 'd':
                        case 'D':
                            if (dayLen >= 0)
                                day[dayLen--] = s[i];
                            break;
                        default:
                            break;
                    }
                int iyear, imonth, iday;
                if (int.TryParse(new string(year), out iyear) && int.TryParse(new string(month), out imonth)
                    && int.TryParse(new string(day), out iday))
                {
                    try
                    {
                        date = new DateTime(iyear, imonth, iday);
                        return true;
                    }
                    catch
                    {
                    }
                }
            }

            int numberStart = 0, numberCount = 0, numberSection = 1;
            string yearStr = string.Empty, monthStr = string.Empty, dayStr = string.Empty;
            foreach (var c in source)
            {
                if (char.IsDigit(c))
                    numberCount++;
                else
                {
                    if (numberSection == 1)
                    {
                        if (numberCount == 2)
                            yearStr = "19" + source.Substring(numberStart, numberCount);
                        else
                            yearStr = source.Substring(numberStart, numberCount);
                    }
                    else if (numberSection == 2)
                    {
                        if (numberCount == 1)
                            monthStr = "0" + source.Substring(numberStart, numberCount);
                        else
                            monthStr = source.Substring(numberStart, numberCount);
                    }
                    else if (numberSection == 3)
                    {
                        if (numberCount == 1)
                            dayStr = "0" + source.Substring(numberStart, numberCount);
                        else
                            dayStr = source.Substring(numberStart, numberCount);
                    }
                    numberStart = numberStart + numberCount;
                    numberSection++;
                }
            }
            if (DateTime.TryParse(yearStr + "-" + monthStr + "-" + dayStr, out date))
                return true;
            return false;
        }

        /// <summary>
        /// 把SQL按Go拆分开
        /// </summary>
        public static List<string> SplitGoSql(string sqlText, string resultCrlf)
        {
            var lines = sqlText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return SplitGoSql(lines, resultCrlf);
        }
        /// <summary>
        /// 把SQL按Go拆分开
        /// </summary>
        public static List<string> SplitGoSql(string[] sqlList, string crlf)
        {
            var list = new List<string>();
            if (sqlList == null || sqlList.Length == 0)
                return list;
            StringBuilder sb = new StringBuilder();
            foreach (string s in sqlList)
            {
                string s2 = s.Trim();
                if (string.Compare(s2, "GO", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    if (sb.Length > 0)
                        list.Add(sb.ToString());
                    sb.Length = 0;
                }
                else if (!string.IsNullOrEmpty(s))
                {
                    if (sb.Length > 0)
                        sb.Append(crlf);
                    sb.Append(s);
                }
            }
            if (sb.Length > 0)
                list.Add(sb.ToString());
            return list;
        }
        /// <summary>
        /// 把SQL按Go拆分开
        /// </summary>
        public static List<string> SplitGoMSSql(string[] sqlList)
        {
            return SplitGoSql(sqlList, "\r\n");
        }
        /// <summary>
        /// 把SQL按Go拆分开
        /// </summary>
        public static List<string> SplitGoMSSql(string sqlText)
        {
            return SplitGoSql(sqlText, "\r\n");
        }

        /// <summary>
        /// 生成In里的SQL
        /// </summary>
        public static string InSQLPart(IEnumerable list, bool needQuote)
        {
            if (needQuote)
            {
                if (list == null)
                    return "''";
                StringBuilder sb = new StringBuilder();
                foreach (var item in list)
                    sb.AppendQuoteSingle(item.ToString()).Append(",");
                if (sb.Length > 0)
                {
                    sb.Length--;
                    return sb.ToString();
                }
                return "''";
            }
            if (list == null)
                return "0";
            StringBuilder sb2 = new StringBuilder();
            foreach (var item in list)
                sb2.Append(item.ToString()).Append(",");
            if (sb2.Length > 0)
            {
                sb2.Length--;
                return sb2.ToString();
            }
            return "0";
        }

        /// <summary>
        /// 统计列表个数，为null统计为0
        /// </summary>
        public static int CountIf<T>(List<T> list)
        {
            if (list == null)
                return 0;
            return list.Count;
        }

        /// <summary>
        /// 判断列表有没有重复项
        /// </summary>
        /// <param name="allowSort">是否允许排序，排序后会影响到列表</param>
        /// <param name="compare">比较列表中任意两项的委托方法</param>
        public static bool HasRepeatItem<T>(List<T> list, bool allowSort, Func<T, T, int> compare)
        {
            if (CountIf(list) <= 1)
                return false;

            bool hasCompareHander = compare != null;

            Type type = typeof(T);
            if (compare == null)
            {
                if (type.GetInterface("IComparable`1") != null)
                    compare = delegate(T t1, T t2)
                    {
                        var com = t1 as IComparable<T>;
                        if (com != null)
                            return com.CompareTo(t2);
                        return 1;
                    };
                else if (type.GetInterface("IComparable") != null)
                    compare = delegate(T t1, T t2)
                    {
                        var com = t1 as IComparable;
                        if (com != null)
                            return com.CompareTo(t2);
                        return 1;
                    };
                else
                    throw new InvalidOperationException("未能比较数组中的两个元素");
            }

            if (allowSort)
            {
                if (hasCompareHander)
                    list.Sort((t1, t2) => compare(t1, t2));
                else
                    list.Sort();
                for (int i = 1; i < list.Count; i++)
                {
                    if (compare(list[i - 1], list[i]) == 0)
                        return true;
                }
                return false;
            }
            for (int i = 0; i < list.Count; i++)
            {
                T t1 = list[i];
                for (int j = 0; j < list.Count; j++)
                {
                    if (compare(t1, list[j]) == 0)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 加码(Microsoft.JScript.dll)
        /// </summary>
        public static string JSEscape(string source)
        {
            string text1 = source;
            string text2 = "0123456789ABCDEF";
            int num1 = text1.Length;
            StringBuilder builder1 = new StringBuilder(num1 * 2);
            int num3 = -1;
            while (++num3 < num1)
            {
                char ch1 = text1[num3];
                int num2 = ch1;
                if ((((0x41 > num2) || (num2 > 90)) && ((0x61 > num2) || (num2 > 0x7a))) && ((0x30 > num2) || (num2 > 0x39)))
                {
                    switch (ch1)
                    {
                        case '@':
                        case '*':
                        case '_':
                        case '+':
                        case '-':
                        case '.':
                        case '/':
                            goto Label_0125;
                    }
                    builder1.Append('%');
                    if (num2 < 0x100)
                    {
                        builder1.Append(text2[num2 / 0x10]);
                        ch1 = text2[num2 % 0x10];
                    }
                    else
                    {
                        builder1.Append('u');
                        builder1.Append(text2[(num2 >> 12) % 0x10]);
                        builder1.Append(text2[(num2 >> 8) % 0x10]);
                        builder1.Append(text2[(num2 >> 4) % 0x10]);
                        ch1 = text2[num2 % 0x10];
                    }
                }
            Label_0125:
                builder1.Append(ch1);
            }
            return builder1.ToString();
        }

        /// <summary>
        /// 解码(Microsoft.JScript.dll)
        /// </summary>
        public static string JSUnescape(string source)
        {
            string text1 = source;
            int num1 = text1.Length;
            StringBuilder builder1 = new StringBuilder(num1);
            int num6 = -1;
            while (++num6 < num1)
            {
                char ch1 = text1[num6];
                if (ch1 == '%')
                {
                    int num2;
                    int num3;
                    int num4;
                    int num5;
                    if (((((num6 + 5) < num1) && (text1[num6 + 1] == 'u')) && (((num2 = HexToInt(text1[num6 + 2])) != -1) &&
                        ((num3 = HexToInt(text1[num6 + 3])) != -1))) && (((num4 = HexToInt(text1[num6 + 4])) != -1) &&
                        ((num5 = HexToInt(text1[num6 + 5])) != -1)))
                    {
                        ch1 = (char)((ushort)((((num2 << 12) + (num3 << 8)) + (num4 << 4)) + num5));
                        num6 += 5;
                    }
                    else if ((((num6 + 2) < num1) && ((num2 = HexToInt(text1[num6 + 1])) != -1)) && ((num3 = HexToInt(text1[num6 + 2])) != -1))
                    {
                        ch1 = (char)((ushort)((num2 << 4) + num3));
                        num6 += 2;
                    }
                }
                builder1.Append(ch1);
            }
            return builder1.ToString();
        }

        /// <summary>
        /// 16进制转整数, 转换失败返回 -1 
        /// </summary>
        public static int HexToInt(char c)
        {
            if ((c >= '0') && (c <= '9'))
            {
                return (c - '0');
            }
            if ((c >= 'A') && (c <= 'F'))
            {
                return (('\n' + c) - 'A');
            }
            if ((c >= 'a') && (c <= 'f'))
            {
                return (('\n' + c) - 'a');
            }
            return -1;
        }
        /// <summary>
        /// 16进制转整数, 转换失败返回 -1 
        /// </summary>
        public static int HexToInt(string source)
        {
            if (source.IsNullOrEmpty())
                return -1;
            int v = 0;
            for (int i = source.Length - 1; i >= 0; i--)
            {
                int v2 = HexToInt(source[i]);
                if (v2 == -1)
                    return -1;
                v += v2 * Convert.ToInt32(Math.Pow(16, i));
            }
            return v;
        }

        /// <summary>
        /// 将字符串分割成数组
        /// </summary>
        /// <param name="source">要分割的字符串</param>
        /// <param name="separator">分割符</param>
        /// <returns></returns>
        public static string[] Split(string source, char separator)
        {
            return Split(source, separator, false);
        }

        /// <summary>
        /// 将字符串分割成数组
        /// </summary>
        /// <param name="source">要分割的字符串</param>
        /// <param name="separator">分割符</param>
        /// <param name="distinct">是否需要去掉重复项</param>
        /// <returns></returns>
        public static string[] Split(string source, char separator, bool distinct)
        {
            string[] splits = source.Split(separator);
            var tmpList = new List<string>(splits.Length);
            foreach (var s in splits)
            {
                if (string.IsNullOrEmpty(s)) continue;
                if (distinct && tmpList.Contains(s)) continue;
                tmpList.Add(s);
            }
            return tmpList.ToArray();
        }
    }
}
