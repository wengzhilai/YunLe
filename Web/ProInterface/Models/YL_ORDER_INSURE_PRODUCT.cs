
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class YL_ORDER_INSURE_PRODUCT
    {

        /// <summary>
        /// INSURER_ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "INSURER_ID")]
        public int INSURER_ID { get; set; }
        /// <summary>
        /// PRODUCT_ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "PRODUCT_ID")]
        public int PRODUCT_ID { get; set; }
        /// <summary>
        /// INSURE_ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "INSURE_ID")]
        public int INSURE_ID { get; set; }
        /// <summary>
        /// 最大赔率
        /// </summary>
        [StringLength(100)]
        [Display(Name = "最大赔率")]
        public string MAX_PAY { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "金额")]
        public Nullable<decimal> COST { get; set; }


    }
}
