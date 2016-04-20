using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
	/// <summary>
	/// Represents the damage a weapon can inflict as well as the type of weapon; ranged or melee.
	/// </summary>
	public class WeaponDamage
	{
		/// <summary>
		/// Gets the power value of the damage delivered by the weapon.
		/// </summary>
		public int Power { get; set; }

		/// <summary>
		/// Gets the range of the weapon. A numeric value indication the range of the weapon in tiles.
		/// </summary>
		public int Range { get; set; }	
	}
}
