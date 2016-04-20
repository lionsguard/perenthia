using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
	public static class GameTime
	{
		public static TimeSpan ElapsedTime { get; private set; }
		public static TimeSpan TotalTime { get; private set; }
		public static DateTime LastTick { get; private set; }

		static GameTime()
		{
			ElapsedTime = TimeSpan.Zero;
			TotalTime = TimeSpan.Zero;
			LastTick = DateTime.Now;
		}

		public static void Update()
		{
			DateTime now = DateTime.Now;
			ElapsedTime = now - LastTick;
			LastTick = now;
			TotalTime += ElapsedTime;
		}
	}
}
