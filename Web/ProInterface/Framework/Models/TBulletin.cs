using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    public class TBulletin : BULLETIN
    {
        public TBulletin()
        {
            AllFiles = new List<FILES>();
            AllChildrenItem = new List<BulletinReview>();
        }
        /// <summary>
        /// 公告文件
        /// </summary>
        public IList<FILES> AllFiles { get; set; }

        public string AllFilesStr { get; set; }

        /// <summary>
        /// 归属区县
        /// </summary>
        public string DistrictName { get; set; }

        /// <summary>
        /// 可查看的角色
        /// </summary>
        [Display(Name = "可查看的角色")]
        public string AllRoleId { get; set; }


        [Display(Name = "评论区")]
        public IList<BulletinReview> AllChildrenItem { get; set; }

        public int reviewNum { get; set; }
        public string TypeName { get; set; }
        public string UserName { get; set; }
        
    }
}
