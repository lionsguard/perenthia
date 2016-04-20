using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Radiance.Configuration
{
	public class ModuleElement : ConfigurationElement
	{
		private const string NameAttributeName = "name";
		private const string TypeAttributeName = "type";

		[ConfigurationProperty(NameAttributeName)]
		public string Name
		{
			get { return (string)base[NameAttributeName]; }
			set { base[NameAttributeName] = value; }
		}

		[ConfigurationProperty(TypeAttributeName)]
		public string Type
		{
			get { return (string)base[TypeAttributeName]; }
			set { base[TypeAttributeName] = value; }
		}
	}
}
