using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Radiance.Markup;

namespace Radiance
{
	public class AuthKeyMessageInspector : IDispatchMessageInspector
	{
		#region IDispatchMessageInspector Members

		public object AfterReceiveRequest(ref Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
		{
			if (!request.IsFault && !request.IsEmpty)
			{
				// TODO: Implement auth key message behavior.
				//var commands = RdlCommandGroup.FromBytes(request.GetBody<byte[]>());
				//if (commands.Count > 0)
				//{
				//    if (!String.IsNullOrEmpty(commands.AuthKey))
				//    {
				//        AuthKey key = AuthKey.Get(commands.AuthKey);
				//        Server s = new Server();
				//        if (!s.World.Provider.ValidateAuthKey(key))
				//        {
				//            throw new FaultException(Resources.Resource.AuthorizationFailed);
				//        }
				//    }
				//}
			}
			return null;
		}

		public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
		{
		}

		#endregion
	}
}
