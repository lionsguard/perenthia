using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
	/// <summary>
	/// Defines the types of objects used within the virtual world.
	/// </summary>
	public enum ObjectType
	{
		/// <summary>
		/// Specifies the default Actor object.
		/// </summary>
		Actor	= 0,
		/// <summary>
		/// Specifies a computer controlled avatar object.
		/// </summary>
		Mobile	= 1,
		/// <summary>
		/// Specifies a player controlled avatar object.
		/// </summary>
		Player	= 2,
		/// <summary>
		/// Specifies a place object.
		/// </summary>
		Place	= 3,
		/// <summary>
		/// Specifies a race object.
		/// </summary>
		Race	= 4,
        /// <summary>
        /// Specifies an award or achievement.
        /// </summary>
        Award   = 5,
        /// <summary>
        /// Specifies a quest or mission.
        /// </summary>
        Quest   = 6,
	}

	/// <summary>
	/// Provides an enumeration of gender types.
	/// </summary>
	public enum Gender
	{
		/// <summary>
		/// Specifies that a gender is not selected or does not apply.
		/// </summary>
		None	= 0,
		/// <summary>
		/// Specifies a male.
		/// </summary>
		Male	= 1,
		/// <summary>
		/// Specifies a female.
		/// </summary>
		Female	= 2,
	}

	/// <summary>
	/// Defines the types of attributes common to all Avatars.
	/// </summary>
	public enum AttributeType
	{
		/// <summary>
		/// Specifies the strength attribute.
		/// </summary>
		Strength		= 0,
		/// <summary>
		/// Specifies the dexterity attribute.
		/// </summary>
		Dexterity		= 1,
		/// <summary>
		/// Specifies the stamina attribute.
		/// </summary>
		Stamina			= 2,
		/// <summary>
		/// Specifies the beauty attribute.
		/// </summary>
		Beauty			= 3,
		/// <summary>
		/// Specifies the intelligence attribute.
		/// </summary>
		Intelligence	= 4,
		/// <summary>
		/// Specifies the perception attribute.
		/// </summary>
		Perception		= 5,
		/// <summary>
		/// Specifies the endurance attribute.
		/// </summary>
		Endurance		= 6,
		/// <summary>
		/// Specifies the affinity attribute.
		/// </summary>
		Affinity		= 7,
	}

	/// <summary>
	/// Defines an enumeration of values of an attribute.
	/// </summary>
	public enum AttributeValue
	{
		/// <summary>
		/// Specifies the attribute has no value or is not used.
		/// </summary>
		None			= 0, // Do Not Use
		/// <summary>
		/// Specifies a futile attribute level.
		/// </summary>
		Futile			= 1, // Do Not Use
		/// <summary>
		/// Specifies a very poor attribute level.
		/// </summary>
		VeryPoor		= 2, // Do Not Use
		/// <summary>
		/// Specifies the strength of a small child or the intelligence of a smart pet. Effectively retarded in this area.
		/// </summary>
		Bad				= 3,
		/// <summary>
		/// Specifies less than the normal atribute level.
		/// </summary>
		BelowAverage	= 4,
		/// <summary>
		/// Specifies the standard human attribute level.
		/// </summary>
		Average			= 5,
		/// <summary>
		/// Specifies the strength of a lumber jack, or the stamina of a long-distance runner.
		/// </summary>
		AboveAverage	= 6,
		/// <summary>
		/// Specifies the best a normal human can hope to achieve; the intelligence of Einstein or the dexterity of Houdini.
		/// </summary>
		Excellent		= 7,
	}

	/// <summary>
	/// Provides an enumration of initiatives used for combat.
	/// </summary>
	public enum InitiativeType
	{
		/// <summary>
		/// Specifies no initiative, combat participants take turns in the order they are listed.
		/// </summary>
		None		= 0,
		/// <summary>
		/// Specifies that with each turn a new initiative is rolled for each participant.
		/// </summary>
		Individual	= 1,
		/// <summary>
		/// Specifies that initiative is rolled once for each participate before combat but not re-rolled with each turn.
		/// </summary>
		Cyclic		= 2
	}

	/// <summary>
	/// Provides an enumeration of article types used for displaying names in sentences.
	/// </summary>
	public enum ArticleType
	{
		/// <summary>
		/// Specifies that an article should not be included.
		/// </summary>
		None,
		/// <summary>
		/// Specifies "a" or "an" should be included with the name.
		/// </summary>
		A,
		/// <summary>
		/// Specifies "the" should be included with the name.
		/// </summary>
		The,
		/// <summary>
		/// Specifies a possessive should be included with the name.
		/// </summary>
		Your,
	}

	/// <summary>
	/// Provides an enumeration of known directions.
	/// </summary>
	public enum KnownDirection
	{
		/// <summary>
		/// Specified no direction or non movement.
		/// </summary>
		None		= 0,
		/// <summary>
		/// Specifies a northward direction.
		/// </summary>
		North		= 1,
		/// <summary>
		/// Specifies a southward direction.
		/// </summary>
		South		= 2,
		/// <summary>
		/// Specifies a eastward direction.
		/// </summary>
		East		= 3,
		/// <summary>
		/// Specifies a westward direction.
		/// </summary>
		West		= 4,
		/// <summary>
		/// Specifies a northeastward direction.
		/// </summary>
		Northeast	= 5,
		/// <summary>
		/// Specifies a northwestward direction.
		/// </summary>
		Northwest	= 6,
		/// <summary>
		/// Specifies a southeastward direction.
		/// </summary>
		Southeast	= 7,
		/// <summary>
		/// Specifies a southwestward direction.
		/// </summary>
		Southwest	= 8,
		/// <summary>
		/// Specifies a upwards direction.
		/// </summary>
		Up			= 9,
		/// <summary>
		/// Specifies a downwards direction.
		/// </summary>
		Down		= 10,
	}

	/// <summary>
	/// Defines an enumerator of walk types for places.
	/// </summary>
	[Flags]
	public enum WalkTypes
	{
		/// <summary>
		/// Specifies that movement is prevented.
		/// </summary>
		None		= 0,
		/// <summary>
		/// Specifies that walking is permitted.
		/// </summary>
		Walk		= 1,
		/// <summary>
		/// Specifies that swimming is required.
		/// </summary>
		Swim		= 2,
		/// <summary>
		/// Secifies that flying is permitted.
		/// </summary>
		Fly			= 4,
	}

	#region MobileType
	/// <summary>
	/// Provides an enumeration of types of mobiles.
	/// </summary>
	[Flags]
	public enum MobileTypes
	{
		/// <summary>
		/// Speicifes a mobile without a function.
		/// </summary>
		None = 0,
		/// <summary>
		/// Specified a mobile that walks a set path between places.
		/// </summary>
		Sentinal = 1,
		/// <summary>
		/// Specifies a mobile that is stationary.
		/// </summary>
		Guard = 2,
		/// <summary>
		/// Specifies a mobile that can buy and sell goods.
		/// </summary>
		Merchant = 4,
		/// <summary>
		/// Specifies a mobile that can provide rest for faster healing of body.
		/// </summary>
		Innkeeper = 8,
		/// <summary>
		/// Sepecifies a mobile that can interface the bank and provide deposits, withdrawls and loans.
		/// </summary>
		Banker = 16,
		/// <summary>
		/// Specifies a mobile that can train skills.
		/// </summary>
		Trainer = 32,
		/// <summary>
		/// Specifies a mobile that provides quests.
		/// </summary>
		QuestGiver = 64,
		/// <summary>
		/// Specifies a mobile that can provide healing to both body and mind.
		/// </summary>
		Priest = 128,
		/// <summary>
		/// Specifies a mobile that wonders around a specific area.
		/// </summary>
		Roamer = 256,
		/// <summary>
		/// Specifies a mobile that can offer repair services.
		/// </summary>
		Repairer = 512,
	}
	#endregion

	#region EquipLocation
	/// <summary>
	/// Defines an enumerator of locations where items can be equipped.
	/// </summary>
	public enum EquipLocation
	{
		None = 0,
		Head = 1,
		Ear = 2,
		Neck = 3,
		Shoulders = 4,
		Arms = 5,
		Chest = 6,
		Back = 7,
		Finger = 8,
		Wrists = 9,
		Hands = 10,
		Waist = 11,
		Legs = 12,
		Feet = 13,
		Hat = 14,
		Shirt = 15,
		Robe = 16,
		Pants = 17,
		Nose = 18,
		Weapon = 19,
		Shield = 20,
		Bag = 21,
		Ammo = 22,
		Pendant = 23,
		Light = 24,
		Spell = 25,
		Ability = 26,
		Psionic = 27,
	}

	public enum EquipmentSlot
	{
		None = 0,
		Head = 1,
		Ear1 = 2,
		Ear2 = 3,
		Neck = 4,
		Shoulders = 5,
		Arms = 6,
		Chest = 7,
		Back = 8,
		Finger1 = 9,
		Finger2 = 10,
		Wrists = 11,
		Hands = 12,
		Waist = 13,
		Legs = 14,
		Feet = 15,
		Hat = 16,
		Shirt = 17,
		Robe = 18,
		Pants = 19,
		Nose = 20,
		Weapon1 = 21,
		Weapon2 = 22,
		Shield = 23,
		Ammo = 24,
		Pendant = 25,
		Light = 26,
	}
	#endregion

	#region Actor Flags
	/// <summary>
	/// Defines an enumeration of actor flags.
	/// </summary>
	[Flags]
	public enum ActorFlags
	{
		/// <summary>
		/// Specifies that flags have not been set.
		/// </summary>
		None		= 0,
		/// <summary>
		/// Specifies the actor instance can not be sold.
		/// </summary>
		NoSell		= 1,
	}
	#endregion

	#region AffectType
	public enum AffectType
	{
		None,
		Heal,
		Mind,
		Damage,
		Stun,
		Snare,
		Protection,
		Strength,
		Dexterity,
		Stamina,
		Beauty,
		Intelligence,
		Perception,
		Endurance,
		Affinity,
	}
	#endregion

    #region Household Dues Term Type
    public enum HouseholdDuesTermType
    {
        Monthly,
        Quarterly,
        SemiAnnually,
        Annually,
    }
    #endregion

    #region ItemQualityType
    public enum ItemQualityType
    {
        Poor = 0,
        Fair = 1,
        Moderate = 2,
        Good = 3,
        Excellent = 4,
        Master = 5,
        Legendary = 6,
    }
    #endregion

    #region HouseholdRelationType
    public enum HouseholdRelationType
    {
        Neutral = 1,
        Alliance = 2,
        Hostile = 3,
    }
    #endregion

    #region HouseholdPermissions
    [Flags]
    public enum HouseholdPermissions
    {
        HeadOfHousehold = 0,
        CanEditImage = 1,
        CanEditMotto = 2,
        CanEditDescription = 4,
        CanEditRanks = 8,
        CanEditHonors = 16,
        CanAccessArmory = 32,
        CanAddMembers = 64,
        CanRemoveMembers = 128,
        CanPromoteMembers = 256,
    }
    #endregion
}
