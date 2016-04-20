using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia
{
	#region Room
	public class Room : Place
	{
		public Room()
			: base()
		{
			Init();
		}

		public Room(RdlPlace placeTag)
			: base(placeTag)
		{
			Init();
		}

		protected virtual void Init()
		{
		}

		protected virtual IEnumerable<PerenthiaMobile> GetHostileMobiles(IAvatar traveler)
		{
			return (from c in this.GetAllChildren() where c is PerenthiaMobile && (c as PerenthiaMobile).IsHostile select (c as PerenthiaMobile).GetOrCreateInstance(traveler.ID));
		}

		protected virtual void SetInitiative(IAvatar avatar, IMobile mobile)
		{
			CombatManager.DetermineInitiative(InitiativeType.Cyclic, avatar, mobile);
		}

		public override void OnEnter(IActor sender, IMessageContext context, Direction direction)
		{
			IAvatar avatar = sender as IAvatar;
			if (avatar != null)
			{
				//this.SendAll(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
				//    String.Format(Resources.PlaceAvatarEnters, avatar.FirstName, direction.Name)),
				//    avatar);

				// Cast the avatar as a Character instance for quest and monster checks.
				Character player = avatar as Character;

				// If the place is not safe and is occupied by a Mobile then determine if the mobile
				// should attack the player.
				if (!this.IsSafe && player != null)
				{
					// Get hostile mobiles actually retrieves from the Instances property of the mobiles.
					var mobiles = this.GetHostileMobiles(avatar);
					foreach (var mob in mobiles)
					{
						if (mob == null)
							continue;

						// Allow mobiles to re-spawn even if way lower level than the player.
						//// If the mobile is lower than 2 levels from the player then don't bother
						//// having it attack the player.
						//if (mob.Level < (player.Level - 4))
						//{
						//    continue;
						//}

						// If the mobile was recently killed by this player then skip over it.
						if (mob.KilledBy.WasKilledBy(player.ID, DateTime.Now))
						{
							continue;
						}

						// If dead, check for and respawn as needed.
						if (mob.IsDead)
						{
							// Do not respawn if last killed by the current player.
							if (mob.RespawnTime < DateTime.Now.Subtract(mob.RespawnDelay))
							{
								mob.GenerateStats();
								mob.IsDead = false;
								mob.Target = null;
							}
						}

						// If the mobile is not dead, check for attack initiative and set the current player target.
						if (!mob.IsDead && mob.Target == null)
						{
							player.SetTarget(mob);

							if (mob != null)
							{
								// Only allow mob to attack first if player is not too far over the mob's level.
								if (player.Level <= (mob.Level + 4))
								{
									this.SetInitiative(player, mob);
									// Lowest initiative attacks first.
									if (mob.Initiative < player.Initiative)
									{
										// Mobile should attack player.
										mob.Attack(player, context);
									}
								}
							}

							// Exit the mobiles loop.
							break;
						}
					}
				}
			}
		}

		protected override IEnumerable<IActor> GetVisibleChildren(IAvatar who)
		{
			var id = 0;
			if (who != null)
				id = who.ID;

			var actors = (from c in this.Children
					where (!(c is PerenthiaMobile) && !(c is Quest))
					select c).ToList();

			var mobs = this.Children.Where(c => (c is PerenthiaMobile)).Select(c => (c as PerenthiaMobile).GetOrCreateInstance(id));
			foreach (var mob in mobs)
			{
				actors.Add(mob);
			}

			return actors;
		}

		public override void OnExit(IActor sender, IMessageContext context, Direction direction)
		{
			IAvatar avatar = sender as IAvatar;
			if (avatar != null)
			{
				//this.SendAll(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
				//    String.Format(Resources.PlaceAvatarExits, avatar.FirstName, direction.Name)),
				//    avatar);

				var avatars = (from c in this.Children where c is IAvatar select c as IAvatar);
				var whoRdl = avatar.ToSimpleRdl();
				foreach (var a in avatars)
				{
					if (a.ID != avatar.ID)
					{
						// Send down a command to remove this avatar from the the current room.
						a.Context.Add(new RdlCommand("AVATARREMOVE", avatar.ID));
					}
				}
			}
		}

		public override void Update()
		{
			var mobiles = GetVisibleChildren(null).Where(c => c.ObjectType == ObjectType.Mobile);
			foreach (var mob in mobiles)
			{
				(mob as Avatar).Heartbeat();
			}
		}
	}
	#endregion

	#region Dungeon
	public class Dungeon : Room
	{
		public Dungeon()
			: base()
		{
		}

		public Dungeon(RdlPlace placeTag)
			: base(placeTag)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.IsSafe = false;
		}

		//protected override void SetInitiative(IAvatar avatar, IMobile mobile)
		//{
		//    // mobile always attacks first in a dungeon, very hostile environment.
		//    mobile.Initiative = 1;
		//    avatar.Initiative = 2;
		//}
	}
	#endregion

	#region DungeonEntrance
	public class DungeonEntrance : Dungeon
	{
		public int DiscoveryExperience
		{
			get { return this.Properties.GetValue<int>(DiscoveryExperienceProperty); }
			set { this.Properties.SetValue(DiscoveryExperienceProperty, value); }
		}
		public static readonly string DiscoveryExperienceProperty = "DiscoveryExperience";

		public DungeonEntrance()
			: base()
		{
		}

		public DungeonEntrance(RdlPlace placeTag)
			: base(placeTag)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.DiscoveryExperience = 25;
		}

		public override void OnEnter(IActor sender, IMessageContext context, Direction direction)
		{
			base.OnEnter(sender, context, direction);

			Character player = sender as Character;
			if (player != null)
			{
				if (!player.Properties.GetValue<bool>(this.GetDungeonName()))
				{
					context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive, 
						String.Format(Resources.PlaceDiscovered, this.The())));

					int xp = this.DiscoveryExperience;
					player.Experience += xp;
					player.TotalExperience += xp;

                    player.TotalDiscoveries++;

					context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
						String.Format(Resources.PlaceDiscoveryExperienceGained, xp)));

					context.AddRange(player.GetRdlProperties(Character.ExperienceProperty, Character.ExperienceMaxProperty));

					LevelManager.AdvanceIfAble(player);

					player.Properties.SetValue(this.GetDungeonName(), true);
				}
			}
		}

		private string GetDungeonName()
		{
			return String.Format("HasDiscovered_{0}", this.Alias);
		}
	}
	#endregion

	#region DungeonFinalRoom
	public class DungeonFinalRoom : DungeonEntrance
	{
		public DungeonFinalRoom()
			: base()
		{
		}

		public DungeonFinalRoom(RdlPlace placeTag)
			: base(placeTag)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.DiscoveryExperience = 50;
		}
	}
	#endregion

	#region Temple
	public class Temple : Room
	{
		public Temple()
			: base()
		{	
		}

		public Temple(RdlPlace placeTag)
			: base(placeTag)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.IsSafe = true;
		}

		public void Resurrect(IAvatar avatar)
		{
			avatar.SetBody(avatar.BodyMax);
			avatar.SetMind(avatar.MindMax);
			avatar.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive, "You have been resurrected!"));
			avatar.Context.AddRange(avatar.GetRdlProperties(
				Avatar.BodyProperty, 
				Avatar.BodyMaxProperty,
				Avatar.MindProperty,
				Avatar.MindMaxProperty));

			this.Enter(avatar, Direction.Empty);
            // Don't want to send down the whole map. Just send down the zone.
			//avatar.Context.AddRange(this.World.GetRdlMap(avatar.X, avatar.Y));

            Character player = avatar as Character;
            if (player != null)
            {
                MapManager.MapDetail detail = this.World.Map.GetDetail(player.Location);
                if (detail != null)
                {
                    string zone = player.Properties.GetValue<string>(Character.ZoneProperty);
                    if (detail.Name != zone)
                    {
                        player.Properties.SetValue(Character.ZoneProperty, detail.Name);
                        player.Context.Add(new RdlProperty(player.ID, Character.ZoneProperty, zone));
                    }
                }
            }
		}
	}
    #endregion

    #region Wilderness
    public class Wilderness : Room
    {
        public int CreatureMinLevel
        {
            get { return this.Properties.GetValue<int>(CreatureMinLevelProperty); }
            set { this.Properties.SetValue(CreatureMinLevelProperty, value); }
        }
        public static readonly string CreatureMinLevelProperty = "CreatureMinLevel";

        public int CreatureMaxLevel
        {
            get { return this.Properties.GetValue<int>(CreatureMaxLevelProperty); }
            set { this.Properties.SetValue(CreatureMaxLevelProperty, value); }
        }
        public static readonly string CreatureMaxLevelProperty = "CreatureMaxLevel";

        public Wilderness()
            : base()
        {
        }

        public Wilderness(RdlPlace placeTag)
            : base(placeTag)
        {
        }

        protected override void Init()
        {
            base.Init();
            this.IsSafe = false;
        }

        protected override IEnumerable<PerenthiaMobile> GetHostileMobiles(IAvatar traveler)
        {
            if (traveler != null)
            {
                // TODO: If the player is riding in an armored carraige then no need to encounter anything...

                // Creatures are more active at night so increase the chance of an encounter if it is night.
                int encounterModifier = Time.GetTwentyFourHourValue();
                if (encounterModifier == 0) encounterModifier = 12;
                else if (encounterModifier > 0 && encounterModifier < 3) encounterModifier += 12;
                else if (encounterModifier > 6 && encounterModifier < 18) encounterModifier = 0;

                if (Dice.EncounterRoll(encounterModifier, 0))
                {
                    var mobiles = base.GetHostileMobiles(traveler);
                    if (mobiles.Count() == 0)
                    {
						var list = this.World.Provider.GetTemplates(typeof(Creature))
                            .Where(c => c.Properties.GetValue<int>(Creature.LevelProperty) >= this.CreatureMinLevel 
                                && c.Properties.GetValue<int>(Creature.LevelProperty) <= this.CreatureMaxLevel).ToList();
						
                        int index = Dice.Random(0, list.Count() - 1);
                        IAvatar mobile = (IAvatar)list[index].Clone();
                        mobile.ID = Game.GetNextTempObjectId(); // mobile is a template, need and instance.

						this.Enter(mobile, Direction.Empty);
                        return base.GetHostileMobiles(traveler);
                    }
                }
            }
            return new PerenthiaMobile[0];
        }
    }
    #endregion
}
