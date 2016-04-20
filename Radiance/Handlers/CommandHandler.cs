using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance.Handlers
{
	/// <summary>
	/// Provides the abstract base class for objects that process and handle commands from clients.
	/// </summary>
	public abstract class CommandHandler
	{
		/// <summary>
		/// Gets the IClient instance of the current handler.
		/// </summary>
		public IClient Client { get; private set; }	

		/// <summary>
		/// Initializes a new instance of the CommandHandler class.
		/// </summary>
		/// <param name="client">The IClient instance this handler represents.</param>
		protected CommandHandler(IClient client)
		{
			this.Client = client;
		}

		/// <summary>
		/// Processes the commands in the RdlCommandGroup.
		/// </summary>
		/// <param name="server">The current server instance.</param>
		/// <param name="commands">The RdlCommandGroup containing the commands to process.</param>
		public abstract void ProcessCommands(Server server, RdlCommandGroup commands);
	}
}
