using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Lionsguard.Handlers
{
	public class LoginHandler : System.Web.IHttpHandler
	{
		#region IHttpHandler Members

		public void ProcessRequest(HttpContext context)
		{
			try
			{
				context.Response.Redirect(String.Concat(
					Lionsguard.Settings.LoginUrl,
					"?ReturnUrl=",
					HttpUtility.UrlEncode(String.Concat(Util.GetServerUrl(context), Lionsguard.Settings.RedirectUrlAfterLogin))));
			}
			catch (System.Threading.ThreadAbortException) { }
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		#endregion
	}
}
