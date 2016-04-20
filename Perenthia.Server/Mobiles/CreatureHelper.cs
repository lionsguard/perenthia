using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia.Mobiles
{
	public static class CreatureHelper
	{
		#region Find
		public static Creature Find(IAvatar avatar)
		{
			// Find a creature that closely matches the current avatar level but does not exceed the level by more than 2 or 3.

			return Creatures.Rat;
		}
		#endregion
	}
}
