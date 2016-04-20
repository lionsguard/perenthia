using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Timers;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Xml;
using Lionsguard.Configuration;
using Lionsguard.Data;

namespace Lionsguard.Security
{
	[DataObject]
	public static class SecurityManager
	{
		#region Initialize
		private static bool _initialized = false;
		private static object _lock = new object();
		private static Exception _initException = null;

		private static void Initialize()
		{
			if (!_initialized)
			{
				lock (_lock)
				{
					if (!_initialized)
					{
						try
						{
							SecuritySection section = ConfigurationManager.GetSection("lionsguard/security") as SecuritySection;
							if (section != null)
							{
								_encryptIV = section.IV;
								_encryptKey = section.Key;
							}
							else
							{
								throw new ConfigurationErrorsException("The 'lionsguard/security' section of the application configuration file was not found.");
							}
						}
						catch (Exception ex)
						{
							_initException = ex;
						}
						_initialized = true;
					}
				}
			}
			if (_initException != null)
			{
				throw _initException;
			}
		}
		#endregion

		#region Properties
		private static string _encryptKey;
		public static string EncryptKey
		{
			get
			{
				Initialize();
				return _encryptKey;
			}
		}

		private static string _encryptIV;
		public static string EncryptIV
		{
			get
			{
				Initialize();
				return _encryptIV;
			}
		}

		public static int MinPasswordLength
		{
			get
			{
				Initialize();
				return Membership.Provider.MinRequiredPasswordLength;
			}
		}
		#endregion

		[DataObjectMethod(DataObjectMethodType.Select)]
		public static MembershipUserCollection GetUsers(int startingRowIndex, int maxRows)
		{
			int totalRecords = 0;
			return Membership.GetAllUsers(startingRowIndex, maxRows, out totalRecords);
		}

		public static int GetUserCount()
		{
			int totalRecords = 0;
			Membership.Provider.GetAllUsers(0, -1, out totalRecords);
			return totalRecords;
		}

        public static string Encrypt(string plainText)
        {
            return Cryptography.Encrypt(plainText, EncryptKey, EncryptIV, EncryptionAlgorithm.Rijndael);
        }

        public static string Decrypt(string cipherText)
        {
			return Cryptography.Decrypt(cipherText, EncryptKey, EncryptIV, EncryptionAlgorithm.Rijndael);
        }

		public static User GetUser(HttpContext context)
		{
			if (context != null)
			{
				if (context.User.Identity.IsAuthenticated)
				{
					MembershipUser mUser = Membership.GetUser(context.User.Identity.Name);
					if (mUser != null && mUser is User)
					{
						return mUser as User;
					}
				}
			}
			return null;
		}

		public static User GetUser(string username)
		{
			if (!String.IsNullOrEmpty(username))
			{
				MembershipUser mUser = Membership.GetUser(username);
				if (mUser != null && mUser is User)
				{
					return mUser as User;
				}
			}
			return null;
		}

		public static User GetUser(AuthKey key)
		{
			if (key != AuthKey.Empty)
			{
				MembershipUser user = Membership.GetUser(key.UserName);
				if (user != null && user is User)
				{
					// Ensure that the supplied key matches the encryption value for this user.
					AuthKey checkKey = new AuthKey(user.UserName, Roles.GetRolesForUser(user.UserName));
					if (checkKey.ToString().Equals(key.ToString()))
					{
						return user as User;
					}
				}
			}
			return null;
		}

		public static User FindUserByEmail(string email)
		{
			if (!String.IsNullOrEmpty(email))
			{
				var users = Membership.FindUsersByEmail(email);
				foreach (var user in users)
				{
					if (user is User)
						return user as User;
				}
			}
			return null;
		}

		public static bool MigrateUser(MembershipUser user, string password)
		{
			try
			{
				if (Membership.Provider is SqlMembershipProvider)
				{
					(Membership.Provider as SqlMembershipProvider).MigrateUser(user, password);
					return true;
				}
			}
			catch (Exception ex)
			{
				Log.Write(ex.ToString());
			}
			return false;
		}

		public static bool MigrateUserQuestionAndAnswer(string username, string question, string answer)
		{
			try
			{
				if (Membership.Provider is SqlMembershipProvider)
				{
					(Membership.Provider as SqlMembershipProvider).MigrateUserQuestionAndAnswer(username, question, answer);
					return true;
				}
			}
			catch (Exception ex)
			{
				Log.Write(ex.ToString());
			}
			return false;
		}

		/// <summary>
		/// Use as the event handler for the Global.asax Application_AuthenticateRequest method.
		/// </summary>
		public static void AuthenticateRequest(HttpContext context)
		{
			if (context != null)
			{
				HttpCookie cookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
				if (cookie == null)
				{
					// Have the security service check the Lionsguard site for the cookie.
				}
				if (cookie != null)
				{
					FormsAuthenticationTicket fat = FormsAuthentication.Decrypt(cookie.Value);
					if (fat != null)
					{
						string[] roles = fat.UserData.Split(',');
						FormsIdentity identity = new FormsIdentity(fat);
						GenericPrincipal principal = new GenericPrincipal(identity, roles);
						context.User = principal;
					}
				}
			}
		}

		public static void SignIn(string userName, bool createPersistentCookie)
		{
			FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
		}

		public static void SetAuthCookie(HttpContext context, string cookieValue)
		{
			FormsAuthenticationTicket fat = FormsAuthentication.Decrypt(cookieValue);

			if (fat != null)
			{
				HttpCookie c = new HttpCookie(FormsAuthentication.FormsCookieName);
				c.Value = cookieValue;
				c.Expires = fat.Expiration;

				context.Response.Cookies.Add(c);
			}
		}

		public static void SignOut()
		{
			FormsAuthentication.SignOut();
		}

		public static void Unsubscribe(string subscriptionName, string userName)
		{
			try
			{
				if (Membership.Provider is SqlMembershipProvider)
				{
					(Membership.Provider as SqlMembershipProvider).Unsubscribe(subscriptionName, userName);
				}
			}
			catch (Exception ex)
			{
				Log.Write(ex.ToString());
			}
		}

		public static bool ValidateUser(string userName, string password, out bool isApproved)
		{
			isApproved = false;
			if (Membership.Provider.ValidateUser(userName, password))
			{
				MembershipUser user = Membership.Provider.GetUser(userName, false);
				if (user != null)
				{
					isApproved = user.IsApproved;
				}
				return true;
			}
			return false;
		}

		public static MembershipCreateStatus CreateUser(string username, string displayName, string password, string email, string birthDate, string securityQuestion, string securityAnswer)
		{
			MembershipCreateStatus status;
			Membership.Provider.CreateUser(username, password, email, securityQuestion, securityAnswer, false, null, out status);

			if (status == MembershipCreateStatus.Success)
			{
				DateTime bd = DateTime.Now;
				if (!DateTime.TryParse(birthDate, out bd))
					bd = DateTime.Now;

				User user = Membership.GetUser(username) as User;
				if (user != null)
				{
					user.DisplayName = displayName;
					user.BirthDate = bd;
					user.IsApproved = false;
					Membership.Provider.UpdateUser(user);

					Roles.AddUserToRole(user.UserName, RoleNames.Mortal);

					Notification.SendEmail("no-reply@lionsguard.com", user.Email, "New Lionsguard ID Created",
						Resource.GetLocalResource("Lionsguard.Resources.NewUserEmail.htm", 
						user.UserName, user.ConfirmationCode, user.Email));
				}
				else
				{
					status = MembershipCreateStatus.ProviderError;
					Log.Write(String.Format("User SignUp Error for UserName '{0}', Email '{1}'.", username, email), true);
				}
			}
			return status;
		}

		public static bool ChangePassword(string userName, string oldPassword, string newPassword)
		{
			MembershipUser currentUser = Membership.Provider.GetUser(userName, true /* userIsOnline */);
			if (currentUser.ChangePassword(oldPassword, newPassword))
			{
				User user = SecurityManager.GetUser(userName);
				if (user != null)
				{
					Notification.SendEmail("no-reply@lionsguard.com", user.Email, "Lionsguard ID Password Change",
						Resource.GetLocalResource("Lionsguard.Resources.ChangedPassword.htm", user.UserName));
					return true;
				}
			}
			return false;
		}

		public static string GetSecurityQuestion(ref string username, string email)
		{
			MembershipUser user = null;
			if (String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(email))
			{
				username = Membership.Provider.GetUserNameByEmail(email);
			}
			if (!String.IsNullOrEmpty(username))
			{
				user = Membership.Provider.GetUser(username, false);
			}
			if (user != null)
			{
				username = user.UserName;
				return user.PasswordQuestion;
			}
			return String.Empty;
		}

		public static bool ResendPassword(string username, string securityAnswer)
		{
			MembershipUser user = Membership.Provider.GetUser(username, false);
			if (user != null)
			{
				string password = Membership.Provider.GetPassword(username, securityAnswer);
				if (!String.IsNullOrEmpty(password))
				{
					Notification.SendEmail("no-reply@lionsguard.com", user.Email, "Lionsguard ID Details",
						Resource.GetLocalResource("Lionsguard.Resources.ForgotPasswordEmail.htm", username, password));
					return true;
				}
			}
			return false;
		}

		public static bool ResendPasswordWithoutSecurityAnswer(string username, string email)
		{
			MembershipUser user = null;
			if (String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(email))
			{
				username = Membership.Provider.GetUserNameByEmail(email);
			}
			if (!String.IsNullOrEmpty(username))
			{
				user = Membership.Provider.GetUser(username, false);
			}
			if (user != null)
			{
				string password = (Membership.Provider as SqlMembershipProvider).GetPassword(username);
				if (!String.IsNullOrEmpty(password))
				{
					Notification.SendEmail("no-reply@lionsguard.com", user.Email, "Lionsguard ID Details",
						Resource.GetLocalResource("Lionsguard.Resources.ForgotPasswordEmail.htm", username, password));
					return true;
				}
			}
			return false;
		}

		public static bool ConfirmUser(string email, string code)
		{
			MembershipUser user = Membership.Provider.GetUser(Membership.Provider.GetUserNameByEmail(email), false);
			if (user != null)
			{
				User lgUser = user as User;
				if (lgUser != null)
				{
					if (lgUser.ConfirmationCode.ToString().Equals(code))
					{
						lgUser.IsApproved = true;
						Membership.Provider.UpdateUser(lgUser);
						return true;
					}
				}
			}
			return false;
		}

		public static string ErrorCodeToString(MembershipCreateStatus createStatus)
		{
			// See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
			// a full list of status codes.
			switch (createStatus)
			{
				case MembershipCreateStatus.DuplicateUserName:
					return "Username already exists. Please enter a different user name.";

				case MembershipCreateStatus.DuplicateEmail:
					return "A username for that e-mail address already exists. Please enter a different e-mail address.";

				case MembershipCreateStatus.InvalidPassword:
					return "The password provided is invalid. Please enter a valid password value.";

				case MembershipCreateStatus.InvalidEmail:
					return "The e-mail address provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidAnswer:
					return "The password retrieval answer provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidQuestion:
					return "The password retrieval question provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidUserName:
					return "The user name provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.ProviderError:
					return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact customer support.";

				case MembershipCreateStatus.UserRejected:
					return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact customer support.";

				default:
					return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact customer support.";
			}
		}
	}
}
