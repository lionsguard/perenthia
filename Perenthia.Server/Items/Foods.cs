using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia.Items
{
	public static class Foods
	{
		public static Food Bread
		{
			get
			{
				return new Food("Bread", "", PowerType.VerySmall, PowerGroup.Heal)
				{
					Cost = new Currency(2),
					ImageUri = "item-food-pie.png"
				};
			}
		}

		public static Food Cheese
		{
			get
			{
				return new Food("Cheese", "", PowerType.Miniscule, PowerGroup.Heal)
				{
					Cost = new Currency(2),
					ImageUri = "item-food-pie.png"
				};
			}
		}

		public static Food Water
		{
			get
			{
				return new Food("Water", "", PowerType.Miniscule, PowerGroup.Heal)
				{
					Cost = new Currency(2),
					ImageUri = "item-food-ale.png"
				};
			}
		}

	}
}
