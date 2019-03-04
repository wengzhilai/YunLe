using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    public class TModule:MODULE
    {
        [Display(Name = "角色")]
        public string AllRoleIdArrStr { get; set; }
    }
}
