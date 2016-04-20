using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Text;

namespace Perenthia.Web
{
    public static class AppHelper
    {
        private const string LeftArrow = "&#x25C0;";
        private const string RightArrow = "&#x25BA;";
        private const string UpArrow = "&#x25B2;";
        private const string DownArrow = "&#x25BC;";
        private const string NonBreakingSpace = "&nbsp;";

        public static string Pager(HtmlHelper helper, string actionName, string controller, int pageNumber, int pageSize, int totalRowCount)
        {
            StringBuilder sb = new StringBuilder();

            int pageIndex = (pageNumber / pageSize) - 1;
            int pageCount = totalRowCount / pageSize;
            if (pageCount == 0) pageCount = 1;

            if (pageCount > 1)
            {
                int prevPageIndex = pageIndex - 1;
                if (prevPageIndex < 0) prevPageIndex = 0;
                sb.Append(helper.ActionLink(LeftArrow, actionName, controller, new { start = prevPageIndex }, null));
                sb.Append(NonBreakingSpace);

                for (int i = 0; i < pageCount; i++)
                {
                    string linkNumber = (i + 1).ToString();
                    if ((i + 1) == pageNumber)
                    {
                        sb.Append(linkNumber);
                    }
                    else
                    {
                        sb.Append(helper.ActionLink(linkNumber, actionName, controller, new { start = i }, null));
                    }
                    sb.Append(NonBreakingSpace);
                }

                int nextPageIndex = pageIndex + 1;
                if (nextPageIndex > 0 && nextPageIndex <= (pageCount - 1))
                {
                    sb.Append(NonBreakingSpace);
                    sb.Append(helper.ActionLink(RightArrow, actionName, controller, new { start = nextPageIndex }, null));
                }
            }
            else
            {
                sb.Append("[ 1 ]");
            }

            return sb.ToString();
        }

        public static string ReverseSortDirection(object direction)
        {
            if (direction != null)
            {
                return direction.ToString().ToUpper() == "ASC" ? "DESC" : "ASC";
            }
            return "ASC";
        }

        public static string AppendSortDirectionArrow(string linkName, string sortExpression, string sortDirection)
        {
            if (linkName.ToLower() == sortExpression.ToLower())
            {
                if (sortDirection.ToLower() == "desc")
                {
                    linkName = String.Concat(linkName, " ", DownArrow);
                }
                else
                {
					linkName = String.Concat(linkName, " ", UpArrow);
                }
            }
            return linkName;
        }

        public static string StatBar(string title, string color, int value, int max)
        {
            // Stat
            double valPercentage = Convert.ToDouble((double)value * (100.0 / (double)max));
            double dispPercentage = Convert.ToDouble(valPercentage * (245.0 / 100.0));

            //        <div style="padding-top:4px" title="Health">
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<div class=\"statbar\" title=\"{0} {1}/{2}\">", title, value, max);

            //            <div style="margin-right:2px;width:178px;height:5px;background-color:#808080;margin-top:1px;">
            sb.AppendFormat("<div class=\"statbar-container\" style=\"width:{0}px;\">", dispPercentage);

            //                <div style="width:178px;height:5px;float:left;background-color:#FF0000;"></div>
            sb.AppendFormat("<div class=\"statbar-content\" style=\"background-color:{0};\"></div>", color);

            //            </div>
            sb.Append("</div>");

            //        </div>
            sb.Append("</div>");

            return sb.ToString();
        }
    }
}
