using System;
using System.ServiceModel;

namespace Radiance.Contract
{
	[ServiceContract(Namespace = Constants.ServiceNamespace, ConfigurationName = "IGameService", CallbackContract = typeof(IGameServiceCallback))]
	public interface IGameService
	{

		[OperationContract(IsOneWay = true, AsyncPattern = true, Action = Constants.GameServiceProcess)]
		System.IAsyncResult BeginProcess(byte[] data, System.AsyncCallback callback, object asyncState);

		void EndProcess(System.IAsyncResult result);
	}
}
