using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface.Models.Api
{
    public class ApiPagingDataBean
    {
        public ApiPagingDataBean()
        {
            para = new List<ApiKeyValueBean>();
        }
        public IList<ApiKeyValueBean> para { get; set; }
        public int currentPage { get; set; }
        public int pageSize { get; set; }
        public int totalPage { get; set; }
        public int totalCount { get; set; }
        public object data { get; set; }
    }

    public class ApiPagingDataBean<T>
    {
        public ApiPagingDataBean()
        {
            para = new List<ApiKeyValueBean>();
        }
        public IList<ApiKeyValueBean> para { get; set; }
        public int currentPage { get; set; }
        public int pageSize { get; set; }
        public int totalPage { get; set; }
        public int totalCount { get; set; }
        public IList<T> data { get; set; }
    }
}
