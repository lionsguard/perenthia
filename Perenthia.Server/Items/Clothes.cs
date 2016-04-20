using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia.Items
{
	#region Clothes
	public static class Clothes
	{
		public static Clothing WoolenShirt
		{
			get
			{
				return new Clothing("Woolen Shirt", "", EquipLocation.Shirt)
				{
					Cost = new Currency(15),
					ImageUri = "item-clothing-shirt.png"
				};
			}
		}

		public static Clothing LinenShirt
		{
			get
			{
				return new Clothing("Linen Shirt", "", EquipLocation.Shirt)
				{
					Cost = new Currency(50),
					ImageUri = "item-clothing-shirt.png"
				};
			}
		}

		public static Clothing SilkenShirt
		{
			get
			{
				return new Clothing("Silken Shirt", "", EquipLocation.Shirt)
				{
					Cost = new Currency(150),
					ImageUri = "item-clothing-shirt.png"
				};
			}
		}

		public static Clothing WoolenPants
		{
			get
			{
				return new Clothing("Woolen Pants", "", EquipLocation.Pants)
				{
					Cost = new Currency(20),
					ImageUri = "item-clothing-pants.png"
				};
			}
		}

		public static Clothing LinenPants
		{
			get
			{
				return new Clothing("Linen Pants", "", EquipLocation.Pants)
				{
					Cost = new Currency(75),
					ImageUri = "item-clothing-pants.png"
				};
			}
		}

		public static Clothing SilkenPants
		{
			get
			{
				return new Clothing("Silken Pants", "", EquipLocation.Pants)
				{
					Cost = new Currency(200),
					ImageUri = "item-clothing-pants.png"
				};
			}
		}

	}
	#endregion

}
