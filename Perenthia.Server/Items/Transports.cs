using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia.Items
{
	public static class Transports
	{
		public static Transport Coach
		{
			get
			{
				return new Transport("Coach", "")
				{
					Cost = new Currency(10),
					ImageUri = "item-transport-coach.png"
				};
			}
		}

	}
}
