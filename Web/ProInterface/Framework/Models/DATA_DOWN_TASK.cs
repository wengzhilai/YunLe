
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 下载任务
    /// </summary>
    public class DATA_DOWN_TASK
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public Int64 ID { get; set; }
        /// <summary>
        /// EVENT_ID
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "EVENT_ID")]
        public Nullable<Int64> EVENT_ID { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [StringLength(50)]
        [Display(Name = "表名")]
        public string NAME { get; set; }
        /// <summary>
        /// 查询SQL语句
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "查询SQL语句")]
        public string SELECT_SCRIPT { get; set; }
        /// <summary>
        /// 查询数据库
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "查询数据库")]
        public Nullable<Int64> SELECT_DB_SERVER { get; set; }
        /// <summary>
        /// 查询服务器
        /// </summary>
        [StringLength(200)]
        [Display(Name = "查询服务器")]
        public string SELECT_SERVER { get; set; }
        /// <summary>
        /// 查询用户名
        /// </summary>
        [StringLength(32)]
        [Display(Name = "查询用户名")]
        public string SELECT_UID { get; set; }
        /// <summary>
        /// 查询密码
        /// </summary>
        [StringLength(32)]
        [Display(Name = "查询密码")]
        public string SELECT_PWD { get; set; }
        /// <summary>
        /// 目标数据库
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "目标数据库")]
        public Nullable<Int64> TO_DB_SERVER { get; set; }
        /// <summary>
        /// 目标服务器
        /// </summary>
        [StringLength(200)]
        [Display(Name = "目标服务器")]
        public string TO_SERVER { get; set; }
        /// <summary>
        /// 目标用户名
        /// </summary>
        [StringLength(200)]
        [Display(Name = "目标用户名")]
        public string TO_UID { get; set; }
        /// <summary>
        /// 目标密码
        /// </summary>
        [StringLength(200)]
        [Display(Name = "目标密码")]
        public string TO_PWD { get; set; }
        /// <summary>
        /// 建表脚本
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "建表脚本")]
        public string CREATE_SCRIPT { get; set; }
        /// <summary>
        /// 重试次数
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "重试次数")]
        public Nullable<int> ERROR_NUM { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [StringLength(10)]
        [Display(Name = "状态")]
        public string STATUS { get; set; }
        /// <summary>
        /// 每页大小
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "每页大小")]
        public Nullable<Int64> PAGE_SIZE { get; set; }
        /// <summary>
        /// 分隔符
        /// </summary>
        [StringLength(20)]
        [Display(Name = "分隔符")]
        public string SPLIT_STR { get; set; }
        /// <summary>
        /// 完成页数
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "完成页数")]
        public Nullable<Int64> SUCC_PAGE_NUM { get; set; }
        /// <summary>
        /// 是否取消
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "是否取消")]
        public Nullable<Int16> IS_CANCEL { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "排序")]
        public Nullable<Int64> ORDER_NUM { get; set; }
        /// <summary>
        /// 记录日志
        /// </summary>
        [Display(Name = "记录日志")]
        public string LOG { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        [StringLength(1024)]
        [Display(Name = "错误信息")]
        public string ERR_MSG { get; set; }
        /// <summary>
        /// 源记录数
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "源记录数")]
        public Nullable<Int64> ALL_NUM { get; set; }
        /// <summary>
        /// 上月记录数
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "上月记录数")]
        public Nullable<Int64> LAST_MONTH_NUM { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        [StringLength(500)]
        [Display(Name = "文件名")]
        public string FILE_NAME { get; set; }
        /// <summary>
        /// 刷新时间
        /// </summary>
        [Display(Name = "刷新时间")]
        public Nullable<DateTime> REFRESH_TIME { get; set; }
        /// <summary>
        /// 保存路径
        /// </summary>
        [StringLength(300)]
        [Display(Name = "保存路径")]
        public string TO_PATH { get; set; }
        /// <summary>
        /// EVENT_TYPE
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "EVENT_TYPE")]
        public Nullable<Int64> EVENT_TYPE { get; set; }
        /// <summary>
        /// 成功后执行脚本
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "成功后执行脚本")]
        public string SUCC_SCRIPT { get; set; }

    }
}
