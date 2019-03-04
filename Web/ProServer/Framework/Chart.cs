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
using System.Data.Entity.Validation;
using System.Web.Mvc;
using System.Data;


namespace ProServer
{
    public partial class Service : IChart
    {
        public FCMultiSeries ChartGetByQueryCodeMulti(string loginKey, ref ErrorInfo err, string queryCode, IList<QueryPara> paraList, ref string reSql)
        {
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时，请重新登录";
            }
            FCMultiSeries fc = new FCMultiSeries();
            using (DBEntities db = new DBEntities())
            {
                var query = db.YL_QUERY.SingleOrDefault(x => x.CODE == queryCode);
                TQuery ent = Fun.ClassToCopy<YL_QUERY, TQuery>(query);

                string whereStr = "";
                DataTable reDt = QueryDataTable(loginKey, ref err, queryCode, null, whereStr, paraList, ref reSql);
                FCMultiSeries_Categories categories = new FCMultiSeries_Categories();
                for (int i = 1; i < reDt.Columns.Count; i++)
                {
                    categories.category.Add(new FCMultiSeries_Category_label { label = reDt.Columns[i].Caption });
                }
                fc.categories.Add(categories);
                for (int i = 0; i < reDt.Rows.Count; i++)
                {
                    FCMultiSeries_Dataset dataset = new FCMultiSeries_Dataset();
                    dataset.seriesname = reDt.Rows[i][0].ToString();
                    for (int x = 1; x < reDt.Columns.Count; x++)
                    {
                        dataset.data.Add(new FCMultiSeries_Dataset_Value { value = reDt.Rows[i][x].ToString() });
                    }
                    fc.dataset.Add(dataset);
                }
            }
            return fc;
        }

        public FCSingleSeries ChartGetByQueryCodeSingle(string loginKey, ref ErrorInfo err, string queryCode, IList<QueryPara> paraList, ref string reSql)
        {
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时，请重新登录";
            }
            FCSingleSeries fc = new FCSingleSeries();
            using (DBEntities db = new DBEntities())
            {
                var query = db.YL_QUERY.SingleOrDefault(x => x.CODE == queryCode);
                TQuery ent = Fun.ClassToCopy<YL_QUERY, TQuery>(query);

                string whereStr = "";
                DataTable reDt = QueryDataTable(loginKey, ref err, queryCode, null, whereStr, paraList, ref reSql);
                for (int i = 0; i < reDt.Rows.Count; i++)
                {
                    fc.data.Add(new FCSingleSeries_data { label = reDt.Rows[i][0].ToString(), value = reDt.Rows[i][1].ToString() });
                }
            }
            return fc;
        }
    }
}
