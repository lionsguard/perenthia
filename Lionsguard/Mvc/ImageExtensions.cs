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
    public static class ImageExtensions
    {
        public static string ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName)
        {
            return htmlHelper.ActionImage(imageUrl, linkText, actionName, null, new RouteValueDictionary(), new RouteValueDictionary());
        }

        public static string ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, object routeValues)
        {
            return htmlHelper.ActionImage(imageUrl, linkText, actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary());
        }

        public static string ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, string controllerName)
        {
            return htmlHelper.ActionImage(imageUrl, linkText, actionName, controllerName, new RouteValueDictionary(), new RouteValueDictionary());
        }

        public static string ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, RouteValueDictionary routeValues)
        {
            return htmlHelper.ActionImage(imageUrl, linkText, actionName, null, routeValues, new RouteValueDictionary());
        }

        public static string ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, object routeValues, object htmlanchorAttributes)
        {
            return htmlHelper.ActionImage(imageUrl, linkText, actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlanchorAttributes), null);
        }

        public static string ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlanchorAttributes, IDictionary<string, object> htmlImageAttributes)
        {
            return htmlHelper.ActionImage(imageUrl, linkText, actionName, null, routeValues, htmlanchorAttributes, htmlImageAttributes);
        }

        public static string ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, string controllerName, object routeValues, object htmlanchorAttributes)
        {
            return htmlHelper.ActionImage(imageUrl, linkText, actionName, controllerName, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlanchorAttributes), null);
        }

        public static string ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, object routeValues, object htmlanchorAttributes, object htmlImageAttributes)
        {
            return htmlHelper.ActionImage(imageUrl, linkText, actionName, controllerName, protocol, hostName, fragment, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlanchorAttributes), new RouteValueDictionary(htmlImageAttributes));
        }

        public static string ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlanchorAttributes, IDictionary<string, object> htmlImageAttributes)
        {
            // get the bare url for the Action using the current
            // request context
            //
            var _url = new UrlHelper(htmlHelper.ViewContext.RequestContext).Action(actionName, controllerName, routeValues);

            return GetImageLink(_url, linkText, imageUrl, htmlanchorAttributes, htmlImageAttributes);

        }

        public static string ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> htmlanchorAttributes, IDictionary<string, object> htmlImageAttributes)
        {

            // get the bare url for the Action using the current
            // request context
            //
            var _url = new UrlHelper(htmlHelper.ViewContext.RequestContext).Action(actionName, controllerName, routeValues, protocol, hostName);

            return GetImageLink(_url, linkText, imageUrl, htmlanchorAttributes, htmlImageAttributes);

        }

		public static string Image(this HtmlHelper htmlHelper, string src, string alt)
		{
			return htmlHelper.ImageInternal(src, alt, new RouteValueDictionary());
		}

		public static string Image(this HtmlHelper htmlHelper, string src, string alt, object htmlAttributes)
		{
			return htmlHelper.ImageInternal(src, alt, new RouteValueDictionary(htmlAttributes));
		}

		public static string Image(this HtmlHelper htmlHelper, string src, string alt, IDictionary<string, object> htmlAttributes)
		{
			return htmlHelper.ImageInternal(src, alt, htmlAttributes);
		}

		internal static string ImageInternal(this HtmlHelper htmlHelper, string src, string alt, IDictionary<string, object> htmlAttributes)
		{
			TagBuilder img = new TagBuilder("img");
			if (htmlAttributes != null) img.MergeAttributes(htmlAttributes);
			img.MergeAttribute("src", src);
			img.MergeAttribute("alt", alt);
			return img.ToString(TagRenderMode.SelfClosing);
		}

        /// <summary>
        /// Build up the anchor and image tag.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="linkText">The link text.</param>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="htmlanchorAttributes">The HTML anchor attributes.</param>
        /// <param name="htmlImageAttributes">The HTML image attributes.</param>
        /// <returns></returns>
        internal static string GetImageLink(string url, string linkText, string imageUrl, IDictionary<string, object> htmlanchorAttributes, IDictionary<string, object> htmlImageAttributes)
        {
            // build up the image link.
            // <a href=\"ActionUrl\"><img src=\"ImageUrl\" alt=\"Your Link Text\" /></a>
            //

            var _linkText = !string.IsNullOrEmpty(linkText) ? HttpUtility.HtmlEncode(linkText) : string.Empty;

            // build the img tag
            //
            TagBuilder _image = new TagBuilder("img");
            _image.MergeAttributes(htmlImageAttributes);
            _image.MergeAttribute("src", imageUrl);
            _image.MergeAttribute("alt", _linkText);

            // build the anchor tag
            //
            TagBuilder _link = new TagBuilder("a");
            _link.MergeAttributes(htmlanchorAttributes);
			_link.MergeAttribute("title", _linkText);
            _link.MergeAttribute("href", url);

            // place the img tag inside the anchor tag.
            //
            _link.InnerHtml = _image.ToString(TagRenderMode.Normal);

            // render the image link.
            //
            return _link.ToString(TagRenderMode.Normal);

        }
    }
}
