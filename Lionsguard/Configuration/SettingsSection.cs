using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Lionsguard.Configuration
{
	public class SettingsSection : ConfigurationSection
	{
		[ConfigurationProperty("defaultProvider")]
		public string DefaultProvider
		{
			get { return (string)base["defaultProvider"]; }
			set { base["defaultProvider"] = value; }
		}

		[ConfigurationProperty("providers")]
		public ProviderSettingsCollection Providers
		{
			get { return (ProviderSettingsCollection)base["providers"]; }
		}

		[ConfigurationProperty("redirectUrlAfterSignUp", DefaultValue="")]
		public string RedirectUrlAfterSignUp
		{
			get { return (string)base["redirectUrlAfterSignUp"]; }
			set { base["redirectUrlAfterSignUp"] = value; }
		}

		[ConfigurationProperty("redirectUrlAfterLogin", DefaultValue = "")]
		public string RedirectUrlAfterLogin
		{
			get { return (string)base["redirectUrlAfterLogin"]; }
			set { base["redirectUrlAfterLogin"] = value; }
		}

		[ConfigurationProperty("redirectUrlAfterLogout", DefaultValue = "")]
		public string RedirectUrlAfterLogout
		{
			get { return (string)base["redirectUrlAfterLogout"]; }
			set { base["redirectUrlAfterLogout"] = value; }
		}
	}
}
