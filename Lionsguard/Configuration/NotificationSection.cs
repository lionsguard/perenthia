using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Lionsguard.Configuration
{
	public class NotificationSection : ConfigurationSection
	{
		[ConfigurationProperty("adminEmail")]
		public string AdminEmail
		{
			get { return (string)base["adminEmail"]; }
			set { base["adminEmail"] = value; }
		}

		[ConfigurationProperty("emailSubject")]
		public string EmailSubject
		{
			get { return (string)base["emailSubject"]; }
			set { base["emailSubject"] = value; }
		}

		[ConfigurationProperty("smtpServer")]
		public string SmtpServer
		{
			get { return (string)base["smtpServer"]; }
			set { base["smtpServer"] = value; }
		}

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
	}
}
