using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Configuration;
using Lionsguard;
using Radiance.Markup;
using System.ServiceModel.Channels;

namespace Perenthia.ServiceModel
{
	public class ServiceErrorHandler : IServiceBehavior, IErrorHandler
	{
		#region IServiceBehavior Members

		public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
		{
			IErrorHandler errorHandler = new ServiceErrorHandler();

			foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
			{
				ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
				channelDispatcher.ErrorHandlers.Add(errorHandler);
			} 
		}

		public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
		{
		}

		#endregion

		#region IErrorHandler Members

		public bool HandleError(Exception error)
		{
			Logger.LogError(error.ToString());
			return true;
		}

		public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
		{
			var tag = new RdlErrorMessage(error.Message);
			fault = Message.CreateMessage(version, "Fault", tag.ToBytes());
		}

		#endregion
	}

	public class ServiceErrorHandlerBehavior : BehaviorExtensionElement
	{
		public override Type BehaviorType
		{
			get { return typeof(ServiceErrorHandler); }
		}

		protected override object CreateBehavior()
		{
			return new ServiceErrorHandler();
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class ErrorHandlerAttribute : Attribute, IServiceBehavior
	{
		#region IServiceBehavior Members

		public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
		{
			IErrorHandler errorHandler = new ServiceErrorHandler();

			foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
			{
				ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
				channelDispatcher.ErrorHandlers.Add(errorHandler);
			}
		}

		public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
		{
		}

		#endregion
	}
}
