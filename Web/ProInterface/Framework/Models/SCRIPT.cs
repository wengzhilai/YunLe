
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 口径脚本
    /// </summary>
    public class SCRIPT
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public Int64 ID { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "代码")]
        public string CODE { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [StringLength(255)]
        [Display(Name = "名称")]
        public string NAME { get; set; }
        /// <summary>
        /// 任务脚本
        /// </summary>
        [Required]
        [Display(Name = "任务脚本")]
        [Description(@"
在字符串中调用方法：@{方法名(参数)}

设置当前数据库
void setnowdb(string dbNickName=null)

写日志
void log(string message, string sql_text="""",Int16 logType=0)

执行SQL命令
int exec(string sql)

返回执行返回值
object execute_scalar(string sql)

获取当前日期格式""yyyyMMdd""
<param name=""value"">增加的天数，可为负数</param>
string day(int value=0)

获取当前月份格式""yyyyMM""
<param name=""value"">增加的月份，可为负数</param>
string month(int value)

获取当前月份格式""yyyy""
<param name=""value"">增加的月份，可为负数</param>
string year(int value)

当月最后一天
<param name=""value"">当前月增加月份</param>
string last_day(int value=0)

导出表，生成的文件路径：~/UpFiles/DownDB/{tableName}.zip
<param name=""sql"">sql语句</param>
<param name=""tableName"">导入的文件名</param>
<returns>文件路径</returns>
string down_db_to_file(string sql, string tableName)

下载表到数据库
<param name=""sql"">查询语句</param>
<param name=""tableName"">生成的表名</param>
<param name=""dbNickName"">导入的数据库</param>
<param name=""isCreat"">0(默认值)表示不创建表;1:表示自动创建表(在导数据之前，要删除已经存在表);</param>
<param name=""pageSize"">设置每页导出的大小。值越大速度越快(对服务器压力大，内容要足够大) 默认值：1000000</param>
void down_db_to_db(string sql, string tableName, string dbNickName, int isCreatTable = 1,int pageSize = 1000000)

快速删除表
<param name=""tables"">可以是多表，以逗号分开</param>
void drop_table(string tables)

快速清空资料表
<param name=""tables"">可以是多表，以逗号分开</param>
void truncate_table(string tables)

判断表是否存在
<param name=""tables"">可以是多表，以逗号分开</param>
bool is_table_exists(string tables)

添加等待发送的内容
<returns>主键</returns>
string SmsAdd(string phoneNo, string content)

扫描表XX_SMS_SEND并发送短信
<returns>发送条数</returns>
int SmsSend()

返回表的自增加ID
GetSeqID(""表名"")

")]
        public string BODY_TEXT { get; set; }
        /// <summary>
        /// 脚本哈希值
        /// </summary>
        [Required]
        [StringLength(255)]
        [Display(Name = "脚本哈希值")]
        public string BODY_HASH { get; set; }
        /// <summary>
        /// 时间表达式
        /// </summary>
        [StringLength(30)]
        [Display(Name = "时间表达式")]
        public string RUN_WHEN { get; set; }
        /// <summary>
        /// 脚本参数
        /// </summary>
        [StringLength(255)]
        [Display(Name = "脚本参数")]
        public string RUN_ARGS { get; set; }

        /// <summary>
        /// 运行时间
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "运行时间")]
        [Description(@"
支持的格式：
-1d
1d
-1m
1m
20140101
")]
        public string RUN_DATA { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [StringLength(10)]
        [Display(Name = "状态")]
        public string STATUS { get; set; }
        /// <summary>
        /// 禁用原因
        /// </summary>
        [StringLength(50)]
        [Display(Name = "禁用原因")]
        public string DISABLE_REASON { get; set; }
        /// <summary>
        /// 服务标识
        /// </summary>
        [StringLength(50)]
        [Display(Name = "服务标识")]
        public string SERVICE_FLAG { get; set; }
        /// <summary>
        /// REGION
        /// </summary>
        [StringLength(10)]
        [Display(Name = "REGION")]
        public string REGION { get; set; }

        /// <summary>
        /// 是否是组
        /// </summary>
        [Display(Name = "是否是组")]
        public short IS_GROUP { get; set; }
    }
}


