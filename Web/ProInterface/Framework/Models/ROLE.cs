
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 系统角色
    /// </summary>
    public class ROLE
    {

            /// <summary>
            /// ID
            /// </summary>
            [Required]
            [Display(Name = "ID")]
            public int ID { get; set; }
            /// <summary>
            /// 角色名
            /// </summary>
            [StringLength(80)]
            [Display(Name = "角色名")]
            public string NAME { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            [StringLength(255)]
            [Display(Name = "备注")]
            public string REMARK { get; set; }
            /// <summary>
            /// 类型
            /// </summary>
            [Display(Name = "类型")]
            public Nullable<int> TYPE { get; set; }
            
       
    }
}
