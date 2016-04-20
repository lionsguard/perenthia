using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Lionsguard.Handlers
{
	public class LogoutHandler : System.Web.IHttpHandler
	{
		#region IHttpHandler Members

		public void ProcessRequest(HttpContext context)
		{
			if (context.Session != null)
			{
				context.Session.Clear();
				context.Session.Abandon();
			}

			FormsAuthentication.SignOut();
			FormsAuthentication.RedirectToLoginPage();
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
