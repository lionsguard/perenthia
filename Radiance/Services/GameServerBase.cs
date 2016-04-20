using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;

using Radiance.Markup;

namespace Radiance.Services
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class GameServerBase : IGameServer
	{
        //public void Command(Message msg)
        //{
        //    this.ProcessCommands(OperationContext.Current.GetCallbackChannel<IGameClient>(), 
        //        RdlCommandGroup.FromString(msg.GetBody<string>()));
        //}

        //public virtual void ProcessCommands(IGameClient client, RdlCommandGroup commands)
        //{
        //}
        #region IGameServer Members

        public void ProcessCommand(Message message)
        {
        }

        #endregion
    }
}
