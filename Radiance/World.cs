using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Radiance.Configuration;
using Radiance.Handlers;
using Radiance.Markup;
using Radiance.Providers;
using Radiance.Security;

namespace Radiance
{
	/// <summary>
	/// The main virtual world component that will contain the world's logic.
	/// </summary>
	public class World : IDisposable
	{
		#region Properties
		/// <summary>
		/// Gets or sets the ID of the current World.
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Gets or sets the description of the current world.
		/// </summary>
		public string Description { get; set; }	

		/// <summary>
		/// Gets or sets a value indicating how deadly combat is; a value of 2 is the default, 1 is real to life and 3 is larger than life.
		/// </summary>
		public int RealismMultiplier { get; set; }

		/// <summary>
		/// Gets or sets the amount of magic used in the game; a value of 2 is the default, 1 requires players to use magic 
		/// more sparingly and 3 will allow them to use powers more than they probably should.
		/// </summary>
		public int PowerMultiplier { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not magic is enabled for the virtual world.
		/// </summary>
		public bool EnableMagic { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not psionics are enabled for the virtual world.
		/// </summary>
		public bool EnablePsionics { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not command logging is enabled for the virtual world.
		/// </summary>
		public bool EnableCommandLogging { get; set; }

		/// <summary>
		/// Gets or sets the name of the current virtual world.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets a dictionary of skill name/description values for all the skills available in the virtual world.
		/// </summary>
		public SkillDictionary Skills { get; internal set; }

		/// <summary>
		/// Gets a collection of pre-defined groups of skils used in the current virtual world as templates or starter kits.
		/// </summary>
		public SkillGroupCollection SkillGroups { get; private set; }	

		/// <summary>
		/// Gets a dictionary of commands available in the virtual world.
		/// </summary>
		public CommandDictionary Commands { get; internal set; }

		/// <summary>
		/// Gets a dictionary of Terrain available in the virtual world.
		/// </summary>
		public TerrainDictionary Terrain { get; internal set; }

		/// <summary>
		/// Gets the dictionary of races available to the current world.
		/// </summary>
		public RaceDictionary Races { get; internal set; }

		/// <summary>
		/// Gets or sets the number of minutes before a connected client times out and is 
		/// disconnected and removed from the virtual world.
		/// </summary>
		public int ClientTimeoutMinutes { get; set; }

		/// <summary>
		/// Gets or sets the default number of Characters a user can create and own.
		/// </summary>
		public int DefaultMaxCharacters { get; set; }	

		/// <summary>
		/// Gets the current WorldProvider instance.
		/// </summary>
		public WorldProvider Provider { get; private set; }

		/// <summary>
		/// Gets the collection of WorldProvider instances for the current virtual world.
		/// </summary>
		public WorldProviderCollection Providers { get; private set; }

		/// <summary>
		/// Gets the Server instance where the current virtual world is running.
		/// </summary>
		public Server Server { get; internal set; }

		/// <summary>
		/// Gets the dictionary of places loaded into the current world.
		/// </summary>
		public PlaceCollection Places { get; private set; }

		/// <summary>
		/// Gets the dictionary of avatars loaded into the current world.
		/// </summary>
		public AvatarDictionary Avatars { get; private set; }

		/// <summary>
		/// Gets the MapManager instance for the current world.
		/// </summary>
		public MapManager Map { get; private set; }

        /// <summary>
        /// Gets the HouseholdManager instance for the current world.
        /// </summary>
        public HouseholdManager Households { get; private set; }

		/// <summary>
		/// Gets a value indicating whether or not the current World is active and online.
		/// </summary>
		public bool IsOnline { get; private set; }

		#endregion

        public event AvatarKilledAvatarEventHandler AvatarKilledAvatar = delegate { };

        public event PlayerAdvancedEventHandler PlayerAdvanced = delegate { };

		/// <summary>
		/// Initializes a new instance of the World class.
		/// </summary>
		public World()
		{
			this.Commands = new CommandDictionary();
			this.Skills = new SkillDictionary();
			this.SkillGroups = new SkillGroupCollection();
			this.Terrain = new TerrainDictionary();
			this.Races = new RaceDictionary(this);
			this.Places = new PlaceCollection(this);
			this.Avatars = new AvatarDictionary(this);
            this.Households = new HouseholdManager(this);
		}

		/// <summary>
		/// Configures the virtual world from the radiance.world configuration section.
		/// </summary>
		/// <param name="section">The WorldSection configuration section from the application configuration file.</param>
		internal void ConfigureWorld(WorldSection section)
		{
			this.Name = section.Name;
#if DEBUG
			this.ClientTimeoutMinutes = 5;
#else
			this.ClientTimeoutMinutes = section.ClientTimeoutMinutes;
#endif
			this.DefaultMaxCharacters = section.DefaultMaxCharacters;
			this.EnableMagic = section.EnableMagic;
			this.EnablePsionics = section.EnablePsionics;
			this.EnableCommandLogging = section.EnableCommandLogging;
			this.PowerMultiplier = section.PowerMultiplier;
			this.RealismMultiplier = section.RealismMultiplier;

			this.Providers = new WorldProviderCollection();
			this.Provider = ProviderUtil.InstantiateProviders<WorldProvider, WorldProviderCollection>(
				section.Providers, 
				section.DefaultProvider, 
				this.Providers);
			if (this.Provider == null)
			{
				throw new ConfigurationErrorsException(
					SR.ConfigDefaultWorldProviderNotFound, 
					section.ElementInformation.Properties["defaultProvider"].Source, 
					section.ElementInformation.Properties["defaultProvider"].LineNumber);
			}

			// Set the name of the world as dictated by the provider.
			this.Provider.WorldName = this.Name;
			this.Provider.World = this;

			// Load any custom properties or values into the world instance.
			this.Provider.WorldLoaded();

			// Map Manager
			this.Map = Activator.CreateInstance(Type.GetType(section.MapManagerType, true, true), new object[]{ this }) as MapManager;
			this.Provider.LoadMaps(this.Map);

			// Load Skills from the provider
			this.Skills = this.Provider.GetSkills();

			// Load Commands from the provider
			this.Commands = this.Provider.GetCommands();

			// Load the terrain from the provider.
			this.Terrain = this.Provider.GetTerrain();
		}

        public void OnPlayerAdvanced(IPlayer player, string message)
        {
            this.PlayerAdvanced(player, message);
        }

        public void OnAvatarKilledAvatar(IAvatar attacker, IAvatar defender)
        {
            this.AvatarKilledAvatar(attacker, defender);
        }

		/// <summary>
		/// Takes the World offline, preventing connections and command processing.
		/// </summary>
		public void TakeOffline()
		{
			this.IsOnline = false;
			this.Provider.SaveWorld(this);
		}

		/// <summary>
		/// Brings the World online, allowing connections and command processing.
		/// </summary>
		public void BringOnline()
		{
			this.IsOnline = true;
			this.Provider.SaveWorld(this);
		}

		/// <summary>
		/// Executes the virtual world heartbeat, in which objects in the world can react to changes, move around, etc.
		/// </summary>
		public void Heartbeat()
		{
			// Update the combat manager.
			CombatManager.Update();
		}

		#region Access and Persist Actors
		/// <summary>
		/// Gets a copy of the IActor instance that serves as a template for the specified Type and name.
		/// </summary>
		/// <param name="name">The name of the template to retrieve.</param>
		/// <returns>A copy of the IActor instance that serves as a template for the specified Type and name.</returns>
		public T CreateFromTemplate<T>(string name) where T : IActor
		{
			try
			{
				T obj = (T)this.Provider.GetTemplate(typeof(T), name).Clone();
				obj.World = this;
				return obj;
			}
			catch (NullReferenceException ex)
			{
				throw new NullReferenceException(
					String.Format("A Template of type '{0}' could not be found and loaded with the name '{1}'.",
					typeof(T).FullName, name), ex);
			}
		}

		/// <summary>
		/// Gets an Actor instance from the specified alias and belonging to owner if specified.
		/// </summary>
		/// <param name="alias">The Actor aliased named. Can be the ID of the Actor, the Name of the Actor 
		/// or the Alias value of the Name and ID.</param>
		/// <param name="owner">The Actor instance to search within. If null is specified then a global search will occur.</param>
		/// <returns>An of the specified Actor if found; otherwise null</returns>
		public IActor GetActor(string alias, IActor owner)
		{
			return this.GetActor(alias, owner, true, false);
		}

		/// <summary>
		/// Gets an Actor instance from the specified alias and belonging to owner if specified.
		/// </summary>
		/// <param name="alias">The Actor aliased named. Can be the ID of the Actor, the Name of the Actor 
		/// or the Alias value of the Name and ID.</param>
		/// <param name="owner">The Actor instance to search within. If null is specified then a global search will occur.</param>
		/// <param name="loadIfNotFound">A value indicating whether or not the object should be loaded from the data 
		/// store if not found within the current world.</param>
		/// <returns>An of the specified Actor if found; otherwise null</returns>
		public IActor GetActor(string alias, IActor owner, bool loadIfNotFound)
		{
			return this.GetActor(alias, owner, loadIfNotFound, false);
		}

		/// <summary>
		/// Gets an Actor instance from the specified alias and belonging to owner if specified.
		/// </summary>
		/// <param name="alias">The Actor aliased named. Can be the ID of the Actor, the Name of the Actor 
		/// or the Alias value of the Name and ID.</param>
		/// <param name="owner">The Actor instance to search within. If null is specified then a global search will occur.</param>
		/// <param name="loadIfNotFound">A value indicating whether or not the object should be loaded from the data 
		/// store if not found within the current world.</param>
		/// <param name="includeTemplates">A value indicating that templates should be included in the search.</param>
		/// <returns>An of the specified Actor if found; otherwise null</returns>
		public IActor GetActor(string alias, IActor owner, bool loadIfNotFound, bool includeTemplates)
		{
			// Parse the alias string to get the name and/or id of the Actor.
			int id;
			string name;
			Strings.ParseAlias(alias, out id, out name);

			IActor obj = null;

			// If owner is specified then only search within the owner's child collection for the Actor instance.
			if (owner != null)
			{
				obj = (from c in owner.GetAllChildren()
					   where c.ID == id
					   || Strings.FormatAliasName(c.Name) == name
					   select c).FirstOrDefault();
			}
			else
			{
				// Owner was not specified, perform a global search within the world avatars, places, place avatars and children. 
				obj = (from a in this.Avatars.Values
					   where a.ID == id
					   || Strings.FormatAliasName(a.Name) == name
					   select a).FirstOrDefault();

				if (obj == null)
				{
					// Places
					obj = (from p in this.Places.Values
						   where p.ID == id
							|| Strings.FormatAliasName(p.Name) == name
						   select p).FirstOrDefault();
				}
				if (obj == null)
				{
					// Place Children, Grand and Great Grand Children
					Place place = (from p in this.Places.Values
								   where p.GetAllChildren().Count(c => (c.ID == id || Strings.FormatAliasName(c.Name) == name)
								   || (c.GetAllChildren().Count(i => (i.ID == id || Strings.FormatAliasName(i.Name) == name)) > 0)) > 0
								   select p).FirstOrDefault();
					if (place != null)
					{
						obj = (from c in place.Children
							   where c.ID == id
								|| Strings.FormatAliasName(c.Name) == name
							   select c).FirstOrDefault();
						if (obj == null)
						{
							foreach (var child in place.Children)
							{
								obj = child.GetAllChildren().Where(c => c.ID == id || Strings.FormatAliasName(c.Name) == name).FirstOrDefault();
								if (obj != null)
									break;
							}

						}
					}
				}

				// If none of those searches turn up anything then query the object from the data store.
				if (obj == null && loadIfNotFound)
				{
					if (id > 0)
					{
						obj = this.Provider.GetActor<IActor>(id);
					}
					else
					{
						obj = this.Provider.GetActor<IActor>(name);
					}

					// Could be a place instance.
					if (obj == null)
					{
						if (id > 0)
						{
							obj = this.Provider.GetPlace(id);
						}
					}
				}
				if (obj == null && includeTemplates)
				{
					// Search the templates for the object.
					if (id > 0)
					{
						obj = this.Provider.GetTemplate(id);
					}
					else
					{
						obj = this.Provider.GetTemplate(typeof(IActor), name);
					}
				}
			}

			if (obj != null && obj.World == null)
			{
				obj.World = this;
			}

			return obj;
		}
		/// <summary>
		/// Gets an actor instance with the specified ID value. Will search the list of loaded actors before attempting to access the 
		/// data store to retrieve the actor information.
		/// </summary>
		/// <param name="id">The ID value of the actor to retrieve.</param>
		/// <returns>An instance of Actor if found; otherwise null.</returns>
		public IActor GetActor(int id)
		{
			return this.GetActor(id.ToString(), null);
		}

		/// <summary>
		/// Saves the specified Actor instance to the data store.
		/// </summary>
		/// <param name="actor">The actor instance to save.</param>
		public void SaveActor(IActor actor)
		{
			if (actor is Place)
			{
				this.Provider.SavePlace<Place>((Place)actor);
			}
			else
			{
				this.Provider.SaveActor<IActor>(actor);
				if (!(actor is IPlayer) && actor.Owner != null && actor.Owner is Place)
				{
					Place place = this.Places.Values.Where(p => p.ID == actor.Owner.ID).FirstOrDefault();
					if (place != null)
					{
						place.Children.Add(actor);
					}
				}
			}
		}

		/// <summary>
		/// Updates the specified properties of the actor represented by the RdlProperty.ID value.
		/// </summary>
		/// <param name="actor">The RdlActor instance defining the actor to update.</param>
		/// <param name="properties">The list of properties to update.</param>
		public void SaveActor(RdlActor actor, List<RdlProperty> properties)
		{
			IActor obj = this.GetActor(actor.ID);

			if (obj != null)
			{
				// Update the properties and persist to the database.
				foreach (var item in properties)
				{
					if (item.ID == actor.ID)
					{
						obj.Properties.SetValue(item.Name, item.Value);
					}
				}
				this.SaveActor(obj);
			}
		}
		#endregion

		#region CreateActor
		/// <summary>
		/// Creates an IActor instance of the specified Type and searches the virtual world templates for a 
		/// template with the specified name.
		/// </summary>
		/// <param name="actorType">The System.Type of the IActor instance to create.</param>
		/// <param name="name">The name of the template to use when creating the instance.</param>
		/// <returns>A new instance of the specified type of Actor.</returns>
		public IActor CreateActor(Type actorType, string name)
		{
			IActor actor = this.Provider.GetTemplate(actorType, name);
			if (actor == null) actor = new Actor { Name = name };
			actor.World = this;
			return actor;
		}

		/// <summary>
		/// Creates an actor instance within the current virtual world.
		/// </summary>
		/// <param name="actorType">The System.Type of the Actor derived instance to create.</param>
		/// <param name="constructorArgs">Any arguments to pass to the constructor of the object.</param>
		/// <returns>An instance of an Actor of the specified type.</returns>
		public Actor CreateActor(Type actorType, params object[] constructorArgs)
		{
			if (actorType.IsSubclassOf(typeof(Actor)) || actorType == typeof(Actor))
			{
				Actor actor = Activator.CreateInstance(actorType, constructorArgs) as Actor;
				if (actor != null)
				{
					actor.World = this;
					return actor;
				}
			}
			return new Actor() { World = this };
		}
		#endregion

		#region CreatePlayer
		/// <summary>
		/// Creates a player instance in the virtual world.
		/// </summary>
		/// <param name="context">The current IMessageContext of the requestor.</param>
		/// <param name="cmd">The RdlCommand containing the player parameters.</param>
		/// <param name="player">The player instance to create.</param>
		public bool CreatePlayer(IMessageContext context, RdlCommand cmd, IPlayer player)
		{
			// Command arguments are grouped into key:value pairs, each argument is a key:value pair.

			// Find the user from rdlcommandgroup associated with this command.
			bool success = false;
			string message = SR.CreateCharacterInvalidUser;
			if (cmd.Group != null)
			{
				AuthKey key = AuthKey.Get(cmd.Group.AuthKey);

				//=================================================================================
				// Ensure the user can create additional character.
				//=================================================================================
				UserDetail user = this.Provider.GetUserDetail(key.UserName);
				if (user == null)
				{
					// Use the default error message.
					goto finished;
				}
				int playerCount = this.Provider.GetPlayers(user.UserName).Count;
				if (user.MaxCharacters == 0)
				{
					// Preset the max characters to the world default.
					user.MaxCharacters = this.DefaultMaxCharacters;
					this.Provider.SaveUserDetail(user);
				}
				if (playerCount >= user.MaxCharacters)
				{
					message = SR.CreateCharacterMaxExceeded(this.DefaultMaxCharacters);
					goto finished;
				}
				player.Properties.SetValue("UserName", user.UserName);
				
				//=================================================================================
				// Parse the key:value pairs.
				//=================================================================================
				foreach (var item in cmd.Args)
				{
					if (item != null)
					{
						string[] pairs = item.ToString().Split(':');
						if (pairs != null && pairs.Length == 2)
						{
							if (pairs[0].ToLower().Equals("name"))
							{
								player.Name = pairs[1];
							}
							else
							{
								player.Properties.SetValue(pairs[0], pairs[1]);
							}
						}
					}
				}

				//=================================================================================
				// Validate player name.
				//=================================================================================
				if (!this.IsValidName(player.Name, out message))
				{
					goto finished;
				}

				//=================================================================================
				// Ensure a gender is specified.
				//=================================================================================
				if (player.Gender == Gender.None)
				{
					message = SR.CreateCharacterInvalidGender;
					goto finished;
				}

				//=================================================================================
				// Ensure a race is specified.
				//=================================================================================
				if (String.IsNullOrEmpty(player.Race) || !this.Races.ContainsKey(player.Race))
				{
					message = SR.CreateCharacterInvalidRace;
					goto finished;
				}
				// Set the player's location from the race instance.
				player.Location = this.Races[player.Race].StartingLocation;

				//=================================================================================
				// Ensure that attributes are within acceptable range.
				//=================================================================================
				int attrTotal = 0;
				foreach (var item in player.Attributes)
				{
					if (item.Value < -1 || item.Value > 8)
					{
						message = SR.CreateCharacterAttributeOutOfRange(item.Key);
						goto finished;
					}
					else
					{
						attrTotal += item.Value;
					}
				}
				int raceDefaultAttrTotal = 0;
				foreach (var item in this.Races[player.Race].Attributes)
				{
					if (item.Value > 0)
					{
						raceDefaultAttrTotal += item.Value;
					}
				}
				// The max number of attribute points depends largely on the value of the attributes selected.
				if (attrTotal > (AttributeList.MaxAttributePoints + raceDefaultAttrTotal)) // plus race bonus defaults
				{
					message = SR.CreateCharacterAttributePointsOverLimit;
					goto finished;
				}

				//=================================================================================
				// Set Body and Mind values.
				//=================================================================================
				int body = player.Attributes.Stamina * this.RealismMultiplier;
				int mind = player.Attributes.Endurance * this.PowerMultiplier;
				if (body == 0) body = 4;
				player.SetBody(body, body);
				player.SetMind(mind, mind);
				

				//=================================================================================
				// Ensure that skills contain acceptable values and that they do not exceed 32 in value.
				//=================================================================================
				double skillTotal = 0;
				foreach (var item in player.Skills)
				{
					if (item.Value < 0 || item.Value > SkillManager.MaxSkillPointsInCharacterCreation)
					{
						message = SR.CreateCharacterSkillOutOfRange(item.Key);
						goto finished;
					}
					else
					{
						skillTotal += item.Value;
					}
				}
				if ((int)skillTotal > SkillManager.MaxSkillPointsInCharacterCreation)
				{
					message = SR.CreateCharacterSkillPointsOverLimit;
					goto finished;
				}

				//=================================================================================
				// Add any missing skills to the players list of skills.
				//=================================================================================
				foreach (var skill in this.Skills)
				{
					if (!player.Skills.ContainsKey(skill.Key))
					{
						player.Skills.Add(skill.Key, 0);
					}
				}

				//=================================================================================
				// Check to ensure the name has not been taken and it passes the word filter.
				//=================================================================================
				if (!this.IsValidName(player.Name, out message))
				{
					goto finished;
				}

				//=================================================================================
				// Save the player instance and send back a success message.
				//=================================================================================
				this.Provider.SavePlayer<IPlayer>(player);
				if (player.ID > 0)
				{
					success = true;
					message = SR.CreateCharacterSuccess;
				}
			}

			finished:
			context.Add(new RdlCommandResponse(cmd.TypeName, success, message));

			return success;
		}
		#endregion

		#region Player Methods
		/// <summary>
		/// Loads a Player instance into the current virtual world.
		/// </summary>
		/// <param name="playerId"></param>
		/// <param name="userName"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public IPlayer LoadPlayer(int playerId, string userName, IMessageContext context)
		{
			IPlayer player = this.Provider.GetPlayer(playerId);
			if (player != null)
			{
				IClient client = null;
				if (this.Server.Clients.ContainsKey(userName))
				{
					client = this.Server.Clients[userName];
				}
				if (client != null)
				{
					if (player.UserName.Equals(userName) && client.UserName.Equals(userName))
					{
						player.World = this;
						client.Player = player;
						player.Client = client;
						player.Context = context;

						// Change to the player command handler.
						client.Handler = new PlayerCommandHandler(client);

						// Clear the player's target if one was saved.
						player.Target = null;

						// Add the player to current list of avatars connected to the world.
						if (this.Avatars.ContainsKey(player.Name))
						{
							this.Avatars.Remove(player.Name);
						}
						this.Avatars.Add(player.Name, player);

						return player;
					}
				}
			}
			return null;
		}
		#endregion

		#region Find Methods
		/// <summary>
		/// Finds a Place instance at the specified location.
		/// </summary>
		/// <param name="location">The world coordinates of the place to find.</param>
		/// <returns>An instance of Place at the specified location or null if not found.</returns>
		public Place FindPlace(Point3 location)
		{
			return this.Places[location];
		}
		#endregion

		public bool IsAvatarOnline(IAvatar avatar)
		{
			return this.Avatars.Values.Where(a => a.ID == avatar.ID).FirstOrDefault() != null;
		}

		public IPlayer GetFeaturedPlayer()
        {
            int id = this.Provider.GetFeaturedPlayerId();

            // Attempt to find the player online.
            IPlayer player = this.Avatars.Values.Where(a => a.ID == id).FirstOrDefault() as IPlayer;
            if (player == null)
            {
                player = this.Provider.GetPlayer(id);
            }
            return player;
		}

		#region IsValidName
		private static Regex _safeNameRegEx = new Regex("^[a-zA-Z]*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);

		/// <summary>
		/// Gets a value indicating whether or not the specified name is a valid name, not already taken and passes the 
		/// word filtering list.
		/// </summary>
		/// <param name="name">The name to validate.</param>
		/// <param name="message">A message indicating the reason for failure.</param>
		/// <returns>True if the name is valid and available; otherwise false.</returns>
		public bool IsValidName(string name, out string message)
		{
			// Rules
			// - Min length == 3
			if (String.IsNullOrEmpty(name) || name.Length < 3)
			{
				message = SR.NameValidationInvalidLengthShort;
				return false;
			}
			// - Max of 32 characters for a name
			if (name.Length > 32)
			{
				message = SR.NameValidationInvalidLengthLong;
				return false;
			}
			// - No spaces in name
			// - No numbers in name
			// - No special characters [A-Za-z]
			if (!_safeNameRegEx.IsMatch(name))
			{
				message = SR.NameValidationInvalidCharacters;
				return false;
			}
			// - No words from invalid word list
			if (!this.Provider.IsSafeWord(name))
			{
				message = SR.NameValidationInvalidName;
				return false;
			}

			// Name has not already been used.
			if (this.Provider.ActorNameExists(name))
			{
				message = SR.NameValidationNameAlreadyExists;
				return false;
			}

			message = String.Empty;
			return true;
		}
		#endregion

		#region RDL Methods

        /// <summary>
        /// Cpnverts the specified list of places to RdlTags.
        /// </summary>
		/// <param name="places">The list of places to convert ot tags.</param>
        /// <returns>An array of RdlTag instances.</returns>
		public RdlTag[] GetRdlMap(List<Place> places)
		{
			List<RdlTag> tags = new List<RdlTag>();
			foreach (var place in places)
			{
				tags.AddRange(place.ToRdl());
			}
			return tags.ToArray();
		}


		/// <summary>
		/// Gets an array of RdlTerrain tags representing all the available terrain in the virtual world.
		/// </summary>
		/// <returns>An array of RdlTerrain tags representing all the available terrain in the virtual world.</returns>
		public RdlTerrain[] GetRdlTerrain()
		{
			List<RdlTerrain> list = new List<RdlTerrain>();
			foreach (var item in this.Terrain)
			{
				list.Add(new RdlTerrain(item.Value.ID, item.Value.Name, item.Value.Color, (int)item.Value.WalkType, item.Value.ImageUrl));
			}
			return list.ToArray();
		}
		#endregion

		#region IDisposable Members

		/// <summary>
		/// Immediately releases the unmanaged resources used by this object.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases all resources used by the World class.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				lock (this)
				{	
					
				}
			}
		}

		#endregion
	}
}
