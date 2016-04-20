using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using Radiance.Configuration;
using Radiance.Markup;
using Radiance.Providers;

namespace Radiance
{
	/// <summary>
	/// Provides static methods for executing and handling commands from clients.
	/// </summary>
	public static class CommandManager
	{
		private static Dictionary<string, List<ICommandHandler>> _handlers = new Dictionary<string, List<ICommandHandler>>(StringComparer.InvariantCultureIgnoreCase);

		#region Initialize
		private static CommandProvider _provider = null;
		private static CommandProviderCollection _providers = null;
		internal static void Initialize(CommandSection section)
		{
			_providers = new CommandProviderCollection();
			_provider = ProviderUtil.InstantiateProviders<CommandProvider, CommandProviderCollection>(
				section.Providers, section.DefaultProvider, _providers);
			if (_provider == null)
			{
				throw new ConfigurationErrorsException(
					SR.ConfigDefaultLogProviderNotFound,
					section.ElementInformation.Properties["defaultProvider"].Source,
					section.ElementInformation.Properties["defaultProvider"].LineNumber);
			}
		}
		#endregion

		/// <summary>
		/// Processes the specified command, providing results to the IMessageContext of the specified IClient instance.
		/// </summary>
		/// <param name="server">The current Radiance.Server instance this command is to be executed in.</param>
		/// <param name="cmd">The RdlCommand to execute.</param>
		/// <param name="client">The IClient instance executing the specified command.</param>
		public static void ProcessCommand(Server server, RdlCommand cmd, IClient client)
		{
			// Check to see if a handler exists for the current command, if not use the old provider.
			List<ICommandHandler> handlers;
			if (_handlers.TryGetValue(cmd.TypeName, out handlers))
			{
				for (int i = 0; i < handlers.Count; i++)
				{
					handlers[i].HandleCommand(server, cmd, client);
				}
			}
			else
			{
				_provider.ProcessCommand(server, cmd, client);
			}
		}

		/// <summary>
		/// Adds a new ICommandHandler instance to the collection of command handlers.
		/// </summary>
		/// <param name="commandName">The name of the command to handle.</param>
		/// <param name="handler">The ICommandHandler instance to add.</param>
		public static void AddHandler(string commandName, ICommandHandler handler)
		{
			lock (_handlers)
			{
				List<ICommandHandler> handlers;
				if (!_handlers.TryGetValue(commandName, out handlers))
					handlers = new List<ICommandHandler>();

				var existingHandler = handlers.Where(h => ReferenceEquals(h, handler)).FirstOrDefault();
				if (existingHandler == null)
					handlers.Add(handler);

				_handlers[commandName] = handlers;
			}
		}

		//public static void RemoveHandler(string commandName, ICommandHandler handler)
		//{
		//}
	}
}
