using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia.Items
{
	public static class Weapons
	{
		#region Rusty Dagger
		public static Weapon RustyDagger
		{
			get
			{
				return new Weapon("Rusty Dagger", "", AttackType.Melee, DamageValue.Small)
				{
					Skill = "Daggers",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(35),
					ImageUri = "item-weapon-dagger.png"
				};
			}
		}
		#endregion

		#region Rusty Short Sword
		public static Weapon RustyShortSword
		{
			get
			{
				return new Weapon("Rusty Short Sword", "", AttackType.Melee, DamageValue.Small)
				{
					Skill = "Swords",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(75),
					ImageUri = "item-weapon-sword.png"
				};
			}
		}
		#endregion

		#region Rusty Sword
		public static Weapon RustySword
		{
			get
			{
				return new Weapon("Rusty Sword", "", AttackType.Melee, DamageValue.Medium)
				{
					Skill = "Swords",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(150),
					ImageUri = "item-weapon-sword.png"
				};
			}
		}
		#endregion

		#region Worn Axe
		public static Weapon WornAxe
		{
			get
			{
				return new Weapon("Worn Axe", "", AttackType.Melee, DamageValue.Medium)
				{
					Skill = "Axes",
					SkillLevelRequiredToEquip = 2,
					Cost = new Currency(80),
					ImageUri = "item-weapon-axe.png"
				};
			}
		}
		#endregion

		#region Ash Shortbow
		public static Weapon AshShortbow
		{
			get
			{
				return new Weapon("Ash Shortbow", "", AttackType.Ranged, DamageValue.Medium)
				{
					Skill = "Bows",
					SkillLevelRequiredToEquip = 2,
					Cost = new Currency(150),
					ImageUri = "item-weapon-bow.png"
				};
			}
		}
		#endregion

		#region Ash Longbow
		public static Weapon AshLongbow
		{
			get
			{
				return new Weapon("Ash Longbow", "", AttackType.Ranged, DamageValue.Heavy)
				{
					Skill = "Bows",
					SkillLevelRequiredToEquip = 2,
					Cost = new Currency(3000),
					ImageUri = "item-weapon-bow.png"
				};
			}
		}
		#endregion

		#region Wooden Mace
		public static Weapon WoodenMace
		{
			get
			{
				return new Weapon("Wooden Mace", "", AttackType.Melee, DamageValue.Medium)
				{
					Skill = "Maces",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(65),
					ImageUri = "item-weapon-mace.png"
				};
			}
		}
		#endregion

		#region Wooden Club
		public static Weapon WoodenClub
		{
			get
			{
				return new Weapon("Wooden Club", "", AttackType.Melee, DamageValue.Small)
				{
					Skill = "Clubs",
					SkillLevelRequiredToEquip = 1,
					Cost = new Currency(15),
					ImageUri = "item-weapon-club.png"
				};
			}
		}
		#endregion

	}

}
