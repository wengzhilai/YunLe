
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 查询
    /// </summary>
    public class QUERY
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "名称")]
        public string NAME { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "代码")]
        public string CODE { get; set; }
        /// <summary>
        /// 自动加载
        /// </summary>
        [Required]
        [Display(Name = "自动加载")]
        public short AUTO_LOAD { get; set; }
        /// <summary>
        /// 页面大小
        /// </summary>
        [Required]
        [Display(Name = "页面大小")]
        public int PAGE_SIZE { get; set; }
        /// <summary>
        /// 复选框
        /// </summary>
        [Required]
        [Display(Name = "复选框")]
        public short SHOW_CHECKBOX { get; set; }

        /// <summary>
        /// 调试
        /// </summary>
        [Required]
        [Display(Name = "调试")]
        public short IS_DEBUG { get; set; }

        /// <summary>
        /// 查询语句
        /// </summary>
        [Display(Name = "查询语句")]
        [Description(@"
有变量传入:用@(变量名);用于替换URL中的参数

替换当前用户信息
@{USER_ID} 当前的用户ID
@{DISTRICT_ID} 当前的组织结构ID
@{DISTRICT_CODE} 当前的组织结构CODE
@{REGION} 当前的区域
@{ALL_ROLE} 当前用户的角色 格式：5,1,2 用于SQL的IN方法
@{ALL_REGION} 当前的组织结构的所有层级ID 用于SQL的IN方法
@{RULE_REGION} 当前的用户的管辖区域 用于SQL的IN方法
@{NOW_LEVEL_ID} 当前的组织结构的层级值1表示市，2表示县，3表示片区，4表示乡镇
@{day(0)} 当前的日期
@{month(0)} 当前的月份
@{year(0)} 当前的年份
@{last_day(0)} 月份最后一天


with TT as()临时表开始标识
-----WithStart-----
with TT as()临时表结束标识
-----WithEnd-----
")]
        public string QUERY_CONF { get; set; }
        /// <summary>
        /// 配置信息
        /// </summary>
        [Display(Name = "配置信息")]
        public string QUERY_CFG_JSON { get; set; }
        /// <summary>
        /// 传入参数
        /// </summary>
        [Display(Name = "传入参数")]
        public string IN_PARA_JSON { get; set; }
        /// <summary>
        /// JS脚本
        /// </summary>
        [Display(Name = "JS脚本")]
        public string JS_STR { get; set; }
        /// <summary>
        /// 行按钮
        /// </summary>
        [Display(Name = "行按钮")]
        [Description(@"

")]
        public string ROWS_BTN { get; set; }
        /// <summary>
        /// 表头按钮
        /// </summary>
        [Display(Name = "表头按钮")]
        [Description(@"
有取URL地址的参数，用换位符@(参数名)

")]
        public string HEARD_BTN { get; set; }
        /// <summary>
        /// 报表脚本
        /// </summary>
        [Display(Name = "报表脚本")]
        public string REPORT_SCRIPT { get; set; }

        /// <summary>
        /// 图表配置
        /// </summary>
        [Display(Name = "图表配置")]
        [Description(@"
SQL脚本必须包含【LABEL,VALUE】  如：SELECT  LABEL, VALUE FROM DUAL

基本格式:
{
    ""caption"": ""角色的用户占比图"",
    ""subCaption"": ""角色用户"",
    ""xAxisName"": ""角色名称"",
    ""yAxisName"": ""用户数"",
    ""baseFont"": ""宋体"",
    ""type"": ""line"",
    ""polar"": false,
    ""baseFontSize"": ""12""
}

type:column为柱图，line为线图, pie为饼图
polar为true，则可将图转成极地图 默认是： false.
themes为设置Highcharts的样式，样式有 dark-blue,dark-green,dark-unica,gray,grid-light,grid,sand-signika,skies
其它参数：

功能特性
animation是否动画显示数据，默认为1(True)
showNames是否显示横向坐标轴(x轴)标签名称
rotateNames是否旋转显示标签，默认为0(False):横向显示
showValues是否在图表显示对应的数据值，默认为1(True)
yAxisMinValue指定纵轴(y轴)最小值，数字
yAxisMaxValue 指定纵轴(y轴)最小值，数字
showLimits是否显示图表限值(y轴最大、最小值)，默认为1(True)

图表标题和轴名称 
caption图表主标题
subCaption图表副标题
xAxisName横向坐标轴(x轴)名称
yAxisName纵向坐标轴(y轴)名称

图表和画布的样式
bgColor图表背景色，6位16进制颜色值
canvasBgColor画布背景色，6位16进制颜色值
canvasBgAlpha画布透明度，[0-100]
canvasBorderColor画布边框颜色，6位16进制颜色值
canvasBorderThickness画布边框厚度，[0-100]
shadowAlpha投影透明度，[0-100]
showLegend是否显示系列名，默认为1(True)

字体属性
baseFont图表字体样式
baseFontSize图表字体大小
baseFontColor图表字体颜色，6位16进制颜色值
outCnvBaseFont图表画布以外的字体样式
outCnvBaseFontSize图表画布以外的字体大小
outCnvBaseFontColor图表画布以外的字体颜色，6位16进制颜色值

分区线和网格
numDivLines画布内部水平分区线条数，数字
divLineColor水平分区线颜色，6位16进制颜色值
divLineThickness水平分区线厚度，[1-5]
divLineAlpha水平分区线透明度，[0-100]
showAlternateHGridColor是否在横向网格带交替的颜色，默认为0(False)
alternateHGridColor横向网格带交替的颜色，6位16进制颜色值
alternateHGridAlpha横向网格带的透明度，[0-100]
showDivLineValues是否显示Div行的值，默认？？
numVDivLines画布内部垂直分区线条数，数字
vDivLineColor垂直分区线颜色，6位16进制颜色值
vDivLineThickness垂直分区线厚度，[1-5]
vDivLineAlpha垂直分区线透明度，[0-100]
showAlternateVGridColor是否在纵向网格带交替的颜色，默认为0(False)
alternateVGridColor纵向网格带交替的颜色，6位16进制颜色值
alternateVGridAlpha纵向网格带的透明度，[0-100]

数字格式
numberPrefix增加数字前缀
numberSuffix增加数字后缀% 为 '%25'
formatNumberScale是否格式化数字,默认为1(True),自动的给你的数字加上K（千）或M（百万）；若取0,则不加K或M
decimalPrecision指定小数位的位数，[0-10]例如：='0' 取整
divLineDecimalPrecision指定水平分区线的值小数位的位数，[0-10]
limitsDecimalPrecision指定y轴最大、最小值的小数位的位数，[0-10]
formatNumber逗号来分隔数字(千位，百万位),默认为1(True)；若取0,则不加分隔符
decimalSeparator指定小数分隔符,默认为'.'
thousandSeparator指定千分位分隔符,默认为','

Tool-tip/Hover标题
showhovercap是否显示悬停说明框，默认为1(True)
hoverCapBgColor悬停说明框背景色，6位16进制颜色值
hoverCapBorderColor悬停说明框边框颜色，6位16进制颜色值
hoverCapSepChar指定悬停说明框内值与值之间分隔符,默认为','

折线图的参数
lineThickness折线的厚度
anchorRadius折线节点半径，数字
anchorBgAlpha折线节点透明度，[0-100]
anchorBgColor折线节点填充颜色，6位16进制颜色值
anchorBorderColor折线节点边框颜色，6位16进制颜色值

Set标签使用的参数
value数据值
color颜色
link链接（本窗口打开[Url]，新窗口打开[n-Url]，调用JS函数[JavaScript:函数]）
name横向坐标轴标签名称 
")]
        public string CHARTS_CFG { get; set; }


        /// <summary>
        /// 图表类型
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "图表类型")]
        public string CHARTS_TYPE { get; set; }


        /// <summary>
        /// 统计范围
        /// </summary>
        [Required]
        [Display(Name = "统计范围")]
        [Description(@"如果选中，SQL语句里必须包括@{DISTRICT_ID}，筛选的节点将替换@{DISTRICT_ID}")]
        public short FILTR_LEVEL { get; set; }

        /// <summary>
        /// 过滤配置
        /// </summary>
        [Display(Name = "过滤配置")]
        [Description(@"定义要传入SQL的参数，在SQL代码@(名称);所有要参数过滤的控件，都必须要加class='filtrClass'")]
        public string FILTR_STR { get; set; }

        /// <summary>
        /// 查询库
        /// </summary>
        [Display(Name = "查询库")]
        public int? DB_SERVER_ID { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [Display(Name = "说明")]
        public string REMARK { get; set; }

        /// <summary>
        /// 最新时间
        /// </summary>
        [StringLength(50)]
        [Display(Name = "最新时间")]
        public string NEW_DATA { get; set; }
    }
}
