using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance.Commands
{
	public class LookCommand : Command
	{
		public override void Execute(RdlCommand command, Avatar caller, IMessageContext context)
		{
			context.Add(new Radiance.Markup.RdlSystemMessage(0, caller.Place.ToString()));
		}
	}
}
