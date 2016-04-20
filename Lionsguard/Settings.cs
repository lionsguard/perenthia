using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

using Lionsguard.Configuration;
using Lionsguard.Data;
using Lionsguard.Providers;

namespace Lionsguard
{
	public static class Settings
	{
		#region Properties
		private static Lionsguard.Providers.SettingsProvider _provider = null;
		private static Lionsguard.Providers.SettingsProviderCollection _providers = null;

		private static Dictionary<string, string> _settings = null;

		public static Lionsguard.Providers.SettingsProvider Provider
		{
			get
			{
				Initialize();
				return _provider;
			}
		}

		public static Lionsguard.Providers.SettingsProviderCollection Providers
		{
			get
			{
				Initialize();
				return _providers;
			}
		}

		public static string RootUrl
		{
			get
			{
				Initialize();
				return GetHost();
			}
		}

		public static string LoginUrl
		{
			get
			{
				Initialize();
				return GetUrlFromSettings("LoginUrl");
			}
		}

		public static string LogoutUrl
		{
			get
			{
				Initialize();
				return GetUrlFromSettings("LogoutUrl");
			}
		}

		public static string ChangePasswordUrl
		{
			get
			{
				Initialize();
				return GetUrlFromSettings("ChangePasswordUrl");
			}
		}

		public static string SignUpUrl
		{
			get
			{
				Initialize();
				return GetUrlFromSettings("SignUpUrl");
			}
		}

		public static string ForgotPasswordUrl
		{
			get
			{
				Initialize();
				return GetUrlFromSettings("ForgotPasswordUrl");
			}
		}

		public static string EmblemUrl
		{
			get
			{
				Initialize();
				return GetUrlFromSettings("EmblemUrl");
			}
		}

		public static string MyAccountUrl
		{
			get
			{
				Initialize();
				return GetUrlFromSettings("MyAccountUrl");
			}
		}

		public static string ForumsUrl
		{
			get
			{
				Initialize();
				return GetUrlFromSettings("ForumsUrl");
			}
		}

		public static string PrivacyUrl
		{
			get
			{
				Initialize();
				return GetUrlFromSettings("PrivacyUrl");
			}
		}

		public static string TermsOfServiceUrl
		{
			get
			{
				Initialize();
				return GetUrlFromSettings("TermsOfServiceUrl");
			}
		}

		private static string _redirectUrlAfterSignUp = null;
		private static string _redirectUrlAfterLogin = null;
		private static string _redirectUrlAfterLogout = null;
		public static string RedirectUrlAfterSignUp
		{
			get
			{
				Initialize();
				return _redirectUrlAfterSignUp;
			}
		}
		public static string RedirectUrlAfterLogin
		{
			get
			{
				Initialize();
				return _redirectUrlAfterLogin;
			}
		}
		public static string RedirectUrlAfterLogout
		{
			get
			{
				Initialize();
				return _redirectUrlAfterLogout;
			}
		}
		#endregion

		#region Initialize
		private static bool _initialized = false;
		private static object _lock = new object();
		private static Exception _initException = null;

		private static void Initialize()
		{
			if (!_initialized)
			{
				lock (_lock)
				{
					if (!_initialized)
					{
						try
						{
							SettingsSection section = ConfigurationManager.GetSection("lionsguard/settings") as SettingsSection;
							if (section != null)
							{
								_redirectUrlAfterLogin = section.RedirectUrlAfterLogin;
								_redirectUrlAfterLogout = section.RedirectUrlAfterLogout;
								_redirectUrlAfterSignUp = section.RedirectUrlAfterSignUp;

								_providers = new Lionsguard.Providers.SettingsProviderCollection();
								ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof(Lionsguard.Providers.SettingsProvider));
								_provider = _providers[section.DefaultProvider];
								if (_provider == null)
								{
									throw new ConfigurationErrorsException("Default SettingsProvider not found in application configuration file.", section.ElementInformation.Properties["defaultProvider"].Source, section.ElementInformation.Properties["defaultProvider"].LineNumber);
								}

								_settings = _provider.GetSettings();
							}
						}
						catch (Exception ex)
						{
							_initException = ex;
						}
						_initialized = true;
					}
				}
			}
			if (_initException != null)
			{
				throw _initException;
			}
		}
		#endregion

		private static string GetUrlFromSettings(string name)
		{
			if (_settings.ContainsKey(name))
			{
				return FormatUrl(_settings[name]);
			}
			return GetHost();
		}

		private static string FormatUrl(string url)
		{
			if (!String.IsNullOrEmpty(url))
			{
				if (url.StartsWith("http://") || url.StartsWith("https://"))
				{
					return url;
				}
				if (!url.StartsWith("/"))
				{
					url = String.Concat("/", url);
				}
				return String.Concat(GetHost(), url);
			}
			return GetHost();
		}

		private static string GetHost()
		{
			HttpContext context = HttpContext.Current;
			if (context != null)
			{
				if (context.Request != null)
				{
					if (context.Request.Url.IsLoopback)
					{
						return "http://localhost";
					}
				}
			}
			return "http://www.lionsguard.com";
		}
	}
}
