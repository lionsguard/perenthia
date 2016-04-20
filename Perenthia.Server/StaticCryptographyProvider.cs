using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Lionsguard;
using Lionsguard.Data;
using Lionsguard.Providers;
using Lionsguard.Security;
using Radiance;
using Radiance.Markup;
using Radiance.Providers;

namespace Perenthia
{
	public class StaticCryptographyProvider : CryptographyProvider
	{
		public override string Encrypt(string plainText)
		{
			try
			{
				return Cryptography.Encrypt(plainText, SecurityManager.EncryptKey, SecurityManager.EncryptIV, EncryptionAlgorithm.Rijndael);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(String.Format("Encrypt Failed: plainText={0}, Key={1}, IV={2}", plainText, SecurityManager.EncryptKey, SecurityManager.EncryptIV), ex);
			}
		}

		public override string Decrypt(string cipherText)
		{
			try
			{
				return Cryptography.Decrypt(cipherText, SecurityManager.EncryptKey, SecurityManager.EncryptIV, EncryptionAlgorithm.Rijndael);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(String.Format("Decrypt Failed: cipherText={0}, Key={1}, IV={2}", cipherText, SecurityManager.EncryptKey, SecurityManager.EncryptIV), ex);
			}
		}
	}
}
