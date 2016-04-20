using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Radiance.Contract;

namespace Perenthia.Utility.ServiceModel
{
	public class DepotServiceFactory : ServiceHostFactoryBase
	{
		private static DepotService _instance = new DepotService();

		public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
		{
			var service = new ServiceHost(_instance, baseAddresses);

			var binding = new CustomBinding(
				new BinaryMessageEncodingBindingElement(),
				new HttpTransportBindingElement());

			service.Description.Behaviors.Add(new ServiceMetadataBehavior());
			service.AddServiceEndpoint(typeof(IDepotService), binding, String.Empty);
			service.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
			return service;
		}
	}
}
