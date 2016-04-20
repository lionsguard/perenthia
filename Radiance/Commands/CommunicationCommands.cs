using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance.Commands
{
	#region Communication
	public abstract class CommunicationCommand : Command
	{
	}
	#endregion

	#region Say
	public class SayCommand : CommunicationCommand
	{
		public override void Execute(RdlCommand command, Avatar caller, IMessageContext context)
		{
			caller.Place.SendAll(new RdlChatMessage(caller.Name, String.Format(SR.MsgSayFormat, caller.Name, command.GetArg<string>(0))), caller);
		}
	}
	#endregion

	#region Shout
	public class ShoutCommand : CommunicationCommand
	{
		public override void Execute(RdlCommand command, Avatar caller, IMessageContext context)
		{
			this.Server.SendAll(new RdlChatMessage(caller.Name, String.Format(SR.MsgShoutFormat, caller.Name, command.GetArg<string>(0).ToUpper())), caller);
		}
	}
	#endregion

	#region Tell
	public class TellCommand : CommunicationCommand
	{
		public override void Execute(RdlCommand command, Avatar caller, IMessageContext context)
		{
			// Args:
			// 0 = To
			// 1 = Message
			if (command.Args.Count >= 2)
			{
				string name = command.GetArg<string>(0);
				string message = command.GetArg<string>(1);

				Avatar who = this.Server.World.FindAvatar(name);
				if (who != null)
				{
					who.Context.Add(new Radiance.Markup.RdlTellMessage(name, String.Format(SR.MsgTellFormat, caller.Name, message)));
				}
				else
				{
					context.Add(new Radiance.Markup.RdlErrorMessage(SR.MsgTellNotFound(name)));
				}
			}
			else
			{
				context.Add(new Radiance.Markup.RdlErrorMessage(SR.MsgTellNoAvatarDefined));
			}
		}
	}
	#endregion
}
