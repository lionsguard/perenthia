using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perenthia
{
	public enum ColorType
	{
		None = 0,
		White = 1,
		Black = 2,
		Blue = 3,
		Green = 4,
		Red = 5,
		Yellow = 6,
		Orange = 7,
		Purple = 8,
		Pink = 9,
		Tan = 10,
		Brown = 11,
	}

	public enum PotionType
	{
		Heal,
		Mind,
		Damage,
	}

	public enum OrderType
	{
		None,
		Knight,
		Mage,
		Rogue,
		Ranger,
	}

    public enum TaskType
    {
        None,
        Discovery,
        Collection,
        Kill,
        KillWithItem,
        Delivery,
    }
}
