using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance.Markup
{
	#region RdlObject
	/// <summary>
	/// Represents an OBJ tag in the Radiance Definition Language (RDL).
	/// </summary>
	public abstract class RdlObject : RdlTag
	{
		private int _IDIndex = 0;

		/// <summary>
		/// Gets or sets the ID of the object represented by this tag.
		/// </summary>
		public int ID
		{
			get { return this.GetArg<int>(_IDIndex); }
			set { this.Args[_IDIndex] = value; }
		}

		/// <summary>
		/// Gets a list of properties associated with the current object tag.
		/// </summary>
		public RdlPropertyCollection Properties { get; private set; }	

		/// <summary>
		/// Initializes a new instance of the OBJ tag.
		/// </summary>
		protected RdlObject()
			: this(String.Empty, 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ tag.
		/// </summary>
		/// <param name="typeName">The object tag type.</param>
		protected RdlObject(RdlObjectTypeName typeName)
			: this(typeName, 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ tag.
		/// </summary>
		/// <param name="id">The ID of the object represented by this tag.</param>
		protected RdlObject(int id)
			: this(String.Empty, id)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ tag.
		/// </summary>
		/// <param name="typeName">The object tag type.</param>
		/// <param name="id">The ID of the object represented by this tag.</param>
		protected RdlObject(RdlObjectTypeName typeName, int id)
			: this(typeName.ToString(), id)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ tag.
		/// </summary>
		/// <param name="typeName">The object tag type.</param>
		/// <param name="id">The ID of the object represented by this tag.</param>
		protected RdlObject(string typeName, int id)
			: base(RdlTagName.OBJ, typeName)
		{
			_IDIndex = this.GetNextIndex();
			this.Args.Insert(_IDIndex, id);
			this.Properties = new RdlPropertyCollection();
		}

		/// <summary>
		/// Gets the string representation of the current OBJ and all its PROP tags.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(base.ToString());
			foreach (var item in this.Properties)
			{
				sb.Append(item.ToString());
			}
			return sb.ToString();
		}
	}
	#endregion

	#region RdlProperty
	/// <summary>
	/// Represents an OBJ|PROP tag in the Radiance Definition Language (RDL).
	/// </summary>
	public class RdlProperty : RdlObject
	{
		private int _nameIndex;
		private int _valueIndex;

		/// <summary>
		/// Gets or sets the name of the property this tag represents.
		/// </summary>
		public string Name
		{
			get { return this.GetArg<string>(_nameIndex); }
			set { this.Args[_nameIndex] = value; }
		}

		/// <summary>
		/// Gets or sets the value of the property this tag represents.
		/// </summary>
		public object Value
		{
			get { return this.Args[_valueIndex]; }
			set { this.Args[_valueIndex] = value; }
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|PROP tag.
		/// </summary>
		public RdlProperty()
			: this(0, String.Empty, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|PROP tag.
		/// </summary>
		/// <param name="id">The ID of the object that contains the property this tag represents.</param>
		/// <param name="name">The name of the property this tag represents.</param>
		/// <param name="value">The value of the property this tag represents.</param>
		public RdlProperty(int id, string name, object value)
			: base(RdlObjectTypeName.PROP, id)
		{
			_nameIndex = this.GetNextIndex();
			_valueIndex = this.GetNextIndex();
			this.Args.Insert(_nameIndex, name);
			this.Args.Insert(_valueIndex, value);
		}

		/// <summary>
		/// Gets the value of the property tag.
		/// </summary>
		/// <typeparam name="T">The System.Type of the value to retrieve.</typeparam>
		/// <returns>The value of the current property or the default of T.</returns>
		public T GetValue<T>()
		{
			object val = this.Value;
			if (val != null)
			{
				if (typeof(T) == typeof(bool))
				{
					val = Boolean.Parse(val.ToString());
				}
				else if (typeof(T).IsEnum)
				{
					return (T)Enum.Parse(typeof(T), val.ToString(), true);
				}
				return (T)Convert.ChangeType(val, typeof(T), null);
			}
			return default(T);
		}
	}
	#endregion

	#region RdlPropertyCollection
	/// <summary>
	/// Represents a collection of RdlProperty tags.
	/// </summary>
	public class RdlPropertyCollection : List<RdlProperty>
	{
		public object GetValue(string name)
		{
			var prop = this.Where(p => p.Name == name).FirstOrDefault();
			if (prop != null)
			{
				return prop.Value;
			}
			return null;
		}

		/// <summary>
		/// Gets the value of the specified property.
		/// </summary>
		/// <typeparam name="T">The System.Type of the value to return.</typeparam>
		/// <param name="name">The name of the property to find.</param>
		/// <returns>The value of the property or th default value of T if the property does not exist.</returns>
		public T GetValue<T>(string name)
		{
			var prop = this.Where(p => p.Name == name).FirstOrDefault();
			if (prop != null)
			{
				return prop.GetValue<T>();
			}
			return default(T);
		}

		/// <summary>
		/// Sets the value of the specified property.
		/// </summary>
		/// <param name="name">The name of the property to find.</param>
		/// <param name="value">The new value of the property.</param>
		public void SetValue(string name, object value)
		{
			var prop = this.Where(p => p.Name == name).FirstOrDefault();
			if (prop != null)
			{
				prop.Value = value;
			}
			else
			{
				int ownerId = this.Select(p => p.ID).FirstOrDefault();
				this.Add(new RdlProperty(ownerId, name, value));
			}
		}
	}
	#endregion

	#region RdlActor
	/// <summary>
	/// Represents an OBJ|ACTOR tag in the Radiance Definition Language (RDL).
	/// </summary>
	public class RdlActor : RdlObject
	{
		private int _ownerIdIndex;
		private int _nameIndex;
		private int _descIndex;

		/// <summary>
		/// Gets or sets the unique identifier of the owner of the actor this tag represents.
		/// </summary>
		public int OwnerID
		{
			get { return this.GetArg<int>(_ownerIdIndex); }
			set { this.Args[_ownerIdIndex] = value; }
		}

		/// <summary>
		/// Gets or sets the name of the actor this tag represents.
		/// </summary>
		public string Name
		{
			get { return this.GetArg<string>(_nameIndex); }
			set { this.Args[_nameIndex] = value; }
		}

		/// <summary>
		/// Gets or sets the description of the actor this tag represents.
		/// </summary>
		public string Description
		{
			get { return this.GetArg<string>(_descIndex); }
			set { this.Args[_descIndex] = value; }
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|ACTOR tag.
		/// </summary>
		public RdlActor()
			: this(0, 0, String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|ACTOR tag.
		/// </summary>
		/// <param name="id">The ID of the object that this tag represents.</param>
		/// <param name="name">The name of the actor.</param>
		public RdlActor(int id, string name)
			: this(id, 0, name)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|ACTOR tag.
		/// </summary>
		/// <param name="id">The ID of the object that this tag represents.</param>
		/// <param name="name">The name of the actor.</param>
		/// <param name="description">The description of the actor.</param>
		public RdlActor(int id, string name, string description)
			: this(id, 0, name, description)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|ACTOR tag.
		/// </summary>
		/// <param name="id">The ID of the object that this tag represents.</param>
		/// <param name="ownerId">The ID of the owner of the current actor.</param>
		/// <param name="name">The name of the actor.</param>
		public RdlActor(int id, int ownerId, string name)
			: this(RdlObjectTypeName.ACTOR, id, ownerId, name, String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|ACTOR tag.
		/// </summary>
		/// <param name="id">The ID of the object that this tag represents.</param>
		/// <param name="ownerId">The ID of the owner of the current actor.</param>
		/// <param name="name">The name of the actor.</param>
		/// <param name="description">The description of the actor.</param>
		public RdlActor(int id, int ownerId, string name, string description)
			: this(RdlObjectTypeName.ACTOR, id, ownerId, name, description)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|ACTOR tag.
		/// </summary>
		/// <param name="typeName">The type of the actor tag.</param>
		/// <param name="id">The ID of the object that this tag represents.</param>
		/// <param name="ownerId">The ID of the owner of the current actor.</param>
		/// <param name="name">The name of the actor.</param>
		protected RdlActor(RdlObjectTypeName typeName, int id, int ownerId, string name)
			: this(typeName, id, ownerId, name, String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|ACTOR tag.
		/// </summary>
		/// <param name="typeName">The type of the actor tag.</param>
		/// <param name="id">The ID of the object that this tag represents.</param>
		/// <param name="ownerId">The ID of the owner of the current actor.</param>
		/// <param name="name">The name of the actor.</param>
		/// <param name="description">The description of the actor.</param>
		protected RdlActor(RdlObjectTypeName typeName, int id, int ownerId, string name, string description)
			: base(typeName, id)
		{
			_ownerIdIndex = this.GetNextIndex();
			_nameIndex = this.GetNextIndex();
			_descIndex = this.GetNextIndex();
			this.Args.Insert(_ownerIdIndex, ownerId);
			this.Args.Insert(_nameIndex, name);
			this.Args.Insert(_descIndex, description);
		}
	}
	#endregion

	#region RdlActorDictionary
	public class RdlActorDictionary : IDictionary<int, RdlActor>
	{
		private Dictionary<int, RdlActor> _items = new Dictionary<int, RdlActor>();
		private object _lock = new object();

		public RdlActorDictionary()
		{
		}

		#region IDictionary<int,RdlActor> Members

		public void Add(int key, RdlActor value)
		{
			lock (_lock)
			{
				_items.Add(key, value);	
			}
		}

		public bool ContainsKey(int key)
		{
			lock (_lock)
			{
				return _items.ContainsKey(key);	
			}
		}

		public ICollection<int> Keys
		{
			get
			{
				lock (_lock)
				{
					return _items.Keys;
				}
			}
		}

		public bool Remove(int key)
		{
			lock (_lock)
			{
				return _items.Remove(key);
			}
		}

		public bool TryGetValue(int key, out RdlActor value)
		{
			lock (_lock)
			{
				return _items.TryGetValue(key, out value);
			}
		}

		public ICollection<RdlActor> Values
		{
			get
			{
				lock (_lock)
				{
					return _items.Values;
				}
			}
		}

		public RdlActor this[int key]
		{
			get
			{
				lock (_lock)
				{
					return _items[key];
				}
			}
			set
			{
				lock (_lock)
				{
					_items[key] = value;
				}
			}
		}

		#endregion

		#region ICollection<KeyValuePair<int,RdlActor>> Members

		public void Add(KeyValuePair<int, RdlActor> item)
		{
			lock (_lock)
			{
				_items.Add(item.Key, item.Value);
			}
		}

		public void Clear()
		{
			lock (_lock)
			{
				_items.Clear();
			}
		}

		public bool Contains(KeyValuePair<int, RdlActor> item)
		{
			lock (_lock)
			{
				return _items.ContainsKey(item.Key) && _items.ContainsValue(item.Value);
			}
		}

		public void CopyTo(KeyValuePair<int, RdlActor>[] array, int arrayIndex)
		{
			lock (_lock)
			{
				if (array != null && arrayIndex >= 0 && arrayIndex < array.Length)
				{
					int index = 0;
					foreach (var item in _items)
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
					return _items.Count;
				}
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<int, RdlActor> item)
		{
			lock (_lock)
			{
				return _items.Remove(item.Key);
			}
		}

		#endregion

		#region IEnumerable<KeyValuePair<int,RdlActor>> Members

		public IEnumerator<KeyValuePair<int, RdlActor>> GetEnumerator()
		{
			lock (_lock)
			{
				return _items.GetEnumerator();
			}
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

	#region RdlPlayer
	/// <summary>
	/// Represents an OBJ|PLAYER tag in the Radiance Definition Language (RDL).
	/// </summary>
	public class RdlPlayer : RdlActor
	{
		private int _userNameIndex;

		/// <summary>
		/// Gets or sets the usernname of the player this tag represents.
		/// </summary>
		public string UserName
		{
			get { return this.GetArg<string>(_userNameIndex); }
			set { this.Args[_userNameIndex] = value; }
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|PLAYER tag.
		/// </summary>
		public RdlPlayer()
			: this(0, 0, String.Empty, String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|PLAYER tag.
		/// </summary>
		/// <param name="id">The ID of the object that this tag represents.</param>
		/// <param name="name">The name of the player.</param>
		/// <param name="username">The username of the player.</param>
		public RdlPlayer(int id, string name, string username)
			: this(id, 0, name, username)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|PLAYER tag.
		/// </summary>
		/// <param name="id">The ID of the object that this tag represents.</param>
		/// <param name="ownerId">The ID of the owner of the current actor.</param>
		/// <param name="name">The name of the player.</param>
		/// <param name="username">The username of the player.</param>
		public RdlPlayer(int id, int ownerId, string name, string username)
			: base(RdlObjectTypeName.PLAYER, id, ownerId, name)
		{
			_userNameIndex = this.GetNextIndex();
			this.Args.Insert(_userNameIndex, username);
		}
	}
	#endregion

	#region RdlPlace
	/// <summary>
	/// Represents an OBJ|PLACE tag in the Radiance Definition Language (RDL).
	/// </summary>
	public class RdlPlace : RdlActor
	{
		private int _xIndex;
		private int _yIndex;
		private int _zIndex;

		/// <summary>
		/// Gets or sets the x world coordinate of the place this tag represents.
		/// </summary>
		public int X
		{
			get { return this.GetArg<int>(_xIndex); }
			set { this.Args[_xIndex] = value; }
		}

		/// <summary>
		/// Gets or sets the y world coordinate of the place this tag represents.
		/// </summary>
		public int Y
		{
			get { return this.GetArg<int>(_yIndex); }
			set { this.Args[_yIndex] = value; }
		}

		/// <summary>
		/// Gets or sets the z world coordinate of the place this tag represents.
		/// </summary>
		public int Z
		{
			get { return this.GetArg<int>(_zIndex); }
			set { this.Args[_zIndex] = value; }
		}

		public List<RdlActor> Actors { get; private set; }	

		/// <summary>
		/// Initializes a new instance of the OBJ|PLACE tag.
		/// </summary>
		public RdlPlace()
			: this(0, 0, String.Empty, 0, 0, 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|PLACE tag.
		/// </summary>
		/// <param name="id">The ID of the object that this tag represents.</param>
		/// <param name="name">The name of the place.</param>
		/// <param name="x">The x world coordinate of the place.</param>
		/// <param name="y">The y world coordinate of the place.</param>
		/// <param name="z">The z world coordinate of the place.</param>
		public RdlPlace(int id, string name, int x, int y, int z)
			: this(id, 0, name, x, y, z)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|PLACE tag.
		/// </summary>
		/// <param name="id">The ID of the object that this tag represents.</param>
		/// <param name="ownerId">The ID of the owner of the current actor.</param>
		/// <param name="name">The name of the place.</param>
		/// <param name="x">The x world coordinate of the place.</param>
		/// <param name="y">The y world coordinate of the place.</param>
		/// <param name="z">The z world coordinate of the place.</param>
		public RdlPlace(int id, int ownerId, string name, int x, int y, int z)
			: base(RdlObjectTypeName.PLACE, id, ownerId, name)
		{
			_xIndex = this.GetNextIndex();
			_yIndex = this.GetNextIndex();
			_zIndex = this.GetNextIndex();
			this.Args.Insert(_xIndex, x);
			this.Args.Insert(_yIndex, y);
			this.Args.Insert(_zIndex, z);
			this.Actors = new List<RdlActor>();
		}
	}
	#endregion

	#region RdlRace
	/// <summary>
	/// Represents an OBJ|RACE tag in the Radiance Definition Language (RDL).
	/// </summary>
	public class RdlRace : RdlActor
	{
		/// <summary>
		/// Initializes a new instance of the OBJ|RACE tag.
		/// </summary>
		public RdlRace()
			: this(0, String.Empty, String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|RACE tag.
		/// </summary>
		/// <param name="id">The ID of the object that this tag represents.</param>
		/// <param name="name">The name of the item.</param>
		/// <param name="description">The description of the race.</param>
		public RdlRace(int id, string name, string description)
			: base(RdlObjectTypeName.RACE, id, 0, name, description)
		{
		}
	}
	#endregion

	#region RdlTerrain
	/// <summary>
	/// Represents an OBJ|TERRAIN tag in the Radiance Definition Language (RDL).
	/// </summary>
	public class RdlTerrain : RdlObject
	{
		private int _nameIndex;
		private int _colorIndex;
		private int _walTypeIndex;
		private int _imageUrlIndex;

		/// <summary>
		/// Gets or sets the name of the terrain this tag represents.
		/// </summary>
		public string Name
		{
			get { return this.GetArg<string>(_nameIndex); }
			set { this.Args[_nameIndex] = value; }
		}

		/// <summary>
		/// Gets or sets the color of the terrain this tag represents.
		/// </summary>
		public int Color
		{
			get { return this.GetArg<int>(_colorIndex); }
			set { this.Args[_colorIndex] = value; }
		}

		/// <summary>
		/// Gets or sets the walk type of the terrain this tag represents.
		/// </summary>
		public int WalkType
		{
			get { return this.GetArg<int>(_walTypeIndex); }
			set { this.Args[_walTypeIndex] = value; }
		}

		/// <summary>
		/// Gets or sets the image url of the terrain this tag represents.
		/// </summary>
		public string ImageUrl
		{
			get { return this.GetArg<string>(_imageUrlIndex); }
			set { this.Args[_imageUrlIndex] = value; }
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|TERRAIN tag.
		/// </summary>
		public RdlTerrain()
			: this(0, String.Empty, 0, 0, String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|TERRAIN tag.
		/// </summary>
		/// <param name="id">The ID of the terrain that this tag represents.</param>
		/// <param name="name">The name of the terrain.</param>
		/// <param name="color">The color of the terrain.</param>
		/// <param name="walkType">The walk type of the terrain.</param>
		/// <param name="imageUrl">The image url of the terrain.</param>
		public RdlTerrain(int id, string name, int color, int walkType, string imageUrl)
			: base(RdlObjectTypeName.TERRAIN, id)
		{
			_nameIndex = this.GetNextIndex();
			_colorIndex = this.GetNextIndex();
			_walTypeIndex = this.GetNextIndex();
			_imageUrlIndex = this.GetNextIndex();
			this.Args.Insert(_nameIndex, name);
			this.Args.Insert(_colorIndex, color);
			this.Args.Insert(_walTypeIndex, walkType);
			this.Args.Insert(_imageUrlIndex, imageUrl);
		}
	}
	#endregion

	#region RdlSkill
	/// <summary>
	/// Represents an OBJ|SKILL tag in the Radiance Definition Language (RDL).
	/// </summary>
	public class RdlSkill : RdlObject
	{
		private int _nameIndex;
		private int _descIndex;
		private int _valueIndex;
		private int _groupIndex;

		/// <summary>
		/// Gets or sets the name of the skill this tag represents.
		/// </summary>
		public string Name
		{
			get { return this.GetArg<string>(_nameIndex); }
			set { this.Args[_nameIndex] = value; }
		}

		/// <summary>
		/// Gets or sets the description of the skill this tag represents.
		/// </summary>
		public string Description
		{
			get { return this.GetArg<string>(_descIndex); }
			set { this.Args[_descIndex] = value; }
		}

		/// <summary>
		/// Gets or sets the value of the skill this tag represents.
		/// </summary>
		public int Value
		{
			get { return this.GetArg<int>(_valueIndex); }
			set { this.Args[_valueIndex] = value; }
		}

		/// <summary>
		/// Gets or sets the group name of the current skill.
		/// </summary>
		public string GroupName
		{
			get { return this.GetArg<string>(_groupIndex); }
			set { this.Args[_groupIndex] = value; }
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|SKILL tag.
		/// </summary>
		public RdlSkill()
			: this(0, String.Empty, String.Empty, String.Empty, 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OBJ|SKILL tag and presets the tag values.
		/// </summary>
		/// <param name="id">The id of the skill.</param>
		/// <param name="name">The name of the skill.</param>
		/// <param name="description">The description of the skill.</param>
		/// <param name="group">The group this skill belongs in.</param>
		/// <param name="value">The value of the skill.</param>
		public RdlSkill(int id, string name, string description, string group, int value)
			: base(RdlObjectTypeName.SKILL, id)
		{
			_nameIndex = this.GetNextIndex();
			_descIndex = this.GetNextIndex();
			_groupIndex = this.GetNextIndex();
			_valueIndex = this.GetNextIndex();
			this.Args.Insert(_nameIndex, name);
			this.Args.Insert(_descIndex, description);
			this.Args.Insert(_groupIndex, group);
			this.Args.Insert(_valueIndex, value);
		}
	}
	#endregion
}
