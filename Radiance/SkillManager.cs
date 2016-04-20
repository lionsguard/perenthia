using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using Radiance.Configuration;
using Radiance.Markup;
using Lionsguard;

namespace Radiance
{
	#region SkillManager
	/// <summary>
	/// Provides static methods and properties related to skills, skill tests, etc.
	/// </summary>
	public static class SkillManager
	{
		/// <summary>
		/// Gets a value indicating how many skill points should be available during Character Creation.
		/// </summary>
		internal static readonly int MaxSkillPointsInCharacterCreation = 32;

        /// <summary>
        /// Gets a value indicating the max number of skill levels higher than the required skill level a 
        /// player can be and still gain skill advancement for use of the skill.
        /// </summary>
        internal static readonly int MaxSkillLevelOffset = 4;

		#region PerformSkillTest
		/// <summary>
		/// Performs a skill test for the specified avatar, skill and difficulty.
		/// </summary>
		/// <param name="who">The Avatar instance for which the skill test is being performed.</param>
		/// <param name="skillName">The name of the skill being tested.</param>
		/// <param name="attributeType">The AttributeType of the attribute used to perform the skill.</param>
		/// <param name="difficulty">The difficulty rating of the action being performed. If 0 a value will be assigned.</param>
        /// <param name="requiredSkillLevel">The skill level required in order for this skill test to be a success.</param>
		/// <param name="opposeSkillTest">A value indicating whether or not an oppossing skill test should be automatically calculated. For non-combative actions.</param>
		/// <param name="successCount">The number of success dice left after the skill test.</param>
		/// <returns>True if the skill test was successful; otherwise false.</returns>
		public static bool PerformSkillTest(
			IAvatar who, 
			string skillName, 
			AttributeType attributeType, 
			int difficulty,
            int requiredSkillLevel,
			bool opposeSkillTest,
			out int successCount)
		{
			//Before the action is attempted by the character, you must determine a numeric difficulty rating for the skill
			//test. This difficulty rating dictates how many d10 you will roll for opposing successes. A difficulty rating of about
			//half of a character’s skill rating will provide a moderate challenge. A lower difficulty rating will make it easier for
			//the character to succeed to a greater degree and a higher difficulty rating will make it correspondengly harder to do
			//so.
			successCount = 0;
			int skillLevel = (int)who.Skills[skillName];
			int attrValue = who.Attributes[attributeType];

            // If the skill level is below the required skill level then perform an unskilled test.
            if (skillLevel < (requiredSkillLevel - MaxSkillLevelOffset)) skillLevel = 0;
            
            // The skill level should cap at the required skillevel + max skill offset to prevent low level weapons and spells
            // causing far too much damage or healing entirely too much.
            if (skillLevel > (requiredSkillLevel + MaxSkillLevelOffset))
            {
                skillLevel = (requiredSkillLevel + MaxSkillLevelOffset);
            }

			if (skillLevel == 0)
			{
				// Unskilled Test
				//If a character must make a skill test in a skill that he is not proficient in, the roll is made on a single d20. If the
				//character has a skill similar to the one being used (such as a character with a rapier skill using a broadsword), the
				//roll may be made on a number of d12 equal to the level of the similar skill rather than the single d20 (this is called
				//a semi-skilled test).
				//For the purposes of critical failures, a semi-skilled die (d12) is a critical failure on a roll of 11-12. An unskilled
				//skill die (d20) is a critical failure on a roll of 17-20.
				//If a character has two similar skills, one at a higher rating than the other, the player may opt to roll the lower
				//skill as a semi-skilled (d12) check on the higher rated skill, rather than rolling a lower number of d10. For example,
				//a character with a pistol skill of 3 and a rifle skill of 1 could choose to roll his rifle skill test on three d12 rather than
				//one d10.
				skillLevel = Dice.Roll(1, 20);
				if (skillLevel < 17 && skillLevel <= attrValue)
				{
					successCount++;
				}
			}
			else
			{
				// Skilled Test
				if (difficulty == 0) difficulty = requiredSkillLevel;

				//The player (or you, the GM, if you are resolving an action for an NPC) then rolls a number of d10 equal to the
				//skill he is attempting to use, and compares the results to a specific attribute (as determined by you, the GM, based
				//on the type of action being attempted — Pistol vs. Perception would be used to shoot something while Pistol vs.
				//IQ might be used to fix one). Any die that shows a number equal to or less than the attribute being rolled against
				//is considered a success.
				int skillDice = skillLevel;
				int critSuccess = 0;
				while (skillDice > 0)
				{
					int roll = Dice.SkillRoll();
					if (roll <= attrValue)
					{
						successCount++;
					}
					if (roll == 1)
					{
						//Any roll of 1 indicates a critical success, and another die is added into the roll. If another 1 is rolled, another
						//die may be added and so on. A roll in the critical failure range on these bonus dice does not indicate a critical
						//failure (see below), it is simply treated as an unsuccessful die.
						// Critical Success, do not decrement the skillDice.
						critSuccess++;
					}
					else if (roll == Dice.SkillDice)
					{
						if (critSuccess > 0)
						{
							// Decrement critical success so a critical failure can occur if the critSuccess gets down to 0.
							critSuccess--;
						}
						else
						{
							// Critical Failure
							//Any roll of 10 on a normal skill check indicates a critical failure (the critical failure range on semi-skilled and
							//unskilled tests is larger, see section 1.2.3), and one existing success (if one exists) is removed, along with the
							//critical failure die. Another die is added to the roll. This penalty die may not be counted as a success, but if it’s
							//roll is in the critical failure range, it is treated as another critical failure and another penalty die is added to the
							//roll. Penalty dice are chain rolled like this for as long as they show results in the critical failure range. If more
							//critical failures are rolled than there are successes to remove, a disastrous failure has occurred; something
							//unusually bad happens. The exact nature of the disaster is up to the GM, but it usually indicates breaking
							//things, dropping or damaging your weapon, falling on your head, etc. Note that Opposing Successes do not
							//reduce a character’s effective successes lower than zero. Any left over opposing successes are simply discarded;
							//thus, they may not cause critical failure, no matter how many are rolled. Successes are removed as a result
							//of critical failure dice before successes are removed as a result of opposing successes. Thus, a character rolling
							//three successes and one critical failure would remove one success along with the critical failure die first, then
							//potentially remove the remaining success depending on how many opposing successes were scored.
							skillDice--;
							successCount--;
							// Continue to roll if critical failure keeps occuring.
							int critFailureRollCount = successCount;
							while (critFailureRollCount > 0)
							{
								int critFailureRoll = Dice.SkillRoll();
								if (critFailureRoll == 10)
								{
									// Take away another success.
									successCount--;
								}
								if (successCount < 3)
								{
									// No more than a +3 for critical failures.
									critFailureRollCount = 0;
								}
								else
								{
									// Roll another 
									critFailureRollCount--;
								}
							}
						}
					}
					else
					{
						// Normal, remove one dice.
						skillDice--;
					}
				}
			}

			if (opposeSkillTest)
			{
				//You, the GM, must then roll a number of d10 equal to the difficulty rating that you determined above. Any die
				//showing a value of 5 or less is considered to be an opposing success. Each opposing success that you score removes
				//one of the character’s successes. If all of the character’s successes are removed, then the action has failed.
				//The greater the number of successes remaining, the greater the extent to which the attempted action has succeeded.
				//For example, only one remaining success indicates that the action has just barely been performed, while a
				//large number of remaining successes indicate that the action has been performed amazingly well.
				if (successCount > 0)
				{
					if (difficulty == 0) difficulty = 1;
					for (int i = 0; i < difficulty; i++)
					{
						int roll = Dice.SkillRoll();
						if (roll <= 5)
						{
							if (successCount > 0) successCount--;
						}
					}
				}
			}

			// Return the results.
			return successCount > 0;
		}
		#endregion

		#region AdvanceSkill
		/// <summary>
		/// Advances the skill level of the specified Avatar, accordng to the current level of the specified skill.
		/// </summary>
		/// <param name="avatar">The Avatar advancing the skill.</param>
		/// <param name="skillName">The name of the skill to advance.</param>
		/// <param name="context">The IMessageContext instance to write out the results to.</param>
		public static void AdvanceSkill(IAvatar avatar, string skillName, int requiredSkillLevel, IMessageContext context)
		{
            double skillLevel = avatar.Skills[skillName];

            // Can not advance skill beyond the required skill level + max skill offset.
            if (skillLevel >= (requiredSkillLevel + MaxSkillLevelOffset))
            {
                // Do not advance skill.
				Logger.LogDebug("[ {0} ] maxed at {1}, can not advance", skillName, skillLevel);
                return;
            }

			// Advance the skill level.
			if (skillLevel > 0 && skillLevel < 10)
			{
				skillLevel += 0.025;
			}
			else if (skillLevel >= 10 && skillLevel < 20)
			{
				skillLevel += 0.0075;
			}
			else if (skillLevel >= 20 && skillLevel < 30)
			{
				skillLevel += 0.005;
			}
			else if (skillLevel >= 30 && skillLevel < 40)
			{
				skillLevel += 0.0025;
			}
			else if (skillLevel >= 40 && skillLevel < 50)
			{
				skillLevel += 0.001;
			}
			else if (skillLevel >= 50 && skillLevel < 60)
			{
				skillLevel += 0.0005;
			}
			else if (skillLevel >= 60 && skillLevel < 70)
			{
				skillLevel += 0.00025;
			}
			else if (skillLevel >= 70 && skillLevel < 80)
			{
				skillLevel += 0.0001;
			}
			else if (skillLevel >= 80 && skillLevel < 90)
			{
				skillLevel += 0.000075;
			}
			else if (skillLevel >= 100)
			{
				skillLevel += 0.00005;
			}
			else
			{
				skillLevel += 0.000001;
			}
			int prevLevel = (int)avatar.Skills[skillName];
			avatar.Skills[skillName] = skillLevel;

			Logger.LogDebug("[ {0} ] :: prevSkill = {1}, updatedSkill = {2}", skillName, prevLevel, skillLevel);

			if ((int)skillLevel > prevLevel)
			{
				// Notify of skill advancement.
				context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Level, SR.SkillAdvanced(skillName.Trim(), (int)skillLevel)));

				// Send down the actual skill value.
				context.AddRange(avatar.GetRdlProperties(avatar.Skills.GetSkillKey(skillName)));
			}
		}
		#endregion
	}
#endregion

	/// <summary>
	/// Represents a list of skills and values.
	/// </summary>
	public class SkillList : ActorOwnedDictionaryBase<double>
	{
		/// <summary>
		/// Initializes a new instance of the SkillList class.
		/// </summary>
		/// <param name="owner">The owner of the skills contained within the collection.</param>
		public SkillList(IActor owner)
			: base(owner, "Skill_")
		{
		}

		/// <summary>
		/// Initializes a new instance of the SkillList class preloaded with the skills fromthe specified SkillDictionary.
		/// </summary>
		/// <param name="owner">The owner of the skills contained within the collection.</param>
		/// <param name="knownSkills">The skills to load.</param>
		public SkillList(IActor owner, SkillDictionary knownSkills)
			: base(owner, "Skill_")
		{
			// Preload the skills from the known skills for the current game.
			foreach (var item in knownSkills)
			{
				this.Add(item.Key, 0);
			}
		}

		public void AddRange(IEnumerable<KeyValuePair<string, double>> skills)
		{
			foreach (var skill in skills)
			{
				this.Add(skill.Key.Replace(Prefix, String.Empty), skill.Value);
			}
		}

		public override bool TryGetValue(string key, out double value)
		{
			value = 0;
			if (this.Owner.Properties.ContainsKey(this.GetPrefixedName(key)))
			{
				value = this.Owner.Properties.GetValue<double>(this.GetPrefixedName(key));
				return true;
			}
			return false;
		}

		public string GetSkillKey(string name)
		{
			return this.GetPrefixedName(name);
		}
	}

	/// <summary>
	/// Represents known skills providing a name and description of each skill.
	/// </summary>
	public class SkillDictionary : Dictionary<string, Skill>
	{
		/// <summary>
		/// Initializes a new instance of the KnownSkillList class.
		/// </summary>
		public SkillDictionary()
			: base (StringComparer.InvariantCultureIgnoreCase)
		{
		}

		/// <summary>
		/// Converts and returns to the current skills in the dictionary to RdlSkill tags.
		/// </summary>
		/// <returns>An array of RdlSkill tags for the skills contained in the dictionary.</returns>
		public RdlSkill[] ToRdl()
		{
			List<RdlSkill> list = new List<RdlSkill>();
			foreach (var item in this)
			{
				list.Add(new RdlSkill(item.Value.ID, item.Value.Name, item.Value.Description, item.Value.GroupName, 0));
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			return this.ToRdl().ToRdlString();
		}
	}

	/// <summary>
	/// Represents a skill within the virtual world.
	/// </summary>
	public class Skill
	{
		/// <summary>
		/// Gets or sets the unique ID of the skill.
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Gets or sets the name of the skill.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the description of the skill.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the group name of the skill.
		/// </summary>
		public string GroupName { get; set; }	
	}

	/// <summary>
	/// Represents a group of skill values.
	/// </summary>
	public class SkillGroup : Actor
	{
		public SkillList Skills { get; private set; }

		public SkillGroup()
		{
			this.Skills = new SkillList(this);
		}

		protected override RdlProperty[] GetPropertyTags()
		{
			List<RdlProperty> list = new List<RdlProperty>();

			foreach (var item in this.Skills)
			{
				// Replace the skill prefix since these should be treated like skill tags on the client.
				list.Add(new RdlProperty(this.ID, item.Key.Replace(this.Skills.Prefix, String.Empty), item.Value));
			}

			return list.ToArray();
		}

		public override string ToString()
		{
			return this.ToRdl().ToRdlString();
		}
	}

	/// <summary>
	/// Represents a collection of SkillGroup instances.
	/// </summary>
	public class SkillGroupCollection : List<SkillGroup>
	{
		/// <summary>
		/// Gets an array of RdlObject tags for the current SkillGroup.
		/// </summary>
		/// <returns>An array of RdlObject tags for the current SkillGroup.</returns>
		public RdlObject[] ToRdl()
		{
			List<RdlObject> tags = new List<RdlObject>();
			foreach (var item in this)
			{
				tags.AddRange(item.ToRdl());
			}
			return tags.ToArray();
		}
	}
}
