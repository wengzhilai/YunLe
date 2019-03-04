using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface.Models.Api
{
    public class ApiRequesPageBean : ApiRequesEntityBean
    {
        public ApiRequesPageBean()
        {
            attachParams = new List<ApiKeyValueBean>();
            searchKey = new List<ApiKeyValueBean>();
            orderBy = new List<ApiKeyValueBean>();
        }
        public int currentPage { get; set; }
        public int pageSize { get; set; }
        public IList<ApiKeyValueBean> attachParams { get; set; }
        public IList<ApiKeyValueBean> searchKey { get; set; }
        public IList<ApiKeyValueBean> orderBy { get; set; }
    }
}
