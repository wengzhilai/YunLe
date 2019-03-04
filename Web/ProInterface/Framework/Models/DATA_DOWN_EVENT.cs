
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 下表事件
    /// </summary>
    public class DATA_DOWN_EVENT
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public Int64 ID { get; set; }
        /// <summary>
        /// DATA_DOWN_ID
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "DATA_DOWN_ID")]
        public Nullable<Int64> DATA_DOWN_ID { get; set; }
        /// <summary>
        /// 目录表名
        /// </summary>
        [StringLength(100)]
        [Display(Name = "目录表名")]
        public string TARGET_NAME { get; set; }
        /// <summary>
        /// 上月记录数
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "上月记录数")]
        public Nullable<Int64> LAST_MONTH_NUM { get; set; }
        /// <summary>
        /// 源记录数
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "源记录数")]
        public Nullable<Int64> ALL_NUM { get; set; }
        /// <summary>
        /// 入库记录数
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "入库记录数")]
        public Nullable<Int64> SUCC_NUM { get; set; }
        /// <summary>
        /// 所属月份
        /// </summary>
        [StringLength(8)]
        [Display(Name = "所属月份")]
        public string BELONG_MONTH { get; set; }
        /// <summary>
        /// 采集启动时间
        /// </summary>
        [Display(Name = "采集启动时间")]
        public Nullable<DateTime> START_TIME { get; set; }
        /// <summary>
        /// 采集完成时间
        /// </summary>
        [Display(Name = "采集完成时间")]
        public Nullable<DateTime> END_TIME { get; set; }
        /// <summary>
        /// 启动人
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "启动人")]
        public Nullable<Int64> USER_ID { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        [StringLength(10)]
        [Display(Name = "路径")]
        public string PATH { get; set; }


    }
}
