﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface.Models.Api
{
    public class ApiRequesEntityBean
    {
        public ApiRequesEntityBean()
        {
            para = new List<ApiKeyValueBean>();
        }
        public IList<ApiKeyValueBean> para { get; set; }
        public int userId { get; set; }
        public string authToken { get; set; }
        public int id { get; set; }
    }
}
