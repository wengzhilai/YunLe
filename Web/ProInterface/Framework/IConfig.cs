using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ProInterface
{
    public interface IConfig : IZ_Config
    {
        IList<SelectListItem> ConfigGetSelectListItem(string code);
    }
}
