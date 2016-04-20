using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;
using Radiance.Combat;

namespace Radiance
{
	/// <summary>
	/// Provides static methods and properties for handling combat among Avatars.
	/// </summary>
	public static class CombatManager
	{
		private static Dictionary<Guid, CombatMatch> _matches = new Dictionary<Guid, CombatMatch>();

		/// <summary>
		/// Begins a combat match between the attackers and defenders.
		/// </summary>
		/// <param name="attackers">The first collection of combatants.</param>
		/// <param name="defenders">The second collection of combatants.</param>
		public static void BeginMatch(IAvatar[] attackers, IAvatar[] defenders)
		{
			var match = new CombatMatch(attackers, defenders);
			match.Completed += (e) =>
				{
					lock (_matches)
					{
						_matches.Remove(e.ID);
					}
				};
			lock (_matches)
			{
				_matches.Add(match.ID, match);
			}
			match.Start();
		}

		/// <summary>
		/// Updates the CombatManager, performing combat rounds for any current matches.
		/// </summary>
		public static void Update()
		{
			lock (_matches)
			{
				foreach (var match in _matches.Values)
				{
					match.Update();
				}
			}
		}

		/// <summary>
		/// Determines and sets the initiative of each of the specified avatars.
		/// </summary>
		/// <param name="initiativeType">The InitiativeType for this session of combat.</param>
		/// <param name="participants">The list of avatars participating in combat.</param>
		public static void DetermineInitiative(InitiativeType initiativeType, params IAvatar[] participants)
		{
			if (participants != null && participants.Length > 0)
			{
				for (int i = 0; i < participants.Length; i++)
				{
					switch (initiativeType)
					{	
						case InitiativeType.Individual:
						case InitiativeType.Cyclic:
							participants[i].Initiative = Dice.Roll(1, 10);
							break;
						default:
							participants[i].Initiative = i;
							break;
					}
				}
			}
		}

		/// <summary>
		/// Performs a simple combat turn.
		/// </summary>
		/// <param name="attacker">The attacking avatar.</param>
		/// <param name="attackerWeapon">The weapon to be used during the attack.</param>
		/// <param name="defender">The defending avatar.</param>
		/// <param name="defensiveSkill">The skill used to defender against the attack.</param>
		/// <param name="defenderAttributeType">The attribute used to defend.</param>
		/// <param name="range">The range from the attacker to the defender.</param>
		/// <returns>True if the defender was killed during the attack; otherwise false.</returns>
        public static bool PerformSimpleCombatTurn(
            IAvatar attacker,
            IWeapon attackerWeapon,
            IAvatar defender,
            string defensiveSkill,
            AttributeType defenderAttributeType,
            int range)
        {
            return PerformSimpleCombatTurn(attacker, attackerWeapon, attackerWeapon.Skill, defender, defensiveSkill, defenderAttributeType, range);
        }

		/// <summary>
		/// Performs a simple combat turn.
		/// </summary>
		/// <param name="attacker">The attacking avatar.</param>
		/// <param name="attackerWeapon">The weapon to be used during the attack.</param>
		/// <param name="defender">The defending avatar.</param>
		/// <param name="defensiveSkill">The skill used to defender against the attack.</param>
		/// <param name="defenderAttributeType">The attribute used to defend.</param>
		/// <param name="range">The range from the attacker to the defender.</param>
		/// <returns>True if the defender was killed during the attack; otherwise false.</returns>
		public static bool PerformSimpleCombatTurn(
			IAvatar attacker, 
			IWeapon attackerWeapon,
            string offensiveSkill,
			IAvatar defender, 
			string defensiveSkill,
			AttributeType defenderAttributeType,
			int range)
		{
			// Only true if the defender gets killed.
			bool result = false;

            int defenderProtection = defender.GetArmor().Protection;

			//Phase 1: The Combat Skill Test
			//The attacker makes a combat skill test vs. Physical Dexterity (in the case of melee attacks or point blank ranged
			//attacks) or Mental Perception (in the case of ranged attacks at short range or greater), while the defender makes a
			//defense skill test against an appropriate combative or defensive skill, to determine the number of opposing successes.
			//If the number of successes scored by the attacker do not exceed the number of successes scored by the defender, then
			//the target has been hit.
			int attackerSkillLevel = (int)attacker.Skills[offensiveSkill];
			int defenderSkillLevel = (int)defender.Skills[defensiveSkill];

			if (attackerWeapon.Durability <= 0) attackerSkillLevel = 0;

			AttributeType attackerAttrType = AttributeType.Strength; // Was Dex, should it go back to Dex?
			if (attackerWeapon.Range > 1) attackerAttrType = AttributeType.Perception;

			// Perform skill rolls.
			int attackerSuccessCount;
			SkillManager.PerformSkillTest(attacker, offensiveSkill, attackerAttrType, 0, attackerWeapon.SkillLevelRequiredToEquip, false, out attackerSuccessCount);

			int defenderSuccessCount;
            SkillManager.PerformSkillTest(defender, defensiveSkill, defenderAttributeType, 0, defender.GetArmor().SkillLevelRequiredToEquip, false, out defenderSuccessCount);

			// TODO: Implement disatrous failure in combat.
			//• In the case of a disastrous failure, the character has fumbled and loses his next attack. A very high degree
			//(3+) disastrous failure may indicate weapon breakage. This applies to both the offensive and defense sides of
			//the combat.

			// Range
			//• While a ranged attack at point blank range suffers no penalty, other ranged attacks will incur a penalty to
			//the attack roll target number based on the distance to the target. Attacks attempted within short range for
			//the weapon will suffer a penalty of -1, while attacks within medium range will suffer a -2 penalty and attacks
			//within long range will suffer a penalty of -4.
			if (range > 1)
			{
				attackerSuccessCount -= range;
				if (attackerSuccessCount == 0)
				{
					attacker.Context.Add(new RdlSystemMessage(0, SR.AttackOutOfRange));
				}
			}

			int outcome = attackerSuccessCount - defenderSuccessCount;

			// Raise an event on the attacker so that the outcome can be modified based on world specific conditions.
			attacker.OnAttack(ref outcome);

			if (outcome > 0)
			{
				// DEFENDER WAS HIT.

				//Phase 2: Damage Determination
				//The damage value of the attacker’s weapon is added to the number of combat successes that the attacker has
				//remaining. This is the Damage Value.
				//Small weapons will typically cause 1 point of damage, medium weapons 2 points, and heavy weapons 3 points of
				//damage. Additionally, melee weapon damage is modified by a value equal to the attacker’s strength value minus 5.
				//If a character is exceptionally weak, this will subtract damage from the final value (base damage plus successes). The
				//modified damage value can never be less than zero.
				int damage = attackerSuccessCount + attackerWeapon.Power;
				damage += attacker.Attributes[AttributeType.Strength] - 5;
				if (damage <= 0) damage = 1;

				//Phase 3: Application of Armor
				//A piece of armor has two statistics, coverage rating (CR) and absorption rating (AR). CR ranges from 1 (very
				//little coverage) to 5 (complete coverage). AR can range from as little as 1 for thin leather or heavy cloth to as much
				//as 8 or more for magical or unusual armors.
				//The attacker’s remaining successes are applied to the coverage rating. If the number of successes exceeds the
				//armor’s coverage rating, then the armor has been bypassed (a better executed hit is more likely to bypass armor), in
				//which case all damage goes straight to the defender. If the attack fails to bypass the armor, then the armor removes
				//one point of damage per point of absorption. Any damage equal to or beyond the armor’s absorption rating indicates
				//that the armor has been pierced or rendered ineffective in some way. When this happens, any excess damage is
				//applied to the character (see below), and the armor’s coverage rating is reduced by one (the armor is damaged) until
				//it can be repaired by someone with the appropriate skill (this could include the character if they have said skills).
				//Armor with a CR of 5+ cannot be damaged by penetration. This CR is usually only used for creatures with
				//natural armor or who are made of unnaturally hard substances.
				defender.OnApplyProtection(attacker, ref damage);
				//if (attackerSuccessCount <= (defenderProtection * 0.5))
				//{
				//    // Let armor absrob some or all of the damage.
				//    // Do not reduce damage more than half because of armor.
				//    int newDmg = damage - (int)(defenderProtection * 0.5);
				//    if (newDmg < (damage * 0.5))
				//    {
				//        damage = (int)(damage * 0.5);
				//    }
				//    if (damage <= 0) damage = 1;
				//}

				if (damage > 0)
				{
					// Decrement the durability of the defender's armor.
                    defender.GetArmor().Durability--;

					//Phase 4: Apply Damage
					//Any remaining wounds after the application of armor are subtracted from the character’s Body stat.
					defender.SetBody(defender.Body - damage);
					attacker.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Melee, SR.AttackHitDefender(defender.The(), damage)));
					defender.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Negative, SR.AttackHitByAttacker(attacker.The(), damage)));

					if (defender.IsDead)
					{
						result = true;
						attacker.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Melee, 
							SR.AttackYouKilledDefender(defender.The())));
						defender.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Negative, 
							SR.AttackYouWereKilledByAttacker(attacker.The())));
					}
					else if (defender.IsUnconscious)
					{
						attacker.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Melee, 
							SR.AttackUnconsciousDefender(defender.TheUpper())));
						defender.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Negative, 
							SR.AttackYouAreUnconscious));
					}
				}
				else
				{
					attacker.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Negative, SR.AttackFailed));
					defender.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None, SR.AttackAttackerFailed(attacker.A())));
				}
			}
			else
			{
				// Attacker missed.
				attacker.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Negative, SR.AttackMiss));
				defender.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None, SR.AttackAttackerFailed(attacker.A())));
			}

			// Regardless of outcome advance the skill of the attacker and the defender.
			SkillManager.AdvanceSkill(attacker, attackerWeapon.Skill, attackerWeapon.SkillLevelRequiredToEquip, attacker.Context);
			// Use attackerWeapon as the required skill level so the defender can not elevate beyond the skill used
			// for the attack.
			SkillManager.AdvanceSkill(defender, defensiveSkill, attackerWeapon.SkillLevelRequiredToEquip, defender.Context);

			// Return result of the combat action
			return result;
		}
	}
}
