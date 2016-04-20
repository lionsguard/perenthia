using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Lionsguard.Security
{
	public static class Authentication
	{
		public static string Authenticate(string username, string password)
		{
			if (Membership.ValidateUser(username, password))
			{
				return Authentication.CreateTicket(username);
			}
			return String.Empty;
		}

		public static bool SetAuthenticationTicket(string ticket, HttpContext context)
		{
			if (!String.IsNullOrEmpty(ticket))
			{
				FormsAuthenticationTicket fat = FormsAuthentication.Decrypt(ticket);

				HttpCookie c = new HttpCookie(FormsAuthentication.FormsCookieName);
				c.Value = ticket;
				c.Expires = fat.Expiration;

				context.Response.Cookies.Add(c);

				return true;
			}
			return false;
		}

		public static void SignOut()
		{
			FormsAuthentication.SignOut();
		}

		public static string CreateTicket(MembershipUser user)
		{
			string[] roles = Roles.GetRolesForUser(user.UserName);

			if (!Roles.RoleExists(RoleNames.Mortal))
			{
				Roles.CreateRole(RoleNames.Mortal);
			}

			if (roles == null || roles.Length == 0)
			{
				roles = new string[] { RoleNames.Mortal };
				Roles.AddUserToRole(user.UserName, RoleNames.Mortal);
			}

			FormsAuthenticationTicket fat = new FormsAuthenticationTicket(2, user.UserName,
					DateTime.Now, DateTime.Now.AddDays(1), true, String.Join(",", roles));

			return FormsAuthentication.Encrypt(fat);
		}

		public static string CreateTicket(string username)
		{
			MembershipUser user = Membership.GetUser(username);
			if (user != null)
			{
				return Authentication.CreateTicket(user);
			}
			return String.Empty;
		}
	}
}
