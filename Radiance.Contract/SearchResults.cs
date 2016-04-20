using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

namespace Radiance.Contract
{
	[DataContract]
	[KnownType(typeof(NameValuePair))]
	[KnownType(typeof(GameObjectData))]
	[KnownType(typeof(ActorData))]
	[KnownType(typeof(AvatarData))]
	[KnownType(typeof(ItemData))]
	[KnownType(typeof(SkillData))]
	[KnownType(typeof(RaceData))]
	[KnownType(typeof(HouseholdData))]
	public class SearchResults
	{
		[DataMember]
		public Dictionary<string, int> PageCounts { get; set; }

		[DataMember]
		public IEnumerable Data { get; set; }

		public SearchResults()
		{
			this.PageCounts = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
		}
	}
}
