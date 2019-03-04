using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LINQExtensions;
using System.Linq.Expressions;
using System.IO;
using ProInterface;
using ProInterface.Models;
using System.Web.Mvc;


namespace ProServer
{
    public partial class Service : IConfig
    {

        public IList<System.Web.Mvc.SelectListItem> ConfigGetSelectListItem(string code)
        {
            using (DBEntities db = new DBEntities())
            {
                return db.YL_CONFIG.Where(x => x.CODE == code).Select(x => new SelectListItem { Text = x.VALUE, Value = x.VALUE }).ToList();
            }
        }
    }
}
