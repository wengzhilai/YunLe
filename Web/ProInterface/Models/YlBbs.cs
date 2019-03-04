using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface.Models.Api
{
    public class Ylbbs:YL_BBS
    {
        public Ylbbs()
        {
            AllChildrenItem = new List<Ylbbs>();
            AllFiles = new List<FILES>();
        }
        public string iconURL { get; set; }
        public int goodNum { get; set; }
        public int reviewNum { get; set; }
        public string addUserName { get; set; }
        public string CaseTypeName { get; set; }
        public string userRole { get; set; }
        public IList<string> goodUserName { get; set; }
        public IList<Ylbbs> AllChildrenItem { get; set; }
        public IList<FILES> AllFiles { get; set; }
        public string userPhone { get; set; }
        public string distictName { get; set; }
    }
}
