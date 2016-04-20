using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Radiance.Contract;

namespace Perenthia.ServiceModel
{
	public class DuplexServiceFactory : ServiceHostFactoryBase
	{
		private static DuplexService _instance = new DuplexService();

		public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
		{
			var service = new ServiceHost(_instance, baseAddresses);

			var binding = new CustomBinding(
				new PollingDuplexBindingElement(),
				new BinaryMessageEncodingBindingElement(),
				new HttpTransportBindingElement());

			service.Description.Behaviors.Add(new ServiceMetadataBehavior());
			service.AddServiceEndpoint(typeof(IGameService), binding, String.Empty);
			service.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
			return service;
		}
	}
}
