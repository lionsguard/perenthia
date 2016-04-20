using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Security;

using Radiance;
using Radiance.Markup;

using Lionsguard.Security;

namespace Perenthia
{
	#region Quest
	public class Quest : Item, IQuest
	{
		#region Properties
		/// <summary>
		/// Gets or sets a description of the Actor.
		/// </summary>
		public override string Description
		{
			get { return this.Properties.GetValue<string>(DescriptionProperty); }
			set { this.Properties.SetValue(DescriptionProperty, value, true); }
		}

		public string BeginMessage
		{
			get { return this.Properties.GetValue<string>(BeginMessageProperty); }
			set { this.Properties.SetValue(BeginMessageProperty, value, true); }
		}
		public static readonly string BeginMessageProperty = "BeginMessage";

		public string InProgressMessage
		{
			get { return this.Properties.GetValue<string>(InProgressMessageProperty); }
			set { this.Properties.SetValue(InProgressMessageProperty, value, true); }
		}
		public static readonly string InProgressMessageProperty = "InProgressMessage";

		public string CompletedMessage
		{	
			get { return this.Properties.GetValue<string>(CompletedMessageProperty); }
			set { this.Properties.SetValue(CompletedMessageProperty, value, true); }
		}
		public static readonly string CompletedMessageProperty = "CompletedMessage";

		public Currency RewardCurrency
		{	
			get { return new Currency(this.Properties.GetValue<int>(RewardCurrencyProperty)); }
			set { this.Properties.SetValue(RewardCurrencyProperty, value.Value, true); }
		}
		public static readonly string RewardCurrencyProperty = "RewardCurrency";	

		public int RewardEmblem
		{
			get { return this.Properties.GetValue<int>(RewardEmblemProperty); }
			set { this.Properties.SetValue(RewardEmblemProperty, value, true); }
		}
		public static readonly string RewardEmblemProperty = "RewardEmblem";

		public int RewardExperience
		{
			get { return this.Properties.GetValue<int>(RewardExperienceProperty); }
			set { this.Properties.SetValue(RewardExperienceProperty, value, true); }
		}
        public static readonly string RewardExperienceProperty = "RewardExperience";

        public int RewardSkillValue
        {
            get { return this.Properties.GetValue<int>(RewardSkillValueProperty); }
            set { this.Properties.SetValue(RewardSkillValueProperty, value, true); }
        }
        public static readonly string RewardSkillValueProperty = "RewardSkillValue";

		public int MinimumLevel
		{
			get { return this.Properties.GetValue<int>(MinimumLevelProperty); }
			set { this.Properties.SetValue(MinimumLevelProperty, value, true); }
		}
		public static readonly string MinimumLevelProperty = "MinimumLevel";

		public int MaximumLevel
		{
			get { return this.Properties.GetValue<int>(MaximumLevelProperty); }
			set { this.Properties.SetValue(MaximumLevelProperty, value, true); }
		}
		public static readonly string MaximumLevelProperty = "MaximumLevel";

		public OrderType RequiredOrder
		{
			get { return this.Properties.GetValue<OrderType>(RequiredOrderProperty); }
			set { this.Properties.SetValue(RequiredOrderProperty, value, true); }
		}
		public static readonly string RequiredOrderProperty = "RequiredOrder";

		public string ParentQuestName
		{
			get { return this.Properties.GetValue<string>(ParentQuestNameProperty); }
			set { this.Properties.SetValue(ParentQuestNameProperty, value, true); }
		}
		public static readonly string ParentQuestNameProperty = "ParentQuestName";

		public DateTime StartedDate
		{
			get { return this.Properties.GetValue<DateTime>(StartedDateProperty); }
			set { this.Properties.SetValue(StartedDateProperty, value); }
		}
		public static readonly string StartedDateProperty = "StartedDate";

		public DateTime CompletedDate
		{
			get { return this.Properties.GetValue<DateTime>(CompletedDateProperty); }
			set { this.Properties.SetValue(CompletedDateProperty, value); }
		}
        public static readonly string CompletedDateProperty = "CompletedDate";

        public string RequiredSkill
        {
            get { return this.Properties.GetValue<string>(RequiredSkillProperty); }
            set { this.Properties.SetValue(RequiredSkillProperty, value, true); }
        }
        public static readonly string RequiredSkillProperty = "RequiredSkill";

        public int RequiredSkillValue
        {
            get { return this.Properties.GetValue<int>(RequiredSkillValueProperty); }
            set { this.Properties.SetValue(RequiredSkillValueProperty, value, true); }
        }
        public static readonly string RequiredSkillValueProperty = "RequiredSkillValue";

		/// <summary>
		/// Gets or sets a value indicating whether or not the current quest has been started.
		/// </summary>
		public bool IsStarted
		{
			get { return this.Properties.GetValue<bool>(IsStartedProperty); }
			set 
			{ 
				this.Properties.SetValue(IsStartedProperty, value);
				if (value) this.StartedDate = DateTime.Now;
			}
		}
		public static readonly string IsStartedProperty = "IsStarted";

		/// <summary>
		/// Gets or sets a value indicating whether or not the current quest has been completed.
		/// </summary>
		public bool IsComplete
		{	
			get { return this.Properties.GetValue<bool>(IsCompleteProperty); }
			set 
			{ 
				this.Properties.SetValue(IsCompleteProperty, value);
				if (value) this.CompletedDate = DateTime.Now;
			}
		}
		public static readonly string IsCompleteProperty = "IsComplete";

		/// <summary>
		/// Gets or sets a value indicating whether or not the quest task has been completed but the quest has not been turned in.
		/// </summary>
		public bool IsFinished
		{
			get { return this.Properties.GetValue<bool>(IsFinishedProperty); }
			set { this.Properties.SetValue(IsFinishedProperty, value); }
		}
		public static readonly string IsFinishedProperty = "IsFinished";	

		/// <summary>
		/// Gets or sets the name of the quest to auto start once this quest is complete.
		/// </summary>
		public string OnCompleteAutoStartQuestName
		{
			get { return this.Properties.GetValue<string>(OnCompleteAutoStartQuestNameProperty); }
			set { this.Properties.SetValue(OnCompleteAutoStartQuestNameProperty, value, true); }
		}
		public static readonly string OnCompleteAutoStartQuestNameProperty = "OnCompleteAutoStartQuestName";

		public TemplateItemCollection RewardItems { get; private set; }

		public QuestFacilitatorCollection StartsWithCollection { get; private set; }
		public QuestFacilitatorCollection EndsWithCollection { get; private set; }

        public TaskCollection Tasks { get; private set; }

        public bool HasEventsRegistered { get; protected set; }   
		#endregion

		public Quest()
		{
            this.Tasks = new TaskCollection(this);
            this.Tasks.TaskCompleted += new TaskEventHandler(Tasks_TaskCompleted);
			this.RewardItems = new TemplateItemCollection(this, "RewardItem_");
			this.StartsWithCollection = new QuestFacilitatorCollection(this, "StartsWith_");
			this.EndsWithCollection = new QuestFacilitatorCollection(this, "EndsWith_");
			this.ObjectType = ObjectType.Quest;
			this.RequiredOrder = OrderType.None;
		}

		public Quest(string name, string description)
			: this()
		{
			this.Name = name;
			this.Description = description;
        }

        private void Tasks_TaskCompleted(TaskEventArgs e)
        {
            if (this.Owner is Character)
            {
                if (this.Tasks.HasCompletedAllTasks(this.Owner as Character))
                {
                    this.Finished();
                }
            }
        }

		protected override string GetImageUri()
		{
			string uri = String.Empty;// this.Properties.GetValue<string>(ImageUriProperty);
			if (String.IsNullOrEmpty(uri))
			{
				uri = String.Format(Resources.ImgOrder, this.RequiredOrder).ToLower();
				this.Properties.SetValue(ImageUriProperty, uri, true);
			}
			return uri;
		}

		public void AddStartsWith(IActor actor)
		{
			this.StartsWithCollection.Add(actor);
		}

		public void AddEndsWith(IActor actor)
		{
			this.EndsWithCollection.Add(actor);
		}

		public bool StartsWith(IActor actor)
		{
			return this.StartsWithCollection.ContainsKey(actor.ID.ToString());
		}

		public bool EndsWith(IActor actor)
		{
			return this.EndsWithCollection.ContainsKey(actor.ID.ToString());
		}

		protected override void AddSimpleRdlProperties(List<RdlObject> list)
		{
			base.AddSimpleRdlProperties(list);
			list.AddRange(this.GetRdlProperties(
				Quest.MinimumLevelProperty,
				Quest.MaximumLevelProperty,
				Quest.RewardExperienceProperty,
				Quest.RewardCurrencyProperty,
				Quest.RewardEmblemProperty,
				Quest.BeginMessageProperty,
				Quest.InProgressMessageProperty,
				Quest.CompletedMessageProperty,
				Quest.RequiredOrderProperty));
		}

		/// <summary>
		/// Validates the requirements of the quest and writes messages to the owner's IMessageContext if specified.
		/// </summary>
		/// <param name="writeToContext">A value indicating whether or not to write output messages to the owner's 
		/// IMessageContext if validation fails.</param>
		/// <returns>True if the requirements for this quest have been met; otherwise false.</returns>
		public virtual bool Validate(bool writeToContext)
		{
			return true;
		}

		protected virtual void OnComplete()
		{
		}

		#region Start
		/// <summary>
		/// Verifies that this quest has not already been completed and allows the specified questor to start the quest.
		/// </summary>
		/// <param name="questor">The IAvatar undertaking the quest.</param>
		public void Start(IAvatar questor)
		{
			Quest quest = questor.Children.Where(c => c is Quest && c.Name == this.Name).Select(c => c as Quest).FirstOrDefault();
			if (quest != null)
			{
				if (quest.IsComplete)
				{
					questor.Context.Add(new RdlErrorMessage("You have already completed this quest."));
				}
				else
				{
					// The quest is in progress, send the in progress message to the questor.
					questor.Context.Add(new RdlChatMessage(this.Owner.Name,
						String.Format(Resources.MsgChat, this.Owner.Name, quest.InProgressMessage)));
				}
			}
			else
			{
                // Ensure the questor has the proper skill value required to take this quest.
                if (this.RequiredSkillValue > 0)
                {
                    if (questor.Skills[this.RequiredSkill] < this.RequiredSkillValue)
                    {
                        questor.Context.Add(new RdlErrorMessage(String.Format(Resources.QuestDoesNotHaveRequiredSkill,
                            this.RequiredSkill)));
                        return;
                    }
                }

				// Ensure the quest does not have an uncompleted parent.
				if (!String.IsNullOrEmpty(this.ParentQuestName))
				{
					Quest parentQuest = questor.Children.Where(c => c is Quest 
						&& c.Name == this.ParentQuestName
						&& (c as Quest).IsComplete).Select(c => c as Quest).FirstOrDefault();
					if (parentQuest == null)
					{
						questor.Context.Add(new RdlErrorMessage(String.Format(Resources.QuestParentRequired,
							this.ParentQuestName)));
						return;
					}
                }

				// Clone this quest instance
                quest = this.Clone() as Quest;

                // Hook up events that are fired on the questor and will be handled by the quest.
                if (questor is IPlayer) quest.HookEvents(questor as IPlayer);

                // Start the quest and add it to the questor's inventory.
				quest.IsStarted = true;
				questor.Children.Add(quest);
				quest.Save();

				// Send the start message down to the questor.
				if (this.Owner != null)
				{
					questor.Context.Add(new RdlChatMessage(this.Owner.Name,
						String.Format(Resources.MsgChat, this.Owner.Name, quest.BeginMessage)));
				}
				else
				{
					questor.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None, quest.BeginMessage));
				}

				// Send the actual quest instance down to the questor.
				questor.Context.AddRange(quest.ToRdl());

				// Inform the questor that the quest has been added.
				questor.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
					String.Format(Resources.QuestAddedToLog, quest.Name)));
			}
		}

        public virtual void HookEvents(IPlayer questor)
        {
            this.Tasks.HookEvents(questor as Character);
            this.HasEventsRegistered = true;
        }

        public virtual void UnHookEvents(IPlayer questor)
        {
            this.Tasks.UnHookEvents(questor as Character);
            this.HasEventsRegistered = false;
        }
		#endregion

		#region Finished
		public void Finished()
		{
			this.IsFinished = true;
			this.Save();

            // Unhook the events.
            if (this.Owner is IPlayer)
            {
                this.UnHookEvents(this.Owner as IPlayer);
            }

			// Send down the updated quest details to the owner.
			if (this.Owner != null && this.Owner is IAvatar)
			{
				(this.Owner as IAvatar).Context.AddRange(this.ToRdl());
			}
		}
		#endregion

		#region Complete
		/// <summary>
		/// Validates the requirements for this quest and if valid, completes the quest and issues the rewards to the owner.
		/// </summary>
		public void Complete()
		{
			if (this.Validate(true))
			{
				Character owner = this.Owner as Character;
				if (owner != null)
				{
					if (!this.IsComplete)
					{
						// Complete the quest.
						this.IsComplete = true;

						// Raise the onComplete event.
						this.OnComplete();

						owner.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
							String.Format(Resources.QuestCompleteLogUpdated, this.Name)));
						owner.Context.AddRange(this.ToRdl());

						// Emblem
						if (this.RewardEmblem > 0)
						{
							User user = SecurityManager.GetUser(owner.UserName);
							if (user != null)
							{
								user.Tokens += this.RewardEmblem;
								owner.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
									String.Format(Resources.EmblemGained,
									this.RewardEmblem, this.Name)));
								owner.Context.Add(new RdlProperty(owner.ID, "Emblem", user.Tokens));
							}
						}

						// Currency
						if (this.RewardCurrency.Value > 0)
						{
							owner.Currency.Value += this.RewardCurrency.Value;
							owner.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
								String.Format(Resources.CurrencyGained, this.RewardCurrency.ToString())));
							owner.Context.AddRange(owner.GetRdlProperties(Character.CurrencyProperty));
						}

						// Items
						bool processLoop = true;
						foreach (var reward in this.RewardItems)
						{
							if (processLoop)
							{
								for (int i = 0; i < reward.Quantity; i++)
								{
									Item item = this.World.CreateFromTemplate<Item>(reward.Name);
									if (item != null)
									{
										Container container = owner.GetFirstAvailableContainer();
										if (container != null)
										{
											item.Drop(container);
											item.Save();
											owner.Context.AddRange(item.ToRdl());
											owner.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
												String.Format(Resources.ItemGained, item.A())));
										}
										else
										{
											owner.Context.Add(new RdlErrorMessage(Resources.InventoryFull));
											processLoop = false;
											break;
										}
									}
								}
							}
							else
								break;
						}

						// Experience
						if (this.RewardExperience > 0)
						{
							owner.Experience += this.RewardExperience;
							owner.TotalExperience += this.RewardExperience;
							owner.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
								String.Format(Resources.ExperienceGained, this.RewardExperience)));

							// Cause the player to advance if the required experience requirements are met.
							LevelManager.AdvanceIfAble(owner);

							owner.Context.AddRange(owner.GetRdlProperties(
								Character.ExperienceProperty,
								Character.ExperienceMaxProperty));
						}

                        // Skill
                        if (this.RewardSkillValue > 0)
                        {
                            owner.Skills[this.RequiredSkill] += this.RewardSkillValue;

                            owner.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
                                String.Format(Resources.SkillGained, this.RewardSkillValue)));

                            owner.Context.AddRange(owner.GetRdlProperties(String.Concat(owner.Skills.Prefix, this.RequiredSkill)));
                        }

						// Save the owner instance.
						owner.Save();
						// Save the quest instance.
						this.Save();

						// Send down the completed quest details to the owner.
						owner.Context.AddRange(this.ToRdl());

						// TODO: If a quest auto starts another quest then the UI should display the new quest right away
						// instead of auot adding it.
						//// Some quests may auto start other quests without being given from an Actor instance.
						//if (!String.IsNullOrEmpty(this.OnCompleteAutoStartQuestName))
						//{
						//    Quest quest = this.World.GetTemplate<Quest>(this.OnCompleteAutoStartQuestName);
						//    if (quest != null)
						//    {
						//        quest.Start(owner);
						//    }
						//}
					}
					else
					{
						owner.Context.Add(new RdlErrorMessage(Resources.QuestAlreadyComplete));
					}
				}
			}
		}
		#endregion
	}

	public class QuestFacilitatorCollection : ActorOwnedDictionaryBase<int>
	{
		public new int this[string key]
		{
			get { return this.Owner.Properties.GetValue<int>(this.GetPrefixedName(key)); }
			set { this.Owner.Properties.SetValue(this.GetPrefixedName(key), value.ToString(), true); }
		}

		public QuestFacilitatorCollection(IActor owner, string prefix)
			: base(owner, prefix)
		{
		}

		public void Add(IActor actor)
		{
			this.Add(actor.ID.ToString(), actor.ID);
		}

		public override void Add(string key, int value)
		{
			this.Owner.Properties.SetValue(this.GetPrefixedName(key), value, true);
		}

		public override bool TryGetValue(string key, out int value)
		{
			if (this.ContainsKey(this.GetPrefixedName(key)))
			{
				value = this[this.GetPrefixedName(key)];
				return true;
			}
			value = 0;
			return false;
		}
	}
	#endregion

    //#region DiscoveryQuest
    //public class DiscoveryQuest : Quest
    //{
    //    /// <summary>
    //    /// Gets or sets the name of the actor to discover.
    //    /// </summary>
    //    public string ActorName
    //    {	
    //        get { return this.Properties.GetValue<string>(ActorNameProperty); }
    //        set { this.Properties.SetValue(ActorNameProperty, value, true); }
    //    }
    //    public static readonly string ActorNameProperty = "ActorName";

    //    /// <summary>
    //    /// Gets or sets the ObjectType of the actor to discover.
    //    /// </summary>
    //    public ObjectType ActorType
    //    {
    //        get { return this.Properties.GetValue<ObjectType>(ActorTypeProperty); }
    //        set { this.Properties.SetValue(ActorTypeProperty, value, true); }
    //    }
    //    public static readonly string ActorTypeProperty = "ActorType";

    //    public IActor DiscoveredActor { get; set; }	

    //    public DiscoveryQuest()
    //    {

    //    }

    //    /// <summary>
    //    /// Validates the requirements of the quest and writes messages to the owner's IMessageContext if specified.
    //    /// </summary>
    //    /// <param name="writeToContext">A value indicating whether or not to write output messages to the owner's 
    //    /// IMessageContext if validation fails.</param>
    //    /// <returns>True if the requirements for this quest have been met; otherwise false.</returns>
    //    public override bool Validate(bool writeToContext)
    //    {
    //        if (this.DiscoveredActor != null)
    //        {
    //            return (this.DiscoveredActor.ObjectType == this.ActorType
    //                && this.DiscoveredActor.Name == this.ActorName);
    //        }
    //        return false;
    //    }

    //    public override void HookEvents(IPlayer questor)
    //    {
    //        questor.PlaceEntered += new ActorEventHandler<IPlayer>(questor_PlaceEntered);
    //        base.HookEvents(questor);
    //    }

    //    private void questor_PlaceEntered(ActorEventArgs<IPlayer> e)
    //    {
    //        switch (this.ObjectType)
    //        {
    //            case ObjectType.Actor:
    //            case ObjectType.Mobile:
    //            case ObjectType.Player:
    //                // If the actor exists in the current place then they have been discovered.
    //                var actor = e.Actor.Place.GetAllChildren().Where(c => c.Name.Equals(this.ActorName)
    //                    && (c.ObjectType == ObjectType.Actor || c.ObjectType == ObjectType.Mobile || c.ObjectType == ObjectType.Player)).FirstOrDefault();
    //                if (actor != null && !this.IsComplete)
    //                {
    //                    this.Finished();
    //                }
    //                break;
    //            case ObjectType.Place:
    //                if (e.Actor.Place.Name.Equals(this.ActorName) && !this.IsComplete)
    //                {
    //                    this.Finished();
    //                }
    //                break;
    //        }
    //    }

    //    public override void UnHookEvents(IPlayer questor)
    //    {
    //        questor.PlaceEntered -= new ActorEventHandler<IPlayer>(questor_PlaceEntered);
    //        base.UnHookEvents(questor);
    //    }
    //}
    //#endregion

    //#region CollectionQuest
    //public class CollectionQuest : Quest
    //{
    //    /// <summary>
    //    /// Gets or sets the name of the actor to collect.
    //    /// </summary>
    //    public string ActorName
    //    {
    //        get { return this.Properties.GetValue<string>(ActorNameProperty); }
    //        set { this.Properties.SetValue(ActorNameProperty, value, true); }
    //    }
    //    public static readonly string ActorNameProperty = "ActorName";

    //    /// <summary>
    //    /// Gets or sets the number of ActorName objects that must be collected.
    //    /// </summary>
    //    public new int Quantity
    //    {
    //        get { return this.Properties.GetValue<int>(QuantityProperty); }
    //        set { this.Properties.SetValue(QuantityProperty, value, true); }
    //    }
    //    public static readonly string QuantityProperty = "Quantity";

    //    /// <summary>
    //    /// Gets or sets the number of items collected so far.
    //    /// </summary>
    //    public int Count
    //    {
    //        get { return this.Properties.GetValue<int>(CountProperty); }
    //        set { this.Properties.SetValue(CountProperty, value); }
    //    }
    //    public static readonly string CountProperty = "Count";

    //    public CollectionQuest()
    //    {

    //    }

    //    /// <summary>
    //    /// Validates the requirements of the quest and writes messages to the owner's IMessageContext if specified.
    //    /// </summary>
    //    /// <param name="writeToContext">A value indicating whether or not to write output messages to the owner's 
    //    /// IMessageContext if validation fails.</param>
    //    /// <returns>True if the requirements for this quest have been met; otherwise false.</returns>
    //    public override bool Validate(bool writeToContext)
    //    {
    //        return (this.Count >= this.Quantity);
    //    }

    //    public override void HookEvents(IPlayer questor)
    //    {
    //        questor.ItemReceived += new ActorEventHandler<IItem>(questor_ItemReceived);
    //        questor.ItemDropped += new ActorEventHandler<IItem>(questor_ItemDropped);
    //        base.HookEvents(questor);
    //    }

    //    private void questor_ItemReceived(ActorEventArgs<IItem> e)
    //    {
    //        if (e.Actor.Name.Equals(this.ActorName))
    //        {
    //            this.Count++;
    //            if (this.Count >= this.Quantity && !this.IsComplete)
    //            {
    //                this.Finished();
    //            }
    //            this.Save();
    //        }
    //    }

    //    private void questor_ItemDropped(ActorEventArgs<IItem> e)
    //    {
    //        if (e.Actor.Name.Equals(this.ActorName))
    //        {
    //            this.Count--;
    //            if (this.Count < this.Quantity && this.IsFinished && !this.IsComplete)
    //            {
    //                this.IsFinished = false;
    //            }
    //            this.Save();
    //        }
    //    }

    //    public override void UnHookEvents(IPlayer questor)
    //    {
    //        questor.ItemReceived -= new ActorEventHandler<IItem>(questor_ItemReceived);
    //        questor.ItemDropped -= new ActorEventHandler<IItem>(questor_ItemDropped);
    //        base.UnHookEvents(questor);
    //    }
    //}
    //#endregion

    //#region KillQuest
    //public class KillQuest : CollectionQuest
    //{
    //    public KillQuest()
    //    {

    //    }
    //}
    //#endregion	

    //#region DeliveryQuest
    //public class DeliveryQuest : CollectionQuest
    //{
    //    /// <summary>
    //    /// Gets or sets the name of the actor receiving the items being delivered.
    //    /// </summary>
    //    public string RecipientActorName
    //    {
    //        get { return this.Properties.GetValue<string>(RecipientActorNameProperty); }
    //        set { this.Properties.SetValue(RecipientActorNameProperty, value, true); }
    //    }
    //    public static readonly string RecipientActorNameProperty = "RecipientActorName";

    //    public IActor DeliveredTo { get; set; }	

    //    public DeliveryQuest()
    //    {

    //    }

    //    protected override void OnComplete()
    //    {
    //        // Remove the items from the owner.
    //        IAvatar owner = this.Owner as IAvatar;
    //        var items = this.Owner.GetAllChildren().Where(c => c.Name == this.ActorName).Take(this.Quantity);
    //        foreach (var item in items)
    //        {
    //            this.Owner.Children.Remove(item);
    //            if (owner != null)
    //            {
    //                owner.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Negative,
    //                    String.Format(Resources.ItemRemoved, item.AUpper())));
    //                owner.Context.AddRange(item.ToSimpleRdl());
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Validates the requirements of the quest and writes messages to the owner's IMessageContext if specified.
    //    /// </summary>
    //    /// <param name="writeToContext">A value indicating whether or not to write output messages to the owner's 
    //    /// IMessageContext if validation fails.</param>
    //    /// <returns>True if the requirements for this quest have been met; otherwise false.</returns>
    //    public override bool Validate(bool writeToContext)
    //    {
    //        return (this.DeliveredTo != null && this.DeliveredTo.Name == this.RecipientActorName);
    //    }
    //}
    //#endregion
}
