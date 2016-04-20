using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance
{
	public static class MagicManager
	{
		/// <summary>
		/// Performs a cast of a spell from the caster onto the target Actor instance.
		/// </summary>
		/// <param name="spell">The spell being cast.</param>
		/// <param name="caster">The avatar casting the spell.</param>
		/// <param name="target">The target Actor instance, avatar or item.</param>
		/// <returns>The results of the casting.</returns>
		public static CastResults PerformCast(
			ISpell spell,
			IAvatar caster,
			IActor target)
		{
			CastResults results = new CastResults();

			// Used for sending messages to the defender's Context.
			IAvatar defender = target as IAvatar;

			// Ensure that the spell can be cast on the target.
			if (Object.ReferenceEquals(caster, target))
			{
				if (spell.IsDamageSpell)
				{
					caster.Context.Add(new RdlErrorMessage(SR.CastNotSelf));
					return results;
				}
			}

			// To determine the effect’s difficulty rating, all foci scores are added together.
			int difficulty = spell.Foci.GetDifficulty();

			//The sphere or discipline skill test is then made vs. Intelligence, opposed by the opposing successes of the difficulty
			//rating roll (see Section 1.2.1). If no successes are remain, then the effect fails and the character burns no mind.
			//If a disastrous failure is rolled, the character takes the effect himself (if it’s offensive), it has its opposite effect, or
			//whatever the GM decides; the character also burns extra mind equal to the extent of the critical failure (if two 10s
			//were rolled on 2d10, the character would eat the effect and burn 2 extra mind).
			//Note that offensive effects with no area foci require a successful combat roll to strike the target, on the sphere or
			//discipline skill in the case of ranged attacks, or on an unarmed combat (or similar) skill in the case of touch effects.
			//Called shots can be made as in normal combat.
			//The defender may be allowed a Dodge roll for defense against being hit by some effects. Generally, if the attacker
			//must make a perception or dexterity roll to hit, the defender will probably be allowed a Dodge roll.
			int casterSuccessCount;
			SkillManager.PerformSkillTest(caster, spell.Skill, AttributeType.Intelligence, difficulty, spell.SkillLevelRequiredToEquip, true, out casterSuccessCount);

			// Raise the OnCastSkillRoll event to determine if the success roll should be overwritten.
			caster.OnCastSkillRoll(spell, ref casterSuccessCount);

			results.CastSuccessCount = casterSuccessCount;

			//Producing an effect, if successful, burns Mind. The amount of Mind burned is equal to the effect’s difficulty
			//minus the number of effective successes rolled (that is, what successes are left after the opposing successes are taken
			//into account), for a minimum of zero.
			int mindValueUsed = (difficulty - casterSuccessCount);
			if (mindValueUsed <= 0) mindValueUsed = 1;

			// Caster needs to the required amount of mind value.
			if (caster.Mind < mindValueUsed)
			{
				caster.Context.Add(new RdlErrorMessage("You do not have the required Willpower to cast this spell."));
				return results;
			}

			if (casterSuccessCount < 0 && spell.IsDamageSpell)
			{
				// Disatrous failure, spell backfires!
				caster.SetMind(caster.Mind - mindValueUsed);
				caster.SetMind(caster.Mind + casterSuccessCount);

				// Apply damage to the caster.
				caster.SetBody(caster.Body - spell.Foci.Power);

				caster.Context.Add(new RdlErrorMessage(SR.CastBackfired));

				if (caster.IsDead)
				{
					results.CasterDied = true;
					caster.Context.Add(new RdlErrorMessage(SR.CastBackfiredKilledCaster));
				}
				else if (caster.IsUnconscious)
				{
					results.CasterUnconscious = true;
					caster.Context.Add(new RdlErrorMessage(SR.CastBackfiredUnconsciousCaster));
				}
			}
			else if (casterSuccessCount > 0)
			{
				results.CastSuccessful = true;

				// Spell successful.
				caster.SetMind(caster.Mind - mindValueUsed);

				// Inform the caster of the successful cast.
				//caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
				//    SR.CastSuccess(spell.Name, target.A())));

				spell.Cast(caster, target, results);

				caster.OnCastSuccess(spell);

				// Handle death messages.
				if (defender.IsDead)
				{
					caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
						SR.AttackYouKilledDefender(defender.The())));
					defender.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Negative,
						SR.AttackYouWereKilledByAttacker(caster.The())));
				}
				else if (defender.IsUnconscious)
				{
					caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast, 
						SR.AttackUnconsciousDefender(defender.TheUpper())));
					defender.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Negative, 
						SR.AttackYouAreUnconscious));
				}

				// Send down both the body and mind values of both the caster and the target to both the
				// caster and the target.
				RdlTag[] casterTags = caster.GetRdlProperties(Avatar.BodyProperty, Avatar.MindProperty);
				RdlTag[] defenderTags = null;
				if (defender != null) defenderTags = defender.GetRdlProperties(Avatar.BodyProperty, Avatar.MindProperty);

				caster.Context.AddRange(casterTags);
				if (defenderTags != null) caster.Context.AddRange(defenderTags);

				if (defender != null)
				{
					defender.Context.AddRange(defenderTags);
					defender.Context.AddRange(casterTags);
				}
			}
			else
			{
				// Missed.
				caster.Context.Add(new RdlErrorMessage(SR.CastFailed));
				if (defender != null) defender.Context.Add(new RdlErrorMessage(SR.CasterCastFailed(caster.A())));
			}

			//If an effect burns more mind than the character has available, then any overflow causes wounds. This is called
			//channeling, and can be done even when the caster has zero mind. If a character channels to below 0 body, his life
			//begins seeping away (treat it as any other wound below zero), and he must be magically or psionically healed or he
			//will die when he passes his negative physical endurance.
			if (caster.Mind < 0)
			{
				int channelingDamage = caster.Mind;
				caster.SetMind(0);
				caster.SetBody(caster.Body - channelingDamage);
				caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Negative, SR.CastChanneling));
			}

			// Regardless of outcome advance both the caster and target skills.
			SkillManager.AdvanceSkill(caster, spell.Skill, spell.SkillLevelRequiredToEquip, caster.Context);

			// NOTE: Do not have resist skills...
			//if (defender != null)
			//{
			//    // Use attackerWeapon as the required skill level so the defender can not elevate beyond the skill used
			//    // for the attack.
			//    SkillManager.AdvanceSkill(defender, defensiveSkill, spell.SkillLevelRequiredToEquip, defender.Context);
			//}

			return results;
		}
	}

	public class CastResults
	{
		public int CastSuccessCount { get; set; }	
		public bool CastSuccessful { get; set; }
		public bool TargetDied { get; set; }
		public bool TargetUnconscious { get; set; }	
		public bool CasterDied { get; set; }
		public bool CasterUnconscious { get; set; }	
	}
}
