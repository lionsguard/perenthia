using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Lionsguard.Configuration
{
	public class ContentSection : ConfigurationSection
	{
		[ConfigurationProperty("sourceName")]
		public string SourceName
		{
			get { return (string)base["sourceName"]; }
			set { base["sourceName"] = value; }
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
