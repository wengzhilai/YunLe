using System.Web.Mvc;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Collections;

namespace System.Web.Mvc.Html
{

    /// <summary>  
    /// 自定义控件   
    /// </summary>  
    public static class LabelGenderExtensions
    {

        public static MvcHtmlString RadioBoxList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> selectList)
        {
            return RadioBoxList(helper, name, selectList, new { });
        }

        public static MvcHtmlString RadioBoxList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return MakeBoxList(helper, name, selectList, htmlAttributes, "radio");
        }
        public static MvcHtmlString RadioBoxListFor<T, TValue>(this HtmlHelper<T> htmlHelper, Expression<Func<T, TValue>> expression, IEnumerable<SelectListItem> selectList) where T : new()
        {
            return MakeBoxListFor(htmlHelper, expression, selectList, new { }, "radio");
        }
        public static MvcHtmlString RadioBoxListFor<T, TValue>(this HtmlHelper<T> htmlHelper, Expression<Func<T, TValue>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes) where T : new()
        {
            return MakeBoxListFor(htmlHelper, expression, selectList, htmlAttributes, "radio");
        }


        public static MvcHtmlString CheckBoxList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> selectList)
        {
            return CheckBoxList(helper, name, selectList, new { });
        }
        public static MvcHtmlString CheckBoxList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return MakeBoxList(helper, name, selectList, htmlAttributes, "checkbox");
        }
        public static MvcHtmlString CheckBoxListFor<T, TValue>(this HtmlHelper<T> htmlHelper, Expression<Func<T, TValue>> expression, IEnumerable<SelectListItem> selectList) where T : new()
        {
            return MakeBoxListFor(htmlHelper, expression, selectList, new { }, "checkbox");
        }
        public static MvcHtmlString CheckBoxListFor<T, TValue>(this HtmlHelper<T> htmlHelper, Expression<Func<T, TValue>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes) where T : new()
        {
            return MakeBoxListFor(htmlHelper, expression, selectList, htmlAttributes, "checkbox");
        }

        public static MvcHtmlString MakeBoxList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes, string type)
        {
            if (selectList == null)
                return MvcHtmlString.Create("");
            IDictionary<string, object> HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            int columnNum = 0;
            //不生成隐藏的输入框
            bool noInput = false;
            if (HtmlAttributes.Keys.SingleOrDefault(x => x == "ColumnNum") != null)
            {
                try
                {
                    columnNum = Convert.ToInt32(HtmlAttributes["ColumnNum"]);
                    HtmlAttributes.Remove("ColumnNum");
                }
                catch { }
            }
            if (HtmlAttributes.Keys.SingleOrDefault(x => x == "NoInput") != null)
            {
                try
                {
                    noInput = Convert.ToBoolean(HtmlAttributes["NoInput"]);
                    HtmlAttributes.Remove("NoInput");
                }
                catch { }
            }
            HashSet<string> set = new HashSet<string>();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var tmp in selectList.Where(x => x.Selected == true).ToList())
            {
                set.Add(tmp.Value);
            }
            int[] byteLeng = new int[selectList.Count()];
            int k = 0;
            int maxLen = 0;
            foreach (SelectListItem item in selectList)
            {
                var nowLeng = System.Text.Encoding.Default.GetBytes(item.Text).Length;

                if (nowLeng > maxLen) maxLen = nowLeng;
                byteLeng.SetValue(nowLeng, k);
                //  byteLeng = item.Text.Length;
                item.Selected = (item.Value != null) ? set.Contains(item.Value) : set.Contains(item.Text);
                list.Add(item);
                k++;
            }
            selectList = list;

            if (type == "checkbox")
            {
                HtmlAttributes.Add("type", "checkbox");
            }
            else
            {
                HtmlAttributes.Add("type", "radio");
            }

            HtmlAttributes.Add("style", "margin:0 0 0 10px; line-height:1em;vertical-align:-3px;border:none;");

            StringBuilder stringBuilder = new StringBuilder();
            string js = @"
<script type=""text/javascript"">

$(function(){{
    BoxListSetV('{0}');
    $('input[name ={0}_JSITEM]').click(function(){{
      BoxListSetV('{0}',this);
    }});
}})
</script>
";
            stringBuilder.Append(string.Format(js, name));
            if (!noInput)
            {
                stringBuilder.Append(string.Format("<input id=\"{0}\" name=\"{0}\" type=\"hidden\" value=\"\" />", name));
            }
            if (columnNum != 0)
            {
                stringBuilder.AppendLine("<table><tr>");
            }
            for (int i = 0; i < selectList.Count(); i++)
            {
                SelectListItem selectItem = selectList.ToList()[i];
                IDictionary<string, object> newHtmlAttributes = HtmlAttributes.DeepCopy();
                newHtmlAttributes.Add("id", name + "_" + i);
                newHtmlAttributes.Add("name", name + "_JSITEM");
                newHtmlAttributes.Add("value", selectItem.Value);
                //newHtmlAttributes.Add("onclick", "BoxListSetV('" + name + "',this)");
                if (selectItem.Selected)
                {
                    newHtmlAttributes.Add("checked", "checked");
                }
                TagBuilder tagBuilder = new TagBuilder("input");
                tagBuilder.MergeAttributes<string, object>(newHtmlAttributes);
                string inputAllHtml = tagBuilder.ToString(TagRenderMode.SelfClosing);
                if (columnNum == 0)
                {
                    stringBuilder.AppendFormat(@"<label style='width:" + (maxLen * 9 + 30) + "px;'> {0}  {1}</label>",
                       inputAllHtml, selectItem.Text.Trim());
                }
                else
                {
                    stringBuilder.AppendFormat(@"<td><label> {0}  {1}</label><td>",
                                          inputAllHtml, selectItem.Text.Trim());
                    if ((i + 1) % columnNum == 0)
                    {
                        stringBuilder.AppendLine("</tr><tr>");
                    }
                }
            }
            if (columnNum != 0)
            {
                stringBuilder.AppendLine("<tr></table>");
            }
            return MvcHtmlString.Create(stringBuilder.ToString());
        }

        public static MvcHtmlString MakeBoxListFor<T, TValue>(this HtmlHelper<T> htmlHelper, Expression<Func<T, TValue>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes, string type) where T : new()
        {
            T obj = htmlHelper.ViewData.Model;
            if (obj == null) obj = new T();
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<T, TValue>(expression, htmlHelper.ViewData);

            //metadata.DisplayName//字段描述
            string name = ExpressionHelper.GetExpressionText(expression);//从Lambda表达式中获取模型对应属性的名称

            PropertyInfo proInfo = obj.GetType().GetProperty(name);//得到该类的所有公共属性
            Type proType = proInfo.PropertyType;//属性的类型
            object propertyValue = proInfo.GetValue(obj, null);


            if (propertyValue != null && propertyValue != DBNull.Value)
            {
                foreach (var t in selectList)
                {
                    t.Selected = false;
                }
                foreach (var t in propertyValue.ToString().Split(','))
                {
                    if (!string.IsNullOrEmpty(t.ToLower()))
                    {
                        var selectItem = selectList.SingleOrDefault(x => x.Value == t.ToLower());
                        if (selectItem != null) selectItem.Selected = true;
                    }
                }
            }
            return MakeBoxList(htmlHelper, name, selectList, htmlAttributes, type);
        }
        private static IDictionary<string, object> DeepCopy(this IDictionary<string, object> ht)
        {
            Dictionary<string, object> _ht = new Dictionary<string, object>();

            foreach (var p in ht)
            {
                _ht.Add(p.Key, p.Value);
            }
            return _ht;
        }

        public static MvcHtmlString SpanFor<T, TValue>(this HtmlHelper<T> htmlHelper, Expression<Func<T, TValue>> expression, object htmlAttributes = null) where T : new()
        {
            T obj = htmlHelper.ViewData.Model;
            if (obj == null) obj = new T();
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<T, TValue>(expression, htmlHelper.ViewData);

            //metadata.DisplayName//字段描述
            string name = ExpressionHelper.GetExpressionText(expression);//从Lambda表达式中获取模型对应属性的名称

            PropertyInfo proInfo = obj.GetType().GetProperty(name);//得到该类的所有公共属性
            Type proType = proInfo.PropertyType;//属性的类型
            object propertyValue = proInfo.GetValue(obj, null);
            string v = "";
            if (propertyValue != null && propertyValue != DBNull.Value)
            {
                v = propertyValue.ToString();
            }
            return Span(htmlHelper, name, v, htmlAttributes);
        }

        public static MvcHtmlString SpanNameFor<T, TValue>(this HtmlHelper<T> htmlHelper, Expression<Func<T, TValue>> expression, object htmlAttributes = null) where T : new()
        {

            T obj = htmlHelper.ViewData.Model;
            if (obj == null) obj = new T();
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<T, TValue>(expression, htmlHelper.ViewData);
            //metadata.DisplayName//字段描述
            string name = ExpressionHelper.GetExpressionText(expression);//从Lambda表达式中获取模型对应属性的名称

            TagBuilder tagBuilder = new TagBuilder("span");
            tagBuilder.InnerHtml = metadata.DisplayName;
            tagBuilder.Attributes.Add("for", name);
            IDictionary<string, object> HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            foreach (var t in HtmlAttributes)
            {
                tagBuilder.Attributes.Add(t.Key, t.Value.ToString());
            }

            foreach (var item in htmlHelper.ViewData.ModelMetadata.Properties)
            {
                if (item.PropertyName == name && item.IsRequired == true)
                {
                    tagBuilder.InnerHtml += "<span style=\"padding:0;color:red;margin:0;height:1em;\">*</span>";
                }
            }
            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        public static MvcHtmlString Span(this HtmlHelper html, string id, string value = "", object htmlAttributes = null)
        {
            TagBuilder tagBuilder = new TagBuilder("span");
            tagBuilder.InnerHtml = value;
            if (!string.IsNullOrEmpty(id))
            {
                tagBuilder.Attributes.Add("id", id);
            }
            IDictionary<string, object> HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            foreach (var t in HtmlAttributes)
            {
                tagBuilder.Attributes.Add(t.Key, t.Value.ToString());
            }
            return MvcHtmlString.Create(tagBuilder.ToString());

        }


        #region LabelFor


        public static MvcHtmlString LabelFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, string>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            var name = memberExpression.Member.Name;

            foreach (var item in htmlHelper.ViewData.ModelMetadata.Properties)
            {
                if (item.PropertyName == name && item.IsRequired == true)
                {
                    return new MvcHtmlString(htmlHelper.LabelFor<TModel, string>(expression).ToString() + "<span class=\"required\">*</span>");
                }
            }

            return htmlHelper.LabelFor<TModel, string>(expression);
        }

        public static MvcHtmlString LabelFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, int>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            var name = memberExpression.Member.Name;

            foreach (var item in htmlHelper.ViewData.ModelMetadata.Properties)
            {
                if (item.PropertyName == name && item.IsRequired == true)
                {
                    return new MvcHtmlString(htmlHelper.LabelFor<TModel, int>(expression).ToString() + "<span class=\"required\">*</span>");
                }
            }

            return htmlHelper.LabelFor<TModel, int>(expression);
        }

        #endregion


        #region DropDownList
        public static MvcHtmlString MyDropDownListFor<T, TValue>(this HtmlHelper<T> htmlHelper, Expression<Func<T, TValue>> expression, IEnumerable<SelectListItem> selectList = null, object htmlAttributes = null) where T : new()
        {
            T obj = htmlHelper.ViewData.Model;
            if (obj == null) obj = new T();
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<T, TValue>(expression, htmlHelper.ViewData);

            //metadata.DisplayName//字段描述
            string name = ExpressionHelper.GetExpressionText(expression);//从Lambda表达式中获取模型对应属性的名称

            PropertyInfo proInfo = obj.GetType().GetProperty(name);//得到该类的所有公共属性
            Type proType = proInfo.PropertyType;//属性的类型
            object propertyValue = proInfo.GetValue(obj, null);

            if (selectList != null)
            {
                if (propertyValue != null && propertyValue != DBNull.Value)
                {
                    foreach (var t in selectList)
                    {
                        t.Selected = false;
                    }
                    foreach (var t in propertyValue.ToString().Split(','))
                    {
                        if (!string.IsNullOrEmpty(t.ToLower()))
                        {
                            var selectItem = selectList.SingleOrDefault(x => x.Value == t.ToLower());
                            if (selectItem != null) selectItem.Selected = true;
                        }
                    }
                }
            }

            return MyDropDownList(htmlHelper, name, selectList, htmlAttributes);
        }



        public static MvcHtmlString MyDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList = null, object htmlAttributes = null)
        {

            TagBuilder tagBuilder = new TagBuilder("select");
            tagBuilder.Attributes.Add("id", name);
            tagBuilder.Attributes.Add("name", name);
            IDictionary<string, object> HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            foreach (var t in HtmlAttributes)
            {
                tagBuilder.Attributes.Add(t.Key, t.Value.ToString());
            }
            if (selectList != null)
            {
                foreach (var item in selectList)
                {
                    if (item.Selected)
                    {
                        tagBuilder.InnerHtml += string.Format("<option value=\"{0}\" selected=\"selected\">{1}</option>\r\n", item.Value, item.Text);
                    }
                    else
                    {
                        tagBuilder.InnerHtml += string.Format("<option value=\"{0}\">{1}</option>\r\n", item.Value, item.Text);
                    }
                }
            }
            return MvcHtmlString.Create(tagBuilder.ToString());
        }


        public static MvcHtmlString MyDateTimeFor<T, TValue>(this HtmlHelper<T> htmlHelper, Expression<Func<T, TValue>> expression, object htmlAttributes = null) where T : new()
        {

            T obj = htmlHelper.ViewData.Model;
            if (obj == null) obj = new T();
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<T, TValue>(expression, htmlHelper.ViewData);
            //metadata.DisplayName//字段描述
            string name = ExpressionHelper.GetExpressionText(expression);//从Lambda表达式中获取模型对应属性的名称

            PropertyInfo proInfo = obj.GetType().GetProperty(name);//得到该类的所有公共属性
            Type proType = proInfo.PropertyType;//属性的类型
            object propertyValue = proInfo.GetValue(obj, null);


            //<input class="form-control form_datetime valid" data-val="true" data-val-date="字段 开考时间 必须是日期。" data-val-required="开考时间 字段是必需的。" id="START_TIME" name="START_TIME" type="text" value="2014/12/4 0:00:00">
            TagBuilder tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes.Add("id", name);
            tagBuilder.Attributes.Add("name", name);
            tagBuilder.Attributes.Add("type", "text");
            if (propertyValue != null)
            {
                try
                {
                    if (propertyValue != null)
                    {
                        DateTime dt = Convert.ToDateTime(propertyValue);
                        if (dt < new DateTime(1901, 1, 1))
                        {
                            dt = DateTime.Now;
                        }
                        propertyValue = dt.ToString("yyyy-MM-dd HH:mm");
                    }
                }
                catch { }
                tagBuilder.Attributes.Add("value", propertyValue.ToString());
            }
            IDictionary<string, object> HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            foreach (var t in HtmlAttributes)
            {
                tagBuilder.Attributes.Add(t.Key, t.Value.ToString());
            }
            if (tagBuilder.Attributes.ContainsKey("class"))
            {
                tagBuilder.Attributes["class"] = tagBuilder.Attributes["class"] + " valid";
            }
            else
            {
                tagBuilder.Attributes.Add("class", "valid");
            }

            foreach (var item in htmlHelper.ViewData.ModelMetadata.Properties)
            {
                if (item.PropertyName == name)
                {
                    if (item.IsRequired == true)
                    {
                        tagBuilder.Attributes.Add("data-val-required", string.Format("{0} 字段是必需的。", item.DisplayName));
                    }
                    tagBuilder.Attributes.Add("data-val-date", "字段 开考时间 必须是日期。");
                }

            }
            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        #endregion
    }

}