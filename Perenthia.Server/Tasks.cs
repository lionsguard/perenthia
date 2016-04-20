using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia
{
    #region Task
    [System.Runtime.Serialization.DataContract]
    public class Task
    {
        public event TaskEventHandler Completed = delegate { };

        [System.Runtime.Serialization.DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the actor that is the target of the current task.
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public string TargetActorName { get; set; }

        /// <summary>
        /// Gets or sets the ObjectType of the target actor.
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public ObjectType TargetActorType { get; set; }

        /// <summary>
        /// Gets or sets an actor that must be used during the task to complete it.
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public string UseActorName { get; set; }

        [System.Runtime.Serialization.DataMember]
        public string MaleTitle { get; set; }

        [System.Runtime.Serialization.DataMember]
        public string FemaleTitle { get; set; }

        [System.Runtime.Serialization.DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the quantity required for the current task is in addition to what is already earned.
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public bool IsQuantityAdditional { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the quantity required for the current task is a total rather than additional.
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public bool IsQuantitySequential { get; set; }  

        /// <summary>
        /// Gets or sets the number of required actors.
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the number of actors acquired.
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public int Count { get; set; }

        [System.Runtime.Serialization.DataMember]
        public TaskType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that this task has been completed.
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public bool IsComplete { get; set; }

        private void CompleteTask()
        {
            this.IsComplete = true;
            this.Completed(new TaskEventArgs(this));
        }

        public string GetTitle(Gender gender)
        {
            if (gender == Gender.Female) return this.FemaleTitle;
            return this.MaleTitle;
        }

        public bool HasTaskEventHandler()
        {
            return this.Completed != null;
        }

        public void SetEventHandlers(Character player, bool add)
        {
            switch (this.Type)
            {
                case TaskType.Discovery:
                    if (add)
                        player.PlaceEntered += new ActorEventHandler<IPlayer>(player_PlaceEntered);
                    else
                        player.PlaceEntered -= new ActorEventHandler<IPlayer>(player_PlaceEntered);
                    break;
                case TaskType.Collection:
                    if (add)
                    {
                        player.ItemReceived += new ActorEventHandler<IItem>(player_ItemReceived);
                        player.ItemDropped += new ActorEventHandler<IItem>(player_ItemDropped);
                    }
                    else
                    {
                        player.ItemReceived -= new ActorEventHandler<IItem>(player_ItemReceived);
                        player.ItemDropped -= new ActorEventHandler<IItem>(player_ItemDropped);
                    }
                    break;
                case TaskType.Kill:
                case TaskType.KillWithItem:
                    if (add)
                        player.KilledActor += new ActorEventHandler<IActor>(player_KilledActor);
                    else
                        player.KilledActor -= new ActorEventHandler<IActor>(player_KilledActor);
                    break;
                case TaskType.Delivery:
                    if (add)
                        player.ItemDropped += new ActorEventHandler<IItem>(player_DeliveryItemDropped);
                    else
                        player.ItemDropped -= new ActorEventHandler<IItem>(player_DeliveryItemDropped);
                    break;  
            }
        }

        #region Task Event Handlers

        private void player_DeliveryItemDropped(ActorEventArgs<IItem> e)
        {
            if (e.Actor.Name.Equals(this.TargetActorName))
            {
                this.CompleteTask();
            }
        }

        private void player_KilledActor(ActorEventArgs<IActor> e)
        {
            if (e.Actor.Name.Equals(this.TargetActorName))
            {
                //if (this.Type == TaskType.KillWithItem && e.Actor.Name != this.UseActorName)
                //{
                //    return;
                //}

                this.Count++;
                if (this.Count >= this.Quantity && !this.IsComplete)
                {
                    this.CompleteTask();
                }
            }
        }

        private void player_ItemReceived(ActorEventArgs<IItem> e)
        {
            if (e.Actor.Name.Equals(this.TargetActorName))
            {
                this.Count++;
                if (this.Count >= this.Quantity && !this.IsComplete)
                {
                    this.CompleteTask();
                }
            }
        }

        private void player_ItemDropped(ActorEventArgs<IItem> e)
        {
            if (e.Actor.Name.Equals(this.TargetActorName))
            {
                this.Count--;
                if (this.Count < this.Quantity && this.IsComplete)
                {
                    this.IsComplete = false;
                }
            }
        }

        private void player_PlaceEntered(ActorEventArgs<IPlayer> e)
        {
            switch (this.TargetActorType)
            {
                case ObjectType.Actor:
                case ObjectType.Mobile:
                case ObjectType.Player:
                    // If the actor exists in the current place then they have been discovered.
                    var actor = e.Actor.Place.GetAllChildren().Where(c => c.Name.Equals(this.TargetActorName)
                        && (c.ObjectType == ObjectType.Actor || c.ObjectType == ObjectType.Mobile || c.ObjectType == ObjectType.Player)).FirstOrDefault();
                    if (actor != null && !this.IsComplete)
                    {
                        this.CompleteTask();
                    }
                    break;
                case ObjectType.Place:
                    if (e.Actor.Place.Name.Equals(this.TargetActorName) && !this.IsComplete)
                    {
                        this.CompleteTask();
                    }
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Returns the current object as a string.
        /// </summary>
        /// <returns>The string representation of the current object.</returns>
        public override string ToString()
        {
            return this.ToJson();
        }

        /// <summary>
        /// Returns a Task instance from the specified string.
        /// </summary>
        /// <param name="stringValue">The string to parse into a Task instance.</param>
        /// <returns>An instance of Task.</returns>
        public static Task FromString(string stringValue)
        {
            return JsonHelper.FromJson<Task>(stringValue);
        }
    }
    #endregion

    #region TaskCollection
    public class TaskCollection : ActorOwnedDictionaryBase<string>
    {
        public event TaskEventHandler TaskCompleted = delegate { };

		/// <summary>
        /// Gets the Task instance with the specified key.
		/// </summary>
		/// <param name="key">The unique key used to located the desired item.</param>
        /// <returns>An instance of Task.</returns>
		public new Task this[string key]
		{
            get { return Task.FromString(this.Owner.Properties.GetValue<string>(this.GetPrefixedName(key))); }
			set { this.Owner.Properties.SetValue(this.GetPrefixedName(key), value.ToString(), this.IsTemplateCollection); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not this template item collection is specified on a template 
		/// record and therefore should have its contents contained on the template control.
		/// </summary>
		public bool IsTemplateCollection { get; set; }	

		/// <summary>
        /// Initializes a new instance of the TaskCollection class.
		/// </summary>
		/// <param name="owner">The owner of the current instance.</param>
        public TaskCollection(IActor owner)
			: base(owner, "Task_")
		{
		}

		/// <summary>
        /// Adds a new Task to the collection.
		/// </summary>
		/// <param name="item">The item to add to the collection.</param>
        public void Add(Task item)
		{
			this.Add(item.Name, item.ToString());
            item.Completed += new TaskEventHandler(task_Completed);
		}

		/// <summary>
		/// Adds a new item to the collection.
		/// </summary>
		/// <param name="key">The key of the item to add.</param>
		/// <param name="value">The ToString value of the template item to add.</param>
		public override void Add(string key, string value)
		{
			this.Owner.Properties.SetValue(this.GetPrefixedName(key), value, this.IsTemplateCollection);
		}

		/// <summary>
		/// Tries to get the value at the specified key.
		/// </summary>
		/// <param name="key">The key in which to search the collection.</param>
        /// <param name="value">The JSON serialized value of the Task instance.</param>
		/// <returns>True if the item was found; otherwise false.</returns>
		public override bool TryGetValue(string key, out string value)
		{
			if (this.Owner.Properties.ContainsKey(this.GetPrefixedName(key)))
			{
				value = this.Owner.Properties.GetValue<string>(this.GetPrefixedName(key));
				return true;
			}
            value = String.Empty; ;
			return false;
		}

		public bool ContainsItem(string name)
		{
			foreach (var t in this)
			{
				if (t.Name.Equals(name)) return true;
			}
			return false;
		}

        public IEnumerable<Task> GetTasks()
		{
            List<Task> list = new List<Task>();
			foreach (var t in this)
			{
				list.Add(t);
			}
			return list.ToArray();
		}

		/// <summary>
        /// Gets an IEnumerator of Task instances contained within the current collection.
		/// </summary>
        /// <returns>An IEnumerator of Task instances.</returns>
        public new IEnumerator<Task> GetEnumerator()
		{
            List<Task> list = new List<Task>();
			foreach (var item in this.Values)
			{
                list.Add(Task.FromString(item));
			}
			return list.GetEnumerator();
		}

        private void EnsureTaskEvents()
        {
            foreach (var task in this)
            {
                if (!task.HasTaskEventHandler())
                {
                    task.Completed += new TaskEventHandler(task_Completed);
                }
            }
        }

        private void task_Completed(TaskEventArgs e)
        {
            this.TaskCompleted(e);
        }

        public void HookEvents(Character player)
        {
            this.EnsureTaskEvents();
            this.SetEventHandlers(player, true);
        }

        public void UnHookEvents(Character player)
        {
            this.SetEventHandlers(player, false);
        }

        private void SetEventHandlers(Character player, bool add)
        {
            // Based on the tasks types, hook up events to the player object.
            foreach (var task in this)
            {
                task.SetEventHandlers(player, add);
            }
        }

        public bool HasCompletedAllTasks(Character player)
        {
            int completeCount = 0;
            foreach (var task in this)
            {
                if (task.IsComplete) completeCount++;
                //switch (task.Type)
                //{
                //    case TaskType.Discovery:
                //        break;
                //    case TaskType.Collection:
                //        break;
                //    case TaskType.Kill:
                //        if (!String.IsNullOrEmpty(task.TargetActorName))
                //        {
                //        }
                //        else
                //        {
                //            if (task.IsQuantityAdditional)
                //            {
                //            }
                //            else if (task.IsQuantitySequential)
                //            {
                //            }
                //            else
                //            {
                //                // TODO: Only condition for now.
                //                if (player.TotalKills >= task.Quantity)
                //                {
                //                    completeCount++;
                //                }
                //            }
                //        }
                //        break;
                //    case TaskType.KillWithItem:
                //        break;
                //    case TaskType.Delivery:
                //        break;
                //}
            }
            return completeCount == this.Count;
        }
    }
    #endregion

    public delegate void TaskEventHandler(TaskEventArgs e);
    public class TaskEventArgs : EventArgs
    {
        public Task Task { get; set; }

        public TaskEventArgs(Task task)
        {
            this.Task = task;
        }
    }
}
