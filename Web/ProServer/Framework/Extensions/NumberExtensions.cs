using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class NumberExtensions
    {
        /// <summary>
        /// 是否在2个数字中间， 包括相等的情况
        /// </summary>
        public static bool CompareBetween(this Int32 source, int low, int high)
        {
            return source >= low && source <= high;
        }
    }
}
