using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using Radiance.Configuration;
using Radiance.Providers;

namespace Radiance.Security
{
	/// <summary>
	/// Provides static methods for encrypting and decrypting strings.
	/// </summary>
	public static class Cryptography
	{
		#region Initialize
		private static CryptographyProvider _provider = null;
		private static CryptographyProviderCollection _providers = null;
		internal static void Initialize(CryptographySection section)
		{
			_providers = new CryptographyProviderCollection();
			_provider = ProviderUtil.InstantiateProviders<CryptographyProvider, CryptographyProviderCollection>(
				section.Providers, section.DefaultProvider, _providers);
			if (_provider == null)
			{
				throw new ConfigurationErrorsException(
					SR.ConfigDefaultCryptoProviderNotFound,
					section.ElementInformation.Properties["defaultProvider"].Source,
					section.ElementInformation.Properties["defaultProvider"].LineNumber);
			}
		}
		#endregion


		/// <summary>
		/// Encrypts the specified plainText and returns the encrypted text.
		/// </summary>
		/// <param name="plainText">The text tp encrypt.</param>
		/// <returns>The encrypted text.</returns>
		public static string Encrypt(string plainText)
		{
			return _provider.Encrypt(plainText);
		}

		/// <summary>
		/// Decryptes the specified cypherText and returns the decrypted value.
		/// </summary>
		/// <param name="cypherText">The text to decrypt.</param>
		/// <returns>The decrypted text.</returns>
		public static string Decrypt(string cypherText)
		{
			return _provider.Decrypt(cypherText);
		}
	}
}
