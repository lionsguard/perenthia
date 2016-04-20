using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows.Threading;

using Lionsguard;
using Radiance;
using Radiance.Markup;
using Radiance.Contract;

namespace Perenthia
{
	public sealed class ServerManager : IDisposable
	{
		private static bool s_useHeartbeat = false;
		private static object s_lock = new object();
		private static ServerManager s_instance = null;
		public static ServerManager Instance
		{
			get
			{
				if (s_instance == null)
				{
					lock (s_lock)
					{
						if (s_instance == null)
						{
							s_instance = new ServerManager();
						}
					}
				}
				return s_instance;
			}
		}


		public static event EventHandler Connected = delegate { };
		public static event EventHandler ConnectFailed = delegate { };

		public event ServerResponseEventHandler Response = delegate { };

		private CommunicationManager _manager;
		private Queue<RdlTagCollection> _receivedTags = new Queue<RdlTagCollection>();
		private ServerResponseEventHandler _altResponse = null;

		private ServerManager()
		{
		}

		~ServerManager()
		{
			this.Dispose(false);
		}

		public static void Configure()
		{
			var failedCount = 0;
			var manager = new CommunicationManager(CommunicationProtocol.Sockets, App.Current.Host.Source, Settings.UserAuthKey, Settings.GameServerPort);
			TryConnect(manager, failedCount);
		}
		private static void TryConnect(CommunicationManager manager, int failedAttempts)
		{
			manager.Connected += (e) =>
			{
				Connected(manager, EventArgs.Empty);
				SetManager(manager);
			};
			manager.ConnectFailed += (e) =>
			{
				ConnectFailed(manager, EventArgs.Empty);
				//if (failedAttempts == 0)
				//{
				//    // Failed socket, connect http.
				//    failedAttempts++;
				//    s_useHeartbeat = true;
				//    TryConnect(new CommunicationManager(CommunicationProtocol.Http, Settings.AppendServiceUri(Settings.GameService), Settings.UserAuthKey), failedAttempts);
				//}
				//else
				//    Connected(manager, EventArgs.Empty); // TODO: Throw exception instead...
			};
			manager.Connect();
		}
		private static void SetManager(CommunicationManager manager)
		{
			Instance._manager = manager;
			Instance._manager.Failed += (args) =>
			{
			};
			Instance._manager.Error += (args) =>
			{
				StorageManager.WriteError(args.Error.ToString());
			};
		}

		public void Reset()
		{
			this.Response = delegate { };
		}

		public bool ReadTags(out RdlTagCollection tags)
		{
			lock (_receivedTags)
			{
				if (_receivedTags.Count > 0)
				{
					tags = _receivedTags.Dequeue();
					return true;
				}
			}
			tags = null;
			return false;
		}

		#region Heartbeat
		private DispatcherTimer _heartbeatTimer = null;

		public void StartHeartbeat()
		{
			if (!s_useHeartbeat)
				return;

			_heartbeatTimer = new DispatcherTimer();
			_heartbeatTimer.Interval = TimeSpan.FromSeconds(10);
			_heartbeatTimer.Tick += (o, e) =>
				{
					SendUserCommand(RdlCommand.Heartbeat);
				};
			_heartbeatTimer.Start();
		}
		#endregion

		#region SendCommand
		public void SendUserCommand(string commandName)
		{
			this.SendCommand(Settings.UserAuthKey, "User", commandName, null, null);
		}
		public void SendUserCommand(string commandName, params object[] args)
		{
			this.SendCommand(Settings.UserAuthKey, "User", commandName, null, args);
		}
		public void SendUserCommand(string commandName, ServerResponseEventHandler callback, params object[] args)
		{
			this.SendCommand(Settings.UserAuthKey, "User", commandName, callback, args);
		}

		public void SendUserCommand(RdlCommand cmd)
		{
			this.SendCommand(Settings.UserAuthKey, "User", cmd, null, null);
		}
		public void SendUserCommand(RdlCommand cmd, ServerResponseEventHandler callback)
		{
			this.SendCommand(Settings.UserAuthKey, "User", cmd, callback, null);
		}

		public void SendCommand(string commandName)
		{
			this.SendCommand(Settings.PlayerAuthKey, "Player", commandName, null, null);
		}
		public void SendCommand(string commandName, params object[] args)
		{
			this.SendCommand(Settings.PlayerAuthKey, "Player", commandName, null, args);
		}
		public void SendCommand(string commandName, ServerResponseEventHandler callback, params object[] args)
		{
			this.SendCommand(Settings.PlayerAuthKey, "Player", commandName, callback, args);
		}

		public void SendCommand(RdlCommand cmd)
		{
			this.SendCommand(cmd, null);
		}
		public void SendCommand(RdlCommand cmd, ServerResponseEventHandler callback)
		{
			this.SendCommand(Settings.PlayerAuthKey, "Player", cmd, callback, null);
		}

		private void SendCommand(string authKey, string authKeyType, string commandName, ServerResponseEventHandler callback, params object[] args)
		{
			RdlCommand cmd = new RdlCommand(commandName.ToUpper());
			if (args != null && args.Length > 0)
			{
				cmd.Args.AddRange(args);
			}
			this.SendCommand(authKey, authKeyType, cmd, callback, null);
		}
		public void SendCommand(string authKey, string authKeyType, RdlCommand cmd, ServerResponseEventHandler callback)
		{
			this.SendCommand(authKey, authKeyType, cmd, callback, null);
		}
		public void SendCommand(string authKey, string authKeyType, RdlCommand cmd, ServerResponseEventHandler callback, RdlTagCollection tags)
		{
			//RdlCommandGroup group = new RdlCommandGroup(new RdlCommand[] { cmd });
			//group.AuthKey = authKey;
			//group.AuthKeyType = authKeyType;
			//if (tags != null)
			//{
			//    group.Tags.AddRange(tags);
			//}

//#if DEBUG
			//            Logger.LogDebug(String.Concat("CLIENT CMD = ", group.ToString()));
//#endif
			//var commands = new RdlCommandGroup(new RdlCommand[] { cmd });
			//commands.AuthKey = authKey;
			//commands.AuthKeyType = authKeyType;

			Logger.LogDebug(String.Format("Sending Command: {0}", cmd));

			_altResponse = callback;
			//_client.ProcessAsync(commands.ToBytes());

			_manager.AuthKey = authKey;
			_manager.AuthKeyType = authKeyType;
			_manager.SendCommand(cmd, (e) =>
				{
					if (e.Tags.Count > 0)
					{
						if (_altResponse != null)
						{
							_altResponse(new ServerResponseEventArgs(e.Tags));
							_altResponse = null;
						}
						else
						{
							this.Response(new ServerResponseEventArgs(e.Tags));
						}
					}
				});
		}
		#endregion

		#region IDisposable Members

		private bool _disposed = false;

		public void Dispose()
		{
			this.Dispose(true);

			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (_heartbeatTimer != null)
						_heartbeatTimer.Stop();
				}

				_disposed = true;
			}
		}

		#endregion
	}

	public delegate void ServerResponseEventHandler(ServerResponseEventArgs e);
	public class ServerResponseEventArgs : EventArgs
	{
		public RdlTagCollection Tags { get; set; }

		public ServerResponseEventArgs(RdlTagCollection tags)
		{
			this.Tags = tags;
		}
	}
}
