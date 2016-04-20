using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

namespace Lionsguard.Providers
{
	public class LogProviderCollection : ProviderCollection
	{
		public LogProviderCollection() { }

		public override void Add(ProviderBase provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (!(provider is LogProvider))
			{
				throw new ArgumentException("The supplied provider must implement the Lionsguard.Providers.LogProvider type.", "provider");
			}
			base.Add(provider);
		}

		public void CopyTo(LogProvider[] array, int index)
		{
			base.CopyTo(array, index);
		}

		public new LogProvider this[string name]
		{
			get { return (LogProvider)base[name]; }
		}
	}
}
