using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Radiance.Configuration
{
	/// <summary>
	/// Represents the configuration file section for the Radiance game engine.
	/// </summary>
	public class RadianceSection : ConfigurationSection
	{
		private const string ModulesElementName = "modules";

		[ConfigurationProperty("world", IsRequired = true)]
		public WorldSection World
		{
			get { return (WorldSection)base["world"]; }
			set { base["world"] = value; }
		}

		[ConfigurationProperty("log", IsRequired = true)]
		public LogSection Log
		{
			get { return (LogSection)base["log"]; }
			set { base["log"] = value; }
		}

		[ConfigurationProperty("cryptography", IsRequired = true)]
		public CryptographySection Cryptography
		{
			get { return (CryptographySection)base["cryptography"]; }
			set { base["cryptography"] = value; }
		}

		[ConfigurationProperty("command", IsRequired = true)]
		public CommandSection Command
		{
			get { return (CommandSection)base["command"]; }
			set { base["command"] = value; }
		}

		[ConfigurationProperty("script", IsRequired = true)]
		public ScriptSection Script
		{
			get { return (ScriptSection)base["script"]; }
			set { base["script"] = value; }
		}

		[ConfigurationProperty(ModulesElementName, IsRequired = false)]
		public ModuleElementCollection Modules
		{
			get { return (ModuleElementCollection)base[ModulesElementName]; }
			set { base[ModulesElementName] = value; }
		}
	}
}
