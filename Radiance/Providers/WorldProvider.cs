using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Linq;
using System.Text;

using Radiance.Markup;
using Radiance.Security;

namespace Radiance.Providers
{
	/// <summary>
	/// Provides the abstract base class for the world data provider.
	/// </summary>
	public abstract class WorldProvider : ProviderBase
	{
		/// <summary>
		/// Gets the name of the current world.
		/// </summary>
		public string WorldName { get; set; }

		/// <summary>
		/// Gets or sets the instance of the current world to allow loaded objects to have their World property 
		/// set once loaded from the data store.
		/// </summary>
		public World World { get; set; }

		/// <summary>
		/// An event raised when the World object has been created, loaded and set to the current provider.
		/// </summary>
		public abstract void WorldLoaded();

		/// <summary>
		/// Saves the specified World instance to the data store.
		/// </summary>
		/// <param name="world">The World instance to save.</param>
		public abstract void SaveWorld(World world);

		/// <summary>
		/// Authenticates the specified user and returns the encrypted AuthKey string of the user.
		/// </summary>
		/// <param name="username">The username of the user to authenticate.</param>
		/// <param name="password">The password of the user to authenticate.</param>
		/// <returns>An encrypted string containing the AuthKey of the authenticated user or null if authentication failed.</returns>
		public abstract AuthKey AuthenticateUser(string username, string password);

		/// <summary>
		/// Gets a value indicating whether or not the specified actor name has already been used.
		/// </summary>
		/// <param name="name">The name to check.</param>
		/// <returns>True if name exists and has been used; otherwise false.</returns>
		public abstract bool ActorNameExists(string name);

		/// <summary>
		/// Validates the specified AuthKey.
		/// </summary>
		/// <param name="authKey">The AuthKey to validate.</param>
		/// <returns>True if the AuthKey specified is valid; otherwise false.</returns>
		public abstract bool ValidateAuthKey(AuthKey authKey);

		/// <summary>
		/// Validates that the specified user is in the specified role.
		/// </summary>
		/// <param name="username">The username of the user to validate.</param>
		/// <param name="role">The role to validate against the roles of the specified user.</param>
		/// <returns>True if the user is in the specified role, otherwise false.</returns>
		public abstract bool ValidateRole(string username, string role);

		/// <summary>
		/// Validates the specified word against a word filter and returns a value indicating whether or not the word is safe to use.
		/// </summary>
		/// <param name="word">The word to check.</param>
		/// <returns>True if the specified word is safe to use; otherwise false.</returns>
		public abstract bool IsSafeWord(string word);

		/// <summary>
		/// Gets the UserDetail instance for the specified user.
		/// </summary>
		/// <param name="username">The username of the current user.</param>
		/// <returns>A UserDetail instance for the current user or null if the user was not found.</returns>
		public abstract UserDetail GetUserDetail(string username);

		/// <summary>
		/// Saves the specified UserDetail instance to the data store.
		/// </summary>
		/// <param name="user">The current user detail instance.</param>
		public abstract void SaveUserDetail(UserDetail user);

		/// <summary>
		/// Creates a new instance of the specified type for the current world.
		/// </summary>
		/// <param name="typeName">The type of the instance to create.</param>
		/// <param name="constructorArgs">Ay arguments to supply to the type's constructor.</param>
		/// <returns>An instance of IActor.</returns>
		public abstract IActor CreateInstance(string typeName, params object[] constructorArgs);

		/// <summary>
		/// Gets an Actor instance for the specified id value.
		/// </summary>
		/// <typeparam name="T">An Actor derived instance.</typeparam>
		/// <param name="id">The id of the actor to retrieve.</param>
		/// <returns>An instance of the specified T for the specified id value.</returns>
		public abstract T GetActor<T>(int id) where T : IActor;

		/// <summary>
		/// Gets an Actor instance for the specified id value.
		/// </summary>
		/// <typeparam name="T">An Actor derived instance.</typeparam>
		/// <param name="name">The name of the actor to retrieve.</param>
		/// <returns>An instance of the specified T for the specified id value.</returns>
		public abstract T GetActor<T>(string name) where T : IActor;

		/// <summary>
		/// Gets a list of Actor instances with the specified ObjectType value.
		/// </summary>
		/// <param name="type">The ObjectType value of Actor instances to retrieve.</param>
		/// <returns>A list of Actor instances with the specified ObjectType value.</returns>
		public abstract List<IActor> GetActors(ObjectType type);

		/// <summary>
		/// Gets a list of Actor instances with the specified ObjectType value.
		/// </summary>
		/// <param name="type">The System.Type value of Actor instances to retrieve.</param>
		/// <returns>A list of Actor instances with the specified System.Type value.</returns>
		public abstract List<IActor> GetActors(Type type);

		/// <summary>
		/// Gets a list of quests where the specified actor starts or ends the quest.
		/// </summary>
		/// <param name="startsOrEndsWithActor">The actor instance starting or ending the quest.</param>
		/// <returns>A list of quests where the specified actor starts or ends the quest.</returns>
		public abstract List<IQuest> GetQuests(IActor startsOrEndsWithActor);

		/// <summary>
		/// Gets a list of Actor instances specified as templates.
		/// </summary>
		/// <param name="startRowIndex">The starting row indexed used with paging.</param>
		/// <param name="maxRows">The max number of rows to return.</param>
		/// <returns>A list of Actor instances specified as templates.</returns>
		public abstract List<IActor> GetTemplates(int startRowIndex, int maxRows);

		/// <summary>
		/// Gets the number of templates currently available in the virtual world.
		/// </summary>
		/// <returns>The number of templates currently available in the virtual world.</returns>
		public abstract int GetTemplatesCount();

		/// <summary>
		/// Gets a player instance for the specified id value.
		/// </summary>
		/// <param name="id">The id of the player to retrieve.</param>
		/// <returns>An IPlayer instance or null if a player instance could not be found.</returns>
		public abstract IPlayer GetPlayer(int id);

		public abstract IPlayer GetPlayer(string name);

		/// <summary>
		/// Gets a list of Actor instances for the specified user.
		/// </summary>
		/// <typeparam name="T">An type derived from Radiance.IAvatar.</typeparam>
		/// <param name="username">The username of the player.</param>
		/// <returns>A List of Actor derived instances for the current user.</returns>
		public abstract List<IPlayer> GetPlayers(string username);

		public abstract IEnumerable<IPlayer> GetPlayersFromQuery(string query);

		public abstract int GetPlayerCount();

		/// <summary>
		/// Gets the ID of the currently featured player within the world.
		/// </summary>
		/// <returns>The ID of the featured player.</returns>
		public abstract int GetFeaturedPlayerId();
		
		/// <summary>
		/// Gets the Place instance at the specified location.
		/// </summary>
		/// <param name="location">The world coordinates of the place to retrieve.</param>
		/// <returns>An instance of Place at the specified location or null if a place was not found.</returns>
		public abstract Place GetPlace(Point3 location);

		/// <summary>
		/// Gets a Place instance with the specified ID.
		/// </summary>
		/// <param name="id">The ID of the place to retrieve.</param>
		/// <returns>An instance of Place for the specified id; otherwise null.</returns>
		public abstract Place GetPlace(int id);

		/// <summary>
		/// Saves the specified Actor instance to the database.
		/// </summary>
		/// <typeparam name="T">An Actor derived instance.</typeparam>
		/// <param name="obj">The Actor instance to persist to the database.</param>
		public abstract void SaveActor<T>(T obj) where T : IActor;

		/// <summary>
		/// Saves the specified IPlayer instance to the database.
		/// </summary>
		/// <typeparam name="T">An IPlayer derived instance.</typeparam>
		/// <param name="obj">The IPlayer instance to persist to the database.</param>
		public abstract void SavePlayer<T>(T obj) where T : IPlayer;

		/// <summary>
		/// Saves the specified Place instance to the database.
		/// </summary>
		/// <typeparam name="T">An Place derived instance.</typeparam>
		/// <param name="obj">The Place instance to persist to the database.</param>
		public abstract void SavePlace<T>(T obj) where T : Place;

		/// <summary>
		/// Deletes the specified IPlayer instance.
		/// </summary>
		/// <typeparam name="T">The type of IPlayer instance to delete.</typeparam>
		/// <param name="obj">The IPlayer instance to delete.</param>
		public abstract void DeletePlayer<T>(T obj) where T : IPlayer;

		/// <summary>
		/// Removes the specified property from the specified object in the data store.
		/// </summary>
		/// <typeparam name="T">An IActor derived instance.</typeparam>
		/// <param name="obj">The object containing the property to remove.</param>
		/// <param name="propertyName">The name of the property to remove.</param>
		public abstract void RemoveProperty<T>(T obj, string propertyName) where T : IActor;

		/// <summary>
		/// Gets the collection of skills available to the current virtual world.
		/// </summary>
		/// <returns>The collection of skills available to the current virtual world.</returns>
		public abstract SkillDictionary GetSkills();

		/// <summary>
		/// Gets the collection of commands available to the current virtual world.
		/// </summary>
		/// <returns>The collection of commands available to the current virtual world.</returns>
		public abstract CommandDictionary GetCommands();

		/// <summary>
		/// Gets the collection of terrain available to the current virtual world.
		/// </summary>
		/// <returns>The collection of terrain available to the current virtual world.</returns>
		public abstract TerrainDictionary GetTerrain();

		/// <summary>
		/// Gets an array of RdlTag instances that make up the objects and properties that define a map with the specified center x, y and z.
		/// </summary>
		/// <param name="x">The center x coordinate.</param>
		/// <param name="y">The center y coordinate.</param>
		/// <param name="z">The center z coordinate.</param>
		/// <param name="width">The width of the map, int tiles.</param>
		/// <param name="height">The height of the map, int tiles.</param>
		/// <returns>An array of RdlTag instances.</returns>
		public abstract List<Place> GetMap(int x, int y, int z, int width, int height);

		/// <summary>
		/// Saves the specified map details.
		/// </summary>
		/// <param name="map">The map detail t save.</param>
		public abstract void SaveMap(MapManager.MapDetail map);

		/// <summary>
		/// Gets a list of System.Types available in the virtual world for the specified ObjectType value.
		/// </summary>
		/// <param name="objectType">One of the ObjectType values.</param>
		/// <returns>A list of System.Types available in the virtual world for the specified ObjectType value.</returns>
		public abstract List<Type> GetKnownTypes(ObjectType objectType);

		/// <summary>
		/// Gets a list of instances used as templates for the specified type.
		/// </summary>
		/// <param name="type">The type of the instance to return.</param>
		/// <returns>A list of Actor derived instances for the specified type.</returns>
		public abstract List<IActor> GetTemplates(Type type);

		/// <summary>
		/// Gets an instance used as a template for the specified type.
		/// </summary>
		/// <param name="type">The type of the instance to return.</param>
		/// <param name="name">The name of the template to retrieve.</param>
		/// <returns>An IActor derived instance for the specified type.</returns>
		public abstract IActor GetTemplate(Type type, string name);

		/// <summary>
		/// Gets an instance used as a template for the specified type.
		/// </summary>
		/// <param name="id">The ID of the template to retrieve.</param>
		/// <returns>An IActor derived instance for the specified type.</returns>
		public abstract IActor GetTemplate(int id);

		/// <summary>
		/// Initializes and loads the map details for the current world. Map details contain the tile top and left values along 
		/// with a width and height of the map for effecient loading and managing of places within the virtual world.
		/// </summary>
		/// <param name="maps">The MapManager instance that will contain the map details.</param>
		public abstract void LoadMaps(MapManager maps);

		/// <summary>
		/// Gets a list of types that can be imported via CSV files.
		/// </summary>
		/// <returns>A list of types that can be imported via CSV files.</returns>
		public abstract List<string> GetImportTypes();

		/// <summary>
		/// Imports the data from the specified CSV file into the virtual world.
		/// </summary>
		/// <param name="type">The type of data to import.</param>
		/// <param name="csvFileName">The full path to the CSV file containing the data to import.</param>
		public abstract void Import(string type, string csvFileName);

        public abstract IEnumerable<FileUpdate> GetFileUpdates();

        public abstract IEnumerable<Household> GetHouseholds();
		public abstract IEnumerable<Household> GetHouseholdsFromQuery(string query);
		public abstract Household GetHousehold(int householdId);
		public abstract Household GetHousehold(string name);
        public abstract IEnumerable<IPlayer> GetHouseholdMembers(int householdId, int startingRowIndex, int maxRows);
        public abstract IEnumerable<HouseholdRank> GetHouseholdRanks(int householdId);
		public abstract HouseholdRank GetHeadOfHouseholdRank(int order);
        public abstract IEnumerable<IItem> GetHouseholdArmory(int householdId);
        public abstract void SaveHousehold(Household household);
        public abstract void SaveHouseholdMember(IPlayer member, int householdId, int rankId);
        public abstract void SaveHouseholdRank(int householdId, HouseholdRank rank);

        public abstract int GetPageCount(string query);

		public abstract void CreateSession(IClient client, string ipAddress);
		public abstract void UpdateSession(IClient client);
		public abstract void RemoveSession(IClient client);
	}
}
