using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Text;

namespace Lionsguard.Security
{
	[Serializable]
	public class User : MembershipUser
	{
		public Guid ConfirmationCode { get; set; }
		public int Tokens { get; set; }
		public string DisplayName { get; set; }
		public string ImageUrl { get; set; }
		public DateTime BirthDate { get; set; }

		public int ID
		{
			get
			{
				if (this.ProviderUserKey != null && this.ProviderUserKey != DBNull.Value)
				{
					return Convert.ToInt32(this.ProviderUserKey);
				}
				return 0;
			}
		}

		public User()
			: base()
		{
			this.ConfirmationCode = Guid.NewGuid();
		}

		public User(string providerName, string name, object providerUserKey, string email, string passwordQuestion, string comment,
			bool isApproved, bool isLockedOut, DateTime creationDate, DateTime lastLoginDate, DateTime lastActivityDate,
			DateTime lastPasswordChangedDate, DateTime lastLockOutDate, Guid confirmationCode, int tokens,
			string displayName, string imageUrl, DateTime birthDate)
			: base(providerName, name, providerUserKey, email, passwordQuestion, comment, isApproved, isLockedOut,
				creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, lastLockOutDate)
		{
			this.ConfirmationCode = confirmationCode;
			this.Tokens = tokens;
			this.DisplayName = displayName;
			this.ImageUrl = imageUrl;
			this.BirthDate = birthDate;
		}
	}
}
