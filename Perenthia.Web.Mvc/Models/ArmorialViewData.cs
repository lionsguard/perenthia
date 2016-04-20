using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Text;
using System.Web.Routing;

using Lionsguard.Mvc;

namespace Perenthia.Web.Models
{
	public class ArmorialViewData
	{
		public string Query { get; set; }
		public string SearchType { get; set; }
		public string SortBy { get; set; }
		public string SortDirection { get; set; }
		public ArmorialTab Tab { get; set; }
		public int PageIndex { get; set; }
		public int MaxRows { get; set; }
		public int TotalRowCount { get; set; }	
		public IEnumerable Results { get; set; }

		public string ActionLink(HtmlHelper htmlHelper, string linkText)
		{
			return this.ActionLink(htmlHelper, linkText, this.ToRouteValues());
		}

		public string ActionLink(HtmlHelper htmlHelper, string linkText, object routeValues)
		{
			return this.ActionLink(htmlHelper, linkText, new RouteValueDictionary(routeValues));
		}

		public string ActionLink(HtmlHelper htmlHelper, string linkText, RouteValueDictionary routeValues)
		{
			UrlHelper url = new UrlHelper(htmlHelper.ViewContext.RequestContext);
			return String.Format("<a href=\"{0}\">{1}</a>", url.Action("Search", "Armorial", routeValues), linkText);
		}

		public string ActionSortLink(HtmlHelper htmlHelper, string linkText)
		{
			RouteValueDictionary values = this.ToRouteValues();
			string arrow = String.Empty;
			if (this.SortBy.ToLower() == linkText.ToLower())
			{
				string sortDir = (this.SortDirection == "asc" ? "desc" : "asc");

				if (sortDir == "desc")
				{
					arrow = String.Concat(HtmlExtensions.NonBreakingSpace, HtmlExtensions.UpArrow);
				}
				else
				{
					arrow = String.Concat(HtmlExtensions.NonBreakingSpace, HtmlExtensions.DownArrow);
				}
				values["sortDirection"] = sortDir;
				linkText = String.Concat(linkText, arrow);
			}
			else
			{
				values["sortBy"] = linkText;
				values["sortDirection"] = "asc";
			}
			return this.ActionLink(htmlHelper, linkText, values);
		}

		public RouteValueDictionary ToRouteValues()
		{
			return new RouteValueDictionary( 
				new
				{
					action = "Search",
					controller = "Armorial",
					searchType = this.SearchType,
					sortBy = this.SortBy,
					sortDirection = this.SortDirection,
					tab = this.Tab.ToString(),
					start = this.PageIndex,
					query = this.Query
				});
		}
	}
}
