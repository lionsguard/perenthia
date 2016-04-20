using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
	/// <summary>
	/// Represents the focus for creating and using magical abilities and psionics.
	/// </summary>
	public class Foci
	{
		public int Power { get; set; }
		public int Range { get; set; }
		public TimeSpan Duration { get; set; }	

		public Foci()
			: this(0, 0, TimeSpan.FromSeconds(0))
		{
		}

		public Foci(int power, int range)
			: this(power, range, TimeSpan.FromMinutes(5))
		{
		}

		public Foci(int power, int range, TimeSpan duration)
		{
			this.Power = power;
			this.Range = range;
			this.Duration = duration;
		}

		public int GetDifficulty()
		{
			return this.Range + this.Power;
		}
	}
}
