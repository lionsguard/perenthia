using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Providers;

namespace Perenthia
{
	public static class ImportManager
	{
		public static List<string> GetImportTypes()
		{
			return new List<string>(new string[] { ImportTypes.Creature, ImportTypes.Item, ImportTypes.Npc, ImportTypes.Quest, ImportTypes.Award });
		}

		public static void Import(string type, string csvFileName, WorldProvider provider)
		{
			using (StreamReader reader = new StreamReader(csvFileName))
			{
				Import(type, reader.BaseStream, provider);
			}
		}

		public static void Import(string type, Stream csvFileStream, WorldProvider provider)
		{
			List<string> lines = new List<string>();
			StringBuilder sb = new StringBuilder();
			using (CsvReader reader = new CsvReader(csvFileStream))
			{
				string[] line = reader.GetCSVLine();
				while ((line = reader.GetCSVLine()) != null)
				{
					switch (type)
					{
						case ImportTypes.Creature:
							ParseCreature(line, sb, provider);
							break;
						case ImportTypes.Npc:
							ParseNpc(line, sb, provider);
							break;
						case ImportTypes.Item:
							ParseItem(line, sb, provider);
							break;
						case ImportTypes.Quest:
							ParseQuest(line, sb, provider);
                            break;
                        case ImportTypes.Award:
                            ParseAward(line, sb, provider);
                            break;
					}
				}
			}
		}

		private static void ParseCreature(string[] parts, StringBuilder sb, WorldProvider provider)
		{
			// Columns
			// Name,Creature Type,CreatureClass,Level,Min Attribute,Max Attribute,Gender,Race,ImageUri,DroppableItems
			if (parts != null && parts.Length > 0)
			{
				// Changes for Perenthia 11/23/2008
				string name = parts[0];
				string type = parts[1];
				string group = parts[2];
				string level = parts[3];
				string minAttr = parts[4];
				string maxAttr = parts[5];
				string gender = parts[6];
				string race = parts[7];
				string imageUri = parts[8];
				string drpItms = parts[9];

				string formattedName = FormatName(name);

				List<string> dropItems = new List<string>();
				if (!String.IsNullOrEmpty(drpItms.Replace("\"", "").Trim()))
				{
					dropItems.AddRange(drpItms.Replace("\"", "").Split(',').Select(i => i.Trim()));
				}

				Type t = Type.GetType(String.Concat("Perenthia.", group, "Creature"));
				Creature creature = provider.GetTemplate(t, name) as Creature;
				if (creature == null)
				{
					creature = Activator.CreateInstance(t) as Creature;
				}
				if (creature != null)
				{
					creature.Properties.IsTemplateCollection = true;
					creature.World = provider.World;
					creature.TemplateID = 0;
					creature.Name = name;
					creature.Race = race;
					creature.ImageUri = imageUri;
					creature.MobileType = (MobileTypes)Enum.Parse(typeof(MobileTypes), type, true);
					creature.Level = Convert.ToInt32(level);
					creature.AttributeMinimum = Convert.ToInt32(minAttr);
					creature.AttributeMaximum = Convert.ToInt32(maxAttr);
					creature.Skills.AddRange(GetSkillGroup(group).Skills);
					creature.GenerateStats();

					Gender g = (Gender)Enum.Parse(typeof(Gender), gender, true);
					if (g == Gender.None)
					{
						creature.Gender = (Gender)Dice.Random(1, 2);
					}
					else
					{
						creature.Gender = g;
					}

					foreach (var item in dropItems)
					{
						creature.AddDroppableItem(item);
					}

					provider.SaveActor<Creature>(creature);
				}

				//sb.AppendFormat("\t\t#region {0}", formattedName).AppendLine();
				//sb.AppendFormat("\t\tpublic static Creature {0}", formattedName).AppendLine();
				//sb.Append("\t\t{").AppendLine();
				//sb.Append("\t\t\tget { ");
				//sb.AppendFormat("return new Creature(\"{0}\", \"{1}\", {2}, {3}, {4}, MobileTypes.{5}, new {6}SkillGroup()); ",
				//    name, String.Empty, level, minAttr, maxAttr, type, group);
				//sb.Append("}").AppendLine();
				//sb.Append("\t\t}").AppendLine();
				//sb.Append("\t\t#endregion").AppendLine().AppendLine();
			}
		}

		private static void ParseNpc(string[] parts, StringBuilder sb, WorldProvider provider)
		{
			// Name,Mobile Type,Level,Gender,Race,AttrMin,AttrMax,CanAttack,Currency,SkillGroup,
			// Markup,Markdown,Droppable Items,OnEnterMsg,Keywords,Responses
			if (parts != null && parts.Length > 0)
			{
				string name = parts[0];
				string type = parts[1];
				string level = parts[2];
				string gender = parts[3];
				string race = parts[4];
				string attrMin = parts[5];
				string attrMax = parts[6];
				string canAttack = parts[7];
				string currency = parts[8];
				string skillGroup = parts[9];
				string markup = parts[10];
				string markdown = parts[11];
				string drpItems = parts[12];
				string onEnterMsg = parts[13];
				string keywords = parts[14];
				string responses = parts[15];

				string formattedName = FormatName(name);

				List<string> dropItems = new List<string>();
				if (!String.IsNullOrEmpty(drpItems.Replace("\"", "").Trim()))
				{
					dropItems.AddRange(drpItems.Replace("\"", "").Split(',').Select(i => i.Trim()));
				}

				Type t = Type.GetType(String.Concat("Perenthia.", type));
				Npc npc = provider.GetTemplate(t, name) as Npc;
				if (npc == null)
				{
					npc = Activator.CreateInstance(t) as Npc;
				}

				npc.Properties.IsTemplateCollection = true;
				npc.World = provider.World;
				npc.TemplateID = 0;
				npc.Name = name;
				npc.Level = Convert.ToInt32(level);
				npc.AttributeMinimum = Convert.ToInt32(attrMin);
				npc.AttributeMaximum = Convert.ToInt32(attrMax);
				npc.MobileType = (MobileTypes)Enum.Parse(typeof(MobileTypes), type, true);
				npc.Skills.AddRange(GetSkillGroup(skillGroup).Skills);
				npc.GenerateStats();

				foreach (var item in dropItems)
				{
					npc.AddDroppableItem(item);
				}


				//sb.AppendFormat("\t\t#region {0}", formattedName).AppendLine();
				//sb.AppendFormat("\t\tpublic static Npc {0}", formattedName).AppendLine();
				//sb.Append("\t\t{").AppendLine();
				//sb.Append("\t\t\tget").AppendLine().Append("\t\t\t{ ").AppendLine();
				//sb.AppendFormat("\t\t\t\treturn new Npc(\"{0}\", \"{1}\", {2}, {3}, {4}, MobileTypes.{5}, new {6}SkillGroup())",
				//    name, String.Empty, level, attrMin, attrMax, type, skillGroup).AppendLine();

				//sb.Append("\t\t\t\t{ ").AppendLine();

				// Default properties not in constructor.
				//int propCount = 0;
				if (!String.IsNullOrEmpty(gender))
				{
					npc.Gender = (Gender)Enum.Parse(typeof(Gender), gender, true);
					//sb.AppendFormat("\t\t\t\t\tGender = Gender.{0}", gender);
					//propCount++;
				}
				if (!String.IsNullOrEmpty(race.Trim()))
				{
					npc.Race = race;
					//if (propCount > 0) sb.Append(",").AppendLine();
					//sb.AppendFormat("\t\t\t\t\tRace = {0}Race.RaceName", race.Trim());
					//propCount++;
				}
				if (!String.IsNullOrEmpty(currency))
				{
					npc.Currency = new Currency(Convert.ToInt32(currency));
					//if (propCount > 0) sb.Append(",").AppendLine();
					//sb.AppendFormat("\t\t\t\t\tCurrency = new Currency({0})", currency);
					//propCount++;
				}
				if (!String.IsNullOrEmpty(canAttack))
				{
					npc.CanAttack = false;
					//if (propCount > 0) sb.Append(",").AppendLine();
					//sb.AppendFormat("\t\t\t\t\tCanAttack = {0}", canAttack.ToLower());
					//propCount++;
				}
				if (!String.IsNullOrEmpty(markup))
				{
					npc.MarkupPercentage = Convert.ToDouble(markup);
					//if (propCount > 0) sb.Append(",").AppendLine();
					//sb.AppendFormat("\t\t\t\t\tMarkupPercentage = {0}", markup);
					//propCount++;
				}
				if (!String.IsNullOrEmpty(markdown))
				{
					npc.MarkdownPercentage = Convert.ToDouble(markdown);
					//if (propCount > 0) sb.Append(",").AppendLine();
					//sb.AppendFormat("\t\t\t\t\tMarkdownPercentage = {0}", markdown);
					//propCount++;
				}
				if (!String.IsNullOrEmpty(onEnterMsg))
				{
					npc.OnEnterMessage = onEnterMsg.Replace("\"", "");
					//if (propCount > 0) sb.Append(",").AppendLine();
					//sb.AppendFormat("\t\t\t\t\tOnEnterMessage = \"{0}\"", onEnterMsg);
					//propCount++;
				}
				//sb.AppendLine().Append("\t\t\t\t};").AppendLine();
				
				//sb.Append("\t\t\t}").AppendLine();
				//sb.Append("\t\t}").AppendLine();
				//sb.Append("\t\t#endregion").AppendLine().AppendLine();

				provider.SaveActor<Npc>(npc);
			}
		}

		private static void ParseItem(string[] parts, StringBuilder sb, WorldProvider provider)
		{
			// Columns
			// ItemType,Name,Skills,SkillLevelReq,EquipLocation,Power,Protection,SpellType/FoodType,Range,Durability,
			// Capacity,Currency,Emblem,Flags,Affects,ImageUri,Description,Custom Properties,IngreidentItems

			if (parts != null && parts.Length > 0)
			{
				string type = parts[0];
				string name = parts[1];
				string skill = parts[2];
				string reqSkill = parts[3];
				string equip = parts[4];
				string power = parts[5];
				string protection = parts[6];
				string spellType = parts[7];
				string range = parts[8];
				string durability = parts[9];
				string capacity = parts[10];
				string currency = parts[11];
				string emblem = parts[12];
				string flags = parts[13];
				string affects = parts[14];
				string image = parts[15];
				string desc = parts[16];
				string props = parts[17];
				string ingredItems = parts[18];

				CustomPropertyCollection customProps = CustomPropertyCollection.Parse(props);

				if (!String.IsNullOrEmpty(ingredItems)) ingredItems.Replace("\"", "").Trim();

				string formattedName = FormatName(name);
				ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), type, true);

				if (String.IsNullOrEmpty(desc)) desc = String.Empty;

				Type t = Type.GetType(String.Concat("Perenthia.", type));
				if (itemType == ItemType.Spell)
				{
					// Spells have additional types beyond just the Spell sub class.
					t = Type.GetType(String.Concat("Perenthia.", spellType, "Spell"));
				}
				Item item = provider.GetTemplate(t, name) as Item;
				if (item == null)
				{
					item = Activator.CreateInstance(t) as Item;
				}
				item.Properties.IsTemplateCollection = true;
				item.TemplateID = 0;
				item.Name = name;
				// Set description as a template item, can be overridden with a custom description.
				item.Properties.SetValue(Actor.DescriptionProperty, desc.Replace("\"", String.Empty).Trim(), true);

				switch (itemType)
				{
					case ItemType.Item:
					case ItemType.QuestItem:
						break;
					case ItemType.Armor:
					case ItemType.Clothing:
						(item as Armor).Protection = Convert.ToInt32(protection);
						break;
					case ItemType.Container:
						(item as Container).Capacity = Convert.ToInt32(capacity);
						break;
					case ItemType.Food:
						(item as Food).Power = Convert.ToInt32(power);
						break;
					case ItemType.Light:
						(item as Light).Range = Convert.ToInt32(range);
						break;
					case ItemType.Spell:
						(item as Spell).Power = Convert.ToInt32(power);
						(item as Spell).Range = Convert.ToInt32(range);
						break;
					case ItemType.Weapon:
						(item as Weapon).Power = Convert.ToInt32(power);
						(item as Weapon).Range = Convert.ToInt32(range);
						break;
					case ItemType.Shield:
						(item as Shield).Protection = Convert.ToInt32(protection);
						if (!String.IsNullOrEmpty(power) && !String.IsNullOrEmpty(range))
						{
							(item as Shield).Power = Convert.ToInt32(power);
							(item as Shield).Range = Convert.ToInt32(range);
						}
						break;
					case ItemType.Transport:
						break;
					case ItemType.RepairKit:
						break;
					case ItemType.Artifact:
						break;
					case ItemType.Recipe:
						(item as Recipe).SkillRequired = skill.Trim();
						(item as Recipe).SkillValueRequired = Convert.ToInt32(reqSkill);
						(item as Recipe).Ingredients.IsTemplateCollection = true;
						foreach (var ingredient in TemplateItem.Parse(ingredItems))
						{
							(item as Recipe).Ingredients.Add(ingredient);
						}
						break;
					case ItemType.TrainSkill:
						(item as TrainSkill).Skill = skill.Trim();
						break;
					case ItemType.Potion:
						(item as Potion).Power = Convert.ToInt32(power);
						(item as Potion).Type = (PotionType)Enum.Parse(typeof(PotionType), spellType, true);
						break;
				}

				//sb.Append("\t\t\t\t{ ").AppendLine();

				if (!String.IsNullOrEmpty(equip))
				{
					item.EquipLocation = (EquipLocation)Enum.Parse(typeof(EquipLocation), equip, true);
				}

				// Default properties not in constructor.
				//ImageUri = "item-light-candle.png" 
				//int propCount = 0;
				if (!String.IsNullOrEmpty(flags))
				{
					item.Flags = (ActorFlags)Enum.Parse(typeof(ActorFlags), flags, true);
					//sb.AppendFormat("\t\t\t\t\tFlags = {0}", GetFlags(flags));
					//propCount++;
				}
				if (!String.IsNullOrEmpty(skill) && !String.IsNullOrEmpty(skill.Trim()))
				{
					item.Skill = skill.Trim();
					//if (propCount > 0) sb.Append(",").AppendLine();
					//sb.AppendFormat("\t\t\t\t\tSkill = \"{0}\"", skill.Trim());
					//propCount++;
				}
				if (!String.IsNullOrEmpty(reqSkill))
				{
					item.SkillLevelRequiredToEquip = Convert.ToInt32(reqSkill);
					//if (propCount > 0) sb.Append(",").AppendLine();
					//sb.AppendFormat("\t\t\t\t\tSkillLevelRequiredToEquip = {0}", reqSkill);
					//propCount++;
				}
				if (!String.IsNullOrEmpty(currency))
				{
					item.Cost = new Currency(Convert.ToInt32(currency));
					//if (propCount > 0) sb.Append(",").AppendLine();
					//sb.AppendFormat("\t\t\t\t\tCost = new Currency({0})", currency);
					//propCount++;
				}
				if (!String.IsNullOrEmpty(emblem))
				{
					item.EmblemCost = Convert.ToInt32(emblem);
					//if (propCount > 0) sb.Append(",").AppendLine();
					//sb.AppendFormat("\t\t\t\t\tEmblemCost = {0}", emblem);
					//propCount++;
				}
				if (!String.IsNullOrEmpty(image))
				{
					item.ImageUri = image;
					//if (propCount > 0) sb.Append(",").AppendLine();
					//sb.AppendFormat("\t\t\t\t\tImageUri = \"{0}\"", image);
					//propCount++;
				}
				if (!String.IsNullOrEmpty(durability))
				{
					item.DurabilityMax = item.Durability = Convert.ToInt32(durability);
					//if (propCount > 0) sb.Append(",").AppendLine();
					//sb.AppendFormat("\t\t\t\t\tDurability = {0}", durability);
					//propCount++;
				}
				if (!String.IsNullOrEmpty(affects))
				{
					// Affects Format - <AffectName>=1 | Strength=1
					foreach (var affect in affects.Replace("\"", "").Split('|'))
					{
						string[] pairs = affect.Split('=');
						if (pairs != null && pairs.Length == 2)
						{
							item.Affects[(AttributeType)Enum.Parse(typeof(AttributeType), pairs[0], true)] = Convert.ToInt32(pairs[1]);
						}
					}
				}
				if (customProps.Count > 0)
				{
					foreach (var p in customProps)
					{
						item.Properties.SetValue(p.Name, p.Value);
						//if (propCount > 0) sb.Append(",").AppendLine();
						//sb.AppendFormat("\t\t\t\t\t{0} = {1}", item.Name, item.GetValue());
						//propCount++;
					}
				}

				provider.SaveActor<Item>(item);
			}
		}

		private static void ParseQuest(string[] parts, StringBuilder sb, WorldProvider provider)
		{
			if (parts != null && parts.Length > 0)
			{
				// Name,QuestType,RequiredSkill,RequiredSkillValue,MinLevel,MaxLevel,RequiredOrder,RewardCurrency,RewardEmblem,RewardXP,RewardSkillValue,
				// "RewardItems (templateName=quantity,(n))",ParentQuestName,
				// Description,BeingMsg,InProgMsg,CompleteMsg,ActorName,ActorType,Quantity,RecipientActorName
				// OnCompleteAutoStartName
				string name = parts[0];
				string type = parts[1];
                string reqSkill = parts[2];
                string reqSkillValue = parts[3];
				string minLvl = parts[4];
				string maxLvl = parts[5];
				string order = parts[6];
				string currency = parts[7];
				string emblem = parts[8];
				string xp = parts[9];
                string skill = parts[10];
				string rewardItemsStr = parts[11];
				string parent = parts[12];
				string desc = parts[13];
				string beginMsg = parts[14];
				string inProgMsg = parts[15];
				string compMsg = parts[16];
				string actorName = parts[17];
				string actorType = parts[18];
				string quantity = parts[19];
				string recipient = parts[20];
				string autoStart = parts[21];

				string formattedName = FormatName(name);
				if (!String.IsNullOrEmpty(rewardItemsStr)) rewardItemsStr = rewardItemsStr.Replace("\"", String.Empty).Trim();
				if (!String.IsNullOrEmpty(beginMsg)) beginMsg = beginMsg.Replace("\"", String.Empty).Trim();
				if (!String.IsNullOrEmpty(inProgMsg)) inProgMsg = inProgMsg.Replace("\"", String.Empty).Trim();
				if (!String.IsNullOrEmpty(compMsg)) compMsg = compMsg.Replace("\"", String.Empty).Trim();

				Quest quest = provider.GetTemplate(typeof(Quest), name) as Quest;
                //if (quest == null)
                //{
                //    switch (type)
                //    {	
                //        case "Discovery":
                //            quest = new DiscoveryQuest() { Name = name };
                //            break;
                //        case "Delivery":
                //            quest = new DeliveryQuest() { Name = name };
                //            break;
                //        case "Collection":
                //            quest = new CollectionQuest() { Name = name };
                //            break;
                //        case "Kill":
                //            quest = new KillQuest() { Name = name };
                //            break;
                //    }
                //}

				quest.Properties.IsTemplateCollection = true;
				quest.RewardItems.IsTemplateCollection = true;
				quest.TemplateID = 0;
				quest.Description = desc;
				quest.BeginMessage = beginMsg;
				quest.CompletedMessage = compMsg;
				quest.InProgressMessage = inProgMsg;

				quest.MaximumLevel = Convert.ToInt32(maxLvl);
				quest.MinimumLevel = Convert.ToInt32(minLvl);

				quest.RewardExperience = Convert.ToInt32(xp);

                quest.RequiredSkill = reqSkill;
                quest.RequiredSkillValue = Convert.ToInt32(reqSkillValue);
                quest.RewardSkillValue = Convert.ToInt32(skill);

				if (!String.IsNullOrEmpty(parent))
				{
					quest.Properties.SetValue(Quest.ParentQuestNameProperty, parent);
				}
				if (!String.IsNullOrEmpty(currency))
				{
					quest.Properties.SetValue(Quest.RewardCurrencyProperty, currency);
				}
				if (!String.IsNullOrEmpty(emblem))
				{
					quest.Properties.SetValue(Quest.RewardEmblemProperty, emblem);
				}
				quest.RequiredOrder = OrderType.None;
				if (!String.IsNullOrEmpty(order) && order != "None")
				{
					quest.Properties.SetValue(Quest.RequiredOrderProperty, order);
				}

				// Reward Items
				if (!String.IsNullOrEmpty(rewardItemsStr))
				{
					foreach (var rewardItem in TemplateItem.Parse(rewardItemsStr))
					{
						quest.RewardItems.Add(rewardItem);
					}
				}

				//// ActorName
				//if (!String.IsNullOrEmpty(actorName))
				//{
				//    quest.Properties.SetValue(CollectionQuest.ActorNameProperty, actorName);
				//}

				//// ActorType
				//if (!String.IsNullOrEmpty(actorType))
				//{
				//    quest.Properties.SetValue(DiscoveryQuest.ActorTypeProperty, actorType);
				//}

				//// Quantity
				//if (!String.IsNullOrEmpty(quantity))
				//{
				//    quest.Properties.SetValue(CollectionQuest.QuantityProperty, quantity);
				//}

				//// RecipientActorName
				//if (!String.IsNullOrEmpty(recipient))
				//{
				//    quest.Properties.SetValue(DeliveryQuest.RecipientActorNameProperty, recipient);
				//}

				if (!string.IsNullOrEmpty(autoStart))
				{
					quest.OnCompleteAutoStartQuestName = autoStart;
				}

				provider.SaveActor<Quest>(quest);
			}
		}

        private static Award _currentAward = null;
        private static void ParseAward(string[] parts, StringBuilder sb, WorldProvider provider)
        {
            // Name,Object Type,TaskType,TargetActorName,UseActorName,Quantity,IsQuantityAdditional,IsQuantitySequential,MaleTitle,FemaleTitle,ImageUri,Description
            if (parts != null && parts.Length > 0)
            {
                string name = parts[0];
                string objType = parts[1];
                string taskType = parts[2];
                string targetActorName = parts[3];
                string useActorName = parts[4];
                string quantity = parts[5];
                string isQtyAdd = parts[6];
                string isQtySeq = parts[7];
                string maleTitle = parts[8];
                string femaleTitle = parts[9];
                string imageUri = parts[10];
                string desc = parts[11];

                switch (objType)    
                {
                    case "Award":
                        if (_currentAward != null)
                        {
                            provider.SaveActor<Award>(_currentAward);
                            _currentAward = null;
                        }
                        if (_currentAward == null)
                        {
                            _currentAward = provider.GetTemplate(typeof(Award), name) as Award;
                            if (_currentAward == null) _currentAward = new Award() { Name = name };

                            _currentAward.Properties.IsTemplateCollection = true;
                            _currentAward.TemplateID = 0;

                            if (!String.IsNullOrEmpty(imageUri))
                            {
                                _currentAward.ImageUri = imageUri;
                            }

                            if (!String.IsNullOrEmpty(desc))
                            {
                                _currentAward.Description = desc;
                            }
                        }
                        break;
                    case "Task":
                        if (_currentAward != null)
                        {
                            Task task = new Task();
                            task.Name = name;
                            task.Type = (TaskType)Enum.Parse(typeof(TaskType), taskType, true);
                            task.TargetActorName = targetActorName;
                            task.UseActorName = useActorName;
                            task.Quantity = Convert.ToInt32(quantity);
                            task.IsQuantityAdditional = Convert.ToBoolean(isQtyAdd);
                            task.IsQuantitySequential = Convert.ToBoolean(isQtySeq);
                            task.MaleTitle = maleTitle;
                            task.FemaleTitle = femaleTitle;
                            task.Description = desc;
                            _currentAward.Tasks.Add(task);
                        }
                        break;
                }

            }
        }

		private static SkillGroup GetSkillGroup(string name)
		{
			switch (name)
			{
				case "Fighter": return new FighterSkillGroup();
				case "Caster": return new CasterSkillGroup();
				case "Thief": return new ThiefSkillGroup();
				case "Explorer": return new ExplorerSkillGroup();
				default: return new SkillGroup();
			}
		}

		private static string FormatName(string name)
		{
			return name.Replace(" ", "").Replace("'", "").Replace("-", "").Replace("_", "");
		}

		private static string FormatConstName(string prefix, string name)
		{
			if (!prefix.EndsWith("_") && !String.IsNullOrEmpty(prefix)) prefix = prefix + "_";

			return String.Concat(prefix, name.Replace(' ', '_')).ToUpper();
		}

		private static string GetFlags(string flags)
		{
			string[] parts = flags.Split(',');
			if (parts != null && parts.Length > 0)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < parts.Length; i++)
				{
					if (i > 0) sb.Append(" | ");
					sb.Append("ActorFlags.").Append(parts[i]);
				}
				return sb.ToString();
			}
			return String.Empty;
		}

		private static string GetPluralName(ItemType type)
		{
			switch (type)
			{
				case ItemType.Clothing: return "Clothes";
				default: return type.ToString() + "s";
			}
		}
	}


	public enum TemplateType
	{
		Creature,
		Item,
	}

	public enum ItemType
	{
		Item = 0,
		Armor = 1,
		Clothing = 2,
		Container = 3,
		Food = 4,
		Light = 5,
		Spell = 6,
		Weapon = 7,
		Shield = 8,
		Transport = 9,
		RepairKit = 10,
		Artifact = 11,
		Recipe = 12,
		TrainSkill = 13,
		Potion = 14,
		QuestItem = 15,
	}

	public class CustomPropertyCollection : List<CustomProperty>
	{
		public static CustomPropertyCollection Parse(string entries)
		{
			CustomPropertyCollection props = new CustomPropertyCollection();
			string[] pairs = entries.Replace("\"", "").Split('|');
			if (pairs != null && pairs.Length > 0)
			{
				foreach (var pair in pairs)
				{
					// Pair Format
					// Type=Name=Value
					string[] values = pair.Split('=');
					if (values != null && values.Length == 3)
					{
						props.Add(new CustomProperty { Type = values[0], Name = values[1], Value = values[2] });
					}
				}
			}
			return props;
		}
	}

	public class CustomProperty
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public object Value { get; set; }

		public string GetValue()
		{
			if (this.Type.Equals("string"))
			{
				return String.Concat('"', this.Value, '"');
			}
			return this.Value.ToString();
		}
	}

	public static class ItemGroups
	{
		private static Dictionary<ItemType, List<string>> _items = new Dictionary<ItemType, List<string>>();

		public static void AddLine(ItemType type, string line)
		{
			if (!_items.ContainsKey(type))
			{
				_items.Add(type, new List<string>());
			}

			_items[type].Add(line);
		}

		public static List<string> GetLines(ItemType type)
		{
			if (_items.ContainsKey(type))
			{
				return _items[type];
			}
			return new List<string>();
		}
	}

	public class ImportTypes
	{
		public const string Creature = "Creature";
		public const string Item = "Item";
		public const string Npc = "NPC";
		public const string Quest = "Quest";
        public const string Award = "Award";
	}
}
