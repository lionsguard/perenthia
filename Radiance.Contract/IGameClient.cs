using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Radiance.Contract
{
	[ServiceContract(Namespace = Constants.ServiceNamespace)]
	public interface IGameClient
	{
		/// <summary>
		/// Begins the Receive operation asynchronously.
		/// </summary>
		/// <param name="data">The data to send to the connected client.</param>
		/// <param name="callback">The asynchronous callback method.</param>
		/// <param name="state">User state.</param>
		/// <returns>An IAsyncResult instance representing the current operation.</returns>
		[OperationContract(IsOneWay = true, AsyncPattern = true, Action= Constants.GameServiceReceive)]
		IAsyncResult BeginReceive(Message msg, AsyncCallback callback, object state);

		/// <summary>
		/// Ends the Recevie operation.
		/// </summary>
		/// <param name="result">An IAsyncResult instance representing the current operation.</param>
		void EndReceive(IAsyncResult result);
	}
}
