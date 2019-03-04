using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ProInterface.Models
{
    public class TNodeFlow
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Display(Name = "ID")]
        public int ID { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Display(Name = "流程ID")]
        public int? FlowID { get; set; }
    }
}
