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
	public class DuplexServiceFactory<TInstance, TContract> : StaticServiceFactoryBase<TInstance, TContract> where TInstance : new()
	{
		protected override Binding CreateBinding()
		{
			return new CustomBinding(
				   new PollingDuplexBindingElement(),
				   new BinaryMessageEncodingBindingElement(),
				   new HttpTransportBindingElement());
		}
	}
}
