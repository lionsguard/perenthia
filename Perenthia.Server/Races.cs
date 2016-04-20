using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;

namespace Perenthia
{
	#region Playable Races
	public class NorvicRace : Race
	{
		public const string RaceName = "Norvic";

		public NorvicRace()
		{
			this.ID = 1;
			this.Name = RaceName;
			this.Description = "Battle is a way of life for the Norvic people, pushing their sons and daughters into war at an early age has fashioned an unparalleled warrior society. Both the men and women of the Norvic people are feared and respected equally among their peers and the other races of the continent. Much was lost on the origins of the people during the Great War, where they came from and what life was like before the Darkness. All that remains of their heritage are the giant fortresses in the frozen lands of the North.";
			this.StartingLocation = new Point3(456, 497, 0);
			this.Attributes.Strength = 1;
			this.Attributes.Stamina = 1;
			this.Attributes.Intelligence = -1;
			this.IsPlayable = true;
		}
	}

	public class NajiiRace : Race
	{
		public const string RaceName = "Najii";

		public NajiiRace()
		{
			this.ID = 2;
			this.Name = RaceName;
			this.Description = "Wandering from place to place and eventually settling in the desert mountains in the southern tip of the continent the Najii have accumulated an extensive knowledge of the landscape and terrain of Perenthia. Known for their cleverness and skill they are fierce warriors, shrewd negotiators and master craftspeople. As a society their nomadic lifestyle has left them a bit of a mix of cultures and customs. Borrowing from the peaceful meditation of the Xhin, the warrior upbringing of the Norvic and the refined studies of the Peren the Najii have adopted a balanced and peaceful society.";
            this.StartingLocation = new Point3(456, 497, 0);
			this.Attributes.Strength = -1;
			this.Attributes.Dexterity = 1;
			this.Attributes.Intelligence = 1;
			this.IsPlayable = true;
		}
	}

	public class PerenRace : Race
	{
		public const string RaceName = "Peren";

		public PerenRace()
		{
			this.ID = 3;
			this.Name = RaceName;
			this.Description = "Considered the founders of the New Alliance and having given their name to the continent; the Peren continue to provide leadership to the free cities, striving to expand their reign and drive out the foul darkness that plagues the land. Following the Great War the Peren reached out to the patches of survivors in an attempt to unite the humans against the beasts that now had dominion. Their society focuses on the fine arts and scientific studies; they are also said to be the finest ship builders on the continent.";
            this.StartingLocation = new Point3(456, 497, 0);
			this.Attributes.Dexterity = 1;
			this.Attributes.Stamina = -1;
			this.Attributes.Perception = 1;
			this.IsPlayable = true;
		}
	}

	public class XhinRace : Race
	{
		public const string RaceName = "Xhin";

		public XhinRace()
		{
			this.ID = 4;
			this.Name = RaceName;
			this.Description = "Arcane knowledge is said to have come from the Xhin and all the Great Wizards of the Golden Age where instructed by the Xhin. They are also recognized as the oldest race of Perenthia and are said to have lived on the continent long before the Wizards of the Golden Age came into power. A somewhat quiet and guarded people the Xhin have shared the Arcane with the survivors of the Great War in attempt to fight back the destructive powers of the dark warriors. Theirs is a peaceful society and although many great warriors have come from their ranks they still remain paramount in Arcane magic.";
            this.StartingLocation = new Point3(456, 497, 0);
			this.Attributes.Strength = -1;
			this.Attributes.Intelligence = 1;
			this.Attributes.Endurance = 1;
			this.IsPlayable = true;
		}
	}
	#endregion

	#region Creature and NPC Races
	public class CreatureRace : Race
	{
		public const string RaceName = "Creatue";

		public CreatureRace()
		{
			this.ID = 5;
			this.Name = RaceName;
			this.Description = String.Empty;
			this.IsPlayable = false;
		}
	}
	#endregion
}
