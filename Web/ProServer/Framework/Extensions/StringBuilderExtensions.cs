using ProServer.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Text
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// 查找字符串
        /// </summary>
        public static int IndexOf(this StringBuilder source, string value)
        {
            return source.ToString().IndexOf(value);
        }
        /// <summary>
        /// 查找字符串
        /// </summary>
        public static int IndexOf(this StringBuilder source, string value, int startIndex)
        {
            return source.ToString().IndexOf(value, startIndex);
        }
        /// <summary>
        /// 查找字符串
        /// </summary>
        public static int IndexOf(this StringBuilder source, string value, StringComparison comparisonType)
        {
            return source.ToString().IndexOf(value, comparisonType);
        }
        /// <summary>
        /// 查找字符串
        /// </summary>
        public static int IndexOf(this StringBuilder source, string value, int startIndex, StringComparison comparisonType)
        {
            return source.ToString().IndexOf(value, startIndex, comparisonType);
        }
        /// <summary>
        /// 查找字符串
        /// </summary>
        public static int IndexOf(this StringBuilder source, string value, int startIndex, int count, StringComparison comparisonType)
        {
            return source.ToString().IndexOf(value, startIndex, count, comparisonType);
        }
        /// <summary>
        /// 替换字符
        /// </summary>
        public static StringBuilder Replace(this StringBuilder source, string oldValue, string newValue, bool ignoreCase)
        {
            return new StringBuilder(StringHelper.Replace(source.ToString(), oldValue, newValue, ignoreCase));
        }
        /// <summary>
        /// 生成引用字符串，使用<c>'</c>作为引用字符
        /// </summary>
        public static StringBuilder AppendQuoteSingle(this StringBuilder source, string value)
        {
            return source.Append(value.ToQuoteSingle());
        }
    }
}
