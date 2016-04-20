using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance
{
	/// <summary>
	/// Represents the abstract base class for all people and creatures within the virtual world.
	/// </summary>
	public abstract class Avatar : Actor
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
		public int Luck { get; set; }

		/// <summary>
		/// Gets or sets the initiative of the current Avatar. This value is set during combat by the game engine.
		/// </summary>
		public int Initiative { get; internal set; }

		/// <summary>
		/// Gets a value indicating whether or not the Avatar has been knocked unconscious.
		/// </summary>
		public bool IsUnconscious { get; private set; }

		/// <summary>
		/// Gets a value indicating whether or not the Avatar is dead.
		/// </summary>
		public bool IsDead { get; private set; }	

		private int _body = 0;
		/// <summary>
		/// Gets or sets a value indicating the physical health of the Avatar.
		/// </summary>
		public int Body
		{
			get { return _body; }
			set
			{
				_body = value;
				this.CheckHealth();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating the mental health of the Avatar.
		/// </summary>
		public int Mind { get; set; }

		/// <summary>
		/// Gets or sets the value of any magical protection applied to the avatar.
		/// </summary>
		public int Protection { get; private set; }

		/// <summary>
		/// Gets the place where the current avatar resides.
		/// </summary>
		public Place Place { get; internal set; }

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
		public string FirstName { get; set; }

		/// <summary>
		/// Gets or sets the last name of the avatar.
		/// </summary>
		public string LastName { get; set; }
		#endregion

		/// <summary>
		/// Initializes a new instance of the Avatar class.
		/// </summary>
		protected Avatar()
		{
			this.Attributes = new AttributeList();
			this.Skills = new SkillList();
		}

		#region CheckHealth
		private void CheckHealth()
		{
			//When a character’s Body score drops past the negative of his physical stamina, the character is dead.
			//When a character reaches 0 or less Body, the character falls unconscious.
			//When a character’s Body score goes below zero, the character will receive another wound each minute from blood
			//loss, etc, until the character receives first aid or other healing. Of course, if the character receives wounds past the
			//negative of his Physical Stamina, he is dead (no first aid can help that).
			if (this.Body <= -(this.Attributes.GetValue(AttributeType.Stamina)))
			{
				this.IsDead = false;
			}
			else if (this.Body == 0)
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

		#region AddProtection
		/// <summary>
		/// Adds magical protection to the current avatar of the specified value and for the specified duration.
		/// </summary>
		/// <param name="value">The value to add to protection.</param>
		/// <param name="duration">The duration of the added protection.</param>
		public void AddProtection(int value, DurationType duration)
		{
			// TODO: Keep track of protection values added, along with the date they were added.
			// On avatar heartbeat check and remove any of these protection values.
			// this.ProtectionList.Add(value, DateTime.Now);
		}
		#endregion

		#region Armor Rating
		/// <summary>
		/// Gets the ArmorRating of the current Avatar, including any affects from items or spells that are applied to the avatar.
		/// </summary>
		/// <returns>The ArmorRating of the current Avatar.</returns>
		public ArmorRating GetArmorRating()
		{
			ArmorRating rating = new ArmorRating();
			// TODO: Collect the armor rating from each piece of equipped armor.

			// Append the protection value from buffs, items, etc.
			rating.AbsorptionRating += this.Protection;
			return rating;
		}

		/// <summary>
		/// Reduces the coverage rating of a random piece of armor by one.
		/// </summary>
		public void ReduceArmorCoverageRating()
		{
			// TODO: Reduce a random piece of equipped armor's coverage rating, unless already 0.
		}
		#endregion

		#region Messaging
		/// <summary>
		/// Adds an output tag specific to the current avatar. In the case of a player instance the tag might 
		/// contain the results of an action, combat, chat, etc.
		/// </summary>
		/// <param name="tag">The Tag instance to add.</param>
		public virtual void AddTag(RdlTag tag)
		{
		}

		/// <summary>
		/// Tells the current avatar the specified text from the specified avatar.
		/// </summary>
		/// <param name="from">The avatar sending the tell.</param>
		/// <param name="text">The text of the tell message.</param>
		public virtual void Tell(Avatar from, string text)
		{
		}
		#endregion
	}
}
