using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterface.Models
{
    public class YlOrder : YL_ORDER
    {
        public YlOrder()
        {
            AllFlow = new List<YL_ORDER_FLOW>();
            AllFiles = new List<FILES>();
        }
        public YlCar Car { get; set; }
        public YlClient Client { get; set; }

        public IList<YL_ORDER_FLOW> AllFlow { get; set; }
        public string AllFilesStr { get; set; }
        public IList<FILES> AllFiles { get; set; }

        [Display(Name = "客户姓名")]
        public string ClientName { get; set; }
        [Display(Name = "客户电话")]
        public string ClientPhone { get; set; }
        [Display(Name = "车牌号")]
        public string CarPlateNumber { get; set; }
        public string iconURL { get; set; }
        public string idNoUrl { get; set; }
        public string idNoUrl1 { get; set; }
        public string DrivingPicUrl { get; set; }
        public string DrivingPicUrl1 { get; set; }
        public string driverPicUrl { get; set; }
        public string driverPicUrl1 { get; set; }
        /// <summary>
        /// 下步操作按钮
        /// </summary>
        public IList<string> NextButton { get; set; }

        public string LastStatus { get; set; }

        public string AddressStr { get; set; }

        public int TaskId { get; set; }
        public int TaskFlowId { get; set; }
        /// <summary>
        /// 处理人
        /// </summary>
        public string UserIdArrStr { get; set; }
    }
}
