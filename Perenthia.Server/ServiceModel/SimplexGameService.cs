using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radiance.Contract;
using System.ServiceModel.Activation;
using Radiance.Markup;
using System.ServiceModel;
using Radiance;

namespace Perenthia.ServiceModel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class SimplexGameService : ISimplexGameService, ICommunicationHandler
	{
		#region ISimplexGameService Members

		public string Process(string data)
		{
			var response = String.Empty;

			var client = Game.Server.ProcessCommands(this, RdlCommandGroup.FromString(data), Guid.NewGuid(), OperationContext.Current.Channel.LocalAddress.ToString());

			if (client != null)
			{
				var tags = new RdlTagCollection();
				RdlTag tag;
				while (client.Context.Read(out tag))
				{
					tags.Add(tag);
				}
				response = tags.ToString();
			}

			return response;
		}

		#endregion

		#region ICommunicationHandler Members

		public IClient CreateClient(Guid sessionId)
		{
			return new NetworkClient(sessionId);
		}

		#endregion
	}
}
