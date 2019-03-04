using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    public class DataDown : DATA_DOWN
    {
        [Display(Name = "数据源")]
        public string FormServerStr { get; set; }
        [Display(Name = "目标数据库")]
        public string ToServerStr { get; set; }
    }
}
