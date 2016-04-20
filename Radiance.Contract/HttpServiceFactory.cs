using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Radiance.Contract
{
	public class HttpStaticServiceFactory<TInstance, TContract> : StaticServiceFactoryBase<TInstance, TContract> where TInstance : new()
	{
		protected override Binding CreateBinding()
		{
			return new CustomBinding(
				   new BinaryMessageEncodingBindingElement(),
				   new HttpTransportBindingElement());
		}
	}

	public class HttpServiceFactory<TInstance, TContract> : ServiceFactoryBase<TInstance, TContract> where TInstance : new()
	{
		protected override Binding CreateBinding()
		{
			return new CustomBinding(
				   new BinaryMessageEncodingBindingElement(),
				   new HttpTransportBindingElement());
		}
	}

	public class WebServiceFactory<TInstance, TContract> : ServiceFactoryBase<TInstance, TContract> where TInstance : new()
	{
		protected override Binding CreateBinding()
		{
			return new WebHttpBinding();
		}

		protected override void AddServiceBehaviors(ServiceHost service, Binding binding)
		{
			service.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
			var ep = service.AddServiceEndpoint(typeof(TContract), binding, String.Empty);
			ep.Behaviors.Add(new WebHttpBehavior());
		}
	}
}
