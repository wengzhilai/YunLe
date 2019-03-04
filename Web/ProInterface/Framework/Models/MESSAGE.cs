
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 提醒内容
    /// </summary>
    public class MESSAGE
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// 类型ID
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "类型ID")]
        public Nullable<int> MESSAGE_TYPE_ID { get; set; }
        /// <summary>
        /// 关联主键
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "关联主键")]
        public Nullable<int> KEY { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(100)]
        [Display(Name = "标题")]
        public string TITLE { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [StringLength(500)]
        [Display(Name = "内容")]
        public string CONTENT { get; set; }
        /// <summary>
        /// 生成时间
        /// </summary>
        [Display(Name = "生成时间")]
        public DateTime CREATE_TIME { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        [StringLength(50)]
        [Display(Name = "创建用户")]
        public string CREATE_USERNAME { get; set; }
        /// <summary>
        /// 创建用户ID
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "创建用户ID")]
        public Nullable<int> CREATE_USERID { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [StringLength(10)]
        [Display(Name = "状态")]
        public string STATUS { get; set; }
        /// <summary>
        /// 推送方式
        /// </summary>
        [StringLength(10)]
        [Display(Name = "推送方式")]
        [Description(@"
智能推送：先APP推送,再WEB推送，如都失败，则30分钟后由短信推送
短信推送：立即给用户发送短信
APP推送：只由用户APP获取，前提是用户APP在运行
WEB推送：只由用户WEB获取，前提是用户WEB在线
        ")]
        public string PUSH_TYPE { get; set; }
        /// <summary>
        /// 归属地
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "归属地")]
        public Nullable<int> DISTRICT_ID { get; set; }
        /// <summary>
        /// 选择角色
        /// </summary>
        [StringLength(500)]
        [Display(Name = "选择角色")]
        public string ALL_ROLE_ID { get; set; }


    }
}
