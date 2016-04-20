using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Lionsguard.Security
{
	[Serializable]
	public class SecurityRedirect
	{
		public string ReturnUrl { get; set; }
		public string AuthReturnUrl { get; set; }
		public bool SignedOut { get; set; }	

		public bool HasValues
		{
			get { return (!String.IsNullOrEmpty(this.AuthReturnUrl) && !String.IsNullOrEmpty(this.ReturnUrl)); }
		}

		public SecurityRedirect() { }

		public string GetRedirectUrl(string authCookieValue)
		{
			SecureQueryString qs = new SecureQueryString();
			qs.Add("ReturnUrl", this.ReturnUrl);
			qs.Add("SignedOut", this.SignedOut.ToString());
			qs.Add("Auth", authCookieValue);
			return String.Format("{0}?{1}", HttpUtility.UrlDecode(this.AuthReturnUrl), qs.ToString());
		}

		public void RedirectIfAble(HttpContext context, string authCookieValue)
		{
			if (context != null)
			{
				if (!String.IsNullOrEmpty(this.AuthReturnUrl) && !String.IsNullOrEmpty(this.ReturnUrl))
				{
					try
					{
						context.Response.Redirect(this.GetRedirectUrl(authCookieValue));
					}
					catch (System.Threading.ThreadAbortException) { }
				}
			}
		}

		public static SecurityRedirect FromContext(HttpContext context)
		{
			SecurityRedirect sr = new SecurityRedirect();
			if (context != null)
			{
				if (context.Session != null)
				{
					sr = context.Session["SecurityRedirect"] as SecurityRedirect;
				}
				if (sr == null) sr = new SecurityRedirect();
				if (!sr.HasValues && (context.Request.QueryString.Count > 0))
				{
					sr = new SecurityRedirect();
					SecureQueryString qs = new SecureQueryString(context.Request.QueryString.ToString());
					sr.ReturnUrl = qs["ReturnUrl"];
					sr.AuthReturnUrl = qs["AuthReturnUrl"];

					if (context.Session != null)
					{
						context.Session["SecurityRedirect"] = sr;
					}
				}
			}
			return sr;
		}
	}
}
