
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProInterface.Models
{
    /// <summary>
    /// ��֯�ṹ
    /// </summary>
    public class DISTRICT
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        public int ID { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [Range(0, 2147483647)]
        [Display(Name = "����")]
        public Nullable<int> PARENT_ID { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [Required]
        [StringLength(255)]
        [Display(Name = "����")]
        public string NAME { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [Required]
        [StringLength(255)]
        [Display(Name = "����")]
        public string CODE { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "����")]
        public Int16? IN_USE { get; set; }
        /// <summary>
        /// �㼶
        /// </summary>
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "�㼶")]
        public int LEVEL_ID { get; set; }
        /// <summary>
        /// ·��
        /// </summary>
        [StringLength(200)]
        [Display(Name = "·��")]
        public string ID_PATH { get; set; }
        /// <summary>
        /// REGION
        /// </summary>
        [Required]
        [StringLength(10)]
        [Display(Name = "REGION")]
        public string REGION { get; set; }


    }
}
