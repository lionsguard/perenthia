using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using Radiance.Markup;
using Radiance;
using Perenthia.ServiceModel;
using Radiance.Contract;
using System.ServiceModel;

namespace Perenthia.Utility.ServiceModel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	//[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
	public class DepotService : IDepotService
	{
		#region Map Services
		public string GetMapNames()
		{
			return Depot.GetMapNames().ToString();
		}

		public MapChunk GetMapChunk(string mapName, int startX, int startY, bool includeActors)
		{
			return Depot.GetMapChunk(mapName, startX, startY, includeActors);
		}

		public void SubmitError(string remoteHost, string errorData)
		{
			Depot.SubmitErrorData(remoteHost, errorData);
		}

		#endregion
	}
}
