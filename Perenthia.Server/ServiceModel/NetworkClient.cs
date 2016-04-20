using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radiance;
using Radiance.Handlers;
using Radiance.Markup;
using Lionsguard;

namespace Perenthia.ServiceModel
{
	public class NetworkClient : IClient
	{
		/// <summary>
		/// Gets the session id for the current connected client.
		/// </summary>
		public Guid SessionId { get; private set; }

		/// <summary>
		/// Gets or sets the remote address of the current client.
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// Gets a value indicating whether or not the current client is still connected.
		/// </summary>
		public bool Connected { get; private set; }

		/// <summary>
		/// In a derived class, gets or sets the AuthKey of the current client.
		/// </summary>
		public AuthKey AuthKey { get; set; }

		/// <summary>
		/// Gets or sets the authenticated user associated with the current connection.
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// Gets the DateTime this client instance was last accessed.
		/// </summary>
		public DateTime LastHeartbeatDate { get; set; }

		/// <summary>
		/// Gets the messaging context or the current client.
		/// </summary>
		public IMessageContext Context { get; private set; }

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
		/// Initializes a new instance of the NetworkClient class.
		/// </summary>
		public NetworkClient(Guid sessionId)
		{
			this.Context = new NetworkMessageContext();
			this.AuthKey = AuthKey.Empty;
			this.LastHeartbeatDate = DateTime.Now;
			this.Handler = new LoginCommandHandler(this);
			this.SessionId = sessionId;
			this.Connected = true;
		}

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
			Logger.LogDebug("SERVER: Expiring client {0}, LastHeartbeatDate = {1}",
				this.SessionId.ToString(), this.LastHeartbeatDate);
			this.Connected = false;
			this.LastHeartbeatDate = DateTime.Now.Subtract(TimeSpan.FromDays(1));
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
	}
}
