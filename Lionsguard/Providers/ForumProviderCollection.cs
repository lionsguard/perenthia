using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

namespace Lionsguard.Providers
{
	public class ForumProviderCollection : ProviderCollection
	{
		public ForumProviderCollection() { }

		public override void Add(ProviderBase provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (!(provider is ForumProvider))
			{
				throw new ArgumentException("The supplied provider must implement the Lionsguard.Providers.ForumProvider type.", "provider");
			}
			base.Add(provider);
		}

		public void CopyTo(ForumProvider[] array, int index)
		{
			base.CopyTo(array, index);
		}

		public new ForumProvider this[string name]
		{
			get { return (ForumProvider)base[name]; }
		}
	}
}
