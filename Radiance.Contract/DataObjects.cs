using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Radiance.Contract
{
	#region NameValuePair
	[DataContract]
	public class NameValuePair
	{
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public int Value { get; set; }
		[DataMember]
		public string Description { get; set; }
	}
	#endregion

	#region GameObject
	[DataContract]
	public class GameObjectData
	{
		[DataMember]
		public int ID { get; set; }
		[DataMember]
		public string Name { get; set; }
	}
	#endregion

	#region Actor
	[DataContract]
	public class ActorData : GameObjectData
	{
		[DataMember]
		public int Type { get; set; }
		[DataMember]
		public string Description { get; set; }
	}
	#endregion

	#region Avatar
	[DataContract]
	public class AvatarData : ActorData
	{
		[DataMember]
		public string Zone { get; set; }
		[DataMember]
		public int X { get; set; }
		[DataMember]
		public int Y { get; set; }
		[DataMember]
		public int Z { get; set; }
		[DataMember]
		public string Race { get; set; }
		[DataMember]
		public string Gender { get; set; }
		[DataMember]
		public bool IsOnline { get; set; }
		[DataMember]
		public int Health { get; set; }
		[DataMember]
		public int HealthMax { get; set; }
		[DataMember]
		public int Willpower { get; set; }
		[DataMember]
		public int WillpowerMax { get; set; }
		[DataMember]
		public int Level { get; set; }
		[DataMember]
		public string HouseholdName { get; set; }
		[DataMember]
		public string HouseholdImageUri { get; set; }
		[DataMember]
		public string RankName { get; set; }
		[DataMember]
		public string RankImageUri { get; set; }
		[DataMember]
		public int RankOrder { get; set; }
		[DataMember]
		public NameValuePair[] Skills { get; set; }
		//[DataMember]
		//public AwardData[] Awards { get; set; }
	}
	#endregion

	#region Item
	[DataContract]
	public class ItemData : ActorData
	{
		[DataMember]
		public NameValuePair RequiredSkill { get; set; }
		[DataMember]
		public bool OwnerHasRequiredSkill { get; set; }
		[DataMember]
		public int GoldCost { get; set; }
		[DataMember]
		public int EmblemCost { get; set; }
		[DataMember]
		public bool IsViewerOwner { get; set; }
		[DataMember]
		public AvatarData Seller { get; set; }
	}
	#endregion

	#region Skill
	[DataContract]
	public class SkillData : ActorData
	{
		[DataMember]
		public string GroupName { get; set; }
		[DataMember]
		public int Value { get; set; }
	}
	#endregion

	#region Race
	[DataContract]
	public class RaceData : ActorData
	{
		[DataMember]
		public IEnumerable<NameValuePair> Attributes { get; set; }
	}
	#endregion

	#region Household
	[DataContract]
	public class HouseholdData : GameObjectData
	{
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public string ImageUri { get; set; }
		[DataMember]
		public string Motto { get; set; }
		[DataMember]
		public string Type { get; set; }
		[DataMember]
		public int HonorPoints { get; set; }
		[DataMember]
		public int MemberCount { get; set; }
		[DataMember]
		public DateTime DateCreated { get; set; }
		[DataMember]
		public AvatarData[] Members { get; set; }
	}
	#endregion
}
