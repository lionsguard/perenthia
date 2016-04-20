using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;
using System.Threading;

using Radiance;
using Radiance.Markup;
using Lionsguard;
using System.Text;

namespace Perenthia
{
	[DataObject]
	public static class Game
	{
		public static double HeartbeatInterval = 5000;

		private static object _lock = new object();

		private static Server _server = null;
		/// <summary>
		/// Gets the current Radiance.Server instance.
		/// </summary>
		public static Server Server
		{
			get
			{
				if (_server == null)
				{
					lock (_lock)
					{
						_server = new Server();
						Initialize();
					}
				}
				return _server;
			}
		}

		private static Dictionary<Point3, Temple> _temples = new Dictionary<Point3, Temple>();

		/// <summary>
		/// Initializes the game framework, loading races, skills, and all actor templates.
		/// </summary>
		public static void Initialize()
		{
			// Accessing the server property the first time creates a new instance.

			//=====================================================================================
			// RACES
			//=====================================================================================
			Server.World.Races.Add(NorvicRace.RaceName, new NorvicRace());
			Server.World.Races.Add(NajiiRace.RaceName, new NajiiRace());
			Server.World.Races.Add(PerenRace.RaceName, new PerenRace());
			Server.World.Races.Add(XhinRace.RaceName, new XhinRace());



			//=====================================================================================
			// SKILL GROUPS
			//=====================================================================================
			Server.World.SkillGroups.Add(new FighterSkillGroup());
			Server.World.SkillGroups.Add(new CasterSkillGroup());
			Server.World.SkillGroups.Add(new ThiefSkillGroup());
			Server.World.SkillGroups.Add(new ExplorerSkillGroup());
		}

		public static string GetVersion()
		{
			return GetVersion(3);
		}

		public static string GetVersion(int fieldCount)
		{
			return typeof(Character).Assembly.GetName().Version.ToString(fieldCount);
		}

		internal static void AddTemple(Temple temple)
		{
			if (!_temples.ContainsKey(temple.Location))
			{
				_temples.Add(temple.Location, temple);
			}
		}

		public static Temple FindTemple(Point3 location)
		{
			// There should typically be one temple in any given area.
			double curDistance = -1;
			Temple curTemple = null;
			foreach (var item in _temples)
			{
				double distance = Math.Sqrt(location.X - location.Y) + Math.Sqrt(item.Key.X - item.Key.Y);
				if (curDistance == -1)
				{
					curDistance = distance;
					curTemple = item.Value;
				}

				if (distance < curDistance)
				{
					curTemple = item.Value;
					break;
				}
			}
			return curTemple;
		}

        private static int _tempObjectId = 0;
        private static object _tempObjectIdLock = new object();
        public static int GetNextTempObjectId()
        {
            lock (_tempObjectIdLock)
            {
                return --_tempObjectId;
            }
        }

        public static void UpdateTwitter(string message)
        {
#if !DEBUG
            ThreadPool.QueueUserWorkItem(new WaitCallback((object o) =>
                {
                    try
                    {
                        // Send the message to the Perenthia twitter account (update status).
                        string username = ConfigurationManager.AppSettings["TwitterUserName"];
                        string password = ConfigurationManager.AppSettings["TwitterPassword"];
                        string url = ConfigurationManager.AppSettings["TwitterUrl"];
                        string msg = o as string;

                        if (String.IsNullOrEmpty(msg) || String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
                            return;

                        url = String.Concat(url, "?status=", HttpUtility.UrlEncode(msg));
                        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                        request.Method = "POST";
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.UserAgent = "Perenthia Updater";
                        request.Credentials = new NetworkCredential(username, password);
                        request.ContentLength = 0L;

                        WebResponse response = request.GetResponse();
                    }
                    catch (Exception ex)
                    {
						Logger.LogError(ex.ToString());
                    }
                }), message);
#endif
        }

		public static void AddUserToMailingList(string username, string email)
		{
			ThreadPool.QueueUserWorkItem((o) =>
				{
					try
					{
						var user = o as MLUser;
						var url = ConfigurationManager.AppSettings["MailingListSubscribeUri"];

						var request = (HttpWebRequest)HttpWebRequest.Create(url);
						request.Method = "POST";
						request.ContentType = "application/x-www-form-urlencoded";
						request.UserAgent = "Perenthia Updater";

						var data = Encoding.UTF8.GetBytes(String.Format("YMP0={0}&YMP1={1}", user.Email, user.UserName));
						request.ContentLength = data.Length;

						var stream = request.GetRequestStream();
						stream.Write(data, 0, data.Length);		

						var response = request.GetResponse();
						//if (response != null)
						//{
						//    using (var sr = new StreamReader(response.GetResponseStream()))
						//    {
						//        Logger.LogDebug(sr.ReadToEnd());
						//    }
						//}
					}
					catch (Exception ex)
					{
						Logger.LogError(ex.ToString());
					}
				}, new MLUser { UserName = username, Email = email });
		}
		private class MLUser
		{
			public string UserName { get; set; }
			public string Email { get; set; }	
		}

		#region Data Object Methods
		[DataObjectMethod(DataObjectMethodType.Select, true)]
		public static IEnumerable<IClient> GetClients()
		{
			return Game.Server.Clients;
		}

		[DataObjectMethod(DataObjectMethodType.Select, false)]
		public static int GetClientCount()
		{
			return Game.Server.Clients.Count;
		}

		[DataObjectMethod(DataObjectMethodType.Select, false)]
		public static IEnumerable<IActor> GetTemplates(int startingRowIndex, int maxRows)
		{
			return Game.Server.World.Provider.GetTemplates(startingRowIndex, maxRows);
		}

		[DataObjectMethod(DataObjectMethodType.Select, false)]
		public static int GetTemplatesCount()
		{
			return Game.Server.World.Provider.GetTemplatesCount();
		}
		#endregion
	}
}
