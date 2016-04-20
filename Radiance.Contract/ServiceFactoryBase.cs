using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Configuration;

namespace Radiance.Contract
{
	public abstract class StaticServiceFactoryBase<TInstance, TContract> : ServiceFactoryBase<TInstance, TContract> where TInstance : new()
	{
		private static TInstance _instance = new TInstance();

		protected override ServiceHost CreateHost(Uri[] baseAddresses)
		{
			return new ServiceHost(_instance, baseAddresses);
		}
	}

	public abstract class ServiceFactoryBase<TInstance, TContract> : ServiceHostFactory where TInstance : new()
	{
		protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
		{
			EnsureBaseAddresses(ref baseAddresses);

			var service = CreateHost(baseAddresses);

			var binding = CreateBinding();

			AddServiceBehaviors(service, binding);

			AddMexBehavior(service);

			return service;
		}

		public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
		{
			return this.CreateServiceHost(typeof(TInstance), baseAddresses);
		}

		protected virtual ServiceHost CreateHost(Uri[] baseAddresses)
		{
			return new ServiceHost(typeof(TInstance), baseAddresses);
		}

		protected virtual void EnsureBaseAddresses(ref Uri[] baseAddresses)
		{
			if (baseAddresses == null && baseAddresses.Length == 0)
			{
				baseAddresses = new Uri[] { new Uri(ConfigurationManager.AppSettings["BaseAddress"]) };
			}
		}

		protected virtual void AddServiceBehaviors(ServiceHost service, Binding binding)
		{
			service.Description.Behaviors.Add(new ServiceMetadataBehavior());
			service.AddServiceEndpoint(typeof(TContract), binding, String.Empty);
		}

		protected virtual void AddMexBehavior(ServiceHost service)
		{
			service.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
		}

		protected abstract Binding CreateBinding();
	}
}
