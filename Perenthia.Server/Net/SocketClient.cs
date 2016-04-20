using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radiance.Handlers;
using Radiance.Markup;
using System.Net.Sockets;
using Radiance;

namespace Perenthia.Net
{
	public class SocketClient : IClient
	{
		#region IClient Members
		/// <summary>
		/// Gets the session id for the current connected client.
		/// </summary>
		public Guid SessionId { get; private set; }

		public string Address { get; set; }	

		/// <summary>
		/// Gets a value indicating whether or not the current client is still connected.
		/// </summary>
		public bool Connected
		{
			get
			{
				if (_socket != null)
					return _socket.Connected;
				return false;
			}
		}

		/// <summary>
		/// Gets or sets the AuthKey instance for the current client.
		/// </summary>
		public AuthKey AuthKey { get; set; }

		/// <summary>
		/// Gets or sets the username of the connected user.
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// Gets or sets the date the client was last accessed using a client heartbeart.
		/// </summary>
		public DateTime LastHeartbeatDate { get; set; }

		/// <summary>
		/// Gets the message context for the current client.
		/// </summary>
		public IMessageContext Context { get; protected set; }

		/// <summary>
		/// Gets or sets the player instance represented by the current client.
		/// </summary>
		public IPlayer Player { get; set; }

		/// <summary>
		/// Gets or sets the command handler for the current client.
		/// </summary>
		public CommandHandler Handler { get; set; }

		/// <summary>
		/// Gets or sets the last command group executed by the current client.
		/// </summary>
		public RdlCommandGroup LastCommandGroup { get; set; }

		/// <summary>
		/// Gets the interval in minutes between the current time and the last heartbeat date.
		/// </summary>
		/// <returns>The interval in minutes between the current time and the last heartbeat date.</returns>
		public double GetLastHeartbeatInterval()
		{
			return DateTime.Now.Subtract(this.LastHeartbeatDate).TotalMinutes;
		}

		/// <summary>
		/// Expires the client for removal, purging the player instance and removing it from the virtual world.
		/// </summary>
		public void Expire()
		{
			this.LastHeartbeatDate = DateTime.Now.AddDays(-1);
			if (this.Player != null)
			{
				// Remove the player from the current place they reside in.
				Place place = this.Player.Place;
				if (place != null)
				{
					place.Exit(this.Player, Direction.Empty);
				}

				// Remove the player from the list of avatars in the world.
				if (this.Player.World != null)
				{
					this.Player.World.Avatars.Remove(this.Player.Name);
					this.Player.World.SaveActor(this.Player);
				}
			}
		}

		public void Flush()
		{
			(Context as SocketMessageContext).Flush();
		}

		public void Close()
		{
			if (_socket != null)
				_socket.Close();
		}

		#endregion	

		public ICommunicationHandler CommunicationHandler { get; set; }

		private Socket _socket;

		public SocketClient(Socket socket)
		{
			_socket = socket;
			this.Context = new SocketMessageContext(socket, this);
			this.AuthKey = AuthKey.Empty;
			this.LastHeartbeatDate = DateTime.Now;
			this.Handler = new LoginCommandHandler(this);
			this.SessionId = Guid.NewGuid();
		}
	}
}
