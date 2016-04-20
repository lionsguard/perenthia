using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance
{
	#region Avatar
	/// <summary>
	/// Represents the abstract base class for all people and creatures within the virtual world.
	/// </summary>
	public abstract class Avatar : Actor, IAvatar
	{
		#region Properties
		/// <summary>
		/// Gets the list of attributes for the current Avatar.
		/// </summary>
		public AttributeList Attributes { get; private set; }

		/// <summary>
		/// Gets the list of skills of the current Avatar.
		/// </summary>
		public SkillList Skills { get; private set; }

		/// <summary>
		/// Gets or sets the luck value for the current Avatar.
		/// </summary>
		public int Luck
		{	
			get { return this.Properties.GetValue<int>(LuckProperty); }
			set { this.Properties.SetValue(LuckProperty, value); }
		}
		public static readonly string LuckProperty = "Luck";
	

		/// <summary>
		/// Gets or sets the initiative of the current Avatar. This value is set during combat by the game engine.
		/// </summary>
		public int Initiative
		{	
			get { return this.Properties.GetValue<int>(InitiativeProperty); }
			set { this.Properties.SetValue(InitiativeProperty, value); }
		}
		public static readonly string InitiativeProperty = "Initiative";
	

		/// <summary>
		/// Gets a value indicating whether or not the Avatar has been knocked unconscious.
		/// </summary>
		public bool IsUnconscious
		{	
			get { return this.Properties.GetValue<bool>(IsUnconsciousProperty); }
			protected set { this.Properties.SetValue(IsUnconsciousProperty, value); }
		}
		public static readonly string IsUnconsciousProperty = "IsUnconscious";

		/// <summary>
		/// Gets or sets a value indicating the physical health of the Avatar.
		/// </summary>
		public override int Body
		{
			get { return base.Body; }
			protected set
			{
				base.Body = value;
				this.CheckHealth();
			}
		}		

		/// <summary>
		/// Gets a value indicating the mental health of the Avatar.
		/// </summary>
		public int Mind
		{	
			get { return this.Properties.GetValue<int>(MindProperty); }
			protected set { this.Properties.SetValue(MindProperty, value); }
		}
		public static readonly string MindProperty = "Mind";
	

		/// <summary>
		/// Gets a value indicating the maximum mental health of the Avatar.
		/// </summary>
		public int MindMax
		{	
			get { return this.Properties.GetValue<int>(MindMaxProperty); }
			protected set { this.Properties.SetValue(MindMaxProperty, value); }
		}
		public static readonly string MindMaxProperty = "MindMax";
	

		/// <summary>
		/// Gets or sets the value of any magical protection applied to the avatar.
		/// </summary>
		public int Protection
		{
			get { return this.Properties.GetValue<int>(ProtectionProperty); }
			set { this.Properties.SetValue(ProtectionProperty, value); }
		}
		public static readonly string ProtectionProperty = "Protection";
	

		private Place _place = null;
		/// <summary>
		/// Gets or sets the place where the current avatar resides.
		/// </summary>
		public Place Place
		{
			get
			{
				if (_place == null)
				{
					_place = this.World.FindPlace(this.Location);
				}
				return _place;
			}
			set
			{
				_place = value;
				this.Location = value.Location;
			}
		}

		/// <summary>
		/// Gets or sets the name of the Avatar.
		/// </summary>
		public override string Name
		{
			get
			{
				return String.Format("{0} {1}", this.FirstName, this.LastName).Trim();
			}
			set
			{
				string[] names = value.Split(' ');
				if (names != null && names.Length > 0)
				{
					this.FirstName = names[0];
					if (names.Length > 1)
					{
						string[] remainder = new string[names.Length - 1];
						Array.Copy(names, 1, remainder, 0, names.Length - 1);
						this.LastName = String.Join(" ", remainder);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the first name of the avatar.
		/// </summary>
		public string FirstName
		{	
			get { return this.Properties.GetValue<string>(FirstNameProperty); }
			set { this.Properties.SetValue(FirstNameProperty, value); }
		}
		public static readonly string FirstNameProperty = "FirstName";
	

		/// <summary>
		/// Gets or sets the last name of the avatar.
		/// </summary>
		public string LastName
		{	
			get { return this.Properties.GetValue<string>(LastNameProperty); }
			set { this.Properties.SetValue(LastNameProperty, value); }
		}
		public static readonly string LastNameProperty = "LastName";
	

		/// <summary>
		/// Gets or sets the location of the current avatar.
		/// </summary>
		public Point3 Location
		{
			get
			{
				return new Point3
				{
					X = this.Properties.GetValue<int>(XProperty),
					Y = this.Properties.GetValue<int>(YProperty),
					Z = this.Properties.GetValue<int>(ZProperty)
				};
			}
			set
			{
				this.Properties.SetValue(XProperty, value.X);
				this.Properties.SetValue(YProperty, value.Y);
				this.Properties.SetValue(ZProperty, value.Z);
			}
		}

		/// <summary>
		/// Gets or sets the x coordinate of the current avatar.
		/// </summary>
		public int X
		{
			get { return this.Properties.GetValue<int>(XProperty); }
			set { this.Properties.SetValue(XProperty, value); }
		}
		public static readonly string XProperty = "X";
	
		
		/// <summary>
		/// Gets or sets the y coordinate of the current avatar.
		/// </summary>
		public int Y
		{
			get { return this.Properties.GetValue<int>(YProperty); }
			set { this.Properties.SetValue(YProperty, value); }
		}
		public static readonly string YProperty = "Y";
	

		/// <summary>
		/// Gets or sets the z coordinate of the current avatar.
		/// </summary>
		public int Z
		{	
			get { return this.Properties.GetValue<int>(ZProperty); }
			set { this.Properties.SetValue(ZProperty, value); }
		}
		public static readonly string ZProperty = "Z";
	

		/// <summary>
		/// Gets or sets the IMessageContext used for the current avatar.
		/// </summary>
		public IMessageContext Context { get; set; }

		/// <summary>
		/// Gets or sets the gender of the avatar.
		/// </summary>
		public virtual Gender Gender
		{	
			get { return this.Properties.GetValue<Gender>(GenderProperty); }
			set { this.Properties.SetValue(GenderProperty, value); }
		}
		public static readonly string GenderProperty = "Gender";
	

		/// <summary>
		/// Gets or sets the race of the avatar.
		/// </summary>
		public virtual string Race
		{	
			get { return this.Properties.GetValue<string>(RaceProperty); }
			set { this.Properties.SetValue(RaceProperty, value); }
		}
		public static readonly string RaceProperty = "Race";

		/// <summary>
		/// Gets or sets the Target Actor of the current Avatar.
		/// </summary>
		public IActor Target
		{
			get
			{
				if (_target == null)
				{
					int id = this.Properties.GetValue<int>(TargetIDProperty);
					if (id > 0 && this.World != null)
					{
						_target = this.World.GetActor(id);
					}
				}
				return _target;
			}
			set
			{
				_target = value;
				if (value != null)
				{
					this.Properties.SetValue(TargetIDProperty, value.ID);
				}
				else
				{
					this.Properties.SetValue(TargetIDProperty, 0);
				}
			}
		}
		private IActor _target = null;
		public static readonly string TargetIDProperty = "TargetID";

		public bool IsBuilder { get; set; }

		/// <summary>
		/// Gets a collection of actors affecting the current avatar.
		/// </summary>
		public AffectCollection Affects { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not the avatar has been poisoned.
		/// </summary>
		public bool IsPoisoned
		{
			get { return this.Properties.GetValue<bool>(IsPoisonedProperty); }
			set { this.Properties.SetValue(IsPoisonedProperty, value, false); }
		}
		public static readonly string IsPoisonedProperty = "IsPoisoned";	

		/// <summary>
		/// Gets or sets a value indicating whether or no the avatar is sutnned.
		/// </summary>
		public bool IsStunned
		{
			get { return this.Properties.GetValue<bool>(IsStunnedProperty); }
			set { this.Properties.SetValue(IsStunnedProperty, value, false); }
		}
		public static readonly string IsStunnedProperty = "IsStunned";

		/// <summary>
		/// Gets or sets a value indicating whether or not the avatar is immobilized.
		/// </summary>
		public bool IsSnared
		{
			get { return this.Properties.GetValue<bool>(IsSnaredProperty); }
			set { this.Properties.SetValue(IsSnaredProperty, value, false); }
		}
		public static readonly string IsSnaredProperty = "IsSnared";

		/// <summary>
		/// Gets or sets a value indicating whether or not the avatar is on fire.
		/// </summary>
		public bool IsInflamed
		{
			get { return this.Properties.GetValue<bool>(IsInflamedProperty); }
			set { this.Properties.SetValue(IsInflamedProperty, value, false); }
		}
		public static readonly string IsInflamedProperty = "IsInflamed";

		/// <summary>	
		/// Gets or sets a value indicating whether or not the avatar is frozen.
		/// </summary>
		public bool IsFrozen
		{
			get { return this.Properties.GetValue<bool>(IsFrozenProperty); }
			set { this.Properties.SetValue(IsFrozenProperty, value, false); }
		}
		public static readonly string IsFrozenProperty = "IsFrozen";

		/// <summary>
		/// Gets a value indicating whether or not the current avatar can perform an action.
		/// </summary>
		public bool CanPerformAction
		{
			get
			{
				return !IsDead && !IsFrozen && !IsInflamed && !IsPoisoned && !IsSnared && !IsStunned && !IsUnconscious;
			}
		}

		/// <summary>
		/// Gets or sets the alias to the item to use for combat or null if pass on combat turn.
		/// </summary>
		public string CombatAction { get; set; }

		/// <summary>
		/// Gets or sets the current match this avatar is participating in.
		/// </summary>
		public Guid CombatMatch
		{
			get { return _combatMatch; }
			set { _combatMatch = value; }
		}
		private Guid _combatMatch = Guid.Empty;

		private object _lock = new object();
		#endregion

		#region Events
		/// <summary>
		/// An event that is raised when the current IAvatar is attacking a target, the skill rolls have been completed and 
		/// the outcome dice have been modified. This event is raised right before a check to the outcome dice to determine 
		/// if the attacker struct its target.
		/// </summary>
		/// <param name="outcomeSuccessDice">The current number of success dice for this attack.</param>
		public virtual void OnAttack(ref int outcomeSuccessDice)
		{
		}

		/// <summary>
		/// Applies protection of the current avatar to the overall damage value.
		/// </summary>
		/// <param name="attacker">The IAvatar attacking the current avatar.</param>
		/// <param name="damage">The current damage value to modify based on protection.</param>
		public virtual void OnApplyProtection(IAvatar attacker, ref int damage)
		{
		}

		/// <summary>
		/// An event that is raised after a skill roll during a spell casting. Can be used to allow the 
		/// caster to always successfully cast the spell.
		/// </summary>
		/// <param name="spell">The spell being cast.</param>
		/// <param name="castSuccessCount">The current successful number of dice rolled for this spell's skill value.</param>
		public virtual void OnCastSkillRoll(ISpell spell, ref int castSuccessCount)
		{
		}

		/// <summary>
		/// An event that is raised after a spell has been cast on the target. Can be used to reset the mind value of the caster.
		/// </summary>
		/// <param name="spell">The spell being cast.</param>
		public virtual void OnCastSuccess(ISpell spell)
		{
		}
		#endregion

		/// <summary>
		/// Initializes a new instance of the Avatar class.
		/// </summary>
		protected Avatar()
		{
			this.Attributes = new AttributeList(this);
			this.Attributes.AttributeAffected += new AttributeAffectedEventHandler(OnAttributeAffected);
			this.Attributes.AttributeFaded += new AttributeAffectFadedEventHandler(OnAttributeFaded);
			this.Affects = new AffectCollection(this);
			this.Skills = new SkillList(this);
			this.Context = new Internal.InternalMessageContext();
			this.Gender = Gender.None;
		}

		#region AttributeList Events
		protected virtual void OnAttributeAffected(AttributeAffectedEventArgs e)
		{
			if (e.Value > 0)
			{
				this.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
					SR.AttributeAffectedIncreased(e.Attribute, e.Value)));
			}
			else if (e.Value < 0)
			{
				this.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Negative,
					SR.AttributeAffectedDecreased(e.Attribute, e.Value)));
			}

			if (e.Attribute == AttributeType.Stamina || e.Attribute == AttributeType.Endurance)
			{
				AttributeList.SetBodyAndMind(this, this.World);

				this.Context.AddRange(this.GetRdlProperties(
					Avatar.BodyProperty,
					Avatar.BodyMaxProperty,
					Avatar.MindProperty,
					Avatar.MindMaxProperty));
			}
		}

		protected virtual void OnAttributeFaded(AttributeAffectFadedEventArgs e)
		{
			this.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
				SR.AttributeFaded(e.Attribute)));

			if (e.Attribute == AttributeType.Stamina || e.Attribute == AttributeType.Endurance)
			{
				AttributeList.SetBodyAndMind(this, this.World);

				this.Context.AddRange(this.GetRdlProperties(
					Avatar.BodyProperty,
					Avatar.BodyMaxProperty,
					Avatar.MindProperty,
					Avatar.MindMaxProperty));
			}
		}
		#endregion

		#region SetMind
		/// <summary>
		/// Sets the value of the Mind score for the current Avatar.
		/// </summary>
		/// <param name="value">The value of the new Mind score.</param>
		public void SetMind(int value)
		{
			this.SetMind(value, this.MindMax);
		}

		/// <summary>
		/// Sets the value and maximum value of the Mind score for the current Avatar.
		/// </summary>
		/// <param name="value">The value of the new Mind score.</param>
		/// <param name="max">The maximum value of the new Mind score.</param>
		public void SetMind(int value, int max)
		{
			if (max != this.MindMax)
			{
				this.MindMax = max;
			}
			if (value > max) value = max;
			this.Mind = value;
		}
		#endregion

		#region CheckHealth
		private void CheckHealth()
		{
			//When a character’s Body score drops past the negative of his physical stamina, the character is dead.
			//When a character reaches 0 or less Body, the character falls unconscious.
			//When a character’s Body score goes below zero, the character will receive another wound each minute from blood
			//loss, etc, until the character receives first aid or other healing. Of course, if the character receives wounds past the
			//negative of his Physical Stamina, he is dead (no first aid can help that).
			if (this.Body <= 0)//-(this.Attributes[AttributeType.Stamina]))
			{
				this.IsDead = true;
			}
			else if (this.Body == 1)//0)
			{
				this.IsUnconscious = true;
			}
			else
			{
				this.IsUnconscious = false;
				this.IsDead = false;
			}
		}
		#endregion

		#region Heartbeat
		/// <summary>
		/// Performs the heartbeat for the current avatar.
		/// </summary>
		public virtual void Heartbeat()
		{
			// Loop through current Affects and remove any that have expired.
			DateTime now = DateTime.Now.ToUniversalTime();
			List<string> removes = new List<string>();
			lock (_lock)
			{
				foreach (var affect in this.Affects)
				{
					var actor = this.GetAllChildren().Where(c => c.Name == affect.Key.Replace(this.Affects.Prefix, String.Empty)).FirstOrDefault();
					if (actor != null)
					{
						if (actor is IAffectItem)
						{
							TimeSpan remainder = now.Subtract(new DateTime(affect.Value));
							if (remainder.TotalMinutes > (actor as IAffectItem).Duration.TotalMinutes)
							{
								// Remove the affect.
								(actor as IAffectItem).RemoveAffect(this);
								removes.Add(affect.Key);
							}
							else
							{
								(actor as IAffectItem).ApplyIncrementAffect(this);
							}
						}
						else if (actor is IItem)
						{
							this.Attributes.RemoveAffects((actor as IItem).Affects);
							this.Context.AddRange(this.Attributes.ToRdl());
							removes.Add(affect.Key);
						}
					}
				}
				foreach (var key in removes)
				{
					// Delete property from avatar instance, remove the value from the collection.
					if (this.World != null)
					{
						this.World.Provider.RemoveProperty<Avatar>(this, key);
					}
				}
			}
		}
		#endregion

		#region GetTotalProtection, ReduceArmorDurability, GetWeapon
		/// <summary>
		/// Gets an IArmor instance that represents the total cumaltive armor/protection rating for the current Actor.
		/// </summary>
		/// <returns>The total protection value of all armor applied to the Actor.</returns>
		public virtual int GetTotalProtection()
		{
			int protection = this.GetAllChildren()
				.Where(a => (a is IArmor && (a as IArmor).IsEquipped && (a as IArmor).Durability > 0))
				.Sum(a => (a as IArmor).Protection);
			protection += this.Protection;
			return protection;
		}

		/// <summary>
		/// Gets an IWeapon instance representing the primary weapon of the Actor.
		/// </summary>
		/// <returns></returns>
		public virtual IWeapon GetWeapon()
		{
			return this.GetAllChildren().Where(a => a is IWeapon && (a as IWeapon).IsEquipped).Select(a => a as IWeapon).FirstOrDefault();
        }

        /// <summary>
        /// Gets an IWeapon instance representing the secondary weapon of the Actor.
        /// </summary>
        /// <returns></returns>
        public virtual IWeapon GetSecondaryWeapon()
        {
            IWeapon weapon = this.GetWeapon();
            return this.GetAllChildren().Where(a => a is IWeapon && (a as IWeapon).IsEquipped && a.ID != weapon.ID).Select(a => a as IWeapon).FirstOrDefault();
        }

        /// <summary>
        /// Gets an IArmor instance representing the primary armor of the actor.
        /// </summary>
        /// <returns></returns>
        public virtual IArmor GetArmor()
        {
            return this.GetAllChildren().Where(a => a is IArmor && (a as IArmor).IsEquipped).OrderByDescending(a => (a as IArmor).Durability).Select(a => a as IArmor).FirstOrDefault();
        }
		#endregion

		#region Messaging

		/// <summary>
		/// Tells the current avatar the specified text from the specified avatar.
		/// </summary>
		/// <param name="from">The avatar sending the tell.</param>
		/// <param name="text">The text of the tell message.</param>
		public void Tell(Avatar from, string text)
		{
			this.Context.Add(new RdlTellMessage(from.Name, text));
		}
		#endregion

		#region RDL
		protected override void AddSimpleRdlProperties(List<RdlObject> list)
		{
			base.AddSimpleRdlProperties(list);
			
			// Need to only include the basic information, not all the properties of the object.
			list.AddRange(this.GetRdlProperties(
				Avatar.GenderProperty,
				Avatar.MindProperty,
				Avatar.MindMaxProperty,
				Avatar.RaceProperty,
				Avatar.XProperty,
				Avatar.YProperty,
				Avatar.ZProperty));
			list.AddRange(this.Attributes.ToRdl());
			foreach (var affect in this.Affects)
			{
				list.Add(new RdlProperty(this.ID, affect.Key, affect.Value));
			}
		}
		#endregion
	}
	#endregion

	#region AffectCollection
	/// <summary>
	/// Represents a list of currently applied actors that affect the owner. The key is the ID of the actor affecting the 
	/// owner and the value is the DateTime the affect was applied.
	/// </summary>
	public class AffectCollection : ActorOwnedDictionaryBase<long>
	{
		public AffectCollection(IActor owner)
			: base(owner, "Affect_")
		{
		}

		public void Add(IItem item, TimeSpan duration)
		{
			this.Add(item.Name, DateTime.Now.ToUniversalTime().Add(duration).Ticks);
		}

		public override bool Remove(string key)
		{
			this.Owner.Properties.Remove(this.GetPrefixedName(key));
			return true;
		}

		public override bool TryGetValue(string key, out long value)
		{
			if (this.ContainsKey(this.GetPrefixedName(key)))
			{
				value = this[this.GetPrefixedName(key)];
				return true;
			}
			value = 0;
			return false;
		}

		public RdlProperty GetRdlProperty(string key)
		{
			return new RdlProperty(this.Owner.ID, this.GetPrefixedName(key), this[this.GetPrefixedName(key)]);
		}

		public RdlProperty[] ToRdl()
		{
			List<RdlProperty> list = new List<RdlProperty>();
			foreach (var item in this)
			{
				list.Add(new RdlProperty(this.Owner.ID, item.Key, item.Value));
			}
			return list.ToArray();
		}
	}
	#endregion

	#region AvatarDictionary
	/// <summary>
	/// Represents a dictionary of avatar instances with the avatar name as the key.
	/// </summary>
	public class AvatarDictionary : Dictionary<string, IAvatar>
	{
		private World _world;

		/// <summary>
		/// Initializes a new instance of the AvatarDictionary class.
		/// </summary>
		public AvatarDictionary(World world)
			: base(new NameComparer())
		{
			_world = world;
		}

		public new void Add(string key, IAvatar value)
		{
			value.World = _world;
			base.Add(key, value);
		}
	}
	#endregion
}
