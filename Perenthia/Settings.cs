using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;

using Radiance;

namespace Perenthia
{
	public static class Settings
	{
		public static string UserAuthKey = String.Empty;
		public static string PlayerAuthKey = String.Empty;

		public static string ServicesRootUri = String.Empty;

		public static string GameService = String.Empty;
		public static string BuilderService = String.Empty;
		public static string ArmorialService = String.Empty;
		public static string SecurityService = String.Empty;
		public static string DepotService = String.Empty;

		public static int GameServerPort = 0;
		public static int DepotServerPort = 0;
		public static int ArmorialServerPort = 0;

		public static string MediaUri = String.Empty;

		public static string GameVersion = "1.0.0";

		public static bool IsAdminMode = false;

		public static string Role = String.Empty;

		public static string Mode = "play";

		public static void LoadSettings(IDictionary<string, string> initParams)
		{
			UserAuthKey = GetStringValue("authKey", initParams);

			ServicesRootUri = GetStringValue("servicesRootUri", initParams);

			GameService = GetStringValue("gameService", initParams);
			BuilderService = GetStringValue("builderService", initParams);
			ArmorialService = GetStringValue("armorialService", initParams);
			SecurityService = GetStringValue("securityService", initParams);
			DepotService = GetStringValue("depotService", initParams);

			GameServerPort = GetInt32Value("gameServerPort", initParams);
			DepotServerPort = GetInt32Value("depotServerPort", initParams);
			ArmorialServerPort = GetInt32Value("armorialServerPort", initParams);

			GameVersion = GetStringValue("version", initParams);

			MediaUri = GetStringValue("mediaUri", initParams);

			Mode = GetStringValue("mode", initParams);
		}

		private static string GetPathValue(string key, IDictionary<string, string> values)
		{
			string value = String.Empty;
			if (values.ContainsKey(key))
			{
				value = values[key];
				if (!value.EndsWith("/"))
				{
					value = String.Concat(value, "/");
				}
			}
			return value;
		}

		private static string GetStringValue(string key, IDictionary<string, string> values)
		{
			if (values.ContainsKey(key))
			{
				return values[key];
			}
			return String.Empty;
		}

		private static int GetInt32Value(string key, IDictionary<string, string> values)
		{
			if (values.ContainsKey(key))
			{
				int result;
				if (Int32.TryParse(values[key], out result))
				{
					return result;
				}
			}
			return 0;
		}

		private static bool GetBooleanVaue(string key, IDictionary<string, string> values)
		{
			if (values.ContainsKey(key))
			{
				bool result;
				if (Boolean.TryParse(values[key], out result))
				{
					return result;
				}
			}
			return false;
		}

		public static Uri AppendServiceUri(string url)
		{
			if (!url.StartsWith("/")) url = String.Concat("/", url);
			return new Uri(String.Concat(Settings.ServicesRootUri, url), UriKind.Absolute);
		}

		public static Uri AppendServiceUri(string url, params KeyValuePair<string,string>[] args)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var item in args)
			{
				sb.Append(item.Key).Append("=").Append(item.Value).Append("&");
			}
			if (sb.Length > 0)
			{
				url = String.Concat(url, "?", sb.ToString());
			}
			return AppendServiceUri(url);
		}
	}
}
