using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Radiance.Contract
{
	public static class ClientFactory
	{
		public static T CreateClient<T>(string serviceUri)
		{
			return new ChannelFactory<T>(CreateBinding(), CreateEndpoint(serviceUri)).CreateChannel();
		}

		private static Binding CreateBinding()
		{
			return new CustomBinding(
				new BinaryMessageEncodingBindingElement(),
				new HttpTransportBindingElement());
		}

		private static EndpointAddress CreateEndpoint(string uri)
		{
			return new EndpointAddress(uri);
		}
	}
}
