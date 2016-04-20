using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance.Providers
{
	/// <summary>
	/// Provides the abstract base class for the Radiance virtual world command providers.
	/// </summary>
	public abstract class CommandProvider : ProviderBase
	{
		/// <summary>
		/// Processes the specified command, providing results to the IMessageContext of the specified IClient instance.
		/// </summary>
		/// <param name="server">The current Radiance.Server instance this command is to be executed in.</param>
		/// <param name="cmd">The RdlCommand to execute.</param>
		/// <param name="client">The IClient instance executing the specified command.</param>
		public abstract void ProcessCommand(Server server, RdlCommand cmd, IClient client);
	}
}
