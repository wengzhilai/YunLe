using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    public class QuartzRunStatus
    {
        public bool IsRun { get; set; }
        /// <summary>
        /// jobq名称
        /// </summary>
        public string JobName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public DateTime StatusTime { get; set; }
    }
}
