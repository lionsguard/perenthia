using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance
{
	/// <summary>
	/// Represents a list of attributes common to all avatars.
	/// </summary>
	public class AttributeList : IEnumerable<KeyValuePair<string, int>>
	{
		#region Static Members
		internal static readonly int MaxAttributePoints = 40;
		internal static readonly string Prefix = "Attr_";
		internal static readonly string AffectPrefix = "AttrAffect_";
		internal static readonly string AffectAppliedPrefix = "AttrAffectApplied_";
		internal static readonly string AffectDurationPrefix = "AttrAffectDuration_";

		public static readonly Dictionary<string, string> Descriptions;

		static AttributeList()
		{
			Descriptions = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			Descriptions.Add(AttributeType.Strength.ToString(), "Physical strength reflects the raw muscle power of the character. It affects damage dealt with some melee weapons, ability to lift heavy objects, and other feats of strength.");
			Descriptions.Add(AttributeType.Dexterity.ToString(), "Physical dexterity reflects the reflexes and agility of the character. It affects some combat skills and other feats requiring nimbleness and quickness.");
			Descriptions.Add(AttributeType.Stamina.ToString(), "Physical stamina reflects the health and robustness of the character. It affects how much damage can be taken by the character, the carrying of heavy loads, running, and other forms of long-term physical exertion.");
			Descriptions.Add(AttributeType.Beauty.ToString(), "Physical beauty is a gauge of the attractiveness of the character. It affects reactions of other characters that are based on appearance.");
			Descriptions.Add(AttributeType.Intelligence.ToString(), "Intelligence is a measure of the raw mental power of the character. It affects the speaking of foreign languages, repair and construction of objects, and other such skills. Intelligence can more of less be regarded as 'mental strength'.");
			Descriptions.Add(AttributeType.Perception.ToString(), "Perception is a measure of the characters awareness. It affects noticing details, finding traps (though physical dexterity or Intelligence might be used to disarm them), ranged combat, and other skills involving quick or precise mental reflexes. Perception can be regarded as 'mental reflexes'.");
			Descriptions.Add(AttributeType.Endurance.ToString(), "Mental endurance is a measure of the level of mental fortitude and sanity. This attribute would affect things like the ability to resist interrogation or torture. This attribute will also affect the amount of raw magical power available to the character.");
			Descriptions.Add(AttributeType.Affinity.ToString(), "Mental affinity reflects the force of personality of the character. It affects reactions during conversation, calming of frightened animals, the ability to command effectively, and other skills requiring a charismatic or forceful personality.");
		}

		public static void SetBodyAndMind(IAvatar avatar, World world)
		{
			int body = avatar.Attributes.Stamina * world.RealismMultiplier;
			int mind = avatar.Attributes.Endurance * world.PowerMultiplier;
			avatar.SetBody(body, body);
			avatar.SetMind(mind, mind);
		}
		#endregion

		/// <summary>
		/// Gets or sets the strength attribute value.
		/// </summary>
		public int Strength
		{
			get { return this.GetModifiedValue(AttributeType.Strength); }
			set { this.SetValue(AttributeType.Strength, value); }
		}
		/// <summary>
		/// Gets or sets the dexterity attribute value.
		/// </summary>
		public int Dexterity
		{
			get { return this.GetModifiedValue(AttributeType.Dexterity); }
			set { this.SetValue(AttributeType.Dexterity, value); }
		}
		/// <summary>
		/// Gets or sets the stamina attribute value.
		/// </summary>
		public int Stamina
		{
			get { return this.GetModifiedValue(AttributeType.Stamina); }
			set { this.SetValue(AttributeType.Stamina, value); }
		}
		/// <summary>
		/// Gets or sets the beauty attribute value.
		/// </summary>
		public int Beauty
		{
			get { return this.GetModifiedValue(AttributeType.Beauty); }
			set { this.SetValue(AttributeType.Beauty, value); }
		}
		/// <summary>
		/// Gets or sets the intelligence attribute value.
		/// </summary>
		public int Intelligence
		{
			get { return this.GetModifiedValue(AttributeType.Intelligence); }
			set { this.SetValue(AttributeType.Intelligence, value); }
		}
		/// <summary>
		/// Gets or sets the perception attribute value.
		/// </summary>
		public int Perception
		{
			get { return this.GetModifiedValue(AttributeType.Perception); }
			set { this.SetValue(AttributeType.Perception, value); }
		}
		/// <summary>
		/// Gets or sets the endurance attribute value.
		/// </summary>
		public int Endurance
		{
			get { return this.GetModifiedValue(AttributeType.Endurance); }
			set { this.SetValue(AttributeType.Endurance, value); }
		}
		/// <summary>
		/// Gets or sets the affinity attribute value.
		/// </summary>
		public int Affinity
		{
			get { return this.GetModifiedValue(AttributeType.Affinity); }
			set { this.SetValue(AttributeType.Affinity, value); }
		}

		/// <summary>
		/// Gets or sets the value of the attribute specified by the type.
		/// </summary>
		public int this[AttributeType type]
		{
			get { return this.GetModifiedValue(type); }
			set { this.SetValue(type, value); }
		}

		/// <summary>
		/// Gets or sets the value of the attribute with the specified name.
		/// </summary>
		public int this[string attribute]
		{
			get
			{
				AttributeType type = (AttributeType)Enum.Parse(typeof(AttributeType), attribute, true);
				return this.GetModifiedValue(type);
			}
			set
			{
				AttributeType type = (AttributeType)Enum.Parse(typeof(AttributeType), attribute, true);
				this.SetValue(type, value);
			}
		}

		/// <summary>
		/// Gets or sets the owner of the attributes contained within the list.
		/// </summary>
		public Actor Owner { get; set; }

		/// <summary>
		/// An event that is raised when the affect applied to an attribute has faded.
		/// </summary>
		public event AttributeAffectFadedEventHandler AttributeFaded = delegate { };

		/// <summary>
		/// An event that is raised when an affect is applied to an attribute.
		/// </summary>
		public event AttributeAffectedEventHandler AttributeAffected = delegate { };

		/// <summary>
		/// Initializes a new instance of the AttributeList class.
		/// </summary>
		public AttributeList(Actor owner)
		{
			this.Owner = owner;
		}

		/// <summary>
		/// Applies an affect to the specified stat for the specified duration.
		/// </summary>
		/// <param name="type">The type of stat to apply the affect.</param>
		/// <param name="value">The value of the affect to apply.</param>
		/// <param name="duration">The amount of time that affect will last.</param>
		public void ApplyAffect(AttributeType type, int value, TimeSpan duration)
		{
			this.Owner.Properties.SetValue(this.GetAffectName(type), value);
			this.Owner.Properties.SetValue(this.GetAffectAppliedName(type), DateTime.Now.ToUniversalTime().Ticks);
			this.Owner.Properties.SetValue(this.GetAffectDurationName(type), duration.Ticks);

			if (value != 0)
			{
				this.AttributeAffected(new AttributeAffectedEventArgs(type, value));
			}
		}

		/// <summary>
		/// Applies the values from the specified AttributeList to the current attributes.
		/// </summary>
		/// <param name="affects">The affects to apply.</param>
		public void ApplyAffects(AttributeList affects)
		{
			foreach (var affect in affects.Where(a => a.Value != 0))
			{
				this.ApplyAffect((AttributeType)Enum.Parse(typeof(AttributeType), affect.Key, true), affect.Value, TimeSpan.Zero);
			}
		}

		public void RemoveAffects(AttributeList affects)
		{
			foreach (var affect in affects.Where(a => a.Value != 0))
			{
				AttributeType type = (AttributeType)Enum.Parse(typeof(AttributeType), affect.Key, true);

				this.Owner.Properties.SetValue(this.GetAffectName(type), 0);
				this.Owner.Properties.SetValue(this.GetAffectAppliedName(type), DateTime.Now.ToUniversalTime().AddDays(-1).Ticks);
				this.Owner.Properties.SetValue(this.GetAffectDurationName(type), TimeSpan.Zero.Ticks);

				this.AttributeFaded(new AttributeAffectFadedEventArgs(type));
			}
		}

		private int GetAffectValue(AttributeType type)
		{
			int value = this.Owner.Properties.GetValue<int>(this.GetAffectName(type));
			if (value > 0)
			{
				long appliedTicks = this.Owner.Properties.GetValue<long>(this.GetAffectAppliedName(type));
				long durationTicks = this.Owner.Properties.GetValue<long>(this.GetAffectDurationName(type));
				if (appliedTicks > 0 && durationTicks > 0)
				{
					DateTime now = DateTime.Now;
					DateTime appliedTime = new DateTime(appliedTicks);
					TimeSpan duration = TimeSpan.FromTicks(durationTicks);
					TimeSpan remainder = now.Subtract(appliedTime);
					if (remainder.TotalSeconds > duration.TotalSeconds)
					{
						value = 0;
						this.ApplyAffect(type, 0, TimeSpan.FromTicks(0));
						this.AttributeFaded(new AttributeAffectFadedEventArgs(type));
					}
				}
			}
			return value;
		}

		private int GetValue(AttributeType type)
		{
			return this.Owner.Properties.GetValue<int>(this.GetName(type));
		}

		private int GetModifiedValue(AttributeType type)
		{
			return this.GetValue(type) + this.GetAffectValue(type);
		}

		private void SetValue(AttributeType type, int value)
		{
			this.Owner.Properties.SetValue(this.GetName(type), value);
		}

		private string GetName(AttributeType type)
		{
			return String.Concat(Prefix, type.ToString());
		}

		private string GetAffectName(AttributeType type)
		{
			return String.Concat(AffectPrefix, type.ToString());
		}

		private string GetAffectAppliedName(AttributeType type)
		{
			return String.Concat(AffectAppliedPrefix, type.ToString());
		}

		private string GetAffectDurationName(AttributeType type)
		{
			return String.Concat(AffectDurationPrefix, type.ToString());
		}

		/// <summary>
		/// Gets an array of RdlProperty tags for the current attribute list.
		/// </summary>
		/// <returns>An array of RdlProperty tags for the current attribute list.</returns>
		public RdlProperty[] ToRdl()
		{
			List<RdlProperty> list = new List<RdlProperty>();
			list.Add(new RdlProperty(this.Owner.ID, this.GetName(AttributeType.Strength), this.Strength));
			list.Add(new RdlProperty(this.Owner.ID, this.GetName(AttributeType.Dexterity), this.Dexterity));
			list.Add(new RdlProperty(this.Owner.ID, this.GetName(AttributeType.Stamina), this.Stamina));
			list.Add(new RdlProperty(this.Owner.ID, this.GetName(AttributeType.Beauty), this.Beauty));
			list.Add(new RdlProperty(this.Owner.ID, this.GetName(AttributeType.Intelligence), this.Intelligence));
			list.Add(new RdlProperty(this.Owner.ID, this.GetName(AttributeType.Perception), this.Perception));
			list.Add(new RdlProperty(this.Owner.ID, this.GetName(AttributeType.Endurance), this.Endurance));
			list.Add(new RdlProperty(this.Owner.ID, this.GetName(AttributeType.Affinity), this.Affinity));
			return list.ToArray();
		}

		#region IEnumerable<KeyValuePair<string,int>> Members

		public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
		{
			Dictionary<string, int> values = new Dictionary<string, int>();
			values.Add(AttributeType.Strength.ToString(), this.Strength);
			values.Add(AttributeType.Dexterity.ToString(), this.Dexterity);
			values.Add(AttributeType.Stamina.ToString(), this.Stamina);
			values.Add(AttributeType.Beauty.ToString(), this.Beauty);
			values.Add(AttributeType.Intelligence.ToString(), this.Intelligence);
			values.Add(AttributeType.Perception.ToString(), this.Perception);
			values.Add(AttributeType.Endurance.ToString(), this.Endurance);
			values.Add(AttributeType.Affinity.ToString(), this.Affinity);
			return values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}

	public delegate void AttributeAffectFadedEventHandler(AttributeAffectFadedEventArgs e);
	public class AttributeAffectFadedEventArgs : EventArgs
	{
		public AttributeType Attribute { get; set; }

		public AttributeAffectFadedEventArgs(AttributeType attribute)
		{
			this.Attribute = attribute;
		}
	}

	public delegate void AttributeAffectedEventHandler(AttributeAffectedEventArgs e);
	public class AttributeAffectedEventArgs : EventArgs
	{
		public AttributeType Attribute { get; set; }
		public int Value { get; set; }	

		public AttributeAffectedEventArgs(AttributeType attribute, int value)
		{
			this.Attribute = attribute;
			this.Value = value;
		}
	}
}
