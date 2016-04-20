using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Lionsguard;

namespace Perenthia
{
	public class PerenthiaMapManager : MapManager
	{
		public override int DefaultWidth { get { return 15; } }
		public override int DefaultHeight { get { return 15; } }

		public PerenthiaMapManager(World world)
			: base(world)
		{
		}

		public override List<Place> GetMap(string mapName, int x, int y, int width, int height)
		{
			List<Place> map = new List<Place>();
			if (this.World != null)
			{
				int startX = x;
				int endX = x + width;
				int startY = y;
				int endY = y + height;
				Logger.LogDebug("GetMap: startX = {0}, startY = {1}, endX = {2}, endY = {3}",
					startX, startY, endX, endY);

				MapDetail detail = detail = this.GetDetail(mapName);
				if (detail != null)
				{
					if (!detail.IsLoaded)
					{
						// Load the map from the database.
						Logger.LogDebug("GetMap: Getting entire map from database: StartX={0}, StartY={1}, Width={2}, Height={3}",
							detail.Key.StartX, detail.Key.StartY, detail.Width, detail.Height);

						map = this.World.Provider.GetMap(detail.Key.StartX, detail.Key.StartY, 0, detail.Width, detail.Height);

						Logger.LogDebug("GetMap: Retrieved {0} places.", map.Count);
						for (int i = 0; i < map.Count; i++)
						{
							this.World.Places.Add(map[i].Location, map[i]);
							if (map[i] is Temple)
							{
								Game.AddTemple(map[i] as Temple);
							}
						}
						detail.IsLoaded = true;
					}
				}
				else
				{
					Logger.LogDebug("GetMap: MapDetail not found.");
				}

				// Query the map from the current list of places in the world or load it from the database.
				map = (from p in this.World.Places.Values
					   where (p.X >= startX && p.X <= endX)
					   && (p.Y >= startY && p.Y <= endY)
					   select p).ToList();
				Logger.LogDebug("GetMap: World.Places.Count = {0}, map.Count = {1}",
					this.World.Places.Count, map.Count);
			}
			return map;
		}

		public override void SaveMap(MapDetail map)
		{
			this.World.Provider.SaveMap(map);
		}
	}
}
