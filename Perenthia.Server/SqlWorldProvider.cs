using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Security;
using System.Threading;

using Lionsguard;
using Lionsguard.Data;
using Lionsguard.Providers;
using Radiance;
using Radiance.Markup;
using Radiance.Providers;
using Radiance.Security;

namespace Perenthia
{
	/// <summary>
	/// Represents a Radiance.WorldProvider that uses SQL Server as a backend data store.
	/// </summary>
	public class SqlWorldProvider : WorldProvider
	{
		#region Initialize
		private string _connectionString;
		public string ConnectionString
		{
			get { return _connectionString; }
		}

		/// <summary>
		/// Initializes the SqlWorldProvider.
		/// </summary>
		/// <param name="name">The name of the current provider.</param>
		/// <param name="config">The NameValueCollection containing the provider specific details.</param>
		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			base.Initialize(name, config);

			_connectionString = ProviderUtil.GetConnectionString(config);

			ProviderUtil.CheckUnrecognizedAttributes(config);
		}
		#endregion

		/// <summary>
		/// An event raised when the World object has been created, loaded and set to the current provider.
		/// </summary>
		public override void WorldLoaded()
		{
			bool isOnline = false;
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Worlds_GetWorld",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName)))
			{
				if (reader.Read())
				{
					this.World.ID = reader.GetInt32("WorldId");
					this.World.Description = reader.GetString("Description");
					isOnline = reader.GetBoolean("IsOnline");
				}
			}

			if (isOnline) this.World.BringOnline();
			else this.World.TakeOffline();
		}

		/// <summary>
		/// Saves the specified World instance to the data store.
		/// </summary>
		/// <param name="world">The World instance to save.</param>
		public override void SaveWorld(World world)
		{
			using (SqlCommand cmd = SqlHelper.ExecuteNonQuery(_connectionString, "dbo.rad_Worlds_Save",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, world.Name),
				SqlHelper.CreateInputParam("@IsOnline", SqlDbType.Bit, world.IsOnline),
				SqlHelper.CreateInputParam("@Description", SqlDbType.NVarChar, world.Description),
				SqlHelper.CreateInputOutputParam("@WorldId", SqlDbType.Int, world.ID)))
			{
				world.ID = Convert.ToInt32(cmd.Parameters["@WorldId"].Value);
			}
		}

		/// <summary>
		/// Authenticates the specified user and returns the encrypted AuthKey string of the user.
		/// </summary>
		/// <param name="username">The username of the user to authenticate.</param>
		/// <param name="password">The password of the user to authenticate.</param>
		/// <returns>An encrypted string containing the AuthKey of the authenticated user or null if authentication failed.</returns>
		public override AuthKey AuthenticateUser(string username, string password)
		{
			if (Membership.ValidateUser(username, password))
			{
				MembershipUser user = Membership.GetUser(username);
				if (user != null)
				{
					return new AuthKey(Guid.Empty, username, (int)user.ProviderUserKey);
				}
			}
			return AuthKey.Empty;
		}

		/// <summary>
		/// Gets a value indicating whether or not the specified name exists.
		/// </summary>
		/// <param name="name">The name to verify.</param>
		/// <returns>True if the name exists; otherwise false.</returns>
		public override bool ActorNameExists(string name)
		{
			object obj = SqlHelper.ExecuteScalar(_connectionString, "dbo.rad_Objects_NameExists",
				SqlHelper.CreateInputParam("@Name", SqlDbType.NVarChar, name),
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName));
			if (obj != null && obj != DBNull.Value)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Validates the specified AuthKey.
		/// </summary>
		/// <param name="authKey">The AuthKey to validate.</param>
		/// <returns>True if the AuthKey specified is valid; otherwise false.</returns>
		public override bool ValidateAuthKey(AuthKey authKey)
		{
			TimeSpan remainder = DateTime.Now.Subtract(authKey.Date);
			if (remainder.TotalHours < 24)
			{
				if (authKey.ID > 0)
				{
					object obj = SqlHelper.ExecuteScalar(_connectionString, "dbo.rad_Objects_ValidateUserObject",
						SqlHelper.CreateInputParam("@ObjectId", SqlDbType.Int, authKey.ID),
						SqlHelper.CreateInputParam("@UserName", SqlDbType.NVarChar, authKey.UserName));
					return (obj != null && obj != DBNull.Value);
				}
				else if (!String.IsNullOrEmpty(authKey.UserName))
				{
					MembershipUser user = Membership.GetUser(authKey.UserName);
					return (user != null);
				}
				else
				{
					return !authKey.SessionId.Equals(Guid.Empty);
				}
			}
			return false;
		}

		/// <summary>
		/// Validates that the specified user is in the specified role.
		/// </summary>
		/// <param name="username">The username of the user to validate.</param>
		/// <param name="role">The role to validate against the roles of the specified user.</param>
		/// <returns>True if the user is in the specified role, otherwise false.</returns>
		public override bool ValidateRole(string username, string role)
		{
			return Roles.IsUserInRole(username, role);
		}

		/// <summary>
		/// Validates the specified word against a word filter and returns a value indicating whether or not the word is safe to use.
		/// </summary>
		/// <param name="word">The word to check.</param>
		/// <returns>True if the specified word is safe to use; otherwise false.</returns>
		public override bool IsSafeWord(string word)
		{
			return Lionsguard.Content.ContentManager.IsWordSafe(word);
		}

		/// <summary>
		/// Gets the UserDetail instance for the specified user.
		/// </summary>
		/// <param name="username">The username of the current user.</param>
		/// <returns>A UserDetail instance for the current user or null if the user was not found.</returns>
		public override UserDetail GetUserDetail(string username)
		{
			// Ensure the user exists.
			MembershipUser user = Membership.GetUser(username);
			if (user != null)
			{
				using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_UserDetails_GetUserDetail",
					SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
					SqlHelper.CreateInputParam("@UserName", SqlDbType.NVarChar, username)))
				{
					if (reader.Read())
					{
						return new UserDetail
						{
							ID = reader.GetInt32("UserDetailId"),
							UserName = username,
							Properties = XmlHelper.FromXml<Radiance.PropertyCollection>(reader.GetSqlXml("Properties").CreateReader())
						};
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Saves the specified UserDetail instance to the data store.
		/// </summary>
		/// <param name="user">The current user detail instance.</param>
		public override void SaveUserDetail(UserDetail user)
		{
			SqlHelper.ExecuteNonQuery(_connectionString, "dbo.rad_UserDetails_Save",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
				SqlHelper.CreateInputParam("@UserName", SqlDbType.NVarChar, user.UserName),
				SqlHelper.CreateInputParam("@Properties", SqlDbType.Xml, user.Properties.ToXml()));
		}

		/// <summary>
		/// Creates a new instance of the specified type for the current world.
		/// </summary>
		/// <param name="typeName">The type of the instance to create.</param>
		/// <param name="constructorArgs">Ay arguments to supply to the type's constructor.</param>
		/// <returns>An instance of IActor.</returns>
		public override IActor CreateInstance(string typeName, params object[] constructorArgs)
		{
			if (constructorArgs != null && constructorArgs.Length > 0)
			{
				return Activator.CreateInstance(Type.GetType(typeName, true, true), constructorArgs) as IActor;
			}
			else
			{
				return Activator.CreateInstance(Type.GetType(typeName, true, true)) as IActor;
			}
		}

		/// <summary>
		/// Gets an IActor instance for the specified id value.
		/// </summary>
		/// <typeparam name="T">An IActor derived instance.</typeparam>
		/// <param name="id">The id of the actor to retrieve.</param>
		/// <returns>An instance of the specified T for the specified id value.</returns>
		public override T GetActor<T>(int id)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Objects_GetObject",
				SqlHelper.CreateInputParam("@ObjectId", SqlDbType.Int, id)))
			{
				return this.LoadActorFromReader<T>(reader);
			}
		}

		/// <summary>
		/// Gets an Actor instance for the specified id value.
		/// </summary>
		/// <typeparam name="T">An Actor derived instance.</typeparam>
		/// <param name="name">The name of the actor to retrieve.</param>
		/// <returns>An instance of the specified T for the specified id value.</returns>
		public override T GetActor<T>(string name)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Objects_GetObjectByName",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
				SqlHelper.CreateInputParam("@ObjectName", SqlDbType.NVarChar, name)))
			{
				return this.LoadActorFromReader<T>(reader);
			}
		}

		private T LoadActorFromReader<T>(SqlNullDataReader reader) where T : IActor
		{
			T obj = default(T);
			// Parent
			if (reader.Read())
			{
				obj = GetActorFromReader<T>(reader);
			}

			// Children
			AddActorsFromReader<T>(obj, reader);

			// Grand Children
			if (obj != null && obj.Children.Count > 0 && reader.NextResult())
			{
				while (reader.Read())
				{
					IActor child = obj.Children.Find(new Predicate<IActor>(
						delegate(IActor a) { return a.ID == reader.GetInt32("OwnerObjectId"); }));
					if (child != null)
					{
						IActor grandChild = this.GetActorFromReader<IActor>(reader);
						grandChild.Owner = child;
						child.Children.Add(grandChild);
					}
				}
			}
			return obj;
		}

		/// <summary>
		/// Gets a list of Actor instances with the specified ObjectType value.
		/// </summary>
		/// <param name="type">The ObjectType value of Actor instances to retrieve.</param>
		/// <returns>A list of Actor instances with the specified ObjectType value.</returns>
		public override List<IActor> GetActors(ObjectType type)
		{
			List<IActor> list = new List<IActor>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Objects_GetAllObjectsByType",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
				SqlHelper.CreateInputParam("@ObjectType", SqlDbType.NVarChar, type.ToString())))
			{
				list = this.GetActorsFromReader<IActor>(reader);
			}
			return list;
		}

		/// <summary>
		/// Gets a list of Actor instances with the specified ObjectType value.
		/// </summary>
		/// <param name="type">The System.Type value of Actor instances to retrieve.</param>
		/// <returns>A list of Actor instances with the specified System.Type value.</returns>
		public override List<IActor> GetActors(Type type)
		{
			List<IActor> list = new List<IActor>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Objects_GetAllObjectsByRuntimeType",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
				SqlHelper.CreateInputParam("@TypeName", SqlDbType.NVarChar, type.FullName)))
			{
				list = this.GetActorsFromReader<IActor>(reader);
			}
			return list;
		}

		/// <summary>
		/// Gets a list of quests where the specified actor starts or ends the quest.
		/// </summary>
		/// <param name="startsOrEndsWithActor">The actor instance starting or ending the quest.</param>
		/// <returns>A list of quests where the specified actor starts or ends the quest.</returns>
		public override List<IQuest> GetQuests(IActor startsOrEndsWithActor)
		{
			List<IActor> list = new List<IActor>();
			var types = this.GetTypes(typeof(Quest)).Select(t => t.FullName).ToList();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Objects_GetQuestsForActor",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
				SqlHelper.CreateInputParam("@ObjectId", SqlDbType.NVarChar, startsOrEndsWithActor.ID),
				SqlHelper.CreateInputParam("@Types", SqlDbType.Xml, types.ToXml())))
			{
				list = this.GetActorsFromReader<IActor>(reader);
			}
			return list.Where(q => q is IQuest).Select(q => q as IQuest).ToList();
		}

		private List<IActor> GetActorsFromReader<T>(SqlNullDataReader reader) where T : IActor
		{
			List<IActor> list = new List<IActor>();

			// Parent
			while (reader.Read())
			{
				list.Add(GetActorFromReader<IActor>(reader));
			}

			// Children
			if (reader.NextResult())
			{
				while (reader.Read())
				{
					IActor parent = (from a in list where a.ID == reader.GetInt32("OwnerObjectId") select a).FirstOrDefault();
					if (parent != null)
					{
						IActor c = GetActorFromReader<IActor>(reader);
						c.Owner = parent;
						parent.Children.Add(c);
					}
				}
			}

			// Grand Children
			if (reader.NextResult())
			{
				while (reader.Read())
				{
					int childId = reader.GetInt32("OwnerObjectId");
					IActor child = null;
					foreach (var parent in list)
					{
						child = parent.Children.Where(c => c.ID == childId).FirstOrDefault();
						if (child != null) break;
					}
					if (child != null)
					{
						IActor grandChild = this.GetActorFromReader<IActor>(reader);
						grandChild.Owner = child;
						child.Children.Add(grandChild);
					}
				}
			}

			return list;
		}

		/// <summary>
		/// Gets a list of Actor instances specified as templates.
		/// </summary>
		/// <param name="startRowIndex">The starting row indexed used with paging.</param>
		/// <param name="maxRows">The max number of rows to return.</param>
		/// <returns>A list of Actor instances specified as templates.</returns>
		public override List<IActor> GetTemplates(int startRowIndex, int maxRows)
		{
			List<IActor> list = new List<IActor>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Objects_GetTemplates",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
				SqlHelper.CreateInputParam("@ObjectType", SqlDbType.NVarChar, null),
				SqlHelper.CreateInputParam("@StartRowIndex", SqlDbType.Int, startRowIndex),
				SqlHelper.CreateInputParam("@MaxRows", SqlDbType.Int, maxRows)))
			{
				list = this.GetActorsFromReader<IActor>(reader);
			}
			return list;
		}

		/// <summary>
		/// Gets the number of templates currently available in the virtual world.
		/// </summary>
		/// <returns>The number of templates currently available in the virtual world.</returns>
		public override int GetTemplatesCount()
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Objects_GetTemplatesCount",
				   SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
				   SqlHelper.CreateInputParam("@ObjectType", SqlDbType.NVarChar, null)))
			{
				if (reader.Read())
				{
					return reader.GetInt32(0);
				}
			}
			return 0;
		}

		/// <summary>
		/// Gets a player instance for the specified id value.
		/// </summary>
		/// <param name="id">The id of the player to retrieve.</param>
		/// <returns>An IPlayer instance or null if a player instance could not be found.</returns>
		public override IPlayer GetPlayer(int id)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Players_GetPlayer",
				SqlHelper.CreateInputParam("@PlayerId", SqlDbType.Int, id)))
			{
				return this.LoadActorFromReader<Character>(reader);
			}
		}

		public override IPlayer GetPlayer(string name)
		{
			string[] parts = name.Split(' ');
			if (parts != null && parts.Length > 0)
			{
				name = parts[0];
			}

			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Players_GetPlayerByName",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
				SqlHelper.CreateInputParam("@FirstName", SqlDbType.NVarChar, name)))
			{
				return this.LoadActorFromReader<Character>(reader);
			}
		}

		/// <summary>
		/// Gets a list of IAvatar instances for the specified user.
		/// </summary>
		/// <typeparam name="T">An type derived from Radiance.IAvatar.</typeparam>
		/// <param name="username">The username of the player.</param>
		/// <returns>A List of IAvatar derived instances for the current user.</returns>
		public override List<IPlayer> GetPlayers(string username)
		{
			List<IPlayer> list = new List<IPlayer>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Players_GetPlayers",
				SqlHelper.CreateInputParam("@UserName", SqlDbType.NVarChar, username),
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName)))
			{
				while (reader.Read())
				{
					list.Add(GetActorFromReader<Character>(reader));
				}
			}
			return list;
		}

		public override IEnumerable<IPlayer> GetPlayersFromQuery(string query)
		{
			List<IPlayer> list = new List<IPlayer>();
			if (!String.IsNullOrEmpty(query))
			{
				using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, query, CommandType.Text))
				{
					while (reader.Read())
					{
						list.Add(GetActorFromReader<Character>(reader));
					}
				}
			}
			return list;
		}

		public override int GetPlayerCount()
		{
			object obj = SqlHelper.ExecuteScalar(_connectionString, "dbo.rad_Players_GetCount",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName));
			if (obj != null && obj != DBNull.Value)
			{
				int result;
				if (Int32.TryParse(obj.ToString(), out result))
				{
					return result;
				}
			}
			return 0;
		}

		/// <summary>
		/// Deletes the specified IPlayer instance.
		/// </summary>
		/// <typeparam name="T">The type of IPlayer instance to delete.</typeparam>
		/// <param name="obj">The IPlayer instance to delete.</param>
		public override void DeletePlayer<T>(T obj)
		{
			SqlHelper.ExecuteNonQuery(_connectionString, "dbo.rad_Players_Delete",
				SqlHelper.CreateInputParam("@PlayerId", SqlDbType.Int, obj.ID));
		}

		/// <summary>
		/// Removes the specified property from the specified object in the data store.
		/// </summary>
		/// <typeparam name="T">An IActor derived instance.</typeparam>
		/// <param name="obj">The object containing the property to remove.</param>
		/// <param name="propertyName">The name of the property to remove.</param>
		public override void RemoveProperty<T>(T obj, string propertyName)
		{
			obj.Properties.Remove(propertyName);
			if (obj is IPlayer)
			{
				SqlHelper.ExecuteNonQuery(_connectionString, "dbo.rad_Players_RemoveProperty",
					SqlHelper.CreateInputParam("@PlayerId", SqlDbType.Int, obj.ID),
					SqlHelper.CreateInputParam("@PropertyName", SqlDbType.NVarChar, propertyName));
			}
			else
			{
				SqlHelper.ExecuteNonQuery(_connectionString, "dbo.rad_Objects_RemoveProperty",
					SqlHelper.CreateInputParam("@ObjectId", SqlDbType.Int, obj.ID),
					SqlHelper.CreateInputParam("@PropertyName", SqlDbType.NVarChar, propertyName));
			}
		}

		private T GetActorFromReader<T>(SqlNullDataReader reader) where T : IActor
		{
			Type t = Type.GetType(reader.GetString("TypeName"), false, true);
			if (t != null)
			{
				T obj = (T)Activator.CreateInstance(t);
				if (obj != null)
				{
					// Load the template properties and then allow the instance properties to overwrite
					// any of the template properties.
					if (!reader.IsDBNull("TemplateProperties"))
					{
						Radiance.PropertyCollection tempProps = XmlHelper.FromXml<Radiance.PropertyCollection>(reader.GetSqlXml("TemplateProperties").CreateReader());
						if (tempProps != null)
						{
							// Allow templates to overwrite class defaults.
							obj.Properties.AddRange(tempProps.Select(p => p.Value), true);
						}
					}
					Radiance.PropertyCollection props = XmlHelper.FromXml<Radiance.PropertyCollection>(reader.GetSqlXml("Properties").CreateReader());
					if (props != null)
					{
						// Instance properties will overwrite template properties.
						obj.Properties.AddRange(props.Select(p => p.Value), true);
					}
					obj.ID = reader.GetInt32("ObjectId");
					obj.TemplateID = reader.GetInt32("TemplateId");
					obj.Name = reader.GetString("ObjectName");

                    // Player Specific Properties
                    if (obj is IPlayer)
                    {
                        IPlayer player = obj as IPlayer;
                        player.Household = this.GetHouseholdInfoFromReader(reader);
                    }

					if (this.World != null)
					{
						obj.World = this.World;
					}

					obj.RaiseLoadComplete();
				}
				return obj;
			}
			return default(T);
		}

		private void AddActorsFromReader<T>(T owner, SqlNullDataReader reader) where T : IActor
		{
			if (owner != null && reader.NextResult())
			{
				while (reader.Read())
				{
					IActor obj = GetActorFromReader<IActor>(reader);
					obj.Owner = owner;
					owner.Children.Add(obj);
				}
			}
		}

		/// <summary>
		/// Saves the specified Actor instance to the database.
		/// </summary>
		/// <typeparam name="T">An Actor derived instance.</typeparam>
		/// <param name="obj">The Actor instance to persist to the database.</param>
		public override void SaveActor<T>(T obj)
		{
			// Perform the save on a background thread.
			if (obj.ID > 0)
			{
				this.PerformSaveOnBackgroundThread<T>(obj);
			}
			else
			{
				this.PerformSaveActor<T>(obj);
			}
		}
		private void PerformSaveActor<T>(T obj) where T : IActor
		{
			if (obj == null) return;
			if (obj is IPlayer)
			{
				this.PerformSavePlayer<IPlayer>((IPlayer)obj);
			}
			else
			{
				int ownerPlayerId = 0;
				if (obj.Owner != null && obj.Owner is IPlayer)
				{
					ownerPlayerId = obj.Owner.ID;
				}
				using (SqlCommand cmd = SqlHelper.ExecuteNonQuery(_connectionString, "dbo.rad_Objects_Save",
					SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
					SqlHelper.CreateInputParam("@ObjectName", SqlDbType.NVarChar, obj.Name),
					SqlHelper.CreateInputParam("@TypeName", SqlDbType.NVarChar, obj.GetType().FullName),
					SqlHelper.CreateInputParam("@OwnerObjectId", SqlDbType.Int, (obj.Owner != null ? obj.Owner.ID : 0)),
					SqlHelper.CreateInputParam("@OwnerPlayerId", SqlDbType.Int, ownerPlayerId),
					SqlHelper.CreateInputParam("@TemplateId", SqlDbType.Int, obj.TemplateID),
					SqlHelper.CreateInputParam("@Properties", SqlDbType.Xml, obj.Properties.ToXml()),
					SqlHelper.CreateInputOutputParam("@ObjectId", SqlDbType.Int, obj.ID)))
				{
					obj.ID = Convert.ToInt32(cmd.Parameters["@ObjectId"].Value);
				}
			}
		}

		/// <summary>
		/// Saves the specified IPlayer instance to the database.
		/// </summary>
		/// <typeparam name="T">An IPlayer derived instance.</typeparam>
		/// <param name="obj">The IPlayer instance to persist to the database.</param>
		public override void SavePlayer<T>(T obj)
		{
			// Perform the save on a background thread.
			if (obj.ID > 0)
			{
				this.PerformSaveOnBackgroundThread<T>(obj);
			}
			else
			{
				this.PerformSavePlayer<T>(obj);
			}
		}
		private void PerformSavePlayer<T>(T obj) where T : IPlayer
		{
			using (SqlCommand cmd = SqlHelper.ExecuteNonQuery(_connectionString, "dbo.rad_Players_Save",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
				SqlHelper.CreateInputParam("@UserName", SqlDbType.NVarChar, obj.UserName),
				SqlHelper.CreateInputParam("@FirstName", SqlDbType.NVarChar, obj.FirstName),
				SqlHelper.CreateInputParam("@LastName", SqlDbType.NVarChar, obj.LastName),
                SqlHelper.CreateInputParam("@TypeName", SqlDbType.NVarChar, obj.GetType().FullName),
                SqlHelper.CreateInputParam("@HouseholdId", SqlDbType.Int, obj.Household.HouseholdID),
                SqlHelper.CreateInputParam("@RankId", SqlDbType.Int, obj.Household.RankID),
				SqlHelper.CreateInputParam("@Properties", SqlDbType.Xml, obj.Properties.ToXml()),
				SqlHelper.CreateInputOutputParam("@PlayerId", SqlDbType.Int, obj.ID)))
			{
				obj.ID = Convert.ToInt32(cmd.Parameters["@PlayerId"].Value);
			}
		}

		private void PerformSaveOnBackgroundThread<T>(T obj) where T : IActor
		{
			try
			{
				Thread t = new Thread(new ParameterizedThreadStart((object o) =>
				{
					if (o is Place)
					{
						this.PerformSavePlace<Place>((Place)o);
					}
					else if (o is IPlayer)
					{
						this.PerformSavePlayer<IPlayer>((IPlayer)o);
					}
					else
					{
						this.PerformSaveActor<IActor>((IActor)o);
					}
				}));
				t.Start(obj);
			}
			catch (Exception ex)
			{
#if DEBUG
				throw (ex);
#else
				Logger.LogError(ex.ToString());
#endif
			}
		}

		/// <summary>
		/// Saves the specified Place instance to the database.
		/// </summary>
		/// <typeparam name="T">An Place derived instance.</typeparam>
		/// <param name="obj">The Place instance to persist to the database.</param>
		public override void SavePlace<T>(T obj)
		{
			// Perform the save on a background thread.
			if (obj.ID > 0)
			{
				this.PerformSaveOnBackgroundThread<T>(obj);
			}
			else
			{
				this.PerformSavePlace<T>(obj);
			}
		}
		private void PerformSavePlace<T>(T obj) where T : Place
		{
			using (SqlCommand cmd = SqlHelper.ExecuteNonQuery(_connectionString, "dbo.rad_Objects_SavePlace",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
				SqlHelper.CreateInputParam("@ObjectName", SqlDbType.NVarChar, obj.Name),
				SqlHelper.CreateInputParam("@TypeName", SqlDbType.NVarChar, obj.GetType().FullName),
				SqlHelper.CreateInputParam("@OwnerObjectId", SqlDbType.Int, (obj.Owner != null ? obj.Owner.ID : 0)),
				SqlHelper.CreateInputParam("@Properties", SqlDbType.Xml, obj.Properties.ToXml()),
				SqlHelper.CreateInputParam("@X", SqlDbType.Int, obj.X),
				SqlHelper.CreateInputParam("@Y", SqlDbType.Int, obj.Y),
				SqlHelper.CreateInputParam("@Z", SqlDbType.Int, obj.Z),
				SqlHelper.CreateInputOutputParam("@ObjectId", SqlDbType.Int, obj.ID)))
			{
				obj.ID = Convert.ToInt32(cmd.Parameters["@ObjectId"].Value);
			}
		}

		/// <summary>
		/// Gets the Place instance at the specified location.
		/// </summary>
		/// <param name="location">The world coordinates of the place to retrieve.</param>
		/// <returns>An instance of Place at the specified location or null if a place was not found.</returns>
		public override Place GetPlace(Point3 location)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Objects_GetPlace",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
				SqlHelper.CreateInputParam("@X", SqlDbType.Int, location.X),
				SqlHelper.CreateInputParam("@Y", SqlDbType.Int, location.Y),
				SqlHelper.CreateInputParam("@Z", SqlDbType.Int, location.Z)))
			{
				return LoadPlaceFromReader(reader);
			}
		}

		/// <summary>
		/// Gets a Place instance with the specified ID.
		/// </summary>
		/// <param name="id">The ID of the place to retrieve.</param>
		/// <returns>An instance of Place for the specified id; otherwise null.</returns>
		public override Place GetPlace(int id)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Objects_GetPlaceById",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
				SqlHelper.CreateInputParam("@ObjectId", SqlDbType.Int, id)))
			{
				return LoadPlaceFromReader(reader);
			}
		}

		private Place LoadPlaceFromReader(SqlNullDataReader reader)
		{
			Room place = null;
			if (reader.Read())
			{
				place = GetActorFromReader<Room>(reader);
				place.X = reader.GetInt32("X");
				place.Y = reader.GetInt32("Y");
				place.Z = reader.GetInt32("Z");
			}
			// Actors owned by the place and non-player avatars in the area.
			if (reader.NextResult())
			{
				while (reader.Read())
				{
					// Get the child actor instance and add it to the current place.
					IActor child = GetActorFromReader<IActor>(reader);
					if (child != null)
					{
						place.Children.Add(child);
					}
				}
			}

			// Child Actors of the actors in the current place.
			if (reader.NextResult()) // Place.Actor.Children
			{
				while (reader.Read())
				{
					IActor parent = (from a in place.Children where a.ID == reader.GetInt32("OwnerObjectId") select a).FirstOrDefault();
					if (parent != null)
					{
						IActor c = GetActorFromReader<IActor>(reader);
						parent.Children.Add(c);
					}
				}
			}

			// Grandchildren of the actors in the current place.
			if (reader.NextResult()) // Place.Actor.Children.Children
			{
				while (reader.Read())
				{
					int childId = reader.GetInt32("OwnerObjectId");
					IActor child = null;
					foreach (var parent in place.Children)
					{
						child = parent.Children.Where(c => c.ID == childId).FirstOrDefault();
						if (child != null) break;
					}
					if (child != null)
					{
						IActor grandChild = this.GetActorFromReader<IActor>(reader);
						child.Children.Add(grandChild);
					}
				}
			}
			return place;
		}

		/// <summary>
		/// Gets the collection of skills available to the current virtual world.
		/// </summary>
		/// <returns>The collection of skills available to the current virtual world.</returns>
		public override SkillDictionary GetSkills()
		{
			SkillDictionary skills = new SkillDictionary();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Skills_GetSkills",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName)))
			{
				while (reader.Read())
				{
					Skill skill = new Skill
					{
						ID = reader.GetInt32("SkillId"),
						Name = reader.GetString("SkillName"),
						Description = reader.GetString("Description"),
						GroupName = reader.GetString("SkillTypeName")
					};
					skills.Add(skill.Name, skill);
				}
			}
			return skills;
		}

		/// <summary>
		/// Gets the collection of commands available to the current virtual world.
		/// </summary>
		/// <returns>The collection of commands available to the current virtual world.</returns>
		public override CommandDictionary GetCommands()
		{
			CommandDictionary commands = new CommandDictionary();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Commands_GetCommands",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName)))
			{
				while (reader.Read())
				{
					commands.Add(reader.GetString("CommandName"),
						new CommandInfo
						{
							ID = reader.GetInt32("CommandId"),
							CommandName = reader.GetString("CommandName"),
							RequiredRole = reader.GetString("RequiredRole"),
							Syntax = reader.GetString("Syntax"),
							Arguments = reader.GetString("Arguments"),
							Help = reader.GetString("Help"),
							IsVisible = reader.GetBoolean("IsVisible")
						});
				}
			}
			return commands;
		}

		/// <summary>
		/// Gets the collection of terrain available to the current virtual world.
		/// </summary>
		/// <returns>The collection of terrain available to the current virtual world.</returns>
		public override TerrainDictionary GetTerrain()
		{
			TerrainDictionary list = new TerrainDictionary();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Terrain_GetTerrain",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName)))
			{
				while (reader.Read())
				{
					Terrain t = new Terrain
					{
						ID = reader.GetInt32("TerrainId"),
						Name = reader.GetString("TerrainName"),
						Color = reader.GetInt32("Color"),
						WalkType = (WalkTypes)reader.GetInt32("WalkType"),
						ImageUrl = reader.GetString("ImageUrl")
					};
					list.Add(t.ID, t);
				}
			}
			return list;
		}

		/// <summary>
		/// Gets an array of RdlTag instances that make up the objects and properties that define a map with the specified center x, y and z.
		/// </summary>
		/// <param name="x">The center x coordinate.</param>
		/// <param name="y">The center y coordinate.</param>
		/// <param name="z">The center z coordinate.</param>
		/// <param name="width">The width of the map, int tiles.</param>
		/// <param name="height">The height of the map, int tiles.</param>
		/// <returns>An array of RdlTag instances.</returns>
		public override List<Place> GetMap(int x, int y, int z, int width, int height)
		{
			List<Place> list = new List<Place>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Objects_GetMap",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
				SqlHelper.CreateInputParam("@X", SqlDbType.Int, x),
				SqlHelper.CreateInputParam("@Y", SqlDbType.Int, y),
				SqlHelper.CreateInputParam("@Z", SqlDbType.Int, z),
				SqlHelper.CreateInputParam("@Width", SqlDbType.Int, width),
				SqlHelper.CreateInputParam("@Height", SqlDbType.Int, height)))
			{
				// Parent
				while (reader.Read())
				{
					Room room = GetActorFromReader<Room>(reader);
					room.X = reader.GetInt32("X");
					room.Y = reader.GetInt32("Y");
					room.Z = reader.GetInt32("Z");
					list.Add(room);
				}

				//if (reader.NextResult())
				//{
				//    // Actors owned by the place and non-player avatars in the area.
				//    List<IActor> childActors = this.GetActorsFromReader<IActor>(reader);

				//    // Add actors to the place they are owned by.
				//    foreach (var child in childActors)
				//    {
				//        Place place = list.Where(p => p.X == child.Properties.GetValue<int>(Avatar.XProperty)
				//            && p.Y == child.Properties.GetValue<int>(Avatar.YProperty)
				//            && p.Z == child.Properties.GetValue<int>(Avatar.ZProperty)).FirstOrDefault();
				//        if (place != null)
				//        {
				//            place.Children.Add(child);
				//        }
				//    }
				//}
			}
			return list;
		}

		/// <summary>
		/// Saves the specified map details.
		/// </summary>
		/// <param name="map">The map detail t save.</param>
		public override void SaveMap(MapManager.MapDetail map)
		{
			SqlHelper.ExecuteNonQuery(_connectionString, "dbo.rad_Maps_Save",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
				SqlHelper.CreateInputParam("@MapName", SqlDbType.NVarChar, map.Name),
				SqlHelper.CreateInputParam("@X", SqlDbType.Int, map.Key.StartX),
				SqlHelper.CreateInputParam("@Y", SqlDbType.Int, map.Key.StartY),
				SqlHelper.CreateInputParam("@Width", SqlDbType.Int, map.Width),
				SqlHelper.CreateInputParam("@Height", SqlDbType.Int, map.Height));
		}

		/// <summary>
		/// Gets a list of System.Types available in the virtual world for the specified ObjectType value.
		/// </summary>
		/// <param name="objectType">One of the ObjectType values.</param>
		/// <returns>A list of System.Types available in the virtual world for the specified ObjectType value.</returns>
		public override List<Type> GetKnownTypes(ObjectType objectType)
		{
			switch (objectType)
			{	
				case ObjectType.Mobile:
					if (_mobileTypes.Count == 0)
					{
						_mobileTypes = this.GetTypes(typeof(IMobile));
					}
					return _mobileTypes;
				case ObjectType.Player:
					if (_playerTypes.Count == 0)
					{
						_playerTypes = this.GetTypes(typeof(IPlayer));
					}
					return _playerTypes;
				case ObjectType.Place:
					if (_placeTypes.Count == 0)
					{
						_placeTypes = this.GetTypes(typeof(Place));
					}
					return _placeTypes;
                case ObjectType.Quest:
                    if (_questTypes.Count == 0)
                    {
                        _questTypes = this.GetTypes(typeof(Quest));
                    }
                    return _questTypes;
                case ObjectType.Award:
                    if (_awardTypes.Count == 0)
                    {
                        _awardTypes = this.GetTypes(typeof(Award));
                    }
                    return _awardTypes;
				default:
					// Default Actors (Items)
					if (_actorTypes.Count == 0)
					{
						_actorTypes = this.GetTypes(typeof(Item));
					}
					return _actorTypes;
			}
		}
		private List<Type> GetTypes(Type baseType)
		{
			return (from t in this.GetType().Assembly.GetTypes()
					where (t.IsSubclassOf(baseType) || t == baseType || t.GetInterface(baseType.FullName, true) != null)
					&& !t.IsAbstract
					select t).ToList();
		}
		private List<Type> _actorTypes = new List<Type>();
		private List<Type> _mobileTypes = new List<Type>();
		private List<Type> _placeTypes = new List<Type>();
		private List<Type> _playerTypes = new List<Type>();
        private List<Type> _questTypes = new List<Type>();
        private List<Type> _awardTypes = new List<Type>();

		/// <summary>
		/// Gets a list of instances used as templates for the virtual world.
		/// </summary>
		/// <param name="type">The type of actor instances to return.</param>
		/// <returns>A list of Actor derived instances for the specified type.</returns>
		public override List<IActor> GetTemplates(Type type)
		{
            if (!_templates.ContainsKey(type))
            {
                List<IActor> list = new List<IActor>();
                var types = this.GetTypes(type).Select(t => t.FullName).ToList();
                using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Objects_GetTemplates",
                    SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
                    SqlHelper.CreateInputParam("@Types", SqlDbType.Xml, types.ToXml())))
                {
                    while (reader.Read())
                    {
                        IActor obj = this.GetActorFromReader<IActor>(reader);
                        obj.Properties.IsTemplateCollection = true;
                        list.Add(obj);
                    }
                }
                _templates.Add(type, list);
            }
            return _templates[type];
		}
        private Dictionary<Type, List<IActor>> _templates = new Dictionary<Type, List<IActor>>();

		/// <summary>
		/// Gets an instance used as a template for the specified type.
		/// </summary>
		/// <param name="type">The type of the instance to return.</param>
		/// <returns>An IActor derived instance for the specified type.</returns>
		public override IActor GetTemplate(Type type, string name)
		{
			//return this.GetTemplates(type).Where(t => t.Name.Equals(name)).FirstOrDefault();
			var types = this.GetTypes(type).Select(t => t.FullName).ToList();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Objects_GetTemplate",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
				SqlHelper.CreateInputParam("@Name", SqlDbType.NVarChar, name),
				SqlHelper.CreateInputParam("@Types", SqlDbType.Xml, types.ToXml())))
			{
				if (reader.Read())
				{
					IActor obj = this.GetActorFromReader<IActor>(reader);
					obj.Properties.IsTemplateCollection = true;
					return obj;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets an instance used as a template for the specified type.
		/// </summary>
		/// <param name="id">The ID of the template to retrieve.</param>
		/// <returns>An IActor derived instance for the specified type.</returns>
		public override IActor GetTemplate(int id)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Objects_GetTemplateById",
				SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
				SqlHelper.CreateInputParam("@ObjectId", SqlDbType.Int, id)))
			{
				if (reader.Read())
				{
					IActor obj = this.GetActorFromReader<IActor>(reader);
					obj.Properties.IsTemplateCollection = true;
					return obj;
				}
			}
			return null;
		}

		/// <summary>
		/// Initializes and loads the map details for the current world. Map details contain the tile top and left values along 
		/// with a width and height of the map for effecient loading and managing of places within the virtual world.
		/// </summary>
		/// <param name="maps">The MapManager instance that will contain the map details.</param>
		public override void LoadMaps(MapManager maps)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Maps_GetAllMaps",
				   SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName)))
			{
				while (reader.Read())
				{
					MapManager.MapDetail detail = new MapManager.MapDetail(reader.GetString("MapName"),
						reader.GetInt32("X"), reader.GetInt32("Y"), reader.GetInt32("Width"), reader.GetInt32("Height"));
					maps.MapDetails.Add(detail.Key, detail);
				}
			}
		}

		/// <summary>
		/// Gets a list of types that can be imported via CSV files.
		/// </summary>
		/// <returns>A list of types that can be imported via CSV files.</returns>
		public override List<string> GetImportTypes()
		{
			return ImportManager.GetImportTypes();
		}

		/// <summary>
		/// Imports the data from the specified CSV file into the virtual world.
		/// </summary>
		/// <param name="type">The type of data to import.</param>
		/// <param name="csvFileName">The full path to the CSV file containing the data to import.</param>
		public override void Import(string type, string csvFileName)
		{
			ImportManager.Import(type, csvFileName, this);
		}

		public override int GetFeaturedPlayerId()
		{
			// Get the current featured player.
			int id = 0;
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_FeaturedPlayer_Get"))
			{
				if (reader.Read())
				{
					id = reader.GetInt32("PlayerId");
				}
			}

			// If more than a day has passed then get a new featured player.
			if (id == 0)
			{
				// Get a new random player.
				using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Players_GetRandomPlayer"))
				{
					if (reader.Read())
					{
						id = reader.GetInt32("PlayerId");
					}
				}
				SqlHelper.ExecuteNonQuery(_connectionString, "dbo.rad_FeaturedPlayer_Save",
					SqlHelper.CreateInputParam("@PlayerId", SqlDbType.Int, id));
			}

			return id;
        }

        public override IEnumerable<FileUpdate> GetFileUpdates()
        {
            List<FileUpdate> list = new List<FileUpdate>();
            using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_FileUpdates_GetAll",
                SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName)))
            {
                while (reader.Read())
                {
                    list.Add(new FileUpdate { FileName = reader.GetString("FileUpdateName"), LastUpdateDate = reader.GetDateTime("LastUpdateDate") });
                }
            }
            return list;
        }



        public override IEnumerable<Household> GetHouseholds()
        {
            using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Households_GetAll",
                SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName)))
            {
				return this.GetHouseholdsFromReader(reader);
            }
		}

		public override IEnumerable<Household> GetHouseholdsFromQuery(string query)
		{
            if (!String.IsNullOrEmpty(query))
			{
				using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, query, CommandType.Text))
				{
					while (reader.Read())
					{
						return this.GetHouseholdsFromReader(reader);
					}
				}
			}
			return new List<Household>();
		}

        public override Household GetHousehold(int householdId)
        {
            using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Households_Get",
                SqlHelper.CreateInputParam("@HouseholdId", SqlDbType.Int, householdId)))
            {
				var list = this.GetHouseholdsFromReader(reader);
				if (list.Count > 0)
				{
					return list[0];
				}
            }
            return null;
		}

		public override Household GetHousehold(string name)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Households_GetByName",
				SqlHelper.CreateInputParam("@HouseholdName", SqlDbType.NVarChar, name)))
			{
				var list = this.GetHouseholdsFromReader(reader);
				if (list.Count > 0)
				{
					return list[0];
				}
			}
			return null;
		}

		public override IEnumerable<IPlayer> GetHouseholdMembers(int householdId, int startingRowIndex, int maxRows)
        {
            List<IPlayer> list = new List<IPlayer>();
            using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Households_GetMembers",
				SqlHelper.CreateInputParam("@HouseholdId", SqlDbType.Int, householdId),
				SqlHelper.CreateInputParam("@StartRowIndex", SqlDbType.Int, startingRowIndex),
				SqlHelper.CreateInputParam("@MaxRows", SqlDbType.Int, maxRows)))
            {
                while (reader.Read())
                {
                    list.Add(this.GetActorFromReader<Character>(reader));
                }
            }
            return list;
        }

        public override IEnumerable<HouseholdRank> GetHouseholdRanks(int householdId)
        {
            List<HouseholdRank> list = new List<HouseholdRank>();
            using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Households_GetRanks",
                SqlHelper.CreateInputParam("@HouseholdId", SqlDbType.Int, householdId)))
            {
                while (reader.Read())
                {
                    list.Add(this.GetHouseholdRankFromReader(reader));
                }
            }
            return list;
        }

		public override HouseholdRank GetHeadOfHouseholdRank(int order)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Households_GetHeadOfHouseholdRank",
				   SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
				   SqlHelper.CreateInputParam("@RankOrder", SqlDbType.Int, order)))
			{
				if (reader.Read())
				{
					return this.GetHouseholdRankFromReader(reader);
				}
			}
			return null;
		}

        public override IEnumerable<IItem> GetHouseholdArmory(int householdId)
        {
            List<IItem> list = new List<IItem>();
            using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Households_GetArmory",
                SqlHelper.CreateInputParam("@HouseholdId", SqlDbType.Int, householdId)))
            {
                while (reader.Read())
                {
                    list.Add(this.GetActorFromReader<IItem>(reader));
                }
            }
            return list;
        }

        public override void SaveHousehold(Household household)
        {
            using (SqlCommand cmd = SqlHelper.ExecuteNonQuery(_connectionString, "dbo.rad_Households_Save",
                       SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
                       SqlHelper.CreateInputParam("@HouseholdName", SqlDbType.NVarChar, household.Name),
                       SqlHelper.CreateInputParam("@ImageUri", SqlDbType.NVarChar, household.ImageUri),
                       SqlHelper.CreateInputParam("@Properties", SqlDbType.Xml, household.Properties.ToXml()),
                       SqlHelper.CreateInputOutputParam("@HouseholdId", SqlDbType.Int, household.ID)))
            {
                household.ID = Convert.ToInt32(cmd.Parameters["@HouseholdId"].Value);
            }
        }

        public override void SaveHouseholdMember(IPlayer member, int householdId, int rankId)
        {
            using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.rad_Households_SaveMember",
                       SqlHelper.CreateInputParam("@HouseholdId", SqlDbType.NVarChar, householdId),
                       SqlHelper.CreateInputParam("@PlayerId", SqlDbType.Xml, member.ID),
                       SqlHelper.CreateInputParam("@RankId", SqlDbType.Int, rankId)))
            {
                if (reader.Read())
                {
                    member.Household = this.GetHouseholdInfoFromReader(reader);
                }
            }
        }

        public override void SaveHouseholdRank(int householdId, HouseholdRank rank)
        {
            using (SqlCommand cmd = SqlHelper.ExecuteNonQuery(_connectionString, "dbo.rad_Households_SaveRank",
                       SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
                       SqlHelper.CreateInputParam("@HouseholdId", SqlDbType.Int, householdId),
                       SqlHelper.CreateInputParam("@RankName", SqlDbType.Xml, rank.Name),
                       SqlHelper.CreateInputParam("@RankOrder", SqlDbType.Int, rank.Order),
                       SqlHelper.CreateInputParam("@ImageUri", SqlDbType.NVarChar, rank.ImageUri),
                       SqlHelper.CreateInputParam("@Permissions", SqlDbType.Int, (int)rank.Permissions),
                       SqlHelper.CreateInputParam("@TitleMale", SqlDbType.NVarChar, rank.TitleMale),
                       SqlHelper.CreateInputParam("@TitleFemale", SqlDbType.NVarChar, rank.TitleFemale),
                       SqlHelper.CreateInputParam("@RequiredMemberCount", SqlDbType.Int, rank.RequiredMemberCount),
                       SqlHelper.CreateInputParam("@EmblemCost", SqlDbType.Int, rank.EmblemCost),
                       SqlHelper.CreateInputOutputParam("@RankId", SqlDbType.Int, rank.ID)))
            {
                rank.ID = Convert.ToInt32(cmd.Parameters["@RankId"].Value);
            }
        }

        private Household GetHouseholdFromReader(SqlNullDataReader reader)
        {
            Household house = new Household();
            house.ID = reader.GetInt32("HouseholdId");
            house.Name = reader.GetString("HouseholdName");
            house.ImageUri = reader.GetString("ImageUri");
            house.DateCreated = reader.GetDateTime("DateCreated");
            house.MemberCount = reader.GetInt32("MemberCount");
            Radiance.PropertyCollection props = XmlHelper.FromXml<Radiance.PropertyCollection>(reader.GetSqlXml("Properties").CreateReader());
            if (props != null)
            {
                // Instance properties will overwrite template properties.
                house.Properties.AddRange(props.Select(p => p.Value), true);
            }
            return house;
        }

		private List<Household> GetHouseholdsFromReader(SqlNullDataReader reader)
		{
			var list = new List<Household>();
			while (reader.Read())
			{
				list.Add(this.GetHouseholdFromReader(reader));
			}

			// Relations
			if (reader.NextResult())
			{
				while (reader.Read())
				{
					var household = list.Where(h => h.ID == reader.GetInt32("HouseholdId")).FirstOrDefault();
					if (household != null)
					{
						int h1 = reader.GetInt32("PrimaryHouseholdId");
						int h2 = reader.GetInt32("SecondaryHouseholdId");
						int houseId = (h1 == household.ID ? h2 : h1);
						household.Relations.Add(houseId, (HouseholdRelationType)reader.GetInt32("RelationTypeId"));
					}
				}
			}

			// Ranks
			if (reader.NextResult())
			{
				while (reader.Read())
				{
					var household = list.Where(h => h.ID == reader.GetInt32("HouseholdId")).FirstOrDefault();
					if (household != null)
					{
						household.Ranks.Add(this.GetHouseholdRankFromReader(reader));
					}
				}
			}

			return list;
		}

		private HouseholdRank GetHouseholdRankFromReader(SqlNullDataReader reader)
		{
			HouseholdRank rank = new HouseholdRank();
            rank.EmblemCost = reader.GetInt32("EmblemCost");
            rank.ID = reader.GetInt32("RankId");
            rank.ImageUri = reader.GetString("ImageUri");
            rank.Name = reader.GetString("RankName");
            rank.Order = reader.GetInt32("RankOrder");
            rank.Permissions = (HouseholdPermissions)reader.GetInt32("Permissions");
            rank.RequiredMemberCount = reader.GetInt32("RequiredMemberCount");
            rank.TitleFemale = reader.GetString("TitleFemale");
            rank.TitleMale = reader.GetString("TitleMale");
			return rank;
		}

        private HouseholdInfo GetHouseholdInfoFromReader(SqlNullDataReader reader)
        {
            HouseholdInfo info = new HouseholdInfo();
            info.HouseholdID = reader.GetInt32("HouseholdId");
            info.HouseholdImageUri = reader.GetString("HouseholdImageUri");
            info.HouseholdName = reader.GetString("HouseholdName");
            info.RankID = reader.GetInt32("RankId");
            info.RankImageUri = reader.GetString("RankImageUri");
            info.RankName = reader.GetString("RankName");
			info.RankOrder = reader.GetInt32("RankOrder");
            info.Title = reader.GetString("Title");
            return info;
        }

        public override int GetPageCount(string query)
        {
			if (!String.IsNullOrEmpty(query))
			{
				object obj = SqlHelper.ExecuteScalar(_connectionString, query, CommandType.Text);
				if (obj != null && obj != DBNull.Value)
				{
					int result;
					if (Int32.TryParse(obj.ToString(), out result))
					{
						return result;
					}
				}
			}
            return 0;
        }



		public override void CreateSession(IClient client, string ipAddress)
		{
			var t = new Thread(() =>
			{
				try
				{
					SqlHelper.ExecuteNonQuery(_connectionString, "dbo.rad_Sessions_Create",
						SqlHelper.CreateInputParam("@SessionId", SqlDbType.UniqueIdentifier, client.SessionId),
						SqlHelper.CreateInputParam("@IPAddress", SqlDbType.NVarChar, ipAddress));
				}
				catch (Exception ex)
				{
					Logger.LogError(ex.ToString());
				}
			});
			t.Name = "Thread:CreateSession";
			t.Start();
		}

		public override void UpdateSession(IClient client)
		{
			var t = new Thread(() =>
			{
				try
				{
					var lastCmd = String.Empty;
					if (client.LastCommandGroup != null)
						lastCmd = client.LastCommandGroup.ToString();

					object playerId = null;
					if (client.Player != null)
						playerId = client.Player.ID;

					SqlHelper.ExecuteNonQuery(_connectionString, "dbo.rad_Sessions_Update",
						SqlHelper.CreateInputParam("@SessionId", SqlDbType.UniqueIdentifier, client.SessionId),
						SqlHelper.CreateInputParam("@UserName", SqlDbType.NVarChar, client.UserName),
						SqlHelper.CreateInputParam("@PlayerId", SqlDbType.Int, playerId),
						SqlHelper.CreateInputParam("@LastHeartbeat", SqlDbType.DateTime, client.LastHeartbeatDate),
						SqlHelper.CreateInputParam("@LastCommand", SqlDbType.NVarChar, lastCmd));
				}
				catch (Exception ex)
				{
					Logger.LogError(ex.ToString());
				}
			});
			t.Name = "Thread:UpdateSession";
			t.Start();
		}

		public override void RemoveSession(IClient client)
		{
			var t = new Thread(() =>
			{
				try
				{
					SqlHelper.ExecuteNonQuery(_connectionString, "dbo.rad_Sessions_Remove",
						SqlHelper.CreateInputParam("@SessionId", SqlDbType.UniqueIdentifier, client.SessionId));
				}
				catch (Exception ex)
				{
					Logger.LogError(ex.ToString());
				}
			});
			t.Name = "Thread:RemoveSession";
			t.Start();
		}
	}
}
