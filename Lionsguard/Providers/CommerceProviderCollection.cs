using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

namespace Lionsguard.Providers
{
	public class CommerceProviderCollection : ProviderCollection
	{
		public CommerceProviderCollection() { }

		public override void Add(ProviderBase provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (!(provider is CommerceProvider))
			{
				throw new ArgumentException("The supplied provider must implement the Lionsguard.Providers.CommerceProvider type.", "provider");
			}
			base.Add(provider);
		}

		public void CopyTo(CommerceProvider[] array, int index)
		{
			base.CopyTo(array, index);
		}

		public new CommerceProvider this[string name]
		{
			get { return (CommerceProvider)base[name]; }
		}
	}
}
