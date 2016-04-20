using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance
{
	/// <summary>
	/// Represents a race within a virtual world, containing properties and methods common to all races of the virtual world.
	/// </summary>
	public class Race : Actor
	{
		/// <summary>
		/// Gets or sets the starting location of the current race.
		/// </summary>
		public Point3 StartingLocation
		{
			get
			{
				return new Point3
				{
					X = this.Properties.GetValue<int>("X"),
					Y = this.Properties.GetValue<int>("Y"),
					Z = this.Properties.GetValue<int>("Z")
				};
			}
			set
			{
				this.Properties.SetValue("X", value.X);
				this.Properties.SetValue("Y", value.Y);
				this.Properties.SetValue("Z", value.Z);
			}
		}

		/// <summary>
		/// Gets or sets the starting x coordinate of the current race.
		/// </summary>
		public int X
		{
			get { return this.Properties.GetValue<int>("X"); }
			set { this.Properties.SetValue("X", value); }
		}

		/// <summary>
		/// Gets or sets the starting y coordinate of the current race.
		/// </summary>
		public int Y
		{
			get { return this.Properties.GetValue<int>("Y"); }
			set { this.Properties.SetValue("Y", value); }
		}

		/// <summary>
		/// Gets or sets the starting z coordinate of the current race.
		/// </summary>
		public int Z
		{
			get { return this.Properties.GetValue<int>("Z"); }
			set { this.Properties.SetValue("Z", value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the current race is playable.
		/// </summary>
		public bool IsPlayable
		{
			get { return this.Properties.GetValue<bool>(IsPlayableProperty); }
			set { this.Properties.SetValue(IsPlayableProperty, value); }
		}
		public static readonly string IsPlayableProperty = "IsPlayable";
	
		/// <summary>
		/// Gets the collection of attributes modified by the current race.
		/// </summary>
		public AttributeList Attributes { get; private set; }

		/// <summary>
		/// Gets the collection of bonus skills provided by the current race.
		/// </summary>
		public SkillList Skills { get; private set; }

		/// <summary>
		/// Initializes a new instance of the Race class.
		/// </summary>
		public Race()
		{
			this.ObjectType = ObjectType.Race;
			this.StartingLocation = Point3.Empty;
			this.Attributes = new AttributeList(this);
			this.Skills = new SkillList(this);
		}
	}

	/// <summary>
	/// Represents a dictionary of Race instances where th race name is the key and Race instance is the value.
	/// </summary>
	public class RaceDictionary : Dictionary<string, Race>
	{
		public World World { get; private set; }

		public new Race this[string key]
		{
			get { return base[key]; }
			set { this.Add(key, value); }
		}

		/// <summary>
		/// Initializes a new instance of the RaceDictionary class.
		/// </summary>
		/// <param name="world">The current virtual world.</param>
		public RaceDictionary(World world)
			: base(StringComparer.InvariantCultureIgnoreCase)
		{
			this.World = world;
		}

		public new void Add(string key, Race value)
		{
			value.World = this.World;
			if (!this.ContainsKey(key))
			{
				base.Add(key, value);
			}
			else
			{
				base[key] = value;
			}
		}

		public RdlObject[] ToRdl()
		{
			List<RdlObject> list = new List<RdlObject>();
			foreach (var item in this)
			{
				list.Add(new RdlRace(item.Value.ID, item.Value.Name, item.Value.Description));
				list.AddRange(item.Value.Attributes.ToRdl());
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			RdlTagCollection tags = new RdlTagCollection();
			tags.AddRange(this.ToRdl());
			return tags.ToString();
		}
	}
}
