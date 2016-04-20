using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

using Lionsguard.Commerce;

namespace Lionsguard.Providers
{
	public abstract class CommerceProvider : ProviderBase
	{
		public abstract List<Product> GetProducts();
		public abstract List<Product> GetActiveProducts();

		public abstract Product GetProduct(int id);

		public abstract void CreateOrder(Order order);
	}
}
