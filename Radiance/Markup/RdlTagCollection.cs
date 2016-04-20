using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Radiance.Markup
{
	/// <summary>
	/// Provides a collection of RdlTags that define a message body.
	/// </summary>
	public class RdlTagCollection : ICollection<RdlTag>, IEnumerable<RdlTag>
	{
		private Dictionary<TagKey, RdlTag> _tags = new Dictionary<TagKey, RdlTag>(new TagKeyComparer());

		/// <summary>
		/// Provides a locking object used to syncronize the collection.
		/// </summary>
		public object SyncLock = new object();

		public RdlTagCollection() { }

		public RdlTagCollection(IEnumerable<RdlTag> tags)
		{
			this.AddRange(tags);
		}

		#region ICollection<RdlTag> Members

		/// <summary>
		/// Adds a new RdlTag instance to the collection.
		/// </summary>
		/// <param name="item">The RdlTag instance to add to the collection.</param>
		public void Add(RdlTag item)
		{
			TagKey key = new TagKey(item);
			lock (this.SyncLock)
			{
				if (_tags.ContainsKey(key))
				{
					_tags[key] = item;
				}
				else
				{
					_tags.Add(key, item);
				}
			}
		}

		/// <summary>
		/// Clears all of the items from the collection.
		/// </summary>
		public void Clear()
		{
			lock (this.SyncLock)
			{
				_tags.Clear();
			}
		}

		/// <summary>
		/// Gets a value indicating whether or not the specified tag exists within the collection.
		/// </summary>
		/// <param name="item">The RdlTag to check within the collection.</param>
		/// <returns>True if the item exists within the collection; otherwise false.</returns>
		public bool Contains(RdlTag item)
		{
			return _tags.ContainsKey(new TagKey(item));
		}

		/// <summary>
		/// Copies the elements from the current collection to the specified array starting a the specified index.
		/// </summary>
		/// <param name="array">The initialized array in which to copy the items.</param>
		/// <param name="arrayIndex">The index at which to start copying.</param>
		public void CopyTo(RdlTag[] array, int arrayIndex)
		{
			lock (this.SyncLock)
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
		}

		/// <summary>
		/// Gets the number of items in the collection.
		/// </summary>
		public int Count
		{
			get { return _tags.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether or not this collection is readonly.
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// Removes the specified RdlTag from the collection.
		/// </summary>
		/// <param name="item">The RdlTag to remove from the collection.</param>
		/// <returns>True if the tag was removed; otherwise false.</returns>
		public bool Remove(RdlTag item)
		{
			return _tags.Remove(new TagKey(item));
		}

		#endregion

		#region IEnumerable<RdlTag> Members

		/// <summary>
		/// Gets an enumerator of the items in the current collection.
		/// </summary>
		/// <returns>An IEnumerator of RdlTag instances.</returns>
		public IEnumerator<RdlTag> GetEnumerator()
		{
			return _tags.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _tags.Values.GetEnumerator();
		}

		#endregion

		#region AddRange
		/// <summary>
		/// Adds the specified RdlTag array to the collection.
		/// </summary>
		/// <param name="collection">The RdlTag array to add to the collection.</param>
		public void AddRange(IEnumerable<RdlTag> collection)
		{
			try
			{
				if (collection != null && collection.Count() > 0)
				{
					foreach (var item in collection)
					{
						this.Add(item);
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		#endregion

		#region Get Methods
		/// <summary>
		/// Gets a list of tags in the collection that derive from the ObjectActorTag class.
		/// </summary>
		/// <returns>A list of actor tags in the current collection.</returns>
		public List<RdlActor> GetActors()
		{
			return this.GetObjects<RdlActor>();
		}

		/// <summary>
		/// Gets a list of tags in the collection that derive from the RdlPlace class.
		/// </summary>
		/// <returns>A list of place tags in the current collection.</returns>
		public List<RdlPlace> GetPlaces()
		{
			return this.GetObjects<RdlPlace>();
		}

		/// <summary>
		/// Gets the first available RdlCommandResponse tag in the collection.
		/// </summary>
		/// <returns>The first available RdlCommandResponse if found; otherwise null.</returns>
		public RdlCommandResponse GetCommandResponse()
		{
			return (from t in this where t.TagName.Equals(RdlTagName.RESP.ToString()) select t as RdlCommandResponse).FirstOrDefault();
		}

		/// <summary>
		/// Gets a list of tags that derive from the RdlObject class, including any RdlProperty tags associated with the object.
		/// </summary>
		/// <typeparam name="T">The RdlObject type of tag to retrieve.</typeparam>
		/// <returns>A list of RdlObject tags.</returns>
		public List<T> GetObjects<T>() where T : RdlObject
		{
			List<Type> types = new List<Type>();
			types.Add(typeof(T));
			if (typeof(T) == typeof(RdlActor))
			{
				types.Add(typeof(RdlPlayer));
				types.Add(typeof(RdlRace));
				types.Add(typeof(RdlPlace));
			}

			List<T> objects = (from t in this
							   where t.TagName.Equals(RdlTagName.OBJ.ToString())
								   && types.Contains(t.GetType())
							   select t as T).ToList();
			foreach (var obj in objects)
			{
				obj.Properties.AddRange(this.GetAndRemoveProperties(obj.ID));
			}
			return objects;
		}

		/// <summary>
		/// Gets a list of MSG tags contained within the collection.
		/// </summary>
		/// <returns>A list of MSG tags contained within the collection.</returns>
		public List<RdlMessage> GetMessages()
		{
			return (from t in this where t.TagName == RdlTagName.MSG.ToString() select (RdlMessage)t).ToList();
		}

		/// <summary>
		/// Gets a list of OBJ|PROP tags contained within the collection.
		/// </summary>
		/// <returns>A list of OBJ|PROP tags contained within the collection.</returns>
		public List<RdlProperty> GetProperties()
		{
			return (from t in this where t.TypeName == RdlObjectTypeName.PROP.ToString() select t as RdlProperty).ToList();
		}

		/// <summary>
		/// Gets a list of OBJ|PROP tags contained within the collection for the specified object.
		/// </summary>
		/// <param name="objectId">The id of the object properties to return.</param>
		/// <returns>A list of OBJ|PROP tags contained within the collection.</returns>
		public List<RdlProperty> GetProperties(int objectId)
		{
			return (from t in this 
					where t.TypeName == RdlObjectTypeName.PROP.ToString() 
					&& (t as RdlObject).ID.Equals(objectId)
					select t as RdlProperty).ToList();
		}

		private List<RdlProperty> GetAndRemoveProperties(int objectId)
		{
			lock (this.SyncLock)
			{
				var items = this.GetProperties(objectId);
				foreach (var item in items)
				{
					this.Remove(item);
				}
				return items.ToList();
			}
		}

		/// <summary>
		/// Gets a specific RdlProperty instance for the specified object and with the specified name.
		/// </summary>
		/// <param name="objectId">The id of the object property to return.</param>
		/// <param name="name">The name of the property to return.</param>
		/// <returns>An RdlProperty instance for the specified object.</returns>
		public RdlProperty GetProperty(int objectId, string name)
		{
			return (from t in this
					where t.TagName == RdlTagName.OBJ.ToString()
						&& t.TypeName == RdlObjectTypeName.PROP.ToString()
						&& (t as RdlProperty).ID.Equals(objectId)
						&& (t as RdlProperty).Name.Equals(name)
					select t as RdlProperty).FirstOrDefault();
		}

		/// <summary>
		/// Gets a list of CMD tags contained within the collection.
		/// </summary>
		/// <returns>A list of CMD tags contained within the collection.</returns>
		public List<RdlCommand> GetCommands()
		{
			return (from t in this where t.TagName == RdlTagName.CMD.ToString() select t as RdlCommand).ToList();
		}

		/// <summary>
		/// Gets a list of RdlTags of the specified type for the specified TagName and TypeName values.
		/// </summary>
		/// <typeparam name="T">The RdlTag derived type to return.</typeparam>
		/// <param name="tagName">The TagName value of the tags to return.</param>
		/// <param name="typeName">The TypeName value of the tags to return.</param>
		/// <returns>A list of RdlTags of the specified type for the specified TagName and TypeName values.</returns>
		public List<T> GetTags<T>(string tagName, string typeName) where T : RdlTag
		{
			return (from t in this
					where t.TagName.Equals(tagName.ToUpper())
						&& t.TypeName.Equals(typeName.ToUpper())
					select t as T).ToList();
		}
		#endregion

		#region ToString
		/// <summary>
		/// Gets the string representation of the tags contained within the collection.
		/// </summary>
		/// <returns>The string representation of the tags contained within the collection.</returns>
		public override string ToString()
		{
			lock (this.SyncLock)
			{
				using (RdlTagWriter writer = new RdlTagWriter())
				{
					foreach (var tag in this)
					{
						tag.WriteTag(writer);
					}
					return writer.ToString();
				}
			}
		}
		#endregion

		#region ToBytes
		/// <summary>
		/// Converts the collection of RdlTag instances to a byte array.
		/// </summary>
		/// <returns>An array of bytes of the current collection.</returns>
		public byte[] ToBytes()
		{
			return Encoding.UTF8.GetBytes(this.ToString());
		}
		#endregion

		#region Static Methods
		/// <summary>
		/// Creates a new RdlTagCollection from the specified tagString.
		/// </summary>
		/// <param name="tagString">The string containing multiple tags that will become part of the new collection.</param>
		/// <returns>A new RdlTagCollection instance containing the parsed tags.</returns>
		public static RdlTagCollection FromString(string tagString)
		{
			RdlTagCollection col = new RdlTagCollection();
			using (RdlTagReader reader = new RdlTagReader(tagString))
			{
				RdlTag tag = null;
				while ((tag = reader.ReadTag()) != null)
				{
					col.Add(tag);
				}
			}
			return col;
		}

		/// <summary>
		/// Creates a new RdlTagCollection instance from the specified byte array.
		/// </summary>
		/// <param name="data">The array bytes containing the tags to parse.</param>
		/// <returns>A new RdlTagCollection from the specified byte array.</returns>
		public static RdlTagCollection FromBytes(byte[] data)
		{
			return FromString(Encoding.UTF8.GetString(data, 0, data.Length));
		}
		#endregion

		#region Private Classes
		private class TagKey
		{
			public string Key { get; set; }	

			public TagKey(RdlTag tag)
			{
				if (tag.TagName == RdlTagName.OBJ.ToString())
				{
					if (tag.TypeName == RdlObjectTypeName.ACTOR.ToString())
					{
						RdlObject obj = tag as RdlObject;
						this.Key = String.Concat(tag.TagName, tag.TypeName, obj.ID);
					}
					else if (tag.TypeName == RdlObjectTypeName.PROP.ToString())
					{
						RdlProperty prop = tag as RdlProperty;
						this.Key = String.Concat(tag.TagName, tag.TypeName, prop.ID, prop.Name);
					}
					else
					{
						this.Key = tag.ToString();
					}
				}
				else
				{
					this.Key = tag.ToString();
				}
			}

			public override bool Equals(object obj)
			{
				if (obj is TagKey)
				{
					return this.Key.Equals((obj as TagKey).Key);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return this.Key.GetHashCode();
			}
		}

		private class TagKeyComparer : IEqualityComparer<TagKey>
		{
			#region IEqualityComparer<TagKey> Members

			public bool Equals(TagKey x, TagKey y)
			{
				return x.Key.Equals(y.Key);
			}

			public int GetHashCode(TagKey obj)
			{
				return obj.Key.GetHashCode();
			}

			#endregion
		}
		#endregion
	}
}
