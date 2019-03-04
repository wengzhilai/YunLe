using System;
using System.Linq;
using System.Text;
using System.Collections;
using ProServer.Helper;

namespace System.Collections.Generic
{
    public static class IListExtensions
    {
        /// <summary>
        /// 取元素个数，如果为null返回0
        /// </summary>
        public static int CountIf<T>(this IList<T> source)
        {
            if (source == null)
                return 0;
            return source.Count;
        }
        /// <summary>
        /// 搜索指定的对象，并返回整个 List 中第一个匹配项的索引。
        /// </summary>
        public static int IndexOf(this IList<string> source, string item, bool ignoreCase)
        {
            if (source == null)
                return -1;
            for (int i = 0; i < source.Count; i++)
                if (StringHelper.IsSame(source[i], item, ignoreCase))
                    return i;
            return -1;
        }
        /// <summary>
        /// 生成按 逗号 分隔的字符串
        /// </summary>
        public static string GetDelimitedText(this IList source)
        {
            return StringHelper.GetListDelimitedText(source);
        }
        /// <summary>
        /// 赋值按 逗号 分隔的字符串
        /// </summary>
        public static void SetDelimitedText(this IList source, string delimitedText)
        {
            StringHelper.SetListDelimitedText(source, delimitedText, false);
        }
        /// <summary>
        /// 判断列表有没有重复项
        /// </summary>
        /// <param name="allowSort">是否允许排序，排序后会影响到原来列表，但排序后会加快判断速度</param>
        /// <param name="compare">比较列表中任意两项的委托方法</param>
        public static bool HasRepeatItem<T>(this List<T> source, bool allowSort, Func<T, T, int> compare)
        {
            return StringHelper.HasRepeatItem(source, allowSort, compare);
        }
        /// <summary>
        /// 交换位置
        /// </summary>
        public static void ExchangeItem<T>(this IList<T> source, int index, int index2)
        {
            T temp = source[index];
            source[index] = source[index2];
            source[index] = temp;
        }
        /// <summary>
        /// 移动元素
        /// </summary>
        public static void MoveItem<T>(this IList<T> source, int from, int to)
        {
            if (from == to)
                return;
            if (from > to)
            {
                T temp = source[from];
                for (int i = from - 1; i >= to; i--)
                    source[i + 1] = source[i];
                source[to] = temp;
            }
            else
            {
                T temp = source[from];
                for (int i = to - 1; i >= from; i--)
                    source[i + 1] = source[i];
                source[to] = temp;
            }
        }
        /// <summary>
        /// 随机获取数组元素
        /// </summary>
        /// <param name="array">当前数组</param>
        /// <param name="num">需要的数组元素个数,小于等于数组大小</param>
        /// <returns>随机取到的数组</returns>
        public static ArrayList Random(this ArrayList array, int num)
        {
            ArrayList list = new ArrayList();

            int count = array.Count;
            if (num > count)
            {
                return list;
            }
            else
            {
                int m = num;
                int[] have = new int[m];
                for (int i = 0; i < num; i++)
                {
                    RandomNum(count, ref  have, i);
                }

                foreach (int k in have)
                {
                    list.Add(array[k]);
                }
                return list;
            }
        }
        private static void RandomNum(int count, ref int[] have, int index)
        {
            Random random = new Random();
            int n = random.Next(0, count);
            if (have.Contains(n) == true)
            {
                RandomNum(count, ref have, index);
            }
            else
            {
                have.SetValue(n, index);
            }
        }
    }
}
