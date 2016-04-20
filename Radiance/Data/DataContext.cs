using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Radiance.Configuration;
using Lionsguard;
using Lionsguard.Data;

namespace Radiance.Data
{
	/// <summary>
	/// Provides static methods and properties for accessing data for the current virtual world.
	/// </summary>
	public static class DataContext
	{
		private static string _connectionString;
		private static string _worldName;

		#region Initialize
		private static bool _initialized = false;
		private static object _initLock = new object();
		private static Exception _initException = null;

		private static void Initialize()
		{
			if (!_initialized)
			{
				lock (_initLock)
				{
					if (!_initialized)
					{
						try
						{
							RadianceSection section = ConfigurationManager.GetSection("radiance") as RadianceSection;
							if (section != null)
							{
								_connectionString = ConfigurationManager.ConnectionStrings[section.ConnectionStringName].ConnectionString;
								_worldName = section.WorldName;
							}
						}
						catch (Exception ex)
						{
							_initException = ex;
						}
						_initialized = true;
					}
				}
			}
			if (_initException != null)
			{
				throw _initException;
			}
		}
		#endregion

		/// <summary>
		/// Gets a value indicating whether or not the specified name exists.
		/// </summary>
		/// <param name="name">The name to verify.</param>
		/// <returns>True if the name exists; otherwise false.</returns>
		public static bool NameExists(string name)
		{
			Initialize();
			object obj = SqlHelper.ExecuteScalar(_connectionString, "dbo.rad_Objects_NameExists",
				SqlHelper.CreateInputParam("@Name", SqlDbType.NVarChar, name),
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, _worldName));
			if (obj != null && obj != DBNull.Value)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets an IActor instance for the specified id value.
		/// </summary>
		/// <typeparam name="T">An IActor derived instance.</typeparam>
		/// <param name="id">The id of the actor to retrieve.</param>
		/// <returns>An instance of the specified T for the specified id value.</returns>
		public static T GetActor<T>(int id) where T : IActor
		{
			Initialize();
			T obj = default(T);
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Objects_GetObject",
				SqlHelper.CreateInputParam("@ObjectId", SqlDbType.Int, id)))
			{
				if (reader.Read())
				{
					obj = GetActorFromReader<T>(reader);
				}
				AddActorsFromReader<T>(obj, reader);
			}
			return obj;
		}

		/// <summary>
		/// Gets a list of IActor derived instances where the properties match the specified queryProperties.
		/// </summary>
		/// <typeparam name="T">An IActor derived instance.</typeparam>
		/// <param name="queryProperties">The collection of properties used to query for the list of actors.</param>
		/// <returns>A List of IActor derived instances.</returns>
		public static List<T> GetActors<T>(PropertyCollection queryProperties) where T : IActor
		{
			Initialize();
			List<T> list = new List<T>();
			//// TODO: Create the parameters from the properties list.
			//using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Objects_GetObject",
			//    SqlHelper.CreateInputParam("@ObjectId", SqlDbType.Int, id)))
			//{
			//    while (reader.Read())
			//    {
			//        list.Add(GetActorFromReader<T>(reader));
			//    }
			//    T owner = (from o in list where o.ID == obj.Properties.GetValue<int>("OwnerID") select o).FirstOrDefault();
			//    if (owner != null)
			//    {
			//        AddActorsFromReader<T>(owner, reader);
			//    }
			//}
			return list;
		}

		/// <summary>
		/// Gets a list of IAvatar instances for the specified user.
		/// </summary>
		/// <typeparam name="T">An type derived from Radiance.IAvatar.</typeparam>
		/// <param name="username">The username of the player.</param>
		/// <returns>A List of IAvatar derived instances for the current user.</returns>
		public static List<T> GetPlayers<T>(string username) where T : IAvatar
		{
			Initialize();
			List<T> list = new List<T>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Objects_GetObjectsForUserName",
				SqlHelper.CreateInputParam("@UserName", SqlDbType.NVarChar, username),
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, _worldName)))
			{
				while (reader.Read())
				{
					list.Add(GetActorFromReader<T>(reader));
				}
				foreach (T owner in list)
				{
					if (owner != null)
					{
						AddActorsFromReader<T>(owner, reader);
					}
				}
			}
			return list;
		}

		private static T GetActorFromReader<T>(SqlNullDataReader reader) where T : IActor
		{
			Initialize();
			T obj = Activator.CreateInstance<T>();
			obj.ID = reader.GetInt32("ObjectId");
			obj.Name = reader.GetString("ObjectName");
			obj.Properties = XmlHelper.FromXml<PropertyCollection>(reader.GetSqlXml("Properties").CreateReader());
			return obj;
		}

		private static void AddActorsFromReader<T>(T owner, SqlNullDataReader reader) where T : IActor
		{
			if (owner != null && reader.NextResult())
			{
				while (reader.Read())
				{
					T obj = GetActorFromReader<T>(reader);
					obj.Owner = owner;
					// TODO: Add the object to the owner.
				}
			}
		}

		/// <summary>
		/// Saves the specified IActor instance to the database.
		/// </summary>
		/// <typeparam name="T">An IActor derived instance.</typeparam>
		/// <param name="obj">The IActor instance to persist to the database.</param>
		public static void SaveActor<T>(T obj) where T : IActor
		{
			Initialize();
			//@WorldName			nvarchar(64),
			//@ObjectName			nvarchar(64),
			//@OwnerObjectId		int = null,
			//@Properties			xml,
			//@ObjectId				int output
			using (SqlCommand cmd = SqlHelper.ExecuteNonQuery(_connectionString, "dbo.rad_Objects_Save",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, _worldName),
				SqlHelper.CreateInputParam("@ObjectName", SqlDbType.NVarChar, obj.Name),
				SqlHelper.CreateInputParam("@OwnerObjectId", SqlDbType.Int, (obj.Owner != null ? obj.Owner.ID : 0)),
				SqlHelper.CreateInputParam("@Properties", SqlDbType.Xml, obj.Properties.ToXml()),
				SqlHelper.CreateInputParam("@ObjectId", SqlDbType.Int, obj.ID)))
			{
				obj.ID = Convert.ToInt32(cmd.Parameters["@ObjectId"].Value);
			}
		}
	}
}
