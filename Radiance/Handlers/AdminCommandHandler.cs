using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;
using Radiance.Security;

namespace Radiance.Handlers
{
	/// <summary>
	/// Represents a handler that processes admin commands from a user.
	/// </summary>
	public class AdminCommandHandler : AuthenticatedUserCommandHandler
	{
		/// <summary>
		/// Initializes a new instance of the AdminCommandHandler class.
		/// </summary>
		/// <param name="client">The IClient instance this handler represents.</param>
		public AdminCommandHandler(IClient client)
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
			return true;
		}
	}
}
