using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

using Lionsguard;
using Lionsguard.Providers;
using Lionsguard.Data;

namespace Lionsguard.Security
{
	public class SqlRoleProvider : System.Web.Security.RoleProvider
	{
		private string _appName;
		private string _connectionString;

		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			base.Initialize(name, config);

			_appName = ProviderUtil.GetAppName(config);
			_connectionString = ProviderUtil.GetConnectionString(config);

			ProviderUtil.CheckUnrecognizedAttributes(config);
		}

		public override string ApplicationName
		{
			get
			{
				return _appName;
			}
			set
			{
				_appName = value;
			}
		}

		public override void AddUsersToRoles(string[] usernames, string[] roleNames)
		{
			foreach (string username in usernames)
			{
				foreach (string role in roleNames)
				{
					SqlHelper.ExecuteNonQuery(_connectionString, "dbo.lg_Roles_AddUserToRole",
						SqlHelper.CreateInputParam("@UserName", SqlDbType.NVarChar, username),
						SqlHelper.CreateInputParam("@RoleName", SqlDbType.NVarChar, role));
				}
			}
		}

		public override void CreateRole(string roleName)
		{
			SqlHelper.ExecuteNonQuery(_connectionString, "dbo.lg_Roles_CreateRole",
				SqlHelper.CreateInputParam("@RoleName", SqlDbType.NVarChar, roleName));
		}

		public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
		{
			throw new NotSupportedException("Deletion of roles not permitted.");
		}

		public override string[] FindUsersInRole(string roleName, string usernameToMatch)
		{
			List<string> users = new List<string>();
			string[] allUsers = this.GetUsersInRole(roleName);
			for (int i = 0; i < allUsers.Length; i++)
			{
				if (allUsers[i].Equals(usernameToMatch))
				{
					users.Add(allUsers[i]);
				}
			}
			return users.ToArray();
		}

		public override string[] GetAllRoles()
		{
			List<string> list = new List<string>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Roles_GetAll"))
			{
				while (reader.Read())
				{
					list.Add(reader.GetString("RoleName"));
				}
			}
			return list.ToArray();
		}

		public override string[] GetRolesForUser(string username)
		{
			List<string> list = new List<string>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Roles_GetRolesForUser",
				SqlHelper.CreateInputParam("@UserName", SqlDbType.NVarChar, username)))
			{
				while (reader.Read())
				{
					list.Add(reader.GetString("RoleName"));
				}
			}
			return list.ToArray();
		}

		public override string[] GetUsersInRole(string roleName)
		{
			List<string> list = new List<string>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Roles_GetUsersInRole",
				SqlHelper.CreateInputParam("@RoleName", SqlDbType.NVarChar, roleName)))
			{
				while (reader.Read())
				{
					list.Add(reader.GetString("UserName"));
				}
			}
			return list.ToArray();
		}

		public override bool IsUserInRole(string username, string roleName)
		{
			object obj = SqlHelper.ExecuteScalar(_connectionString, "dbo.lg_Roles_IsUserInRole",
				SqlHelper.CreateInputParam("@UserName", SqlDbType.NVarChar, username),
				SqlHelper.CreateInputParam("@RoleName", SqlDbType.NVarChar, roleName));
			if (obj != null && obj != DBNull.Value)
			{
				return true;
			}
			return false;
		}

		public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
		{
			throw new NotSupportedException("Removing users from roles in batch is not permitted.");
		}

		public override bool RoleExists(string roleName)
		{
			object obj = SqlHelper.ExecuteScalar(_connectionString, "dbo.lg_Roles_GetRole",
				SqlHelper.CreateInputParam("@RoleName", SqlDbType.NVarChar, roleName));
			if (obj != null && obj != DBNull.Value)
			{
				return true;
			}
			return false;
		}
	}
}
