using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

// Should be data drive rather than hard coded.
namespace Perenthia.Items
{
	#region Armors
	public static class Armors
	{
	}
	#endregion


	#region Clothes
	public static class Clothes
	{
		#region Woolen Shirt
		public static Clothing WoolenShirt
		{
			get 
			{ 
				return new Clothing("Woolen Shirt", "", EquipLocation.Shirt)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(15),
					ImageUri = "item-clothing-shirt.png",
					Durability = 0
				};
			}
		}
		#endregion

		#region Linen Shirt
		public static Clothing LinenShirt
		{
			get 
			{ 
				return new Clothing("Linen Shirt", "", EquipLocation.Shirt)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(50),
					ImageUri = "item-clothing-shirt.png",
					Durability = 0
				};
			}
		}
		#endregion

		#region Silken Shirt
		public static Clothing SilkenShirt
		{
			get 
			{ 
				return new Clothing("Silken Shirt", "", EquipLocation.Shirt)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(150),
					ImageUri = "item-clothing-shirt.png",
					Durability = 0
				};
			}
		}
		#endregion

		#region Woolen Pants
		public static Clothing WoolenPants
		{
			get 
			{ 
				return new Clothing("Woolen Pants", "", EquipLocation.Pants)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(20),
					ImageUri = "item-clothing-pants.png",
					Durability = 0
				};
			}
		}
		#endregion

		#region Linen Pants
		public static Clothing LinenPants
		{
			get 
			{ 
				return new Clothing("Linen Pants", "", EquipLocation.Pants)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(75),
					ImageUri = "item-clothing-pants.png",
					Durability = 0
				};
			}
		}
		#endregion

		#region Silken Pants
		public static Clothing SilkenPants
		{
			get 
			{ 
				return new Clothing("Silken Pants", "", EquipLocation.Pants)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(200),
					ImageUri = "item-clothing-pants.png",
					Durability = 0
				};
			}
		}
		#endregion

	}
	#endregion


	#region Containers
	public static class Containers
	{
		#region Woolen Pouch
		public static Container WoolenPouch
		{
			get 
			{ 
				return new Container("Woolen Pouch", "", 2)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(800),
					ImageUri = "item-container-pouch.png",
					Durability = 0
				};
			}
		}
		#endregion

		#region Linen Pouch
		public static Container LinenPouch
		{
			get 
			{ 
				return new Container("Linen Pouch", "", 4)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(1500),
					ImageUri = "item-container-pouch.png",
					Durability = 0
				};
			}
		}
		#endregion

		#region Silken Pouch
		public static Container SilkenPouch
		{
			get 
			{ 
				return new Container("Silken Pouch", "", 6)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(2000),
					ImageUri = "item-container-pouch.png",
					Durability = 0
				};
			}
		}
		#endregion

		#region Leather Pouch
		public static Container LeatherPouch
		{
			get 
			{ 
				return new Container("Leather Pouch", "", 8)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(2500),
					ImageUri = "item-container-bag.png",
					Durability = 0
				};
			}
		}
		#endregion

		#region Woolen Bag
		public static Container WoolenBag
		{
			get 
			{ 
				return new Container("Woolen Bag", "", 6)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(1200),
					ImageUri = "item-container-bag.png",
					Durability = 0
				};
			}
		}
		#endregion

		#region Linen Bag
		public static Container LinenBag
		{
			get 
			{ 
				return new Container("Linen Bag", "", 8)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(18000),
					ImageUri = "item-container-bag.png",
					Durability = 0
				};
			}
		}
		#endregion

		#region Silken Bag
		public static Container SilkenBag
		{
			get 
			{ 
				return new Container("Silken Bag", "", 12)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(28000),
					ImageUri = "item-container-bag.png",
					Durability = 0
				};
			}
		}
		#endregion

		#region Leather Bag
		public static Container LeatherBag
		{
			get 
			{ 
				return new Container("Leather Bag", "", 14)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(35000),
					ImageUri = "item-container-bag.png",
					Durability = 0
				};
			}
		}
		#endregion

		#region Adventurer's Backpack
		public static Container AdventurersBackpack
		{
			get 
			{ 
				return new Container("Adventurer's Backpack", "", 16)
				{ 
					Flags = ActorFlags.NoSell,
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(1000000),
					ImageUri = "item-container-backpack.png",
					Durability = 0
				};
			}
		}
		#endregion

	}
	#endregion


	#region Foods
	public static class Foods
	{
		#region Bread
		public static Food Bread
		{
			get 
			{ 
				return new Food("Bread", "", 1)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(2),
					ImageUri = "item-food-pie.png"
				};
			}
		}
		#endregion

		#region Cheese
		public static Food Cheese
		{
			get 
			{ 
				return new Food("Cheese", "", 1)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(2),
					ImageUri = "item-food-pie.png"
				};
			}
		}
		#endregion

		#region Water
		public static Food Water
		{
			get 
			{ 
				return new Food("Water", "", 1)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(2),
					ImageUri = "item-food-ale.png"
				};
			}
		}
		#endregion

	}
	#endregion


	#region Lights
	public static class Lights
	{
		#region Tallow Candle
		public static Light TallowCandle
		{
			get 
			{ 
				return new Light("Tallow Candle", "", 1)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(25),
					ImageUri = "item-light-candle.png"
				};
			}
		}
		#endregion

		#region Wax Candle
		public static Light WaxCandle
		{
			get 
			{ 
				return new Light("Wax Candle", "", 1)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(25),
					ImageUri = "item-light-candle.png"
				};
			}
		}
		#endregion

		#region Torch
		public static Light Torch
		{
			get 
			{ 
				return new Light("Torch", "", 2)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(3500),
					ImageUri = "item-light-torch.png"
				};
			}
		}
		#endregion

		#region Orb
		public static Light Orb
		{
			get 
			{ 
				return new Light("Orb", "", 3)
				{ 
					SkillLevelRequiredToEquip = 0,
					Cost = new Currency(10000),
					ImageUri = "item-light-orb.png"
				};
			}
		}
		#endregion

	}
	#endregion


	#region Spells
	public static class Spells
	{
		#region Priest Healing
		public static Spell PriestHealing
		{
			get 
			{ 
				return new HealSpell("Priest Healing", "", 12, 1)
				{ 
					Cost = new Currency(2),
					ImageUri = "item-spell-heal.png"
				};
			}
		}
		#endregion

		#region Minor Healing
		public static Spell MinorHealing
		{
			get 
			{ 
				return new HealSpell("Minor Healing", "", 4, 1)
				{ 
					Skill = "Life",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(30),
					ImageUri = "item-spell-heal.png"
				};
			}
		}
		#endregion

		#region Spirit of the Bear
		public static Spell SpiritoftheBear
		{
			get 
			{ 
				return new StrengthSpell("Spirit of the Bear", "", 6, 1)
				{ 
					Skill = "Life",
					SkillLevelRequiredToEquip = 4,
					Cost = new Currency(100),
					ImageUri = "item-spell-strength.png"
				};
			}
		}
		#endregion

		#region Renew Health
		public static Spell RenewHealth
		{
			get 
			{ 
				return new HealSpell("Renew Health", "", 8, 1)
				{ 
					Skill = "Life",
					SkillLevelRequiredToEquip = 8,
					Cost = new Currency(500),
					ImageUri = "item-spell-heal.png"
				};
			}
		}
		#endregion

		#region Spirit of the Wolf
		public static Spell SpiritoftheWolf
		{
			get 
			{ 
				return new StaminaSpell("Spirit of the Wolf", "", 8, 1)
				{ 
					Skill = "Life",
					SkillLevelRequiredToEquip = 12,
					Cost = new Currency(1500),
					ImageUri = "item-spell-stamina.png"
				};
			}
		}
		#endregion

		#region Spirit of the Tiger
		public static Spell SpiritoftheTiger
		{
			get 
			{ 
				return new StrengthSpell("Spirit of the Tiger", "", 10, 1)
				{ 
					Skill = "Life",
					SkillLevelRequiredToEquip = 16,
					Cost = new Currency(3500),
					ImageUri = "item-spell-strength.png"
				};
			}
		}
		#endregion

		#region Spirit of the Eagle
		public static Spell SpiritoftheEagle
		{
			get 
			{ 
				return new PerceptionSpell("Spirit of the Eagle", "", 12, 2)
				{ 
					Skill = "Life",
					SkillLevelRequiredToEquip = 20,
					Cost = new Currency(5500),
					ImageUri = "item-spell-perception.png"
				};
			}
		}
		#endregion

		#region Major Healing
		public static Spell MajorHealing
		{
			get 
			{ 
				return new HealSpell("Major Healing", "", 16, 1)
				{ 
					Skill = "Life",
					SkillLevelRequiredToEquip = 24,
					Cost = new Currency(8500),
					ImageUri = "item-spell-heal.png"
				};
			}
		}
		#endregion

		#region Spirit of the Cheetah
		public static Spell SpiritoftheCheetah
		{
			get 
			{ 
				return new DexteritySpell("Spirit of the Cheetah", "", 24, 1)
				{ 
					Skill = "Life",
					SkillLevelRequiredToEquip = 52,
					Cost = new Currency(545000),
					ImageUri = "item-spell-dexterity.png"
				};
			}
		}
		#endregion

		#region Divine Light
		public static Spell DivineLight
		{
			get 
			{ 
				return new HealSpell("Divine Light", "", 120, 2)
				{ 
					Skill = "Life",
					SkillLevelRequiredToEquip = 80,
					EmblemCost = 500,
					ImageUri = "item-spell-heal.png"
				};
			}
		}
		#endregion

		#region Dead Sight
		public static Spell DeadSight
		{
			get 
			{ 
				return new SightSpell("Dead Sight", "", 4, 2)
				{ 
					Skill = "Necromancy",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(40),
					ImageUri = "item-spell-sight.png"
				};
			}
		}
		#endregion

		#region Earth Shield
		public static Spell EarthShield
		{
			get 
			{ 
				return new ProtectionSpell("Earth Shield", "", 8, 1)
				{ 
					Skill = "Elementalism - Earth",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(50),
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Earth Armor
		public static Spell EarthArmor
		{
			get 
			{ 
				return new ProtectionSpell("Earth Armor", "", 8, 1)
				{ 
					Skill = "Elementalism - Earth",
					SkillLevelRequiredToEquip = 4,
					Cost = new Currency(100),
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Earth Aura
		public static Spell EarthAura
		{
			get 
			{ 
				return new ProtectionSpell("Earth Aura", "", 8, 1)
				{ 
					Skill = "Elementalism - Earth",
					SkillLevelRequiredToEquip = 8,
					Cost = new Currency(200),
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Rockslide
		public static Spell Rockslide
		{
			get 
			{ 
				return new DamageSpell("Rockslide", "", 6, 1)
				{ 
					Skill = "Elementalism - Earth",
					SkillLevelRequiredToEquip = 12,
					Cost = new Currency(500),
					ImageUri = "item-spell-damage.png"
				};
			}
		}
		#endregion

		#region Earthquake
		public static Spell Earthquake
		{
			get 
			{ 
				return new DamageSpell("Earthquake", "", 12, 1)
				{ 
					Skill = "Elementalism - Earth",
					SkillLevelRequiredToEquip = 16,
					Cost = new Currency(1000),
					ImageUri = "item-spell-damage.png"
				};
			}
		}
		#endregion

		#region Crater
		public static Spell Crater
		{
			get 
			{ 
				return new DamageSpell("Crater", "", 20, 1)
				{ 
					Skill = "Elementalism - Earth",
					SkillLevelRequiredToEquip = 20,
					Cost = new Currency(5000),
					ImageUri = "item-spell-damage.png"
				};
			}
		}
		#endregion

		#region Air Shield
		public static Spell AirShield
		{
			get 
			{ 
				return new ProtectionSpell("Air Shield", "", 8, 1)
				{ 
					Skill = "Elementalism - Air",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(50),
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Air Armor
		public static Spell AirArmor
		{
			get 
			{ 
				return new ProtectionSpell("Air Armor", "", 8, 1)
				{ 
					Skill = "Elementalism - Air",
					SkillLevelRequiredToEquip = 4,
					Cost = new Currency(100),
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Air Aura
		public static Spell AirAura
		{
			get 
			{ 
				return new ProtectionSpell("Air Aura", "", 8, 1)
				{ 
					Skill = "Elementalism - Air",
					SkillLevelRequiredToEquip = 8,
					Cost = new Currency(200),
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Whirlwind
		public static Spell Whirlwind
		{
			get 
			{ 
				return new DamageSpell("Whirlwind", "", 6, 1)
				{ 
					Skill = "Elementalism - Air",
					SkillLevelRequiredToEquip = 12,
					Cost = new Currency(500),
					ImageUri = "item-spell-damage.png"
				};
			}
		}
		#endregion

		#region Gale Winds
		public static Spell GaleWinds
		{
			get 
			{ 
				return new DamageSpell("Gale Winds", "", 12, 1)
				{ 
					Skill = "Elementalism - Air",
					SkillLevelRequiredToEquip = 16,
					Cost = new Currency(1000),
					ImageUri = "item-spell-damage.png"
				};
			}
		}
		#endregion

		#region Cyclone
		public static Spell Cyclone
		{
			get 
			{ 
				return new DamageSpell("Cyclone", "", 20, 1)
				{ 
					Skill = "Elementalism - Air",
					SkillLevelRequiredToEquip = 20,
					Cost = new Currency(5000),
					ImageUri = "item-spell-damage.png"
				};
			}
		}
		#endregion

		#region Water Shield
		public static Spell WaterShield
		{
			get 
			{ 
				return new ProtectionSpell("Water Shield", "", 8, 1)
				{ 
					Skill = "Elementalism - Water",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(50),
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Water Armor
		public static Spell WaterArmor
		{
			get 
			{ 
				return new ProtectionSpell("Water Armor", "", 8, 1)
				{ 
					Skill = "Elementalism - Water",
					SkillLevelRequiredToEquip = 4,
					Cost = new Currency(100),
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Water Aura
		public static Spell WaterAura
		{
			get 
			{ 
				return new ProtectionSpell("Water Aura", "", 8, 1)
				{ 
					Skill = "Elementalism - Water",
					SkillLevelRequiredToEquip = 8,
					Cost = new Currency(200),
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Flood
		public static Spell Flood
		{
			get 
			{ 
				return new DamageSpell("Flood", "", 6, 1)
				{ 
					Skill = "Elementalism - Water",
					SkillLevelRequiredToEquip = 12,
					Cost = new Currency(500),
					ImageUri = "item-spell-damage.png"
				};
			}
		}
		#endregion

		#region Thunderstorm
		public static Spell Thunderstorm
		{
			get 
			{ 
				return new DamageSpell("Thunderstorm", "", 12, 1)
				{ 
					Skill = "Elementalism - Water",
					SkillLevelRequiredToEquip = 16,
					Cost = new Currency(1000),
					ImageUri = "item-spell-damage.png"
				};
			}
		}
		#endregion

		#region Typhoon
		public static Spell Typhoon
		{
			get 
			{ 
				return new DamageSpell("Typhoon", "", 20, 1)
				{ 
					Skill = "Elementalism - Water",
					SkillLevelRequiredToEquip = 20,
					Cost = new Currency(5000),
					ImageUri = "item-spell-damage.png"
				};
			}
		}
		#endregion

		#region Fire Shield
		public static Spell FireShield
		{
			get 
			{ 
				return new ProtectionSpell("Fire Shield", "", 8, 1)
				{ 
					Skill = "Elementalism - Fire",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(50),
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Fire Armor
		public static Spell FireArmor
		{
			get 
			{ 
				return new ProtectionSpell("Fire Armor", "", 8, 1)
				{ 
					Skill = "Elementalism - Fire",
					SkillLevelRequiredToEquip = 4,
					Cost = new Currency(100),
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Fire Aura
		public static Spell FireAura
		{
			get 
			{ 
				return new ProtectionSpell("Fire Aura", "", 8, 1)
				{ 
					Skill = "Elementalism - Fire",
					SkillLevelRequiredToEquip = 8,
					Cost = new Currency(200),
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Firebolt
		public static Spell Firebolt
		{
			get 
			{ 
				return new DamageSpell("Firebolt", "", 6, 1)
				{ 
					Skill = "Elementalism - Fire",
					SkillLevelRequiredToEquip = 12,
					Cost = new Currency(500),
					ImageUri = "item-spell-damage.png"
				};
			}
		}
		#endregion

		#region Fireball
		public static Spell Fireball
		{
			get 
			{ 
				return new DamageSpell("Fireball", "", 12, 1)
				{ 
					Skill = "Elementalism - Fire",
					SkillLevelRequiredToEquip = 16,
					Cost = new Currency(1000),
					ImageUri = "item-spell-damage.png"
				};
			}
		}
		#endregion

		#region Rain of Fire
		public static Spell RainofFire
		{
			get 
			{ 
				return new DamageSpell("Rain of Fire", "", 20, 1)
				{ 
					Skill = "Elementalism - Fire",
					SkillLevelRequiredToEquip = 20,
					Cost = new Currency(5000),
					ImageUri = "item-spell-damage.png"
				};
			}
		}
		#endregion

		#region Minor Shield
		public static Spell MinorShield
		{
			get 
			{ 
				return new ProtectionSpell("Minor Shield", "", 4, 1)
				{ 
					Skill = "Abjuration",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(50),
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Minor Armor
		public static Spell MinorArmor
		{
			get 
			{ 
				return new ProtectionSpell("Minor Armor", "", 8, 1)
				{ 
					Skill = "Abjuration",
					SkillLevelRequiredToEquip = 20,
					Cost = new Currency(8500),
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Major Shield
		public static Spell MajorShield
		{
			get 
			{ 
				return new ProtectionSpell("Major Shield", "", 12, 1)
				{ 
					Skill = "Abjuration",
					SkillLevelRequiredToEquip = 36,
					Cost = new Currency(85000),
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Major Armor
		public static Spell MajorArmor
		{
			get 
			{ 
				return new ProtectionSpell("Major Armor", "", 16, 1)
				{ 
					Skill = "Abjuration",
					SkillLevelRequiredToEquip = 56,
					Cost = new Currency(360000),
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Spirit Shield
		public static Spell SpiritShield
		{
			get 
			{ 
				return new ProtectionSpell("Spirit Shield", "", 24, 1)
				{ 
					Skill = "Abjuration",
					SkillLevelRequiredToEquip = 80,
					EmblemCost = 200,
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Spirit Armor
		public static Spell SpiritArmor
		{
			get 
			{ 
				return new ProtectionSpell("Spirit Armor", "", 45, 1)
				{ 
					Skill = "Abjuration",
					SkillLevelRequiredToEquip = 100,
					EmblemCost = 500,
					ImageUri = "item-spell-protection.png"
				};
			}
		}
		#endregion

		#region Create Bread
		public static Spell CreateBread
		{
			get 
			{ 
				return new CreateItemSpell("Create Bread", "", 2, 1)
				{ 
					Skill = "Conjuration",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(30),
					ImageUri = "item-spell-inventory.png"
				};
			}
		}
		#endregion

		#region Create Water
		public static Spell CreateWater
		{
			get 
			{ 
				return new CreateItemSpell("Create Water", "", 2, 1)
				{ 
					Skill = "Conjuration",
					SkillLevelRequiredToEquip = 4,
					Cost = new Currency(30),
					ImageUri = "item-spell-inventory.png"
				};
			}
		}
		#endregion

		#region Minor Gate
		public static Spell MinorGate
		{
			get 
			{ 
				return new TravelSpell("Minor Gate", "", 8, 1)
				{ 
					Skill = "Summoning",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(45),
					ImageUri = "item-spell-travel.png"
				};
			}
		}
		#endregion

		#region Temple Gate
		public static Spell TempleGate
		{
			get 
			{ 
				return new TravelSpell("Temple Gate", "", 12, 1)
				{ 
					Skill = "Summoning",
					SkillLevelRequiredToEquip = 4,
					Cost = new Currency(150),
					ImageUri = "item-spell-travel.png"
				};
			}
		}
		#endregion

		#region Major Gate
		public static Spell MajorGate
		{
			get 
			{ 
				return new TravelSpell("Major Gate", "", 20, 2)
				{ 
					Skill = "Summoning",
					SkillLevelRequiredToEquip = 24,
					Cost = new Currency(45000),
					ImageUri = "item-spell-travel.png"
				};
			}
		}
		#endregion

		#region Enchant Weapon
		public static Spell EnchantWeapon
		{
			get 
			{ 
				return new EnchantWeaponSpell("Enchant Weapon", "", 4, 1)
				{ 
					Skill = "Enchantment",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(30),
					ImageUri = "item-spell-weapon.png"
				};
			}
		}
		#endregion

		#region Enchant Pendant
		public static Spell EnchantPendant
		{
			get 
			{ 
				return new EnchantPendantSpell("Enchant Pendant", "", 6, 1)
				{ 
					Skill = "Enchantment",
					SkillLevelRequiredToEquip = 4,
					Cost = new Currency(150),
					ImageUri = "item-spell-pendant.png"
				};
			}
		}
		#endregion

		#region Enchant Ring
		public static Spell EnchantRing
		{
			get 
			{ 
				return new EnchantRingSpell("Enchant Ring", "", 12, 1)
				{ 
					Skill = "Enchantment",
					SkillLevelRequiredToEquip = 8,
					Cost = new Currency(250),
					ImageUri = "item-spell-ring.png"
				};
			}
		}
		#endregion

		#region Enchant Shield
		public static Spell EnchantShield
		{
			get 
			{ 
				return new EnchantShieldSpell("Enchant Shield", "", 16, 1)
				{ 
					Skill = "Enchantment",
					SkillLevelRequiredToEquip = 12,
					Cost = new Currency(3000),
					ImageUri = "item-spell-shield.png"
				};
			}
		}
		#endregion

		#region Enchant Armor
		public static Spell EnchantArmor
		{
			get 
			{ 
				return new EnchantArmorSpell("Enchant Armor", "", 16, 1)
				{ 
					Skill = "Enchantment",
					SkillLevelRequiredToEquip = 16,
					Cost = new Currency(7500),
					ImageUri = "item-spell-armor.png"
				};
			}
		}
		#endregion

	}
	#endregion


	#region Weapons
	public static class Weapons
	{
		#region Rusty Dagger
		public static Weapon RustyDagger
		{
			get 
			{ 
				return new Weapon("Rusty Dagger", "", 2, 1)
				{ 
					Skill = "Daggers",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(35),
					ImageUri = "item-weapon-dagger.png",
					Durability = 25
				};
			}
		}
		#endregion

		#region Rusty Short Sword
		public static Weapon RustyShortSword
		{
			get 
			{ 
				return new Weapon("Rusty Short Sword", "", 2, 1)
				{ 
					Skill = "Swords",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(75),
					ImageUri = "item-weapon-sword.png",
					Durability = 50
				};
			}
		}
		#endregion

		#region Rusty Sword
		public static Weapon RustySword
		{
			get 
			{ 
				return new Weapon("Rusty Sword", "", 3, 1)
				{ 
					Skill = "Swords",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(150),
					ImageUri = "item-weapon-sword.png",
					Durability = 150
				};
			}
		}
		#endregion

		#region Worn Axe
		public static Weapon WornAxe
		{
			get 
			{ 
				return new Weapon("Worn Axe", "", 3, 1)
				{ 
					Skill = "Axes",
					SkillLevelRequiredToEquip = 2,
					Cost = new Currency(80),
					ImageUri = "item-weapon-axe.png",
					Durability = 125
				};
			}
		}
		#endregion

		#region Ash Shortbow
		public static Weapon AshShortbow
		{
			get 
			{ 
				return new Weapon("Ash Shortbow", "", 3, 2)
				{ 
					Skill = "Bows",
					SkillLevelRequiredToEquip = 2,
					Cost = new Currency(150),
					ImageUri = "item-weapon-bow.png",
					Durability = 200
				};
			}
		}
		#endregion

		#region Ash Longbow
		public static Weapon AshLongbow
		{
			get 
			{ 
				return new Weapon("Ash Longbow", "", 4, 3)
				{ 
					Skill = "Bows",
					SkillLevelRequiredToEquip = 2,
					Cost = new Currency(3000),
					ImageUri = "item-weapon-bow.png",
					Durability = 400
				};
			}
		}
		#endregion

		#region Wooden Mace
		public static Weapon WoodenMace
		{
			get 
			{ 
				return new Weapon("Wooden Mace", "", 3, 1)
				{ 
					Skill = "Maces",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(65),
					ImageUri = "item-weapon-mace.png",
					Durability = 35
				};
			}
		}
		#endregion

		#region Wooden Club
		public static Weapon WoodenClub
		{
			get 
			{ 
				return new Weapon("Wooden Club", "", 2, 1)
				{ 
					Skill = "Clubs",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(15),
					ImageUri = "item-weapon-club.png",
					Durability = 15
				};
			}
		}
		#endregion

		#region Fists
		public static Weapon Fists
		{
			get 
			{ 
				return new Weapon("Fists", "", 2, 1)
				{ 
					Skill = "Hand To Hand",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(0),
					ImageUri = "item-weapon-fists.png",
					Durability = 0
				};
			}
		}
		#endregion

	}
	#endregion


	#region Shields
	public static class Shields
	{
		#region Wooden Buckler
		public static Shield WoodenBuckler
		{
			get 
			{ 
				return new Shield("Wooden Buckler", "", 2, 1, 1)
				{ 
					Skill = "Shields",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(25),
					ImageUri = "item-shield-buckler.png",
					Durability = 20
				};
			}
		}
		#endregion

		#region Steel Buckler
		public static Shield SteelBuckler
		{
			get 
			{ 
				return new Shield("Steel Buckler", "", 4, 2, 1)
				{ 
					Skill = "Shields",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(300),
					ImageUri = "item-shield-buckler.png",
					Durability = 40
				};
			}
		}
		#endregion

		#region Wooden Round
		public static Shield WoodenRound
		{
			get 
			{ 
				return new Shield("Wooden Round", "", 6, 2, 1)
				{ 
					Skill = "Shields",
					SkillLevelRequiredToEquip = 2,
					Cost = new Currency(75),
					ImageUri = "item-shield-round.png",
					Durability = 80
				};
			}
		}
		#endregion

		#region Steel Round
		public static Shield SteelRound
		{
			get 
			{ 
				return new Shield("Steel Round", "", 8, 3, 1)
				{ 
					Skill = "Shields",
					SkillLevelRequiredToEquip = 3,
					Cost = new Currency(500),
					ImageUri = "item-shield-round.png",
					Durability = 160
				};
			}
		}
		#endregion

		#region Wooden Heater
		public static Shield WoodenHeater
		{
			get 
			{ 
				return new Shield("Wooden Heater", "", 10, 3, 1)
				{ 
					Skill = "Shields",
					SkillLevelRequiredToEquip = 5,
					Cost = new Currency(150),
					ImageUri = "item-shield-heater.png",
					Durability = 320
				};
			}
		}
		#endregion

		#region Steel Heater
		public static Shield SteelHeater
		{
			get 
			{ 
				return new Shield("Steel Heater", "", 12, 4, 1)
				{ 
					Skill = "Shields",
					SkillLevelRequiredToEquip = 10,
					Cost = new Currency(800),
					ImageUri = "item-shield-heater.png",
					Durability = 640
				};
			}
		}
		#endregion

		#region Wooden Tower
		public static Shield WoodenTower
		{
			get 
			{ 
				return new Shield("Wooden Tower", "", 14, 4, 1)
				{ 
					Skill = "Shields",
					SkillLevelRequiredToEquip = 15,
					Cost = new Currency(3000),
					ImageUri = "item-shield-tower.png",
					Durability = 1280
				};
			}
		}
		#endregion

		#region Steel Tower
		public static Shield SteelTower
		{
			get 
			{ 
				return new Shield("Steel Tower", "", 16, 5, 1)
				{ 
					Skill = "Shields",
					SkillLevelRequiredToEquip = 20,
					Cost = new Currency(15000),
					ImageUri = "item-shield-tower.png",
					Durability = 2560
				};
			}
		}
		#endregion

	}
	#endregion


	#region Transports
	public static class Transports
	{
		#region Coach
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
		#endregion

	}
	#endregion


	#region RepairKits
	public static class RepairKits
	{
	}
	#endregion


	#region Artifacts
	public static class Artifacts
	{
	}
	#endregion


	#region Recipes
	public static class Recipes
	{
	}
	#endregion


}

