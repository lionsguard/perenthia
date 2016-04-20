using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

using Lionsguard.Security;

namespace Lionsguard
{
	public class SecureQueryString : NameValueCollection
	{
		public SecureQueryString()
		{
		}

		public SecureQueryString(string encryptedString)
		{
			this.Deserialize(this.Decrypt(HttpUtility.UrlDecode(encryptedString)));
		}

		public int GetInt32(string key)
		{
			int value;
			if (Int32.TryParse(base[key], out value))
			{
				return value;
			}
			return 0;
		}

		public override string ToString()
		{
			return HttpUtility.UrlEncode(this.Encrypt(this.Serialize()));
		}

		private string Encrypt(string serializedQueryString)
		{
			try
			{
				if (!String.IsNullOrEmpty(serializedQueryString))
				{
					return Cryptography.Encrypt(serializedQueryString, SecurityManager.EncryptKey, SecurityManager.EncryptIV, EncryptionAlgorithm.Rijndael);
				}
			}
			catch (Exception ex)
			{
				Log.Write(ex.ToString());
			}
			return String.Empty;
		}

		private string Decrypt(string encryptedQueryString)
		{
			try
			{
				if (!String.IsNullOrEmpty(encryptedQueryString) && Util.IsBase64(encryptedQueryString))
				{
					return Cryptography.Decrypt(encryptedQueryString, SecurityManager.EncryptKey, SecurityManager.EncryptIV, EncryptionAlgorithm.Rijndael);
				}
			}
			catch (Exception ex)
			{
				Log.Write(ex.ToString());
			}
			return String.Empty;
		}

		internal void Deserialize(string decryptedString)
		{
			string[] pairs = decryptedString.Split('&');
			if (pairs != null && pairs.Length > 0)
			{
				foreach (var pair in pairs)
				{
					string[] keyValues = pair.Split('=');
					if (keyValues != null && keyValues.Length == 2)
					{
						base.Add(keyValues[0], keyValues[1]);
					}
				}
			}
		}

		private string Serialize()
		{
			StringBuilder sb = new StringBuilder();
			foreach (var key in this.AllKeys)
			{
				if (sb.Length > 0) sb.Append("&");
				sb.AppendFormat("{0}={1}", key, base[key]);
			}
			return sb.ToString();
		}
	}
}
