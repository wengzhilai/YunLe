using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterface.Models
{
    public class YlOrderRescue: YL_ORDER_RESCUE
    {
        [Display(Name = "维修站名称")]
        public string GarageName { get; set; }
        [Display(Name = "维修站")]
        public YlGarage Garage { get; set; }

    }
}
