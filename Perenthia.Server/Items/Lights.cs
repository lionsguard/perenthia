using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia.Items
{
	public static class Lights
	{
		public static Light TallowCandle
		{
			get
			{
				return new Light("Tallow Candle", "", RangeType.Touch)
				{
					Cost = new Currency(25),
					ImageUri = "item-light-candle.png"
				};
			}
		}

		public static Light WaxCandle
		{
			get
			{
				return new Light("Wax Candle", "", RangeType.Touch)
				{
					Cost = new Currency(25),
					ImageUri = "item-light-candle.png"
				};
			}
		}

		public static Light Torch
		{
			get
			{
				return new Light("Torch", "", RangeType.InSight)
				{
					Cost = new Currency(3500),
					ImageUri = "item-light-torch.png"
				};
			}
		}

		public static Light Orb
		{
			get
			{
				return new Light("Orb", "", RangeType.OneMile)
				{
					Cost = new Currency(10000),
					ImageUri = "item-light-orb.png"
				};
			}
		}

	}
}
