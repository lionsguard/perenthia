using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
	public abstract class MapManager
	{
		protected World World { get; private set; }

		public abstract int DefaultWidth { get; }
		public abstract int DefaultHeight { get; }

		public Dictionary<MapKey, MapDetail> MapDetails { get; private set; }

		protected MapManager(World world)
		{
			this.World = world;
			this.MapDetails = new Dictionary<MapKey, MapDetail>(new MapKeyEqualityComparer());
		}

		public abstract List<Place> GetMap(string mapName, int x, int y, int width, int height);
		public abstract void SaveMap(MapDetail map);

		public MapDetail GetDetail(MapKey key)
		{
			foreach (var mapKey in this.MapDetails.Keys)
			{
				if (mapKey.Equals(key))
				{
					return this.MapDetails[mapKey];
				}
			}
			return null;
		}

		public MapDetail GetDetail(Point3 location)
		{
			foreach (var mapKey in this.MapDetails.Keys)
			{
				if ((location.X >= mapKey.StartX && location.X <= mapKey.EndX)
					&& (location.Y >= mapKey.StartY && location.Y <= mapKey.EndY))
				{
					return this.MapDetails[mapKey];
				}
			}
			return null;
		}

        public MapDetail GetDetail(string mapName)
        {
            return this.MapDetails.Values.Where(m => m.Name.ToLower().Equals(mapName.ToLower())).FirstOrDefault();
        }

		public class MapDetail
		{
			public string Name { get; set; }
			public int Width { get; set; }
			public int Height { get; set; }
			public MapKey Key { get; set; }
			public bool IsLoaded { get; set; }	

			public int CenterX
			{
				get { return this.Key.StartX + (int)((this.Key.EndX - this.Key.StartX) * 0.5); }
			}

			public int CenterY
			{
				get { return this.Key.StartY + (int)((this.Key.EndY - this.Key.StartY) * 0.5); }
			}

			public MapDetail(string name, int x, int y, int width, int height)
			{
				this.Name = name;
				this.Width = width;
				this.Height = height;
				this.Key = new MapKey(x, y, x + (width - 1), y + (height - 1));
			}

			public override string ToString()
			{
				return String.Format("{0} ( {1}, {2} )", this.Name, this.Key.StartX, this.Key.StartY);
			}
		}

		public class MapKey : IEquatable<MapKey>, IComparable<MapKey>, IComparable
		{
			public int StartX { get; set; }
			public int StartY { get; set; }
			public int EndX { get; set; }
			public int EndY { get; set; }	

			public MapKey(int startX, int startY, int endX, int endY)
			{
				this.StartX = startX;
				this.StartY = startY;
				this.EndX = endX;
				this.EndY = endY;
			}

			public override bool Equals(object obj)
			{
				if (obj != null && obj is MapKey)
				{
					return this.Equals((MapKey)obj);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return this.StartX.GetHashCode() + this.EndX.GetHashCode() + this.StartY.GetHashCode() + this.EndY.GetHashCode();
			}

			#region IEquatable<MapKey> Members

			public bool Equals(MapKey other)
			{
				if (other.StartX >= this.StartX && other.EndX <= this.EndX)
				{
					return (other.StartY >= this.StartY && other.EndY <= this.EndY);
				}
				return false;
			}

			#endregion

			#region IComparable<MapKey> Members

			public int CompareTo(MapKey other)
			{
				int startX = this.StartX.CompareTo(other.StartX);
				int startY = this.StartY.CompareTo(other.StartY);
				int endX = this.EndX.CompareTo(other.EndX);
				int endY = this.EndY.CompareTo(other.EndY);

				if (startX >= 0 && endX <= 0)
				{
					if (startY >= 0 && endY <= 0)
					{
						return 0;
					}
				}
				return -1;
			}

			#endregion

			#region IComparable Members

			public int CompareTo(object obj)
			{
				return this.CompareTo((MapKey)obj);
			}

			#endregion
		}

		public class MapKeyEqualityComparer : IEqualityComparer<MapKey>
		{
			#region IEqualityComparer<MapKey> Members

			public bool Equals(MapKey x, MapKey y)
			{
				return x.Equals(y);
			}

			public int GetHashCode(MapKey obj)
			{
				return obj.GetHashCode();
			}

			#endregion
		}
	}
}
