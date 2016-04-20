using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Radiance.Contract
{
	[ServiceContract(Namespace = Constants.ServiceNamespace)]
	public interface IDepotService
	{
		[OperationContract]
		string GetMapNames();

		[OperationContract]
		MapChunk GetMapChunk(string mapName, int startX, int startY, bool includeActors);

		[OperationContract]
		void SubmitError(string remoteHost, string errorData);
	}
}
