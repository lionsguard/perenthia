using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

namespace Radiance.Providers
{
	public abstract class CryptographyProvider : ProviderBase
	{
		/// <summary>
		/// Encrypts the specified plainText and returns the encrypted text.
		/// </summary>
		/// <param name="plainText">The text tp encrypt.</param>
		/// <returns>The encrypted text.</returns>
		public abstract string Encrypt(string plainText);

		/// <summary>
		/// Decryptes the specified cypherText and returns the decrypted value.
		/// </summary>
		/// <param name="cipherText">The text to decrypt.</param>
		/// <returns>The decrypted text.</returns>
		public abstract string Decrypt(string cipherText);
	}
}
