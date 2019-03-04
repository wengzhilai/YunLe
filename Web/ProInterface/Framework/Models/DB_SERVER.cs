
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 数据库服务器
    /// </summary>
    public class DB_SERVER
    {

            /// <summary>
            /// ID
            /// </summary>
            [Required]
            [Range(0, 2147483647)]
            [Display(Name = "ID")]
            public Int64 ID { get; set; }
            /// <summary>
            /// 类型
            /// </summary>
            [Range(0, 2147483647)]
            [Display(Name = "类型")]
            public Int64 DB_TYPE_ID { get; set; }
            /// <summary>
            /// 类别
            /// </summary>
            [Required]
            [StringLength(10)]
            [Display(Name = "类别")]
            public string TYPE { get; set; }
            /// <summary>
            /// IP
            /// </summary>
            [Required]
            [StringLength(20)]
            [Display(Name = "IP")]
            public string IP { get; set; }
            /// <summary>
            /// 端口
            /// </summary>
            [Required]
            [Range(0, 2147483647)]
            [Display(Name = "端口")]
            public Int64 PORT { get; set; }
            /// <summary>
            /// 数据库名称
            /// </summary>
            [StringLength(20)]
            [Display(Name = "数据库名称")]
            public string DBNAME { get; set; }
            /// <summary>
            /// 用户名
            /// </summary>
            [Required]
            [StringLength(20)]
            [Display(Name = "用户名")]
            public string UID { get; set; }
            /// <summary>
            /// 密码
            /// </summary>
            [Required]
            [StringLength(32)]
            [Display(Name = "密码")]
            public string PASSWORD { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            [StringLength(500)]
            [Display(Name = "备注")]
            public string REMARK { get; set; }
            /// <summary>
            /// 数据库连接
            /// </summary>
            [StringLength(200)]
            [Display(Name = "数据库连接")]
            public string DB_LINK { get; set; }
            /// <summary>
            /// 别名
            /// </summary>
            [StringLength(32)]
            [Display(Name = "别名")]
            public string NICKNAME { get; set; }
            /// <summary>
            /// FTP存放路径（月）
            /// </summary>
            [StringLength(300)]
            [Display(Name = "FTP存放路径（月）")]
            public string TO_PATH_M { get; set; }
            /// <summary>
            /// FTP存放路径（日）
            /// </summary>
            [StringLength(300)]
            [Display(Name = "FTP存放路径（日）")]
            public string TO_PATH_D { get; set; }
            
       
    }
}
