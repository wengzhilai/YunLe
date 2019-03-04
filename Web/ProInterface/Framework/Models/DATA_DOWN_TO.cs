
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 替换值
    /// </summary>
    public class DATA_DOWN_TO
    {

            /// <summary>
            /// DB_SERVER_ID
            /// </summary>
            [Required]
            [Range(0, 2147483647)]
            [Display(Name = "DB_SERVER_ID")]
            public Int64 DB_SERVER_ID { get; set; }
            /// <summary>
            /// DATA_DOWN_ID
            /// </summary>
            [Required]
            [Range(0, 2147483647)]
            [Display(Name = "DATA_DOWN_ID")]
            public Int64 DATA_DOWN_ID { get; set; }
            /// <summary>
            /// 服务名
            /// </summary>
            [StringLength(32)]
            [Display(Name = "服务名")]
            public string TO_SERVER_NAME { get; set; }
            /// <summary>
            /// 替换内容
            /// </summary>
            [StringLength(100)]
            [Display(Name = "替换内容")]
            public string REPLACE_STR { get; set; }
            
       
    }
}
