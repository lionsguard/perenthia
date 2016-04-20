using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia.Items
{

	#region WoolenShirt
	public class WoolenShirt : Clothing
	{
		public WoolenShirt()
			: base("Woolen Shirt", "", 1, 0, EquipLocation.Shirt)
		{
		}
		public WoolenShirt(ColorType color)
			: base("Woolen Shirt", "", 1, 0, EquipLocation.Shirt)
		{
			this.Color = color;
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(15);
			this.ImageUri = "item-clothing-shirt.png";
		}
		#endregion
	}
	#endregion

	#region LinenShirt
	public class LinenShirt : Clothing
	{
		public LinenShirt()
			: base("Linen Shirt", "", 1, 0, EquipLocation.Shirt)
		{
		}
		public LinenShirt(ColorType color)
			: base("Linen Shirt", "", 1, 0, EquipLocation.Shirt)
		{
			this.Color = color;
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(50);
			this.ImageUri = "item-clothing-shirt.png";
		}
		#endregion
	}
	#endregion

	#region SilkenShirt
	public class SilkenShirt : Clothing
	{
		public SilkenShirt()
			: base("Silken Shirt", "", 1, 0, EquipLocation.Shirt)
		{
		}
		public SilkenShirt(ColorType color)
			: base("Silken Shirt", "", 1, 0, EquipLocation.Shirt)
		{
			this.Color = color;
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(150);
			this.ImageUri = "item-clothing-shirt.png";
		}
		#endregion
	}
	#endregion

	#region WoolenPants
	public class WoolenPants : Clothing
	{
		public WoolenPants()
			: base("Woolen Pants", "", 1, 0, EquipLocation.Pants)
		{
		}
		public WoolenPants(ColorType color)
			: base("Woolen Pants", "", 1, 0, EquipLocation.Pants)
		{
			this.Color = color;
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(20);
			this.ImageUri = "item-clothing-pants.png";
		}
		#endregion
	}
	#endregion

	#region LinenPants
	public class LinenPants : Clothing
	{
		public LinenPants()
			: base("Linen Pants", "", 1, 0, EquipLocation.Pants)
		{
		}
		public LinenPants(ColorType color)
			: base("Linen Pants", "", 1, 0, EquipLocation.Pants)
		{
			this.Color = color;
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(75);
			this.ImageUri = "item-clothing-pants.png";
		}
		#endregion
	}
	#endregion

	#region SilkenPants
	public class SilkenPants : Clothing
	{
		public SilkenPants()
			: base("Silken Pants", "", 1, 0, EquipLocation.Pants)
		{
		}
		public SilkenPants(ColorType color)
			: base("Silken Pants", "", 1, 0, EquipLocation.Pants)
		{
			this.Color = color;
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(200);
			this.ImageUri = "item-clothing-pants.png";
		}
		#endregion
	}
	#endregion

	#region WoolenPouch
	public class WoolenPouch : Container
	{
		public WoolenPouch()
			: base("Woolen Pouch", "", 2)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(800);
			this.ImageUri = "item-container-pouch.png";
		}
		#endregion
	}
	#endregion

	#region LinenPouch
	public class LinenPouch : Container
	{
		public LinenPouch()
			: base("Linen Pouch", "", 4)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(1500);
			this.ImageUri = "item-container-pouch.png";
		}
		#endregion
	}
	#endregion

	#region SilkenPouch
	public class SilkenPouch : Container
	{
		public SilkenPouch()
			: base("Silken Pouch", "", 6)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(2000);
			this.ImageUri = "item-container-pouch.png";
		}
		#endregion
	}
	#endregion

	#region LeatherPouch
	public class LeatherPouch : Container
	{
		public LeatherPouch()
			: base("Leather Pouch", "", 8)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(2500);
			this.ImageUri = "item-container-bag.png";
		}
		#endregion
	}
	#endregion

	#region WoolenBag
	public class WoolenBag : Container
	{
		public WoolenBag()
			: base("Woolen Bag", "", 6)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(1200);
			this.ImageUri = "item-container-bag.png";
		}
		#endregion
	}
	#endregion

	#region LinenBag
	public class LinenBag : Container
	{
		public LinenBag()
			: base("Linen Bag", "", 8)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(18000);
			this.ImageUri = "item-container-bag.png";
		}
		#endregion
	}
	#endregion

	#region SilkenBag
	public class SilkenBag : Container
	{
		public SilkenBag()
			: base("Silken Bag", "", 12)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(28000);
			this.ImageUri = "item-container-bag.png";
		}
		#endregion
	}
	#endregion

	#region LeatherBag
	public class LeatherBag : Container
	{
		public LeatherBag()
			: base("Leather Bag", "", 14)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(35000);
			this.ImageUri = "item-container-bag.png";
		}
		#endregion
	}
	#endregion

	#region AdventurersBackpack
	public class AdventurersBackpack : Container
	{
		public AdventurersBackpack()
			: base("Adventurer's Backpack", "", 16)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Flags = ActorFlags.NoSell;
			this.Cost = new Currency(1000000);
			this.ImageUri = "item-container-backpack.png";
		}
		#endregion
	}
	#endregion

	#region Bread
	public class Bread : Food
	{
		public Bread()
			: base("Bread", "", PowerType.VerySmall, PowerGroup.Heal)
		{
		}
		public Bread(int quantity)
			: base("Bread", "", PowerType.VerySmall, PowerGroup.Heal)
		{
			this.Quantity = quantity;
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(2);
			this.ImageUri = "item-food-pie.png";
		}
		#endregion
	}
	#endregion

	#region Cheese
	public class Cheese : Food
	{
		public Cheese()
			: base("Cheese", "", PowerType.Miniscule, PowerGroup.Heal)
		{
		}
		public Cheese(int quantity)
			: base("Cheese", "", PowerType.Miniscule, PowerGroup.Heal)
		{
			this.Quantity = quantity;
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(2);
			this.ImageUri = "item-food-pie.png";
		}
		#endregion
	}
	#endregion

	#region Water
	public class Water : Food
	{
		public Water()
			: base("Water", "", PowerType.Miniscule, PowerGroup.Heal)
		{
		}
		public Water(int quantity)
			: base("Water", "", PowerType.Miniscule, PowerGroup.Heal)
		{
			this.Quantity = quantity;
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(2);
			this.ImageUri = "item-food-ale.png";
		}
		#endregion
	}
	#endregion

	#region Candle
	public class Candle : Light
	{
		public Candle()
			: base("Candle", "", RangeType.Touch)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(25);
			this.ImageUri = "item-light-candle.png";
		}
		#endregion
	}
	#endregion

	#region Torch
	public class Torch : Light
	{
		public Torch()
			: base("Torch", "", RangeType.InSight)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(3500);
			this.ImageUri = "item-light-torch.png";
		}
		#endregion
	}
	#endregion

	#region Orb
	public class Orb : Light
	{
		public Orb()
			: base("Orb", "", RangeType.OneMile)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(10000);
			this.ImageUri = "item-light-orb.png";
		}
		#endregion
	}
	#endregion

	#region Dagger
	public class Dagger : Weapon
	{
		public Dagger()
			: base("Dagger", "", AttackType.Melee, DamageValue.Small)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(35);
			this.ImageUri = "item-weapon-dagger.png";
		}
		#endregion
	}
	#endregion

	#region ShortSword
	public class ShortSword : Weapon
	{
		public ShortSword()
			: base("Short Sword", "", AttackType.Melee, DamageValue.Small)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(75);
			this.ImageUri = "item-weapon-sword.png";
		}
		#endregion
	}
	#endregion

	#region Sword
	public class Sword : Weapon
	{
		public Sword()
			: base("Sword", "", AttackType.Melee, DamageValue.Medium)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(150);
			this.ImageUri = "item-weapon-sword.png";
		}
		#endregion
	}
	#endregion

	#region Coach
	public class Coach : Transport
	{
		#region Properties
		public string DestinationName
		{
			get { return this.Properties.GetValue<string>(DestinationNameProperty); }
			set { this.Properties.SetValue(DestinationNameProperty, value); }
		}
		public static readonly string DestinationNameProperty = "DestinationName";

		public int DestinationX
		{
			get { return this.Properties.GetValue<int>(DestinationXProperty); }
			set { this.Properties.SetValue(DestinationXProperty, value); }
		}
		public static readonly string DestinationXProperty = "DestinationX";

		public int DestinationY
		{
			get { return this.Properties.GetValue<int>(DestinationYProperty); }
			set { this.Properties.SetValue(DestinationYProperty, value); }
		}
		public static readonly string DestinationYProperty = "DestinationY";

		public int DestinationZ
		{
			get { return this.Properties.GetValue<int>(DestinationZProperty); }
			set { this.Properties.SetValue(DestinationZProperty, value); }
		}
		public static readonly string DestinationZProperty = "DestinationZ";

		#endregion
		public Coach()
			: base("Coach", "")
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(10);
			this.ImageUri = "item-transport-coach.png";
			this.DestinationName = "";
			this.DestinationX = 0;
			this.DestinationY = 0;
			this.DestinationZ = 0;
		}
		#endregion
	}
	#endregion

	#region Axe
	public class Axe : Weapon
	{
		public Axe()
			: base("Axe", "", AttackType.Melee, DamageValue.Medium)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(80);
			this.ImageUri = "item-weapon-axe.png";
		}
		#endregion
	}
	#endregion

	#region Shortbow
	public class Shortbow : Weapon
	{
		public Shortbow()
			: base("Shortbow", "", AttackType.Ranged, DamageValue.Medium)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(150);
			this.ImageUri = "item-weapon-bow.png";
		}
		#endregion
	}
	#endregion

	#region Longbow
	public class Longbow : Weapon
	{
		public Longbow()
			: base("Longbow", "", AttackType.Ranged, DamageValue.Heavy)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(3000);
			this.ImageUri = "item-weapon-bow.png";
		}
		#endregion
	}
	#endregion

	#region Mace
	public class Mace : Weapon
	{
		public Mace()
			: base("Mace", "", AttackType.Melee, DamageValue.Medium)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(65);
			this.ImageUri = "item-weapon-mace.png";
		}
		#endregion
	}
	#endregion

	#region Club
	public class Club : Weapon
	{
		public Club()
			: base("Club", "", AttackType.Melee, DamageValue.Small)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(15);
			this.ImageUri = "item-weapon-club.png";
		}
		#endregion
	}
	#endregion

	#region WoodenBuckler
	public class WoodenBuckler : Shield
	{
		public WoodenBuckler()
			: base("Wooden Buckler", "", 1, 1)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(25);
			this.ImageUri = "item-shield-buckler.png";
		}
		#endregion
	}
	#endregion

	#region SteelBuckler
	public class SteelBuckler : Shield
	{
		public SteelBuckler()
			: base("Steel Buckler", "", 2, 1)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(300);
			this.ImageUri = "item-shield-buckler.png";
		}
		#endregion
	}
	#endregion

	#region WoodenRound
	public class WoodenRound : Shield
	{
		public WoodenRound()
			: base("Wooden Round", "", 2, 1)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(75);
			this.ImageUri = "item-shield-round.png";
		}
		#endregion
	}
	#endregion

	#region SteelRound
	public class SteelRound : Shield
	{
		public SteelRound()
			: base("Steel Round", "", 3, 1)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(500);
			this.ImageUri = "item-shield-round.png";
		}
		#endregion
	}
	#endregion

	#region WoodenHeater
	public class WoodenHeater : Shield
	{
		public WoodenHeater()
			: base("Wooden Heater", "", 3, 2)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(150);
			this.ImageUri = "item-shield-heater.png";
		}
		#endregion
	}
	#endregion

	#region SteelHeater
	public class SteelHeater : Shield
	{
		public SteelHeater()
			: base("Steel Heater", "", 4, 2)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(800);
			this.ImageUri = "item-shield-heater.png";
		}
		#endregion
	}
	#endregion

	#region WoodenTower
	public class WoodenTower : Shield
	{
		public WoodenTower()
			: base("Wooden Tower", "", 4, 2)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(3000);
			this.ImageUri = "item-shield-tower.png";
		}
		#endregion
	}
	#endregion

	#region SteelTower
	public class SteelTower : Shield
	{
		public SteelTower()
			: base("Steel Tower", "", 5, 2)
		{
		}
		#region Init
		protected override void Init()
		{
			base.Init();
			this.Cost = new Currency(15000);
			this.ImageUri = "item-shield-tower.png";
		}
		#endregion
	}
	#endregion


}
