using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    public class FCMultiSeries
    {
        public FCMultiSeries()
        {
            categories = new List<FCMultiSeries_Categories>();
            dataset = new List<FCMultiSeries_Dataset>();
        }
        public string chart { get; set; }
        public IList<FCMultiSeries_Categories> categories { get; set; }
        public IList<FCMultiSeries_Dataset> dataset { get; set; }
    }
    public class FCMultiSeries_Categories {
        public FCMultiSeries_Categories()
        {
            category = new List<FCMultiSeries_Category_label>();
        }
        public IList<FCMultiSeries_Category_label> category { get; set; }
    }
    public class FCMultiSeries_Category_label
    {
        public string label { get; set; }
    }
    public class FCMultiSeries_Dataset_Value
    {
        public string value { get; set; }
    }

    public class FCMultiSeries_Dataset
    {
        public FCMultiSeries_Dataset()
        {
            data = new List<FCMultiSeries_Dataset_Value>();
        }
        public string seriesname { get; set; }
        public IList<FCMultiSeries_Dataset_Value> data { get; set; }
        
    }
}
