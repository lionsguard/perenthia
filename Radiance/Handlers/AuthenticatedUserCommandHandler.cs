using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;
using Radiance.Security;
using Lionsguard;

namespace Radiance.Handlers
{
	/// <summary>
	/// Represents the abstract base class for a command handler used with authenticated users.
	/// </summary>
	public abstract class AuthenticatedUserCommandHandler : CommandHandler
	{
		/// <summary>
		/// Initializes a new instance of the AuthenticatedUserCommandHandler class.
		/// </summary>
		/// <param name="client">The IClient instance this handler represents.</param>
		protected AuthenticatedUserCommandHandler(IClient client)
			: base(client)
		{
		}

		/// <summary>
		/// Processes the commands in the RdlCommandGroup.
		/// </summary>
		/// <param name="server">The current server instance.</param>
		/// <param name="commands">The RdlCommandGroup containing the commands to process.</param>
		public override void ProcessCommands(Server server, RdlCommandGroup commands)
		{
			if (this.ValidateCommands(server, commands))
			{
				int count = commands.Count;
				for (int i = 0; i < count; i++)
				{
					this.ProcessCommand(server, commands[i], this.Client.Context);
				}
			}
		}

		private void ProcessCommand(Server server, RdlCommand command, IMessageContext context)
		{
			try
			{
				// Ensure the command exits for this virtual world.
				if (String.IsNullOrEmpty(command.TypeName) || !server.World.Commands.ContainsKey(command.TypeName))
				{
					context.Add(RdlErrorMessage.InvalidCommand);
					return;
				}

				// Validate that the user has permission to execute the command.
				if (!this.ValidateRole(server, this.Client.AuthKey.UserName, server.World.Commands[command.TypeName].RequiredRole))
				{
					context.Add(new RdlErrorMessage(SR.AccessDenied(command.TypeName)));
					return;
				}

				// Execute the command.
				CommandManager.ProcessCommand(server, command, this.Client);
			}
			catch (Exception ex)
			{
#if DEBUG
				Logger.LogInformation(ex.ToString());
				throw ex;
#else
				Logger.LogError(ex.ToString());
				context.Add(RdlErrorMessage.InternalError);
#endif
			}
		}

		/// <summary>
		/// Validates that the specified command can be executed by this handler.
		/// </summary>
		/// <param name="server">The current server instance.</param>
		/// <param name="commands">The commands being executed.</param>
		/// <returns>True if the commands can be executed by the current handler; otherwise false.</returns>
		protected abstract bool ValidateCommands(Server server, RdlCommandGroup commands);

		/// <summary>
		/// Validates that the specified user has permission to execute the current command.
		/// </summary>
		/// <param name="server">The current Server instance contianing the virtual world where the command is being executed.</param>
		/// <param name="username">The username of the executing user.</param>
		/// <param name="role">The role required by the current command.</param>
		/// <returns>True if the user can execute the command; otherwise false.</returns>
		protected bool ValidateRole(Server server, string username, string role)
		{
			if (!role.Equals(RoleNames.Mortal))
			{
				return server.World.Provider.ValidateRole(username, role);
			}
			return true;
		}
	}
}
