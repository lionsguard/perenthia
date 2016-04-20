using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Radiance.Security;

namespace Radiance
{
	public struct AuthKey
	{
		public Guid SessionId;
		public string UserName;
		public int ID;
		public DateTime Date;
		public List<object> Args;

		public AuthKey(Guid sessionId, string username, int id)
			: this(sessionId, username, id, null)
		{
		}
		public AuthKey(Guid sessionId, string username, int id, params object[] args)
		{
			this.SessionId = sessionId;
			this.UserName = username;
			this.ID = id;	
			this.Date = DateTime.Now;
			this.Args = new List<object>();
			if (args != null && args.Length > 0)
				this.Args.AddRange(args);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendFormat("{0}|{1}|{2}|{3}", this.SessionId.ToString(), this.Date.Ticks, this.UserName, this.ID);
			if (this.Args != null && this.Args.Count > 0)
			{
				for (int i = 0; i < this.Args.Count; i++)
				{
					sb.Append("|").Append(this.Args[i]);
				}
			}
			return Cryptography.Encrypt(sb.ToString());
		}

		public static AuthKey Empty = new AuthKey(Guid.Empty, String.Empty, 0);

		public static AuthKey Get(string authenticationKey)
		{
			if (!String.IsNullOrEmpty(authenticationKey))
			{
				string plainText = Cryptography.Decrypt(authenticationKey);

				if (!String.IsNullOrEmpty(plainText))
				{
					string[] values = plainText.Split('|');
					if (values != null && values.Length >= 4)
					{
						var token = new AuthKey(new Guid(values[0]), values[2], 0, null);
						long ticks;
						if (Int64.TryParse(values[1], out ticks))
						{
							token.Date = new DateTime(ticks);
						}
						int id;
						if (Int32.TryParse(values[3], out id))
						{
							token.ID = id;	
						}
						if (values.Length > 4)
						{
							for (int i = 4; i < values.Length; i++)
							{
								token.Args.Add(values[i]);
							}
						}
						return token;
					}
				}
			}
			return AuthKey.Empty;
		}
	}
}
