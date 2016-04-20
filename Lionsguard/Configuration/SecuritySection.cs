using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Lionsguard.Configuration
{
	public class SecuritySection : ConfigurationSection
	{
		[ConfigurationProperty("key")]
		public string Key
		{
			get { return (string)base["key"]; }
			set { base["key"] = value; }
		}

		[ConfigurationProperty("iv")]
		public string IV
		{
			get { return (string)base["iv"]; }
			set { base["iv"] = value; }
		}
	}
}
