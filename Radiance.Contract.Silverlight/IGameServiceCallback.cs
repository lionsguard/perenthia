using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Radiance.Contract
{
	public interface IGameServiceCallback
	{
		[OperationContract(IsOneWay = true, Action = Constants.GameServiceReceive)]
		void Receive(Message msg);
	}
}
