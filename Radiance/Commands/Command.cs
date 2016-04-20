using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance.Commands
{
	/// <summary>
	/// Provides the abstract base class for all game commands.
	/// </summary>
	public abstract class Command
	{
		/// <summary>
		/// Gets the current Server instance.
		/// </summary>
		public Server Server { get; internal set; }	

		/// <summary>
		/// Executes the current command using the specified args and outputing the results to the specified context.
		/// </summary>
		/// <param name="command">The RdlCommand to execute.</param>
		/// <param name="caller">The Avatar executing the command.</param>
		/// <param name="context">The IMessageContext for handling command output responses.</param>
		public abstract void Execute(RdlCommand command, Avatar caller, IMessageContext context);
	}
}
