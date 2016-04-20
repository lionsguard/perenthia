using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Radiance;
using Radiance.Markup;

namespace Perenthia.Models
{
	public class Avatar : Actor
	{
		#region Properties
		public Race Race { get; set; }

		public string NameLevel
		{
			get { return String.Format(SR.NameLevelFormat, this.Name, this.Level); }
		}

		public Gender Gender
		{
			get { return this.Properties.GetValue<Gender>(GenderProperty); }
			set { this.Properties.SetValue(GenderProperty, value); }
		}
		public static readonly string GenderProperty = "Gender";

        public string Zone
        {
            get { return this.Properties.GetValue<string>(ZoneProperty); }
            set { this.Properties.SetValue(ZoneProperty, value); }
        }
        public static readonly string ZoneProperty = "Zone";

		public int Strength
		{
			get { return this.Properties.GetValue<int>(StrengthProperty); }
			set { this.Properties.SetValue(StrengthProperty, value); }
		}
		public static readonly string StrengthProperty = "Attr_Strength";

		public int Dexterity
		{
			get { return this.Properties.GetValue<int>(DexterityProperty); }
			set { this.Properties.SetValue(DexterityProperty, value); }
		}
		public static readonly string DexterityProperty = "Attr_Dexterity";

		public int Stamina
		{
			get { return this.Properties.GetValue<int>(StaminaProperty); }
			set { this.Properties.SetValue(StaminaProperty, value); }
		}
		public static readonly string StaminaProperty = "Attr_Stamina";

		public int Beauty
		{
			get { return this.Properties.GetValue<int>(BeautyProperty); }
			set { this.Properties.SetValue(BeautyProperty, value); }
		}
		public static readonly string BeautyProperty = "Attr_Beauty";

		public int Intelligence
		{
			get { return this.Properties.GetValue<int>(IntelligenceProperty); }
			set { this.Properties.SetValue(IntelligenceProperty, value); }
		}
		public static readonly string IntelligenceProperty = "Attr_Intelligence";

		public int Perception
		{
			get { return this.Properties.GetValue<int>(PerceptionProperty); }
			set { this.Properties.SetValue(PerceptionProperty, value); }
		}
		public static readonly string PerceptionProperty = "Attr_Perception";

		public int Endurance
		{
			get { return this.Properties.GetValue<int>(EnduranceProperty); }
			set { this.Properties.SetValue(EnduranceProperty, value); }
		}
		public static readonly string EnduranceProperty = "Attr_Endurance";

		public int Affinity
		{
			get { return this.Properties.GetValue<int>(AffinityProperty); }
			set { this.Properties.SetValue(AffinityProperty, value); }
		}
		public static readonly string AffinityProperty = "Attr_Affinity";

		public int Body
		{
			get { return this.Properties.GetValue<int>(BodyProperty); }
			set { this.Properties.SetValue(BodyProperty, value); }
		}
		public static readonly string BodyProperty = "Body";

		public int BodyMax
		{
			get { return this.Properties.GetValue<int>(BodyMaxProperty); }
			set { this.Properties.SetValue(BodyMaxProperty, value); }
		}
		public static readonly string BodyMaxProperty = "BodyMax";

		public int Mind
		{
			get { return this.Properties.GetValue<int>(MindProperty); }
			set { this.Properties.SetValue(MindProperty, value); }
		}
		public static readonly string MindProperty = "Mind";

		public int MindMax
		{
			get { return this.Properties.GetValue<int>(MindMaxProperty); }
			set { this.Properties.SetValue(MindMaxProperty, value); }
		}
		public static readonly string MindMaxProperty = "MindMax";

		public string BodyValueMax
		{
			get { return String.Format(SR.BodyValueMaxFormat, this.Body, this.BodyMax); }
		}

		public string MindValueMax
		{
			get { return String.Format(SR.MindValueMaxFormat, this.Mind, this.MindMax); }
		}

		public Currency Currency
		{
			get { return new Currency(this.Properties.GetValue<int>("Currency")); }
		}

		public int Emblem
		{
			get { return this.Properties.GetValue<int>("Emblem"); }
		}

		public int Level
		{
			get { return this.Properties.GetValue<int>(LevelProperty); }
			set { this.Properties.SetValue(LevelProperty, value); }
		}
		public static readonly string LevelProperty = "Level";

		public int Experience
		{
			get { return this.Properties.GetValue<int>(ExperienceProperty); }
			set { this.Properties.SetValue(ExperienceProperty, value); }
		}
		public static readonly string ExperienceProperty = "Experience";

		public int ExperienceMax
		{
			get { return this.Properties.GetValue<int>(ExperienceMaxProperty); }
			set { this.Properties.SetValue(ExperienceMaxProperty, value); }
		}
		public static readonly string ExperienceMaxProperty = "ExperienceMax";

		public int X
		{
			get { return this.Properties.GetValue<int>(XProperty); }
			set { this.Properties.SetValue(XProperty, value); }
		}
		public static readonly string XProperty = "X";

		public int Y
		{
			get { return this.Properties.GetValue<int>(YProperty); }
			set { this.Properties.SetValue(YProperty, value); }
		}
		public static readonly string YProperty = "Y";

		public int Z
		{
			get { return this.Properties.GetValue<int>(ZProperty); }
			set { this.Properties.SetValue(ZProperty, value); }
		}
		public static readonly string ZProperty = "Z";

		public int SkillPoints
		{
			get { return this.Properties.GetValue<int>(SkillPointsProperty); }
			set { this.Properties.SetValue(SkillPointsProperty, value); }
		}
		public static readonly string SkillPointsProperty = "SkillPoints";

		public int LastTellAvatarID
		{	
			get { return this.Properties.GetValue<int>(LastTellAvatarIDProperty); }
			set { this.Properties.SetValue(LastTellAvatarIDProperty, value); }
		}
		public static readonly string LastTellAvatarIDProperty = "LastTellAvatarID";

		/// <summary>
		/// Gets or sets the HouseholdName property value using the underlying Properties collection.
		/// </summary>
		public string HouseholdName
		{
			get { return this.Properties.GetValue<string>(HouseholdNamePropertyName); }
			set { this.Properties.SetValue(HouseholdNamePropertyName, value); }
		}
		/// <summary>
		/// Gets the name of the HouseholdName property as stored in the Properties collection.
		/// </summary>
		public const string HouseholdNamePropertyName = "HouseholdName";

		/// <summary>
		/// Gets or sets the HouseholdImageUri property value using the underlying Properties collection.
		/// </summary>
		public string HouseholdImageUri
		{
			get { return this.Properties.GetValue<string>(HouseholdImageUriPropertyName); }
			set { this.Properties.SetValue(HouseholdImageUriPropertyName, value); }
		}
		/// <summary>
		/// Gets the name of the HouseholdImageUri property as stored in the Properties collection.
		/// </summary>
		public const string HouseholdImageUriPropertyName = "HouseholdImageUri";

		/// <summary>
		/// Gets or sets the RankName property value using the underlying Properties collection.
		/// </summary>
		public string RankName
		{
			get { return this.Properties.GetValue<string>(RankNamePropertyName); }
			set { this.Properties.SetValue(RankNamePropertyName, value); }
		}
		/// <summary>
		/// Gets the name of the RankName property as stored in the Properties collection.
		/// </summary>
		public const string RankNamePropertyName = "RankName";

		/// <summary>
		/// Gets or sets the RankImageUri property value using the underlying Properties collection.
		/// </summary>
		public string RankImageUri
		{	
			get { return this.Properties.GetValue<string>(RankImageUriPropertyName); }
			set { this.Properties.SetValue(RankImageUriPropertyName, value); }
		}
		/// <summary>
		/// Gets the name of the RankImageUri property as stored in the Properties collection.
		/// </summary>
		public const string RankImageUriPropertyName = "RankImageUri";

		/// <summary>
		/// Gets or sets the RankOrder property value using the underlying Properties collection.
		/// </summary>
		public int RankOrder
		{
			get { return this.Properties.GetValue<int>(RankOrderPropertyName); }
			set { this.Properties.SetValue(RankOrderPropertyName, value); }	
		}
		/// <summary>
		/// Gets the name of the RankOrder property as stored in the Properties collection.
		/// </summary>
		public const string RankOrderPropertyName = "RankOrder";

		public ImageSource HouseholdImageSource
		{
			get { return ImageManager.GetImageSource(this.HouseholdImageUri); }
		}

		public ImageSource RankImageSource
		{
			get { return ImageManager.GetImageSource(this.RankImageUri); }
		}

		public bool IsMerchant
		{
			get
			{
				var mobileTypeString = this.Properties.GetValue<string>("MobileType");
				if (!String.IsNullOrEmpty(mobileTypeString))
				{
					MobileTypes type = (MobileTypes)Enum.Parse(typeof(MobileTypes), mobileTypeString, true);
					if (((type & MobileTypes.Banker) == MobileTypes.Banker)
					   || ((type & MobileTypes.Merchant) == MobileTypes.Merchant)
					   || ((type & MobileTypes.Innkeeper) == MobileTypes.Innkeeper)
					   || ((type & MobileTypes.Priest) == MobileTypes.Priest)
					   || ((type & MobileTypes.Trainer) == MobileTypes.Trainer))
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsQuestGiver
		{
			get
			{
				var mobileTypeString = this.Properties.GetValue<string>("MobileType");
				if (!String.IsNullOrEmpty(mobileTypeString))
				{
					MobileTypes type = (MobileTypes)Enum.Parse(typeof(MobileTypes), mobileTypeString, true);
					if ((type & MobileTypes.QuestGiver) == MobileTypes.QuestGiver)
					{
						return true;
					}
				}
				return false;
			}
		}

		public Dictionary<string, Skill> Skills { get; private set; }	
		public ActorCollection Inventory { get; private set; }
		#endregion

		public Avatar()
			: base()
		{
			Init();
		}

		public Avatar(RdlActor actor)
			: base(actor)
		{
			Init();

			string gender = actor.Properties.GetValue<string>("Gender");
			if (!String.IsNullOrEmpty(gender))
			{
				this.Gender = (Gender)Enum.Parse(typeof(Gender), gender, true);
			}
			string race = actor.Properties.GetValue<string>("Race");
			if (!String.IsNullOrEmpty(race) && Game.Races.ContainsKey(race))
			{
				this.Race = Game.Races[race];
			}

			var skills = actor.Properties.Where(p => p.Name.StartsWith("Skill_")).ToList();
			foreach (var skill in skills)
			{
				this.Skills.Add(skill.Name, new Skill { Name = skill.Name, Value = (int)skill.GetValue<double>() });
			}
			Game.EnsureSkillDetails(this.Skills.Values);
		}

		private void Init()
		{
			this.Gender = Gender.Male;
			this.Race = new Race();
			this.Skills = new Dictionary<string, Skill>(StringComparer.InvariantCultureIgnoreCase);
			this.Inventory = new ActorCollection(this);
		}

		public bool HasItem(int itemId)
		{
			return this.Inventory.Where(i => i.ID == itemId).Count() > 0;
		}

		public void UpdateSkill(string name, int value)
		{
			if (this.Skills.ContainsKey(name))
			{
				this.Skills[name].Value = value;
			}
			else
			{
				this.Skills.Add(name, new Skill { Name = name, Value = value });
			}
			Game.EnsureSkillDetails(this.Skills.Values);
		}
	}
}
