using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Radiance.Configuration
{
	public class LogSection : ConfigurationElement
	{
		[ConfigurationProperty("defaultProvider", IsRequired = true)]
		public string DefaultProvider
		{
			get { return (string)base["defaultProvider"]; }
			set { base["defaultProvider"] = value; }
		}

		[ConfigurationProperty("providers", IsRequired = true)]
		public ProviderSettingsCollection Providers
		{
			get { return (ProviderSettingsCollection)base["providers"]; }
			set { base["providers"] = value; }
		}
	}
}
