using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radiance;
using Radiance.Markup;

namespace Perenthia.Combat
{
	public class CombatModule : IModule
	{
		#region IModule Members

		public string Name
		{
			get { return "Combat"; }
		}

		public void Initialize()
		{
		}

		public void Update()
		{
			// TODO: Update matches
		}

		#endregion

		#region ICommandHandler Members

		private static readonly List<string> Commands = new List<string>
		{
			"ATTACK", // Starts a combat match
			"COMBATACTION", // Performs an action during combat
		};

		public IEnumerable<string> HandledCommands
		{
			get { return Commands; }

		}

		public void HandleCommand(Server server, RdlCommand cmd, IClient client)
		{
		}

		#endregion
	}
}
