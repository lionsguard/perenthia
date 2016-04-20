using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance
{
    /// <summary>
    /// Represents a player run group.
    /// </summary>
    public class Household : GameObject
    {
        public string ImageUri { get; set; }
        public DateTime DateCreated { get; set; }
        public int MemberCount { get; set; }    
        public Dictionary<int, HouseholdRelationType> Relations { get; private set; }
		public List<HouseholdRank> Ranks { get; private set; }	

        /// <summary>
        /// Gets or sets the Motto property value using the underlying Properties collection.
        /// </summary>
        public string Motto
        {
            get { return this.Properties.GetValue<string>(MottoPropertyName); }
            set { this.Properties.SetValue(MottoPropertyName, value); }
        }
        /// <summary>
        /// Gets the name of the Motto property as stored in the Properties collection.
        /// </summary>
        public const string MottoPropertyName = "Motto";

        /// <summary>
        /// Gets or sets the Description property value using the underlying Properties collection.
        /// </summary>
        public string Description
        {
            get { return this.Properties.GetValue<string>(DescriptionPropertyName); }
            set { this.Properties.SetValue(DescriptionPropertyName, value); }
        }   
        /// <summary>
        /// Gets the name of the Description property as stored in the Properties collection.
        /// </summary>
        public const string DescriptionPropertyName = "Description";

        /// <summary>
        /// Gets or sets the HonorPoints property value using the underlying Properties collection.
        /// </summary>
        public int HonorPoints
        {
            get { return this.Properties.GetValue<int>(HonorPointsPropertyName); }
            set { this.Properties.SetValue(HonorPointsPropertyName, value); }
        }
        /// <summary>
        /// Gets the name of the HonorPoints property as stored in the Properties collection.
        /// </summary>
        public const string HonorPointsPropertyName = "HonorPoints";

		/// <summary>
		/// Gets or sets the MembershipRequiresApproval property value using the underlying Properties collection.
		/// </summary>
		public bool MembershipRequiresApproval
		{
			get { return this.Properties.GetValue<bool>(MembershipRequiresApprovalPropertyName); }
			set { this.Properties.SetValue(MembershipRequiresApprovalPropertyName, value); }
		}	
		/// <summary>
		/// Gets the name of the MembershipRequiresApproval property as stored in the Properties collection.
		/// </summary>
		public const string MembershipRequiresApprovalPropertyName = "MembershipRequiresApproval";

        public Household()
        {
            this.Relations = new Dictionary<int, HouseholdRelationType>();
			this.Ranks = new List<HouseholdRank>();
        }
    }

    public class HouseholdRank
    {
		public const int MaxRankOrder = 15;

        public int ID { get; set; }
        public string Name { get; set; }
        public string ImageUri { get; set; }
        public string TitleMale { get; set; }
        public string TitleFemale { get; set; } 
        public HouseholdPermissions Permissions { get; set; }
        public int Order { get; set; }
        public int RequiredMemberCount { get; set; }
        public int EmblemCost { get; set; }

        public string GetTitle(Gender gender)
        {
            switch (gender)
            {
                case Gender.Male: return this.TitleMale;
                case Gender.Female: return this.TitleFemale;
            }
            return String.Empty;
        }
    }

    public class HouseholdInfo
    {
        public int HouseholdID { get; set; }
        public string HouseholdName { get; set; }
        public string HouseholdImageUri { get; set; }
        public int RankID { get; set; }
        public string RankName { get; set; }
        public string RankImageUri { get; set; }
		public int RankOrder { get; set; }	
        public string Title { get; set; }

		public RdlProperty[] ToRdlProperties(IPlayer owner)
		{
			return new RdlProperty[]
			{
				new RdlProperty(owner.ID, "HouseholdName", this.HouseholdName),
				new RdlProperty(owner.ID, "HouseholdImageUri", this.HouseholdName),
				new RdlProperty(owner.ID, "RankName", this.HouseholdName),
				new RdlProperty(owner.ID, "RankImageUri", this.HouseholdName),
				new RdlProperty(owner.ID, "RankOrder", this.HouseholdName)
			};
		}

		public static HouseholdInfo GetHouseholdInfo(Gender gender, Household household, HouseholdRank rank)
		{
			HouseholdInfo info = new HouseholdInfo();
			info.HouseholdID = household.ID;
			info.HouseholdImageUri = household.ImageUri;
			info.HouseholdName = household.Name;
			info.RankID = rank.ID;
			info.RankImageUri = rank.ImageUri;
			info.RankName = rank.Name;
			info.RankOrder = rank.Order;
			if (!String.IsNullOrEmpty(rank.TitleMale) && !String.IsNullOrEmpty(rank.TitleFemale))
			{
				info.Title = (gender == Gender.Male) ? rank.TitleMale : rank.TitleFemale;
			}
			return info;
		}
    }
}
