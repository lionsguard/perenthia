using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using Lionsguard;
using Lionsguard.Data;

namespace Perenthia.Web
{
	public static class Tester
	{
		public enum TestPhase
		{
			None,
			Alpha,
			Beta,
		}

		private static string _connectionString = String.Empty;

		static Tester()
		{
			_connectionString = ConfigurationManager.ConnectionStrings["LionsguardConnection"].ConnectionString;
		}

		public static bool IsTester(string username, TestPhase phase)
		{
			object obj = SqlHelper.ExecuteScalar(_connectionString, "SELECT 1 FROM dbo.lg_perenthia_testers WHERE UserName = @UserName AND IsAlphaTester = @IsAlphaTester AND IsBetaTester = @IsBetaTester", CommandType.Text,
							SqlHelper.CreateInputParam("@UserName", SqlDbType.NVarChar, username),
							SqlHelper.CreateInputParam("@IsAlphaTester", SqlDbType.Bit, (phase == TestPhase.Alpha)),
							SqlHelper.CreateInputParam("@IsBetaTester", SqlDbType.Bit, (phase == TestPhase.Beta)));
			if (obj != null && obj != DBNull.Value)
			{
				return true;
			}
			return false;
		}

		public static bool AddUser(string username, TestPhase phase, out string message)
		{
			if (IsTester(username, phase))
			{
				message = String.Format("You are already registered for the Perenthia {0}.", phase);
				return false;
			}
			else
			{
				try
				{
					MembershipUser user = Membership.GetUser(username);
					if (user != null)
					{
						Notification.SendEmail("no-reply@perenthia.com", user.Email, 
							String.Format("Perenthia {0} Registration Confirmation", phase), 
							String.Format("You have successfully registered for the Perenthia {0}. You may visit http://alpha.perenthia.com to begin playing!{1}{1}Perenthia PBBG", phase, Environment.NewLine));

						SqlHelper.ExecuteNonQuery(_connectionString, "INSERT INTO dbo.lg_perenthia_testers (UserName, IsAlphaTester, IsBetaTester) VALUES (@UserName, @IsAlphaTester, @IsBetaTester);", CommandType.Text,
							SqlHelper.CreateInputParam("@UserName", SqlDbType.NVarChar, user.UserName),
							SqlHelper.CreateInputParam("@IsAlphaTester", SqlDbType.Bit, (phase == TestPhase.Alpha)),
							SqlHelper.CreateInputParam("@IsBetaTester", SqlDbType.Bit, (phase == TestPhase.Beta)));

						message = String.Format("You have successfully registered for the Perenthia {0}. You may visit http://alpha.perenthia.com to begin playing.", phase);
						return true;
					}
					else
					{
						message = String.Format("Authentication is required in order to register for the Perenthia {0}.", phase);
						return false;
					}
				}
				catch (Exception ex)
				{
					Log.Write(ex.ToString(), true);
					message = "An error occurred during registration. Technical Support has bee notified of the issue and will work to resolve it ASAP. Please try back again later.";
					return false;
				}
			}
		}
	}
}
