using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Radiance;
using Radiance.Handlers;
using Radiance.Markup;
using Radiance.Contract;
using Lionsguard;
using Microsoft.ServiceModel.PollingDuplex.Scalable;
using System.Threading;

namespace Perenthia.ServiceModel
{
	/// <summary>
	/// Represents a client connected via a polling duplex WCF contract.
	/// </summary>
	public class DuplexNetworkClient : IClient
	{
		/// <summary>
		/// Initializes a new instance of the DuplexClient class.
		/// </summary>
		/// <param name="receiver">The IGameClient instance used to send data back to the connected client.</param>
		public DuplexNetworkClient(PollingDuplexSession duplexSession)
		{
			this.Context = new DuplexMessageContext(this);
			this.AuthKey = AuthKey.Empty;
			this.LastHeartbeatDate = DateTime.Now;
			this.Handler = new LoginCommandHandler(this);
			this.SessionId = new Guid(duplexSession.SessionId);
			this.Address = duplexSession.Address;
			this.Connected = true;
		}

		#region IClient Members
		/// <summary>
		/// Gets the session id for the current connected client.
		/// </summary>
		public Guid SessionId { get; private set; }

		public bool Connected { get; private set; }

		public string Address { get; set; }

		public PollingDuplexSession DuplexSession { get; set; }	

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

		public void Flush()
		{
			(this.Context as DuplexMessageContext).Flush();
		}

		public void Close()
		{
			this.Expire();
		}

		#endregion
	}
}
