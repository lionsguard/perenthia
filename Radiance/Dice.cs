using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Radiance
{
	/// <summary>
	/// Provides static properties and methods used for Dice rolls.
	/// </summary>
	public static class Dice
	{
		private static Regex DiceExp = new Regex(@"^(\d+)[dD](\d+)([+-]\d+)?$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
		private static MersenneTwister Rnd = new MersenneTwister();

		/// <summary>
		/// Gets the numeric side value of a skill dice.
		/// </summary>
		public static readonly int SkillDice = 10;

		/// <summary>
		/// Gets a random value within the specified minValue and maxValue range.
		/// </summary>
		/// <param name="minValue">The starting value of the range.</param>
		/// <param name="maxValue">The ending value of the range.</param>
		/// <returns>A random value within the specified minValue and maxValue range.</returns>
		public static int Random(int minValue, int maxValue)
		{
			return Rnd.Next(minValue, maxValue);
		}

		/// <summary>
		/// Gets a value indicating whether or not an encounter should occur.
		/// </summary>
		/// <returns>True if an encounter should take place; otherwise false.</returns>
		public static bool EncounterRoll()
		{
			return EncounterRoll(0, 0);
		}

		/// <summary>
		/// Gets a value indicating whether or not an encounter should occur.
		/// </summary>
		/// <param name="systemModified">A value to add to the end of the system's auto roll.</param>
		/// <param name="playerModifier">A value to add to the end of the player's auto roll.</param>
		/// <returns>True if an encounter should take place; otherwise false.</returns>
		public static bool EncounterRoll(int systemModifier, int playerModifier)
		{
			int sysRoll = Dice.Roll(1, 20) + systemModifier;
			int roll = Dice.Roll(1, 20) + playerModifier;
			return (sysRoll >= roll);
		}

		/// <summary>
		/// Rolls the dice specified by the expression.
		/// </summary>
		/// <param name="expression">The string expression of a standard dice roll. Example: 2d10</param>
		/// <returns>The result of the specified dice roll.</returns>
		public static int Roll(string expression)
		{
			Match match = DiceExp.Match(expression);
			if (match.Success)
			{
				int diceCount = 0;
				int sideCount = 0;
				int modifier = 0;

				Int32.TryParse(match.Groups[1].Value, out diceCount);
				Int32.TryParse(match.Groups[2].Value, out sideCount);

				if (match.Groups.Count == 3)
				{
					string value = match.Groups[3].Value;
					Int32.TryParse(value.Substring(1), out modifier);
					if (value.StartsWith("-"))
					{
						modifier *= -1;
					}
				}
				return Dice.Roll(diceCount, sideCount, modifier);
			}
			return 0;
		}

		/// <summary>
		/// Rolls the specified diceCount number of dice with the specified sideCount number of sides.
		/// </summary>
		/// <param name="diceCount">The number of dice to roll.</param>
		/// <param name="sideCount">The number of sides of the dice to roll. Example: 6 represents a six sided dice.</param>
		/// <returns>The results of the dice roll.</returns>
		public static int Roll(int diceCount, int sideCount)
		{
			return Dice.Roll(diceCount, sideCount, 0);
		}

		/// <summary>
		/// Rolls the specified diceCount number of dice with the specified sideCount number of sides.
		/// </summary>
		/// <param name="diceCount">The number of dice to roll.</param>
		/// <param name="sideCount">The number of sides of the dice to roll. Example: 6 represents a six sided dice.</param>
		/// <param name="modifier">A modifier added to the final result of the dice roll.</param>
		/// <returns>The results of the dice roll.</returns>
		public static int Roll(int diceCount, int sideCount, int modifier)
		{
			int final = 0;
			for (int i = 0; i < diceCount; i++)
			{
				final += Rnd.Next(1, sideCount);
			}
			if (modifier != 0)
			{
				final += modifier;
			}
			return final;
		}

		/// <summary>
		/// Performs a standard attribute roll of 3d6.
		/// </summary>
		/// <returns>The result of the dice roll.</returns>
		public static int RollAttribute()
		{
			return Dice.Roll(3, 6);
		}

		/// <summary>
		/// Performs a standard skill dice roll using the value of SkillDice as the side count. Default is 1d10.
		/// </summary>
		/// <returns>The result of the dice roll.</returns>
		public static int SkillRoll()
		{
			return Dice.Roll(1, SkillDice);
		}
	}
}
