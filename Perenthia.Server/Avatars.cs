using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia
{
	#region PerenthiaAvatar
	public abstract class PerenthiaAvatar : Avatar
	{
		private Currency _currency = null;
		public Currency Currency
		{
			get
			{
				if (_currency == null || _currency.Value == 0)
				{
					_currency = new Currency(this.Properties.GetValue<int>(CurrencyProperty));
					_currency.Changed += new CurrencyValueChangedEventHandler(OnCurrencyChanged);
				}
				return _currency;
			}
			set
			{
				if (_currency == null)
				{
					_currency = new Currency();
					_currency.Changed += new CurrencyValueChangedEventHandler(OnCurrencyChanged);
				}
				_currency.Value = value.Value;
			}
		}
		public static readonly string CurrencyProperty = "Currency";

		public int Level
		{
			get { return this.Properties.GetValue<int>(LevelProperty); }
			set { this.Properties.SetValue(LevelProperty, value); }
		}
		public static readonly string LevelProperty = "Level";

		/// <summary>
		/// Gets or sets the range, in tiles, in which the avatar can see objects around them.
		/// </summary>
		public int SightRange
		{
			get { return this.Properties.GetValue<int>(SightRangeProperty); }
			set { this.Properties.SetValue(SightRangeProperty, value); }	
		}
		public static readonly string SightRangeProperty = "SightRange";

		protected PerenthiaAvatar()
		{
		}

		public override void OnApplyProtection(IAvatar attacker, ref int damage)
		{
			int protection = this.GetTotalProtection();
			if (protection > 0)
			{
				int startDamage = damage;
				// Protection should be a form of decreasing damage but not to the point of removing it completely.
				// If protection is greater than or equal to the damage then cut the damage in half.
				if (protection >= damage)
				{
					damage = (int)(damage * 0.5);
					if (damage <= 0) damage = 1;
				}

				int attackerLevel = attacker.Properties.GetValue<int>(PerenthiaAvatar.LevelProperty);
				if (attackerLevel > this.Level)
				{
					// If the attacker is higher level then the defender then add a point of damage back
					// for each level.
					int addDamage = attackerLevel = this.Level;
					if ((damage + addDamage) > startDamage) damage = startDamage;
					else damage += addDamage;
				}
			}
		}

		private void OnCurrencyChanged(CurrencyValueChangedEventArgs e)
		{
			this.Properties.SetValue(CurrencyProperty, e.Value);
		}

		public override string GetOffensiveSkill()
		{
			var weapon = (from c in this.GetAllChildren() 
						   where (c as Weapon) != null
						   && (c as Weapon).IsEquipped
						   select c as Weapon).FirstOrDefault();
			if (weapon != null)
			{
				return weapon.Skill;
			}
			return "Hand To Hand";
		}

		public override string GetDefensiveSkill()
		{
			// Check for an equipped shield.
			var shield = this.GetAllChildren().Where(c => c is Shield && (c as Shield).IsEquipped).FirstOrDefault();
			if (shield != null)
			{
				return "Block";
			}

			// Check for an equipped weapon.
			var weapon = this.GetAllChildren().Where(c => c is Weapon && (c as Weapon).IsEquipped).FirstOrDefault();
			if (weapon != null)
			{
				return "Parry";
			}

			return "Dodge";
		}

		public override IWeapon GetWeapon()
		{
			IWeapon weapon = base.GetWeapon();
			if (weapon == null)
			{
				weapon = this.World.CreateFromTemplate<Weapon>("Fists");
			}
			return weapon;
		}

        public override IArmor GetArmor()
        {
            IArmor armor = base.GetArmor();
            if (armor == null)
            {
                armor = this.World.CreateFromTemplate<Armor>("Natural Armor");
            }
            return armor;
        }

		#region Commands
		public void Attack(IAvatar target, IMessageContext context)
		{
			this.SetTarget(target);

			IWeapon weapon = this.GetWeapon();

			(weapon as Item).Attack(this);
		}
		#endregion

	}
	#endregion

	#region PerenthiaMobile
	public abstract class PerenthiaMobile : PerenthiaAvatar, IMobile
	{
		public int AttributeMinimum
		{
			get { return this.Properties.GetValue<int>(AttributeMinimumProperty); }
			set { this.Properties.SetValue(AttributeMinimumProperty, value, true); }
		}
		public static readonly string AttributeMinimumProperty = "AttributeMinimum";

		public int AttributeMaximum
		{
			get { return this.Properties.GetValue<int>(AttributeMaximumProperty); }
			set { this.Properties.SetValue(AttributeMaximumProperty, value, true); }
		}
		public static readonly string AttributeMaximumProperty = "AttributeMaximum";

		public DateTime LastAttackTime
		{
			get { return this.Properties.GetValue<DateTime>(LastAttackTimeProperty); }
			set { this.Properties.SetValue(LastAttackTimeProperty, value); }	
		}
		public static readonly string LastAttackTimeProperty = "LastAttackTime";

		public TimeSpan AttackDelay
		{	
			get { return new TimeSpan(this.Properties.GetValue<long>(AttackDelayProperty)); }
			set { this.Properties.SetValue(AttackDelayProperty, value.Ticks, true); }
		}
		public static readonly string AttackDelayProperty = "AttackDelay";

		public bool CanAttack
		{
			get { return this.Properties.GetValue<bool>(CanAttackProperty); }
			set { this.Properties.SetValue(CanAttackProperty, value, true); }	
		}
		public static readonly string CanAttackProperty = "CanAttack";

		public override string ImageUri
		{	
			get { return this.Properties.GetValue<string>(ImageUriProperty); }
			set { this.Properties.SetValue(ImageUriProperty, value, true); }
		}

		/// <summary>
		/// Gets or sets the gender of the avatar.
		/// </summary>
		public override Gender Gender
		{
			get { return this.Properties.GetValue<Gender>(GenderProperty); }
			set { this.Properties.SetValue(GenderProperty, value, true); }
		}

		/// <summary>
		/// Gets or sets the race of the avatar.
		/// </summary>
		public override string Race
		{
			get { return this.Properties.GetValue<string>(RaceProperty); }
			set { this.Properties.SetValue(RaceProperty, value, true); }
		}

		public MobileKilledByCollection KilledBy { get; private set; }
		protected DroppableItemCollection DroppableItems { get; private set; }

		protected Dictionary<int, PerenthiaMobile> Instances { get; private set; }

		public bool IsInstance { get; private set; }
		public PerenthiaMobile InstanceOwner { get; private set; }	

		public bool IsQuestsLoaded { get; set; }	
		private object _lock = new object();

		protected PerenthiaMobile()
		{
			this.KilledBy = new MobileKilledByCollection(this);
			this.DroppableItems = new DroppableItemCollection(this);
			this.Instances = new Dictionary<int, PerenthiaMobile>();
			this.Level = 1; 
			this.ObjectType = ObjectType.Mobile;
			this.MobileType = MobileTypes.None;
			this.RespawnDelay = TimeSpan.FromMinutes(2);
			this.RespawnTime = DateTime.Now.AddMinutes(-10);
			this.AttackDelay = TimeSpan.FromSeconds(15);
		}

		protected override void AddSimpleRdlProperties(List<RdlObject> list)
		{
			base.AddSimpleRdlProperties(list);
			list.AddRange(this.GetRdlProperties(MobileTypeProperty, LevelProperty, CanAttackProperty, IsHostileProperty,ImageUriProperty));
		}

		public PerenthiaMobile GetOrCreateInstance(int playerId)
		{
			if (!this.Instances.ContainsKey(playerId))
			{
				var copy = this.Copy();
				copy.IsInstance = true;
				copy.InstanceOwner = this;
				this.Instances.Add(playerId, copy);
			}
			return this.Instances[playerId];
		}

		public PerenthiaMobile Copy()
		{
			// Clone the mobile and set it to the property of the Target.
			PerenthiaMobile mob = this.Clone() as PerenthiaMobile;
			// Set the cloned ID to be the same ID.
			mob.ID = this.ID;
			mob.Owner = this.Owner;
			mob.GenerateStats();
			return mob;
		}

		public bool CanAutoAttack()
		{
			if (this.Target != null)
			{
				return DateTime.Now.Subtract(this.LastAttackTime) > this.AttackDelay;
			}
			return false;
		}

		public void AddDroppableItem(string itemName)
		{
			this.DroppableItems.Add(itemName, itemName);
		}
		public IEnumerable<Item> GenerateRandomDropItems()
		{
			// Random item generation.
			//if (this.Children.Count == 0)
			//{
			//    // Generate a random item. Craft items should be sold in the market...
			//    IItem item = ItemGenerator.GetItem(this.Level);
			//    if (item != null)
			//    {
			//        this.Children.Add(item);
			//    }
			//}

			// Going to use random item generator instead.
			if (this.DroppableItems.Count > 0)
			{
				int max = Dice.Random(1, this.DroppableItems.Count);
				int current = 0;
				for (int i = 0; i < this.DroppableItems.Count; i++)
				{
					int index = Dice.Random(0, this.DroppableItems.Count - 1);
					Item item = this.World.CreateFromTemplate<Item>(this.DroppableItems[this.DroppableItems.Keys.ElementAt(index)]);
					this.Children.Add(item);
					item.Save();
					current++;
					if (current == max) break;
				}
			}
			return this.Children.Where(c => c is Item).Select(c => c as Item);
		}

		public void GenerateRandomCurrency()
		{
			// TODO: Figure out better currency generation for mobiles.
			int value = 2;
			if (this.Level < 5) value = Dice.Roll(this.Level, 10);
			else if (this.Level >= 5 && this.Level < 10) value = Dice.Roll(this.Level, 20);
			else value = Dice.Roll(this.Level, 100);
			if (value < 2) value = 2;
			this.Currency.Value = value;
		}

		public override void OnPropertyChanged(string name)
		{
			base.OnPropertyChanged(name);

			if (name.Equals("World"))
			{
				if (this.World != null)
				{
					this.GenerateStats();
				}
			}
		}
		internal void GenerateStats()
		{
			if (this.World != null)
			{
				foreach (var item in this.Attributes)
				{
					this.Attributes[item.Key] = Dice.Random(this.AttributeMinimum, this.AttributeMaximum);
				}
				AttributeList.SetBodyAndMind(this, this.World);

				// Foreach level increase body and mind.
				for (int i = 2; i < this.Level; i++)
				{
					LevelManager.IncreaseBodyAndMind(this, i);
				}

				// Foreach skill level above 0, append the character level.
				foreach (var skill in this.Skills)
				{
					if (skill.Value > 0)
					{
						this.Skills[skill.Key] = this.Level;
					}
				}
				if (!this.Skills.ContainsKey("Hand to Hand"))
				{
					this.Skills.Add("Hand to Hand", 0);
				}
				this.Skills["Hand to Hand"] = this.Level * 2;

				// Protection
				this.Protection = this.Level + Dice.Roll(this.Level, 10);
			}
		}

		public override IEnumerable<IQuest> GetActiveQuests()
		{
			if (!this.IsQuestsLoaded)
			{
				lock (_lock)
				{
					if (!this.IsQuestsLoaded)
					{
						// Get Quests where this mobile is either in the startswith or endswith collections.
						var quests = this.World.Provider.GetQuests(this);
						foreach (var quest in quests)
						{
							this.Children.Add(quest);
						}
						this.IsQuestsLoaded = true;
					}
				}
			}
			return base.GetActiveQuests();
		}

		public override void OnAttack(ref int outcomeSuccessDice)
		{
			if (outcomeSuccessDice <= 0)
			{
				if (Dice.Roll(1, 10) != 1)
				{
					outcomeSuccessDice = 1;
				}
			}
		}

		public override void OnApplyProtection(IAvatar attacker, ref int damage)
		{
			base.OnApplyProtection(attacker, ref damage);
			if (damage <= 0) damage = 1;
		}

		public override void OnBuff(IActor sender, IMessageContext context, Foci foci)
		{
		}

		public override void OnDamage(IActor sender, IMessageContext context, Foci foci)
		{
		}

		public override void OnEnchant(IActor sender, IMessageContext context, Foci foci)
		{
		}

		public override void OnHeal(IActor sender, IMessageContext context, Foci foci)
		{
		}

		#region IMobile Members

		public MobileTypes MobileType
		{
			get { return this.Properties.GetValue<MobileTypes>(MobileTypeProperty); }
			set { this.Properties.SetValue(MobileTypeProperty, value, true); }
		}
		public static readonly string MobileTypeProperty = "MobileType";

		public TimeSpan RespawnDelay
		{
			get { return new TimeSpan(this.Properties.GetValue<long>(RespawnDelayProperty)); }
			set { this.Properties.SetValue(RespawnDelayProperty, value.Ticks); }
		}
		public static readonly string RespawnDelayProperty = "RespawnDelay";

		public DateTime RespawnTime
		{
			get { return this.Properties.GetValue<DateTime>(RespawnTimeProperty); }
			set { this.Properties.SetValue(RespawnTimeProperty, value); }
		}
		public static readonly string RespawnTimeProperty = "RespawnTime";

		public bool IsHostile
		{
			get { return this.Properties.GetValue<bool>(IsHostileProperty); }
			set { this.Properties.SetValue(IsHostileProperty, value, true); }
		}
		public static readonly string IsHostileProperty = "IsHostile";
		#endregion
	}

	public class MobileKilledByCollection : ActorOwnedDictionaryBase<DateTime>
	{
		public MobileKilledByCollection(IActor owner)
			: base(owner, "KilledBy_")
		{
		}

		public void AddKilledBy(int id, DateTime value)
		{
			string key = this.GetPrefixedName(id.ToString());
			if (this.ContainsKey(key))
			{
				this[key] = value;
			}
			else
			{
				this.Add(key, value);
			}
		}

		public bool WasKilledBy(int id, DateTime currentTime)
		{
			string key = this.GetPrefixedName(id.ToString());
			if (this.ContainsKey(key))
			{
				PerenthiaMobile mobile = this.Owner as PerenthiaMobile;
				if (mobile != null)
				{
					DateTime lastKilledTime = this[key];
					TimeSpan difference = currentTime.Subtract(lastKilledTime);
					if (difference.TotalSeconds < mobile.RespawnDelay.TotalSeconds)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override bool TryGetValue(string key, out DateTime value)
		{
			if (this.ContainsKey(key))
			{
				value = this[key];
				return true;
			}
			value = DateTime.MinValue;
			return false;
		}
	}

	public class DroppableItemCollection : ActorOwnedDictionaryBase<string>
	{
		public DroppableItemCollection(IActor owner)
			: base(owner, "DropItem_")
		{
		}

		public override bool TryGetValue(string key, out string value)
		{
			if (this.ContainsKey(this.GetPrefixedName(key)))
			{
				value = this[this.GetPrefixedName(key)];
				return true;
			}
			value = String.Empty;
			return false;
		}
	}
	#endregion

	#region Creature
	public class Creature : PerenthiaMobile
	{
		public int Power
		{
			get { return this.Properties.GetValue<int>(PowerProperty); }
			set { this.Properties.SetValue(PowerProperty, value); }
		}
		public static readonly string PowerProperty = "Power";

		public int Range
		{
			get { return this.Properties.GetValue<int>(RangeProperty); }
			set { this.Properties.SetValue(RangeProperty, value); }
		}
		public static readonly string RangeProperty = "Range";	

		public Creature()
			: base()
		{
			this.Init();
		}

		public Creature(string name, string description, int level, int attributeMin, int attributeMax, MobileTypes type, SkillGroup skills)
		{
			this.Name = name;
			this.Description = description;
			this.Level = level;
			this.AttributeMinimum = attributeMin;
			this.AttributeMaximum = attributeMax;
			this.MobileType = type;
			this.Skills.AddRange(skills.Skills.ToArray());
			this.Init();
		}

		private void Init()
		{
			this.IsHostile = true;
			this.CanAttack = true;
			this.Skills["Hand To Hand"] = 10;
			this.Power = 1;
			this.Range = 1;
		}

		public override string GetDefensiveSkill()
		{
			return "Dodge";
		}

        public override int GetTotalProtection()
        {
            int protection = base.GetTotalProtection();
            if (protection <= 1)
            {
                protection = this.GetArmor().Protection;
            }
            return protection;
        }

		public override IWeapon GetWeapon()
		{
			IWeapon weapon = base.GetWeapon();
            if (weapon == null)
            {
                weapon = this.World.CreateFromTemplate<Weapon>("Fists");
                if (weapon != null)
                {
                    weapon.Power = this.Level;
                }
            }
			return weapon;
		}

        public override IArmor GetArmor()
        {
            IArmor armor = base.GetArmor();
            if (armor == null)
            {
                armor = this.World.CreateFromTemplate<Armor>("Creature Armor");
                if (armor != null)
                {
                    armor.Protection = this.Level;
                }
            }
            return armor;
        }

		public override void Heartbeat()
		{
			if (IsDead || IsUnconscious || IsStunned || IsPoisoned || IsSnared)
				return;

			if (Target != null)
			{
				var avatar = this.Target as Avatar;
				if (avatar == null)
					return;

				var now = DateTime.Now;
				if (now.Subtract(this.LastAttackTime).TotalSeconds > 20)
				{
					// Attack the target.
					this.Attack(avatar, avatar.Context);
				}
			}
			else
			{
				// Heal
				if (this.Body < this.BodyMax)
					this.SetBody(this.Body + this.Attributes.Stamina);
				if (this.Mind < this.MindMax)
					this.SetMind(this.Mind + this.Attributes.Endurance);

				// If a player is sitting around with no target, then attack them.
				var players = this.Place.GetAllChildren().Where(c => c is IPlayer && (c as IPlayer).Target == null);
				foreach (var player in players)
				{
					PerenthiaMobile mob = this;
					if (this.IsInstance)
						mob = this.InstanceOwner.GetOrCreateInstance(player.ID);

					mob.Attack((IAvatar)player, ((IAvatar)player).Context);
				}
			}
		}
	}
	#endregion

	#region Npc
	public class Npc : PerenthiaMobile
	{
		protected static readonly string ResponseNotFoundMessage = "I do not understand what you are talking about, could you re-phrase the question?";

		public double MarkdownPercentage
		{
			get { return this.Properties.GetValue<double>(MarkdownPercentageProperty); }
			set { this.Properties.SetValue(MarkdownPercentageProperty, value, true); }
		}
		public static readonly string MarkdownPercentageProperty = "MarkdownPercentage";

		public double MarkupPercentage
		{
			get { return this.Properties.GetValue<double>(MarkupPercentageProperty); }
			set { this.Properties.SetValue(MarkupPercentageProperty, value, true); }
		}
		public static readonly string MarkupPercentageProperty = "MarkupPercentage";

		public string OnEnterMessage
		{	
			get { return this.Properties.GetValue<string>(OnEnterMessageProperty); }
			set { this.Properties.SetValue(OnEnterMessageProperty, value, true); }
		}
		public static readonly string OnEnterMessageProperty = "OnEnterMessage";

		public Npc()		
			: base()
		{
			this.Init();
			this.Context = new NpcMessageContext();
			(this.Context as NpcMessageContext).MessageReceived += new NpcMessageReceivedEventHandler(OnMessageReceived);
			this.HasProperName = true;
		}

		public Npc(string name, string description, int level, int attributeMin, int attributeMax, MobileTypes type, SkillGroup skills)
			: this()
		{
			this.Name = name;
			this.Description = description;
			this.Level = level;
			this.AttributeMinimum = attributeMin;
			this.AttributeMaximum = attributeMax;
			this.MobileType = type;
			this.Skills.AddRange(skills.Skills.ToArray());
		}

		protected virtual void Init()
		{
		}

		protected virtual void OnMessageReceived(NpcMessageReceivedEventArgs e)
		{
			if (e.Message is RdlTellMessage || e.Message is RdlChatMessage)
			{
				IPlayer from = this.World.GetActor(e.Message.GetArg<string>(0), null) as IPlayer;
				if (from != null)
				{
					string response;
					if (this.FindResponse(e.Message.Text, out response))
					{
						from.Context.Add(new RdlChatMessage(this.Name, String.Format(Resources.MsgChat, this.Name, response)));
					}
					else
					{
						from.Context.Add(new RdlChatMessage(this.Name, String.Format(Resources.MsgChat, this.Name, ResponseNotFoundMessage)));
					}
				}
			}
		}

		protected override void AddSimpleRdlProperties(List<RdlObject> list)
		{
			base.AddSimpleRdlProperties(list);

			list.AddRange(this.GetRdlProperties(
				MarkupPercentageProperty, 
				MarkdownPercentageProperty));
		}

		public override void OnEnter(IActor sender, IMessageContext context, Direction direction)
		{
			if (!String.IsNullOrEmpty(this.OnEnterMessage))
			{
				context.Add(new RdlChatMessage(this.Name, String.Concat(this.Name, " says: ", this.OnEnterMessage)));
			}
		}

		protected bool FindResponse(string text, out string response)
		{
			if (!String.IsNullOrEmpty(text))
			{
				// Break the text into words.
				string[] words = text.Split(' ');
				if (words != null && words.Length > 0)
				{
					// Property:
					// Keyword_Travel = Response_Angarath
					// Keyword_Coach = Response_Angarath
					// Response_Angarath = This coach travels to the city of Angarath and back.

					// Search the properties collection for keywords, search within the keywords for the words
					// of the supplied text.
					var keywords = (from p in this.Properties
									where p.Key.StartsWith("Keyword_")
									&& words.Contains(p.Key.Replace("Keyword_", ""))
									select p.Value.GetValue<string>()).ToList();

					// If keywords found then search for a response.
					if (keywords.Count > 0)
					{
						// Just get the first matching response.
						response = (from p in this.Properties
									where p.Key.Equals(keywords[0])
									select p.Value.GetValue<string>()).FirstOrDefault();
						if (!String.IsNullOrEmpty(response))
						{
							return true;
						}
					}
				}
			}
			response = ResponseNotFoundMessage;
			return false;
		}
	}
	#endregion

	#region Character
	public class Character : PerenthiaAvatar, IPlayer
	{
		#region IPlayer Members

        public string UserName
		{
			get { return this.Properties.GetValue<string>(UserNameProperty); }
			set { this.Properties.SetValue(UserNameProperty, value); }
		}
		public static readonly string UserNameProperty = "UserName";

		public int SkillPoints
		{
			get { return this.Properties.GetValue<int>(SkillPointsProperty); }
			set { this.Properties.SetValue(SkillPointsProperty, value); }
		}
		public static readonly string SkillPointsProperty = "SkillPoints";

        public IClient Client { get; set; }

        /// <summary>
        /// Gets or sets information for the current player's household, rank and title.
        /// </summary>
        public HouseholdInfo Household { get; set; }

		/// <summary>
		/// Gets an AuthKey instance for the current IPlayer.
		/// </summary>
		/// <returns>An AuthKey instance for the current IPlayer.</returns>
		public AuthKey GetAuthKey(Guid sessionId)
		{
			return new AuthKey(sessionId, this.UserName, this.ID);
		}

        public event ActorEventHandler<IAvatar> Died = delegate { };
        public event ActorEventHandler<IActor> KilledActor = delegate { };
        public event ActorEventHandler<IItem> ItemReceived = delegate { };
        public event ActorEventHandler<IItem> ItemDropped = delegate { };
        public event ActorEventHandler<IPlayer> PlaceEntered = delegate { };
        public event ActorEventHandler<IPlayer> PlaceExited = delegate { };

        public void OnDied(IAvatar killer)
        {
            this.TotalKillsInARow = 0;
            this.Died(new ActorEventArgs<IAvatar>(this));
        }

        public void OnKilledActor(IActor actor)
        {
            this.TotalKills++;
            this.TotalKillsInARow++;

			// Check for awards.
            AwardManager.IssueAwardIfAble(this);

            this.KilledActor(new ActorEventArgs<IActor>(actor));
        }

        public void OnItemReceived(IItem item)
        {
            this.ItemReceived(new ActorEventArgs<IItem>(item));
        }

        public void OnItemDropped(IItem item)
        {
            this.ItemDropped(new ActorEventArgs<IItem>(item));
        }

        public void OnPlaceEntered()
        {
            this.PlaceEntered(new ActorEventArgs<IPlayer>(this));
        }

        public void OnPlaceExited()
        {
            this.PlaceExited(new ActorEventArgs<IPlayer>(this));
        }


		#endregion

		public static readonly string ActionPrefix = "Action_";

        public int TotalKills
        {
            get { return this.Properties.GetValue<int>(TotalKillsProperty); }
            set { this.Properties.SetValue(TotalKillsProperty, value); }
        }
        public static readonly string TotalKillsProperty = "TotalKills";

        public int TotalDiscoveries
        {
            get { return this.Properties.GetValue<int>(TotalDiscoveriesProperty); }
            set { this.Properties.SetValue(TotalDiscoveriesProperty, value); }
        }
        public static readonly string TotalDiscoveriesProperty = "TotalDiscoveries";

        public int TotalKillsInARow
        {
            get { return this.Properties.GetValue<int>(TotalKillsInARowProperty); }
            set { this.Properties.SetValue(TotalKillsInARowProperty, value); }
        }
        public static readonly string TotalKillsInARowProperty = "TotalKillsInARow";

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

		public int TotalExperience
		{	
			get { return this.Properties.GetValue<int>(TotalExperienceProperty); }
			set { this.Properties.SetValue(TotalExperienceProperty, value); }
		}
		public static readonly string TotalExperienceProperty = "TotalExperience";

		public DateTime DateCreated
		{
			get { return this.Properties.GetValue<DateTime>(DateCreatedProperty); }
			set { this.Properties.SetValue(DateCreatedProperty, value); }
		}
		public static readonly string DateCreatedProperty = "DateCreated";

		public override string ImageUri
		{
			get { return String.Format(Resources.ImgAvatar, this.Race, this.Gender).ToLower(); }
			set { }
		}

		public bool IsPvpEnabled
		{
			get { return this.Properties.GetValue<bool>(IsPvpEnabledProperty); }
			set { this.Properties.SetValue(IsPvpEnabledProperty, value, false); }
		}
		public static readonly string IsPvpEnabledProperty = "IsPvpEnabled";

        public string Zone
        {
            get { return this.Properties.GetValue<string>(ZoneProperty); }
            set { this.Properties.SetValue(ZoneProperty, value, false); }
        }
        public static readonly string ZoneProperty = "Zone";

        /// <summary>
        /// Gets or sets the TransportID property value using the underlying Properties collection.
        /// </summary>
        public int TransportID
        {
            get { return this.Properties.GetValue<int>(TransportIDPropertyName); }
            set { this.Properties.SetValue(TransportIDPropertyName, value); }
        }
        /// <summary>
        /// Gets the name of the TransportID property as stored in the Properties collection.
        /// </summary>
        public const string TransportIDPropertyName = "TransportID";

		public bool IsAdmin { get; set; }	

		public Spellbook Spells { get; private set; }
		public EquipmentCollection Equipment { get; private set; }
		public BagCollection Bags { get; private set; }
		public ActionCollection Actions { get; private set; }
        public AwardCollection Awards { get; private set; }

		public Character()
			: base()
		{
			this.Init();
		}

		public Character(string userName, IClient client)
			: base()
		{
			this.UserName = userName;
			this.Client = client;
			this.Init();
		}

		private void Init()
		{
            this.Household = new HouseholdInfo();
			this.Spells = new Spellbook(this);
			this.Bags = new BagCollection(this);
			this.Equipment = new EquipmentCollection(this);
			this.Actions = new ActionCollection(this);
            this.Awards = new AwardCollection(this);
			this.ObjectType = ObjectType.Player;
			this.HasProperName = true;
			this.Level = 1;
			this.Experience = 0;
			this.Currency = new Currency(0);
			this.ExperienceMax = LevelManager.GetNextLevelXp(this.Level);
			this.TotalExperience = 0;
			this.DateCreated = DateTime.Now;

            this.Children.Added += new ActorEventHandler(Children_Added);
			//this.Children.Removed += new ActorEventHandler(Children_Removed);
		}

		internal void Reset()
		{
			DateTime dateCreated = this.DateCreated;
			this.Init();
			this.DateCreated = dateCreated;

			// Reset Skills.
			foreach (var skill in this.Skills.Keys)
			{
				this.Skills[skill] = 0;
			}

			// Reset Attributes.
			this.Attributes.Affinity = this.Attributes.Beauty = this.Attributes.Dexterity = this.Attributes.Endurance =
				this.Attributes.Intelligence = this.Attributes.Perception = this.Attributes.Stamina =
				this.Attributes.Strength = 0;

			// Remove all items.
			foreach (var item in this.GetAllChildren())
			{
				item.Owner = null;
				item.Save();
			}
			this.Children.Clear();

			// Remove properties associated with spells, bags, actions, equipment.
			var props = this.Properties.Keys.Where(p => p.StartsWith(this.Spells.Prefix)
				|| p.StartsWith(this.Bags.Prefix) || p.StartsWith(EquipmentCollection.Prefix)
				|| p.StartsWith(this.Actions.Prefix) || p.StartsWith("HasDiscovered")).ToList();
			for (int i = 0; i < props.Count; i++)
			{
				this.Properties.Remove(props[i]);
			}

			// Reset position.
			this.Location = this.World.Races[this.Race].StartingLocation;

			// Misc.
			this.Properties.SetValue(TargetIDProperty, 0);

			this.Properties.SetValue("RequiresReset", false, false);
        }

        private void Children_Added(ActorEventArgs e)
        {
            if (e.Actor is Quest)
            {
                // Register quest events if not already registered.
                if (!(e.Actor as Quest).HasEventsRegistered)
                {
                    (e.Actor as Quest).HookEvents(this);
                }
            }
        }

		private void Children_Removed(ActorEventArgs e)
		{
			if (e.Actor is ISpell)
			{
				this.Spells.Remove((ISpell)e.Actor);
			}
			else if (e.Actor is Container)
			{
				this.Bags.Remove((Container)e.Actor);
			}
		}

		protected override void AddSimpleRdlProperties(List<RdlObject> list)
		{
			base.AddSimpleRdlProperties(list);
			// RequiresReset is an alpha beta property.
			list.AddRange(this.GetRdlProperties(LevelProperty, ExperienceProperty, ExperienceMaxProperty, ZoneProperty, "RequiresReset"));

			// Household Properties
			if (this.Household != null)
			{
				list.AddRange(this.Household.ToRdlProperties(this));
			}

			list.Add(new RdlProperty(this.ID, "IsAdmin", this.IsAdmin));
		}

		protected override RdlActor GetObjectTag()
		{
			return base.GetObjectTag<RdlPlayer>();
		}

		protected override RdlProperty[] GetPropertyTags()
		{
			List<RdlProperty> list = new List<RdlProperty>(base.GetPropertyTags());

			// Remove certain properties from the list.
			list.RemoveAll(new Predicate<RdlProperty>(
				delegate(RdlProperty p)
				{
					return p.Name.Equals(UserNameProperty) 
						|| p.Name.Equals(TotalExperienceProperty)
						|| p.Name.Equals(TotalKillsInARowProperty)
						|| p.Name.Equals(TotalKillsProperty);
				}));

			list.Add(new RdlProperty(this.ID, "IsAdmin", this.IsAdmin));

			return list.ToArray();
		}

        public override void Heartbeat()
        {
            base.Heartbeat();

            bool performUpdate = false;
            // If Character has a target do not increase stats.
            if (this.Target == null ||
                ((this.Target is PerenthiaMobile) &&
                    (!(this.Target as PerenthiaMobile).IsHostile))
                    || this.Target.IsDead)
            {
                int increment = this.Properties.GetValue<int>("Level");
                if (this.Body < this.BodyMax)
                {
                    // Body is controlled by Stamina.
                    increment += this.Attributes.Stamina - 4;
                    if (increment <= 0) increment = 1;
                    this.SetBody(this.Body + increment);
                    performUpdate = true;
                }
                if (this.Mind < this.MindMax)
                {
                    // Mind is controlled by Endurance.
                    increment += this.Attributes.Endurance - 4;
                    if (increment <= 0) increment = 1;
                    this.SetMind(this.Mind + increment);
                    performUpdate = true;
                }
                if (performUpdate)
                {
                    this.Save();
                    this.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None, "You feel your health and willpower returning..."));
                    this.Context.AddRange(this.GetRdlProperties(Avatar.BodyProperty, Avatar.MindProperty));
                }
            }
            
            // Send down the time every minute.
            this.Context.Add(Time.GetTimeProperty());

            if (this.Target != null && this.Target is PerenthiaMobile)
            {
                PerenthiaMobile mobile = this.Target as PerenthiaMobile;
                mobile.Heartbeat();
                if (mobile.IsHostile && !mobile.IsStunned && !mobile.IsFrozen && !this.IsDead)
                {
                    // If the player is fighting a mobile then check the mobile's last attack time and attack the player if enough
                    // time has gone by.
                    if (mobile.CanAutoAttack())
                    {
                        mobile.Attack(this, this.Context);
                    }
                }
            }
        }

		public void CreateStartingItems(Server server)
		{
			// Add starting items and clothing to the new character.
			// Backpack
			Container backpack = server.World.CreateFromTemplate<Container>("Adventurer's Backpack");
			// Candle
			Light candle = server.World.CreateFromTemplate<Light>("Tallow Candle");
			// Shirt
			Clothing shirt = server.World.CreateFromTemplate<Clothing>("Woolen Shirt");
			// Pants
			Clothing pants = server.World.CreateFromTemplate<Clothing>("Woolen Pants");
			// Bread, Cheese and Water
			Food bread = server.World.CreateFromTemplate<Food>("Bread");
			Food cheese = server.World.CreateFromTemplate<Food>("Cheese");
			Food water = server.World.CreateFromTemplate<Food>("Water");

			// All players need the hand to hand skill.
			if (this.Skills["Hand To Hand"] < 2) this.Skills["Hand To Hand"] = 2;

			// The choice of primary, starting weapon and spell is dependent on the skills the player has selected.
			Item weapon = null;
			Spell spell = null;

			// Need Strenght to wield weapons.
			if (this.Skills["Elementalism - Earth"] > 0) weapon = server.World.CreateFromTemplate<Spell>("Falling Rock");
			else if (this.Skills["Elementalism - Air"] > 0) weapon = server.World.CreateFromTemplate<Spell>("Wind Blast");
			else if (this.Skills["Elementalism - Water"] > 0) weapon = server.World.CreateFromTemplate<Spell>("Ice Blast");
			else if (this.Skills["Elementalism - Fire"] > 0) weapon = server.World.CreateFromTemplate<Spell>("Flame Bolt");
			else if (this.Skills["Daggers"] > 0) weapon = server.World.CreateFromTemplate<Weapon>("Rusty Dagger");
			else if (this.Skills["Swords"] > 0) weapon = server.World.CreateFromTemplate<Weapon>("Rusty Short Sword");
			else if (this.Skills["Axes"] > 0) weapon = server.World.CreateFromTemplate<Weapon>("Worn Axe");
			else if (this.Skills["Maces"] > 0) weapon = server.World.CreateFromTemplate<Weapon>("Wooden Mace");
			else if (this.Skills["Clubs"] > 0) weapon = server.World.CreateFromTemplate<Weapon>("Wooden Club");
			else weapon = server.World.CreateFromTemplate<Weapon>("Fists");

			if (this.Skills["Life"] > 0) spell = server.World.CreateFromTemplate<Spell>("Minor Healing");
			else if (this.Skills["Elementalism - Earth"] > 0) spell = server.World.CreateFromTemplate<Spell>("Earth Shield");
			else if (this.Skills["Elementalism - Air"] > 0) spell = server.World.CreateFromTemplate<Spell>("Air Shield");
			else if (this.Skills["Elementalism - Water"] > 0) spell = server.World.CreateFromTemplate<Spell>("Water Shield");
			else if (this.Skills["Elementalism - Fire"] > 0) spell = server.World.CreateFromTemplate<Spell>("Fire Shield");
			else spell = server.World.CreateFromTemplate<Spell>("Combo Attack");

			// Save the items to the database.
			server.World.SaveActor(backpack);
			server.World.SaveActor(weapon);
			server.World.SaveActor(spell);
			server.World.SaveActor(shirt);
			server.World.SaveActor(pants);
			server.World.SaveActor(candle);

			this.Bags.Add(backpack);
			if (weapon is ISpell)
			{
				this.Spells.Add((ISpell)weapon);
			}
			else
			{
				this.Equipment.Equip(EquipmentSlot.Weapon1, weapon);
			}
			this.Equipment.Equip(EquipmentSlot.Shirt, shirt);
			this.Equipment.Equip(EquipmentSlot.Pants, pants);
			this.Equipment.Equip(EquipmentSlot.Light, candle);
			this.Spells.Add(spell);

			// Re-Save the items to the database.
			server.World.SaveActor(backpack);
			server.World.SaveActor(weapon);
			server.World.SaveActor(spell);
			server.World.SaveActor(shirt);
			server.World.SaveActor(pants);
			server.World.SaveActor(candle);

			// Preset Action_0 to the weapon and Action_1 to the spell.
			this.Actions.Set(0, weapon);
			this.Actions.Set(1, spell);

			// Save the bread, cheese and water.
			server.World.SaveActor(bread);
			server.World.SaveActor(cheese);
			server.World.SaveActor(water);

			// Set the owner of the bread, cheese and water to be the backpack.
			backpack.Add(bread);
			backpack.Add(cheese);
			backpack.Add(water);

			// Re-Save the bread, cheese and water.
			server.World.SaveActor(bread);
			server.World.SaveActor(cheese);
			server.World.SaveActor(water);

			// Money
			this.Currency = new Currency(25);

			// Perform a save on the player instance.
			server.World.SaveActor(this);
		}

		public override void OnAttack(ref int outcomeSuccessDice)
		{
			// Allow players of lower level to always succeed in attacking.
			if (this.Level < 10)
			{
				if (outcomeSuccessDice <= 0) outcomeSuccessDice = 1;
			}
		}

		public override void OnCastSkillRoll(ISpell spell, ref int castSuccessCount)
		{
			// Spells should always succeed when the skill is higher than the required level
			// and always fail when not higher.
			if (this.Skills[spell.Skill] >= spell.SkillLevelRequiredToEquip)
			{
				if (castSuccessCount <= 0) castSuccessCount = 1;
			}
			else
			{
				this.Context.Add(new RdlErrorMessage(String.Format("You do not have a high enough level of skill in {0} to cast {1}.",
					spell.Skill.Replace("Skill_", String.Empty), this.Name)));
				castSuccessCount = 0;
			}
		}

		public override void OnBuff(IActor sender, IMessageContext context, Foci foci)
		{
		}

		public override void OnDamage(IActor sender, IMessageContext context, Foci foci)
		{
		}

		public override void OnEnchant(IActor sender, IMessageContext context, Foci foci)
		{
		}

		public override void OnHeal(IActor sender, IMessageContext context, Foci foci)
		{
		}

		public Container GetFirstAvailableContainer(Item item)
		{
			if (item.IsStackable)
			{
				// See if this item already exists in another bag.
				for (int i = 0; i < this.Bags.Count; i++)
				{
					Container bag = this.Bags.Get(i);
					if (bag != null)
					{
						if (bag.GetAllChildren().Where(c => c is Item).Select(c => c as Item).Contains(item, new ItemComparer()))
						{
							return bag;
						}
					}
				}
			}
			return this.GetFirstAvailableContainer();
		}

		public Container GetFirstAvailableContainer()
		{
			for (int i = 0; i < this.Bags.Count; i++)
			{
				Container bag = this.Bags.Get(i);
				if (bag != null && !bag.IsFull())
				{
					return bag;
				}
			}
			return null;
			//// Loop through all the containers for this player and return the first one with open slots.
			//return (from c in this.Children
			//        where c is Container
			//        && !(c as Container).IsFull()
			//        && (c as Container).IsEquipped
			//        select c as Container).FirstOrDefault();
		}

        public void SetAction(int slotNum, IItem item)
        {
            string actionName = String.Concat(this.Actions.Prefix, slotNum);
            if (item != null)
            {
                if (slotNum >= 0 && slotNum <= 20)
                {
                    this.Actions.Set(slotNum, item);
                    this.Save();

                    this.Context.AddRange(this.GetRdlProperties(actionName));
                    this.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
                        String.Format(Resources.ActionSet, slotNum + 1, item.A())));
                }
                else
                {
                    this.Context.Add(new RdlErrorMessage(String.Format(Resources.ActionInvalidSlot, slotNum)));
                }
            }
            else
            {
                if (slotNum >= 0 && slotNum <= 20)
                {
                    // Reset the action slot.
                    this.Properties.SetValue(actionName, 0);
                    this.Save();

                    this.Context.AddRange(this.GetRdlProperties(actionName));
                    this.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
                        String.Format(Resources.ActionReset, slotNum + 1)));
                }
                else
                {
                    this.Context.Add(new RdlErrorMessage(String.Format(Resources.ActionInvalidSlot, slotNum)));
                }
            }
        }

		#region Commands
		public bool Equip(EquipmentSlot slot, IItem item)
		{
			if (item.EquipLocation != EquipLocation.None)
			{
				// Ensure that this item is not already equipped.
				if (item.IsEquipped)
				{
					this.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None, 
						String.Format(Resources.ItemAlreadyEquipped, item.Alias())));
					return true; // Return true even though the item is already equipped.
				}

                // Must have sufficient skill in order to equip an item.
				int skillLevel = 0;
				if (!String.IsNullOrEmpty(item.Skill))
				{
					skillLevel = (int)this.Skills[item.Skill];
				}
				if (skillLevel >= item.SkillLevelRequiredToEquip)
				{
                    // Determine if another item is equipped to the same location.
					if (this.Equipment.IsEquipped(slot))
					{
                        IItem equippedItem = this.Equipment.Get(slot);
                        if (equippedItem != null)
                        {
                            // Find a container to unequip the item to or the equip operation should fail.
                            if (!this.Unequip(slot, equippedItem, this.GetFirstAvailableContainer()))
                            {
                                return false;
                            }
                            
                            // If an action slot exists for the previously equipped item then set the newly equipped item into the action slot instead.
                            int index = this.Actions.IndexOf(equippedItem);
                            if (index >= 0)
                            {
                                this.SetAction(index, item);
                            }
                        }
                        else
                        {
                            // Might not have been removed properly, just unequip the slot.
                            this.Equipment.Unequip(slot);
                        }
					}

					// Equip the item.
					this.Equipment.Equip(slot, item);
					this.World.SaveActor(item);

					// Apply any affects from the item to the player.
					this.Attributes.ApplyAffects(item.Affects);

					// Send down the newly equipped item.
					this.Context.AddRange(item.ToRdl());

					this.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None, String.Format(Resources.ItemEquipped, item.A())));

					return true;
				}
				else
				{
					this.Context.Add(new RdlErrorMessage(String.Format(Resources.ItemSkillLevelRequiredNotMet, 
						item.A(), item.Skill, item.SkillLevelRequiredToEquip)));
				}
			}
			else
			{
				this.Context.Add(new RdlErrorMessage(String.Format(Resources.ItemCanNotEquip, item.A())));
			}
			return false;
		}

		public bool Unequip(EquipmentSlot slot, IItem item, Container container)
		{
			if (item.EquipLocation != EquipLocation.None && item != null)
			{
				// Destination Container
				if (container != null)
				{
					// Ensure the container has room for the item.
					if (container.Count < container.Capacity)
					{
						this.Equipment.Unequip(slot);
						(item as Item).Drop(container);
						this.World.SaveActor(item);

						// Remove any applied affects to the player.
						this.Attributes.RemoveAffects(item.Affects);

						// Send down the changed item and tell the client to remove the item
						// from the equipped slot.
						this.Context.AddRange(item.ToRdl());

						this.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None, String.Format(Resources.ItemUnequipped, item.A())));

						return true;
					}
					else
					{
						this.Context.Add(new RdlErrorMessage(String.Format(Resources.ContainerFull, container.Your())));
					}
				}
				else
				{
					this.Context.Add(new RdlErrorMessage(Resources.ItemCanNotEquipNoContainer));
				}
			}
			else
			{
				this.Context.Add(new RdlErrorMessage(String.Format(Resources.ItemCanNotEquipNoUnequip, item.A())));
			}
			return false;
		}
		#endregion
	}
	#endregion

	#region Spellbook
	/// <summary>
	/// Represents a list of spells learned by the avatar where the value is the spell.
	/// </summary>
	public class Spellbook : ActorOwnedItemCollection<ISpell>
	{
		public Spellbook(IActor owner)
			: base(owner, "Spell_")
		{
		}

		public override void Add(ISpell item)
		{
			foreach (var spell in this)
			{
				if (spell != null && spell.ID == item.ID)
					return;
			}
			base.Add(item);
			item.IsEquipped = true;
		}

		public override bool Remove(ISpell item)
		{
			bool result = base.Remove(item);
			if (result)
			{
				item.IsEquipped = false;
			}
			return result;
		}
	}
	#endregion

	#region BagCollection
	/// <summary>
	/// Represents a collection of bags that can be carried by an avatar where the value is the item.
	/// </summary>
	public class BagCollection : ActorOwnedItemCollection<Container>
	{
		public BagCollection(IActor owner)
			: base(owner, "Bag_")
		{
		}

		public override void Add(Container item)
		{
			// Check to see if the bag already exists in the collection.
			foreach (var bag in this)
			{
				if (bag != null && item != null)
				{
					if (bag.ID == item.ID)
						return;
				}
			}
			base.Add(item);
			item.IsEquipped = true;
		}

		public override bool Remove(Container item)
		{
			bool result = base.Remove(item);
			if (result)
			{
				item.IsEquipped = false;
			}
			return result;
		}
	}
	#endregion

	#region Equipment
	/// <summary>
	/// Represents a dictionary of equipped items where the key is the equip location and the value is the item id.
	/// </summary>
	public class EquipmentCollection : IEnumerable<IItem>
	{
		public static readonly string Prefix = "Equipment_";

		public IAvatar Owner { get; private set; }	

		//public IItem Head
		//{
		//    get { return this.Get(EquipmentSlot.Head); }
		//    set { this.Set(EquipmentSlot.Head, value); }
		//}

		public EquipmentCollection(IAvatar owner)
		{
			this.Owner = owner;
		}

		public static EquipmentSlot ParseEquipmentSlot(string value, IItem item, Character player)
		{
			if (!String.IsNullOrEmpty(value))
			{
				return ParseEquipmentSlot(value);
			}
			EquipmentSlot slot = EquipmentSlot.None;
			switch (item.EquipLocation)
			{
				case EquipLocation.Head:
					slot = EquipmentSlot.Head;
					break;
				case EquipLocation.Ear:
					slot = EquipmentSlot.Ear1;
					if (player.Equipment.IsEquipped(EquipmentSlot.Ear1)
						&& !player.Equipment.IsEquipped(EquipmentSlot.Ear2))
					{
						slot = EquipmentSlot.Ear2;
					}
					break;
				case EquipLocation.Neck:
					slot = EquipmentSlot.Neck;
					break;
				case EquipLocation.Shoulders:
					slot = EquipmentSlot.Shoulders;
					break;
				case EquipLocation.Arms:
					slot = EquipmentSlot.Arms;
					break;
				case EquipLocation.Chest:
					slot = EquipmentSlot.Chest;
					break;
				case EquipLocation.Back:
					slot = EquipmentSlot.Back;
					break;
				case EquipLocation.Finger:
					slot = EquipmentSlot.Finger1;
					if (player.Equipment.IsEquipped(EquipmentSlot.Finger1)
						&& !player.Equipment.IsEquipped(EquipmentSlot.Finger2))
					{
						slot = EquipmentSlot.Finger2;
					}
					break;
				case EquipLocation.Wrists:
					slot = EquipmentSlot.Wrists;
					break;
				case EquipLocation.Hands:
					slot = EquipmentSlot.Hands;
					break;
				case EquipLocation.Waist:
					slot = EquipmentSlot.Waist;
					break;
				case EquipLocation.Legs:
					slot = EquipmentSlot.Legs;
					break;
				case EquipLocation.Feet:
					slot = EquipmentSlot.Feet;
					break;
				case EquipLocation.Hat:
					slot = EquipmentSlot.Hat;
					break;
				case EquipLocation.Shirt:
					slot = EquipmentSlot.Shirt;
					break;
				case EquipLocation.Robe:
					slot = EquipmentSlot.Robe;
					break;
				case EquipLocation.Pants:
					slot = EquipmentSlot.Pants;
					break;
				case EquipLocation.Nose:
					slot = EquipmentSlot.Nose;
					break;
				case EquipLocation.Weapon:
					slot = EquipmentSlot.Weapon1;
					if (player.Equipment.IsEquipped(EquipmentSlot.Weapon1)
						&& !player.Equipment.IsEquipped(EquipmentSlot.Weapon2))
					{
						slot = EquipmentSlot.Weapon2;
					}
					break;
				case EquipLocation.Shield:
					slot = EquipmentSlot.Shield;
					break;
				case EquipLocation.Ammo:
					slot = EquipmentSlot.Ammo;
					break;
				case EquipLocation.Pendant:
					slot = EquipmentSlot.Pendant;
					break;
				case EquipLocation.Light:
					slot = EquipmentSlot.Light;
					break;
			}
			return slot;
		}

		public static EquipmentSlot ParseEquipmentSlot(string value)
		{
			int result;
			if (Int32.TryParse(value, out result))
			{
				return (EquipmentSlot)result;
			}
			return (EquipmentSlot)Enum.Parse(typeof(EquipmentSlot), value, true);
		}

		public bool IsEquipped(EquipmentSlot slot)
		{
			return this.Owner.Properties.GetValue<int>(this.GetPrefixedNamed(slot)) > 0;
		}

		public IItem Get(EquipmentSlot slot)
		{
			int id = this.Owner.Properties.GetValue<int>(this.GetPrefixedNamed(slot));
			return this.Owner.GetAllChildren().Where(c => c.ID == id).Select(c => c as IItem).FirstOrDefault();
		}

		public void Set(EquipmentSlot slot, IItem value)
		{
			value.IsEquipped = true;
			this.Owner.Children.Add(value);
			this.Owner.Properties.SetValue(this.GetPrefixedNamed(slot), value.ID);
			this.Owner.Context.AddRange(this.Owner.GetRdlProperties(this.GetPrefixedNamed(slot)));
		}

		public void Equip(EquipmentSlot slot, IItem value)
		{
			this.Set(slot, value);
		}

		public void Unequip(EquipmentSlot slot)
		{
			IItem item = this.Get(slot);
			if (item != null)
			{
				item.IsEquipped = false;
				this.Owner.Properties.SetValue(this.GetPrefixedNamed(slot), 0);
				this.Owner.Context.AddRange(this.Owner.GetRdlProperties(this.GetPrefixedNamed(slot)));
			}
		}

        internal void Unequip(IItem item)
        {
            Property prop = this.Owner.Properties.Values.Where(p => p.Name.StartsWith(Prefix) && p.Value.Equals(item.ID)).FirstOrDefault();
            if (prop != null)
            {
                item.IsEquipped = false;
                this.Owner.Properties.SetValue(prop.Name, 0);
            }
        }

		private string GetPrefixedNamed(EquipmentSlot slot)
		{
			return String.Concat(Prefix, slot.ToString());
		}

		#region IEnumerable<IItem> Members

		public IEnumerator<IItem> GetEnumerator()
		{
			List<IItem> list = new List<IItem>();
			var props = this.Owner.Properties.Values.Where(p => p.Name.StartsWith(Prefix));
			foreach (var prop in props)
			{
				var item = this.Owner.GetAllChildren().Where(c => c.ID == prop.GetValue<int>()).Select(c => c as IItem).FirstOrDefault();
				if (item != null)
				{
					list.Add(item);
				}
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

	#region ActionCollection
	/// <summary>
	/// Represents a collection of actions where the value is the item set the index/action.
	/// </summary>
	public class ActionCollection : ActorOwnedItemCollection<IItem>
	{
		public ActionCollection(IActor owner)
			: base(owner, "Action_")
		{
		}
	}
	#endregion
}
