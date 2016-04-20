using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
    public class HouseholdManager
    {
        private object _householdLock = new object();

        private Dictionary<int, Household> _households = new Dictionary<int,Household>();

        public World World { get; private set; }

        public HouseholdManager(World world)
        {
            this.World = world;
        }

        public Household GetHousehold(int id)
        {
            if (!_households.ContainsKey(id))
            {
                lock (_householdLock)
                {
                    if (!_households.ContainsKey(id))
                    {
                        Household household = this.World.Provider.GetHousehold(id);
						if (household != null)
						{
							_households.Add(household.ID, household);
						}
                    }
                }
            }
            if (_households.ContainsKey(id))
            {
                return _households[id];
            }
            return null;
        }

		public Household GetHousehold(string name)
		{
			int id = 0;
			lock (_householdLock)
			{
				id = _households.Values.Where(h => String.Compare(h.Name, name, true) == 0).Select(h => h.ID).FirstOrDefault();
			}
			if (id > 0)
			{
				return GetHousehold(id);
			}
			else
			{
				Household household = this.World.Provider.GetHousehold(name);
				if (household != null)
				{
					_households.Add(household.ID, household);
				}
				return household;
			}
		}

        public IEnumerable<Household> GetHouseholds()
        {
            return this.World.Provider.GetHouseholds();
        }

		public IEnumerable<IPlayer> GetMembers(int householdId, int startingRowIndex, int maxRows)
        {
            return this.World.Provider.GetHouseholdMembers(householdId, startingRowIndex, maxRows);
        }

        public IEnumerable<HouseholdRank> GetHouseholdRanks(int householdId)
        {
			var household = GetHousehold(householdId);
			if (household != null)
			{
				return household.Ranks;
			}
            return this.World.Provider.GetHouseholdRanks(householdId);
        }

        public IEnumerable<IItem> GetHouseholdArmory(int householdId)
        {
            return this.World.Provider.GetHouseholdArmory(householdId);
        }
    }
}
