using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;


namespace Lionsguard.Providers
{
	public class ContentProviderCollection : ProviderCollection
	{
		public ContentProviderCollection() { }

		public override void Add(ProviderBase provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (!(provider is ContentProvider))
			{
				throw new ArgumentException("The supplied provider must implement the Lionsguard.Providers.ContentProvider type.", "provider");
			}
			base.Add(provider);
		}

		public void CopyTo(ContentProvider[] array, int index)
		{
			base.CopyTo(array, index);
		}

		public new ContentProvider this[string name]
		{
			get { return (ContentProvider)base[name]; }
		}
	}
}
