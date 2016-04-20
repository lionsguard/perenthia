using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia
{
	/// <summary>
	/// Represents an object used to store references to templates within the Perenthia world.
	/// </summary>
	[System.Runtime.Serialization.DataContract]
	public class TemplateItem
	{
		/// <summary>
		/// Gets or sets the quantity of the item.
		/// </summary>
		[System.Runtime.Serialization.DataMember]
		public int Quantity { get; set; }

		/// <summary>
		/// Gets or sets the name of the template this instance represents.
		/// </summary>
		[System.Runtime.Serialization.DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Returns the current object as a string.
		/// </summary>
		/// <returns>The string representation of the current object.</returns>
		public override string ToString()
		{
			return this.ToJson();
		}

		/// <summary>
		/// Returns a TemplateItem instance from the specified string.
		/// </summary>
		/// <param name="stringValue">The string to parse into a TemplateItem instance.</param>
		/// <returns>An instance of TemplateItem.</returns>
		public static TemplateItem FromString(string stringValue)
		{
			return JsonHelper.FromJson<TemplateItem>(stringValue);
		}

		/// <summary>
		/// Parses a string in the following format into an TemplateItem instance. Format: name=quantity,name=quantity
		/// </summary>
		/// <param name="nameQuantityPairs">The name=quantity,name=quantity formatted string to parse.</param>
		/// <returns>An IEnumerable list of TemplateItem instances from the parsed string; otherwise null.</returns>
		public static IEnumerable<TemplateItem> Parse(string nameQuantityPairs)
		{
			List<TemplateItem> list = new List<TemplateItem>();
			string[] pairs = nameQuantityPairs.Split(',');
			if (pairs != null && pairs.Length > 0)
			{
				for (int i = 0; i < pairs.Length; i++)
				{
					string[] values = pairs[i].Split('=');
					if (values != null && values.Length == 2)
					{
						if (!String.IsNullOrEmpty(values[0]))
						{
							int qty;
							if (Int32.TryParse(values[1].Trim(), out qty))
							{
								list.Add(new TemplateItem { Name = values[0].Trim(), Quantity = qty });
							}
						}
					}
				}
			}
			return list;
		}
	}

	/// <summary>
	/// Represents a collection of TemplateItem instances where the values are stored with the properties of the owner instance.
	/// </summary>
	public class TemplateItemCollection : ActorOwnedDictionaryBase<string>
	{
		/// <summary>
		/// Gets the TemplateItem instance with the specified key.
		/// </summary>
		/// <param name="key">The unique key used to located the desired item.</param>
		/// <returns>An instance of TemplateItem.</returns>
		public new TemplateItem this[string key]
		{
			get { return TemplateItem.FromString(this.Owner.Properties.GetValue<string>(this.GetPrefixedName(key))); }
			set { this.Owner.Properties.SetValue(this.GetPrefixedName(key), value.ToString(), this.IsTemplateCollection); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not this template item collection is specified on a template 
		/// record and therefore should have its contents contained on the template control.
		/// </summary>
		public bool IsTemplateCollection { get; set; }	

		/// <summary>
		/// Initializes a new instance of the TemplateItemCollection class.
		/// </summary>
		/// <param name="owner">The owner of the current instance.</param>
		/// <param name="prefix">The prefix used to store the key/value pairs of the collection on the owner object.</param>
		public TemplateItemCollection(IActor owner, string prefix)
			: base(owner, prefix)
		{
		}

		/// <summary>
		/// Adds a new TemplateItem to the collection.
		/// </summary>
		/// <param name="item">The item to add to the collection.</param>
		public void Add(TemplateItem item)
		{
			this.Add(item.Name, item.ToString());
		}

		/// <summary>
		/// Adds a new item to the collection.
		/// </summary>
		/// <param name="key">The key of the item to add.</param>
		/// <param name="value">The ToString value of the template item to add.</param>
		public override void Add(string key, string value)
		{
			this.Owner.Properties.SetValue(this.GetPrefixedName(key), value, this.IsTemplateCollection);
		}

		/// <summary>
		/// Tries to get the value at the specified key.
		/// </summary>
		/// <param name="key">The key in which to search the collection.</param>
		/// <param name="value">The JSON serialized value of the TemplateItem instance.</param>
		/// <returns>True if the item was found; otherwise false.</returns>
		public override bool TryGetValue(string key, out string value)
		{
			if (this.Owner.Properties.ContainsKey(this.GetPrefixedName(key)))
			{
				value = this.Owner.Properties.GetValue<string>(this.GetPrefixedName(key));
				return true;
			}
			value = typeof(Item).FullName;
			return false;
		}

		public bool ContainsItem(string name)
		{
			foreach (var t in this)
			{
				if (t.Name.Equals(name)) return true;
			}
			return false;
		}

		public IEnumerable<TemplateItem> GetTemplateItems()
		{
			List<TemplateItem> list = new List<TemplateItem>();
			foreach (var t in this)
			{
				list.Add(t);
			}
			return list.ToArray();
		}

		/// <summary>
		/// Gets an IEnumerator of TemplateItem instances contained within the current collection.
		/// </summary>
		/// <returns>An IEnumerator of TemplateItem instances.</returns>
		public new IEnumerator<TemplateItem> GetEnumerator()
		{
			List<TemplateItem> list = new List<TemplateItem>();
			foreach (var item in this.Values)
			{
				list.Add(TemplateItem.FromString(item));
			}
			return list.GetEnumerator();
		}
	}
}
