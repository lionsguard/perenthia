using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia.Items
{
	public static class Shields
	{
		#region Wooden Buckler
		public static Shield WoodenBuckler
		{
			get
			{
				return new Shield("Wooden Buckler", "", 1, 1, AttackType.Melee, DamageValue.Small)
				{
					Skill = "Shields",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(25),
					ImageUri = "item-shield-buckler.png"
				};
			}
		}
		#endregion

		#region Steel Buckler
		public static Shield SteelBuckler
		{
			get
			{
				return new Shield("Steel Buckler", "", 2, 1, AttackType.Melee, DamageValue.Small)
				{
					Skill = "Shields",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(300),
					ImageUri = "item-shield-buckler.png"
				};
			}
		}
		#endregion

		#region Wooden Round
		public static Shield WoodenRound
		{
			get
			{
				return new Shield("Wooden Round", "", 2, 1, AttackType.Melee, DamageValue.Small)
				{
					Skill = "Shields",
					SkillLevelRequiredToEquip = 2,
					Cost = new Currency(75),
					ImageUri = "item-shield-round.png"
				};
			}
		}
		#endregion

		#region Steel Round
		public static Shield SteelRound
		{
			get
			{
				return new Shield("Steel Round", "", 3, 1, AttackType.Melee, DamageValue.Small)
				{
					Skill = "Shields",
					SkillLevelRequiredToEquip = 3,
					Cost = new Currency(500),
					ImageUri = "item-shield-round.png"
				};
			}
		}
		#endregion

		#region Wooden Heater
		public static Shield WoodenHeater
		{
			get
			{
				return new Shield("Wooden Heater", "", 3, 2, AttackType.Melee, DamageValue.Medium)
				{
					Skill = "Shields",
					SkillLevelRequiredToEquip = 5,
					Cost = new Currency(150),
					ImageUri = "item-shield-heater.png"
				};
			}
		}
		#endregion

		#region Steel Heater
		public static Shield SteelHeater
		{
			get
			{
				return new Shield("Steel Heater", "", 4, 2, AttackType.Melee, DamageValue.Medium)
				{
					Skill = "Shields",
					SkillLevelRequiredToEquip = 10,
					Cost = new Currency(800),
					ImageUri = "item-shield-heater.png"
				};
			}
		}
		#endregion

		#region Wooden Tower
		public static Shield WoodenTower
		{
			get
			{
				return new Shield("Wooden Tower", "", 4, 2, AttackType.Melee, DamageValue.Medium)
				{
					Skill = "Shields",
					SkillLevelRequiredToEquip = 15,
					Cost = new Currency(3000),
					ImageUri = "item-shield-tower.png"
				};
			}
		}
		#endregion

		#region Steel Tower
		public static Shield SteelTower
		{
			get
			{
				return new Shield("Steel Tower", "", 5, 2, AttackType.Melee, DamageValue.Medium)
				{
					Skill = "Shields",
					SkillLevelRequiredToEquip = 20,
					Cost = new Currency(15000),
					ImageUri = "item-shield-tower.png"
				};
			}
		}
		#endregion

	}
}
