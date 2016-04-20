using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;

using Radiance;
using Radiance.Markup;
using System.Collections.Generic;

namespace Perenthia.Models
{
	public class Actor
	{
		#region Properties
		public int ID { get; set; }
		public string Name { get; set; }
		public PropertyCollection Properties { get; private set; }

		public IEnumerable<Property> PropertyList
		{
			get { return this.Properties.Values; }
		}

		public string Description
		{
			get { return this.Properties.GetValue<string>(DescriptionProperty); }
			set { this.Properties.SetValue(DescriptionProperty, value); }
		}
		public static readonly string DescriptionProperty = "Description";

		/// <summary>
		/// Gets or sets the IsUsable property value.
		/// </summary>
		public bool IsUsable
		{
			get { return Properties.GetValue<bool>(IsUsablePropertyName); }
			set { Properties.SetValue(IsUsablePropertyName, value); }
		}
		/// <summary>
		/// Gets the name of the IsUsable property as stored in the object property collection.
		/// </summary>
		public const string IsUsablePropertyName = "IsUsable";

		/// <summary>
		/// Gets or sets the IsStackable property value.
		/// </summary>
		public bool IsStackable
		{
			get { return Properties.GetValue<bool>(IsStackablePropertyName); }
			set { Properties.SetValue(IsStackablePropertyName, value); }
		}
		/// <summary>
		/// Gets the name of the IsStackable property as stored in the object property collection.
		/// </summary>
		public const string IsStackablePropertyName = "IsStackable";

		/// <summary>
		/// Gets or sets the IsEquipped property value.
		/// </summary>
		public bool IsEquipped
		{
			get { return Properties.GetValue<bool>(IsEquippedPropertyName); }
			set { Properties.SetValue(IsEquippedPropertyName, value); }
		}
		/// <summary>
		/// Gets the name of the IsEquipped property as stored in the object property collection.
		/// </summary>
		public const string IsEquippedPropertyName = "IsEquipped";	

		/// <summary>
		/// Gets or sets the Quantity property value.
		/// </summary>
		public int Quantity
		{
			get { return Properties.GetValue<int>(QuantityPropertyName); }
			set { Properties.SetValue(QuantityPropertyName, value); }
		}
		/// <summary>
		/// Gets the name of the Quantity property as stored in the object property collection.
		/// </summary>
		public const string QuantityPropertyName = "Quantity";

		/// <summary>
		/// Gets or sets the ImageUri property value.
		/// </summary>
		public string ImageUri
		{
			get { return Properties.GetValue<string>(ImageUriPropertyName); }
			set { Properties.SetValue(ImageUriPropertyName, value); }
		}
		/// <summary>
		/// Gets the name of the ImageUri property as stored in the object property collection.
		/// </summary>
		public const string ImageUriPropertyName = "ImageUri";

		/// <summary>
		/// Gets or sets the ObjectType property value.
		/// </summary>
		public ObjectType ObjectType
		{	
			get { return Properties.GetValue<ObjectType>(ObjectTypePropertyName); }
			set { Properties.SetValue(ObjectTypePropertyName, value); }
		}
		/// <summary>
		/// Gets the name of the ObjectType property as stored in the object property collection.
		/// </summary>
		public const string ObjectTypePropertyName = "ObjectType";

		/// <summary>
		/// Gets or sets the EquipLocation property value.
		/// </summary>
		public EquipLocation EquipLocation
		{
			get { return Properties.GetValue<EquipLocation>(EquipLocationPropertyName); }
			set { Properties.SetValue(EquipLocationPropertyName, value); }
		}
		/// <summary>	
		/// Gets the name of the EquipLocation property as stored in the object property collection.
		/// </summary>
		public const string EquipLocationPropertyName = "EquipLocation";

		public ImageSource ImageSource
		{
			get
			{
				var uri = this.ImageUri;
				if (String.IsNullOrEmpty(uri))
				{
					var race = this.Properties.GetValue<string>("Race");
					var gender = this.Properties.GetValue<Gender>("Gender");

					if (!String.IsNullOrEmpty(race) && gender != Gender.None)
					{
						uri = String.Format(Asset.AVATAR_FORMAT,
							race,
							gender);
					}
				}
				return ImageManager.GetImageSource(uri);
			}
		}
		#endregion

		public Actor()
		{
			this.Properties = new PropertyCollection();
		}

		public Actor(RdlActor tag)
			: this()
		{
			this.ID = tag.ID;
			this.Name = tag.Name;

			// Properties
			foreach (var property in tag.Properties)
			{
				this.Properties.SetValue(property.Name, property.Value);
			}
		}

		public RdlActor ToRdlActor()
		{
			RdlActor actor = new RdlActor(this.ID, this.Name);
			foreach (var prop in this.Properties.Values)
			{
				actor.Properties.Add(new RdlProperty(this.ID, prop.Name, prop.Value));
			}
			return actor;
		}
	}


	public class ActorCollection : List<RdlActor>
	{
		public Avatar Owner { get; private set; }

		public ActorCollection(Avatar owner)
		{
			this.Owner = owner;
		}

		public void Remove(int ownerId, int itemId)
		{
			int index = -1;
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].ID == itemId && (this.Owner.ID == ownerId || this[i].OwnerID == ownerId))
				{
					index = i;
					break;
				}
			}
			if (index >= 0)
			{
				this.RemoveAt(index);
			}
		}

		public void Update(RdlActor item)
		{
			// If the item already exits then remove it.
			int index = -1;
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].ID == item.ID)
				{
					index = i;
					break;
				}
			}
			if (index >= 0)
			{
				this.RemoveAt(index);
			}

			// Add the item to the collection.
			if (item.OwnerID == this.Owner.ID || this.Count(i => item.OwnerID == i.ID) > 0)
			{
				this.Add(item);
			}
		}

		public List<RdlActor> GetContainers()
		{
			List<RdlActor> list = new List<RdlActor>();
			foreach (var id in this.Owner.Properties.Values.Where(p => p.Name.StartsWith("Bag_")).Select(p => p.GetValue<int>()))
			{
				var item = this.Where(i => i.ID == id).FirstOrDefault();
				if (item != null)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public List<RdlActor> GetSpells()
		{
			List<RdlActor> list = new List<RdlActor>();
			foreach (var id in this.Owner.Properties.Values.Where(p => p.Name.StartsWith("Spell_")).Select(p => p.GetValue<int>()))
			{
				var item = this.Where(i => i.ID == id).FirstOrDefault();
				if (item != null)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public List<RdlActor> GetEquipment()
		{
			List<RdlActor> list = new List<RdlActor>();
			foreach (var id in this.Owner.Properties.Values.Where(p => p.Name.StartsWith("Equipment_")).Select(p => p.GetValue<int>()))
			{
				var item = this.Where(i => i.ID == id && i.IsEquipped()).FirstOrDefault();
				if (item != null)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public List<RdlActor> GetQuests()
		{
			return this.Where(i => i.ObjectType() == ObjectType.Quest).ToList();
		}

		public List<RdlActor> GetContents()
		{
			List<RdlActor> list = new List<RdlActor>();

			this.GetContentsInternal(this.Owner.ID, list);

			foreach (var item in this.Where(i => i.OwnerID == this.Owner.ID))
			{
				this.GetContentsInternal(item.ID, list);
			}

			return list;
		}

		public List<RdlActor> GetContents(int ownerId)
		{
			List<RdlActor> list = new List<RdlActor>();

			this.GetContentsInternal(ownerId, list);

			return list;
		}

		public void GetContentsInternal(int ownerId, List<RdlActor> list)
		{
			foreach (var item in this.Where(i => i.OwnerID == ownerId && i.ObjectType() != ObjectType.Quest))
			{
				if (item.IsStackable())
				{
					var existingItem = list.Where(i => i.OwnerID == ownerId
						&& i.Name.Equals(item.Name)
						 && i.ObjectType() != ObjectType.Quest).FirstOrDefault();
					if (existingItem != null)
					{
						existingItem.Properties.SetValue("Quantity", this.Count(i => i.OwnerID == ownerId && i.Name.Equals(item.Name)));
					}
					else
					{
						list.Add(item);
					}
				}
				else
				{
					list.Add(item);
				}
			}
		}
	}
}
