using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance.Handlers
{
	/// <summary>
	/// Represents a handler that processes login commands.
	/// </summary>
	public class LoginCommandHandler : CommandHandler
	{
		/// <summary>
		/// Initializes a new instance of the LoginCommandHandler class.
		/// </summary>
		/// <param name="client">The IClient instance this handler represents.</param>
		public LoginCommandHandler(IClient client)
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
			if (String.IsNullOrEmpty(commands.AuthKey))
			{
				// If authkey is null but a login command exists then process the login command.
				// Check for a login command.
				var cmds = commands.Where(c => c.TypeName == KnownCommands.Login
					|| c.TypeName == KnownCommands.Heartbeat
					|| c.TypeName == KnownCommands.ForgotPassword
					|| c.TypeName == KnownCommands.SignUp);
				if (cmds != null)
				{
					foreach (var cmd in cmds)
					{
						CommandManager.ProcessCommand(server, cmd, this.Client);
					}
					//string username = cmd.GetArg<string>(0);
					//AuthKey authKey = server.World.Provider.AuthenticateUser(username, cmd.GetArg<string>(1));
					//RdlAuthKey user = new RdlAuthKey(authKey.ToString(), authKey.Type.ToString());
					//this.Client.Context.Add(user);

					//if (!String.IsNullOrEmpty(authKey.ToString()))
					//{
					//    this.Client.Handler = new UserCommandHandler(this.Client);
					//    this.Client.UserName = username;
					//    this.Client.AuthKey = authKey;
					//    //if (!server.Clients.ContainsKey(username))
					//    //{
					//    ////    // Expire the current client.
					//    ////    server.Clients[username].Expire();
					//    ////    // Reset the current client.
					//    ////    server.Clients[username] = this.Client;
					//    ////}
					//    ////else
					//    ////{
					//    //    server.Clients.Add(this.Client);
					//    //}
					//}
				}
			}
			else
			{
				this.Client.Handler = new UserCommandHandler(this.Client);
				this.Client.Handler.ProcessCommands(server, commands);
			}
		}
	}
}
