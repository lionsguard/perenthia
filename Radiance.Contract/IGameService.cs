using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Microsoft.ServiceModel.PollingDuplex.Scalable;

namespace Radiance.Contract
{
	[ServiceContract(Namespace = Constants.ServiceNamespace)]
	public interface IGameService : IPollingDuplex
	{
		/// <summary>
		/// Parses and executes commands on the server.
		/// </summary>
		/// <param name="data">The command data to parse and execute.</param>
		[OperationContract(IsOneWay=true)]
		void Process(byte[] data);
	}
}
