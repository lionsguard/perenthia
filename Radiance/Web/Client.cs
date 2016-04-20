using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Radiance.Markup;
using Radiance.Handlers;

namespace Radiance.Web
{
	/// <summary>
	/// Provides a client connection that uses the HTTP protocol to send and receive messages to and from the server.
	/// </summary>
	public class Client : IClient
	{
		#region IClient Members
		/// <summary>
		/// Gets the session id for the current connected client.
		/// </summary>
		public Guid SessionId { get; private set; }

		/// <summary>
		/// Gets or sets the remote address of the current client.
		/// </summary>
		public string Address { get; set; }

		public bool Connected { get { return true; } }

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

		public void Flush() { }

		#endregion	

		/// <summary>
		/// Gets a TagCollection instance used to queu messages to send the HTTP client.
		/// </summary>
		public RdlTagCollection Tags { get; private set; }

		/// <summary>
		/// Initializes a new instance of the Client class for an HTTP connected client.
		/// </summary>
		public Client()
		{
			this.Tags = new RdlTagCollection();
			this.Context = new HttpMessageContext(this.Tags);
			this.AuthKey = AuthKey.Empty;
			this.LastHeartbeatDate = DateTime.Now;
			this.Handler = new LoginCommandHandler(this);
			this.SessionId = Guid.NewGuid();
		}

		/// <summary>
		/// Writes any queued messages to the specified HttpResponse.
		/// </summary>
		/// <param name="response">The HttpResponse of the current request.</param>
		public virtual void WriteOutputToResponse(HttpResponse response)
		{
			lock (this.Tags.SyncLock)
			{
				response.Write(this.Tags.ToString());
				this.Tags.Clear();
			}
		}
	}
}
