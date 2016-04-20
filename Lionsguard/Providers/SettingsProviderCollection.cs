using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

namespace Lionsguard.Providers
{
	public class SettingsProviderCollection : ProviderCollection
	{
		public SettingsProviderCollection() { }

		public override void Add(ProviderBase provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (!(provider is SettingsProvider))
			{
				throw new ArgumentException("The supplied provider must implement the Lionsguard.Providers.SettingsProvider type.", "provider");
			}
			base.Add(provider);
		}

		public void CopyTo(SettingsProvider[] array, int index)
		{
			base.CopyTo(array, index);
		}

		public new SettingsProvider this[string name]
		{
			get { return (SettingsProvider)base[name]; }
		}
	}
}
