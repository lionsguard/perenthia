using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia
{
	public static class ItemGenerator
	{
		private static ArrayContainer _names = new ArrayContainer();
		private static SafeDictionary<ItemQualityType, ItemInfo[]> _prefixes = new SafeDictionary<ItemQualityType, ItemInfo[]>();
		private static SafeDictionary<ItemQualityType, ItemInfo[]> _suffixes = new SafeDictionary<ItemQualityType, ItemInfo[]>();

		static ItemGenerator()
		{
			var a = new[] { new { Name = "" }, new{ Name = ""} };

			// Prefixes
			_prefixes.Add(ItemQualityType.Poor, new ItemInfo[] { null });
			_prefixes.Add(ItemQualityType.Fair, new ItemInfo[] { null });
			_prefixes.Add(ItemQualityType.Moderate, new ItemInfo[] { null });
			_prefixes.Add(ItemQualityType.Good, new ItemInfo[] { null });
			_prefixes.Add(ItemQualityType.Excellent, new ItemInfo[] { null });
			_prefixes.Add(ItemQualityType.Master, new ItemInfo[] { null });
			_prefixes.Add(ItemQualityType.Legendary, new ItemInfo[] { null });

			// Suffixes
			_suffixes.Add(ItemQualityType.Poor, new ItemInfo[] { null });
			_suffixes.Add(ItemQualityType.Fair, new ItemInfo[] { null });
			_suffixes.Add(ItemQualityType.Moderate, new ItemInfo[] { null });
			_suffixes.Add(ItemQualityType.Good, new ItemInfo[] { null });
			_suffixes.Add(ItemQualityType.Excellent, new ItemInfo[] { null });
			_suffixes.Add(ItemQualityType.Master, new ItemInfo[] { null });
			_suffixes.Add(ItemQualityType.Legendary, new ItemInfo[] { null });

			// Need base templates for all root names...
			_names.Add(ItemType.Armor, EquipLocation.Head, new ItemInfo[] 
				{
					new ItemInfo{ Name = "Leather Skull Cap", Gold = 50, Power = 1},
					new ItemInfo{ Name = "Boiled Leather Skull Cap", Gold = 50, Power = 1},
					new ItemInfo{ Name = "Studded Leather Skull Cap", Gold = 50, Power = 1},
					new ItemInfo{ Name = "Bone Skull Cap", Gold = 50, Power = 1},
					new ItemInfo{ Name = "Iron Skull Cap", Gold = 50, Power = 1},
					new ItemInfo{ Name = "Steel Skull Cap", Gold = 50, Power = 1},
					new ItemInfo{ Name = "Chain Coif", Gold = 50, Power = 1},
					new ItemInfo{ Name = "Scale Coif", Gold = 50, Power = 1}
				});
		}

		public static IItem GetItem(int level)
		{
			// Determine item quality type.
			//Poor = 0,
			//Fair = 1,
			//Moderate = 2,
			//Good = 3,
			//Excellent = 4,
			//Master = 5,
			//Legendary = 6,
			ItemQualityType qualityType = ItemQualityType.Poor;
			if (level >= 20 && level < 33) qualityType = ItemQualityType.Fair;
			if (level >= 34 && level < 47) qualityType = ItemQualityType.Moderate;
			if (level >= 48 && level < 61) qualityType = ItemQualityType.Good;
			if (level >= 62 && level < 75) qualityType = ItemQualityType.Excellent;
			if (level >= 76 && level < 89) qualityType = ItemQualityType.Master;
			if (level >= 90) qualityType = ItemQualityType.Legendary;

			// Get a random item type.
			ItemType itemType = GetItemType();

			// Get the equip location.
			EquipLocation equipLocation = GetEquipLocation(itemType);

			var prefixes = _prefixes[qualityType];
			var names = _names[itemType, equipLocation];
			var suffixes = _suffixes[qualityType];

			// Generate an item based on quality type.
			switch (qualityType)
			{
				case ItemQualityType.Fair:
					break;
				case ItemQualityType.Moderate:
					break;
				case ItemQualityType.Good:
					break;
				case ItemQualityType.Excellent:
					break;
				case ItemQualityType.Master:
					break;
				case ItemQualityType.Legendary:
					break;
				default:
					// Poor
					// return CreateItem(itemType, equipLocation, qualityType, level);
					break;
			}
			return null;
		}

		private static ItemType GetItemType()
		{
			var types = new ItemType[] { ItemType.Armor, ItemType.Clothing, ItemType.Container, ItemType.Food, ItemType.Light, ItemType.Potion, ItemType.Shield, ItemType.Spell, ItemType.Weapon };
			return types[Dice.Random(0, types.Length - 1)];
		}

		private static EquipLocation GetEquipLocation(ItemType itemType)
		{
			EquipLocation[] equipLocations = new EquipLocation[] { EquipLocation.None };
			switch (itemType)
			{
				case ItemType.Armor:
					equipLocations = new EquipLocation[] { EquipLocation.Head, EquipLocation.Neck, EquipLocation.Shoulders, 
						EquipLocation.Arms, EquipLocation.Wrists, EquipLocation.Hands, EquipLocation.Chest, 
						EquipLocation.Back, EquipLocation.Waist, EquipLocation.Legs, EquipLocation.Feet };
					break;
				case ItemType.Clothing:
					equipLocations = new EquipLocation[] { EquipLocation.Hat, EquipLocation.Shirt, EquipLocation.Pants, 
						EquipLocation.Ear, EquipLocation.Finger, EquipLocation.Nose, EquipLocation.Pendant };
					break;
				case ItemType.Container:
					equipLocations = new EquipLocation[] { EquipLocation.Bag };
					break;
				case ItemType.Light:
					equipLocations = new EquipLocation[] { EquipLocation.Light };
					break;
				case ItemType.Spell:
					equipLocations = new EquipLocation[] { EquipLocation.Spell };
					break;
				case ItemType.Weapon:
					equipLocations = new EquipLocation[] { EquipLocation.Weapon };
					break;
				case ItemType.Shield:
					equipLocations = new EquipLocation[] { EquipLocation.Shield };
					break;
			}

			if (equipLocations.Length > 1)
			{
				return equipLocations[Dice.Random(0, equipLocations.Length - 1)];
			}
			return equipLocations[0];
		}


		private class ItemInfo
		{
			public string Name { get; set; }
			public int Gold { get; set; }
			public int Emblem { get; set; }
			public int Power { get; set; }	
		}


		private class SafeDictionary<TKey, TValue> : Dictionary<TKey, TValue>
		{
			public new TValue this[TKey key]
			{
				get
				{
					if (this.ContainsKey(key))
					{
						return base[key];
					}
					return default(TValue);
				}
				set
				{
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
		}



		private class ArrayKey
		{
			public ItemType ItemType { get; set; }
			public EquipLocation EquipLocation { get; set; }

			public override bool Equals(object obj)
			{
				ArrayKey key = obj as ArrayKey;
				if (key != null)
				{
					if (key.ItemType == this.ItemType)
					{
						return key.EquipLocation == this.EquipLocation;
					}
				}
				return false;
			}

			public override int GetHashCode()
			{
				return ((int)this.ItemType) + ((int)this.EquipLocation);
			}
		}

		private class ArrayContainer : SafeDictionary<ArrayKey, ItemInfo[]>
		{
			public ItemInfo[] this[ItemType itemType, EquipLocation equipLocation]
			{
				get { return base[new ArrayKey { ItemType = itemType, EquipLocation = equipLocation }]; }
				set { base[new ArrayKey { ItemType = itemType, EquipLocation = equipLocation }] = value; }
			}

			public void Add(ItemType itemType, EquipLocation equipLocation, ItemInfo[] value)
			{
				base.Add(new ArrayKey { ItemType = itemType, EquipLocation = equipLocation }, value);
			}
		}
	}
}
