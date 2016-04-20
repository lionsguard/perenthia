using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Radiance
{
	/// <summary>
	/// Provides the methods and properties of the Radiance.Direction class.
	/// </summary>
	public struct Direction
	{
		private string _name;
		private string _counterName;
		private string[] _altNames;
		private Point3 _value;
		private KnownDirection _knownDirection;

		/// <summary>
		/// An empty instance of the Direction structure.
		/// </summary>
		public static readonly Direction Empty;

		/// <summary>
		/// Initializes the statis instance of the Direction structure.
		/// </summary>
		static Direction()
		{
			Direction.Empty = new Direction("Void", Point3.Empty, KnownDirection.None, "Void", "");
		}

		/// <summary>
		/// Initializes a new instance of the Radiance.Direction structure.
		/// </summary>
		/// <param name="name">The name of the direction.</param>
		/// <param name="value">The Position value of the direction.</param>
		/// <param name="knownDirection">The KnownDirection enumration value for the new direction.</param>
		/// <param name="counterDirectionName">The name of the direction exaclty opposite of this direction.</param>
		/// <param name="altNames">Alternate names for the direction.</param>
		public Direction(string name, Point3 value, KnownDirection knownDirection, string counterDirectionName, params string[] altNames)
		{
			_name = name;
			_counterName = counterDirectionName;
			_altNames = altNames;
			_value = value;
			_knownDirection = knownDirection;
		}



		#region GetHashCode
		public override int GetHashCode()
		{
			return (this._knownDirection.GetHashCode());
		}
		#endregion

		#region Equals
		public bool Equals(Direction obj)
		{
			if (!String.IsNullOrEmpty(obj.Name) && !String.IsNullOrEmpty(this.Name))
			{
				return (obj.Name.Equals(this.Name));
			}
			return obj.Value.Equals(this.Value);
		}

		public override bool Equals(object obj)
		{
			if (obj is Direction)
			{
				return this.Equals((Direction)obj);
			}
			return base.Equals(obj);
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static Direction FromName(string name)
		{
			Direction obj = DirectionConverter.GetNamedDirection(name);
			if (!String.IsNullOrEmpty(obj.Name) && !obj.Name.Equals(Direction.Empty.Name))
			{
				return obj;
			}
			return Direction.FromAlternateName(name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="altName"></param>
		/// <returns></returns>
		public static Direction FromAlternateName(string altName)
		{
			return DirectionConverter.GetAltNamedDirection(altName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="knownDirection"></param>
		/// <returns></returns>
		public static Direction FromKnownDirection(KnownDirection knownDirection)
		{
			return Direction.FromName(knownDirection.ToString());
		}

		public static Direction FromPosition(Point3 current, Point3 destination)
		{
			KnownDirection dir = KnownDirection.North;
			if (current.Y > destination.Y)
			{
				// North
				dir = KnownDirection.North;
			}
			else if (current.Y < destination.Y)
			{
				// South
				dir = KnownDirection.South;
			}

			if (current.X > destination.X)
			{
				// West
				if (dir == KnownDirection.North)
				{
					dir = KnownDirection.Northwest;
				}
				else if (dir == KnownDirection.South)
				{
					dir = KnownDirection.Southwest;
				}
				else
				{
					dir = KnownDirection.West;
				}
			}
			else if (current.X < destination.X)
			{
				// East
				if (dir == KnownDirection.North)
				{
					dir = KnownDirection.Northeast;
				}
				else if (dir == KnownDirection.South)
				{
					dir = KnownDirection.Southeast;
				}
				else
				{
					dir = KnownDirection.East;
				}
			}

			if (current.Z > destination.Z)
			{
				// Down
				dir = KnownDirection.Down;
			}
			else if (current.Z < destination.Z)
			{
				// Up
				dir = KnownDirection.Up;
			}

			return Direction.FromKnownDirection(dir);
		}

		/// <summary>
		/// The full name of the direction.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// The full name of the direction exactly opposite of this direction.
		/// </summary>
		public string CounterName
		{
			get { return _counterName; }
			set { _counterName = value; }
		}

		/// <summary>
		/// Gets the Direction that is exactly opposite of the current Direction.
		/// </summary>
		public Direction CounterDirection
		{
			get { return Direction.FromName(this.CounterName); }
		}

		/// <summary>
		/// An array of alternate names for the direction.
		/// </summary>
		public string[] AlternateNames
		{
			get { return _altNames; }
			set { _altNames = value; }
		}

		/// <summary>
		/// Gets the Position value of the current Radiance.Direction instance.
		/// </summary>
		public Point3 Value
		{
			get { return _value; }
			set { _value = value; }
		}	

		public KnownDirection KnownDirection
		{
			get { return _knownDirection; }
			set { _knownDirection = value; }
		}
	


		/// <summary>
		/// Gets an instance of the Direction object.
		/// </summary>
		public static Direction North
		{
			get { return new Direction("North", new Point3(0, -1, 0), KnownDirection.North, "South", "N"); }
		}

		/// <summary>
		/// Gets an instance of the Direction object.
		/// </summary>
		public static Direction South
		{
			get { return new Direction("South", new Point3(0, 1, 0), KnownDirection.South, "North", "S"); }
		}

		/// <summary>
		/// Gets an instance of the Direction object.
		/// </summary>
		public static Direction East
		{
			get { return new Direction("East", new Point3(1, 0, 0), KnownDirection.East, "West", "E"); }
		}

		/// <summary>
		/// Gets an instance of the Direction object.
		/// </summary>
		public static Direction West
		{
			get { return new Direction("West", new Point3(-1, 0, 0), KnownDirection.West, "East", "W"); }
		}

		/// <summary>
		/// Gets an instance of the Direction object.
		/// </summary>
		public static Direction Northeast
		{
			get { return new Direction("Northeast", new Point3(1, -1, 0), KnownDirection.Northeast, "Southwest", "NE"); }
		}

		/// <summary>
		/// Gets an instance of the Direction object.
		/// </summary>
		public static Direction Northwest
		{
			get { return new Direction("Northwest", new Point3(-1, -1, 0), KnownDirection.Northwest, "Southeast", "NW"); }
		}

		/// <summary>
		/// Gets an instance of the Direction object.
		/// </summary>
		public static Direction Southeast
		{
			get { return new Direction("Southeast", new Point3(1, 1, 0), KnownDirection.Southeast, "Northwest", "SE"); }
		}

		/// <summary>
		/// Gets an instance of the Direction object.
		/// </summary>
		public static Direction Southwest
		{
			get { return new Direction("Southwest", new Point3(-1, 1, 0), KnownDirection.Southwest, "Northeast", "SW"); }
		}

		/// <summary>
		/// Gets an instance of the Direction object.
		/// </summary>
		public static Direction Up
		{
			get { return new Direction("Up", new Point3(0, 0, 1), KnownDirection.Up, "Down", "U"); }
		}

		/// <summary>
		/// Gets an instance of the Direction object.
		/// </summary>
		public static Direction Down
		{
			get { return new Direction("Down", new Point3(0, 0, -1), KnownDirection.Down, "Up", "D"); }
		}
	}

	/// <summary>
	/// Provides the methods and properties of the Radiance.DirectionConverter class.
	/// </summary>
	public static class DirectionConverter
	{
		private static string DirectionConstantsLock;
		private static string DirectionAltConstantsLock;
		private static Dictionary<string, Direction> _directionConstants;
		private static Dictionary<string, Direction> _directionAltConstants;

		/// <summary>
		/// Initializes the static instance of the DirectionConverter class.
		/// </summary>
		static DirectionConverter()
		{
			DirectionConverter.DirectionConstantsLock = "directionConstants";
			DirectionConverter.DirectionAltConstantsLock = "directionAltConstants";
		}

		/// <summary>
		/// Gets a named direction.
		/// </summary>
		/// <param name="name">The name of the direction to find.</param>
		/// <returns>An object instance of a Direction.</returns>
		internal static Direction GetNamedDirection(string name)
		{
			if (DirectionConverter.Directions.ContainsKey(name))
				return DirectionConverter.Directions[name];
			else
				return Direction.Empty;
		}

		/// <summary>
		/// Gets a named Direction from the specified alternate name.
		/// </summary>
		/// <param name="altName">The alternate name of the direction.</param>
		/// <returns>An object instance of a Direction.</returns>
		internal static Direction GetAltNamedDirection(string altName)
		{
			if (DirectionConverter.DirectionsAlt.ContainsKey(altName))
				return DirectionConverter.DirectionsAlt[altName];
			else
				return Direction.Empty;
		}

		private static void FillConstants(Dictionary<string, Direction> hash, Dictionary<string, Direction> hashAlt, Type enumType)
		{
			MethodAttributes attributes1 = MethodAttributes.Static | MethodAttributes.Public;
			PropertyInfo[] infoArray1 = enumType.GetProperties();
			for (int num1 = 0; num1 < infoArray1.Length; num1++)
			{
				PropertyInfo info1 = infoArray1[num1];
				if (info1.PropertyType == typeof(Direction))
				{
					MethodInfo info2 = info1.GetGetMethod();
					if ((info2 != null) && ((info2.Attributes & attributes1) == attributes1))
					{
						object[] objArray1 = null;
						Direction obj = (Direction)info1.GetValue(null, objArray1);
						hash[info1.Name] = obj;

						// Need to fill the alt names hash.
						foreach (string alt in obj.AlternateNames)
						{
							hashAlt[alt] = obj;
						}
					}
				}
			}
		}

		internal static Dictionary<string, Direction> Directions
		{
			get
			{
				if (DirectionConverter._directionConstants == null)
				{
					lock (DirectionConverter.DirectionConstantsLock)
					{
						if (DirectionConverter._directionConstants == null)
						{
							Dictionary<string, Direction> hash = new Dictionary<string, Direction>(StringComparer.InvariantCultureIgnoreCase);
							Dictionary<string, Direction> hashAlt = new Dictionary<string, Direction>(StringComparer.InvariantCultureIgnoreCase);
							DirectionConverter.FillConstants(hash, hashAlt, typeof(Direction));
							DirectionConverter._directionConstants = hash;
							DirectionConverter._directionAltConstants = hashAlt;
						}
					}
				}
				return DirectionConverter._directionConstants;
			}
		}

		internal static Dictionary<string, Direction> DirectionsAlt
		{
			get
			{
				if (DirectionConverter._directionAltConstants == null)
				{
					lock (DirectionConverter.DirectionAltConstantsLock)
					{
						if (DirectionConverter._directionAltConstants == null)
						{
							Dictionary<string, Direction> hash = new Dictionary<string, Direction>(StringComparer.InvariantCultureIgnoreCase);
							Dictionary<string, Direction> hashAlt = new Dictionary<string, Direction>(StringComparer.InvariantCultureIgnoreCase);
							DirectionConverter.FillConstants(hash, hashAlt, typeof(Direction));
							DirectionConverter._directionConstants = hash;
							DirectionConverter._directionAltConstants = hashAlt;
						}
					}
				}
				return DirectionConverter._directionAltConstants;
			}
		}
	}
}
