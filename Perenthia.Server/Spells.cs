using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia
{
	#region MindSpell
	public class MindSpell : Spell
	{
		public MindSpell()
			: base()
		{
		}

		public MindSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.Duration = TimeSpan.FromMinutes(5);
			this.AffectType = AffectType.Mind;
		}

		public override void Cast(IAvatar caster, IActor target, CastResults results)
		{
			IAvatar defender = target as IAvatar;

			// Do not heal creatures or NPCs
			if (defender is Creature || defender is Npc)
			{
				target = caster;
				defender = caster;
			}

			// Heal the mind value of the target.
	        if (defender != null)
	        {
	            if (defender.Mind == defender.MindMax)
				{
					this.NoAffect(caster);
					this.NoAffect(defender);
	            }
	            else
	            {
					int power = this.Foci.Power + results.CastSuccessCount;
					defender.SetMind(defender.Mind + power);
	                if (Object.ReferenceEquals(caster, defender))
	                {
	                    // Target is self, only need a message to the caster.
	                    caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
							String.Format(Resources.SpellWillpowerGained, power)));
	                }
	                else
	                {
	                    // Need to inform both the caster the target of healing.
						caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
							String.Format(Resources.SpellWillpowerGainedTarget, power, target.The())));
						defender.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
							String.Format(Resources.SpellWillpowerGainedByTarget, power, caster.A())));
	                }
	            }
	        }
	        else
			{
				this.NoAffect(caster);
	        }
		}
	}
	#endregion

	#region DamageSpell
	public class DamageSpell : Spell
	{
		public DamageSpell()
			: base()
		{
		}

		public DamageSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.IsDamageSpell = true;
			this.AffectType = AffectType.Damage;
		}

		public override void Cast(IAvatar caster, IActor target, CastResults results)
		{
			IAvatar defender = target as IAvatar;

			// TODO: Defense against spells?
			int power = this.Foci.Power + results.CastSuccessCount;
			target.SetBody(target.Body - power);

			caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
				String.Format(Resources.SpellDamagedTarget, target.A(), power)));
			if (defender != null) defender.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Negative,
				String.Format(Resources.SpellDamagedByTarget, power, caster.A())));
			if (defender != null) defender.Context.AddRange(defender.GetRdlProperties(Avatar.BodyProperty));

			if (target.IsDead)
			{
				results.TargetDied = true;
				if (defender != null)
				{
					// Killed an Avatar.
					caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
						String.Format(Resources.SpellKilledTarget, target.A())));
					defender.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Negative,
						String.Format(Resources.SpellKilledByTarget, caster.A())));
				}
				else
				{
					// Destroyed an object.
					caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
						String.Format(Resources.SpellDestroyedTraget, target.A())));
				}
			}
			else if (defender != null && defender.IsUnconscious)
			{
				results.TargetUnconscious = true;
				caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
					String.Format(Resources.SpellUnconsciousTarget, target.A())));
				if (defender != null) defender.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Negative,
					String.Format(Resources.SpellUnconsciousByTarget, caster.A())));
			}
		}
	}
	#endregion

	#region HealSpell
	public class HealSpell : Spell
	{
		public HealSpell()
			: base()
		{
		}

		public HealSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.AffectType = AffectType.Heal;
		}

		public override void Cast(IAvatar caster, IActor target, CastResults results)
		{
			IAvatar defender = target as IAvatar;

			// Do not heal creatures or NPCs
			if (defender is PerenthiaMobile)
			{
				target = caster;
				defender = caster;
			}

			// Heal or increase the body value of the target.
			if (target.Body == target.BodyMax)
			{
				this.NoAffect(caster);
				if (defender != null) this.NoAffect(defender);
			}
			else
			{
				int power = this.Foci.Power + results.CastSuccessCount;
				target.SetBody(target.Body + power);
				if (Object.ReferenceEquals(caster, target))
				{
					// Target is self, only need a message to the caster.
					caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
						String.Format(Resources.SpellHealthGained, power)));
				}
				else
				{
					// Need to inform both the caster the target of healing.
					caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
						String.Format(Resources.SpellHealthGainedTarget, power, target.The())));
					if (defender != null) defender.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
						String.Format(Resources.SpellHealthGainedByTarget, power, caster.A())));
				}
			}
		}
	}
	#endregion

	#region ProtectionSpell
	public class ProtectionSpell : Spell
	{
		public ProtectionSpell()
			: base()
		{
		}

		public ProtectionSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.Duration = TimeSpan.FromMinutes(5);
			this.AffectType = AffectType.Protection;
		}

		public override void Cast(IAvatar caster, IActor target, CastResults results)
		{
			IAvatar defender = target as IAvatar;

			// Increase the protection value of the target.
			if (defender != null)
			{
                if (defender is PerenthiaMobile) defender = caster;

				int power = this.Foci.Power + results.CastSuccessCount;
				this.AffectPower = power;
				this.Save();

                // If a protection spell already exists using the same skill then remove it
                // and add the current one.
                // Search the list of spells with the same skill and try to find those spells in the
                // affects collection.
                var spellNames = defender.GetAllChildren().Where(c => c is ISpell && (c as ISpell).Skill == this.Skill).Select(c => c.Name);
                foreach (var spellName in spellNames)
                {
                    // If this protection already exists then remove it and add the new one.
                    if (defender.Affects.ContainsKey(spellName))
                    {
                        this.RemoveAffect(defender, false);
                    }
                }

				defender.Protection += power;
				defender.Affects.Add(this, this.Foci.Duration);
				defender.Context.AddRange(defender.Affects.ToRdl());
				if (Object.ReferenceEquals(caster, defender))
				{
					caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
						String.Format(Resources.SpellProtectionGained, power)));
				}
				else
				{
					caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
						String.Format(Resources.SpellProtectionGainedTarget, target.A(), power)));
					defender.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
						String.Format(Resources.SpellProtectionGainedByTarget, power, caster.A())));
					if (defender != null) defender.Context.AddRange(defender.GetRdlProperties(Avatar.ProtectionProperty));
				}
			}
			else
			{
				this.NoAffect(caster);
			}
		}

        private void RemoveAffect(IAvatar avatar, bool wornOff)
        {
            int power = this.AffectPower;
            if (power == 0) power = this.Foci.Power;
            avatar.Protection -= power;
            if (wornOff)
            {
                avatar.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
                    String.Format("The affects of the {0} spell have worn off.", this.Name)));
            }
            avatar.Context.AddRange(avatar.GetRdlProperties(Avatar.ProtectionProperty));
            avatar.Context.Add(new RdlCommand("AFFECTREMOVE", String.Concat(avatar.Affects.Prefix, this.Name)));
        }

		public override void RemoveAffect(IAvatar avatar)
		{
            this.RemoveAffect(avatar, true);
		}
	}
	#endregion

	#region StunSpell
	public class StunSpell : Spell
	{
		public StunSpell()
			: base()
		{
		}

		public StunSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.Duration = TimeSpan.FromSeconds(15);
			this.AffectType = AffectType.Stun;
		}
	}
	#endregion

	#region SnareSpell
	public class SnareSpell : Spell
	{
		public SnareSpell()
			: base()
		{
		}

		public SnareSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.Duration = TimeSpan.FromSeconds(15);
			this.AffectType = AffectType.Snare;
		}
	}
	#endregion

	#region StatSpell
	public abstract class StatSpell : Spell
	{
		protected abstract AttributeType StatName { get; }

		protected StatSpell()
			: base()
		{
		}

		protected StatSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.Duration = TimeSpan.FromMinutes(5);
		}

		public override void Cast(IAvatar caster, IActor target, CastResults results)
		{
			IAvatar defender = target as IAvatar;

			if (defender != null)
			{
                if (defender is PerenthiaMobile) defender = caster;

				int power = this.Foci.Power + results.CastSuccessCount;
				defender.Attributes.ApplyAffect(this.StatName, power, this.Foci.Duration);
				defender.Context.AddRange(defender.Affects.ToRdl());
				if (Object.ReferenceEquals(caster, defender))
				{
					caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
						String.Format(Resources.SpellStatGained, this.StatName, power)));
				}
				else
				{
					caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
						String.Format(Resources.SpellStatGainedTarget, this.StatName, defender.A(), power)));
					defender.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
						String.Format(Resources.SpellStatGainedByTarget, caster.A(), this.StatName, power)));
					defender.Context.AddRange(defender.Attributes.ToRdl());
				}
			}
			else
			{
				this.NoAffect(caster);
			}
		}

		public override void RemoveAffect(IAvatar avatar)
		{
			avatar.Attributes.RemoveAffects(this.Affects);
			avatar.Context.AddRange(avatar.Attributes.ToRdl());
			avatar.Context.Add(new RdlCommand("AFFECTREMOVE", this.ID));
		}
	}
	#endregion

	#region StrengthSpell
	public class StrengthSpell : StatSpell
	{
		protected override AttributeType StatName
		{
			get { return AttributeType.Strength; }
		}

		public StrengthSpell()
			: base()
		{
		}

		public StrengthSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.AffectType = AffectType.Strength;
		}
	}
	#endregion

	#region StaminaSpell
	public class StaminaSpell : StatSpell
	{
		protected override AttributeType StatName
		{
			get { return AttributeType.Stamina; }
		}

		public StaminaSpell()
			: base()
		{
		}

		public StaminaSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.AffectType = AffectType.Stamina;
		}
	}
	#endregion

	#region DexteritySpell
	public class DexteritySpell : StatSpell
	{
		protected override AttributeType StatName
		{
			get { return AttributeType.Dexterity; }
		}

		public DexteritySpell()
			: base()
		{
		}

		public DexteritySpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.AffectType = AffectType.Dexterity;
		}
	}
	#endregion

	#region BeautySpell
	public class BeautySpell : StatSpell
	{
		protected override AttributeType StatName
		{
			get { return AttributeType.Beauty; }
		}

		public BeautySpell()
			: base()
		{
		}

		public BeautySpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.AffectType = AffectType.Beauty;
		}
	}
	#endregion

	#region PerceptionSpell
	public class PerceptionSpell : StatSpell
	{
		protected override AttributeType StatName
		{
			get { return AttributeType.Perception; }
		}

		public PerceptionSpell()
			: base()
		{
		}

		public PerceptionSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.AffectType = AffectType.Perception;
		}
	}
	#endregion

	#region IntelligenceSpell
	public class IntelligenceSpell : StatSpell
	{
		protected override AttributeType StatName
		{
			get { return AttributeType.Intelligence; }
		}

		public IntelligenceSpell()
			: base()
		{
		}

		public IntelligenceSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.AffectType = AffectType.Intelligence;
		}
	}
	#endregion

	#region EnduranceSpell
	public class EnduranceSpell : StatSpell
	{
		protected override AttributeType StatName
		{
			get { return AttributeType.Endurance; }
		}

		public EnduranceSpell()
			: base()
		{
		}

		public EnduranceSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.AffectType = AffectType.Endurance;
		}
	}
	#endregion

	#region AffinitySpell
	public class AffinitySpell : StatSpell
	{
		protected override AttributeType StatName
		{
			get { return AttributeType.Affinity; }
		}

		public AffinitySpell()
			: base()
		{
		}

		public AffinitySpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.AffectType = AffectType.Affinity;
		}
	}
	#endregion

	#region SightSpell
	public class SightSpell : Spell
	{
		public SightSpell()
			: base()
		{
		}

		public SightSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.Duration = TimeSpan.FromMinutes(5);
		}

		public override void Cast(IAvatar caster, IActor target, CastResults results)
		{
			Character player = caster as Character;
			if (player == null) player = target as Character;
			if (player != null)
			{
				player.SightRange = this.Foci.Range;
				player.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast, this.SuccessMessage));
			}
			else
			{
				this.NoAffect(caster);
			}
		}
	}
	#endregion

	#region CreateItemSpell
	public class CreateItemSpell : Spell
	{
		public string ItemTypeName
		{
			get { return this.Properties.GetValue<string>(ItemTypeNameProperty); }
			set { this.Properties.SetValue(ItemTypeNameProperty, value); }
		}
		public static readonly string ItemTypeNameProperty = "ItemTypeName";

		public string ItemName
		{
			get { return this.Properties.GetValue<string>(ItemNameProperty); }
			set { this.Properties.SetValue(ItemNameProperty, value); }
		}
		public static readonly string ItemNameProperty = "ItemName";	

		public CreateItemSpell()
			: base()
		{
		}

		public CreateItemSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		public override void Cast(IAvatar caster, IActor target, CastResults results)
		{
			if (!String.IsNullOrEmpty(this.ItemTypeName))
			{
				Item item = this.World.CreateActor(Type.GetType(this.ItemTypeName), this.ItemName) as Item;
				if (item != null)
				{
					if (caster is Character)
					{
						Container container = (caster as Character).GetFirstAvailableContainer(item);
						if (container != null)
						{
							item.Drop(container);
							item.Save();

							caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
								String.Format(Resources.ItemCreated, item.A())));
						}
						else
						{
							caster.Context.Add(new RdlErrorMessage(Resources.InventoryFull));
						}
					}
					else
					{
						item.Drop(caster);
					}
				}
				else
				{
					caster.Context.Add(new RdlErrorMessage(Resources.CastFailed));
				}
			}
		}
	}
	#endregion

	#region TravelSpell
	public class TravelSpell : Spell
	{
		public int DestinationX
		{
			get { return this.Properties.GetValue<int>(DestinationXProperty); }
			set { this.Properties.SetValue(DestinationXProperty, value); }
		}
		public static readonly string DestinationXProperty = "DestinationX";

		public int DestinationY
		{
			get { return this.Properties.GetValue<int>(DestinationYProperty); }
			set { this.Properties.SetValue(DestinationYProperty, value); }
		}
		public static readonly string DestinationYProperty = "DestinationY";

		public int DestinationZ
		{
			get { return this.Properties.GetValue<int>(DestinationZProperty); }
			set { this.Properties.SetValue(DestinationZProperty, value); }
		}
		public static readonly string DestinationZProperty = "DestinationZ";

		public TravelSpell()
			: base()
		{
		}

		public TravelSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		public override void Cast(IAvatar caster, IActor target, CastResults results)
		{
			IAvatar traveler = target as IAvatar;
			if (traveler != null)
			{
				Place place = this.World.FindPlace(new Point3(this.DestinationX, this.DestinationY, this.DestinationZ));
				if (place != null && place.HasExits())
				{
					place.Enter(traveler, Direction.Empty);

					if (Object.ReferenceEquals(caster, traveler))
					{
						caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
							String.Format(Resources.SpellTransported, place.Name)));
					}
					else
					{
						caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
							String.Format(Resources.SpellTransportedTarget, traveler.The(), place.Name)));
						traveler.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
							String.Format(Resources.SpellTransportedByTarget, place.Name)));
					}
				}
				else
				{
					this.NoAffect(caster);
				}
			}
			else
			{
				this.NoAffect(caster);
			}
		}
	}
	#endregion

	#region EnchantSpell
	public abstract class EnchantSpell : Spell
	{
		protected abstract string EnchantPropertyName { get; }

		public EnchantSpell()
			: base()
		{
		}

		public EnchantSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}

		public override void Cast(IAvatar caster, IActor target, CastResults results)
		{
			Item item = target as Item;
			if (item != null)
			{
				item.Properties.SetValue(this.EnchantPropertyName, this.Power);
				caster.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Cast,
					String.Format(Resources.SpellEnchanted, item.A())));
			}
			else
			{
				this.NoAffect(caster);
			}
		}
	}
	#endregion

	#region EnchantWeaponSpell
	public class EnchantWeaponSpell : EnchantSpell
	{
		protected override string EnchantPropertyName
		{
			get { return "Power"; }
		}

		public EnchantWeaponSpell()
			: base()
		{
		}

		public EnchantWeaponSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}
	}
	#endregion

	#region EnchantPendantSpell
	public class EnchantPendantSpell : EnchantSpell
	{
		protected override string EnchantPropertyName
		{
			get { return "Protection"; }
		}

		public EnchantPendantSpell()
			: base()
		{
		}

		public EnchantPendantSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}
	}
	#endregion

	#region EnchantRingSpell
	public class EnchantRingSpell : EnchantSpell
	{
		protected override string EnchantPropertyName
		{
			get { return "Protection"; }
		}

		public EnchantRingSpell()
			: base()
		{
		}

		public EnchantRingSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}
	}
	#endregion

	#region EnchantShieldSpell
	public class EnchantShieldSpell : EnchantSpell
	{
		protected override string EnchantPropertyName
		{
			get { return "Protection"; }
		}

		public EnchantShieldSpell()
			: base()
		{
		}

		public EnchantShieldSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}
	}
	#endregion

	#region EnchantArmorSpell
	public class EnchantArmorSpell : EnchantSpell
	{
		protected override string EnchantPropertyName
		{
			get { return "Protection"; }
		}

		public EnchantArmorSpell()
			: base()
		{
		}

		public EnchantArmorSpell(string name, string description, int power, int range)
			: base(name, description, power, range)
		{
		}
	}
	#endregion
}
