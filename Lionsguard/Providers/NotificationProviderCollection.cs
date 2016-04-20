using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

namespace Lionsguard.Providers
{
	public class NotificationProviderCollection : ProviderCollection
	{
		public NotificationProviderCollection() { }

		public override void Add(ProviderBase provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (!(provider is NotificationProvider))
			{
				throw new ArgumentException("The supplied provider must implement the Lionsguard.Providers.LogProvider type.", "provider");
			}
			base.Add(provider);
		}

		public void CopyTo(NotificationProvider[] array, int index)
		{
			base.CopyTo(array, index);
		}

		public new NotificationProvider this[string name]
		{
			get { return (NotificationProvider)base[name]; }
		}
	}
}
