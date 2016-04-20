using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;
using Radiance.Handlers;

namespace Radiance
{
	#region IClient
	/// <summary>
	/// Defines a client connected to the server.
	/// </summary>
	public interface IClient
	{
		/// <summary>
		/// Gets the session id for the current connected client.
		/// </summary>
		Guid SessionId { get; }

		/// <summary>
		/// Gets or sets the remote address of the current client.
		/// </summary>
		string Address { get; set; }

		/// <summary>
		/// Gets a value indicating whether or not the current client is still connected.
		/// </summary>
		bool Connected { get; }

		/// <summary>
		/// In a derived class, gets or sets the AuthKey of the current client.
		/// </summary>
		AuthKey AuthKey { get; set; }

		/// <summary>
		/// Gets or sets the authenticated user associated with the current connection.
		/// </summary>
		string UserName { get; set; }

		/// <summary>
		/// Gets the DateTime this client instance was last accessed.
		/// </summary>
		DateTime LastHeartbeatDate { get; set; }

		/// <summary>
		/// Gets the messaging context or the current client.
		/// </summary>
		IMessageContext Context { get; }

		/// <summary>
		/// Gets or sets the player instance represented by the current client.
		/// </summary>
		IPlayer Player { get; set; }

		/// <summary>
		/// Gets or sets the command handler for the current client.
		/// </summary>
		CommandHandler Handler { get; set; }
		
		/// <summary>
		/// Gets or sets the last command group executed by the current client.
		/// </summary>
		RdlCommandGroup LastCommandGroup { get; set; }

		/// <summary>
		/// Gets the interval in minutes between the current time and the last heartbeat date.
		/// </summary>
		/// <returns>The interval in minutes between the current time and the last heartbeat date.</returns>
		double GetLastHeartbeatInterval();

		/// <summary>
		/// Expires the client for removal, purging the player instance and removing it from the virtual world.
		/// </summary>
		void Expire();
	}
	#endregion

	#region IMessageContext
	/// <summary>
	/// Defines a class used to write and store virtual world tags for display to connected dinizens of the virtual world.
	/// </summary>
	public interface IMessageContext
	{
		/// <summary>
		/// Gets a value indicating whether or not the current message context has queued messages.
		/// </summary>
		bool HasMessages { get; }

		/// <summary>
		/// Gets the total number of messages currently available to be read.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Adds a Tag instance to the context.
		/// </summary>
		/// <param name="tag">The Tag instance to add to the context.</param>
		void Add(RdlTag tag);

		/// <summary>
		/// Adds a range of RdlTag instances to the context.
		/// </summary>
		/// <param name="tags">The tags to add to the context.</param>
		void AddRange(RdlTag[] tags);

		/// <summary>
		/// Reads and removes the next tag in the queue and returns true if a tag was found.
		/// </summary>
		/// <param name="tag">The next available tag in the queue.</param>
		/// <returns>True if a tag exists; otherwise false.</returns>
		bool Read(out RdlTag tag);

		/// <summary>
		/// Flushes all queued tags and make the tags available to the Read method.
		/// </summary>
		void Flush();
	}
	#endregion

	#region ICommunicationHandler
	/// <summary>
	/// Defines a class that is used to handle communication between the client and server.
	/// </summary>
	public interface ICommunicationHandler
	{
		/// <summary>
		/// Creates a new instance of the IClient class used with the current ICommunicationHandler instance.
		/// </summary>
		/// <returns>An instance of IClient for the current context.</returns>
		IClient CreateClient(Guid sessionId);

		///// <summary>
		///// Processes the specified tags and sends the content to the requestor.
		///// </summary>
		///// <param name="tags">The tags to send to the requestor.</param>
		//void ProcessResponse(params RdlTag[] tags);
	}
	#endregion

    #region IGameObject
    /// <summary>
    /// Defines a base game object.
    /// </summary>
    public interface IGameObject
    {
        /// <summary>
        /// Gets or sets the unique identifier for the current object.
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the collection of properties associated with the current object.
        /// </summary>
        PropertyCollection Properties { get; set; }
    }
    #endregion

    #region IActor
    /// <summary>
	/// Defines the base properties and methods of all objects in the virtual world.
	/// </summary>
	public interface IActor : IGameObject, ICloneable
	{
		#region Properties

		/// <summary>
		/// Gets or sets the unique identifier of the template this instance is derived from. Not required.
		/// </summary>
		int TemplateID { get; set; }

		/// <summary>
		/// Gets or sets the type or category of the current object.
		/// </summary>
		ObjectType ObjectType { get; set; }

		/// <summary>
		/// Gets or sets the owner of the current Actor instance.
		/// </summary>
		IActor Owner { get; set; }

		/// <summary>
		/// Gets or sets a description of the Actor.
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// Gets a value indicating whether or not the actor is dead or destroyed.
		/// </summary>
		bool IsDead { get; set; }

		/// <summary>
		/// Gets a value indicating the physical health of the actor.
		/// </summary>
		int Body { get; }

		/// <summary>
		/// Gets a value indicating the maximum physical health of the actor.
		/// </summary>
		int BodyMax { get; }

		/// <summary>
		/// Gets or sets any flags on the current actor.
		/// </summary>
		ActorFlags Flags { get; set; }

		/// <summary>
		/// Gets or sets the current virtual world of the actor.
		/// </summary>
		World World { get; set; }

		/// <summary>
		/// Gets the collection of Actor instances owned by the current actor.
		/// </summary>
		ActorCollection Children { get; }

		/// <summary>
		/// Gets a value indicating whether or not the name of the Actor is a proper name.
		/// </summary>
		bool HasProperName { get; set; }

		/// <summary>
		/// Gets the Alias name for the current Actor.
		/// </summary>
		string Alias { get; }

		/// <summary>
		/// Gets or sets the image used to represent the current IActor.
		/// </summary>
		string ImageUri { get; set; }
		#endregion

		#region Events
		/// <summary>
		/// An event that is raised when another Actor enters or makes contact with the current Actor.
		/// </summary>
		void OnEnter(IActor sender, IMessageContext context, Direction direction);

		/// <summary>
		/// An event that is raised when another Actor exits or leaves contact with the current Actor.
		/// </summary>
		void OnExit(IActor sender, IMessageContext context, Direction direction);

		/// <summary>
		/// An event that is raised when a buff enhancement is applied the current Actor.
		/// </summary>
		void OnBuff(IActor sender, IMessageContext context, Foci foci);

		/// <summary>
		/// An event that is raised when damage is applied to the current Actor.
		/// </summary>
		void OnDamage(IActor sender, IMessageContext context, Foci foci);

		/// <summary>
		/// An event that is raised when an enchantment is applied to the current Actor.
		/// </summary>
		void OnEnchant(IActor sender, IMessageContext context, Foci foci);

		/// <summary>
		/// An event that is raised when healing has been applied to the current Actor.
		/// </summary>
		void OnHeal(IActor sender, IMessageContext context, Foci foci);

		/// <summary>
		/// Raised internally when the Actor has been loaded from the database.
		/// </summary>
		void OnLoadComplete();

		/// <summary>
		/// Raises internally when a property has changed and has been specifically set to raise this event.
		/// </summary>
		void OnPropertyChanged(string name);
		#endregion

		#region Save
		/// <summary>
		/// Persists the current Actor instance to the data store.
		/// </summary>
		void Save();
		#endregion

		#region GetValue
		/// <summary>
		/// Gets the value of the specified property from the Properties collection.
		/// </summary>
		/// <typeparam name="T">The System.Type of the value of the property.</typeparam>
		/// <param name="propertyName">The name of the property value to retrieve.</param>
		/// <returns>The value of the named property.</returns>
		T GetValue<T>(string propertyName);
		#endregion

		#region SetBody
		/// <summary>
		/// Sets the value of the Body score for the current Actor.
		/// </summary>
		/// <param name="value">The value of the new body score.</param>
		void SetBody(int value);

		/// <summary>
		/// Sets the value and maximum value of the Body score for the current Actor.
		/// </summary>
		/// <param name="value">The value of the new body score.</param>
		/// <param name="max">The maximum value of the new body score.</param>
		void SetBody(int value, int max);
		#endregion

		#region RaiseLoadComplete
		/// <summary>
		/// Causes the Actor to raise the load complete event.
		/// </summary>
		void RaiseLoadComplete();
		#endregion

		#region RDL Methods
		/// <summary>
		/// Converts the current Actor to RDL tags and returns the tags as an array.
		/// </summary>
		/// <returns>The array of RDL tags for the current Actor.</returns>
		RdlObject[] ToRdl();

		/// <summary>
		/// Converts the current Actor to a simple set of basic RDL tags.
		/// </summary>
		/// <returns>The array of basic RDL tags for the current Actor.</returns>
		RdlObject[] ToSimpleRdl();

		/// <summary>
		/// Gets an array of RdlProperty tags for the specified property names.
		/// </summary>
		/// <param name="propertyNames">The array of property names to retrieve RdlProperty instances for.</param>
		/// <returns>An array of RdlProperty tags for the specified property names.</returns>
		RdlProperty[] GetRdlProperties(params string[] propertyNames);
		#endregion

		#region ToString
		/// <summary>
		/// Converts the current Actor instance to a string representation. Default is the name of the Actor.
		/// </summary>
		/// <returns>The string representation of the current Actor.</returns>
		string ToString();
		#endregion

		#region GetOffensiveSkill, GetDefensiveSkill
		/// <summary>
		/// Gets the offensive skill for the current Actor.
		/// </summary>
		/// <returns>The offensive skill for the current Actor.</returns>
		string GetOffensiveSkill();

		/// <summary>
		/// Gets the defensive skill for the current Actor.
		/// </summary>
		/// <returns>The defensive skill for the current Actor.</returns>
		string GetDefensiveSkill();
		#endregion

		#region GetAllChildren
		/// <summary>
		/// Gets all children and grand children of the current Actor.
		/// </summary>
		/// <returns>A list of all child and grand child actor instances owned by the current Actor.</returns>
		List<IActor> GetAllChildren();
		#endregion

		#region GetActiveQuests
		/// <summary>
		/// Gets an enuerable list of IQuest instances where the quest is active, meanining it is not complete and/or 
		/// starts with the current actor.
		/// </summary>
		/// <returns>A list of active quests.</returns>
		IEnumerable<IQuest> GetActiveQuests();
		#endregion

		#region IsOwner
		/// <summary>
		/// Gets a value indicating whether or not the specified Actor is the owner of the current Actor. Shortcut to checking 
		/// for null on the Owner property.
		/// </summary>
		/// <param name="owner">The Actor to check against the current owner for a match.</param>
		/// <returns>True if the specified Actor instance is the owner of the current Actor; otherwise false.</returns>
		bool IsOwner(Actor owner);
		#endregion
	}
	#endregion

	#region IAvatar
	/// <summary>
	/// Defines the properties and methods of Avatars (players, NPCs, etc.) within the virtual world.
	/// </summary>
	public interface IAvatar : IActor
	{
		#region Properties
		/// <summary>
		/// Gets the list of attributes for the current Avatar.
		/// </summary>
		AttributeList Attributes { get; }

		/// <summary>
		/// Gets the list of skills of the current Avatar.
		/// </summary>
		SkillList Skills { get; }

		/// <summary>
		/// Gets or sets the luck value for the current Avatar.
		/// </summary>
		int Luck { get; set; }

		/// <summary>
		/// Gets or sets the initiative of the current Avatar. This value is set during combat by the game engine.
		/// </summary>
		int Initiative { get; set; }

		/// <summary>
		/// Gets a value indicating whether or not the Avatar has been knocked unconscious.
		/// </summary>
		bool IsUnconscious { get; }

		/// <summary>
		/// Gets a value indicating the mental health of the Avatar.
		/// </summary>
		int Mind { get; }

		/// <summary>
		/// Gets a value indicating the maximum mental health of the Avatar.
		/// </summary>
		int MindMax { get; }

		/// <summary>
		/// Gets or sets the value of any magical protection applied to the avatar.
		/// </summary>
		int Protection { get; set; }

		/// <summary>
		/// Gets or sets the place where the current avatar resides.
		/// </summary>
		Place Place { get; set; }

		/// <summary>
		/// Gets or sets the first name of the avatar.
		/// </summary>
		string FirstName { get; set; }

		/// <summary>
		/// Gets or sets the last name of the avatar.
		/// </summary>
		string LastName { get; set; }

		/// <summary>
		/// Gets or sets the location of the current avatar.
		/// </summary>
		Point3 Location { get; set; }

		/// <summary>
		/// Gets or sets the x coordinate of the current avatar.
		/// </summary>
		int X { get; set; }

		/// <summary>
		/// Gets or sets the y coordinate of the current avatar.
		/// </summary>
		int Y { get; set; }

		/// <summary>
		/// Gets or sets the z coordinate of the current avatar.
		/// </summary>
		int Z { get; set; }

		/// <summary>
		/// Gets or sets the IMessageContext used for the current avatar.
		/// </summary>
		IMessageContext Context { get; set; }

		/// <summary>
		/// Gets or sets the gender of the avatar.
		/// </summary>
		Gender Gender { get; set; }

		/// <summary>
		/// Gets or sets the race of the avatar.
		/// </summary>
		string Race { get; set; }

		/// <summary>
		/// Gets or sets the Target Actor of the current Avatar.
		/// </summary>
		IActor Target { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not the current avatar is a builder.
		/// </summary>
		bool IsBuilder { get; set; }

		/// <summary>
		/// Gets a collection of actors affecting the current avatar.
		/// </summary>
		AffectCollection Affects { get; }

		/// <summary>
		/// Gets or sets a value indicating whether or not the avatar has been poisoned.
		/// </summary>
		bool IsPoisoned { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or no the avatar is sutnned.
		/// </summary>
		bool IsStunned { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not the avatar is immobilized.
		/// </summary>
		bool IsSnared { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not the avatar is on fire.
		/// </summary>
		bool IsInflamed { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not the avatar is frozen.
		/// </summary>
		bool IsFrozen { get; set; }

		/// <summary>
		/// Gets a value indicating whether or not the current avatar can perform an action.
		/// </summary>
		bool CanPerformAction { get; }

		/// <summary>
		/// Gets or sets the alias to the item to use for combat or null if pass on combat turn.
		/// </summary>
		string CombatAction { get; set; }

		/// <summary>
		/// Gets or sets the current match this avatar is participating in.
		/// </summary>
		Guid CombatMatch { get; set; }
		#endregion

		#region Events
		/// <summary>
		/// An event that is raised when the current IAvatar is attacking a target, the skill rolls have been completed and 
		/// the outcome dice have been modified. This event is raised right before a check to the outcome dice to determine 
		/// if the attacker struct its target.
		/// </summary>
		/// <param name="outcomeSuccessDice">The current number of success dice for this attack.</param>
		void OnAttack(ref int outcomeSuccessDice);

		/// <summary>
		/// Applies protection of the current avatar to the overall damage value.
		/// </summary>
		/// <param name="attacker">The IAvatar attacking the current avatar.</param>
		/// <param name="damage">The current damage value to modify based on protection.</param>
		void OnApplyProtection(IAvatar attacker, ref int damage);

		/// <summary>
		/// An event that is raised after a skill roll during a spell casting. Can be used to allow the 
		/// caster to always successfully cast the spell.
		/// </summary>
		/// <param name="spell">The spell being cast.</param>
		/// <param name="castSuccessCount">The current successful number of dice rolled for this spell's skill value.</param>
		void OnCastSkillRoll(ISpell spell, ref int castSuccessCount);

		/// <summary>
		/// An event that is raised after a spell has been cast on the target. Can be used to reset the mind value of the caster.
		/// </summary>
		/// <param name="spell">The spell being cast.</param>
		void OnCastSuccess(ISpell spell);
		#endregion

		#region SetMind
		/// <summary>
		/// Sets the value of the Mind score for the current Avatar.
		/// </summary>
		/// <param name="value">The value of the new Mind score.</param>
		void SetMind(int value);

		/// <summary>
		/// Sets the value and maximum value of the Mind score for the current Avatar.
		/// </summary>
		/// <param name="value">The value of the new Mind score.</param>
		/// <param name="max">The maximum value of the new Mind score.</param>
		void SetMind(int value, int max);
		#endregion

		#region Heartbeat
		/// <summary>
		/// Performs the heartbeat for the current avatar.
		/// </summary>
		void Heartbeat();
		#endregion

		#region GetTotalProtection, ReduceArmorDurability, GetWeapon
		/// <summary>
		/// Gets an IArmor instance that represents the total cumaltive armor/protection rating for the current Actor.
		/// </summary>
		/// <returns>The total protection value of all armor applied to the Actor.</returns>
		int GetTotalProtection();

		/// <summary>
		/// Gets an IWeapon instance representing the primary weapon of the Actor.
		/// </summary>
		/// <returns></returns>
		IWeapon GetWeapon();

        /// <summary>
        /// Gets an IWeapon instance representing the secondary weapon of the Actor.
        /// </summary>
        /// <returns></returns>
        IWeapon GetSecondaryWeapon();

        /// <summary>
        /// Gets an IArmor instance representing the primary armor of the actor.
        /// </summary>
        /// <returns></returns>
        IArmor GetArmor();
		#endregion

		#region Messaging
		/// <summary>
		/// Tells the current avatar the specified text from the specified avatar.
		/// </summary>
		/// <param name="from">The avatar sending the tell.</param>
		/// <param name="text">The text of the tell message.</param>
		void Tell(Avatar from, string text);
		#endregion
	}
	#endregion

	#region IMobile
	/// <summary>
	/// Defines an Avatar that is controlled by the virtual world.
	/// </summary>
	public interface IMobile : IAvatar
	{
		/// <summary>
		/// Gets or sets the MobileType of the current Mobile instance.
		/// </summary>
		MobileTypes MobileType { get; set; }

		/// <summary>
		/// Gets or sets the TimeSpan used to determine an amount of time to delay before the Mobile is respawned after death.
		/// </summary>
		TimeSpan RespawnDelay { get; set; }

		/// <summary>
		/// Gets or sets the DateTime at which point the Mobile can respawn.
		/// </summary>
		DateTime RespawnTime { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not the current mobile is hostile to players.
		/// </summary>
		bool IsHostile { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not quests starting or ending with this IMobile instance have been loaded.
		/// </summary>
		bool IsQuestsLoaded { get; set; }
	}
	#endregion

	#region IMerchant
	/// <summary>
	/// Defines an IMobile instance that is a merchant.
	/// </summary>
	public interface IMerchant : IMobile
	{
		/// <summary>
		/// Gets an array of goods and services provided by the merchant.
		/// </summary>
		/// <returns></returns>
		IEnumerable<IItem> GetGoodsAndServices();

		/// <summary>
		/// Adds an IItem as merchandise for the current merchant.
		/// </summary>
		/// <param name="item">The IItem representing the merchandise to add.</param>
		void AddMerchandise(IItem item);
	}
	#endregion

	#region IPlayer
	/// <summary>
	/// Defines an Avatar that is controlled by a physical player connected to the virtual world.
	/// </summary>
	public interface IPlayer : IAvatar
	{
        event ActorEventHandler<IAvatar> Died;
        event ActorEventHandler<IActor> KilledActor;
        event ActorEventHandler<IItem> ItemReceived;
        event ActorEventHandler<IItem> ItemDropped;
        event ActorEventHandler<IPlayer> PlaceEntered;
        event ActorEventHandler<IPlayer> PlaceExited;

		/// <summary>
		/// Gets the username of the user that owns the current player instance.
		/// </summary>
		string UserName { get; set; }

		/// <summary>
		/// Gets or sets the number of Skill Points available to the current player.
		/// </summary>
		int SkillPoints { get; set; }

		/// <summary>
		/// Gets the client of the connected real world human player.
		/// </summary>
		IClient Client { get; set; }

        /// <summary>
        /// Gets or sets information for the current player's household, rank and title.
        /// </summary>
        HouseholdInfo Household { get; set; }

		/// <summary>
		/// Gets an AuthKey instance for the current IPlayer.
		/// </summary>
		/// <returns>An AuthKey instance for the current IPlayer.</returns>
        AuthKey GetAuthKey(Guid sessionId);

        void OnDied(IAvatar killer);

        void OnKilledActor(IActor actor);

        void OnItemReceived(IItem item);

        void OnItemDropped(IItem item);

        void OnPlaceEntered();

        void OnPlaceExited();
	}
	#endregion

	#region IItem
	/// <summary>
	/// Defines an inanimate object within the virtual world suchs as weapons and armor.
	/// </summary>
	public interface IItem : IActor
	{
		/// <summary>
		/// Gets or sets the name of the skill required to use the current item.
		/// </summary>
		string Skill { get; set; }

		/// <summary>
		/// Gets or sets the skill level required in order to equip the current item.
		/// </summary>
		int SkillLevelRequiredToEquip { get; set; }

		/// <summary>
		/// Gets a value indicating the durability or usability of the item.
		/// </summary>
        int Durability { get; set; }

		/// <summary>
		/// Gets the maximum amount of durability that can be restored to the current item.
		/// </summary>
		int DurabilityMax { get; }

		/// <summary>
		/// Gets or sets a value indicating whether or not the current item is equipped.
		/// </summary>
		bool IsEquipped { get; set; }

		/// <summary>
		/// Gets or sets the acceptable EquipLocation of the current item.
		/// </summary>
		EquipLocation EquipLocation { get; set; }

		/// <summary>
		/// Gets a list of attributes affected by the current item.
		/// </summary>
		AttributeList Affects { get; }

        /// <summary>
        /// Gets or sets the quality of the item.
        /// </summary>
        ItemQualityType Quality { get; set; }

		/// <summary>
		/// Sets the durability of the current item.
		/// </summary>
		/// <param name="durability">The durability of the current item.</param>
		void SetDurability(int durability);

		void Use(IActor user, IMessageContext context);
	}
	#endregion

	#region IAffectItem
	public interface IAffectItem : IItem
	{
		TimeSpan Duration { get; set; }

		bool IncreaseAffect { get; set; }

		int IncrementValue { get; set; }

		void ApplyAffect(IAvatar target);

		void ApplyIncrementAffect(IAvatar target);

		/// <summary>
		/// Removes the affect added by the current spell.
		/// </summary>
		/// <param name="target"></param>
		void RemoveAffect(IAvatar target);
	}
	#endregion

	#region ISpell
	/// <summary>
	/// Defines an object that is used to case spells on IActor instances.
	/// </summary>
	public interface ISpell : IAffectItem
	{
		/// <summary>
		/// Gets the values used to focus willpower and cast the current spell.
		/// </summary>
		Foci Foci { get; }

		/// <summary>
		/// Gets or sets a value indicating whether or not this spell does damage to the target.
		/// </summary>
		bool IsDamageSpell { get; }

		/// <summary>
		/// Causes the current spell to be case on the specified target.
		/// </summary>
		/// <param name="caster">The IAvatar instance casting the spell.</param>
		/// <param name="target">The IActor instance having the spell cast on it.</param>
		/// <param name="results">The results of the Cast operation.</param>
		void Cast(IAvatar caster, IActor target, CastResults results);
	}
	#endregion

	#region IWeapon
	/// <summary>
	/// Defines an object used to attack other IActor instances within the virtual world.
	/// </summary>
	public interface IWeapon : IItem
	{
		/// <summary>
		/// Gets or sets the value used to indicate the power of the damge done by the current weapon.
		/// </summary>
		int Power { get; set; }

		/// <summary>
		/// Gets or sets the range for which this weapon can be effective.
		/// </summary>
		int Range { get; set; }
	}
	#endregion

	#region IArmor
	/// <summary>
	/// Defines a object used to protected and defender an IAvatar within the virtual world.
	/// </summary>
	public interface IArmor : IItem
	{
		/// <summary>
		/// Gets or sets the protection offered by the current armor.
		/// </summary>
		int Protection { get; set; }
	}
	#endregion

	#region IQuest
	/// <summary>
	/// Defines an object used to provide a quest tree.
	/// </summary>
	public interface IQuest : IActor
	{
		/// <summary>
		/// Gets or sets the name of the Quest requiring completion before this quest can be undertaken.
		/// </summary>
		string ParentQuestName { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not the current quest has been started.
		/// </summary>
		bool IsStarted { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not the quest task has been completed but the quest has not been turned in.
		/// </summary>
		bool IsFinished { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not the current quest has been completed.
		/// </summary>
		bool IsComplete { get; set; }

		/// <summary>
		/// Adds the specified IActor instance as an actor who can present the quest to a player.
		/// </summary>
		/// <param name="actor">The IActor instance who can present the quest.</param>
		void AddStartsWith(IActor actor);

		/// <summary>
		/// Adds the specified IActor instance as an actor who causes the quest to be completed or where the quest is turned in.
		/// </summary>
		/// <param name="actor">The IActor instance who can complete the quest.</param>
		void AddEndsWith(IActor actor);

		/// <summary>
		/// Gets a value indicating whether or not the current quest starts with the specified IActor.
		/// </summary>
		/// <param name="actor">The IActor instance to verify.</param>
		/// <returns>True if the specified IActor instance starts this quest; otherwise false.</returns>
		bool StartsWith(IActor actor);

		/// <summary>
		/// Gets a value indicating whether or not the current quest ends with the specified IActor.
		/// </summary>
		/// <param name="actor">The IActor instance to verify.</param>
		/// <returns>True if the specified IActor instance ends this quest; otherwise false.</returns>
		bool EndsWith(IActor actor);
	}
	#endregion

	#region ICommandHandler
	/// <summary>
	/// Defines an object that can execute game commands.
	/// </summary>
	public interface ICommandHandler
	{
		/// <summary>
		/// Gets an enumerable list of commands currently handled.
		/// </summary>
		IEnumerable<string> HandledCommands { get; }

		/// <summary>
		/// Handles execution of the specified command.
		/// </summary>
		/// <param name="server">The current server instance.</param>
		/// <param name="cmd">The command to process.</param>
		/// <param name="client">The current IClient instance executing the command.</param>
		void HandleCommand(Server server, RdlCommand cmd, IClient client);
	}
	#endregion

	#region IModule
	/// <summary>
	/// Defines a component that can handle part of the game logic.
	/// </summary>
	public interface IModule : ICommandHandler
	{
		/// <summary>
		/// Gets the name of the module.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Initializes the module.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Causes the current module to update its state for a game tick.
		/// </summary>
		void Update();
	}
	#endregion
}
