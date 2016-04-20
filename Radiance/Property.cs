using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Radiance.Markup;

namespace Radiance
{
	/// <summary>
	/// Represents a property of an object.
	/// </summary>
	public class Property : IComparable<Property>
	{
		/// <summary>
		/// Gets or sets the name of the property.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the value of the property.
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not this property is intended for a template class. This property will 
		/// not be serialized if the parent collection is marked as IsTemplateCollection.
		/// </summary>
		public bool IsTemplateProperty { get; set; }	

		/// <summary>
		/// Gets the data type of the property value.
		/// </summary>
		public Type DataType
		{
			get
			{
				if (this.Value != null)
				{
					return this.Value.GetType();
				}
				return typeof(object);
			}
		}

		/// <summary>
		/// Gets the value of the property cast as the specified T type.
		/// </summary>
		/// <typeparam name="T">The type to cast the value as.</typeparam>
		/// <returns>The value of the current property or null if the property was empty or not found.</returns>
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
				else if (typeof(T) == typeof(String))
				{
					if (!String.IsNullOrEmpty(val.ToString()))
					{
						return (T)val;
					}
				}
				return (T)Convert.ChangeType(val, typeof(T), null);
			}
			return default(T);
		}

		#region IComparable<Property> Members

		/// <summary>
		/// Compares the current property to the specified property.
		/// </summary>
		/// <param name="other">The property to compare to the current property.</param>
		/// <returns>The comparison value.</returns>
		public int CompareTo(Property other)
		{
			return StringComparer.InvariantCultureIgnoreCase.Compare(this.Name, other.Name);
		}

		#endregion
	}

	/// <summary>
	/// Represents a collection of properties of an object.
	/// </summary>
	[System.Xml.Serialization.XmlRoot("properties")]
	public class PropertyCollection : Dictionary<string, Property>, IXmlSerializable
	{
		/// <summary>
		/// Gets the property instance with the specified name.
		/// </summary>
		/// <param name="name">The name of the property to retrieve.</param>
		/// <returns>A property instance with the specified name or null if the property was not found.</returns>
		public new Property this[string name]
		{
			get
			{
				if (this.ContainsKey(name))
				{
					return base[name];
				}
				return new Property() { Name = name, Value = null };
			}
			set
			{
				this.SetValue(name, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not this collection is for a template class, meaning that only properties 
		/// marked as IsTemplateProperty will be serialized. Likewise, if this value is false then properties that do not have 
		/// the IsTemplateProperty value set to true will nt be serialized.
		/// </summary>
		public bool IsTemplateCollection { get; set; }

		/// <summary>
		/// Initializes a new instance of the property collection.
		/// </summary>
		public PropertyCollection()
			: base (StringComparer.InvariantCultureIgnoreCase)
		{
		}

		/// <summary>
		/// Adds a new property intance to the collection.
		/// </summary>
		/// <param name="property">The property instance to add to the collection.</param>
		public void Add(Property property)
		{
			this.SetValue(property.Name, property.Value, property.IsTemplateProperty);
		}

		/// <summary>
		/// Adds a new property to the collection.
		/// </summary>
		/// <param name="name">The name of the property to add to the collection.</param>
		/// <param name="value">The value of the property to add to the collection.</param>
		public void Add(string name, object value)
		{
			this.SetValue(name, value);
		}

		/// <summary>
		/// Adds a new property to the collection.
		/// </summary>
		/// <param name="key">The name of the property to add.</param>
		/// <param name="value">The instance of the property to add.</param>
		public new void Add(string key, Property value)
		{
			this.SetValue(key, value.Value, value.IsTemplateProperty);
		}

		/// <summary>
		/// Adds a range of property instances to the collection.
		/// </summary>
		/// <param name="collection">The list of property instances to add to the collection.</param>
		public void AddRange(IEnumerable<Property> collection)
		{
			this.AddRange(collection, true);
		}

		/// <summary>
		/// Adds a range of property instances to the collection.
		/// </summary>
		/// <param name="collection">The list of property instances to add to the collection.</param>
		/// <param name="overwriteExisting">A value indicating whether or not existing properties of the same 
		/// name are overwritten by the specified collection.</param>
		public void AddRange(IEnumerable<Property> collection, bool overwriteExisting)
		{
			foreach (var item in collection)
			{
				if (this.ContainsKey(item.Name))
				{
					if (overwriteExisting)
					{
						this.SetValue(item.Name, item.Value, item.IsTemplateProperty);
					}
				}
				else
				{
					this.SetValue(item.Name, item.Value, item.IsTemplateProperty);
				}
			}
		}

		/// <summary>
		/// Gets the value of the specified property.
		/// </summary>
		/// <typeparam name="T">The System.Type to cast the value as.</typeparam>
		/// <param name="name">The name of the property to retrieve.</param>
		/// <returns>The value of the property or the default of T if the property was not found.</returns>
		public T GetValue<T>(string name)
		{
			return this[name].GetValue<T>();
		}

		/// <summary>
		/// Gets the value of the specified property.
		/// </summary>
		/// <typeparam name="T">The System.Type to cast the value as.</typeparam>
		/// <param name="name">The name of the property to retrieve.</param>
		/// <param name="defaultValue">The default value to return if the property was not found.</param>
		/// <returns>The value of the property or the default of T if the property was not found.</returns>
		public T GetValue<T>(string name, T defaultValue)
		{
			if (this.ContainsKey(name))
			{
				return this[name].GetValue<T>();
			}
			return defaultValue;
		}

		/// <summary>
		/// Sets or adds the value of the specified property.
		/// </summary>
		/// <param name="name">The name of the property to set.</param>
		/// <param name="value">The value of the property to set.</param>
		public void SetValue(string name, object value)
		{
			this.SetValue(name, value, this.IsTemplateCollection);
		}

		/// <summary>
		/// Sets or adds the value of the specified property.
		/// </summary>
		/// <param name="name">The name of the property to set.</param>
		/// <param name="value">The value of the property to set.</param>
		/// <param name="isTemplateProperty">A value indicating whether or not this is a template property.</param>
		public void SetValue(string name, object value, bool isTemplateProperty)
		{
			if (this.ContainsKey(name))
			{
				base[name].Value = value;
				base[name].IsTemplateProperty = isTemplateProperty;
			}
			else
			{
				base.Add(name, new Property() { Name = name, Value = value, IsTemplateProperty = isTemplateProperty });
		
            }
		}

        /// <summary>
        /// Sets the values of the properties collection using the values from the RdlProperty tags.
        /// </summary>
        /// <param name="properties">The collection of RdlProperties to use when setting the current collection values.</param>
        public void SetValues(List<RdlProperty> properties)
        {
            foreach (var item in properties)
            {
                this.SetValue(item.Name, item.Value);
            }
        }

		#region IXmlSerializable Members
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			if (reader.ReadState != ReadState.Interactive) reader.Read();
			if (reader.IsStartElement("properties") && !reader.IsEmptyElement)
			{
				reader.ReadStartElement("properties");
				while (reader.NodeType != XmlNodeType.EndElement)
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						string key = String.Empty;
						string isTemp = String.Empty;
						object value = null;

						// Serialized with XmlSerializer.
						key = reader.GetAttribute("name");
						isTemp = reader.GetAttribute("isTemplateProperty");
						value = reader.ReadElementContentAsObject();
						if (!String.IsNullOrEmpty(key))
						{
							if (!String.IsNullOrEmpty(isTemp))
							{
								bool result;
								Boolean.TryParse(isTemp, out result);
								this.Add(new Property { Name = key, IsTemplateProperty = result, Value = value });
							}
							else
							{
								this.Add(key, value);
							}
						}
					}
					reader.MoveToContent();
				}
				reader.ReadEndElement();
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			foreach (var item in this)
			{
				if (item.Value.IsTemplateProperty == this.IsTemplateCollection)
				{
					writer.WriteStartElement("property");
					writer.WriteAttributeString("name", item.Key);
					writer.WriteAttributeString("isTemplateProperty", item.Value.IsTemplateProperty.ToString());
					if (item.Value.Value != null && item.Value.Value.GetType().IsEnum)
					{
						writer.WriteValue(item.Value.Value.ToString());
					}
					else
					{
						writer.WriteValue(item.Value.Value);
					}
					writer.WriteEndElement();
				}
			}
		}

		#endregion
	}
}
