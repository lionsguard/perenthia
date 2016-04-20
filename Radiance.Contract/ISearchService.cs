using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Radiance.Contract
{
	[ServiceContract(Namespace=Constants.ServiceNamespace)]
	public interface ISearchService
	{
		[OperationContract]
		AvatarData CreateAvatar();

		[OperationContract]
		SearchResults Search(string query,
			string searchType,
			string sortExpression,
			string sortDirection,
			int startingRowIndex,
			int maxRows);
	}
}
