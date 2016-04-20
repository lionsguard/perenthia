using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Configuration;

using Lionsguard.Configuration;
using Lionsguard.Commerce;
using Lionsguard.Data;
using Lionsguard.Providers;

namespace Lionsguard.Commerce
{
	[DataObject]
	public static class CommerceManager
	{
		#region Properties
		private static CommerceProvider _provider = null;
		private static CommerceProviderCollection _providers = null;

		public static CommerceProvider Provider
		{
			get
			{
				Initialize();
				return _provider;
			}
		}

		public static CommerceProviderCollection Providers
		{
			get
			{
				Initialize();
				return _providers;
			}
		}
		#endregion

		#region Initialize
		private static bool _initialized = false;
		private static object _lock = new object();
		private static Exception _initException = null;

		private static void Initialize()
		{
			if (!_initialized)
			{
				lock (_lock)
				{
					if (!_initialized)
					{
						try
						{
							CommerceSection section = ConfigurationManager.GetSection("lionsguard/commerce") as CommerceSection;
							if (section != null)
							{
								_providers = new CommerceProviderCollection();
								ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof(CommerceProvider));
								_provider = _providers[section.DefaultProvider];
								if (_provider == null)
								{
									throw new ConfigurationErrorsException("Default CommerceProvider not found in application configuration file.", section.ElementInformation.Properties["defaultProvider"].Source, section.ElementInformation.Properties["defaultProvider"].LineNumber);
								}
							}
						}
						catch (Exception ex)
						{
							_initException = ex;
						}
						_initialized = true;
					}
				}
			}
			if (_initException != null)
			{
				throw _initException;
			}
		}
		#endregion

		[DataObjectMethod(DataObjectMethodType.Select, true)]
		public static List<Product> GetProducts()
		{
			return Provider.GetActiveProducts();
		}

		public static Product GetProduct(int id)
		{
			return Provider.GetProduct(id);
		}

		public static void CreateOrder(Order order)
		{
			Provider.CreateOrder(order);
		}
	}
}
