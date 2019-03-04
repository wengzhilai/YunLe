using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    public class FCSingleSeries
    {
        public FCSingleSeries()
        {
            data = new List<FCSingleSeries_data>();
        }
        public string chart { get; set; }
        public IList<FCSingleSeries_data> data { get; set; }
    }
    public class FCSingleSeries_data {
        public string label { get; set; }
        public string value { get; set; }
    }
}
