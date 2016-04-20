using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
	public abstract class QuestManager
	{
		protected World World { get; private set; }
		protected List<IQuest> InnerList { get; private set; }

		protected QuestManager(World world)
		{
			this.World = world;
			this.InnerList = new List<IQuest>();
		}

		public abstract IEnumerable<IQuest> GetQuests();
		public abstract IEnumerable<IQuest> GetQuests(PropertyCollection filterProperties);

		public abstract IQuest GetQuest(int id);
		public abstract IQuest GetQuest(string alias);

		public abstract IQuest GetParent(IQuest quest);
	}
}
