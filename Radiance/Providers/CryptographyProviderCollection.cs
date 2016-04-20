using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

namespace Radiance.Providers
{
	public class CryptographyProviderCollection : ProviderCollection
	{
		/// <summary>
		/// Initializes a new instance of the world provider collection.
		/// </summary>
		public CryptographyProviderCollection() { }

		/// <summary>
		/// Adds a new ProviderBase derived instance to the collection.
		/// </summary>
		/// <param name="provider">The provider to add to the collection.</param>
		public override void Add(ProviderBase provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (!(provider is CryptographyProvider))
			{
				throw new ArgumentException("The supplied provider must implement the Radiance.Providers.CryptographyProvider type.", "provider");
			}
			base.Add(provider);
		}

		/// <summary>
		/// Copys the collection of world providers to the specified array, starting at the specified index.
		/// </summary>
		/// <param name="array">The array to copy the current collection into.</param>
		/// <param name="index">The index position at which copying should begin.</param>
		public void CopyTo(CryptographyProvider[] array, int index)
		{
			base.CopyTo(array, index);
		}

		/// <summary>
		/// Gets the CryptographyProvider instance with the specified name.
		/// </summary>
		/// <param name="name">The name of the provider to retrieve.</param>
		/// <returns>An instance of the CryptographyProvider class with the specified name.</returns>
		public new CryptographyProvider this[string name]
		{
			get { return (CryptographyProvider)base[name]; }
		}
	}
}
