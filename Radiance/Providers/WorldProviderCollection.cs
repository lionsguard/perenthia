using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

namespace Radiance.Providers
{
	/// <summary>
	/// Represents a collection of world provider objects.
	/// </summary>
	public class WorldProviderCollection : ProviderCollection
	{
		/// <summary>
		/// Initializes a new instance of the world provider collection.
		/// </summary>
		public WorldProviderCollection() { }

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
			if (!(provider is WorldProvider))
			{
				throw new ArgumentException("The supplied provider must implement the Radiance.Providers.WorldProvider type.", "provider");
			}
			base.Add(provider);
		}

		/// <summary>
		/// Copys the collection of world providers to the specified array, starting at the specified index.
		/// </summary>
		/// <param name="array">The array to copy the current collection into.</param>
		/// <param name="index">The index position at which copying should begin.</param>
		public void CopyTo(WorldProvider[] array, int index)
		{
			base.CopyTo(array, index);
		}

		/// <summary>
		/// Gets the WorldProvider instance with the specified name.
		/// </summary>
		/// <param name="name">The name of the provider to retrieve.</param>
		/// <returns>An instance of the WorldProvider class with the specified name.</returns>
		public new WorldProvider this[string name]
		{
			get { return (WorldProvider)base[name]; }
		}
	}
}
