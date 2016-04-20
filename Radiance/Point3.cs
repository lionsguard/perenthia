using System;
using System.Collections.Generic;
using System.Text;

namespace Radiance
{
	public struct Point3 : IEquatable<Point3>, IComparable<Point3>, IComparable
	{
		public static readonly Point3 Empty = new Point3(0, 0, 0);

		public Point3(int x, int y, int z)
		{
			_x = x;
			_y = y;
			_z = z;
		}

		public void SetLocation(int x, int y)
		{
			this.SetLocation(x, y, this.Z);
		}

		public void SetLocation(int x, int y, int z)
		{
			_x = x;
			_y = y;
			_z = z;
		}

		public Point3 Copy()
		{
			return new Point3(this.X, this.Y, this.Z);
		}

		public static Point3 FromString(string value)
		{
			if (!String.IsNullOrEmpty(value))
			{
				string[] parts = value.Split(',');
				if (parts != null && parts.Length == 3)
				{
					int x, y, z;
					if (int.TryParse(parts[0], out x))
					{
						if (int.TryParse(parts[1], out y))
						{
							if (int.TryParse(parts[2], out z))
							{
								return new Point3(x, y, z);
							}
						}
					}
				}
			}
			return Point3.Empty;
		}

		#region GetHashCode
		public override int GetHashCode()
		{
			// The Y value should always come first in any kind of sorting, comparison or hashing operations
			// followed by X and then Z because typical loops would start with the Y value.
			return (this.Y.GetHashCode() + this.X.GetHashCode() + this.Z.GetHashCode());
		}
		#endregion

		#region Equals
		public bool Equals(Point3 obj)
		{
			// The Y value should always come first in any kind of sorting, comparison or hashing operations
			// followed by X and then Z because typical loops would start with the Y value.
			if (obj != null)
			{
				if (obj.Y == this.Y)
				{
					if (obj.X == this.X)
					{
						return (obj.Z == this.Z);
					}
				}
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is Point3)
			{
				return this.Equals((Point3)obj);
			}
			return base.Equals(obj);
		}
		#endregion

		#region ToString
		public override string ToString()
		{
			return ToString(false, true);
		}
		public string ToString(bool forDisplay)
		{
			return ToString(forDisplay, true);
		}
		public string ToString(bool forDisplay, bool includeZComponent)
		{
			if (forDisplay)
			{
				if (includeZComponent)
				{
					return String.Format("X = {0}, Y = {1}, Z = {2}", _x, _y, _z);
				}
				else
				{
					return String.Format("X = {0}, Y = {1}", _x, _y);
				}
			}
			return String.Format("{0},{1},{2}", _x, _y, _z);
		}
		#endregion

		#region Operators
		public static Point3 operator +(Point3 v1, Point3 v2)
		{
			return new Point3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
		}

		public static Point3 operator -(Point3 v1, Point3 v2)
		{
			return new Point3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
		}

		public static bool operator ==(Point3 v1, Point3 v2)
		{
			return v1.Equals(v2);
		}

		public static bool operator !=(Point3 v1, Point3 v2)
		{
			return (!v1.Equals(v2));
		}

		public static bool operator >=(Point3 v1, Point3 v2)
		{
			// The Y value should always come first in any kind of sorting, comparison or hashing operations
			// followed by X and then Z because typical loops would start with the Y value.
			if (v1.Y >= v2.Y)
			{
				if (v1.X >= v2.X)
				{
					return (v1.Z >= v2.Z);
				}
			}
			return false;
		}

		public static bool operator <=(Point3 v1, Point3 v2)
		{
			// The Y value should always come first in any kind of sorting, comparison or hashing operations
			// followed by X and then Z because typical loops would start with the Y value.
			if (v1.Y <= v2.Y)
			{
				if (v1.X <= v2.X)
				{
					return (v1.Z <= v2.Z);
				}
			}
			return false;
		}

		public static bool operator >(Point3 v1, Point3 v2)
		{
			// The Y value should always come first in any kind of sorting, comparison or hashing operations
			// followed by X and then Z because typical loops would start with the Y value.
			if (v1.Y > v2.Y)
			{
				if (v1.X > v2.X)
				{
					return (v1.Z > v2.Z);
				}
			}
			return false;
		}

		public static bool operator <(Point3 v1, Point3 v2)
		{
			// The Y value should always come first in any kind of sorting, comparison or hashing operations
			// followed by X and then Z because typical loops would start with the Y value.
			if (v1.Y < v2.Y)
			{
				if (v1.X < v2.X)
				{
					return (v1.Z < v2.Z);
				}
			}
			return false;
		}
		#endregion

		#region Properties
		private int _x;

		public int X
		{
			get { return _x; }
			set { _x = value; }
		}

		private int _y;

		public int Y
		{
			get { return _y; }
			set { _y = value; }
		}

		private int _z;

		public int Z
		{
			get { return _z; }
			set { _z = value; }
		}
		#endregion

		#region IEquatable<Position> Members

		bool IEquatable<Point3>.Equals(Point3 other)
		{
			return this.Equals(other);
		}

		#endregion

		#region IComparable Members

		public int CompareTo(object obj)
		{
			return this.CompareTo((Point3)obj);
		}

		#endregion

		#region IComparable<Position> Members

		int IComparable<Point3>.CompareTo(Point3 other)
		{
			int y = this.Y.CompareTo(other.Y);
			int x = this.X.CompareTo(other.X);
			int z = this.Z.CompareTo(other.Z);
			if (y == 0)
			{
				if (x == 0)
				{
					return z;
				}
				else
				{
					return x;
				}
			}
			else
			{
				return y;
			}
		}

		#endregion
	}
}
