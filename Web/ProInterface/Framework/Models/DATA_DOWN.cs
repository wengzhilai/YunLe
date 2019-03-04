
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 数据下载
    /// </summary>
    public class DATA_DOWN
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public Int64 ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(200)]
        [Display(Name = "名称")]
        public string NAME { get; set; }
        /// <summary>
        /// 建表类型
        /// </summary>
        [StringLength(10)]
        [Display(Name = "建表类型")]
        public string CREATE_TYPE { get; set; }
        /// <summary>
        /// 创建的表名
        /// </summary>
        [StringLength(50)]
        [Display(Name = "创建的表名")]
        [Description(@"
支持的参数有：
例子：DG_GROUP_@{day(0)};
@{day(0)} 当前的日期,生成的格式:yyyyMMdd
@{month(0)} 当前的月份,生成的格式:yyyyMM
@{years(0)} 当前的年份,生成的格式:yyyy

")]
        public string CREATE_TABLE_NAME { get; set; }
        /// <summary>
        /// 查询脚本
        /// </summary>
        [Display(Name = "查询脚本")]
        public string SELECT_SCRIPT { get; set; }
        /// <summary>
        /// 采集模式
        /// </summary>
        [StringLength(50)]
        [Display(Name = "采集模式")]
        public string DOWN_MODE { get; set; }
        /// <summary>
        /// 是否分表
        /// </summary>
        [StringLength(10)]
        [Display(Name = "是否分表")]
        public string IS_CARVED { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        [StringLength(10)]
        [Display(Name = "是否删除")]
        public string IS_DEL { get; set; }
        /// <summary>
        /// 建表脚本
        /// </summary>
        [Display(Name = "建表脚本")]
        public string CREATE_SCRIPT { get; set; }
        /// <summary>
        /// 字段
        /// </summary>
        [Display(Name = "字段")]
        public string FILED { get; set; }
        /// <summary>
        /// 页面大小
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "页面大小")]
        public Int64? PAGE_SIZE { get; set; }
        /// <summary>
        /// 下载类型
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "下载类型")]
        public Nullable<int> DOWN_TYPE { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        public Nullable<DateTime> UPDATE_TIME { get; set; }
        /// <summary>
        /// 中文表名
        /// </summary>
        [StringLength(50)]
        [Display(Name = "中文表名")]
        public string C_NAME { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(1000)]
        [Display(Name = "备注")]
        public string REMARK { get; set; }
        /// <summary>
        /// 分隔符
        /// </summary>
        [StringLength(5)]
        [Display(Name = "分隔符")]
        public string SPLIT_STR { get; set; }
        /// <summary>
        /// 成功后执行脚本
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "成功后执行脚本")]
        public string SUCC_SCRIPT { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [StringLength(10)]
        [Display(Name = "状态")]
        public string STATUS { get; set; }
        /// <summary>
        /// 时间表达式
        /// </summary>
        [StringLength(30)]
        [Display(Name = "时间表达式")]
        public string CRON_EXPRESSION { get; set; }

        /// <summary>
        /// 存放文件的位置
        /// </summary>
        [StringLength(100)]
        [Display(Name = "存放文件的位置")]
        [Description(@"
是指在linux服务器上的存贮下载文件的位置
支持的参数有：
例子：DG_GROUP_@{day(0)};
@{day(0)} 当前的日期,生成的格式:yyyyMMdd
@{month(0)} 当前的月份,生成的格式:yyyyMM
@{years(0)} 当前的年份,生成的格式:yyyy

")]
        public string TO_PATH { get; set; }
    }
}
