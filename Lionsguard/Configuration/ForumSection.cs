using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Lionsguard.Configuration
{
	public class ForumSection : ConfigurationSection
	{
		[ConfigurationProperty("boardName")]
		public string BoardName
		{
			get { return (string)base["boardName"]; }
			set { base["boardName"] = value; }
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
