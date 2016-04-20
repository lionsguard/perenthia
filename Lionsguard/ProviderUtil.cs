using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.IO;
using System.Text;
using System.Web.Configuration;

namespace Lionsguard
{
	public static class ProviderUtil
	{
		public static string GetAppName(NameValueCollection config)
		{
			string appName = config["applicationName"];
			if (String.IsNullOrEmpty(appName))
			{
				appName = "Radiance";
			}
			config.Remove("applicationName");
			return appName;
		}

		public static string GetConnectionString(NameValueCollection config)
		{
			string connStr = ConfigurationManager.ConnectionStrings[config["connectionStringName"]].ConnectionString;
			config.Remove("connectionStringName");
			return connStr;
		}

		public static string GetAndRemoveStringAttribute(NameValueCollection config, string attributeName)
		{
			string val = config.Get(attributeName);
			if (val != null)
			{
				config.Remove(attributeName);
				return val;
			}
			return String.Empty;
		}

		public static int GetAndRemoveInt32Attribute(NameValueCollection config, string attributeName)
		{
			string val = config.Get(attributeName);
			if (!String.IsNullOrEmpty(val))
			{
				int result;
				if (Int32.TryParse(val, out result))
				{
					config.Remove(attributeName);
					return result;
				}
			}
			return 0;
		}

		public static bool GetAndRemoveBooleanAttribute(NameValueCollection config, string attributeName)
		{
			string val = config.Get(attributeName);
			if (!String.IsNullOrEmpty(val))
			{
				bool result;
				if (Boolean.TryParse(val, out result))
				{
					config.Remove(attributeName);
					return result;
				}
			}
			return false;
		}

		public static void CheckUnrecognizedAttributes(NameValueCollection config)
		{
			if (config.Count > 0)
			{
				string key = config.GetKey(0);
				if (!String.IsNullOrEmpty(key))
				{
					throw new ConfigurationErrorsException(String.Format("The attribute '{0}' is not a recognized attribute for this provider.", key));
				}
			}
		}
	}
}
