using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    /// <summary>
    /// User扩展
    /// </summary>
    public class TUser : USER
    {
        public TUser() {
            CanChangeDistrict = new List<DISTRICT>();
            MyModule = new List<MODULE>();
            AllRole = new List<ROLE>();
        }

        /// <summary>
        /// 所有角色
        /// </summary>
        public IList<ROLE> AllRole { get; set; }

        /// <summary>
        /// 所有角色串
        /// </summary>
        public string RoleAllName { get; set; }

        /// <summary>
        /// 区县名称
        /// </summary>
        public string DistrictName { get; set; }
        /// <summary>
        /// 所有角色串
        /// </summary>
        public string RoleAllID { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [StringLength(20)]
        [Display(Name = "电话")]
        public string PHONE_NO { get; set; }
        /// <summary>
        /// 邮件
        /// </summary>
        [StringLength(255)]
        [Display(Name = "邮件")]
        public string EMAIL_ADDR { get; set; }

        /// <summary>
        /// 锁定原因
        /// </summary>
        [StringLength(255)]
        [Display(Name = "锁定原因")]
        public string LOCKED_REASON { get; set; }

        public string PassWord { get; set; }        public string ParentMenu { get; set; }
        public string ChirldMenu { get; set; }
        /// <summary>
        /// 可切换的地市
        /// </summary>
        public IList<DISTRICT> CanChangeDistrict { get; set; }
        /// <summary>
        /// 获取该用户所有模块
        /// </summary>
        [Display(Name = "用户模块")]
        public IList<ProInterface.Models.MODULE> MyModule { get; set; }

        /// <summary>
        /// 管辖区域
        /// </summary>
        [StringLength(255)]
        [Display(Name = "管辖区域")]
        [Description(@"
如果配置了此项。归属地将无效
")]
        public string UserDistrict { get; set; }

        public string OpenId { get; set; }
         [Display(Name = "对应小区")]
        public string UserVillage { get; set; }
    }
}
