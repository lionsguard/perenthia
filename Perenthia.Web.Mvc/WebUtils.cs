using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text;
using RPXLib;

using Lionsguard;
using System.Web.Configuration;

namespace Perenthia.Web
{
	public static class WebUtils
	{
		public static string ArmorialServiceUri
		{
			get { return WebConfigurationManager.AppSettings["ArmorialServiceUri"]; }
		}
		public static string DepotServiceUri
		{
			get { return WebConfigurationManager.AppSettings["DepotServiceUri"]; }
		}
		public static string SecurityServiceUri
		{
			get { return WebConfigurationManager.AppSettings["SecurityServiceUri"]; }
		}

		public static string GetAuthReturnUrl(HttpContext context)
		{
			return String.Concat(Lionsguard.Util.GetServerUrl(context), "/Services/Auth.ashx");
		}

		public static string GetMembersUri()
		{
			return ConfigurationManager.AppSettings["MembersUri"];
		}

		public static string GetArmorialUri(string name)
		{
			string uri = GetMembersUri();
			if (!uri.EndsWith("/")) uri = String.Concat(uri, "/");
			return String.Concat(uri, "Armorial/Armorial.ashx?n=", name);
		}

		public static string GetPlayUri()
		{
			return ConfigurationManager.AppSettings["PlayUri"];
		}

        public static string GetRpxRealm()
        {
            return ConfigurationManager.AppSettings["RpxRealm"];
        }

        public static string GetRpxLoginUrl()
        {
            return String.Format("https://{0}.rpxnow.com/openid/v2/signin", ConfigurationManager.AppSettings["RpxRealm"]);
        }

        public static RPXApiSettings GetRpxSettings()
        {
            return new RPXApiSettings(ConfigurationManager.AppSettings["RpxBaseUrl"], ConfigurationManager.AppSettings["RpxApiKey"]);
		}

		public static string GetForumsUri()
		{
			return ConfigurationManager.AppSettings["ForumsUri"];
		}

		public static string GetBlogUri()
		{
			return ConfigurationManager.AppSettings["BlogUri"];
		}

		public static string SplitWords(string words)
		{
			StringBuilder sb = new StringBuilder();
			char[] chars = words.ToCharArray();
			for (int i = 0; i < chars.Length; i++)
			{
				if (Char.IsUpper(chars[i]) && i > 0) 
					sb.Append(' ');

				sb.Append(chars[i]);
			}
			return sb.ToString();
		}

		public static string CreateSecureQueryString(HttpContext context, string returnUrl)
		{
			SecureQueryString qs = new SecureQueryString();
			qs.Add("AuthReturnUrl", GetAuthReturnUrl(context));
			qs.Add("ReturnUrl", returnUrl);
			return qs.ToString();
		}

        public static string GetHouseholdImageUri(string uri)
        {
			if (!String.IsNullOrEmpty(uri))
			{
				return String.Concat("/common/media/households/", uri);
			}
            return "/common/media/avatar-blank.png";
        }

        public static string GetRankImageUri(string uri)
		{
			if (!String.IsNullOrEmpty(uri))
			{
				return String.Concat("/common/media/ranks/", uri);
			}
			return "/common/media/avatar-blank.png";
        }

        public static string FormatSkillName(object value)
        {
            if (value != null && value != DBNull.Value)
            {
                if (!String.IsNullOrEmpty(value.ToString()))
                {
                    return value.ToString().Replace("Skill_", String.Empty);
                }
            }
            return String.Empty;
        }
	}
}
