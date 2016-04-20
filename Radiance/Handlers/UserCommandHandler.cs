using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;
using Radiance.Security;

namespace Radiance.Handlers
{
	/// <summary>
	/// Represents a handler that processes user commands, not associated with a player.
	/// </summary>
	public class UserCommandHandler : AuthenticatedUserCommandHandler
	{
		/// <summary>
		/// Initializes a new instance of the UserCommandHandler class.
		/// </summary>
		/// <param name="client">The IClient instance this handler represents.</param>
		public UserCommandHandler(IClient client)
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
			if (this.Client.AuthKey.ID > 0)
			{
				this.Client.Handler = new PlayerCommandHandler(this.Client);
				this.Client.Handler.ProcessCommands(server, commands);
				return false;
			}

			RdlCommand cmd = commands.Where(c => c.TypeName == "ADMINBUILD").FirstOrDefault();
			if (cmd != null)
			{
				this.Client.Handler = new AdminCommandHandler(this.Client);
				this.Client.Handler.ProcessCommands(server, commands);
				return false;
			}

			return true;
		}
	}
}
