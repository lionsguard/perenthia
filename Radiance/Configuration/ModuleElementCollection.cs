using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Radiance.Configuration
{
	public class ModuleElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new ModuleElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return (element as ModuleElement).Name;
		}
	}
}
