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
	public static class HtmlExtensions
	{
		public const string LeftArrow = "&#x25C4;";
		public const string RightArrow = "&#x25BA;";
		public const string UpArrow = "&#x25B2;";
		public const string DownArrow = "&#x25BC;";
		public const string NonBreakingSpace = "&nbsp;";

		#region SiteUrl
		public static string SiteUrl(this HtmlHelper helper)
		{
			var uri = helper.ViewContext.HttpContext.Request.Url;
			if (!uri.IsDefaultPort)
			{
				return String.Concat("http://", uri.Host, ":", uri.Port);
			}
			return String.Concat("http://", uri.Host);
		}
		#endregion

		#region Pager
		// TODO: Add routeValues
		public static string Pager(this HtmlHelper helper, string actionName, string controller,
			int pageIndex, int maximumRows, int totalRowCount, int buttonCount)
		{
			return helper.Pager(actionName, controller, pageIndex, maximumRows, totalRowCount, buttonCount,  
				new RouteValueDictionary(),	new RouteValueDictionary(), new RouteValueDictionary());
		}

		public static string Pager(this HtmlHelper helper, string actionName, string controller,
			int pageIndex, int maximumRows, int totalRowCount, int buttonCount, object routeValues)
		{
			return helper.Pager(actionName, controller, pageIndex, maximumRows, totalRowCount, buttonCount, 
				(RouteValueDictionary)routeValues, new RouteValueDictionary(), new RouteValueDictionary());
		}

		public static string Pager(this HtmlHelper helper, string actionName, string controller,
			int pageIndex, int maximumRows, int totalRowCount, int buttonCount, object routeValues,
			object pageHtmAttributes, object buttonsHtmlAttributes)
		{
			return helper.Pager(actionName, controller, pageIndex, maximumRows, totalRowCount, buttonCount,
				(RouteValueDictionary)routeValues, new RouteValueDictionary(pageHtmAttributes), 
				new RouteValueDictionary(buttonsHtmlAttributes));
		}

		public static string Pager(this HtmlHelper helper, string actionName, string controller,
			int pageIndex, int maximumRows, int totalRowCount, int buttonCount, RouteValueDictionary routeValues,
			IDictionary<string, object> pageHtmAttributes, IDictionary<string, object> buttonsHtmlAttributes)
		{
			helper.ViewContext.RouteData.Values.Merge(routeValues);

			StringBuilder sb = new StringBuilder();
			UrlHelper url = new UrlHelper(helper.ViewContext.RequestContext);

			int lastPageIndex = (totalRowCount / maximumRows) - 1;

			if (lastPageIndex > 0)
			{
				if (pageIndex > 0 || (pageIndex > buttonCount && lastPageIndex > (pageIndex + buttonCount)))
				{
					int prevPageIndex = pageIndex - buttonCount;
					if (prevPageIndex < 0) prevPageIndex = 0;

					routeValues["start"] = prevPageIndex;

					TagBuilder prev = new TagBuilder("a");
					prev.MergeAttributes(buttonsHtmlAttributes);
					prev.MergeAttribute("href", url.RouteUrl(routeValues));
					prev.InnerHtml = LeftArrow;

					sb.Append(prev.ToString(TagRenderMode.Normal));
					sb.Append(NonBreakingSpace);
				}

				var startIndex = 0;
				if ((pageIndex + buttonCount) < lastPageIndex)
					startIndex = pageIndex;
				else if (pageIndex > buttonCount && (pageIndex + buttonCount) >= lastPageIndex)
					startIndex = pageIndex;

				for (int i = startIndex; (i < (pageIndex + buttonCount) && i <= lastPageIndex); i++)
				{
					if (i == pageIndex)
					{
						sb.AppendFormat("[ {0} ]", pageIndex + 1);
					}
					else
					{
						routeValues["start"] = i;

						TagBuilder link = new TagBuilder("a");
						link.MergeAttributes(pageHtmAttributes);
						link.MergeAttribute("href", url.RouteUrl(routeValues));
						link.InnerHtml = (i + 1).ToString();
						sb.Append(link.ToString(TagRenderMode.Normal));
					}
					sb.Append(NonBreakingSpace);
				}

				if ((pageIndex + buttonCount) < lastPageIndex)
				{
					int nextPageIndex = pageIndex + buttonCount;
					if (nextPageIndex > lastPageIndex) nextPageIndex = lastPageIndex;

					routeValues["start"] = nextPageIndex;

					TagBuilder next = new TagBuilder("a");
					next.MergeAttributes(buttonsHtmlAttributes);
					next.MergeAttribute("href", url.RouteUrl(routeValues));
					next.InnerHtml = RightArrow;

					sb.Append(next.ToString(TagRenderMode.Normal));
					sb.Append(NonBreakingSpace);
				}
			}
			else
			{
				sb.Append("[ 1 ]");
			}

			return sb.ToString();
		}
		public static string Pager(this HtmlHelper helper, string actionName, string controller, int pageNumber, int pageSize, int totalRowCount)
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
		#endregion

		#region Login
		public static string Login(
			this HtmlHelper htmlHelper,
			string submitHref,
			string signUpAction,
			string signUpController,
			string forgotPasswordAction,
			string forgotPasswordController,
			string cancelAction,
			string cancelController,
			object containerHtmlAttributes,
			object buttonHtmlAttributes)
		{
			StringBuilder sb = new StringBuilder();

			//<div class="login">
			TagBuilder outerDiv = new TagBuilder("div");
			outerDiv.MergeAttributes<string, object>(((IDictionary<string, object>)new RouteValueDictionary(containerHtmlAttributes)));
			sb.Append(outerDiv.ToString(TagRenderMode.StartTag));

			//    <fieldset>
			sb.Append("<fieldset>");

			//        <legend>Login</legend>
			sb.Append("<legend>Login</legend>");

			//        <label for="username">UserName:</label>
			sb.Append("<label for=\"username\">UserName:</label>");

			//        <div>$username$</div>
			sb.Append("<div>").Append(htmlHelper.TextBox("username", null, new { length = 256, width = "150px" })).Append("</div>");

			//        <label for="password">Password:</label>
			sb.Append("<label for=\"password\">Password:</label>");

			//        <div>$password$</div>
			sb.Append("<div>").Append(htmlHelper.Password("password", null, new { length = 256, width = "150px" })).Append("</div>");

			//        <label for="rememberMe">Remember Me:</label>
			sb.Append("<label for=\"rememberMe\">Remember Me:</label>");

			//        <span>$rememberMe$</span>
			sb.Append("<span>").Append(htmlHelper.CheckBox("rememberMe", true)).Append("</span>");

			//        <div class="button">
			//            $login$
			//        </div>
			TagBuilder btnLogin = new TagBuilder("div");
			btnLogin.MergeAttributes<string, object>(((IDictionary<string, object>)new RouteValueDictionary(buttonHtmlAttributes)));
			sb.Append(btnLogin.ToString(TagRenderMode.StartTag));
			sb.AppendFormat("<a href=\"{0}\" title=\"Login\">Login</a>", submitHref);
			sb.Append(btnLogin.ToString(TagRenderMode.EndTag));

			//        <div class="button">
			//            $cancel$
			//        </div>
			TagBuilder btnCancel = new TagBuilder("div");
			btnCancel.MergeAttributes<string, object>(((IDictionary<string, object>)new RouteValueDictionary(buttonHtmlAttributes)));
			sb.Append(btnCancel.ToString(TagRenderMode.StartTag));
			sb.Append(htmlHelper.ActionLink("Cancel", cancelAction, cancelController));
			sb.Append(btnCancel.ToString(TagRenderMode.EndTag));

			//        <div>$signup$</div>
			sb.Append("<div>").Append(htmlHelper.ActionLink("Sign Up", signUpAction, signUpController)).Append("</div>");

			//        <div>$forgotPassword$</div>
			sb.Append("<div>").Append(htmlHelper.ActionLink("Forgot Password?", forgotPasswordAction, forgotPasswordController)).Append("</div>");

			//    </fieldset>
			sb.Append("</fieldset>");

			//</div>
			sb.Append(outerDiv.ToString(TagRenderMode.EndTag));

			return sb.ToString();
		}
		#endregion
	}
}
