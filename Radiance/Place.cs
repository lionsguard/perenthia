using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance
{
	#region Place
	/// <summary>
	/// Represents a place, space or room within the virtual world.
	/// </summary>
	public class Place : Actor
	{
		#region Properties
		/// <summary>
		/// Gets or sets the location of the current place.
		/// </summary>
		public Point3 Location
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
		/// Gets or sets the x coordinate of the current avatar.
		/// </summary>
		public int X
		{
			get { return this.Properties.GetValue<int>("X"); }
			set { this.Properties.SetValue("X", value); }
		}

		/// <summary>
		/// Gets or sets the y coordinate of the current avatar.
		/// </summary>
		public int Y
		{
			get { return this.Properties.GetValue<int>("Y"); }
			set { this.Properties.SetValue("Y", value); }
		}

		/// <summary>
		/// Gets or sets the z coordinate of the current avatar.
		/// </summary>
		public int Z
		{
			get { return this.Properties.GetValue<int>("Z"); }
			set { this.Properties.SetValue("Z", value); }
		}

		/// <summary>
		/// Gets or sets the value of the terrain of the current type.
		/// </summary>
		public int Terrain
		{	
			get { return this.Properties.GetValue<int>("Terrain"); }
			set { this.Properties.SetValue("Terrain", value); }
		}

		/// <summary>
		/// Gets a value indicating whether or not the current Place is safe, meaning combat can not occur here.
		/// </summary>
		public bool IsSafe
		{
			get { return this.Properties.GetValue<bool>(IsSafeProperty); }
			set { this.Properties.SetValue(IsSafeProperty, value); }
		}
		public static readonly string IsSafeProperty = "IsSafe";	
	
		/// <summary>
		/// Gets the collection of exits for the current place.
		/// </summary>
		public ExitCollection Exits { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating that this place has been loaded from the database.
		/// </summary>
		public bool IsLoaded { get; set; }
		#endregion

		/// <summary>
		/// Initializes a new instance of the Place class.
		/// </summary>
		public Place()
		{
			this.Init();
		}

        /// <summary>
        /// Initializes a new instance of the Place class from the specified RdlPlace and RdlProperty tags.
        /// </summary>
        /// <param name="placeTag">The RdlPlace tag containing the ID, Name and Location of the place.</param>
        public Place(RdlPlace placeTag)
            : base((RdlActor)placeTag)
        {
			this.Init();
            this.Location = new Point3(placeTag.X, placeTag.Y, placeTag.Z);
        }

		private void Init()
		{
			this.ObjectType = ObjectType.Place;
			this.Exits = new ExitCollection(this);
		}

		/// <summary>
		/// Gets a value indicating whether or not the current place has any exits.
		/// </summary>
		/// <returns>True if the current place has exists; otherwise false.</returns>
		public bool HasExits()
		{
			return this.Exits.GetKnownExits().Count > 0;
		}

		/// <summary>
		/// Exits the Avatar from a previous Place and enters the current place. Sends notification to other avatars in the Place.
		/// </summary>
		/// <param name="who">The Avatar entering the room.</param>
		/// <param name="direction">The direction from which the avatar is entering.</param>
		public void Enter(IAvatar who, Direction direction)
		{
			if (who.Place != null)
			{
				who.Place.Exit(who, direction.CounterDirection);
			}

			who.Place = this;
			this.Children.Add(who);

			// Clear the target of the current avatar and the avatar's target's target.
			if (who.Target != null)
			{
				IAvatar target = who.Target as IAvatar;
				if (target != null)
				{
					target.Target = null;
					target.Context.AddRange(target.GetRdlProperties(Avatar.TargetIDProperty));
				}
				who.Target = null;
				who.Context.AddRange(who.GetRdlProperties(Avatar.TargetIDProperty));
			}

			// Send down the new x,y,z of the current avatar.
			who.Context.AddRange(who.GetRdlProperties(Avatar.XProperty, Avatar.YProperty, Avatar.ZProperty));

			// Raise the OnEnter event on the current place.
			if (!who.IsBuilder)
			{
				this.OnEnter(who, who.Context, direction);

                if (who is IPlayer)
                {
                    (who as IPlayer).OnPlaceEntered();
                }

				// Raise an event on all avatar instances in the current place that a new avatar has entered.
				var avatars = (from c in this.Children where c is IAvatar select c as IAvatar);
				var whoRdl = who.ToSimpleRdl();
				foreach (var avatar in avatars)
				{
					if (avatar.ID != who.ID)
					{
						avatar.OnEnter(who, who.Context, direction);
					}
				}
			}

			// Cause the newly entered avatar to look at the place.
			this.Look(who);
		}

		/// <summary>
		/// Exits the specified avatar from the current place.
		/// </summary>
		/// <param name="who">The avatar leaving the current place.</param>
		/// <param name="direction">The direction in which the avatar is leaving.</param>
		public void Exit(IAvatar who, Direction direction)
		{
			// Remove the actor from the current place.
			this.Children.Remove(who);

			// Raise the OnExit event on the current place.
			if (direction.KnownDirection != KnownDirection.None && !who.IsBuilder)
			{
				this.OnExit(who, who.Context, direction);

                if (who is IPlayer)
                {
                    (who as IPlayer).OnPlaceExited();
                }

				// Raise an event on all player instances in the current place that a new player has entered.
				var avatars = (from c in this.Children where c is IAvatar select c as IAvatar);
				foreach (var avatar in avatars)
				{
					if (avatar.ID != who.ID)
					{
						avatar.OnExit(who, who.Context, direction);
					}
				}
			}
		}

		/// <summary>
		/// Allows the specified Avatar to view the current place.
		/// </summary>
		/// <param name="who">The avatar viewing the current place.</param>
		public void Look(IAvatar who)
		{
			//who.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.PlaceDescription, this.ToString(who)));
			this.WritePlaceDetails(who);

			// Send down the list of avatars and actors here to allow clients to interact with the specific IDs of the
			// objects.
			foreach (var item in this.GetVisibleChildren(who).Where(c => c.ID != who.ID))
			{
				who.Context.AddRange(item.ToSimpleRdl());
			}
		}

		/// <summary>
		/// Gets the child objects of the current place that are visible to specified IAvatar.
		/// </summary>
		/// <param name="who">The IAvatar in the current place.</param>
		/// <returns>An enumerable list of child IActor instances visible to the specified IAvatar.</returns>
		protected virtual IEnumerable<IActor> GetVisibleChildren(IAvatar who)
		{
			return this.Children;
		}

		/// <summary>
		/// Sends the specified message to all avatars in the current place.
		/// </summary>
		/// <param name="tag">The RdlTag to send to all avatars in the place.</param>
		/// <param name="sender">The avatar sending the message.</param>
		public void SendAll(RdlTag tag, IAvatar sender)
		{
			var avatars = (from c in this.Children where c is Avatar select c as Avatar);
			RdlObject[] senderRdl = sender.ToSimpleRdl();
			foreach (var avatar in avatars)
			{
				if (avatar.ID != sender.ID)
				{
					avatar.Context.Add(tag);
				}
			}
		}

		protected virtual void WritePlaceDetails(IAvatar who)
		{
			StringBuilder sb = new StringBuilder();

			//=====================================================================================
			// Name and Description
			//=====================================================================================
			who.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.PlaceName, this.Name));
			who.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.PlaceDescription, this.Description));

			//=====================================================================================
			// Exits
			//=====================================================================================
			List<KnownDirection> exits = this.Exits.GetKnownExits();
			if (exits.Count > 1)
			{
				sb.Append("You see exits leading ");
				for (int i = 0; i < exits.Count; i++)
				{
					if (i > 0)
					{
						if ((i + 1) < exits.Count) sb.Append(", ");
						if ((i + 1) == exits.Count) sb.Append(" and ");
					}
					sb.Append(exits[i]);
				}
				sb.Append(".");
			}
			else if (exits.Count == 1)
			{
				sb.AppendFormat("You see an exit leading {0}.", exits[0]);
			}
			if (exits.Count > 0) sb.Append(Environment.NewLine);
			who.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.PlaceExits, sb.ToString()));

			//=====================================================================================
			// Avatars
			//=====================================================================================
			sb = new StringBuilder();
			var avatars = (from c in this.Children where c is IAvatar && c.ID != who.ID select c as IAvatar).ToList();
			if (avatars.Count > 1)
			{
				for (int i = 0; i < avatars.Count; i++)
				{
					if (i > 0)
					{
						if ((i + 1) < avatars.Count) sb.Append(", ");
						if ((i + 1) == avatars.Count) sb.Append(" and ");
					}
					sb.Append(avatars[i].A());
				}
				sb.Append(" are here.");
			}
			else if (avatars.Count == 1)
			{
				sb.AppendFormat("{0} is here.", avatars[0].A());
			}
			if (avatars.Count > 0) sb.Append(Environment.NewLine);
			who.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.PlaceAvatars, sb.ToString()));

			//=====================================================================================
			// Other Actors...
			//=====================================================================================
			sb = new StringBuilder();
			var actors = (from c in this.Children where (!(c is IAvatar)) && c.ID != who.ID select c).ToList();
			if (actors.Count > 0)
			{
				sb.Append("You see ");
				for (int i = 0; i < actors.Count; i++)
				{
					if (i > 0)
					{
						if ((i + 1) < actors.Count) sb.Append(", ");
						if ((i + 1) == actors.Count) sb.Append(" and ");
					}
					sb.Append(actors[i].A());
				}
				sb.Append(Environment.NewLine);
			}
			sb.Append(Environment.NewLine);
			who.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.PlaceActors, sb.ToString()));
		}

		/// <summary>
		/// Returns the string representation of the current place instance.
		/// </summary>
		/// <returns>The string representation of the current place instance.</returns>
		public override string ToString()
		{
			return this.ToString(null);
		}

		/// <summary>
		/// Returns the string representation of the current place instance.
		/// </summary>
		/// <param name="who">The Avatar requesting the details of the current place.</param>
		/// <returns>The string representation of the current place instance.</returns>
		public string ToString(IAvatar who)
		{
			StringBuilder sb = new StringBuilder();

			//=====================================================================================
			// Name and Description
			//=====================================================================================
			sb.Append(Environment.NewLine).Append(this.Name).Append(Environment.NewLine);
			sb.Append(this.Description).Append(Environment.NewLine);

			//=====================================================================================
			// Exits
			//=====================================================================================
			List<KnownDirection> exits = this.Exits.GetKnownExits();
			if (exits.Count > 1)
			{
				sb.Append("You see exits leading ");
				for (int i = 0; i < exits.Count; i++)
				{
					if (i > 0)
					{
						if ((i + 1) < exits.Count) sb.Append(", ");
						if ((i + 1) == exits.Count) sb.Append(" and ");
					}
					sb.Append(exits[i]);
				}
				sb.Append(".");
			}
			else if (exits.Count == 1)
			{
				sb.AppendFormat("You see an exit leading {0}.", exits[0]);
			}
			if (exits.Count > 0) sb.Append(Environment.NewLine);

			//=====================================================================================
			// Avatars
			//=====================================================================================
			var avatars = (from c in this.Children where c is IAvatar && c.ID != who.ID select c as IAvatar).ToList();
			if (avatars.Count > 1)
			{
				for (int i = 0; i < avatars.Count; i++)
				{
					if (i > 0)
					{
						if ((i + 1) < avatars.Count) sb.Append(", ");
						if ((i + 1) == avatars.Count) sb.Append(" and ");
					}
					sb.Append(avatars[i].A());
				}
				sb.Append(" are here.");
			}
			else if (avatars.Count == 1)
			{
				sb.AppendFormat("{0} is here.", avatars[0].A());
			}
			if (avatars.Count > 0) sb.Append(Environment.NewLine);

			//=====================================================================================
			// Other Actors...
			//=====================================================================================
			var actors = (from c in this.Children where (!(c is IAvatar)) && c.ID != who.ID select c).ToList();
			if (actors.Count > 0)
			{
				sb.Append("You see ");
				for (int i = 0; i < actors.Count; i++)
				{
					if (i > 0)
					{
						if ((i + 1) < actors.Count) sb.Append(", ");
						if ((i + 1) == actors.Count) sb.Append(" and ");
					}
					sb.Append(actors[i].A());
				}
				sb.Append(Environment.NewLine);
			}


			// Just a new line for some spacing.
			sb.Append(Environment.NewLine);

			return sb.ToString();
		}

        protected override RdlActor GetObjectTag()
        {
            return new RdlPlace(this.ID, this.Name, this.X, this.Y, this.Z);
        }

		/// <summary>
		/// Updates the climate, traps, quests, mobiles, etc. within the current room.
		/// </summary>
		public virtual void Update()
		{
		}
	}
	#endregion

	#region PlaceCollection
	/// <summary>
	/// Represens a dictionary of Place objects where the Place.Location is the key.
	/// </summary>
	public class PlaceCollection : Dictionary<Point3, Place>
	{
		private World _world;

		public new Place this[Point3 key]
		{
			get
			{
				Place place = null;
				if (!this.ContainsKey(key))
				{
					// Load the object from the database.
					place = _world.Provider.GetPlace(key);
					if (place != null)
					{
						place.IsLoaded = true;
						this.Add(key, place);
					}
				}
				else
				{
					place = base[key];
					if (place != null && !place.IsLoaded)
					{
						place = _world.Provider.GetPlace(key);
						place.IsLoaded = true;
						base[key] = place;
					}
				}
				if (place == null)
				{
					// Should never return null.
					place = new Place { Name = "Wilderness", Description = String.Empty, Location = key };
				}
				return place;
			}
			set
			{
				this.Add(key, value);
			}
		}

		/// <summary>
		/// Initializes a new instance of the PlaceCollection class and specifies the current world that owns the collection.
		/// </summary>
		/// <param name="world"></param>
		public PlaceCollection(World world)
			: base()
		{
			_world = world;
		}

		public new void Add(Point3 key, Place value)
		{
			value.World = _world;
			if (this.ContainsKey(key))
			{
				base[key] = value;
			}
			else
			{
				base.Add(key, value);
			}
		}
	}
	#endregion

	#region ExitCollection
	/// <summary>
	/// Represents a collection of exits for a place.
	/// </summary>
	public sealed class ExitCollection
	{
		private static readonly string Prefix = "Exit_";
		private Place _place;

		/// <summary>
		/// Initializes a new instance of the exit collection and sets the owner place instance.
		/// </summary>
		/// <param name="place">The place this collection of exits applies.</param>
		public ExitCollection(Place place)
		{
			_place = place;
		}

		/// <summary>
		/// Sets all of the exit values to the specified value.
		/// </summary>
		/// <param name="value">The value to set all the exits.</param>
		public void SetAll(bool value)
		{
			this.North = this.Northeast = this.Northwest = this.South = this.Southeast = this.Southwest = 
				this.East = this.West = this.Up = this.Down = value;
		}

		/// <summary>
		/// Gets a list of KnownDirection values for the exits in the current collection.
		/// </summary>
		/// <returns>A list of KnownDirection values for the exits in the current collection.</returns>
		public List<KnownDirection> GetKnownExits()
		{
			List<KnownDirection> list = new List<KnownDirection>();
			Array values = Enum.GetValues(typeof(KnownDirection));
			for (int i = 0; i < values.Length; i++)
			{
				if (this.GetValue((KnownDirection)values.GetValue(i)))
				{
					list.Add((KnownDirection)i);
				}
			}
			return list;
		}

		/// <summary>
		/// Sets the exit value at the specified direction.
		/// </summary>
		/// <param name="direction">The KnownDirection value at which to set the exit.</param>
		/// <param name="value">A value indicating whether or not an exit exists.</param>
		public void SetValue(KnownDirection direction, bool value)
		{
			_place.Properties.SetValue(String.Concat(Prefix, direction), value);
		}

		/// <summary>
		/// Gets a value indicating whether or not an exit exists at the specified direction.
		/// </summary>
		/// <param name="direction">The KnownDirection value at which to get the exit exists value.</param>
		/// <returns>True if an exit exists in the specified direction; otherwise false.</returns>
		public bool GetValue(KnownDirection direction)
		{
			return _place.Properties.GetValue<bool>(String.Concat(Prefix, direction));
		}

		/// <summary>
		/// Gets a value indicating whether or not an exit exists at the specified direction.
		/// </summary>
		/// <param name="direction">The string value at which to get the exit exists value.</param>
		/// <returns>True if an exit exists in the specified direction; otherwise false.</returns>
		public bool GetValue(string direction)
		{
			return _place.Properties.GetValue<bool>(String.Concat(Prefix, direction));
		}

        /// <summary>
        /// Gets or sets the exit value with the specified direction name.
        /// </summary>
        /// <param name="direction">The name of the direction.</param>
        /// <returns>True if an exit exists; otherwise false.</returns>
        public bool this[string direction]
        {
            get { return this.GetValue((KnownDirection)Enum.Parse(typeof(KnownDirection), direction, true)); }
            set { this.SetValue((KnownDirection)Enum.Parse(typeof(KnownDirection), direction, true), value); }
        }

		/// <summary>
		/// Gets a value indicating whether or not an exit exists to the north.
		/// </summary>
		public bool North
		{
			get { return this.GetValue(KnownDirection.North); }
			set { this.SetValue(KnownDirection.North, value); }
		}

		/// <summary>
		/// Gets a value indicating whether or not an exit exists to the south.
		/// </summary>
		public bool South
		{
			get { return this.GetValue(KnownDirection.South); }
			set { this.SetValue(KnownDirection.South, value); }
		}

		/// <summary>
		/// Gets a value indicating whether or not an exit exists to the east.
		/// </summary>
		public bool East
		{
			get { return this.GetValue(KnownDirection.East); }
			set { this.SetValue(KnownDirection.East, value); }
		}

		/// <summary>
		/// Gets a value indicating whether or not an exit exists to the west.
		/// </summary>
		public bool West
		{
			get { return this.GetValue(KnownDirection.West); }
			set { this.SetValue(KnownDirection.West, value); }
		}

		/// <summary>
		/// Gets a value indicating whether or not an exit exists to the northeast.
		/// </summary>
		public bool Northeast
		{
			get { return this.GetValue(KnownDirection.Northeast); }
			set { this.SetValue(KnownDirection.Northeast, value); }
		}

		/// <summary>
		/// Gets a value indicating whether or not an exit exists to the northwest.
		/// </summary>
		public bool Northwest
		{
			get { return this.GetValue(KnownDirection.Northwest); }
			set { this.SetValue(KnownDirection.Northwest, value); }
		}

		/// <summary>
		/// Gets a value indicating whether or not an exit exists to the southeast.
		/// </summary>
		public bool Southeast
		{
			get { return this.GetValue(KnownDirection.Southeast); }
			set { this.SetValue(KnownDirection.Southeast, value); }
		}

		/// <summary>
		/// Gets a value indicating whether or not an exit exists to the southwest.
		/// </summary>
		public bool Southwest
		{
			get { return this.GetValue(KnownDirection.Southwest); }
			set { this.SetValue(KnownDirection.Southwest, value); }
		}

		/// <summary>
		/// Gets a value indicating whether or not an exit exists upwards.
		/// </summary>
		public bool Up
		{
			get { return this.GetValue(KnownDirection.Up); }
			set { this.SetValue(KnownDirection.Up, value); }
		}

		/// <summary>
		/// Gets a value indicating whether or not an exit exists downwards.
		/// </summary>
		public bool Down
		{
			get { return this.GetValue(KnownDirection.Down); }
			set { this.SetValue(KnownDirection.Down, value); }
		}
	}
	#endregion
}
