using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace VSS.Portal.Helpers
{
    public class BreadcrumbItem
    {
        public string Action { get; set; }
        public string Label { get; set; }

        public BreadcrumbItem() { }

        public BreadcrumbItem(string action, string label)
        {
            this.Action = action;
            this.Label = label;
        }
    }

    public static partial class BreadcrumbUtils
    {
        public static MvcHtmlString Breadcrumb(this HtmlHelper helper, List<BreadcrumbItem> items, string currentActionLabel)
        {
            bool hasItems = items != null && items.Count > 0;

            var div = new TagBuilder("div");
            div.AddCssClass("breadcrumbs");

            if (hasItems)
            {
                foreach (var item in items)
                {
                    var itemLink = new TagBuilder("a");
                    itemLink.Attributes.Add("href", item.Action);
                    itemLink.Attributes.Add("title", item.Label);
                    itemLink.InnerHtml = item.Label;

                    div.InnerHtml += itemLink.ToString();

                    var spanArrow = new TagBuilder("span");
                    spanArrow.InnerHtml = "&nbsp;&gt;&nbsp;";
                    div.InnerHtml += spanArrow.ToString();
                }
            }

            div.InnerHtml += currentActionLabel;

            return MvcHtmlString.Create(div.ToString());
        } 
    }
}