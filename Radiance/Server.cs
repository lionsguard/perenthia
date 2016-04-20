using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using Radiance.Configuration;
using Radiance.Handlers;
using Radiance.Markup;
using Radiance.Security;
using System.Threading;
using Lionsguard;

namespace Radiance
{
	/// <summary>
	/// Represents the virtual world server that provides access to the virtual world from outside programs and clients.
	/// </summary>
	/// <remarks>
	/// The server handles input from clients from a variety of sources such as HTTP or Sockets and passes that 
	/// information to the virtual world.
	/// </remarks>
	public sealed class Server : IDisposable
	{
		/// <summary>
		/// Gets the current virtual world instance.
		/// </summary>
		public World World { get; private set; }

		/// <summary>
		/// Gets the collection of IClient instances connected to the virtual world.
		/// </summary>
		public ClientManager Clients { get; private set; }

		/// <summary>
		/// Gets the name of the python commands module, where commands are stored.
		/// </summary>
		public string CommandsModuleName { get; private set; }

		/// <summary>
		/// Gets the name of the python events module, where custom actor events are stored.
		/// </summary>
		public string EventsModuleName { get; private set; }

		private System.Timers.Timer _serverTimer = null;
		private AutoResetEvent _signal = new AutoResetEvent(true);
		private bool _running = false;
		private Queue<ClientPacket> _receivedPackets = new Queue<ClientPacket>();

		private List<IModule> _modules = new List<IModule>();

		public bool IsRunning
		{
			get { return _running; }
		}

		/// <summary>
		/// Initializes a new server instance and loads the virtual world from the application configuration file.
		/// </summary>
		public Server()
			: this(ConfigurationManager.GetSection("radiance") as RadianceSection)
		{
		}

		/// <summary>
		/// Initializes a new server instance and loads the virtual world from the specified configuration section.
		/// </summary>
		/// <param name="configSection">The RadianceSection to use in loading the virtual world.</param>
		public Server(RadianceSection configSection)
		{
			this.Clients = new ClientManager();
			this.Clients.ClientAdded += (o, e) => 
			{ 
				if (_serverTimer != null && !_serverTimer.Enabled) 
					_serverTimer.Start(); 
			};
			this.Initialize(configSection);
		}

		#region Initialize
		private void Initialize(RadianceSection section)
		{
			if (section != null)
			{
				this.CommandsModuleName = section.Script.CommandsModuleName;
				this.EventsModuleName = section.Script.EventsModuleName;

				this.World = Activator.CreateInstance(Type.GetType(section.World.WorldType)) as World;
				if (this.World != null)
				{
					this.World.Server = this;
					this.World.ConfigureWorld(section.World);
					CommandManager.Initialize(section.Command);
					Log.Initialize(section.Log);
					Radiance.Security.Cryptography.Initialize(section.Cryptography);

					// Load modules.
					if (section.Modules != null)
					{
						_modules.Clear();
						foreach (var element in section.Modules)
						{
							var moduleElement = (ModuleElement)element;
							var module = Activator.CreateInstance(Type.GetType(moduleElement.Type)) as IModule;
							_modules.Add(module);
						}
					}
				}
				else
				{
					throw new Exception(SR.ConfigWorldTypeNotFound(section.World.WorldType));
				}
			}
			else
			{
				throw new Exception(SR.ConfigRadianceSectionNotFound);
			}
		}
		#endregion

		#region Events
		#endregion

		#region Start/Stop
		/// <summary>
		/// Starts the server timer which performs a heartbeat at the specified interval, updating world objects, etc.
		/// </summary>
		/// <param name="heartbeatInterval">The interval, in milliseconds, between server heartbeats.</param>
		public void Start(double heartbeatInterval)
		{
			// Initialize modules.
			for (int i = 0; i < _modules.Count; i++)
			{
				// Initialize the module.
				_modules[i].Initialize();

				// If the module handles commands register the command handlers.
				if (_modules[i].HandledCommands != null)
				{
					foreach (var command in _modules[i].HandledCommands)
					{
						CommandManager.AddHandler(command, _modules[i]);
					}
				}
			}
					
			// Start the main game loop.
			_running = true;

			_serverTimer = new System.Timers.Timer(heartbeatInterval);
			_serverTimer.Elapsed += (o, e) => { this.Signal(); };

			while (_signal.WaitOne() && _running)
			{
				GameTime.Update();

				if (this.World != null)
				{
					Logger.LogDebug("SERVER: MAIN LOOP:  Starting on Thread [ {0} ] with Elapsed Time = {1}", 
						Thread.CurrentThread.ManagedThreadId, GameTime.ElapsedTime);

					// Process queued commands.
					var packets = new List<ClientPacket>();
					lock (_receivedPackets)
					{
						while (_receivedPackets.Count > 0)
						{
							packets.Add(_receivedPackets.Dequeue());
						}
					}

					Logger.LogDebug("SERVER: MAIN LOOP:  Processing Received Packets");
					for (int i = 0; i < packets.Count; i++)
					{
						packets[i].Client.Handler.ProcessCommands(this, packets[i].Commands);
					}

					// Exceute the world heartbeat
					Logger.LogDebug("SERVER: MAIN LOOP:  World Heartbeat");
					this.World.Heartbeat();

					// Update Modules
					Logger.LogDebug("SERVER: MAIN LOOP:  Updating Modules");
					for (int i = 0; i < _modules.Count; i++)
					{
						_modules[i].Update();
					}

					// Store a list of places that need to be updated.
					var placesToUpdate = new Dictionary<int, Place>();

					// Execute each connected client player's heartbeat.
					Logger.LogDebug("SERVER: MAIN LOOP:  Player Heartbeats");
					for (int i = this.Clients.Count - 1; i >= 0; i--)
					{
						if (this.Clients[i].Player != null
							&& (this.Clients[i].GetLastHeartbeatInterval() > 0.5))
						{
							var client = this.Clients[i];
							// Update the current player.
							client.Player.Heartbeat();
							client.LastHeartbeatDate = DateTime.Now;

							// Include the current player's place in the updates.
							placesToUpdate[client.Player.Place.ID] = client.Player.Place;
						}
					}

					// Update the place objects, allowing mobiles and other system controlled entities
					// to be updated.
					Logger.LogDebug("SERVER: MAIN LOOP:  Updating {0} Places", placesToUpdate.Count);
					foreach (var place in placesToUpdate.Values)
					{
						place.Update();
					}

					// Flush current clients and cleanup disconnected clients.
					Logger.LogDebug("SERVER: MAIN LOOP:  Flush Clients");
					Flush();

					// Start the timer if there are no clients connected.
					if (this.Clients.Count == 0)
						_serverTimer.Stop();

					Logger.LogDebug("SERVER: MAIN LOOP:  Completed");
				}
			}
		}


		/// <summary>
		/// Starts the server timer.
		/// </summary>
		public void Stop()
		{
			_running = false;
			if (this.World != null)
			{
				this.World.Places.Clear();
				this.World.Avatars.Clear();
			}
		}

		public void Signal()
		{
			_signal.Set();
		}

		private void Flush()
		{
			for (int i = this.Clients.Count - 1; i >= 0; i--)
			{
				var client = this.Clients[i];
				if (client.GetLastHeartbeatInterval() >= this.World.ClientTimeoutMinutes
					|| !client.Connected)
				{
					client.Expire();
					this.Clients.RemoveAt(i);
					World.Provider.RemoveSession(client);
					Logger.LogDebug("SERVER: Removing client: {0}", client.UserName);
				}
				else
				{
					this.Clients[i].Context.Flush();
				}
			}
		}
		#endregion

		#region Util Methods
		public string GetUserNameFromCommand(RdlCommand cmd)
		{
			if (cmd.Group != null)
			{
				AuthKey key = AuthKey.Get(cmd.Group.AuthKey);
				return key.UserName;
			}
			return String.Empty;
		}
		public UserDetail GetUserDetail(string username)
		{
			return this.World.Provider.GetUserDetail(username);
		}
		public void Quit(IPlayer player)
		{
			if (this.Clients.ContainsKey(player.UserName))
			{
				// Save the player.
				this.World.Provider.SaveActor<IPlayer>(player);

				// Cause the player to leave the world.
				if (player.Place != null)
				{
					player.Place.Exit(player, Direction.Empty);
				}

				// Remove the player from the list of avatars in the virtual world.
				this.World.Avatars.Remove(player.Name);

				// Reset the connected client.
				IClient client = this.Clients[player.UserName];
				if (client != null)
				{
					client.Player = null;
					client.Handler = new UserCommandHandler(client);
				}
			}
		}
		#endregion

		#region ProcessCommands
		/// <summary>
		/// Processes commands in the context of the virtual world.
		/// </summary>
		/// <param name="client">The IClient processing the request.</param>
		/// <param name="commands">The commands to process.</param>
		/// <returns>The IClient instance of the current requestor.</returns>
		public IClient ProcessCommands(IClient client, RdlCommandGroup commands)
		{
			if (!(client.Handler is LoginCommandHandler))
				client.LastCommandGroup = commands;

			bool processCommands = this.World.IsOnline;
			if (!processCommands)
			{
				processCommands = this.World.Provider.ValidateRole(client.UserName, RoleNames.God);
			}

			if (processCommands)
			{
				if (this.World.EnableCommandLogging)
				{
					if (!(client.Handler is LoginCommandHandler))
						Lionsguard.Logger.LogCommand("CMD LOG: SessionId: {0}, UserName: {1}, Commands: {2}", 
							client.SessionId, client.UserName, commands.ToString());
				}

				// TODO: Spam filter, record the commands to this client, if the same command chat
				// command is sent more than twice in a row then abandon this command group.

				// Queue commands.
				lock (_receivedPackets)
				{
					Logger.LogDebug("SERVER: Queue Commands: {0}, for Client: {1}", commands, client.UserName);
					_receivedPackets.Enqueue(new ClientPacket { Client = client, Commands = commands });
				}

				// Update client session.
				this.World.Provider.UpdateSession(client);

				this.Signal();
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(SR.WorldOffline(this.World.Name)));
				// Send down a command to refresh the web page where the client sits so
				// it will redirect to the app offline page.
				client.Context.Add(new RdlCommand("EXIT"));
			}
			return client;
		}
		/// <summary>
		/// Processes commands in the context of the virtual world.
		/// </summary>
		/// <param name="requestHandler">The ICommunicationHandler processing the request from the client.</param>
		/// <param name="commands">The commands to process.</param>
		/// <returns>The IClient instance of the current requestor.</returns>
		public IClient ProcessCommands(ICommunicationHandler requestHandler, RdlCommandGroup commands, Guid sessionId, string address)
		{
			bool addClient = false;
			IClient client = null;

			if (commands.Count > 0)
			{
				if (!String.IsNullOrEmpty(commands.AuthKey))
				{
					AuthKey key = AuthKey.Get(commands.AuthKey);
					if (this.ValidateAuthKey(key))
					{
						// Clients are stored with the UserName of the current user as the key. This
						// should prevent a user from playing multiple characters at the same time unless
						// they have multiple user names, which violates the TOS.
						if (!this.Clients.ContainsKey(key.SessionId))
						{
							client = requestHandler.CreateClient(key.SessionId);
							client.Address = address;
							client.UserName = key.UserName;
							addClient = true;
						}
						else
						{
							client = this.Clients[key.SessionId];
						}
						if (client != null)
						{
							//client.LastHeartbeatDate = DateTime.Now;
							client.AuthKey = key;
						}
					}
				}
				else
				{
					// New client will implement the LoginCommandHandler.
					client = requestHandler.CreateClient(sessionId);
					client.Address = address;
					addClient = true;
				}

				if (addClient)
				{
					this.Clients.Add(client);
					this.World.Provider.CreateSession(client, client.SessionId.ToString());
					Logger.LogDebug("SERVER: New client connected from: {0}", client.SessionId.ToString());
				}

				if (client != null)
				{
					ProcessCommands(client, commands);
				}
			}
			return client;
		}

		private bool ValidateAuthKey(AuthKey key)
		{
			return this.World.Provider.ValidateAuthKey(key);
		}
		#endregion

		#region SendAll
		/// <summary>
		/// Sends the specified Message to all connected clients.
		/// </summary>
		/// <param name="tag">The Tag to send to all connected clients.</param>
		public void SendAll(RdlTag tag, IAvatar sender)
		{
			RdlObject[] senderRdl = sender.ToSimpleRdl();
			for (int i = this.Clients.Count - 1; i >= 0; i--)
			{
				var client = this.Clients[i];
				if (client.Player != null)
				{
					if (sender != null && sender.ID == client.Player.ID)
						client.Context.Add(tag);
				}
			}
		}
		#endregion

		#region IDisposable Members

		/// <summary>
		/// Immediately releases the unmanaged resources used by this object.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases all resources used by the Server class.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				lock (this)
				{
					this.Stop();
					if (this.World != null) this.World.Dispose();
				}
			}
		}

		#endregion

		private class ClientPacket
		{
			public IClient Client { get; set; }
			public RdlCommandGroup Commands { get; set; }	
		}
	}
}
