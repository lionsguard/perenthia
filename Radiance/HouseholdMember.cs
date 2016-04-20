using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
    public class HouseholdMember
    {
        public int PlayerID { get; set; }
        public bool IsApproved { get; set; }
        public List<HouseholdTitle> Titles { get; set; }    
        public PropertyCollection Properties { get; set; }

        public HouseholdMember()
        {
            this.Properties = new PropertyCollection();
            this.Titles = new List<HouseholdTitle>();
        }
    }

	public class HouseholdRank
	{
		public string HouseholdName { get; set; }
		public string HouseholdImageUri { get; set; }
		public string RankName { get; set; }
		public string RankImageUri { get; set; }	
	}
}
