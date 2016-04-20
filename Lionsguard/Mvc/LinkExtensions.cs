using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web;

namespace Lionsguard.Mvc
{
	public static class LinkExtensions
	{
		public static string ActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, string title)
		{
			return helper.ActionLink(linkText, actionName, controllerName, title, new RouteValueDictionary(), new RouteValueDictionary());
		}
		public static string ActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, string title, 
			object routeValues, object htmlAttributes)
		{
			return helper.ActionLink(linkText, actionName, controllerName, title, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlAttributes));
		}
		public static string ActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, string title,
			IDictionary<string, object> routeValues, IDictionary<string, object> htmlAttributes)
		{
			RouteValueDictionary attr = new RouteValueDictionary(htmlAttributes);
			attr["title"] = title;
			return helper.ActionLink(linkText, actionName, controllerName, new RouteValueDictionary(routeValues), attr);
		}
	}
}
