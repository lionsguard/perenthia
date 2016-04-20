using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel.Web;
using Perenthia.Utility;
using System.ServiceModel;
using Radiance.Contract;

namespace Perenthia.ServiceModel
{
	[ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
	public class PolicyService : IPolicyService
	{
		#region IPolicyService Members

		public Stream GetSilverlightPolicy()
		{
			return GetStream(Depot.ClientAccessPolicyData);
		}

		public Stream GetFlashPolicy()
		{
			return GetStream(Depot.CrossDomainPolicyData);
		}

		private Stream GetStream(string result)
		{
			WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
			return new MemoryStream(Encoding.UTF8.GetBytes(result));
		}

		#endregion
	}
}
