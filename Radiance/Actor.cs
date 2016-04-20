using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance
{
	#region Actor
	/// <summary>
	/// Represents the abstract base class for objects that interact with the game engine.
	/// </summary>
	public class Actor : GameObject, IActor, ICloneable
	{
		#region Properties
		/// <summary>
		/// Gets or sets the unique identifier of the template this instance is derived from. Not required.
		/// </summary>
		public int TemplateID { get; set; }

		/// <summary>
		/// Gets or sets the type or category of the current object.
		/// </summary>
		public ObjectType ObjectType
		{	
			get { return this.Properties.GetValue<ObjectType>(ObjectTypeProperty); }
			set { this.Properties.SetValue(ObjectTypeProperty, value, true); }
		}
		public static readonly string ObjectTypeProperty = "ObjectType";

		private IActor _owner;
		/// <summary>
		/// Gets or sets the owner of the current Actor instance.
		/// </summary>
		public IActor Owner
		{	
			get 
			{
				if (_owner == null)
				{
					int id = this.Properties.GetValue<int>(OwnerIDProperty);
					if (id > 0 && this.World != null)
					{
						_owner = this.World.GetActor(id);
					}
				}
				return _owner; 
			}
			set 
			{
				if (value != null)
				{
					this.Properties.SetValue(OwnerIDProperty, value.ID);
				}
				else
				{
					if (_owner != null)
					{
						_owner.Children.Remove(this);
					}
					this.Properties.SetValue(OwnerIDProperty, 0);
				}
				_owner = value;
			}
		}
		public static readonly string OwnerIDProperty = "OwnerID";

		/// <summary>
		/// Gets or sets a description of the Actor.
		/// </summary>
		public virtual string Description
		{	
			get { return this.Properties.GetValue<string>(DescriptionProperty); }
			set { this.Properties.SetValue(DescriptionProperty, value); }
		}
		public static readonly string DescriptionProperty = "Description";
	

		/// <summary>
		/// Gets a value indicating whether or not the actor is dead or destroyed.
		/// </summary>
		public bool IsDead
		{	
			get { return this.Properties.GetValue<bool>(IsDeadProperty); }
			set { this.Properties.SetValue(IsDeadProperty, value); }
		}
		public static readonly string IsDeadProperty = "IsDead";

		/// <summary>
		/// Gets a value indicating the physical health of the actor.
		/// </summary>
		public virtual int Body
		{
			get { return this.Properties.GetValue<int>(BodyProperty); }
			protected set { this.Properties.SetValue(BodyProperty, value); }
		}
		public static readonly string BodyProperty = "Body";	

		/// <summary>
		/// Gets a value indicating the maximum physical health of the actor.
		/// </summary>
		public virtual int BodyMax
		{
			get { return this.Properties.GetValue<int>(BodyMaxProperty); }
			protected set { this.Properties.SetValue(BodyMaxProperty, value); }
		}
		public static readonly string BodyMaxProperty = "BodyMax";

		/// <summary>
		/// Gets or sets any flags on the current actor.
		/// </summary>
		public ActorFlags Flags
		{	
			get { return this.Properties.GetValue<ActorFlags>(FlagsProperty); }
			set { this.Properties.SetValue(FlagsProperty, value); }
		}
		public static readonly string FlagsProperty = "Flags";

		/// <summary>
		/// Gets the value of the named property in the Properties collection.
		/// </summary>
		/// <param name="name">The name of property value to retrieve.</param>
		/// <returns>The value of the named property.</returns>
		public object this[string name]
		{
			get { return this.Properties[name].Value; }
			set { this.Properties[name].Value = value; }
		}

		private World _world = null;
		/// <summary>
		/// Gets the current virtual world of the actor.
		/// </summary>
		public World World
		{
			get { return _world; }
			set
			{
				_world = value;
				this.OnPropertyChanged("World");
			}
		}

		/// <summary>
		/// Gets the collection of Actor instances owned by the current actor.
		/// </summary>
		public ActorCollection Children { get; private set; }

		/// <summary>
		/// Gets a value indicating whether or not the name of the Actor is a proper name.
		/// </summary>
		public bool HasProperName
		{	
			get { return this.Properties.GetValue<bool>(HasProperNameProperty); }
			set { this.Properties.SetValue(HasProperNameProperty, value); }
		}
		public static readonly string HasProperNameProperty = "HasProperName";

		/// <summary>
		/// Gets a list of property names to ignore when rendering the current Actor as RDL.
		/// </summary>
		protected List<string> RdlIgnoreProperties { get; private set; }

		/// <summary>
		/// Gets the Alias name for the current Actor.
		/// </summary>
		public string Alias
		{
			get { return this.Alias(); }
		}

		/// <summary>
		/// Gets or sets the URI of the image used to represent the current Actor instance.
		/// </summary>
		public virtual string ImageUri
		{
			get { return this.Properties.GetValue<string>(ImageUriProperty); }
			set { this.Properties.SetValue(ImageUriProperty, value); }
		}
		public static readonly string ImageUriProperty = "ImageUri";

		private object _lock = new object();
		#endregion

		#region Events
		/// <summary>
		/// An event that is raised when another Actor enters or makes contact with the current Actor.
		/// </summary>
		public virtual void OnEnter(IActor sender, IMessageContext context, Direction direction)
		{	
		}

		/// <summary>
		/// An event that is raised when another Actor exits or leaves contact with the current Actor.
		/// </summary>
		public virtual void OnExit(IActor sender, IMessageContext context, Direction direction)
		{
		}

		/// <summary>
		/// An event that is raised when a buff enhancement is applied the current Actor.
		/// </summary>
		public virtual void OnBuff(IActor sender, IMessageContext context, Foci foci)
		{
		}

		/// <summary>
		/// An event that is raised when damage is applied to the current Actor.
		/// </summary>
		public virtual void OnDamage(IActor sender, IMessageContext context, Foci foci)
		{
		}

		/// <summary>
		/// An event that is raised when an enchantment is applied to the current Actor.
		/// </summary>
		public virtual void OnEnchant(IActor sender, IMessageContext context, Foci foci)
		{
		}

		/// <summary>
		/// An event that is raised when healing has been applied to the current Actor.
		/// </summary>
		public virtual void OnHeal(IActor sender, IMessageContext context, Foci foci)
		{
		}

		/// <summary>
		/// Raised internally when the Actor has been loaded from the database.
		/// </summary>
		public virtual void OnLoadComplete()
		{
		}

		/// <summary>
		/// Raises internally when a property has changed and has been specifically set to raise this event.
		/// </summary>
		public virtual void OnPropertyChanged(string name)
		{
		}
		#endregion

		/// <summary>
		/// Initializes a new instance of the Actor class.
		/// </summary>
		public Actor()
            : base()
		{
			this.Children = new ActorCollection(this);
			this.RdlIgnoreProperties = new List<string>();
			this.ObjectType = ObjectType.Actor;
			this.Flags = ActorFlags.None;
		}

        /// <summary>
        /// Initializes a new instance of the Actor class from an RdlActor and RdlProperty tags.
        /// </summary>
        /// <param name="actorTag">The RdlActor tag used to set the ID and Name of the actor.</param>
        public Actor(RdlActor actorTag)
            : this()
        {
            this.ID = actorTag.ID;
            this.Name = actorTag.Name;

            foreach (var item in actorTag.Properties)
            {
                this.Properties.SetValue(item.Name, item.Value);
            }
		}

		#region Save
		/// <summary>
		/// Persists the current Actor instance to the data store.
		/// </summary>
		public void Save()
		{
			if (this.World != null)
			{
				this.World.SaveActor(this);
			}
		}
		#endregion

		#region GetValue
		/// <summary>
		/// Gets the value of the specified property from the Properties collection.
		/// </summary>
		/// <typeparam name="T">The System.Type of the value of the property.</typeparam>
		/// <param name="propertyName">The name of the property value to retrieve.</param>
		/// <returns>The value of the named property.</returns>
		public T GetValue<T>(string propertyName)
		{
			return this.Properties.GetValue<T>(propertyName);
		}
		#endregion

		#region SetBody
		/// <summary>
		/// Sets the value of the Body score for the current Actor.
		/// </summary>
		/// <param name="value">The value of the new body score.</param>
		public void SetBody(int value)
		{
			this.SetBody(value, this.BodyMax);
		}

		/// <summary>
		/// Sets the value and maximum value of the Body score for the current Actor.
		/// </summary>
		/// <param name="value">The value of the new body score.</param>
		/// <param name="max">The maximum value of the new body score.</param>
		public void SetBody(int value, int max)
		{
			if (max != this.BodyMax)
			{
				this.BodyMax = max;
			}
			if (value > max) value = max;
			this.Body = value;
		}
		#endregion

		#region RaiseLoadComplete
		/// <summary>
		/// Causes the Actor to raise the load complete event.
		/// </summary>
		public void RaiseLoadComplete()
		{
			this.OnLoadComplete();
		}
		#endregion

		#region RDL Methods
		public RdlObject[] ToRdl()
		{
			List<RdlObject> list = new List<RdlObject>();
			list.Add(this.GetObjectTag());
			list.AddRange(this.GetPropertyTags());
			return list.ToArray();
		}

		public RdlObject[] ToSimpleRdl()
		{
			List<RdlObject> list = new List<RdlObject>();
			list.Add(this.GetObjectTag());
			this.AddSimpleRdlProperties(list);
			return list.ToArray();
		}

		protected virtual void AddSimpleRdlProperties(List<RdlObject> list)
		{
			list.AddRange(this.GetRdlProperties(
				Actor.ObjectTypeProperty,
				Actor.BodyProperty,
				Actor.BodyMaxProperty,
				Actor.IsDeadProperty,
				Actor.HasProperNameProperty,
				Actor.FlagsProperty));
			// Send down the alias.
			list.Add(new RdlProperty(this.ID, "Alias", this.Alias));
		}

		protected virtual RdlActor GetObjectTag()
		{
			return this.GetObjectTag<RdlActor>();
		}

		protected T GetObjectTag<T>() where T : RdlActor
		{
			int ownerId = 0;
			if (this.Owner != null)
			{
				ownerId = this.Owner.ID;
			}
			T obj = Activator.CreateInstance<T>();
			obj.ID = this.ID;
			obj.OwnerID = ownerId;
			obj.Name = this.Name;
			obj.Description = this.Description;
			return obj;
		}

		protected virtual RdlProperty[] GetPropertyTags()
		{
			List<RdlProperty> list = new List<RdlProperty>();
			foreach (var item in this.Properties.Where(p => !this.RdlIgnoreProperties.Contains(p.Key)))
			{
				list.Add(new RdlProperty(this.ID, item.Value.Name, item.Value.Value));
			}
			// Send down the alias.
			list.Add(new RdlProperty(this.ID, "Alias", this.Alias));
			// Send down the runtime type of the object.
			list.Add(new RdlProperty(this.ID, "RuntimeType", this.GetType().FullName));
			return list.ToArray();
		}

		/// <summary>
		/// Gets an array of RdlProperty tags for the specified property names.
		/// </summary>
		/// <param name="propertyNames">The array of property names to retrieve RdlProperty instances for.</param>
		/// <returns>An array of RdlProperty tags for the specified property names.</returns>
		public RdlProperty[] GetRdlProperties(params string[] propertyNames)
		{
			List<RdlProperty> list = new List<RdlProperty>();

			if (propertyNames != null && propertyNames.Length > 0)
			{
				var props = (from p in this.Properties where propertyNames.Contains(p.Key) select p.Value).ToList();
				foreach (var prop in props)
				{
					list.Add(new RdlProperty(this.ID, prop.Name, prop.Value));
				}
			}

			return list.ToArray();
		}
		#endregion

		#region ToString
		public override string ToString()
		{
			return this.Alias();
		}
		#endregion

		#region GetOffensiveSkill, GetDefensiveSkill
		/// <summary>
		/// Gets the offensive skill for the current Actor.
		/// </summary>
		/// <returns>The offensive skill for the current Actor.</returns>
		public virtual string GetOffensiveSkill()
		{
			return String.Empty;
		}

		/// <summary>
		/// Gets the defensive skill for the current Actor.
		/// </summary>
		/// <returns>The defensive skill for the current Actor.</returns>
		public virtual string GetDefensiveSkill()
		{
			return String.Empty;
		}
		#endregion

		#region GetAllChildren
		/// <summary>
		/// Gets all children and grand children of the current Actor.
		/// </summary>
		/// <returns>A list of all child and grand child actor instances owned by the current Actor.</returns>
		public List<IActor> GetAllChildren()
		{
			List<IActor> list = new List<IActor>();
			list.AddRange(this.Children);
			foreach (var actor in this.Children)
			{
				list.AddRange(actor.Children);
			}
			return list;
		}
		#endregion

		#region GetActiveQuests
		/// <summary>
		/// Gets an enuerable list of IQuest instances where the quest is active, meanining it is not complete and/or 
		/// starts or ends with the current actor.
		/// </summary>
		/// <returns>A list of active quests.</returns>
		public virtual IEnumerable<IQuest> GetActiveQuests()
		{
			return this.GetAllChildren().Where(q => q is IQuest 
				&& ((q as IQuest).StartsWith(this) || (q as IQuest).EndsWith(this))).Select(q => q as IQuest);
		}
		#endregion

		#region IsOwner
		/// <summary>
		/// Gets a value indicating whether or not the specified Actor is the owner of the current Actor. Shortcut to checking 
		/// for null on the Owner property.
		/// </summary>
		/// <param name="owner">The Actor to check against the current owner for a match.</param>
		/// <returns>True if the specified Actor instance is the owner of the current Actor; otherwise false.</returns>
		public bool IsOwner(Actor owner)
		{
			int id = this.Properties.GetValue<int>(OwnerIDProperty);
			return owner.ID == id;
		}
		#endregion

		#region ICloneable Members

		/// <summary>
		/// Clones the current Actor instance.
		/// </summary>
		/// <returns>A new instance of the current Actor.</returns>
		public object Clone()
		{
			Actor actor = Activator.CreateInstance(this.GetType()) as Actor;
			if (actor != null)
			{
				actor.Name = this.Name;
				// Allow default class properties to be overwritten.
				actor.Properties.AddRange(this.Properties.Values.ToArray(), true);

				if (this.TemplateID == 0) actor.TemplateID = this.ID;
				else actor.TemplateID = this.TemplateID;

				actor.Owner = null;
				actor.World = this.World;
			}
			return actor;
		}

		#endregion
	}
	#endregion

	#region ActorCollection
	/// <summary>
	/// Represents a collection of Actor instances.
	/// </summary>
	public class ActorCollection : ICollection<IActor>, IEnumerable<IActor>, System.Collections.IEnumerable
	{
		/// <summary>
		/// Gets the owner of the collection.
		/// </summary>
		public IActor Owner { get; private set; }

		/// <summary>
		/// An event that is raised when a new IActor instance is added to the collection.
		/// </summary>
		public event ActorEventHandler Added = delegate { };

		public event ActorEventHandler Removed = delegate { };

		private Dictionary<string, IActor> _actors;
		private object _lock = new object();

		/// <summary>
		/// Initializes a new instance of the ActorCollection specifying the owner of the collection.
		/// </summary>
		/// <param name="owner">The owner of the collection.</param>
		public ActorCollection(IActor owner)
		{
			this.Owner = owner;
			_actors = new Dictionary<string, IActor>(StringComparer.InvariantCultureIgnoreCase);
		}

		#region ICollection<IActor> Members

		public void Add(IActor item)
		{
			if (this.Owner != null)
			{
				if (this.Owner.Owner != null)
				{
					if (this.Owner.Owner.Owner != null)
					{
						throw new InvalidOperationException("The Radiance.ActorCollection class does not allow adding children lower than 2 levels. (Parent --> Child --> GrandChild");
					}
				}
			}
			if (!(item is IPlayer))
			{
				// Only set the owner if the specified item is not a Player instance. Players are not owned by anything but the user.
				item.Owner = this.Owner;
			}
			if (this.Owner.World != null)
			{
				item.World = this.Owner.World;
			}
			lock (_lock)
			{
				if (_actors.ContainsKey(item.Alias))
				{
					_actors[item.Alias] = item;
				}
				else
				{
					_actors.Add(item.Alias, item);
				}
				this.Added(new ActorEventArgs(item));
			}
		}

		public void Clear()
		{
			lock (_lock)
			{
				_actors.Clear();
			}
		}

		public bool Contains(IActor item)
		{
			lock (_lock)
			{
				return _actors.ContainsKey(item.Alias);
			}
		}

		public void CopyTo(IActor[] array, int arrayIndex)
		{
			lock (_lock)
			{
				if (array != null && arrayIndex >= 0 && arrayIndex < array.Length)
				{
					int index = 0;
					foreach (var item in _actors.Values)
					{
						if (index >= arrayIndex)
						{
							array[index] = item;
						}
						index++;
					}
				}
			}
		}

		public int Count
		{
			get 
			{
				lock (_lock)
				{
					return _actors.Count;
				}
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(IActor item)
		{
			lock (_lock)
			{
				bool result = _actors.Remove(item.Alias);
				if (result) item.Owner = null;
				else
				{
					// Attempt to remove at a lower level.
					foreach (var actor in this)
					{
						result = actor.Children.Remove(item);
						if (result) break;
					}
				}
				if (result)
				{
					this.Removed(new ActorEventArgs(item));
				}
				return result;
			}
		}

		#endregion

		#region IEnumerable<IActor> Members

		public IEnumerator<IActor> GetEnumerator()
		{
			lock (_lock)
			{
				return _actors.Values.GetEnumerator();
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		/// <summary>
		/// Adds the specified IEnumarable list of IActor instances to the current collection.
		/// </summary>
		/// <param name="collection">The new items to add to the collection.</param>
		public void AddRange(IEnumerable<IActor> collection)
		{
			foreach (var item in collection)
			{
				this.Add(item);
			}
		}

		/// <summary>
		/// Searches the current collection, including children of the Actors in the collection, for the Actor with the specified ID value.
		/// </summary>
		/// <param name="id">The ID of the Actor to find.</param>
		/// <returns>An instance of the Actor matching the specified ID value or null if the Actor was not found.</returns>
		public T Find<T>(int id) where T : Actor
		{
			// Try the immediate collection first.
			var actor = this.Where(a => a.ID == id).FirstOrDefault();
			if (actor != null)
				return actor as T;

			// If not found then search the children of collection items.
			foreach (var item in this)
			{
				var child = item.Children.Where(i => i.ID == id).FirstOrDefault();
				if (child != null)
					return child as T;
			}

			return null;
		}

		/// <summary>
		/// Searches the current collection, including children of the Actors in the collection, for the Actor with the specified ID value.
		/// </summary>
		/// <param name="propertyValue">The property of the Actor to find.</param>
		/// <returns>An instance of the Actor matching the specified ID value or null if the Actor was not found.</returns>
		public IEnumerable<T> Find<T>(Property propertyValue) where T : Actor
		{
			// Try the immediate collection first.
			var actor = this.Where(a => a.Properties[propertyValue.Name].Value == propertyValue.Value);
			if (actor != null)
				return actor.Cast<T>();

			// If not found then search the children of collection items.
			foreach (var item in this)
			{
				var child = item.Children.Where(i => i.Properties[propertyValue.Name].Value == propertyValue.Value);
				if (child != null)
					return child.Cast<T>();
			}

			return null;
		}

		/// <summary>
		/// Finds the item that matches the specified Predicate.
		/// </summary>
		/// <param name="match">The predicate used to match items.</param>
		/// <returns>An IActor instance if found; otherwise null.</returns>
		public IActor Find(Predicate<IActor> match)
		{
			foreach (var item in this)
			{
				if (match(item))
				{
					return item;
				}
			}
			return null;
		}
	}
	#endregion

	#region ActorOwnedDictionaryBase
	/// <summary>
	/// Provides an abstract base class for a dictionary where the values are stored in the properties of Owner of the dictionary.
	/// </summary>
	/// <typeparam name="TValue">The value type to store.</typeparam>
	public abstract class ActorOwnedDictionaryBase<TValue> : IDictionary<string, TValue>
	{
		/// <summary>
		/// Gets the Actor that owns the current dictionary.
		/// </summary>
		public IActor Owner { get; private set; }

		/// <summary>
		/// Gets the prefix that is pre-pended to the property names when stored on the Owner instance.
		/// </summary>
		public string Prefix { get; private set; }

		/// <summary>
		/// Initializes a new instance of the ActorOwnedDictionaryBase class.
		/// </summary>
		/// <param name="owner">The Actor instance where the values of the dictionary are stored.</param>
		/// <param name="propertyPrefix">The prefix pre-pended to the properties of the dictionary before adding them.</param>
		protected ActorOwnedDictionaryBase(IActor owner, string propertyPrefix)
		{
			this.Owner = owner;
			this.Prefix = propertyPrefix;
		}

		/// <summary>
		/// Affixes the current Prefix to the specified name, if not already added.
		/// </summary>
		/// <param name="name">The name of the property to affix the prefix.</param>
		/// <returns>The prefixed name.</returns>
		protected virtual string GetPrefixedName(string name)
		{
			if (!String.IsNullOrEmpty(name))
			{
				string trimmedName = name.Trim();
				if (!trimmedName.StartsWith(this.Prefix))
				{
					return String.Concat(this.Prefix, trimmedName);
				}
				return trimmedName;
			}
			return String.Empty;
		}

		/// <summary>
		/// Gets the item key named, replacing the container prefix.
		/// </summary>
		/// <param name="name">THe prefixed key name.</param>
		/// <returns>The key name without the class Prefix.</returns>
		protected virtual string GetCleanName(string name)
		{
			if (name.StartsWith(this.Prefix))
			{
				return name.Substring(this.Prefix.Length);
			}
			return name;
		}

		#region IDictionary<string,TValue> Members

		public virtual void Add(string key, TValue value)
		{
			this.Owner.Properties.SetValue(this.GetPrefixedName(key), value);
		}

		public bool ContainsKey(string key)
		{
			return this.Owner.Properties.ContainsKey(this.GetPrefixedName(key));
		}

		public ICollection<string> Keys
		{
			get { return (from s in this select s.Key).ToList(); }
		}

		public virtual bool Remove(string key)
		{
			this.Owner.Properties.SetValue(this.GetPrefixedName(key), false);
			return true;
		}

		public abstract bool TryGetValue(string key, out TValue value);

		public ICollection<TValue> Values
		{
			get { return (from s in this select s.Value).ToList(); }
		}

		public TValue this[string key]
		{
			get { return this.Owner.Properties.GetValue<TValue>(this.GetPrefixedName(key)); }
			set { this.Owner.Properties.SetValue(this.GetPrefixedName(key), value); }
		}

		#endregion

		#region ICollection<KeyValuePair<string,TValue>> Members

		public void Add(KeyValuePair<string, TValue> item)
		{
			this.Add(item.Key, item.Value);
		}

		public void Clear()
		{
		}

		public bool Contains(KeyValuePair<string, TValue> item)
		{
			return this.ContainsKey(item.Key);
		}

		public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
		{
			if (array != null && arrayIndex >= 0 && arrayIndex < array.Length)
			{
				int index = 0;
				foreach (var item in this)
				{
					if (index >= arrayIndex)
					{
						array[index] = item;
					}
					index++;
				}
			}
		}

		public int Count
		{
			get { return (from p in this.Owner.Properties where p.Key.StartsWith(this.Prefix) select p).Count(); }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<string, TValue> item)
		{
			return this.Remove(item.Key);
		}

		#endregion

		#region IEnumerable<KeyValuePair<string,TValue>> Members

		public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
		{
			return (from p in this.Owner.Properties where p.Key.StartsWith(this.Prefix) select new KeyValuePair<string, TValue>(p.Key, p.Value.GetValue<TValue>())).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
	#endregion

	#region ActorOwnedCollectionBase
	/// <summary>
	/// Represents a collection of items stored in the Properties collection of the Owner where the 
	/// key is the prefix and index of the item.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ActorOwnedItemCollection<T> : ICollection<T>, IEnumerable<T> where T : IItem
	{
		/// <summary>
		/// Gets the Actor that owns the current collection.
		/// </summary>
		public IActor Owner { get; private set; }

		/// <summary>
		/// Gets the prefix that is pre-pended to the property names when stored on the Owner instance.
		/// </summary>
		public string Prefix { get; private set; }

		private int _index = 0;

		/// <summary>
		/// Initializes a new instance of the ActorOwnedCollectionBase class.
		/// </summary>
		/// <param name="owner">The Actor instance where the values of the collection are stored.</param>
		/// <param name="propertyPrefix">The prefix pre-pended to the properties of the collection before adding them.</param>
		protected ActorOwnedItemCollection(IActor owner, string propertyPrefix)
		{
			this.Owner = owner;
			this.Prefix = propertyPrefix;
			this.Resize();
		}

		protected void Resize()
		{
			_index = this.Owner.Properties.Values.Where(p => p.Name.StartsWith(this.Prefix)).Count();
		}

		protected string GetPropertyKey(int index)
		{
			return string.Concat(this.Prefix, index);
		}

		protected IEnumerable<Property> GetProperties()
		{
			return this.Owner.Properties.Values.Where(p => p.Name.StartsWith(this.Prefix));
		}

		public T Get(int index)
		{
			int id = this.Owner.Properties.GetValue<int>(this.GetPropertyKey(index));
			if (id > 0)
			{
				return this.Owner.GetAllChildren().Where(c => c.ID == id && c is T).Select(c => (T)c).FirstOrDefault();
			}
			return default(T);
		}

		public void Set(int index, T value)
		{
			this.Owner.Properties.SetValue(this.GetPropertyKey(index), value.ID);
			this.Owner.Children.Add(value);
		}

        public int IndexOf(T item)
        {
            foreach (var prop in this.GetProperties())
            {
                if (prop.GetValue<int>().Equals(item.ID))
                {
                    int index;
                    if (Int32.TryParse(prop.Name.Replace(this.Prefix, String.Empty), out index))
                    {
                        return index;
                    }
                }
            }
            return -1;
        }

		#region ICollection<T> Members

		public virtual void Add(T item)
		{
			this.Set(_index, item);
			_index++;
		}

		public void Clear()
		{
			if (this.Owner.World != null)
			{
				int index = 0;
				foreach (var prop in this.GetProperties())
				{
					this.Owner.World.Provider.RemoveProperty<IActor>(this.Owner, this.GetPropertyKey(index));
					index++;
				}
				this.Resize();
			}
		}

		public bool Contains(T item)
		{
			foreach (var prop in this.GetProperties())
			{
				if (prop.GetValue<int>().Equals(item.ID))
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			if (array != null && arrayIndex >= 0 && arrayIndex < array.Length)
			{
				int index = 0;
				foreach (var item in this)
				{
					if (index >= arrayIndex)
					{
						array[index] = item;
					}
					index++;
				}
			}
		}

		public int Count
		{
			get { return _index; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public virtual bool Remove(T item)
		{
			string key = String.Empty;
			foreach (var prop in this.GetProperties())
			{
				if (prop.GetValue<int>().Equals(item))
				{
					key = prop.Name;
					break;
				}
			}
			if (!String.IsNullOrEmpty(key) && this.Owner.World != null)
			{
				this.Owner.World.Provider.RemoveProperty<IActor>(this.Owner, key);
			}
			return false;
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			List<T> list = new List<T>();
			for (int i = 0; i < _index; i++)
			{
				list.Add(this.Get(i));
			}
			return list.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
	#endregion

	#region ActorComparer
	public class ActorComparer<T> : IEqualityComparer<T> where T : IActor
	{
		#region IEqualityComparer<T> Members

		public bool Equals(T x, T y)
		{
			return x.Name.Equals(y.Name) && x.GetType() == y.GetType();
		}

		public int GetHashCode(T obj)
		{
			return obj.Name.GetHashCode() + obj.GetType().GetHashCode();
		}

		#endregion
	}
	#endregion
}
