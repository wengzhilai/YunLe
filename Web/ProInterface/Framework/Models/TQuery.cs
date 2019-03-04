using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    /// <summary>
    /// Query扩展
    /// </summary>
    public class TQuery : QUERY
    {
        public TQuery()
        {
            QueryCfg = new List<QueryCfg>();
            QueryRowBtnList = new List<QueryRowBtn>();
        }

        public QueryCfg QueryCfgEnt { get; set; }
        /// <summary>
        /// 无权限的模块
        /// </summary>
        public string NoAuthority { get; set; }
        /// <summary>
        /// 查问配置
        /// </summary>
        public IList<QueryCfg> QueryCfg
        {
            get
            {
                IList<QueryCfg> reList = JSON.EncodeToEntity<IList<QueryCfg>>(QUERY_CFG_JSON);
                return reList;
            }
            set
            {
                QUERY_CFG_JSON = JSON.DecodeToStr(value);
            }
        }

        public IList<QueryRowBtn> QueryRowBtnList
        {
            get
            {
                try
                {
                    IList<QueryRowBtn> reList = JSON.EncodeToEntity<IList<QueryRowBtn>>(ROWS_BTN);
                    return reList;
                }
                catch {
                    return new List<QueryRowBtn>();
                }
            }
            set
            {
                QUERY_CFG_JSON = JSON.DecodeToStr(value);
            }
        }

    }
    /// <summary>
    /// 参数
    /// </summary>
    public class QueryPara
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParaName { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public string Value { get; set; }
    }
    /// <summary>
    /// 配置
    /// </summary>
    public class QueryCfg
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        [Display(Name = "字段名称")]
        public string FieldName { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        [Display(Name = "别名")]
        public string Alias { get; set; }
        /// <summary>
        /// 是否可搜索
        /// </summary>
        [Display(Name = "是否可搜索")]
        public bool CanSearch { get; set; }

        /// <summary>
        /// 过滤控件类型
        /// </summary>
        [Display(Name = "过滤控件类型")]
        public string SearchType { get; set; }

        /// <summary>
        /// 过滤控件脚本
        /// </summary>
        [Display(Name = "过滤控件脚本")]
        public string SearchScript { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        [Display(Name = "是否显示")]
        public bool Show { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        [Display(Name = "宽度")]
        public int Width { get; set; }
        /// <summary>
        /// 可排序
        /// </summary>
        [Display(Name = "可排序")]
        public bool Sortable { get; set; }

        /// <summary>
        /// 变量
        /// </summary>
        [Display(Name = "变量")]
        public bool? IsVariable { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        [Display(Name = "字段类型")]
        public string FieldType { get; set; }
        /// <summary>
        /// 格式化
        /// </summary>
        [Display(Name = "格式化")]
        public string Format { get; set; }
    }


    public class QueryRowBtn
    {
        /// <summary>
        /// 按钮名
        /// </summary>
        [Display(Name = "按钮名")]
        public string Name { get; set; }

        /// <summary>
        /// 样式
        /// </summary>
        [Display(Name = "样式")]
        public string IconCls { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [Display(Name = "地址")]
        public string Url{ get; set; }

        /// <summary>
        /// 对话框模式
        /// </summary>
        [Display(Name = "对话框模式")]
        public string DialogMode { get; set; }

        /// <summary>
        /// 对话框宽
        /// </summary>
        [Display(Name = "对话框宽")]
        public string DialogWidth { get; set; }

        /// <summary>
        /// 对话框宽
        /// </summary>
        [Display(Name = "对话框高")]
        public string DialogHeigth { get; set; }

        /// <summary>
        /// 显示条件
        /// </summary>
        [Display(Name = "显示条件")]
        public IList<QueryRowBtnShowCondition> ShowCondition { get; set; }

        /// <summary>
        /// 传值参数
        /// </summary>
        [Display(Name = "参数")]
        public IList<QueryRowBtnParameter> Parameter { get; set; }
    }
    /// <summary>
    /// 显示条件
    /// </summary>
    public class QueryRowBtnShowCondition
    {

        [Display(Name = "对象字段")]
        public string ObjFiled { get; set; }
        [Display(Name = "操作符")]
        public string OpType { get; set; }
        [Display(Name = "值")]
        public string Value { get; set; }
        [Display(Name = "字段类型")]
        public string FieldType { get; set; }
        [Display(Name = "字段名称")]
        public string FieldName { get; set; }
    }

    /// <summary>
    /// 显示条件
    /// </summary>
    public class QueryRowBtnParameter
    {
        [Display(Name = "参数")]
        public string Para { get; set; }
        [Display(Name = "对象值")]
        public string ObjValue { get; set; }
    }

    /// <summary>
    /// 字段类型
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// 字符串
        /// </summary>
        [Description("字符串")]
        字符器 = 0,
        /// <summary>
        /// 数字
        /// </summary>
        [Description("数字")]
        数字 = 1,
        /// <summary>
        /// 日期
        /// </summary>
        [Description("日期")]
        日期 = 2
    }

    /// <summary>
    /// 用于绑定DataGrid数据
    /// </summary>
    public class DataGridDataJson {
        public int total { get; set; }
        public DataTable rows { get; set; }
    }
}
