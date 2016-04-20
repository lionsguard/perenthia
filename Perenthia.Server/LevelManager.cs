using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia
{
	/// <summary>
	/// Provides static properties and methods for managing player levels and experience.
	/// </summary>
	public static class LevelManager
	{
		/// <summary>
		/// Gets the experience required to advance to the next level past the specified level.
		/// </summary>
		/// <param name="level">The current level.</param>
		/// <returns>The experience required to advance to the next level.</returns>
		public static int GetNextLevelXp(int level)
		{
			// Getting the max experience required to advance to the level beyond
			// the specified level, the next level's required experience.
			int baseXp = 400;
			if (level == 0) return 0;
			if (level == 1) return baseXp;
			return (GetNextLevelXp(level - 1) + baseXp + ((level - 1) * 100));
		}

		/// <summary>
		/// Gets the experience reward for kiling a mobile of the specified mobileLevel.
		/// </summary>
		/// <param name="playerLevel">The level of the player killing the mobile.</param>
		/// <param name="mobileLevel">The level of the mobile that was killed.</param>
		/// <returns>THe experience reward for killing the mobile of the specified mobileLevel.</returns>
		public static int GetXpForMobileKill(int playerLevel, int mobileLevel)
		{
			int baseXp = 50;
			if (mobileLevel == 0) return 0;
			if (mobileLevel == 1) return baseXp;
			int xp = baseXp + ((mobileLevel - 1) * 5);
			if (playerLevel < mobileLevel)
			{
				xp += Dice.Roll(1, 10);
			}
            else if (playerLevel > (mobileLevel + 5))
            {
                xp -= Dice.Roll(playerLevel, 10);
            }
            if (xp < 0) xp = 0;
			return xp;
		}

		/// <summary>
		/// Advances the specified Character if the experience earned equals or exceeds the next level required experience.
		/// </summary>
		/// <param name="player">The current player to advance.</param>
		public static void AdvanceIfAble(Character player)
		{
			int nextXp = GetNextLevelXp(player.Level);
			if (player.Experience >= nextXp)
			{
				// Increase level and experience.
				player.Level++;
				player.Experience = player.Experience - nextXp ;
				player.ExperienceMax = GetNextLevelXp(player.Level);

				player.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Level,
					String.Format(Resources.LevelGained, player.Level)));

				// Increase body and mind values.
				IncreaseBodyAndMind(player, player.Level);

				// Notify the player that their health and willpower has increased.
				player.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Level,
					String.Format(Resources.HealthGained, player.Level + 2)));
				player.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Level,
					String.Format(Resources.WillpowerGained, player.Level + 2)));

                // Raise an event to indicate the player has advanced.
                player.World.OnPlayerAdvanced(player, String.Format(Resources.PlayerLevelGained, player.Name, player.Level));

				player.Context.AddRange(player.GetRdlProperties(
					Character.LevelProperty, 
					Character.ExperienceProperty,
					Character.ExperienceMaxProperty,
					Avatar.BodyProperty,
					Avatar.BodyMaxProperty,
					Avatar.MindProperty,
					Avatar.MindMaxProperty));

				player.Save();
			}
		}

		public static void IncreaseBodyAndMind(PerenthiaAvatar avatar, int level)
		{
			int body = avatar.BodyMax + (level + 2);
			int mind = avatar.MindMax + (level + 2);
			avatar.SetBody(body, body);
			avatar.SetMind(mind, mind);
		}
	}
}
