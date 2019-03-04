using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace System.Data
{
    /// <summary>
    /// 数据行扩展类
    /// </summary>
    public static class DataRowExtensions
    {
        private static int IndexOfColumnThrowException(DataRow row, string columnName)
        {
            int index = row.Table.Columns.IndexOf(columnName);
            if (index < 0)
                throw new Exception(columnName + " 不存在");
            return index;
        }

        public static int IndexOfColumn(this DataRow row, string columnName)
        {
            return row.Table.Columns.IndexOf(columnName);
        }

        public static string AsString(this DataRow row, string columnName, string defaultValue)
        {
            int index = IndexOfColumnThrowException(row, columnName);
            return AsString(row, index, defaultValue);
        }
        public static string AsString(this DataRow row, string columnName)
        {
            int index = IndexOfColumnThrowException(row, columnName);
            return AsString(row, index);
        }
        public static string AsString(this DataRow row, int index, string defaultValue)
        {
            return row.IsNull(index) ? defaultValue : row[index].ToString();
        }
        public static string AsString(this DataRow row, int index)
        {
            return row[index].ToString();
        }

        public static Int32 AsInt32(this DataRow row, string columnName, Int32 defaultValue)
        {
            int index = IndexOfColumnThrowException(row, columnName);
            return AsInt32(row, index, defaultValue);
        }
        public static Int32 AsInt32(this DataRow row, string columnName)
        {
            int index = IndexOfColumnThrowException(row, columnName);
            return AsInt32(row, index);
        }
        public static Int32 AsInt32(this DataRow row, int index, Int32 defaultValue)
        {
            return row.IsNull(index) ? defaultValue : Convert.ToInt32(row[index]);
        }
        public static Int32 AsInt32(this DataRow row, int index)
        {
            return Convert.ToInt32(row[index]);
        }

        public static Int64 AsInt64(this DataRow row, string columnName, Int64 defaultValue)
        {
            int index = IndexOfColumnThrowException(row, columnName);
            return AsInt64(row, index, defaultValue);
        }
        public static Int64 AsInt64(this DataRow row, string columnName)
        {
            int index = IndexOfColumnThrowException(row, columnName);
            return AsInt64(row, index);
        }
        public static Int64 AsInt64(this DataRow row, int index, Int64 defaultValue)
        {
            return row.IsNull(index) ? defaultValue : Convert.ToInt64(row[index]);
        }
        public static Int64 AsInt64(this DataRow row, int index)
        {
            return Convert.ToInt64(row[index]);
        }

        public static double AsDouble(this DataRow row, string columnName, double defaultValue)
        {
            int index = IndexOfColumnThrowException(row, columnName);
            return AsDouble(row, index, defaultValue);
        }
        public static double AsDouble(this DataRow row, string columnName)
        {
            int index = IndexOfColumnThrowException(row, columnName);
            return AsDouble(row, index);
        }
        public static double AsDouble(this DataRow row, int index, double defaultValue)
        {
            return row.IsNull(index) ? defaultValue : Convert.ToDouble(row[index]);
        }
        public static double AsDouble(this DataRow row, int index)
        {
            return Convert.ToDouble(row[index]);
        }

        public static decimal AsDecimal(this DataRow row, string columnName, decimal defaultValue)
        {
            int index = IndexOfColumnThrowException(row, columnName);
            return AsDecimal(row, index, defaultValue);
        }
        public static decimal AsDecimal(this DataRow row, string columnName)
        {
            int index = IndexOfColumnThrowException(row, columnName);
            return AsDecimal(row, index);
        }
        public static decimal AsDecimal(this DataRow row, int index, decimal defaultValue)
        {
            return row.IsNull(index) ? defaultValue : Convert.ToDecimal(row[index]);
        }
        public static decimal AsDecimal(this DataRow row, int index)
        {
            return Convert.ToDecimal(row[index]);
        }

        public static bool AsBool(this DataRow row, string columnName, bool defaultValue)
        {
            int index = IndexOfColumnThrowException(row, columnName);
            return AsBool(row, index, defaultValue);
        }
        public static bool AsBool(this DataRow row, string columnName)
        {
            int index = IndexOfColumnThrowException(row, columnName);
            return AsBool(row, index);
        }
        public static bool AsBool(this DataRow row, int index, bool defaultValue)
        {
            return row.IsNull(index) ? defaultValue : Convert.ToInt32(row[index]) != 0;
        }
        public static bool AsBool(this DataRow row, int index)
        {
            return Convert.ToInt32(row[index]) != 0;
        }

        public static DateTime AsDateTime(this DataRow row, string columnName, DateTime defaultValue)
        {
            int index = IndexOfColumnThrowException(row, columnName);
            return AsDateTime(row, index, defaultValue);
        }
        public static DateTime AsDateTime(this DataRow row, string columnName)
        {
            int index = IndexOfColumnThrowException(row, columnName);
            return AsDateTime(row, index);
        }
        public static DateTime AsDateTime(this DataRow row, int index, DateTime defaultValue)
        {
            return row.IsNull(index) ? defaultValue : Convert.ToDateTime(row[index]);
        }
        public static DateTime AsDateTime(this DataRow row, int index)
        {
            return Convert.ToDateTime(row[index]);
        }

        public static T AsEnum<T>(this DataRow row, string columnName, T defaultValue)
        {
            int index = IndexOfColumnThrowException(row, columnName);
            return AsEnum<T>(row, index, defaultValue);
        }
        public static T AsEnum<T>(this DataRow row, string columnName)
        {
            int index = IndexOfColumnThrowException(row, columnName);
            return AsEnum<T>(row, index);
        }

        public static T AsEnum<T>(this DataRow row, int index, T defaultValue)
        {
            if (row.IsNull(index))
                return defaultValue;
            object o = Convert.ToInt32(row[index]);
            return (T)o;
        }
        public static T AsEnum<T>(this DataRow row, int index)
        {
            object o = Convert.ToInt32(row[index]);
            return (T)o;
        }
    }
}
