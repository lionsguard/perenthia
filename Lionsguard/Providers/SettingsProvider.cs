using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

namespace Lionsguard.Providers
{
	public abstract class SettingsProvider : ProviderBase
	{
		public abstract Dictionary<string, string> GetSettings();
		public abstract void SaveSetting(string name, string value);
	}
}
