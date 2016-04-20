using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;

namespace Perenthia
{
	#region FighterSkillGroup
	public class FighterSkillGroup : SkillGroup
	{	
		public FighterSkillGroup()
		{
			this.ID = 1;
			this.Name = "Fighter";
			this.Skills.Add(new KeyValuePair<string, double>("Axes", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Bash", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Block", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Clubs", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Chain Armor", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Dodge", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Daggers", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Leather Armor", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Maces", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Parry", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Polearms", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Shields", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Swords", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Throwing Axes", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Throwing Daggers", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Plate Armor", 2));
		}
	}
	#endregion

	#region CasterSkillGroup
	public class CasterSkillGroup : SkillGroup
	{
		public CasterSkillGroup()
		{
			this.ID = 2;
			this.Name = "Caster";
			this.Skills.Add(new KeyValuePair<string, double>("Abjuration", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Alteration", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Conjuration", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Daggers", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Elementalism - Air", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Elementalism - Fire", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Elementalism - Water", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Elementalism - Earth", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Divination", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Enchantment", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Illusion", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Life", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Nature", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Necromancy", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Staves", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Summoning", 2));
		}
	}
	#endregion

	#region ThiefSkillGroup
	public class ThiefSkillGroup : SkillGroup
	{
		public ThiefSkillGroup()
		{
			this.ID = 3;
			this.Name = "Thief";
			this.Skills.Add(new KeyValuePair<string, double>("Backstab", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Block", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Clubs", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Daggers", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Disguise", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Dual Wield", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Leather Armor", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Lock Picking", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Potions", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Sneaking", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Stealing", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Swords", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Parry", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Maces", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Throwing Daggers", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Dodge", 2));
		}
	}
	#endregion

	#region ExplorerSkillGroup
	public class ExplorerSkillGroup : SkillGroup
	{
		public ExplorerSkillGroup()
		{
			this.ID = 4;
			this.Name = "Explorer";
			this.Skills.Add(new KeyValuePair<string, double>("Animal Lore", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Axes", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Bows", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Daggers", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Dodge", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Hiding", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Leather Armor", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Shields", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Riding", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Shapeshifting", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Tracking", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Trapping", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Throwing Axes", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Throwing Daggers", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Maces", 2));
			this.Skills.Add(new KeyValuePair<string, double>("Swords", 2));
		}
	}
	#endregion
}
