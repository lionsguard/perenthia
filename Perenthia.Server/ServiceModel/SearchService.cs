﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radiance.Contract;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Perenthia.ServiceModel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
	[ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
	public class SearchService : ISearchService
	{
		public AvatarData CreateAvatar()
		{
			return new AvatarData();
		}

		public SearchResults Search(string query, string searchType, string sortExpression, string sortDirection, int startingRowIndex, int maxRows)
		{
			SearchResults results = new SearchResults();
			try
			{
				QueryType queryType = QueryType.All;
				if (Lionsguard.EnumHelper.TryParse<QueryType>(searchType, out queryType))
				{
					List<QueryType> types = new List<QueryType>();
					if (queryType == QueryType.All)
					{
						types.AddRange(new QueryType[] { QueryType.Characters, QueryType.Creatures, QueryType.Households, QueryType.Items });
					}
					else
					{
						types.Add(queryType);
					}

					// Page Counts
					foreach (var type in types)
					{
						string pageCountQuery = this.GetPageCountQuery(query, type);
						results.PageCounts.Add(type.ToString(), Game.Server.World.Provider.GetPageCount(pageCountQuery));
					}

					// Data
					if (queryType == QueryType.All) queryType = QueryType.Characters;
					string searchQuery = this.GetSearchQuery(query, queryType, sortExpression, sortDirection, startingRowIndex, maxRows);
					switch (queryType)
					{
						case QueryType.Characters:
							results.Data = Game.Server.World.Provider.GetPlayersFromQuery(searchQuery).ToServiceArray();
							break;
						case QueryType.Households:
							break;
						case QueryType.Items:
							break;
						case QueryType.Creatures:
							break;
					}
				}
			}
			catch (Exception ex)
			{
#if DEBUG
				throw ex;
#else
                Lionsguard.Log.Write(ex.ToString(), true);
#endif
			}
			return results;
		}

		private string GetPageCountQuery(string query, QueryType type)
		{
			try
			{
				// Clean up and ensure values for the query and type.
				if (String.IsNullOrEmpty(query)) query = String.Empty;
				else query = this.FixQuery(query);

				string where = String.Empty;
				StringBuilder sql = new StringBuilder();

				switch (type)
				{
					case QueryType.Characters:
						if (!String.IsNullOrEmpty(query))
						{
							where = String.Format(" AND FirstName LIKE '%{0}%' ", query);
						}
						sql.AppendFormat(QueryCharacterPageCount, Game.Server.World.ID, where);
						break;
					case QueryType.Households:
						if (!String.IsNullOrEmpty(query))
						{
							where = String.Format(" AND HouseholdName LIKE '%{0}%' ", query);
						}
						sql.AppendFormat(QueryHouseholdPageCount, Game.Server.World.ID, where);
						break;
					case QueryType.Items:
						break;
					case QueryType.Creatures:
						break;
					case QueryType.HouseholdMembers:
						sql.AppendFormat("SELECT COUNT(*) FROM dbo.rad_Players WHERE HouseholdId = {0}", query);
						break;
				}

				return sql.ToString();
			}
			catch (Exception ex)
			{
#if DEBUG
				throw ex;
#else
                Lionsguard.Log.Write(ex.ToString(), true);
                return String.Empty;
#endif
			}
		}

		private string GetSearchQuery(string query, QueryType type, string sortExpression, string sortDirection, int startingRowIndex, int maxRows)
		{
			try
			{
				// Clean up and ensure values for the query and type.
				if (String.IsNullOrEmpty(query)) query = String.Empty;
				else query = this.FixQuery(query);

				// Increment the starting row index by 1.
				startingRowIndex += 1;

				string where = String.Empty;
				string rownum = String.Empty;
				StringBuilder sql = new StringBuilder();

				switch (type)
				{
					case QueryType.Characters:
						if (!String.IsNullOrEmpty(query))
						{
							where = String.Format(" AND ObjectName LIKE '%{0}%' ", query);
						}
						if (String.IsNullOrEmpty(sortExpression)) sortExpression = "level";
						if (String.IsNullOrEmpty(sortDirection)) sortDirection = "DESC";
						switch (sortExpression.ToLower())
						{
							case "level":
								rownum = String.Format("ROW_NUMBER() OVER(ORDER BY Properties.value('(/properties/property[@name=\"Level\"])[1]', 'int') {0}) AS RowNum,", sortDirection);
								break;
							case "name":
								rownum = String.Format("ROW_NUMBER() OVER(ORDER BY ObjectName {0}) AS RowNum,", sortDirection);
								break;
							case "household":
								rownum = String.Format("ROW_NUMBER() OVER(ORDER BY HouseholdName {0}) AS RowNum,", sortDirection);
								break;
						}
						sql.AppendFormat(QuerySearchCharacters, rownum, Game.Server.World.ID, where, startingRowIndex, maxRows);
						break;
					case QueryType.Households:
						if (!String.IsNullOrEmpty(query))
						{
							where = String.Format(" AND HouseholdName LIKE '%{0}%' ", query);
						}
						if (String.IsNullOrEmpty(sortExpression)) sortExpression = "honorpoints";
						if (String.IsNullOrEmpty(sortDirection)) sortDirection = "DESC";
						switch (sortExpression.ToLower())
						{
							case "honorpoints":
								rownum = String.Format("ROW_NUMBER() OVER(ORDER BY Properties.value('(/properties/property[@name=\"HonorPoints\"])[1]', 'int') {0}) AS RowNum,", sortDirection);
								break;
							case "name":
								rownum = String.Format("ROW_NUMBER() OVER(ORDER BY HouseholdName {0}) AS RowNum,", sortDirection);
								break;
						}
						sql.AppendFormat(QuerySearchHouseholds, rownum, Game.Server.World.ID, where, startingRowIndex, maxRows);
						break;
					case QueryType.Items:
						break;
					case QueryType.Creatures:
						break;
					case QueryType.HouseholdMembers:
						break;
				}

				return sql.ToString();
			}
			catch (Exception ex)
			{
#if DEBUG
				throw ex;
#else
                Lionsguard.Log.Write(ex.ToString(), true);
                return String.Empty;
#endif
			}
		}
		#region Queries
		public const string QuerySearchCharacters = @"
    SELECT 
		*
    FROM
		(
		SELECT {0}
			*
		FROM
			dbo.rad_vw_Players
		WHERE	
			WorldId = {1}
            {2}
		) AS Players
	WHERE 
		RowNum BETWEEN {3} AND ({3} + {4}) - 1
";
		public const string QueryCharacterPageCount = @"SELECT COUNT(*) FROM dbo.rad_Players WHERE WorldId = {0} {1}";
		public const string QuerySearchHouseholds = @"
    SELECT 
		*
    FROM
		(
		SELECT {0}
			*
		FROM
			dbo.rad_Households
		WHERE	
			WorldId = {1}
            {2}
		) AS Households
	WHERE 
		RowNum BETWEEN {3} AND ({3} + {4}) - 1
";
		public const string QueryHouseholdPageCount = @"SELECT COUNT(*) FROM dbo.rad_Households WHERE WorldId = {0} {1}";
		#endregion

		private string FixQuery(string query)
		{
			query = System.Web.HttpUtility.UrlEncode(query);
			query = query.Replace("'", String.Empty);
			return query;
		}
	}
}
