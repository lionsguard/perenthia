using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using Lionsguard.Security;
using Radiance.Contract;

namespace Perenthia.Web
{
	public static class Game
	{
        public const string INIT_PARAMS_FORMAT = "LoaderSourceList=/ClientBin/Perenthia.xap,authKey={0},servicesRootUri={1},gameService={2},armorialService={3},builderService={4},securityService={5},mediaUri={6},version={7},mode={8}";

        private static object _racesLock = new object();
		private static object _skillsLock = new object();
		private static object _attributesLock = new object();

        private static List<RaceData> _races = null;
		public static List<RaceData> Races
		{
			get
			{
				if (_races == null)
				{
					lock (_racesLock)
					{
						if (_races == null)
						{
							var client = ClientFactory.CreateClient<IArmorialService>(WebUtils.ArmorialServiceUri);
							_races = new List<RaceData>(client.GetRaces());
						}
					}
				}
				return _races;
			}
		}

		private static List<SkillData> _skills = null;
		public static List<SkillData> Skills
		{
			get
			{
				if (_skills == null)
				{
					lock (_skillsLock)
					{
						if (_skills == null)
						{
							var client = ClientFactory.CreateClient<IArmorialService>(WebUtils.ArmorialServiceUri);
							_skills = new List<SkillData>(client.GetSkills());
						}
					}
				}
				return _skills;
			}
		}

		private static string _version = String.Empty;
		private static object _versionLock = new object();
		public static string GetVersion()
		{
			lock (_versionLock)
			{
				if (String.IsNullOrEmpty(_version))
				{
					var client = ClientFactory.CreateClient<IArmorialService>(WebUtils.ArmorialServiceUri);
					_version = client.GetGameVersion();
				}
				return _version;
			}
		}

		private static Dictionary<string, NameValuePair> _attributes = null;
		public static string GetAttributeDescription(string name)
		{
			if (_attributes == null)
			{
				lock (_attributesLock)
				{
					if (_attributes == null)
					{
						_attributes = new Dictionary<string, NameValuePair>();
						var client = ClientFactory.CreateClient<IArmorialService>(WebUtils.ArmorialServiceUri);
						List<NameValuePair> list = new List<NameValuePair>(client.GetAttributeDetails());
						foreach (var item in list)
						{
							_attributes.Add(item.Name, item);
						}
					}
				}
			}
			if (_attributes.ContainsKey(name))
			{
				return _attributes[name].Description;
			}
			return String.Empty;
		}

		public static bool AttemptLogin(string username, string password, bool persistCookie, out string message)
		{
			message = String.Empty;
			if (Membership.ValidateUser(username, password))
			{
				// Ensure that if the app is in a test phase that only registered testers can play.
				Tester.TestPhase phase = (Tester.TestPhase)Enum.Parse(typeof(Tester.TestPhase), ConfigurationManager.AppSettings["TestPhase"], true);

				if (phase != Tester.TestPhase.None)
				{
					if (!Tester.IsTester(username, phase))
					{
						string msg;
						Tester.AddUser(username, phase, out msg);
						//alert.ErrorText = "The game is currently open only to members who registered specifically for this phase of testing or received an invite code.";
						//return;
					}
				}

                SetAuthCookie(username, persistCookie);

				return true;
			}
			else
			{
				message = "Invalid Login";
				return false;
			}
		}

        public static void SetAuthCookie(string username, bool persistCookie)
        {
            HttpCookie cookie = FormsAuthentication.GetAuthCookie(username, persistCookie);
            FormsAuthentication.SetAuthCookie(username, persistCookie);
        }
	}
}
