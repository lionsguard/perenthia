using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Radiance.Contract;
using Radiance;

namespace Perenthia
{
	public static class ServiceManager
	{
		public static GameServiceClient CreateGameServiceClient()
		{
			return new GameServiceClient(CreateDuplexBinding(), CreateEndPoint(Settings.GameService));
		}

		public static DepotServiceClient CreateDepotServiceClient()
		{
			return new DepotServiceClient(CreateBinding(), CreateEndPoint(Settings.DepotService));
		}

		public static CommunicationManager CreateDepotCommunicator()
		{
			return new CommunicationManager(CommunicationProtocol.Sockets, App.Current.Host.Source, Settings.UserAuthKey, Settings.DepotServerPort);
		}

		private static Binding CreateBinding()
		{
			return new CustomBinding(
				new BinaryMessageEncodingBindingElement(),
				new HttpTransportBindingElement());
		}

		private static Binding CreateDuplexBinding()
		{
			return new PollingDuplexHttpBinding();
			//return new CustomBinding(
			//    new PollingDuplexBindingElement(),
			//    new BinaryMessageEncodingBindingElement(),
			//    new HttpTransportBindingElement());
		}

		private static EndpointAddress CreateEndPoint(string serviceUri)
		{
			return new EndpointAddress(Settings.AppendServiceUri(serviceUri));
		}
	}
}
