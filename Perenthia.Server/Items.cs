using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia
{
	#region Item
	public class Item : Actor, IItem
	{
		#region Properties
		public EquipLocation EquipLocation
		{
			get { return this.Properties.GetValue<EquipLocation>(EquipLocationProperty); }
			set { this.Properties.SetValue(EquipLocationProperty, value, true); }
		}
		public static readonly string EquipLocationProperty = "EquipLocation";

		public bool IsEquipped
		{
			get { return this.Properties.GetValue<bool>(IsEquippedProperty); }
			set { this.Properties.SetValue(IsEquippedProperty, value); }
		}
		public static readonly string IsEquippedProperty = "IsEquipped";

		public override string ImageUri
		{
			get { return this.GetImageUri(); }
			set { this.Properties.SetValue(ImageUriProperty, value, true); }
		}

		public bool IsStackable
		{
			get { return this.Properties.GetValue<bool>(IsStackableProperty); }
			set { this.Properties.SetValue(IsStackableProperty, value, true); }
		}
		public static readonly string IsStackableProperty = "IsStackable";

		public int CurrencyValue
		{
			get { return this.Properties.GetValue<int>(CostProperty); }
			set { this.Properties.SetValue(CostProperty, value); }
		}	
		public Currency Cost
		{
			get { return new Currency(this.Properties.GetValue<int>(CostProperty)); }
			set { this.Properties.SetValue(CostProperty, value.Value, true); }
		}
		public static readonly string CostProperty = "Cost";

		public int EmblemCost
		{
			get { return this.Properties.GetValue<int>(EmblemCostProperty); }
			set { this.Properties.SetValue(EmblemCostProperty, value, true); }
		}
		public static readonly string EmblemCostProperty = "EmblemCost";

		public string Skill
		{
			get { return this.Properties.GetValue<string>(SkillProperty); }
			set { this.Properties.SetValue(SkillProperty, value, true); }
		}
		public static readonly string SkillProperty = "Skill";

		public int SkillLevelRequiredToEquip
		{
			get { return this.Properties.GetValue<int>(SkillLevelRequiredToEquipProperty); }
			set { this.Properties.SetValue(SkillLevelRequiredToEquipProperty, value, true); }
		}
		public static readonly string SkillLevelRequiredToEquipProperty = "SkillLevelRequiredToEquip";

		/// <summary>
		/// Gets or sets a value indicating whether or not the current item can be added to the inventory of an avatar.
		/// </summary>
		public bool IsInventoryItem
		{	
			get { return this.Properties.GetValue<bool>(IsInventoryItemProperty); }
			set { this.Properties.SetValue(IsInventoryItemProperty, value, true); }
		}
		public static readonly string IsInventoryItemProperty = "IsInventoryItem";

		public int Durability
		{
			get 
			{
				if (!this.Properties.ContainsKey(DurabilityProperty))
				{
					this.Durability = this.DurabilityMax;
				}
				return this.Properties.GetValue<int>(DurabilityProperty); 
			}
			set { this.Properties.SetValue(DurabilityProperty, value, false); }
		}
		public static readonly string DurabilityProperty = "Durability";

		public int DurabilityMax
		{	
			get { return this.Properties.GetValue<int>(DurabilityMaxProperty); }
			set { this.Properties.SetValue(DurabilityMaxProperty, value, true); }
		}
		public static readonly string DurabilityMaxProperty = "DurabilityMax";

		public AttributeList Affects { get; private set; }

		public int BuyCost
		{
			get { return this.Properties.GetValue<int>(BuyCostProperty); }
			set { this.Properties.SetValue(BuyCostProperty, value, false); }
		}
		public static readonly string BuyCostProperty = "BuyCost";

		public int SellCost
		{	
			get { return this.Properties.GetValue<int>(SellCostProperty); }
			set { this.Properties.SetValue(SellCostProperty, value, false); }
		}
		public static readonly string SellCostProperty = "SellCost";

		public int EmblemBuyCost
		{
			get { return this.Properties.GetValue<int>(EmblemBuyCostProperty); }
			set { this.Properties.SetValue(EmblemBuyCostProperty, value, false); }
		}
		public static readonly string EmblemBuyCostProperty = "EmblemBuyCost";
			
		public int EmblemSellCost
		{
			get { return this.Properties.GetValue<int>(EmblemSellCostProperty); }
			set { this.Properties.SetValue(EmblemSellCostProperty, value, false); }
		}
        public static readonly string EmblemSellCostProperty = "EmblemSellCost";

        public ItemQualityType Quality
        {
            get { return this.Properties.GetValue<ItemQualityType>(QualityProperty); }
            set { this.Properties.SetValue(QualityProperty, value, true); }
        }
        public static readonly string QualityProperty = "Quality";

        public bool IsUsable
        {
            get { return this.Properties.GetValue<bool>(IsUsableProperty); }
            set { this.Properties.SetValue(IsUsableProperty, value, true); }
        }
        public static readonly string IsUsableProperty = "IsUsable";
		#endregion	

		public Item()
			: base()
		{
			this.Init();	
		}

		public Item(RdlActor itemTag)
			: base(itemTag)
		{
			this.Init();
		}

		public Item(string name, string description)
			: base()
		{
			this.Name = name;
			this.Description = description;
			this.Init();
		}

		public override void OnLoadComplete()
		{
			base.OnLoadComplete();

			// Ensure ImageUri is set.
			this.GetImageUri();
		}
		protected virtual string GetImageUri()
		{
			string uri = this.Properties.GetValue<string>(ImageUriProperty);
			if (String.IsNullOrEmpty(uri))
			{
				uri = String.Format(Resources.ImgItem, 
					this.GetType().BaseType.Name.ToLower(), 
					this.GetType().Name.ToLower());
				this.Properties.SetValue(ImageUriProperty, uri, true);
			}
			return uri;
		}

		protected virtual void Init()
		{
			this.Affects = new AttributeList(this);
			this.IsEquipped = false;
			this.EquipLocation = EquipLocation.None;
			this.IsStackable = true;
			this.Cost = new Currency();
			this.EmblemCost = 0;
			this.SkillLevelRequiredToEquip = 0;
			this.IsInventoryItem = true;
            this.Quality = ItemQualityType.Poor;
            this.IsUsable = false;
		}

		protected override void AddSimpleRdlProperties(List<RdlObject> list)
		{
			base.AddSimpleRdlProperties(list);
			list.AddRange(this.GetRdlProperties(
				ImageUriProperty, 
				IsStackableProperty,
				IsEquippedProperty, 
				EquipLocationProperty, 
				CostProperty, 
				EmblemCostProperty,
                IsUsableProperty));
		}

		public void SetDurability(int durability)
		{
			if (durability < 0) durability = 0;
			if (this.DurabilityMax == 0)
			{
				if (durability > this.Durability) this.DurabilityMax = durability;
				else this.DurabilityMax = this.Durability;
			}
			if (durability > this.DurabilityMax) durability = this.DurabilityMax;
			this.Durability = durability;
		}

		public virtual int GetBuyCost(IAvatar buyer, double markupPercentage)
		{
			this.SetPriceAdjustment(buyer, ref markupPercentage);
			if (markupPercentage == 0) return this.Cost.Value;
			return (int)(this.Cost.Value * markupPercentage);
		}

		public virtual int GetSellCost(IAvatar buyer, double markdownPercentage)
		{
			this.SetPriceAdjustment(buyer, ref markdownPercentage);
			if (markdownPercentage == 0) return this.Cost.Value;
			if (markdownPercentage > 0) markdownPercentage *= -1;
			int sellCost = (int)(this.Cost.Value * markdownPercentage);
			if (sellCost < 0) sellCost = this.Cost.Value;
			return sellCost;
		}

		public virtual int GetEmblemCost(IAvatar buyer)
		{
			return this.EmblemCost;
		}

		protected virtual void SetPriceAdjustment(IAvatar buyer, ref double adjustmentPercentage)
		{
			// Seller is the owner, or owns the container where this item lives.
			//IAvatar seller = this.Owner as IAvatar;
			//double offsetPercentage = 0;
			//if (seller == null && this.Owner.Owner != null)
			//{
			//    seller = this.Owner.Owner as IAvatar;
			//}
			//if (seller != null)
			//{
			//    // If seller and buyer are of the same race then do not impact markup.
			//    if (!seller.Race.Equals(buyer.Race))
			//    {
			//        // Take beauty and affinity into account when buying.
			//        // Need to have a beauty score of at least 3 not to be ripped off.
			//        if (buyer.Attributes.Beauty < 3)
			//        {
			//            offsetPercentage += seller.Attributes.Beauty * 0.5;
			//        }
			//    }
			//}
			//adjustmentPercentage += offsetPercentage;
		}

		public override string GetOffensiveSkill()
		{
			if (!String.IsNullOrEmpty(this.Skill))
			{
				return this.Skill;
			}
			return "Hand To Hand";
		}

		public virtual void Use(IActor user, IMessageContext context)
		{
			context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Negative,
				String.Format(Resources.ItemCanNotBeUsed, this.TheUpper())));
		}

		public int Quantity()
		{
			if (this.Owner is Npc)
			{
				return 20;
			}
			else
			{
				if (this.IsStackable && this.Owner != null)
				{
					// Use Children instead of GetAllChildren because we only want items at the same level as the current.
					return this.Owner.Children.Where(c => c.GetType() == this.GetType() && c.Name.Equals(this.Name)).Count();
				}
				return 1;
			}
		}

		public virtual RdlCommand GetRemoveCommand(int ownerId)
		{
			return new RdlCommand("ITEMREMOVE", ownerId, this.ID);
		}
	}
	#endregion

	#region QuestItem
	public class QuestItem : Item
	{
		public QuestItem()
		{
		}

		public QuestItem(string name, string description)
			: base(name, description)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.IsStackable = false;
		}
	}
	#endregion

	#region Armor
	public class Armor : Item, IArmor
	{
		#region Properties
		public int Protection
		{	
			get { return this.Properties.GetValue<int>(ProtectionProperty); }
			set { this.Properties.SetValue(ProtectionProperty, value); }
		}
		public static readonly string ProtectionProperty = "Protection";
		#endregion

		public Armor()
			: base()
		{
			
		}

		public Armor(RdlActor itemTag)
			: base(itemTag)
		{
		}

		public Armor(string name, string description, int protection, EquipLocation equipLocation)
			: base(name, description)
		{
			this.Protection = protection;
			this.EquipLocation = equipLocation;
		}

		protected override void Init()
		{
			base.Init();
			this.IsStackable = false;
		}
	}
	#endregion

	#region Clothing
	public class Clothing : Armor
	{
		public ColorType Color
		{
			get { return this.Properties.GetValue<ColorType>(ColorProperty); }
			set { this.Properties.SetValue(ColorProperty, value, true); }
		}
		public static readonly string ColorProperty = "Color";

		public Clothing()
			: base()
		{
		}

		public Clothing(RdlActor itemTag)
			: base(itemTag)
		{
		}

		public Clothing(string name, string description, EquipLocation equipLocation)
			: this(name, description, 1, equipLocation, ColorType.None)
		{
		}

		public Clothing(string name, string description, EquipLocation equipLocation, ColorType color)
			: this(name, description, 1, equipLocation, color)
		{
		}

		public Clothing(string name, string description, int protection, EquipLocation equipLocation)
			: this(name, description, protection, equipLocation, ColorType.None)
		{
		}

		public Clothing(string name, string description, int protection, EquipLocation equipLocation, ColorType color)
			: base(name, description, protection, equipLocation)
		{
			this.Color = color;
		}

		protected override void Init()
		{
			base.Init();
			this.Color = ColorType.None;
		}
	}
	#endregion

	#region Shield
	public class Shield : Armor, IWeapon
	{
		#region Properties
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
		#endregion

		public Shield()
			: base()
		{
			
		}

		public Shield(RdlActor itemTag)
			: base(itemTag)
		{
		}

		public Shield(string name, string description, int protection, int power, int range)
			: base (name, description, protection, EquipLocation.Shield)
		{
			this.Power = power;
			this.Range = range;
		}

        protected override void Init()
        {
            base.Init();
            this.IsUsable = true;
        }

		public override string GetOffensiveSkill()
		{
			return "Bash";
		}

		public override void Use(IActor user, IMessageContext context)
		{
			if (user is IAvatar && (user as IAvatar).IsStunned)
			{
				context.Add(new RdlErrorMessage(String.Format(Resources.CanNotUseWhileStunned, this.A())));
				return;
			}
			if (user is IAvatar && (user as IAvatar).IsFrozen)
			{
				context.Add(new RdlErrorMessage(String.Format(Resources.CanNotUseWhileFrozen, this.A())));
				return;
			}

			this.Attack(user);
		}
	}
	#endregion

	#region Weapon
	public class Weapon : Item, IWeapon
	{
		#region Properties
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
		#endregion

		public Weapon()
			: base()
		{
		}

		public Weapon(RdlActor itemTag)
			: base(itemTag)
		{
		}

		public Weapon(string name, string description, int power, int range)
			: base(name, description)
		{
			this.Power = power;
			this.Range = range;
		}

		protected override void Init()
		{
			base.Init();
			this.EquipLocation = EquipLocation.Weapon;
			this.IsStackable = false;
            this.IsUsable = true;
		}

		public override void Use(IActor user, IMessageContext context)
		{
			if (user is IAvatar && (user as IAvatar).IsStunned)
			{
				context.Add(new RdlErrorMessage(String.Format(Resources.CanNotUseWhileStunned, this.A())));
				return;
			}
			if (user is IAvatar && (user as IAvatar).IsFrozen)
			{
				context.Add(new RdlErrorMessage(String.Format(Resources.CanNotUseWhileFrozen, this.A())));
				return;
			}
			this.Attack(user);
		}
	}
	#endregion

	#region AffectItem
	public abstract class AffectItem : Item, IAffectItem
	{
		#region IAffectItem Members

		public abstract TimeSpan Duration { get; set; }

		public bool IncreaseAffect
		{
			get { return this.Properties.GetValue<bool>(IncreaseAffectProperty); }
			set { this.Properties.SetValue(IncreaseAffectProperty, value, false); }
		}
		public static readonly string IncreaseAffectProperty = "IncreaseAffect";

		public int IncrementValue
		{
			get { return this.Properties.GetValue<int>(IncrementValueProperty); }
			set { this.Properties.SetValue(IncrementValueProperty, value, false); }
		}
		public static readonly string IncrementValueProperty = "IncrementValue";

		protected AffectItem()
		{
		}

		protected AffectItem(RdlActor item)
			: base(item)
		{
		}

		protected AffectItem(string name, string description)
			: base(name, description)
		{
		}

		public virtual void ApplyAffect(IAvatar target)
		{
		}

		public virtual void ApplyIncrementAffect(IAvatar target)
		{
		}

		public virtual void RemoveAffect(IAvatar target)
		{
		}

		#endregion
	}
	#endregion

	#region Spell
	public class Spell : AffectItem, ISpell
	{
		#region Properties
		public int Power
		{
			get { return this.Properties.GetValue<int>(PowerProperty); }
			set { this.Properties.SetValue(PowerProperty, value, true); }
		}
		public static readonly string PowerProperty = "Power";

		public int Range
		{
			get { return this.Properties.GetValue<int>(RangeProperty); }
			set { this.Properties.SetValue(RangeProperty, value, true); }
		}
		public static readonly string RangeProperty = "Range";

		public bool IsDamageSpell
		{	
			get { return this.Properties.GetValue<bool>(IsDamageSpellProperty); }
			set { this.Properties.SetValue(IsDamageSpellProperty, value, true); }
		}
		public static readonly string IsDamageSpellProperty = "IsDamageSpell";

		public override TimeSpan Duration
		{
			get { return TimeSpan.FromTicks(this.Properties.GetValue<long>(DurationProperty)); }
			set { this.Properties.SetValue(DurationProperty, value.Ticks, true); }	
		}
		public static readonly string DurationProperty = "Duration";		

		public virtual Foci Foci
		{
			get { return new Foci(this.Power, this.Range, this.Duration); }
		}

		public string SuccessMessage
		{
			get { return this.Properties.GetValue<string>(SuccessMessageProperty); }
			set { this.Properties.SetValue(SuccessMessageProperty, value, true); }
		}
		public static readonly string SuccessMessageProperty = "SuccessMessage";

		/// <summary>
		/// Gets or sets the actual affect power used when casting this spell.
		/// </summary>
		public int AffectPower
		{
			get { return this.Properties.GetValue<int>(AffectPowerProperty); }
			set { this.Properties.SetValue(AffectPowerProperty, value, false); }
		}
		public static readonly string AffectPowerProperty = "AffectPower";

		public AffectType AffectType
		{
			get { return this.Properties.GetValue<AffectType>(AffectTypeProperty); }
			set { this.Properties.SetValue(AffectTypeProperty, value, false); }
		}	
		public static readonly string AffectTypeProperty = "AffectType";
		#endregion

		public Spell()
			: base()
		{
			
		}

		public Spell(RdlActor itemTag)
			: base(itemTag)
		{
		}

		public Spell(string name, string description, int power, int range)
			: base(name, description)
		{
			this.Power = power;
			this.Range = range;
		}

		protected override void Init()
		{
			base.Init();
			this.IsStackable = false;
            this.IsUsable = true;
			this.AffectType = AffectType.None;
		}

		protected override void AddSimpleRdlProperties(List<RdlObject> list)
		{
			base.AddSimpleRdlProperties(list);
			list.AddRange(this.GetRdlProperties(AffectPowerProperty, AffectTypeProperty));
		}

		public override void Use(IActor user, IMessageContext context)
		{
			if (user is IAvatar && (user as IAvatar).IsStunned)
			{
				context.Add(new RdlErrorMessage(String.Format(Resources.CanNotUseWhileStunned, this.A())));
				return;
			}
			if (user is IAvatar && (user as IAvatar).IsFrozen)
			{
				context.Add(new RdlErrorMessage(String.Format(Resources.CanNotUseWhileFrozen, this.A())));
				return;
			}

			if (this.IsDamageSpell)
			{
				this.Attack(user);
			}
			else
			{
				IAvatar caster = user as IAvatar;
				if (caster != null)
				{
					IActor target = caster.Target;
					if (target == null) target = caster;
					CastResults results = MagicManager.PerformCast(this, caster, target);
					caster.Context.AddRange(caster.GetRdlProperties(Avatar.BodyProperty, Avatar.MindProperty));
				}
			}
		}

		#region ISpell Members

		protected void NoAffect(IAvatar caster)
		{
			caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Neutral, "Your spell has no effect."));
		}

		public virtual void Cast(IAvatar caster, IActor target, CastResults results)
		{
			this.NoAffect(caster);
		}
		#endregion
	}
	#endregion

	#region Potion
	public class Potion : AffectItem
	{
		public override TimeSpan Duration
		{
			get { return new TimeSpan(this.Properties.GetValue<long>(DurationProperty)); }
			set { this.Properties.SetValue(DurationProperty, value.Ticks, false); }
		}
		public static readonly string DurationProperty = "Duration";

		public int Power
		{		
			get { return this.Properties.GetValue<int>(PowerProperty); }
			set { this.Properties.SetValue(PowerProperty, value); }
		}
		public static readonly string PowerProperty = "Power";

		public PotionType Type
		{		
			get { return this.Properties.GetValue<PotionType>(TypeProperty); }
			set { this.Properties.SetValue(TypeProperty, value, true); }
		}
		public static readonly string TypeProperty = "Type";

		public Potion()
		{
		}

		public Potion(string name, string description)
			: base(name, description)
		{
		}

		protected override void Init()
		{
			base.Init();
            this.Type = PotionType.Heal;
            this.IsUsable = true;
		}

		public override void Use(IActor user, IMessageContext context)
		{
			bool noEffect = false;

			switch (this.Type)
			{
				case PotionType.Heal:
					if (user.Body < user.BodyMax)
					{
						user.SetBody(user.Body + this.Power);
						context.AddRange(user.GetRdlProperties(Actor.BodyProperty));
						context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
							String.Format(Resources.ItemHealthGained, this.TheUpper())));
					}
					else
						noEffect = true;
					break;
				case PotionType.Mind:
					IAvatar avatar = user as IAvatar;
					if (avatar != null)
					{
						if (avatar.Mind < avatar.MindMax)
						{
							avatar.SetMind(avatar.Mind + this.Power);
							context.AddRange(user.GetRdlProperties(Avatar.MindProperty));
							context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
								String.Format(Resources.ItemWillpowerGained, this.TheUpper())));
						}
						else
							noEffect = true;
					}
					else
						noEffect = true;
					break;
				case PotionType.Damage:
					user.SetBody(user.Body - this.Power);
					context.AddRange(user.GetRdlProperties(Actor.BodyProperty));
					context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Negative,
						String.Format(Resources.ItemHealthLost, this.TheUpper())));
					break;
				default:
					noEffect = true;
					break;
			}

			if (noEffect)
			{
				context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
					String.Format(Resources.ItemNoAffect, this.TheUpper())));
			}

			// Item has been used, remove it.
			if (this.Owner != null)
			{
				this.Owner = null;
				this.Save();
				// Send down a remove command for the item.
				context.Add(this.GetRemoveCommand(user.ID));
			}
		}
	}
	#endregion

	#region Container
	public class Container : Item
	{
		private static string _prefix = "Slot_";

		public int Capacity
		{
			get { return this.Properties.GetValue<int>(CapacityProperty); }
			set { this.Properties.SetValue(CapacityProperty, value); }
		}
		public static readonly string CapacityProperty = "Capacity";

		public int Count
		{
			get  { return this.Properties.GetValue<int>(CountProperty); }
			set { this.Properties.SetValue(CountProperty, value); }
		}
		public static readonly string CountProperty = "Count";

		private object _lock = new object();

		public Container()
			: base()
		{
		}

		public Container(string name, string description, int capacity)
			: base(name, description)
		{
			this.Capacity = capacity;
			this.Count = 0;
		}

		protected override void Init()
		{
			base.Init();
			this.IsStackable = false;
			this.EquipLocation = EquipLocation.Bag;
			this.Capacity = 4;
			this.Children.Added += new ActorEventHandler(Children_Added);
		}

		private void Children_Added(ActorEventArgs e)
		{
			// Insert the actor instance into a slot in the container.
			this.Refresh();
		}

		private void Refresh()
		{
			// Loop through the listed items, grouping stackables and resetting the Count value.
			this.Count = this.Children.Where(c => c is Item).Select(c => c as Item).Distinct(new ItemComparer()).Count();
		}

		public bool Add(Item item)
		{
			if (this.Count < this.Capacity)
			{
				lock (_lock)
				{
					this.Children.Add(item);
				}
				return true;
			}
			return false;
		}

		public bool Remove(Item item)
		{
			bool result = this.Children.Remove(item);
			this.Refresh();
			return result;
		}

		public bool IsFull()
		{
			if (this.Count == 0 && this.Children.Count > 0)
			{
				this.Refresh();
			}
			return this.Count >= this.Capacity;
		}

		public int GetRemainingSlots(Item item)
		{
			this.Refresh();
			if (item.IsStackable)
			{
				int quantity = this.Children.Count(c => c.GetType() == item.GetType() && c.Name.Equals(item.Name));
				if (this.Count > 0 && quantity > 0)
				{
					// Existing stackable items exist in the container so it can hold more of the same.
					return this.Capacity - (this.Count - 1);
				}
			}
			if (this.Count > 0)
			{
				return this.Capacity - this.Count;
			}
			return this.Capacity;
		}
	}
	#endregion

	#region Light
	public class Light : Item
	{
		public int Range
		{
			get { return this.Properties.GetValue<int>(RangeProperty); }
			set { this.Properties.SetValue(RangeProperty, value); }
		}
		public static readonly string RangeProperty = "Range";

		public Light()
			: base()
		{
		}

		public Light(string name, string description, int range)
			: base(name, description)
		{
			this.Range = range;
		}

		protected override void Init()
		{
			base.Init();
            this.IsStackable = false;
            this.IsUsable = true;
			this.EquipLocation = EquipLocation.Light;
			this.Range = 1;
		}
	}
	#endregion

	#region Food
	public class Food : Item
	{
		public int Power
		{
			get { return this.Properties.GetValue<int>(PowerProperty); }
			set { this.Properties.SetValue(PowerProperty, value); }
		}
		public static readonly string PowerProperty = "Power";

		public Food()
		{
		}

		public Food(string name, string description, int power)
			: base(name, description)
		{
			this.Power = power;
		}

		public Food(string name, string description, int power, string imageUri)
			: this(name, description, power)
		{
			this.ImageUri = imageUri;
		}

		protected override void Init()
		{
            base.Init();
            this.IsUsable = true;
		}

		public override void Use(IActor user, IMessageContext context)
		{
			bool noEffect = false;

			if (user.Body < user.BodyMax)
			{
				user.SetBody(user.Body + this.Power);
				context.AddRange(user.GetRdlProperties(Actor.BodyProperty));
				context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
					String.Format(Resources.ItemHealthGained, this.TheUpper())));
			}
			else
				noEffect = true;

			if (noEffect)
			{
				context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
					String.Format(Resources.ItemNoAffect, this.TheUpper())));
			}

			// Item has been used, remove it.
			if (this.Owner != null)
			{
				this.Owner = null;
				this.Save();
				// Send down a remove command for the item.
				context.Add(this.GetRemoveCommand(user.ID));
			}
		}
	}
	#endregion

	#region Transport
	public class Transport : Item
	{
		#region Properties
        /// <summary>
        /// Gets or sets the Range property value using the underlying Properties collection.
        /// </summary>
        public int Range
        {
            get { return this.Properties.GetValue<int>(RangePropertyName); }
            set { this.Properties.SetValue(RangePropertyName, value, true); }
        }   
        /// <summary>
        /// Gets the name of the Range property as stored in the Properties collection.
        /// </summary>
        public const string RangePropertyName = "Range";

        /// <summary>
        /// Gets or sets the Route property value using the underlying Properties collection.
        /// </summary>
        public string Route
        {
            get { return this.Properties.GetValue<string>(RoutePropertyName); }
            set { this.Properties.SetValue(RoutePropertyName, value, false); }
        }
        /// <summary>
        /// Gets the name of the Route property as stored in the Properties collection.
        /// </summary>
        public const string RoutePropertyName = "Route";
		#endregion

		public Transport()
			: base()
		{	
            // TODO: Transports
            // Utilizing a transport allows a character to move more tiles at a time, based on the range of the 
            // transport. Transports may not be used in dungeons, anything below z-index 0. Anything above
            // z-index 0 will depend on the type of the terrain.

            // At the begining of movement an encounter will determine if the player encounters a creature during transport. The
            // full path of the destination should be analyzed to determine if the player is traveling over wilderness and/or
            // traveling at night.

            // Put transport icons up near the player's avatar panel. 

            // Clicking on the transport icon should display a small panel that hides the transport icons and indicates the player
            // is mounted, with a button to dismount or disembark. For ships, disembark will drop at nearest tile, in the facing
            // direction. Use the Character.TransportID value to determine if the player is mounted or not.

            // When mounted the map should enable tile clicks, which should send a move command to the server with the x, y and z of the movement
            // location. The server should then determine how far the play can move in that direction and move them to the tile they clicked or
            // to the max number of tiles based range if the tile clicked is out of range. Display a small help window when mounted to indicate that
            // the tiles are clickable. Should be the start of the help system.
		}

		public Transport(RdlActor actorTag)
			: base(actorTag)
		{
		}

		public Transport(string name, string description)
			: base(name, description)
		{
		}

		protected override void Init()
		{
			base.Init();
            this.IsStackable = false;
            this.IsUsable = true;
			this.IsInventoryItem = false; // Can not be added to inventory.
		}

		public override void Use(IActor user, IMessageContext context)
		{
			// TODO: Implement USE for Transports.
            // 1. Plot path and save current route.

            // 2. Anylize route for unsafe places

            // 3. Check for encounter, return to route after encounter, if not dead.

            // 4. Move player
			base.Use(user, context);
		}
	}
	#endregion

	#region RepairKit
	public class RepairKit : Item
	{
		public RepairKit()
			: base()
		{			
		}

		public RepairKit(RdlActor actorTag)
			: base(actorTag)
		{
		}

		public RepairKit(string name, string description)
			: base (name, description)
		{
		}

        protected override void Init()
        {
            base.Init();
            this.IsUsable = true;
        }

		public override void Use(IActor user, IMessageContext context)
		{
			// TODO: Implement USE for RepairKits.
			base.Use(user, context);
		}
	}
	#endregion

	#region Artifact
	public class Artifact : Item
	{
		public Artifact()
		{
			
		}

		protected override void Init()
		{
            base.Init();
            this.IsUsable = true;
		}
	}
	public class TravelersWand : Artifact
	{
		public Point3 Destination
		{	
			get { return Point3.FromString(this.Properties.GetValue<string>(DestinationProperty)); }
			set { this.Properties.SetValue(DestinationProperty, value.ToString(), false); }
		}
		public static readonly string DestinationProperty = "Destination";

		public TravelersWand()
		{
			this.EmblemCost = 2;
		}
	}
	#endregion

	#region Recipe
	public class Recipe : Item
	{
		public TemplateItemCollection Ingredients { get; private set; }
		public CraftedItemPropertyCollection CraftedItemProperties { get; private set; }	

		public string SkillRequired
		{	
			get { return this.Properties.GetValue<string>(SkillRequiredProperty); }
			set { this.Properties.SetValue(SkillRequiredProperty, value, true); }
		}
		public static readonly string SkillRequiredProperty = "SkillRequired";

		public int SkillValueRequired
		{
			get { return this.Properties.GetValue<int>(SkillValueRequiredProperty); }
			set { this.Properties.SetValue(SkillValueRequiredProperty, value, true); }
		}
		public static readonly string SkillValueRequiredProperty = "SkillValueRequired";

		public string CraftedItemName
		{	
			get { return this.Properties.GetValue<string>(CraftedItemNameProperty); }
			set { this.Properties.SetValue(CraftedItemNameProperty, value, true); }
		}
		public static readonly string CraftedItemNameProperty = "CraftedItemName";

		public string CraftedItemTypeName
		{	
			get { return this.Properties.GetValue<string>(CraftedItemTypeNameProperty); }
			set { this.Properties.SetValue(CraftedItemTypeNameProperty, value, true); }
		}
		public static readonly string CraftedItemTypeNameProperty = "CraftedItemTypeName";

		public Recipe()
			: this(String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, 0)
		{
			//=====================================================================================
			// EXAMPLE - Leather Recipe
			//=====================================================================================
			// 2 Hides
			// 1 Tanning Solution
			// Tanning Skill of 1
			//=====================================================================================
		}

		public Recipe(string name, string description, string skillRequired, int skillValueRequired)
			: this(name, description, String.Empty, String.Empty, skillRequired, skillValueRequired, null)
		{
		}

		public Recipe(string name, string description, string craftedItemName, string craftedItemTypeName, string skillRequired, int skillValueRequired)
			: this(name, description, craftedItemName, craftedItemTypeName, skillRequired, skillValueRequired, null)
		{
		}

		public Recipe(
			string name,
			string description, 
			string craftedItemName, 
			string craftedItemTypeName,
			string skillRequired, 
			int skillValueRequired, 
			params TemplateItem[] ingredients)
			: base(name, description)
		{
			this.CraftedItemProperties = new CraftedItemPropertyCollection(this);
			this.Ingredients = new TemplateItemCollection(this, "Ingredient_");
			this.CraftedItemName = craftedItemName;
			this.CraftedItemTypeName = craftedItemTypeName;
			this.SkillRequired = skillRequired;
			this.SkillValueRequired = skillValueRequired;
			if (ingredients != null && ingredients.Length > 0)
			{
				foreach (var ingredient in ingredients)
				{
					this.Ingredients.Add(ingredient);
				}
			}
		}

		protected override void Init()
		{
            base.Init();
            this.IsUsable = true;
		}

		protected virtual Item CreateItem()
		{
			Item item = this.World.CreateFromTemplate<Item>(this.CraftedItemName);//Activator.CreateInstance(Type.GetType(this.CraftedItemTypeName)) as Item;
			if (item != null)
			{
				item.World = this.World;
				item.Name = this.CraftedItemName;
				foreach (var prop in this.CraftedItemProperties)
				{
					item.Properties.SetValue(prop.Key.Replace(this.CraftedItemProperties.Prefix, ""), prop.Value);
				}
			}
			return item;
		}

		public override void Use(IActor user, IMessageContext context)
		{
			// Attempt to make the item specified in the Recipe.

			// Ensure the owner has the correct skill level.
			if (this.Owner is Avatar)
			{
				int skillLevel = (int)(this.Owner as Avatar).Skills[this.SkillRequired];
				if (skillLevel < this.SkillValueRequired)
				{
					context.Add(new RdlErrorMessage(String.Format(Resources.CraftNotRequiredSkillLevel,
						this.SkillValueRequired, this.SkillRequired, this.CraftedItemName.A(false))));
					return;
				}
			}
			else
			{
				context.Add(RdlErrorMessage.InvalidCommand);
				return;
			}

			// Owner of this recipe must have the ingredients for the recipe in their inventory.
			List<Item> craftItems = new List<Item>();
			foreach (var ingredient in this.Ingredients)
			{
				var items = this.Owner.GetAllChildren().Where(c => c is Item 
					&& c.Name.Equals(ingredient.Name)).Take(ingredient.Quantity).Select(c => c as Item);
				if (items.Count() >= ingredient.Quantity)
				{
					craftItems.AddRange(items);
				}
				else
				{
					context.Add(new RdlErrorMessage(String.Format(Resources.CraftIngredientRequired,
						ingredient.Name.A(false, ingredient.Quantity), this.CraftedItemName)));
					return;
				}
			}

			// If we made it down here then the player has the required items and skill, attempt to craft the item.
			int successCount = 0;
			SkillManager.PerformSkillTest(this.Owner as IAvatar, this.SkillRequired, AttributeType.Perception, 0, this.SkillValueRequired, true, out successCount);
			if (successCount > 0)
			{
				// Success, create the item and add it to the owner's inventory.
				Item craftedItem = this.CreateItem();

				if (this.Owner is Character)
				{
					// Player, requires a container with an open slot.
					Character player = this.Owner as Character;
					Container container = player.GetFirstAvailableContainer();
					if (container != null)
					{
						context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive, String.Format(Resources.CraftSuccess, craftedItem.A())));
						craftedItem.Drop(container);

						// Advance the skill used.
						SkillManager.AdvanceSkill(player, this.SkillRequired, this.SkillValueRequired, player.Context);

						// Delete the recipe.
						this.Drop(null);
						this.Save();
					}
					else
					{
						context.Add(new RdlErrorMessage(String.Format(Resources.CraftNoSpaceLeft,
							this.CraftedItemName.The(false))));
						return;
					}
				}
				else
				{
					// NPC
					craftedItem.Drop(this.Owner);
				}
				craftedItem.Save();
			}
			else
			{
				// Failed.
				context.Add(new RdlErrorMessage(String.Format(Resources.CraftFailed,
					this.CraftedItemName.A(false))));
			}
		}
	}
	public class CraftedItemPropertyCollection : ActorOwnedDictionaryBase<object>
	{
		public CraftedItemPropertyCollection(IActor owner)
			: base(owner, "CraftedItemProp_")
		{
		}

		public override bool TryGetValue(string key, out object value)
		{
			throw new NotImplementedException();
		}
	}
	#endregion

	#region TrainSkill
	public class TrainSkill : Item
	{
		public TrainSkill()
			: this("Train Skill", String.Empty, "Hand To Hand")
		{
			
		}

		public TrainSkill(string name, string description, string skill)
			: base(name, description)
		{
			this.Skill = skill;
			this.IsInventoryItem = false;
			this.IsStackable = false;
		}

        protected override void Init()
        {
            base.Init();
            this.IsUsable = true;
        }

		public override void Use(IActor user, IMessageContext context)
		{
			IAvatar player = user as IAvatar;
			if (player != null)
			{
				player.Skills[this.Skill] += 1;
				player.Save();

				player.Context.Add(new RdlProperty(player.ID, String.Concat(player.Skills.Prefix, this.Skill), player.Skills[this.Skill]));

				player.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
					String.Format(Resources.SkillGained, this.Skill, (int)player.Skills[this.Skill])));
			}
		}

		public override int GetEmblemCost(IAvatar buyer)
		{
			int level = buyer.Properties.GetValue<int>(PerenthiaAvatar.LevelProperty);
			double skillValue = buyer.Skills[this.Skill];
			if (skillValue > 0 && skillValue < 50)
			{
				return this.EmblemCost * level;
			}
			else if (skillValue >= 50)
			{
				return (this.EmblemCost * level) + level;
			}
			return this.EmblemCost;
		}

		protected override void SetPriceAdjustment(IAvatar buyer, ref double adjustmentPercentage)
		{
			// Adjust the price based on the skill level of the buyer and the current skill being trained.
			double skillValue = buyer.Skills[this.Skill];
			if (skillValue > 0)
			{
				// The percentage hike should be 1 whole number for each level attained, making training more expensive as
				// you level up.
				adjustmentPercentage += skillValue;
			}

			// Perform price adjustment based on beauty.
			base.SetPriceAdjustment(buyer, ref adjustmentPercentage);
		}
	}
	#endregion

	#region ItemComparer
	public class ItemComparer : IEqualityComparer<Item>
	{
		#region IEqualityComparer<Item> Members

		public bool Equals(Item x, Item y)
		{
			if (x.IsStackable && y.IsStackable)
			{
				return x.Name.Equals(y.Name) && x.GetType() == y.GetType();
			}
			else
			{
				return x.ID == y.ID;
			}
		}

		public int GetHashCode(Item obj)
		{
			if (obj.IsStackable)
			{
				return obj.Name.GetHashCode() + obj.GetType().GetHashCode();
			}
			else
			{
				return obj.ID.GetHashCode();
			}
		}

		#endregion
	}
	#endregion
}
