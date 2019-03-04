using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    public class TRole:ROLE
    {
        public TRole()
        {
            RoleConfigs = new List<ROLE_CONFIG>();
        }
        /// <summary>
        /// 所有模块的ID
        /// </summary>
        public string ModuleAllStr { get; set; }

        [Display(Name = "角色的参数配置")]
        public IList<ROLE_CONFIG> RoleConfigs { get; set; }
        [Display(Name = "角色的参数配置")]
        public string RoleConfigsStr { get; set; }
    }

    
}
