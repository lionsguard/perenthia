using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Radiance.Contract
{
	[ServiceContract(Namespace = Constants.ServiceNamespace)]
	public interface IArmorialService
	{
		[OperationContract]
		List<RaceData> GetRaces();

		[OperationContract]
		List<SkillData> GetSkills();

		[OperationContract]
		List<NameValuePair> GetAttributeDetails();

		[OperationContract]
		AvatarData GetFeaturedCharacter();

		[OperationContract]
		string GetGameVersion();

		[OperationContract]
		NameValuePair[] GetCharacterSkills(int playerId);

		[OperationContract]
		AvatarData FindCharacter(string name);
		
		[OperationContract]
		AvatarData FindCreature(string name);

		[OperationContract]
		ItemData FindItem(string name);

		[OperationContract]
		HouseholdData FindHousehold(string name);

		[OperationContract]
		IEnumerable<AvatarData> GetHouseholdMembers(int householdId, int startingRowIndex, int maxRows);

		[OperationContract]
		SearchResults Search(string query,
			string searchType,
			string sortExpression,
			string sortDirection,
			int startingRowIndex,
			int maxRows);
	}
}
