using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;
using Radiance.Security;

namespace Radiance.Handlers
{
	/// <summary>
	/// Represents a handler that processes commands from a player object.
	/// </summary>
	public class PlayerCommandHandler : AuthenticatedUserCommandHandler
	{
		/// <summary>
		/// Initializes a new instance of the PlayerCommandHandler class.
		/// </summary>
		/// <param name="client">The IClient instance this handler represents.</param>
		public PlayerCommandHandler(IClient client)
			: base(client)
		{
		}

		/// <summary>
		/// Validates that the specified command can be executed by this handler.
		/// </summary>
		/// <param name="server">The current server instance.</param>
		/// <param name="commands">The commands being executed.</param>
		/// <returns>True if the commands can be executed by the current handler; otherwise false.</returns>
		protected override bool ValidateCommands(Server server, RdlCommandGroup commands)
		{
			if (this.Client.AuthKey.ID == 0)
			{
				this.Client.Handler = new UserCommandHandler(this.Client);
				this.Client.Handler.ProcessCommands(server, commands);
				return false;
			}

			return (this.Client.Player != null && this.Client.Player.UserName == this.Client.AuthKey.UserName);
		}
	}
}
