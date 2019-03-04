using ProInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface
{
    public interface IChart
    {
        FCSingleSeries ChartGetByQueryCodeSingle(string loginKey, ref ErrorInfo err, string queryCode, IList<QueryPara> paraList,ref string reSql);
        FCMultiSeries ChartGetByQueryCodeMulti(string loginKey, ref ErrorInfo err, string queryCode, IList<QueryPara> paraList,ref string reSql);
    }
}
