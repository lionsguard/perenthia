using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web;
using System.Linq.Expressions;

namespace Lionsguard
{
    public static class MvcHelper
    {
		public static string Image(string src, string alt)
		{
			TagBuilder img = new TagBuilder("img");
			img.MergeAttribute("src", src);
			img.MergeAttribute("alt", alt);
			return img.ToString(TagRenderMode.SelfClosing);
		}

        public static string Login(
            HtmlHelper htmlHelper, 
            string submitHref,
            string signUpAction, 
            string signUpController, 
            string forgotPasswordAction, 
            string forgotPasswordController,
            string cancelAction,
            string cancelController)//,
            //object containerHtmlAttributes,
            //object buttonHtmlAttributes)
        {
            //StringBuilder sb = new StringBuilder();

            ////<div class="login">
            //TagBuilder outerDiv = new TagBuilder("div");
            //outerDiv.MergeAttributes<string, object>(((IDictionary<string, object>)new RouteValueDictionary(containerHtmlAttributes)));
            //sb.Append(outerDiv.ToString(TagRenderMode.StartTag));

            ////    <fieldset>
            //sb.Append("<fieldset>");

            ////        <legend>Login</legend>
            //sb.Append("<legend>Login</legend>");

            ////        <label for="username">UserName:</label>
            //sb.Append("<label for=\"username\">UserName:</label>");

            ////        <div>$username$</div>
            //sb.Append("<div>").Append(htmlHelper.TextBox("username", null, new { length = 256, width = "150px" })).Append("</div>");

            ////        <label for="password">Password:</label>
            //sb.Append("<label for=\"password\">Password:</label>");

            ////        <div>$password$</div>
            //sb.Append("<div>").Append(htmlHelper.Password("password", null, new { length = 256, width = "150px" })).Append("</div>");

            ////        <label for="rememberMe">Remember Me:</label>
            //sb.Append("<label for=\"rememberMe\">Remember Me:</label>");

            ////        <span>$rememberMe$</span>
            //sb.Append("<span>").Append(htmlHelper.CheckBox("rememberMe", true)).Append("</span>");

            ////        <div class="button">
            ////            $login$
            ////        </div>
            //TagBuilder btnLogin = new TagBuilder("div");
            //btnLogin.MergeAttributes<string, object>(((IDictionary<string, object>)new RouteValueDictionary(buttonHtmlAttributes)));
            //sb.Append(btnLogin.ToString(TagRenderMode.StartTag));
            //sb.AppendFormat("<a href=\"{0}\" title=\"Login\">Login</a>", submitHref);
            //sb.Append(btnLogin.ToString(TagRenderMode.EndTag));

            ////        <div class="button">
            ////            $cancel$
            ////        </div>
            //TagBuilder btnCancel = new TagBuilder("div");
            //btnCancel.MergeAttributes<string, object>(((IDictionary<string, object>)new RouteValueDictionary(buttonHtmlAttributes)));
            //sb.Append(btnCancel.ToString(TagRenderMode.StartTag));
            //sb.Append(htmlHelper.ActionLink("Cancel", cancelAction, cancelController));
            //sb.Append(btnCancel.ToString(TagRenderMode.EndTag));

            ////        <div>$signup$</div>
            //sb.Append("<div>").Append(htmlHelper.ActionLink("Sign Up", signUpAction, signUpController)).Append("</div>");

            ////        <div>$forgotPassword$</div>
            //sb.Append("<div>").Append(htmlHelper.ActionLink("Forgot Password?", forgotPasswordAction, forgotPasswordController)).Append("</div>");

            ////    </fieldset>
            //sb.Append("</fieldset>");

            ////</div>
            //sb.Append(outerDiv.ToString(TagRenderMode.EndTag));

            //return sb.ToString();

            string html = Resource.GetLocalResource("Lionsguard.Resources.Login.htm", null);
            if (!String.IsNullOrEmpty(html))
            {
                html = html.Replace("$username$", htmlHelper.TextBox("username", null, new { length = 256, width = "150px" }));
                html = html.Replace("$password$", htmlHelper.Password("password", null, new { length = 256, width = "150px" }));
                html = html.Replace("$rememberMe$", htmlHelper.CheckBox("rememberMe", true));
                html = html.Replace("$login$", String.Format("<a href=\"{0}\" title=\"Login\">Login</a>", submitHref));
                html = html.Replace("$signup$", htmlHelper.ActionLink("Sign Up", signUpAction, signUpController));
                html = html.Replace("$cancel$", htmlHelper.ActionLink("Cancel", cancelAction, cancelController));
                html = html.Replace("$forgotPassword$", htmlHelper.ActionLink("Forgot Password?", forgotPasswordAction, forgotPasswordController));
            }
            return html;
        }

        //public static string SignUp(
        //    HtmlHelper htmlHelper)
        //{
        //}
    }
}
