using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia.Items
{
	public static class Containers
	{
		public static Container WoolenPouch
		{
			get
			{
				return new Container("Woolen Pouch", "", 2)
				{
					Cost = new Currency(800),
					ImageUri = "item-container-pouch.png"
				};
			}
		}

		public static Container LinenPouch
		{
			get
			{
				return new Container("Linen Pouch", "", 4)
				{
					Cost = new Currency(1500),
					ImageUri = "item-container-pouch.png"
				};
			}
		}

		public static Container SilkenPouch
		{
			get
			{
				return new Container("Silken Pouch", "", 6)
				{
					Cost = new Currency(2000),
					ImageUri = "item-container-pouch.png"
				};
			}
		}

		public static Container LeatherPouch
		{
			get
			{
				return new Container("Leather Pouch", "", 8)
				{
					Cost = new Currency(2500),
					ImageUri = "item-container-bag.png"
				};
			}
		}

		public static Container WoolenBag
		{
			get
			{
				return new Container("Woolen Bag", "", 6)
				{
					Cost = new Currency(1200),
					ImageUri = "item-container-bag.png"
				};
			}
		}

		public static Container LinenBag
		{
			get
			{
				return new Container("Linen Bag", "", 8)
				{
					Cost = new Currency(18000),
					ImageUri = "item-container-bag.png"
				};
			}
		}

		public static Container SilkenBag
		{
			get
			{
				return new Container("Silken Bag", "", 12)
				{
					Cost = new Currency(28000),
					ImageUri = "item-container-bag.png"
				};
			}
		}

		public static Container LeatherBag
		{
			get
			{
				return new Container("Leather Bag", "", 14)
				{
					Cost = new Currency(35000),
					ImageUri = "item-container-bag.png"
				};
			}
		}

		public static Container AdventurersBackpack
		{
			get
			{
				return new Container("Adventurer's Backpack", "", 16)
				{
					Flags = ActorFlags.NoSell,
					Cost = new Currency(1000000),
					ImageUri = "item-container-backpack.png"
				};
			}
		}

	}
}
