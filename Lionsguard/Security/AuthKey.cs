using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lionsguard.Security
{
	public class AuthKey
	{
		public string UserName { get; set; }
		public string[] Roles { get; set; }
		public DateTime DateCreated { get; private set; }
		public List<object> Args { get; private set; }	

		public AuthKey(string username, string[] roles)
			: this(username, roles, null)
		{
		}

		public AuthKey(string username, string[] roles, params object[] args)
		{
			this.UserName = username;
			this.Roles = roles;
			this.DateCreated = DateTime.Now;
			this.Args = new List<object>();
			if (args != null && args.Length > 0)
				this.Args.AddRange(args);
		}

		public override bool Equals(object obj)
		{
			var key = obj as AuthKey;
			if (key == null)
				return false;

			return key.UserName == this.UserName;
		}

		public override int GetHashCode()
		{
			return this.UserName.GetHashCode();
		}

		public override string ToString()
		{
			try
			{
				if (!String.IsNullOrEmpty(this.UserName) && (this.Roles != null && this.Roles.Length > 0))
				{
					var sb = new StringBuilder();
					sb.AppendFormat("{0}|{1}|{2}", 
						this.UserName,
						String.Join(",", this.Roles),
						this.DateCreated.Ticks);
					if (this.Args != null && this.Args.Count > 0)
					{
						for (int i = 0; i < this.Args.Count; i++)
						{
							sb.Append("|").Append(this.Args[i]);
						}
					}
					return Cryptography.Encrypt(sb.ToString(), SecurityManager.EncryptKey, SecurityManager.EncryptIV, EncryptionAlgorithm.Rijndael);
				}
			}
			catch (Exception ex)
			{
				Log.Write(ex.ToString(), true);
			}
			return String.Empty;
		}

		#region Static Members
		public static AuthKey Empty = new AuthKey(String.Empty, new string[0]);

		public static bool TryParse(string encryptedAuthKey, out AuthKey key)
		{
			try
			{
				if (!String.IsNullOrEmpty(encryptedAuthKey))
				{
					string value = Cryptography.Decrypt(encryptedAuthKey.Replace(' ', '+'), SecurityManager.EncryptKey, SecurityManager.EncryptIV, EncryptionAlgorithm.Rijndael);
					if (!String.IsNullOrEmpty(value))
					{
						string[] parts = value.Split('|');
						if (parts != null && parts.Length >= 3)
						{
							key = new AuthKey(parts[0], parts[1].Split(','));
							key.DateCreated = new DateTime(Convert.ToInt32(parts[2]));
							if (parts.Length > 3)
							{
								for (int i = 2; i < parts.Length; i++)
								{
									key.Args.Add(parts[i]);
								}
							}
							return true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Write(ex.ToString(), true);
			}
			key = AuthKey.Empty;
			return false;
		}
		#endregion
	}
}
