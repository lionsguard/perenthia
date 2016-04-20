using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

namespace Radiance.Providers
{
	/// <summary>
	/// Represents a collection of command provider objects.
	/// </summary>
	public class CommandProviderCollection : ProviderCollection
	{
		/// <summary>
		/// Initializes a new instance of the command provider collection.
		/// </summary>
		public CommandProviderCollection() { }

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
			if (!(provider is CommandProvider))
			{
				throw new ArgumentException("The supplied provider must implement the Radiance.Providers.CommandProvider type.", "provider");
			}
			base.Add(provider);
		}

		/// <summary>
		/// Copys the collection of command providers to the specified array, starting at the specified index.
		/// </summary>
		/// <param name="array">The array to copy the current collection into.</param>
		/// <param name="index">The index position at which copying should begin.</param>
		public void CopyTo(CommandProvider[] array, int index)
		{
			base.CopyTo(array, index);
		}

		/// <summary>
		/// Gets the CommandProvider instance with the specified name.
		/// </summary>
		/// <param name="name">The name of the provider to retrieve.</param>
		/// <returns>An instance of the CommandProvider class with the specified name.</returns>
		public new CommandProvider this[string name]
		{
			get { return (CommandProvider)base[name]; }
		}
	}
}
