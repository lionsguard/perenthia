using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Radiance.Configuration
{
	public class ScriptSection : ConfigurationElement
	{
		[ConfigurationProperty("commandsModuleName", IsRequired = true)]
		public string CommandsModuleName
		{
			get { return (string)base["commandsModuleName"]; }
			set { base["commandsModuleName"] = value; }
		}

		[ConfigurationProperty("eventsModuleName", IsRequired = true)]
		public string EventsModuleName
		{
			get { return (string)base["eventsModuleName"]; }
			set { base["eventsModuleName"] = value; }
		}
	}
}
