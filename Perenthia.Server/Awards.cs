using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia
{
    #region AwardManager
    public static class AwardManager
    {
        private static bool _initialized = false;
        private static object _lock = new object();

        private static List<Award> _awards = null;

        private static List<Award> GetAwards()
        {
            if (!_initialized)
            {
                lock (_lock)
                {
                    if (!_initialized)
                    {
                        _awards = new List<Award>();
                        _awards.AddRange(Game.Server.World.Provider.GetTemplates(typeof(Award)).Select(a => a as Award));
                    }
                }
            }
            return _awards.ToList();
        }

        public static void IssueAwardIfAble(Character player)
        {
            // Find any awards for the player.
            foreach (var award in GetAwards())
            {
                if (!player.Awards.Contains(award) && award.Tasks.HasCompletedAllTasks(player))
                {
                    Award newAward = Game.Server.World.CreateFromTemplate<Award>(award.Name);
                    player.Awards.Add(newAward);
                    newAward.Save();

                    player.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Award, String.Format(Resources.AwardGained, newAward.Name)));
                    player.Context.AddRange(newAward.ToRdl());
                }
            }
        }
    }
    #endregion

    #region Award
    public class Award : Item
    {
        public TaskCollection Tasks { get; private set; }

        public Award()
            : base()
        {
            this.ObjectType = ObjectType.Award;
            this.Tasks = new TaskCollection(this);
        }

        public override void Use(IActor user, IMessageContext context)
        {
        }
    }
    #endregion

    #region AwardCollection
    public class AwardCollection : ActorOwnedItemCollection<Award>
    {
        public AwardCollection(IActor owner)
            : base(owner, "Award_")
        {
        }
    }
    #endregion
}
