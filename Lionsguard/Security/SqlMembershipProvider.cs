using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

using Lionsguard;
using Lionsguard.Providers;
using Lionsguard.Data;

namespace Lionsguard.Security
{
	public class SqlMembershipProvider : System.Web.Security.MembershipProvider
	{
		private string _appName;
		private string _connectionString;

		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			base.Initialize(name, config);

			_appName = ProviderUtil.GetAppName(config);
			_connectionString = ProviderUtil.GetConnectionString(config);

			ProviderUtil.CheckUnrecognizedAttributes(config);
		}

		public override string ApplicationName
		{
			get
			{
				return _appName;
			}
			set
			{
				_appName = value;
			}
		}

		private User GetUserFromReader(SqlNullDataReader reader)
		{
			return new User(this.Name, reader.GetString("UserName"),
							reader.GetInt32("UserId"), reader.GetString("Email"), reader.GetString("PasswordQuestion"),
							null, reader.GetBoolean("IsApproved"), reader.GetBoolean("IsLockedOut"),
							reader.GetDateTime("DateCreated"), reader.GetDateTime("DateLastLogin"),
							DateTime.Now, reader.GetDateTime("DateLastPasswordChanged"), reader.GetDateTime("DateLastLockOut"),
							reader.GetGuid("ConfirmationCode"), reader.GetInt32("Tokens"), reader.GetString("DisplayName"),
							reader.GetString("ImageUrl"), reader.GetDateTime("BirthDate"));
		}

		private bool CheckPassword(string username, string password, out System.Web.Security.MembershipUser user)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Users_GetUserByUserName",
				SqlHelper.CreateInputParam("@UserName", System.Data.SqlDbType.NVarChar, username)))
			{
				if (reader.Read())
				{
					string decryptedPass = Cryptography.Decrypt(reader.GetString("Password"), SecurityManager.EncryptKey, SecurityManager.EncryptIV, EncryptionAlgorithm.Rijndael);
					if (decryptedPass.Equals(password))
					{
						user = this.GetUserFromReader(reader);
						return true;

					}
				}
			}
			user = null;
			return false;
		}

		private string GetDecryptedPassword(string username)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Users_GetUserByUserName",
				   SqlHelper.CreateInputParam("@UserName", System.Data.SqlDbType.NVarChar, username)))
			{
				if (reader.Read())
				{
					return Cryptography.Decrypt(reader.GetString("Password"), SecurityManager.EncryptKey, SecurityManager.EncryptIV, EncryptionAlgorithm.Rijndael);
				}
			}
			return null;
		}

		private string GetDecryptedPassword(string username, string answer)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Users_GetUserByUserName",
				   SqlHelper.CreateInputParam("@UserName", System.Data.SqlDbType.NVarChar, username)))
			{
				if (reader.Read())
				{
					string decryptedAnswer = Cryptography.Decrypt(reader.GetString("PasswordAnswer"), SecurityManager.EncryptKey, SecurityManager.EncryptIV, EncryptionAlgorithm.Rijndael);
					if (decryptedAnswer.Equals(answer))
					{
						return Cryptography.Decrypt(reader.GetString("Password"), SecurityManager.EncryptKey, SecurityManager.EncryptIV, EncryptionAlgorithm.Rijndael);
					}
				}
			}
			return null;
		}

		private User GetMembershipUser(string username)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Users_GetUserByUserName",
				SqlHelper.CreateInputParam("@UserName", System.Data.SqlDbType.NVarChar, username)))
			{
				if (reader.Read())
				{
					return this.GetUserFromReader(reader);
				}
			}
			return null;
		}

		private User GetMembershipUser(int userId)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Users_GetUser",
				SqlHelper.CreateInputParam("@UserId", System.Data.SqlDbType.Int, userId)))
			{
				if (reader.Read())
				{
					return this.GetUserFromReader(reader);
				}
			}
			return null;
		}

		private User GetMembershipUserByEmail(string email)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Users_GetUserByEmail",
				SqlHelper.CreateInputParam("@Email", System.Data.SqlDbType.NVarChar, email)))
			{
				if (reader.Read())
				{
					return this.GetUserFromReader(reader);
				}
			}
			return null;
		}

		private void UpdateUserInternal(System.Web.Security.MembershipUser user, string password, string passwordAnswer)
		{
			this.UpdateUserInternal(user, password, passwordAnswer, user.PasswordQuestion, user.IsLockedOut);
		}
		private void UpdateUserInternal(System.Web.Security.MembershipUser user, string password, string passwordAnswer, string passwordQuestion, bool isLockedOut)
		{
			string pass = String.Empty, answer = String.Empty;
			if (!String.IsNullOrEmpty(password))
			{
				pass = Cryptography.Encrypt(password, SecurityManager.EncryptKey, SecurityManager.EncryptIV, EncryptionAlgorithm.Rijndael);
			}
			if (!String.IsNullOrEmpty(passwordAnswer))
			{
				answer = Cryptography.Encrypt(passwordAnswer, SecurityManager.EncryptKey, SecurityManager.EncryptIV, EncryptionAlgorithm.Rijndael);
			}

			int tokens = 0;
			string displayName = user.UserName;
			string imageUrl = null;
			DateTime? birthDate = null;
			Guid confirmatioCode = Guid.NewGuid();
			User lgUser = null;
			if (user is User)
			{
				lgUser = (user as User);
				tokens = lgUser.Tokens;
				displayName = lgUser.DisplayName;
				imageUrl = lgUser.ImageUrl;
				confirmatioCode = lgUser.ConfirmationCode;
				if (lgUser.BirthDate.CompareTo(DateTime.MinValue) != 0)
				{
					birthDate = lgUser.BirthDate;
				}
			}

			using (SqlCommand cmd = SqlHelper.ExecuteNonQuery(_connectionString, "dbo.lg_Users_SaveUser",
				SqlHelper.CreateInputParam("@UserName", SqlDbType.NVarChar, user.UserName),
				SqlHelper.CreateInputParam("@Password", SqlDbType.NVarChar, pass),
				SqlHelper.CreateInputParam("@DisplayName", SqlDbType.NVarChar, displayName),
				SqlHelper.CreateInputParam("@ImageUrl", SqlDbType.NVarChar, imageUrl),
				SqlHelper.CreateInputParam("@Email", SqlDbType.NVarChar, user.Email),
				SqlHelper.CreateInputParam("@ConfirmationCode", SqlDbType.UniqueIdentifier, confirmatioCode),
				SqlHelper.CreateInputParam("@PasswordQuestion", SqlDbType.NVarChar, passwordQuestion),
				SqlHelper.CreateInputParam("@PasswordAnswer", SqlDbType.NVarChar, answer),
				SqlHelper.CreateInputParam("@IsApproved", SqlDbType.NVarChar, user.IsApproved),
				SqlHelper.CreateInputParam("@IsLockedOut", SqlDbType.NVarChar, isLockedOut),
				SqlHelper.CreateInputParam("@Tokens", SqlDbType.Int, tokens),
				SqlHelper.CreateInputParam("@BirthDate", SqlDbType.DateTime, birthDate)))
			{
			}
		}

		public override bool ChangePassword(string username, string oldPassword, string newPassword)
		{
			System.Web.Security.MembershipUser user;
			if (this.CheckPassword(username, oldPassword, out user))
			{
				this.UpdateUserInternal(user, newPassword, null);
				return true;
			}
			return false;
		}

		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
		{
			System.Web.Security.MembershipUser user;
			if (this.CheckPassword(username, password, out user))
			{
				this.UpdateUserInternal(user, null, newPasswordAnswer, newPasswordQuestion, user.IsLockedOut);
				return true;
			}
			return false;
		}

		public override System.Web.Security.MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out System.Web.Security.MembershipCreateStatus status)
		{
			if (String.IsNullOrEmpty(username) || username.Length > 256)
			{
				status = System.Web.Security.MembershipCreateStatus.InvalidUserName;
				return null;
			}

			if (password.Length < this.MinRequiredPasswordLength)
			{
				status = System.Web.Security.MembershipCreateStatus.InvalidPassword;
				return null;
			}

			if (String.IsNullOrEmpty(email))
			{
				status = System.Web.Security.MembershipCreateStatus.InvalidEmail;
				return null;
			}

			if (String.IsNullOrEmpty(passwordQuestion))
			{
				status = System.Web.Security.MembershipCreateStatus.InvalidQuestion;
				return null;
			}

			if (String.IsNullOrEmpty(passwordAnswer))
			{
				status = System.Web.Security.MembershipCreateStatus.InvalidAnswer;
				return null;
			}

			System.Web.Security.MembershipUser user = this.GetMembershipUser(username);
			if (user != null)
			{
				status = System.Web.Security.MembershipCreateStatus.DuplicateUserName;
				return null;
			}

			user = this.GetMembershipUserByEmail(email);
			if (user != null)
			{
				status = System.Web.Security.MembershipCreateStatus.DuplicateEmail;
				return null;
			}
			
			user = new System.Web.Security.MembershipUser(this.Name, username, 0,
				email, passwordQuestion, null, isApproved, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
			this.UpdateUserInternal(user, password, passwordAnswer);
			status = System.Web.Security.MembershipCreateStatus.Success;
			return user;

		}

		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			throw new NotSupportedException("Deletion of User Accounts is not permitted.");
		}

		public override bool EnablePasswordReset
		{
			get { return true; }
		}

		public override bool EnablePasswordRetrieval
		{
			get { return true; }
		}

		public override System.Web.Security.MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			System.Web.Security.MembershipUserCollection list = new System.Web.Security.MembershipUserCollection();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Users_FindUsersByEmail",
				SqlHelper.CreateInputParam("@Email", SqlDbType.NVarChar, emailToMatch)))
			{
				while (reader.Read())
				{
					list.Add(this.GetUserFromReader(reader));
				}
			}
			totalRecords = list.Count;
			return list;
		}

		public override System.Web.Security.MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			System.Web.Security.MembershipUserCollection list = new System.Web.Security.MembershipUserCollection();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Users_FindUsersByName",
				SqlHelper.CreateInputParam("@UserName", SqlDbType.NVarChar, usernameToMatch)))
			{
				while (reader.Read())
				{
					list.Add(this.GetUserFromReader(reader));
				}
			}
			totalRecords = list.Count;
			return list;
		}

		public override System.Web.Security.MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
			System.Web.Security.MembershipUserCollection list = new System.Web.Security.MembershipUserCollection();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Users_GetAll",
				SqlHelper.CreateInputParam("@StartRowIndex", SqlDbType.Int, pageIndex),
				SqlHelper.CreateInputParam("@MaxRows", SqlDbType.Int, pageSize)))
			{
				while (reader.Read())
				{
					list.Add(this.GetUserFromReader(reader));
				}
			}
			totalRecords = list.Count;
			return list;
		}

		public override int GetNumberOfUsersOnline()
		{
			return 0;
		}

		public override string GetPassword(string username, string answer)
		{
			return this.GetDecryptedPassword(username, answer);
		}

		public string GetPassword(string username)
		{
			return this.GetDecryptedPassword(username);
		}

		public override System.Web.Security.MembershipUser GetUser(string username, bool userIsOnline)
		{
			return this.GetMembershipUser(username);
		}

		public override System.Web.Security.MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
			int id;
			if (providerUserKey != null && Int32.TryParse(providerUserKey.ToString(), out id))
			{
				return this.GetMembershipUser(id);
			}
			return null;
		}

		public override string GetUserNameByEmail(string email)
		{
			System.Web.Security.MembershipUser user = this.GetMembershipUserByEmail(email);
			if (user != null)
			{
				return user.UserName;
			}
			return String.Empty;
		}

		public override int MaxInvalidPasswordAttempts
		{
			get { return 5; }
		}

		public override int MinRequiredNonAlphanumericCharacters
		{
			get { return 0; }
		}

		public override int MinRequiredPasswordLength
		{
			get { return 6; }
		}

		public override int PasswordAttemptWindow
		{
			get { return 10; }
		}

		public override System.Web.Security.MembershipPasswordFormat PasswordFormat
		{
			get { return System.Web.Security.MembershipPasswordFormat.Encrypted; }
		}

		public override string PasswordStrengthRegularExpression
		{
			get { return String.Empty; }
		}

		public override bool RequiresQuestionAndAnswer
		{
			get { return true; }
		}

		public override bool RequiresUniqueEmail
		{
			get { return true; }
		}

		public override string ResetPassword(string username, string answer)
		{
			throw new NotSupportedException("Password reset is not permitted.");
		}

		public override bool UnlockUser(string userName)
		{
			System.Web.Security.MembershipUser user = this.GetMembershipUser(userName);
			if (user != null)
			{
				this.UpdateUserInternal(user, null, null, user.PasswordQuestion, false);
			}
			return false;
		}

		public override void UpdateUser(System.Web.Security.MembershipUser user)
		{
			this.UpdateUserInternal(user, null, null);
		}

		public override bool ValidateUser(string username, string password)
		{
			System.Web.Security.MembershipUser user;
			return this.CheckPassword(username, password, out user);
		}

		public string GetConfirmationCode(string username)
		{
			object obj = SqlHelper.ExecuteScalar(_connectionString, "dbo.lg_Users_GetConfirmationCode",
				SqlHelper.CreateInputParam("@UserName", SqlDbType.NVarChar, username));
			if (obj != null && obj != DBNull.Value)
			{
				return obj.ToString();
			}
			return null;
		}

		public void MigrateUser(System.Web.Security.MembershipUser user, string password)
		{
			//@UserName					nvarchar(256), 
			//@Password					nvarchar(256),
			//@Email						nvarchar(256),
			//@PasswordQuestion			nvarchar(256),
			//@IsApproved					bit, 
			//@IsLockedOut				bit, 
			//@DateCreated				datetime, 
			//@DateLastPasswordChanged	datetime, 
			//@DateLastLockout			datetime, 
			//@DateLastLogin				datetime
			string pass = null;
			if (!String.IsNullOrEmpty(password))
			{
				pass = Cryptography.Encrypt(password, SecurityManager.EncryptKey, SecurityManager.EncryptIV, EncryptionAlgorithm.Rijndael);
			}
			SqlHelper.ExecuteNonQuery(_connectionString, "dbo.lg_Users_MigrateUser",
				SqlHelper.CreateInputParam("@UserName", SqlDbType.NVarChar, user.UserName),
				SqlHelper.CreateInputParam("@Password", SqlDbType.NVarChar, pass),
				SqlHelper.CreateInputParam("@Email", SqlDbType.NVarChar, user.Email),
				SqlHelper.CreateInputParam("@PasswordQuestion", SqlDbType.NVarChar, user.PasswordQuestion),
				SqlHelper.CreateInputParam("@IsApproved", SqlDbType.Bit, user.IsApproved),
				SqlHelper.CreateInputParam("@IsLockedOut", SqlDbType.Bit, user.IsLockedOut),
				SqlHelper.CreateInputParam("@DateCreated", SqlDbType.DateTime, user.CreationDate),
				SqlHelper.CreateInputParam("@DateLastPasswordChanged", SqlDbType.DateTime, user.LastPasswordChangedDate),
				SqlHelper.CreateInputParam("@DateLastLockout", SqlDbType.DateTime, user.LastLockoutDate),
				SqlHelper.CreateInputParam("@DateLastLogin", SqlDbType.DateTime, user.LastLoginDate));
		}

		public void MigrateUserQuestionAndAnswer(string username, string question, string answer)
		{
			// Encrypting of the password and answer are handled in the ChangePasswordQuestionAndAnswer method.
			this.ChangePasswordQuestionAndAnswer(username, this.GetDecryptedPassword(username), question, answer);
		}

		public void Unsubscribe(string subscriptionName, string userName)
		{
			SqlHelper.ExecuteNonQuery(_connectionString, "dbo.lg_Subscriptions_Delete",
				SqlHelper.CreateInputParam("@UserName", SqlDbType.NVarChar, userName),
				SqlHelper.CreateInputParam("@SubscriptionName", SqlDbType.NVarChar, subscriptionName));
		}
	}
}
