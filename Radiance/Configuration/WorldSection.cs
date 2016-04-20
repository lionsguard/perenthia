using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Radiance.Configuration
{
	public class WorldSection : ConfigurationElement
	{
		[ConfigurationProperty("name", IsRequired = true)]
		public string Name
		{
			get { return (string)base["name"]; }
			set { base["name"] = value; }
		}

		[ConfigurationProperty("worldType", IsRequired = true)]
		public string WorldType
		{
			get { return (string)base["worldType"]; }
			set { base["worldType"] = value; }
		}

		[ConfigurationProperty("enableMagic", IsRequired = false, DefaultValue = true)]
		public bool EnableMagic
		{
			get { return (bool)base["enableMagic"]; }
			set { base["enableMagic"] = value; }
		}

		[ConfigurationProperty("enablePsionics", IsRequired = false, DefaultValue = false)]
		public bool EnablePsionics
		{
			get { return (bool)base["enablePsionics"]; }
			set { base["enablePsionics"] = value; }
		}

		[ConfigurationProperty("enableCommandLogging", IsRequired = false, DefaultValue = false)]
		public bool EnableCommandLogging
		{
			get { return (bool)base["enableCommandLogging"]; }
			set { base["enableCommandLogging"] = value; }
		}

		[ConfigurationProperty("realismMultiplier", IsRequired = false, DefaultValue = 2)]
		public int RealismMultiplier
		{
			get { return (int)base["realismMultiplier"]; }
			set { base["realismMultiplier"] = value; }
		}

		[ConfigurationProperty("powerMultiplier", IsRequired = false, DefaultValue = 2)]
		public int PowerMultiplier
		{
			get { return (int)base["powerMultiplier"]; }
			set { base["powerMultiplier"] = value; }
		}

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

		[ConfigurationProperty("clientTimeoutMinutes", IsRequired = false, DefaultValue = 20)]
		public int ClientTimeoutMinutes
		{
			get { return (int)base["clientTimeoutMinutes"]; }
			set { base["clientTimeoutMinutes"] = value; }
		}

		[ConfigurationProperty("defaultMaxCharacters", IsRequired = false, DefaultValue = 1)]
		public int DefaultMaxCharacters
		{
			get { return (int)base["defaultMaxCharacters"]; }
			set { base["defaultMaxCharacters"] = value; }
		}

		[ConfigurationProperty("mapManagerType", IsRequired = true)]
		public string MapManagerType
		{
			get { return (string)base["mapManagerType"]; }
			set { base["mapManagerType"] = value; }
        }
	}
}
