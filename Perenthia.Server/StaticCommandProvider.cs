using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Security;

using Lionsguard.Content;
using Lionsguard.Security;

using Radiance;
using Radiance.Markup;
using Perenthia.Utility;
using System.Diagnostics;
using Lionsguard;
using Radiance.Handlers;

namespace Perenthia
{
	public class StaticCommandProvider : Radiance.Providers.CommandProvider
	{
		private static BindingFlags CommandBindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

		public override void ProcessCommand(Server server, RdlCommand cmd, IClient client)
		{
			try
			{
				if (!cmd.TypeName.Equals("LOGIN"))
					Logger.LogDebug("SERVER: Executing CMD = {0}", cmd);
				this.GetType().InvokeMember(cmd.TypeName, CommandBindingFlags, null, this, new object[] { server, cmd, client });
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.ToString());
				client.Context.Add(RdlErrorMessage.InvalidCommand);
			}
		}

		#region World Loading
		private void IsOnline(Server server, RdlCommand cmd, IClient client)
		{
			client.Context.Add(new RdlTag("ISONLINE", "ISONLINE"));
		}
        private void FileUpdates(Server server, RdlCommand cmd, IClient client)
        {
            // Check to see if any of the files need updates (skills, skillgroups, races, terrain)
            var list = server.World.Provider.GetFileUpdates();
            foreach (var item in list)
            {
                RdlTag tag = new RdlTag("FILEUPDATE", "FILEUPDATE");
                tag.Args.Add(item.FileName);
                tag.Args.Add(item.LastUpdateDate.Ticks);
                client.Context.Add(tag);
            }
        }
		private void SysTerrain(Server server, RdlCommand cmd, IClient client)
		{
			client.Context.AddRange(server.World.GetRdlTerrain());
		}
		private void SysSkills(Server server, RdlCommand cmd, IClient client)
		{
			client.Context.AddRange(server.World.Skills.ToRdl());
		}
		private void SysSkillGroups(Server server, RdlCommand cmd, IClient client)
		{
			client.Context.AddRange(server.World.SkillGroups.ToRdl());
		}
		private void SysRaces(Server server, RdlCommand cmd, IClient client)
		{
			client.Context.AddRange(server.World.Races.ToRdl());
		}
		private void News(Server server, RdlCommand cmd, IClient client)
		{
			List<Post> posts = ContentManager.GetTopPosts("Members-News", 5);
			foreach (var post in posts)
			{
				client.Context.Add(new RdlNewsMessage(post.DateCreated, post.Title, post.Author, post.Text));
			}
		}
		#endregion

		#region Login, ForgotPassword, ChangePassword, SignUp
		private void Login(Server server, RdlCommand cmd, IClient client)
		{
			if (cmd.Args.Count == 1)
			{
				var token = SecurityManager.Decrypt(cmd.GetArg<string>(0));
				if (!String.IsNullOrEmpty(token))
				{
					var pairs = token.Split('|');
					if (pairs != null && pairs.Length == 2)
					{
						AttemptLogin(pairs[0], pairs[1], true, server, client);
						return;
					}
				}
			}
			else
			{
				AttemptLogin(cmd.GetArg<string>(0), cmd.GetArg<string>(1), cmd.GetArg<bool>(2), server, client);
				return;
			}
			client.Context.Add(new RdlErrorMessage(Resources.LoginInvalid));
		}
		private void AttemptLogin(string username, string password, bool remember, Server server, IClient client)
		{
			bool isApproved;
			if (SecurityManager.ValidateUser(username, password, out isApproved))
			{
				if (isApproved)
				{
					var user = SecurityManager.GetUser(username);
					Logger.LogDebug("SERVER: User logged in: {0}", username);
					var key = new Radiance.AuthKey(client.SessionId, username, 0);
					var tag = new RdlAuthKey(key.ToString(), "USER");
					if (remember)
					{
						tag.PersistLoginToken = SecurityManager.Encrypt(String.Format("{0}|{1}", username, password));
					}

					var existingClients = server.Clients.Where(c => c.UserName == username
						&& !c.SessionId.Equals(client.SessionId));
					foreach (var existingClient in existingClients)
					{
						existingClient.Expire();
						existingClient.UserName = String.Empty;
					}

					client.Handler = new UserCommandHandler(client);
					client.UserName = username;
					client.AuthKey = key;
					client.Context.Add(tag);
					return;
				}
			}
			client.Context.Add(new RdlErrorMessage(Resources.LoginInvalid));
		}
		private void ForgotPassword(Server server, RdlCommand cmd, IClient client)
		{
			var username = cmd.GetArg<string>(0);
			var user = SecurityManager.GetUser(username);
			if (user != null)
			{
				SecurityManager.ResendPasswordWithoutSecurityAnswer(username, user.Email);
			}
		}
		private void ChangePassword(Server server, RdlCommand cmd, IClient client)
		{
		}
		private void SignUp(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = username
			// 1 = password
			// 2 = email
			// 3 = display name
			// 4 = birth date
			// 5 = question
			// 6 = answer
			var username = cmd.GetArg<string>(0);
			var password = cmd.GetArg<string>(1);
			var email = cmd.GetArg<string>(2);
			var displayName = cmd.GetArg<string>(3);
			var birthDateTicks = cmd.GetArg<string>(4);
			var question = cmd.GetArg<string>(5);
			var answer = cmd.GetArg<string>(6);
			DateTime birthDate = DateTime.MinValue;

			var retCmd = new RdlCommand("SIGNUP");
			var success = false;
			var msg = String.Empty;

			long ticks;
			if (Int64.TryParse(birthDateTicks, out ticks))
				birthDate = new DateTime(ticks);

			try
			{
				var status = SecurityManager.CreateUser(username, displayName, password, email, birthDate.ToShortDateString(), question, answer);
				success = status == MembershipCreateStatus.Success;
				if (success)
				{
					// Add the user's email address to the mailing list.
					Game.AddUserToMailingList(username, email);
				}
				else
				{
					msg = SecurityManager.ErrorCodeToString(status);
				}
			}
			catch (Exception ex)
			{
				success = false;
				msg = "An error has prevented the signup process from completing. Please contact membership@lionsguard.com for assistance with registration.";
				Logger.LogError(ex.ToString());
			}

			// Send back a SIGNUP command with the following params:
			// 0 = success
			// 1 = error message
			retCmd.Args.Add(success);
			retCmd.Args.Add(msg);

			client.Context.Add(retCmd);
		}
		#endregion

		#region Chat Commands
		private void Say(Server server, RdlCommand cmd, IClient client)
		{
			if (client.Player != null)
			{
				string msg = cmd.GetArg<string>(0);
				if (!String.IsNullOrEmpty(msg))
				{
					// Chat is global now.
					server.SendAll(new RdlChatMessage(
						client.Player.Name,
						String.Format(Resources.MsgChat, client.Player.Name, msg)),
						client.Player);
				}
			}
		}
		private void Shout(Server server, RdlCommand cmd, IClient client)
		{
			if (client.Player != null)
			{
				string msg = cmd.GetArg<string>(0);
				if (!String.IsNullOrEmpty(msg))
				{
					server.SendAll(new RdlChatMessage(client.Player.Name,
						String.Format(Resources.MsgShout, client.Player.Name, msg.ToUpper())),
						client.Player);
				}
			}
		}
		private void Tell(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Target
			// 1 = Message
			string alias = cmd.GetArg<string>(0);
			string msg = cmd.GetArg<string>(1);

			if (!String.IsNullOrEmpty(alias) && !String.IsNullOrEmpty(msg))
			{
				Avatar who = server.World.GetActor(alias, null, false) as Avatar; // Don't load the object from the database if not found.
				if (who != null)
				{
					if (!who.IsDead)
					{
						who.Context.Add(new RdlTellMessage(client.Player.Name,
							String.Format(Resources.MsgTell, client.Player.Name, msg)));
					}
					else
					{
						client.Context.Add(new RdlErrorMessage(Resources.CanNotSpeakToDead));
					}
				}
				else
				{
					client.Context.Add(new RdlErrorMessage(String.Format(Resources.TargetNotOnline, alias)));
				}
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(server.World.Commands["TELL"].Help));
			}
		}
		private void Reply(Server server, RdlCommand cmd, IClient client)
		{
			this.Tell(server, cmd, client);
		}
		#endregion

		#region Who, Help, Emotes
		private void Who(Server server, RdlCommand cmd, IClient client)
		{
			// No Args
			foreach (var avatar in server.World.Avatars.Values.Select(c => c as Character))
			{
				if (avatar != null)
				{
					client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
						String.Format(Resources.WhoFormat, avatar.Name, avatar.Level, avatar.Race, avatar.Gender)));
					//client.Context.AddRange(avatar.ToSimpleRdl());
				}
			}
		}
		private void ShowHelp(Server server, IClient client, string command)
		{
			this.Help(server, new RdlCommand("HELP", command), client);
		}
		private void Help(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Command Name
			string cmdName = cmd.GetArg<string>(0);
			if (!String.IsNullOrEmpty(cmdName) && server.World.Commands.ContainsKey(cmdName))
			{
				client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
					server.World.Commands[cmdName].GetFullHelp()));
			}
			else
			{
				// List all of the visible commands.
				foreach (var command in server.World.Commands.Values.Where(c => c.IsVisible))
				{
					client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
						command.GetFullHelp()));
				}
			}
		}
		private void Emote(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Emote Type
			// 1 = Who ID, Name, Alias
			bool handled = true;
			string type = cmd.GetArg<string>(0);
			string whoAlias = cmd.GetArg<string>(1);
			IAvatar who = null;
			if (!String.IsNullOrEmpty(whoAlias))
			{
				who = server.World.GetActor(whoAlias, null) as IAvatar;
			}
			if (who == null)
			{
				who = client.Player.Target as IAvatar;
			}
			if (!String.IsNullOrEmpty(type))
			{
				switch (type.ToUpper())
				{
					case "DANCE":
						client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote, Resources.EmoteDanceSelf));
						client.Player.Place.SendAll(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote,
							String.Format(Resources.EmoteDance, client.Player.Name)), client.Player);
						break;
					case "WAVE":
						if (who != null)
						{
							client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote,
								String.Format(Resources.EmoteWaveTarget, who.A())));

							who.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote,
								String.Format(Resources.EmoteWaveByTarget, client.Player.Name)));
						}
						else
						{
							client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote, Resources.EmoteWaveSelf));
							client.Player.Place.SendAll(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote,
								String.Format(Resources.EmoteWave, client.Player.Name)), client.Player);
						}
						break;
					case "LOL":
						client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote, Resources.EmoteLaughSelf));
						client.Player.Place.SendAll(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote,
							String.Format(Resources.EmoteLaugh, client.Player.Name)), client.Player);
						break;
					case "CRY":
						client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote, Resources.EmoteCrySelf));
						client.Player.Place.SendAll(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote,
							String.Format(Resources.EmoteCry, client.Player.Name)), client.Player);
						break;
					case "GRIN":
						client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote, Resources.EmoteGrinSelf));
						client.Player.Place.SendAll(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote,
							String.Format(Resources.EmoteGrin, client.Player.Name)), client.Player);
						break;
					case "SMILE":
						client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote, Resources.EmoteSmileSelf));
						client.Player.Place.SendAll(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote,
							String.Format(Resources.EmoteSmile, client.Player.Name)), client.Player);
						break;
					case "SING":
						client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote, Resources.EmoteSingSelf));
						client.Player.Place.SendAll(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote,
							String.Format(Resources.EmoteSing, client.Player.Name)), client.Player);
						break;
					case "WINK":
						if (who != null)
						{
							client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote,
								String.Format(Resources.EmoteWinkTarget, who.A())));

							who.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote,
								String.Format(Resources.EmoteWinkByTarget, client.Player.Name)));
						}
						else
						{
							client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote, Resources.EmoteWinkSelf));
							client.Player.Place.SendAll(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote,
								String.Format(Resources.EmoteWink, client.Player.Name)), client.Player);
						}
						break;
					default:
						handled = false;
						break;
				}
			}
			else
			{
				handled = false;
			}

			if (!handled)
			{
				// String together any arguments found for this command and post them as an emote.
				StringBuilder sb = new StringBuilder();
				foreach (var item in cmd.Args)
				{
					if (item != null)
					{
						sb.Append(item).Append(' ');
					}
				}
				if (!sb.ToString().Trim().EndsWith(".")) sb.Append(".");
				string msg = sb.ToString().Trim();
				client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote, 
					String.Format(Resources.EmoteSelf, msg)));
				client.Player.Place.SendAll(new RdlSystemMessage(RdlSystemMessage.PriorityType.Emote,
					String.Format(Resources.Emote, client.Player.AUpper(), msg)), client.Player);
			}
		}
		private void Wave(Server server, RdlCommand cmd, IClient client)
		{
			this.Emote(server, new RdlCommand("EMOTE", "WAVE"), client);
		}
		private void Dance(Server server, RdlCommand cmd, IClient client)
		{
			this.Emote(server, new RdlCommand("EMOTE", "DANCE"), client);
		}
		private void Lol(Server server, RdlCommand cmd, IClient client)
		{
			this.Emote(server, new RdlCommand("EMOTE", "LOL"), client);
		}
		private void Laugh(Server server, RdlCommand cmd, IClient client)
		{
			this.Emote(server, new RdlCommand("EMOTE", "LOL"), client);
		}
		private void Cry(Server server, RdlCommand cmd, IClient client)
		{
			this.Emote(server, new RdlCommand("EMOTE", "CRY"), client);
		}
		private void Sob(Server server, RdlCommand cmd, IClient client)
		{
			this.Emote(server, new RdlCommand("EMOTE", "SOB"), client);
		}
		private void Grin(Server server, RdlCommand cmd, IClient client)
		{
			this.Emote(server, new RdlCommand("EMOTE", "GRIN"), client);
		}
		private void Smile(Server server, RdlCommand cmd, IClient client)
		{
			this.Emote(server, new RdlCommand("EMOTE", "SMILE"), client);
		}
		private void Sing(Server server, RdlCommand cmd, IClient client)
		{
			this.Emote(server, new RdlCommand("EMOTE", "SING"), client);
		}
		private void Wink(Server server, RdlCommand cmd, IClient client)
		{
			this.Emote(server, new RdlCommand("EMOTE", "WINK"), client);
		}
		#endregion

		#region Heartbeat, Look, Inventory, Map, Equip, Unequip
		private void Heartbeat(Server server, RdlCommand cmd, IClient client)
		{
		}
		private void Look(Server server, RdlCommand cmd, IClient client)
		{
			if (client.Player != null)
			{
				client.Player.Place.Look(client.Player);
			}
		}
		private void Inventory(Server server, RdlCommand cmd, IClient client)
		{
			// If ID, Name or Alias is specified then this is an inventory check on a corpse; otherwise
			// it is the player's inventory.
			string alias = cmd.GetArg<string>(0);
			if (!String.IsNullOrEmpty(alias))
			{
				// Check for the target of the player first.
				IAvatar target = client.Player.Target as IAvatar;
				if (target == null)
				{
					target = server.World.GetActor(alias, null) as IAvatar;
					if (target is PerenthiaMobile)
					{
						target = (target as PerenthiaMobile).GetOrCreateInstance(client.Player.ID);
						client.Player.SetTarget(target);
					}
				}
				if (target != null)
				{
					// Target must be dead, must be in the same room as the player and must have been killed by the player.
					if ((!(target is PerenthiaMobile) || !target.IsDead)
						&& (target.X == client.Player.X && target.Y == client.Player.Y && target.Z == client.Player.Z))
					{
						client.Context.Add(new RdlErrorMessage(String.Format(Resources.CanNotViewTargetInventory, target.A())));
					}
					else
					{
						// Send down the inventory of the target.
						target.Inventory(client.Player, client.Context);
					}
				}
				else
				{
					client.Context.Add(new RdlErrorMessage(String.Format(Resources.TargetNotFound, cmd.GetArg<string>(0))));
				}
			}
			else
			{
				if (client.Player != null)
				{
					client.Player.Inventory(null, client.Context);
				}
			}
		}
		private void Map(Server server, RdlCommand cmd, IClient client)
		{
			MapManager.MapDetail detail = null;
			if (cmd.Args.Count > 1)
            {
                int centerX = cmd.GetArg<int>(0);
                int centerY = cmd.GetArg<int>(1);
                int centerZ = cmd.GetArg<int>(2);
				detail = server.World.Map.GetDetail(new Point3(centerX, centerY, centerZ));
            }
            else
            {
				string mapName = cmd.GetArg<string>(0);
				detail = server.World.Map.GetDetail(mapName);
			}

			if (detail != null)
			{
				client.Context.AddRange(server.World.GetRdlMap(server.World.Map.GetMap(detail.Name, 
					detail.Key.StartX, detail.Key.StartY, detail.Width, detail.Height)));
			}
		}
		private void Equip(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Item ID, Name or Alias
			// 1 = EquipmentSlot
			Character character = client.Player as Character;
			Item item = server.World.GetActor(cmd.GetArg<string>(0), client.Player) as Item;
			EquipmentSlot slot = EquipmentCollection.ParseEquipmentSlot(cmd.GetArg<string>(1), item, character);
			if (item != null)
			{
				if (item is ISpell)
				{
					character.Spells.Add((ISpell)item);
				}
				else if (item is Container)
				{
					if (character.Bags.Count < 5)
					{
						item.Owner = null;
						character.Bags.Add((Container)item);
					}
					else
					{
						client.Context.Add(new RdlErrorMessage(String.Format(Resources.CanNotEquipMoreBags, item.A())));
					}
				}
				else
				{
					character.Equip(slot, item);
				}
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(Resources.ItemCanNotEquipNotInInventory));
			}
		}
		private void Unequip(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Item ID, Name or Alias
			// 1 = EquipmentSlot
			// 2 = Container ID, Name or Alias
			Character character = client.Player as Character;
			Item item = server.World.GetActor(cmd.GetArg<string>(0), client.Player) as Item;
			EquipmentSlot slot = EquipmentCollection.ParseEquipmentSlot(cmd.GetArg<string>(1), item, character);
			if (item != null)
			{
				if (item is ISpell)
				{
					client.Context.Add(new RdlErrorMessage(Resources.ItemCanNotUnequipNotInEquipment));
				}
				else if (item is Container)
				{
					// Container must be empty to unequip it.
					Container bag = item as Container;
					if (bag.Count > 0)
					{
						client.Context.Add(new RdlErrorMessage(String.Format(Resources.CanNotUnequipBag, bag.AUpper())));
					}
					else
					{
						Container destination = character.GetFirstAvailableContainer();
						if (destination != null)
						{
							character.Bags.Remove(bag);
							bag.Drop(destination);
						}
						else
						{
							client.Context.Add(new RdlErrorMessage(Resources.ItemCanNotEquipNoContainer));
						}
					}
				}
				else
				{
					Container container = null;
					if (!String.IsNullOrEmpty(cmd.GetArg<string>(2)))
					{
						container = server.World.GetActor(cmd.GetArg<string>(2), client.Player) as Container;
					}
					if (container == null)
					{
						container = character.GetFirstAvailableContainer(item);
					}
					if (container != null)
					{
						character.Unequip(slot, item, container);
					}
					else
					{
						client.Context.Add(new RdlErrorMessage(Resources.ItemCanNotEquipNoContainer));
					}
				}
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(Resources.ItemCanNotUnequipNotInEquipment));
			}
		}
		#endregion

		#region Goods, Use, Give, Get, Drop, Buy, Sell, CombatAction
		private void Goods(Server server, RdlCommand cmd, IClient client)
		{	
			// Args:
			// 0 = NPC ID, Name or Alias
			Npc npc = server.World.GetActor(cmd.GetArg<string>(0), null) as Npc;
			if (npc != null)
			{
				// Ensure the NPC is in the same room as the player.
				if (npc.Location == client.Player.Location)
				{
					// Send the npc back down as the player's target.
					client.Player.SetTarget(npc);

					// Send down the goods.
					npc.Inventory(client.Player, client.Context);
				}
				else
				{
					client.Context.Add(new RdlErrorMessage(Resources.CanNotViewNpcGoodsNotInPlace));
				}
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(String.Format(Resources.ActorNotFound, cmd.GetArg<string>(0))));
			}
		}
		private void Use(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Item ID, Name or Alias
			Item item = server.World.GetActor(cmd.GetArg<string>(0), client.Player) as Item;
			if (item != null)
			{
				item.Use(client.Player, client.Player.Context);
				server.World.SaveActor(item);
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(String.Format(Resources.ItemNotFound, cmd.GetArg<string>(0))));
			}
		}
		private void Give(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = ItemID, Name or Alias
			// 1 = ReceiverID, Name or Alias
			// 2 = Quantity
			string itemId = cmd.GetArg<string>(0);
			string takerId = cmd.GetArg<string>(1);
			int quantity = cmd.GetArg<int>(2);
			if (quantity == 0) quantity = 1;

			Item item = server.World.GetActor(itemId, client.Player) as Item;
			IActor taker = server.World.GetActor(takerId, null);
			if (item != null)
			{
				if (taker != null)
				{
					if (item.Drop(client.Player, taker, quantity))
					{
						if (taker is IAvatar)
						{
							client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
								String.Format(Resources.GaveItem, item.A(quantity), taker.The())));
						}
						else
						{
							client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
								String.Format(Resources.PutItem, item.A(quantity), taker.The())));
						}
					}
				}
				else
				{
					client.Context.Add(new RdlErrorMessage(Resources.GiveWho));
				}
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(String.Format(Resources.ItemNotFound, itemId)));
			}
		}
		private void Get(Server server, RdlCommand cmd, IClient client)
		{
			// **NOTE: Can only GET items from dead mobiles or items with a Place as an owner and 
			// the place is not owned by another player.

			// Args:
			// 0 = ItemID, Name or Alias
			// 1 = Quantity
			string id = cmd.GetArg<string>(0);
			int quantity = cmd.GetArg<int>(1);
			if (quantity == 0) quantity = 1;
			// Try to get the item from the current target first.
			Item item = server.World.GetActor(id, client.Player.Target) as Item;
			if (item != null)
			{
				Place place = item.Owner as Place;
				if (place != null && place.Owner != null && place.Owner.ID != client.Player.ID)
				{
					client.Context.Add(new RdlErrorMessage(String.Format(Resources.CanNotGet, item.Alias())));
					return;
				}
				Npc npc = item.Owner as Npc;
				if (npc != null && npc is IMerchant)
				{
					client.Context.Add(new RdlErrorMessage(String.Format(Resources.CanNotGet, item.Alias())));
					return;
				}
				PerenthiaMobile mobile = item.Owner as PerenthiaMobile;
				if (mobile != null && !(mobile.IsDead))
				{
					client.Context.Add(new RdlErrorMessage(String.Format(Resources.CanNotGet, item.Alias())));
					return;
				}

				if (item.Drop(item.Owner, client.Player, quantity))
				{
					client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
						   String.Format(Resources.GotItem, item.A(quantity))));
				}
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(String.Format(Resources.ItemNotFound, id)));
			}
		}
		private void Drop(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = ItemID, Name or Alias
			// 1 = Quantity
			string id = cmd.GetArg<string>(0);
			int quantity = cmd.GetArg<int>(1);
			if (quantity == 0) quantity = 1;
			Item item = server.World.GetActor(id, null) as Item;
			if (item != null)
			{
				item.Drop(client.Player, client.Player.Place, quantity);
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(String.Format(Resources.ItemNotFound, id)));
			}
		}
		private void Buy(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = ItemID, Name or Alias
			// 1 = Quantity
			string id = cmd.GetArg<string>(0);
			int quantity = cmd.GetArg<int>(1);
			if (quantity == 0) quantity = 1;
			Item item = server.World.GetActor(id, null) as Item;
			Character player = client.Player as Character;
			bool purchaseSuccess = false;
			if (item != null)
			{
				// Ensure the Owner is an NPC and is also a merchant.
				if (item.Owner != null)
				{
					Npc owner = item.Owner as Npc;
					// Ensure the Owner is an NPC and is also a merchant.
					if (owner != null && owner is IMerchant)
					{
						// Adjust quantity.
						if (quantity > item.Quantity())
						{
							quantity = item.Quantity();
						}

						// Get an empty container to place the new item.
						Container container = player.GetFirstAvailableContainer();
						if (container != null && container.GetRemainingSlots(item) >= quantity)
						{
							// Ensure the player has enough money or emblem.
							if (item.GetEmblemCost(player) > 0)
							{
								// Emblem
								User user = SecurityManager.GetUser(client.UserName);
								if (user.Tokens >= (item.GetEmblemCost(player) * quantity))
								{
									user.Tokens -= item.GetEmblemCost(player) * quantity;
									Membership.UpdateUser(user);

									// Send the new emblem value down.
									client.Context.Add(new RdlProperty(player.ID, "Emblem", user.Tokens));

									purchaseSuccess = true;
								}
								else
								{
									client.Context.Add(new RdlErrorMessage(
										String.Format(Resources.NotEnoughEmblem,
										user.Tokens, item.EmblemCost)));
								}
							}
							else
							{
								// Currency
								if (player.Currency.Value >= (item.GetBuyCost(player, owner.MarkupPercentage) * quantity))
								{
									player.Currency.Value -= item.GetBuyCost(player, owner.MarkupPercentage) * quantity;
									server.World.SaveActor(player);

									// Send the new currency value down.
									client.Context.AddRange(player.GetRdlProperties(Character.CurrencyProperty));

									purchaseSuccess = true;
								}
								else
								{
									client.Context.Add(new RdlErrorMessage(String.Format(Resources.CanNotAfford, item.A(quantity))));
								}
							}

							if (purchaseSuccess)
							{
								if (item.Drop(owner, player, quantity))
								{
									client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
										String.Format(Resources.ItemPurchased, item.A(quantity))));
								}
							}
						}
						else
						{
							client.Context.Add(new RdlErrorMessage(Resources.CanNotPurchaseInventoryFull));
						}
					}
					else
					{
						client.Context.Add(new RdlErrorMessage(String.Format(Resources.ItemCanNotPurchase, item.A(quantity))));
					}
				}
				else
				{
					client.Context.Add(new RdlErrorMessage(String.Format(Resources.ItemNotFound, id)));
				}
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(String.Format(Resources.ItemNotFound, id)));
			}
		}
		private void Sell(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = ItemID, Name or Alias
			// 1 = Quantity
			// 2 = To Whom ID, Name or Alias
			Item item = server.World.GetActor(cmd.GetArg<string>(0), client.Player) as Item;
			int quantity = cmd.GetArg<int>(1);
			if (quantity == 0) quantity = 1;
			// Can only sell to NPCs, Player's TRADE.
			Npc npc = server.World.GetActor(cmd.GetArg<string>(2), null) as Npc;
			Character player = client.Player as Character;
			if (item != null)
			{
				if (npc != null)
				{
					if (npc is IMerchant)
					{
						// Adjust quantity.
						if (quantity > item.Quantity())
						{
							quantity = item.Quantity();
						}

						if (item.GetEmblemCost(npc) > 0)
						{
							// Emblem
							User user = SecurityManager.GetUser(client.UserName);
							user.Tokens += item.GetEmblemCost(npc) * quantity;
							Membership.UpdateUser(user);

							// Send the new emblem value down.
							client.Context.Add(new RdlProperty(player.ID, "Emblem", user.Tokens));
						}
						else
						{
							// Currency
							player.Currency.Value += item.GetSellCost(npc, npc.MarkdownPercentage) * quantity;
							server.World.SaveActor(player);

							// Send the new currency value down.
							client.Context.AddRange(player.GetRdlProperties(Character.CurrencyProperty));
						}

						if (item.Drop(player, npc, quantity))
						{
							client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
								String.Format(Resources.ItemSold, item.A(quantity))));
						}
					}
					else
					{
						client.Context.Add(new RdlErrorMessage(String.Format(Resources.ItemCanNotSell, item.A(), npc.Name)));
					}
				}
				else
				{
					client.Context.Add(new RdlErrorMessage(Resources.ItemsMustBeSoldToNPCs));
				}
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(String.Format(Resources.ItemNotFound, cmd.GetArg<string>(0))));
			}
		}
		private void CombatAction(Server server, RdlCommand cmd, IClient client)
		{
			if (client.Player != null)
				client.Player.CombatAction = cmd.GetArg<string>(0);
		}
		#endregion

		#region SetSkill, SetAction, Target
		private void SetSkill(Server server, RdlCommand cmd, IClient client)
		{
			// TODO: Implement SETSKILL Command.
			// Ensure player has the skill points to make changes.
		}
		private void SetAction(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = ItemID, Name or Alias
			// 1 = Action Slot Number
			Item item = server.World.GetActor(cmd.GetArg<string>(0), client.Player) as Item;
			int slotNum = cmd.GetArg<int>(1); // Slots are zero-based indexed.
            (client.Player as Character).SetAction(slotNum, item);
		}
		private void Target(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Target ID, Name or Alias
			Avatar target = server.World.GetActor(cmd.GetArg<string>(0), null) as Avatar;
			if (target != null)
			{
				if (target.Location == client.Player.Location)
				{
					if (target.ID == client.Player.ID || target.Target == null || target.Target.ID == client.Player.ID)
					{
						if (target is PerenthiaMobile) target = (target as PerenthiaMobile).GetOrCreateInstance(client.Player.ID);
						client.Player.SetTarget(target);
					}
					else
					{
						client.Context.Add(new RdlErrorMessage(String.Format(Resources.TargetEngaged, target.TheUpper())));
					}
				}
				else
				{
					client.Context.Add(new RdlErrorMessage(Resources.CanNotTargetNotInPlace));
				}
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(String.Format(Resources.TargetNotFound, cmd.GetArg<string>(0))));
			}
		}
		#endregion

		#region Attack, Cast
		private void Attack(Server server, RdlCommand cmd, IClient client)
		{
		}
		private void Cast(Server server, RdlCommand cmd, IClient client)
		{
		}
		#endregion

		#region Movement
		private void Move(Server server, RdlCommand cmd, IClient client)
		{
			// cmd.Args[0] = direction string
			string msg = "You can not move in that direction";
			Direction dir = Direction.FromName(cmd.GetArg<string>(0));
			Point3 loc = client.Player.Location;
			loc += dir.Value;

			if (client.Player.Place.Exits.GetValue(dir.KnownDirection))
			{
				Place newPlace = server.World.FindPlace(loc);
				if (newPlace != null)
				{
					// Ensure the player can move into this place.
					bool canMove = false;
					Terrain terrain = server.World.Terrain[newPlace.Terrain];
					if (terrain != null)
					{
						if ((terrain.WalkType & WalkTypes.Walk) == WalkTypes.Walk)
						{
							canMove = true;
						}
						else if ((terrain.WalkType & WalkTypes.Fly) == WalkTypes.Fly)
						{
							// TODO: Need to check to see if the player is flying.
						}
						else if ((terrain.WalkType & WalkTypes.Swim) == WalkTypes.Swim)
						{
							// TODO: Need to check to see if the player can swim or has a boat equipped.
						}
					}

					if (canMove)
					{
						int lastZ = client.Player.Z;

						// Enter the new place.
						newPlace.Enter(client.Player, dir);

                        // Add the player's current zone information.
                        MapManager.MapDetail detail = server.World.Map.GetDetail(client.Player.Location);
                        if (detail != null)
                        {
                            string zone = client.Player.Properties.GetValue<string>(Character.ZoneProperty);
                            if (detail.Name != zone)
                            {
                                client.Player.Properties.SetValue(Character.ZoneProperty, detail.Name);
								client.Context.Add(new RdlProperty(client.Player.ID, Character.ZoneProperty, zone));
                            }
                        }

                        //// If the player moved in a z direction send the map for that z-index.
                        //if (client.Player.Z != lastZ)
                        //{
                        //    client.Context.AddRange(server.World.GetRdlMap(client.Player.X, client.Player.Y, client.Player.Z));
                        //}
						return;
					}
				}
			}
			
			client.Context.Add(new RdlErrorMessage(msg));

            // Resend the contents of the current room.
            client.Player.Place.Look(client.Player);
		}
		private void North(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "North"), client);
		}
		private void South(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "South"), client);
		}
		private void East(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "East"), client);
		}
		private void West(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "West"), client);
		}
		private void Northeast(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "Northeast"), client);
		}
		private void Northwest(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "Northwest"), client);
		}
		private void Southeast(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "Southeast"), client);
		}
		private void Southwest(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "Southwest"), client);
		}
		private void Up(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "Up"), client);
		}
		private void Down(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "Down"), client);
		}
		private void N(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "North"), client);
		}
		private void S(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "South"), client);
		}
		private void E(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "East"), client);
		}
		private void W(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "West"), client);
		}
		private void NE(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "Northeast"), client);
		}
		private void NW(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "Northwest"), client);
		}
		private void SE(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "Southeast"), client);
		}
		private void SW(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "Southwest"), client);
		}
		private void U(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "Up"), client);
		}
		private void D(Server server, RdlCommand cmd, IClient client)
		{
			this.Move(server, new RdlCommand("MOVE", "Down"), client);
		}
		#endregion

		#region Quests, StartQuest, QuestLog
		private void Quests(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Owner ID, Name or Alias
			string alias = cmd.GetArg<string>(0);
			IActor owner = server.World.GetActor(alias, null);
			if (owner != null)
			{
				// Ensure owner is in same location.
				if ((owner is IAvatar && (owner as IAvatar).Location != client.Player.Location)
					|| (owner.Owner != null && (owner.Owner is IAvatar && (owner.Owner as IAvatar).Location != client.Player.Location))
					|| (owner.Owner.Owner != null && (owner.Owner.Owner is IAvatar && (owner.Owner.Owner as IAvatar).Location != client.Player.Location)))
				{
					client.Context.Add(new RdlErrorMessage(Resources.QuestNotInPlace));
					return;
				}

				client.Player.Target = owner;
				if (owner is IAvatar) (owner as IAvatar).Target = client.Player;
				client.Context.AddRange(client.Player.GetRdlProperties(Avatar.TargetIDProperty));
				client.Context.AddRange(owner.ToSimpleRdl());

				// No restrictions for levels.
				foreach (var item in owner.GetActiveQuests())
				{
					client.Context.AddRange(item.ToRdl());
				}
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(String.Format(Resources.ActorNotFound, alias)));
			}
		}
		private void StartQuest(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Quest ID, Name or Alias
			// 1 = Owner ID, Name or Alias
			string questAlias = cmd.GetArg<string>(0);
			string ownerAlias = cmd.GetArg<string>(1);
			IActor owner = server.World.GetActor(ownerAlias, null);
			Quest quest = server.World.GetActor(questAlias, owner) as Quest;
			if (quest != null && owner != null)
			{
				// Ensure the quest giver is in the same room as the player.
				if ((owner is IAvatar && (owner as IAvatar).Location != client.Player.Location)
					|| (owner.Owner != null && (owner.Owner is IAvatar && (owner.Owner as IAvatar).Location != client.Player.Location))
					|| (owner.Owner.Owner != null && (owner.Owner.Owner is IAvatar && (owner.Owner.Owner as IAvatar).Location != client.Player.Location)))
				{
					client.Context.Add(new RdlErrorMessage(Resources.QuestNotInPlace));
					return;
				}

				// Ensure the player meets the order requirements.
				if (quest.RequiredOrder != OrderType.None)
				{
					// TODO: Verify that the player is a part of the same order this quest requires.
				}

				// Let the quest instance determine if the player can start or has already started or compeleted
				// the current quest.
				// StartQuest will append messages to the avatar's context and set property values for the quest.
				quest.Start(client.Player);
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(String.Format(Resources.QuestNotFound, questAlias)));
			}
		}
		private void CompleteQuest(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Quest ID, Name or Alias 
			Quest quest = client.Player.GetActiveQuests().Where(q => q.Name == cmd.GetArg<string>(0)).FirstOrDefault() as Quest;
			if (quest != null)
			{
				quest.Complete();
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(Resources.QuestNotStarted));
			}
		}
		private void QuestLog(Server server, RdlCommand cmd, IClient client)
		{
			//// Args:
			//// No args, only active for current player.

			//// Need to find all of the active and completed quests this player current has stored.
			//var props = client.Player.Properties.Where(p => p.Key.StartsWith("Quest_"));
			//foreach (var p in props)
			//{
			//    Quest quest = server.World.Quests.GetQuest(Quest.GetQuestIDFromKey(p.Key)) as Quest;
			//    if (quest != null)
			//    {
			//        client.Context.AddRange(quest.ToRdl());
			//    }
			//}
		}
		#endregion

		#region Character Commands
		private void Connect(Server server, RdlCommand cmd, IClient client)
		{
			// Should encompass all the commands that occur when an authenticated user first connects
			// to the server.

			// Role
			CheckRole(server, new RdlCommand("CHECKROLE", "God") { Group = cmd.Group }, client);

			// File Updates
			FileUpdates(server, new RdlCommand("FILEUPDATES") { Group = cmd.Group }, client);

			//// Skills
			//SysSkills(server, new RdlCommand("SYSSKILLS") { Group = cmd.Group }, client);

			//// Skill Groups
			//SysSkillGroups(server, new RdlCommand("SYSSKILLGROUPS") { Group = cmd.Group }, client);

			//// Races
			//SysRaces(server, new RdlCommand("SYSRACES") { Group = cmd.Group }, client);

			//// Terrain
			//SysTerrain(server, new RdlCommand("SYSTERRAIN") { Group = cmd.Group }, client);

			// Characters
			Characters(server, new RdlCommand("CHARACTERS") { Group = cmd.Group }, client);

			// Send a connect command back to the client to signal the end of the group.
			client.Context.Add(new RdlCommand("CONNECT"));
		}
		private void CheckRole(Server server, RdlCommand cmd, IClient client)
		{
			bool isInRole = false;
			string role = cmd.GetArg<string>(0);
			if (!String.IsNullOrEmpty(role))
			{
				isInRole = Roles.IsUserInRole(server.GetUserNameFromCommand(cmd), role);
			}
			if (!isInRole) role = RoleNames.Mortal;
			
			client.Context.Add(new RdlCommand("CHECKROLE", role, isInRole));
		}
		private void CheckName(Server server, RdlCommand cmd, IClient client)
		{
			string msg;
			string name = cmd.GetArg<string>(0);
			if (server.World.IsValidName(name, out msg))
			{
				client.Context.Add(new RdlCommandResponse("CHECKNAME", true, String.Format(Resources.NameAvailable, name)));
			}
			else
			{
				client.Context.Add(new RdlCommandResponse("CHECKNAME", false, String.Format(Resources.NameUnavailable, name, msg)));
			}
		}
		private void Characters(Server server, RdlCommand cmd, IClient client)
		{
			string username = server.GetUserNameFromCommand(cmd);

			RdlUser user = new RdlUser();
			user.MaxCharacters = server.GetUserDetail(username).MaxCharacters;
			client.Context.Add(user);

			var players = server.World.Provider.GetPlayers(username);
			if (players.Count > 0)
			{
				foreach (var p in players)
				{
					client.Context.AddRange(p.ToRdl());
				}
			}
			else
			{
				client.Context.Add(RdlTag.Empty);
			}
		}
		private void CreateCharacter(Server server, RdlCommand cmd, IClient client)
		{
			Character player = new Character();

			// Add some currency to the player instance.
			//player.Currency = new Currency(25);

			// Allow the world to create the player instance.
			if (server.World.CreatePlayer(client.Context, cmd, player))
			{
				//// Add starting items and clothing to the new character.
				//// Backpack
				//Container backpack = server.World.CreateFromTemplate<Container>("Adventurer's Backpack");
				//// Candle
				//Light candle = server.World.CreateFromTemplate<Light>("Tallow Candle");
				//// Shirt
				//Clothing shirt = server.World.CreateFromTemplate<Clothing>("Woolen Shirt");
				//// Pants
				//Clothing pants = server.World.CreateFromTemplate<Clothing>("Woolen Pants");
				//// Bread, Cheese and Water
				//Food bread = server.World.CreateFromTemplate<Food>("Bread");
				//Food cheese = server.World.CreateFromTemplate<Food>("Cheese");
				//Food water = server.World.CreateFromTemplate<Food>("Water");

				//// All players need the hand to hand skill.
				//if (player.Skills[Skills.HandToHand] < 2) player.Skills[Skills.HandToHand] = 2;

				//// The choice of primary, starting weapon and spell is dependent on the skills the player has selected.
				//Weapon weapon = null;
				//Spell spell = null;
				//if (player.Skills[Skills.Daggers] > 0) weapon = server.World.CreateFromTemplate<Weapon>("Rusty Dagger");
				//else if (player.Skills[Skills.Swords] > 0) weapon = server.World.CreateFromTemplate<Weapon>("Rusty Short Sword");
				//else if (player.Skills[Skills.Axes] > 0) weapon = server.World.CreateFromTemplate<Weapon>("Worn Axe");
				//else if (player.Skills[Skills.Maces] > 0) weapon = server.World.CreateFromTemplate<Weapon>("Wooden Mace");
				//else if (player.Skills[Skills.Clubs] > 0) weapon = server.World.CreateFromTemplate<Weapon>("Wooden Club");
				//else weapon = server.World.CreateFromTemplate<Weapon>(Skills.Fists);

				//if (player.Skills[Skills.Life] > 0) spell = server.World.CreateFromTemplate<Spell>("Minor Healing");
				//else if (player.Skills[Skills.ElementalismEarth] > 0) spell = server.World.CreateFromTemplate<Spell>("Earth Shield");
				//else if (player.Skills[Skills.ElementalismAir] > 0) spell = server.World.CreateFromTemplate<Spell>("Air Shield");
				//else if (player.Skills[Skills.ElementalismWater] > 0) spell = server.World.CreateFromTemplate<Spell>("Water Shield");
				//else if (player.Skills[Skills.ElementalismFire] > 0) spell = server.World.CreateFromTemplate<Spell>("Fire Shield");
				//else spell = server.World.CreateFromTemplate<Spell>(Skills.ComboAttack);

				//// Set the owner of the items to the current player.
				//backpack.Owner = weapon.Owner = shirt.Owner = pants.Owner = candle.Owner = spell.Owner = player;

				//// Equip the items that need equipping.
				//backpack.IsEquipped = weapon.IsEquipped = shirt.IsEquipped = pants.IsEquipped = candle.IsEquipped = true;

				//// Save the items to the database.
				//server.World.SaveActor(backpack);
				//server.World.SaveActor(weapon);
				//server.World.SaveActor(spell);
				//server.World.SaveActor(shirt);
				//server.World.SaveActor(pants);
				//server.World.SaveActor(candle);

				//// Preset Action_0 to the weapon and Action_1 to the spell.
				//player.Properties.SetValue(String.Concat(Character.ActionPrefix, 0), weapon.ID);
				//player.Properties.SetValue(String.Concat(Character.ActionPrefix, 1), spell.ID);

				//// Set the owner of the bread, cheese and water to be the backpack.
				//bread.Owner = cheese.Owner = water.Owner = backpack;

				//// Save the bread, cheese and water.
				//server.World.SaveActor(bread);
				//server.World.SaveActor(cheese);
				//server.World.SaveActor(water);

				// Perform a save on the player instance.
				//server.World.SaveActor(player);
			}
		}
		private void ResetCharacter(Server server, RdlCommand cmd, IClient client)
		{
			Dictionary<string, string> values = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			Character player = null;
			int id = 0;
			foreach (var item in cmd.Args)
			{
				if (item != null)
				{
					string[] pairs = item.ToString().Split(':');
					if (pairs != null && pairs.Length == 2)
					{
						if (pairs[0].ToLower().Equals("id"))
						{
							Int32.TryParse(pairs[1], out id);
						}
						else
						{
							values.Add(pairs[0], pairs[1]);
						}
					}
				}
			}
			bool success = false;
			string message = String.Empty;
			if (id > 0)
			{
				player = server.World.Provider.GetPlayer(id) as Character;
				if (player != null)
				{
					if (player.Properties.GetValue<bool>("RequiresReset"))
					{
						// Reset player.
						player.Reset();

						foreach (var item in values)
						{
							player.Properties.SetValue(item.Key, item.Value);
						}

						// Reset body and mind.
						AttributeList.SetBodyAndMind(player, server.World);

						player.Save();
						success = true;
					}
					else
					{
						message = "This Character has already been reset.";
					}
				}
				else
				{
					message = "Could not find Character.";
				}
			}
			else
			{
				message = "Could not find Character.";
			}

			client.Context.Add(new RdlCommandResponse(cmd.TypeName, success, message));
		}
		private void Play(Server server, RdlCommand cmd, IClient client)
		{
			// TODO: Combine all commands sent after play into one command.
			string username = server.GetUserNameFromCommand(cmd);

			User user = SecurityManager.GetUser(username);

			// Load the player from the specified ID, username supplied must match
			// the username of the player and connected client.
			var player = server.World.LoadPlayer(cmd.GetArg<int>(0), username, client.Context);

			if (player != null)
			{
				if (!player.Properties.GetValue<bool>("IsFirstTime"))
				{
					(player as Character).CreateStartingItems(server);
					player.Properties.SetValue("IsFirstTime", true);
				}

				// Loop through all items and set the equipment, spells and bags.
				Character character = player as Character;

				character.IsAdmin = Roles.IsUserInRole(user.UserName, RoleNames.God);

				int weaponCount = 0;
				int fingerCount = 0;
				int earCount = 0;
				foreach (var item in player.GetAllChildren().Where(c => c is Item).Select(c => c as Item))
				{
					if (item is ISpell)
					{
						character.Spells.Add((ISpell)item);
					}
					else if (item is Container)
					{
						if (item.IsEquipped)
						{
							character.Bags.Add((Container)item);
						}
						else
						{
							// If the bag has been set in the bag collection then remove it from
							// its current container.
							if (character.Bags.Contains((Container)item))
							{
								item.Owner = character;
								item.IsEquipped = true;
								item.Save();
							}
						}
					}
					else if (item.IsEquipped)
					{
						EquipmentSlot slot = EquipmentSlot.None;
						if (item.EquipLocation == EquipLocation.Weapon)
						{
							if (weaponCount == 0) slot = EquipmentSlot.Weapon1;
							else if (weaponCount == 1) slot = EquipmentSlot.Weapon2;
							weaponCount++;
						}
						else if (item.EquipLocation == EquipLocation.Finger)
						{
							if (fingerCount == 0) slot = EquipmentSlot.Finger1;
							else if (fingerCount == 1) slot = EquipmentSlot.Finger2;
							fingerCount++;
						}
						else if (item.EquipLocation == EquipLocation.Ear)
						{
							if (earCount == 0) slot = EquipmentSlot.Ear1;
							else if (earCount == 1) slot = EquipmentSlot.Ear2;
							earCount++;
						}
						else
						{
							slot = (EquipmentSlot)Enum.Parse(typeof(EquipmentSlot), item.EquipLocation.ToString(), true);
						}
						if (slot != EquipmentSlot.None)
						{
							character.Equipment.Set(slot, item);
						}
					}
				}

				Radiance.AuthKey key = player.GetAuthKey(client.SessionId);

				// Add the player's current zone information.
				MapManager.MapDetail detail = server.World.Map.GetDetail(player.Location);
				if (detail != null)
				{
					character.Zone = detail.Name;
                    if (!detail.IsLoaded)
                    {
                        server.World.Map.GetMap(character.Zone, detail.Key.StartX, detail.Key.StartY, detail.Width, detail.Height);
                    }
				}

				// Send the player and playerAuthKey info down to the client.
				client.Context.Add(new RdlAuthKey(key.ToString(), "PLAYER"));
				client.Context.AddRange(player.ToRdl());
				client.Context.Add(Time.GetTimeProperty());
				client.Context.Add(new RdlProperty(player.ID, "Emblem", user.Tokens));

				// Add the player to the last place they were in.
				player.Place.Enter(player, Direction.Empty);

				// Inventory
				Inventory(server, new RdlCommand("INVENTORY"), client);
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(Resources.PlayerNotFound));
			}
		}
		private void DeleteCharacter(Server server, RdlCommand cmd, IClient client)
		{
			// Args: ID of Character to delete.
			int id = cmd.GetArg<int>(0);
			if (id > 0)
			{
				string username = server.GetUserNameFromCommand(cmd);
				User user = SecurityManager.GetUser(username);
				if (user != null)
				{
					var player = server.World.Provider.GetPlayers(username).Where(p => p.ID == id).FirstOrDefault();
					if (player != null)
					{
						server.World.Provider.DeletePlayer<IPlayer>(player);
					}
				}
			}
		}
		private void Quit(Server server, RdlCommand cmd, IClient client)
		{
			User user = SecurityManager.GetUser(client.Player.UserName);
			server.Quit(client.Player);
			Radiance.AuthKey key = new Radiance.AuthKey(client.SessionId, user.UserName, user.ID);
			client.Context.Add(new RdlAuthKey(key.ToString(), "USER"));
			client.Context.Add(new RdlCommandResponse("QUIT", true, String.Empty));
		}
		#endregion

		#region Admin Commands
		private void AdminCreate(Server server, RdlCommand cmd, IClient client)
		{
			string objName = cmd.GetArg<string>(0);

			Actor obj = null;

			if (String.IsNullOrEmpty(objName)) objName = "actor";
			if (objName.ToLower().Equals("place"))
			{
				RdlPlace placeTag = cmd.Group.Tags.GetPlaces().FirstOrDefault();
				obj = Activator.CreateInstance(Type.GetType(placeTag.Properties.GetValue<string>("RuntimeType")), placeTag) as Place;
				server.World.Places.Add((obj as Place).Location, obj as Place);
			}
			else
			{
				obj = new Actor(cmd.Group.Tags.GetActors().FirstOrDefault());
			}

			if (obj != null)
			{
				server.World.SaveActor(obj);
				client.Context.Add(new RdlActor(obj.ID, obj.Name));
				client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Admin, Resources.AdminActorSaved));
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(Resources.ActorNotCreated));
			}
		}
		private void AdminAddActor(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// TemplateName
			// X
			// Y
			// Z
			string templateName = cmd.GetArg<string>(0);
			int x = cmd.GetArg<int>(1);
			int y = cmd.GetArg<int>(2);
			int z = cmd.GetArg<int>(3);
			IActor actor = server.World.CreateFromTemplate<IActor>(templateName);
			if (actor != null)
			{
				Place place = server.World.FindPlace(new Point3(x, y, z));
				if (place.ID > 0)
				{
					if (actor.ObjectType == ObjectType.Mobile)
					{
						actor.Properties.SetValue(Avatar.XProperty, x);
						actor.Properties.SetValue(Avatar.YProperty, y);
						actor.Properties.SetValue(Avatar.ZProperty, z);
						PerenthiaMobile mob = actor as PerenthiaMobile;
						if (mob != null) mob.GenerateStats();
					}
					place.Children.Add(actor);
					server.World.SaveActor(actor);

					client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Neutral,
						String.Format("{0} saved successfully!", actor.Alias())));

					client.Context.AddRange(place.ToRdl());
					foreach (var a in place.Children)
					{
						client.Context.AddRange(a.ToRdl());
					}
				}
			}
		}
		private void AdminSave(Server server, RdlCommand cmd, IClient client)
		{
			var actor = server.World.GetActor(cmd.GetArg<int>(0));
			if (actor != null)
			{
				var tag = cmd.Group.Tags.GetActors().FirstOrDefault();
				if (tag != null)
				{
					actor.Name = tag.Name;

					actor.Properties.SetValues(cmd.Group.Tags.GetProperties(actor.ID));
					server.World.SaveActor(actor);
				}
				else
				{
					client.Context.Add(new RdlErrorMessage(Resources.ActorNotSaved));
				}
			}
			else
			{
				client.Context.Add(new RdlErrorMessage(String.Format(Resources.ActorNotFound, cmd.GetArg<int>(0))));
			}
		}
		private void AdminBuild(Server server, RdlCommand cmd, IClient client)
		{
			client.Player.IsBuilder = true;
			client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Admin, Resources.AdminEnterBuildMode));
		}
		private void AdminMap(Server server, RdlCommand cmd, IClient client)
		{
			this.Map(server, cmd, client);
		}
		private void AdminTerrain(Server server, RdlCommand cmd, IClient client)
		{
			this.SysTerrain(server, cmd, client);
		}
		private void AdminGetItemTemplates(Server server, RdlCommand cmd, IClient client)
		{
			List<IActor> actors = new List<IActor>(server.World.Provider.GetTemplates(typeof(Item)));
			foreach (var actor in actors)
			{
				client.Context.AddRange(actor.ToRdl());
			}
		}
		private void AdminGetCreatureTemplates(Server server, RdlCommand cmd, IClient client)
		{
			List<IActor> actors = new List<IActor>(server.World.Provider.GetTemplates(typeof(Creature)));
			foreach (var actor in actors)
			{
				client.Context.AddRange(actor.ToRdl());
			}
		}
		private void AdminGetNpcTemplates(Server server, RdlCommand cmd, IClient client)
		{
			List<IActor> actors = new List<IActor>(server.World.Provider.GetTemplates(typeof(Npc)));
			foreach (var actor in actors)
			{
				client.Context.AddRange(actor.ToRdl());	
			}
		}
		private void AdminGetQuestTemplates(Server server, RdlCommand cmd, IClient client)
		{
			List<IActor> actors = new List<IActor>(server.World.Provider.GetTemplates(typeof(IQuest)));
			foreach (var actor in actors)
			{
				client.Context.AddRange(actor.ToRdl());
			}
		}
		private void AdminGetMaps(Server server, RdlCommand cmd, IClient client)
		{
			foreach (var detail in server.World.Map.MapDetails.Values)
			{
				RdlTag tag = new RdlTag("MAP", "MAP");
				tag.Args.Add(detail.Name);
				tag.Args.Add(detail.Width);
				tag.Args.Add(detail.Height);
				client.Context.Add(tag);
			}
		}
		private void AdminGetPlaceTypes(Server server, RdlCommand cmd, IClient client)
		{
			var types = server.World.Provider.GetKnownTypes(ObjectType.Place);
			foreach (var t in types)
			{
				RdlTag tag = new RdlTag("PLACETYPE", "PLACETYPE");
				tag.Args.Add(t.FullName);
				client.Context.Add(tag);
			}
		}
		private void AdminLook(Server server, RdlCommand cmd, IClient client)
		{
			client.Context.AddRange(client.Player.Place.ToRdl());
			foreach (var actor in client.Player.Place.Children)
			{
				client.Context.AddRange(actor.ToRdl());
			}
		}
		private void AdminMove(Server server, RdlCommand cmd, IClient client)
		{
			Direction dir = Direction.FromName(cmd.GetArg<string>(0));
			Point3 loc = client.Player.Location;
			loc += dir.Value;
			Place place = server.World.FindPlace(loc);

			// Verify exits, only for existing places.
			if (place.ID > 0)
			{
				Place prevPlace = server.World.FindPlace(client.Player.Location);
				if (prevPlace.ID > 0)
				{
					if (!place.Exits.GetValue(dir.CounterDirection.KnownDirection))
					{
						place.Exits.SetValue(dir.CounterDirection.KnownDirection, true);
						server.World.SaveActor(place);
					}
					if (!prevPlace.Exits.GetValue(dir.KnownDirection))
					{
						prevPlace.Exits.SetValue(dir.KnownDirection, true);
						server.World.SaveActor(prevPlace);
					}
				}
			}
			
			int lastZ = client.Player.Z;

			// Enter the new place.
			place.Enter(client.Player, dir);

			// If the player moved in a z direction send the map for that z-index.
            //if (client.Player.Z != lastZ)
            //{
            //    client.Context.AddRange(server.World.GetRdlMap(client.Player.X, client.Player.Y, client.Player.Z));
            //}
			client.Context.AddRange(place.ToRdl());
			foreach (var actor in place.Children)
			{
				client.Context.AddRange(actor.ToRdl());
			}
		}
		private void AdminDropItem(Server server, RdlCommand cmd, IClient client)
		{
			// Args: 
			// 0 = Item Template Name
			// 1 = Target Instance ID
			// 2 - n = Additional Args
			Item item = server.World.CreateFromTemplate<Item>(cmd.GetArg<string>(0));
			IActor actor = server.World.GetActor(cmd.GetArg<string>(1), null, true, false);
			if (item != null && actor != null)
			{
				if (item is IQuest)
				{
					IQuest tempQuest = server.World.Provider.GetTemplate(typeof(IQuest), item.Name) as IQuest;
					if (tempQuest != null)
					{
						// Requires Arg0
						string type = cmd.GetArg<string>(2);
						if (String.IsNullOrEmpty(type)) type = "StartsWith";

						// StartsWith and EndsWith are added to the template, not the instance.
						if (type == "StartsWith")
						{
							tempQuest.AddStartsWith(actor);
							client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
								String.Format("{0} added as a quest that starts with {1}.", item.AUpper(), actor.A())));
						}
						else
						{
							tempQuest.AddEndsWith(actor);
							client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
								String.Format("{0} added as a quest that ends with {1}.", item.AUpper(), actor.A())));
						}
						server.World.SaveActor(tempQuest);
						if (actor is IMobile)
						{
							(actor as IMobile).IsQuestsLoaded = false;
						}
					}
				}
				else
				{
					if (actor is IMerchant)
					{
						(actor as IMerchant).AddMerchandise(item);
						server.World.SaveActor(actor);
						client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
							String.Format("{0} added as merchandise for {1}.", item.AUpper(), actor.A())));
					}
					else
					{
						if (item.TemplateID == 0) item = item.Clone() as Item;
						server.World.SaveActor(item);
						actor.Children.Add(item);
						server.World.SaveActor(actor);

						client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
							String.Format("{0} added to the inventory of {1}.", item.AUpper(), actor.A())));
					}
				}
			}
		}
		#endregion

		#region Household Commands
		private void HouseholdCreate(Server server, RdlCommand cmd, IClient client)
		{
			// Create a new household.
			// Must not already belong to or own a household.

			// Args:
			// 0 = Household Name
			// 1 = Requires Approval
			string name = cmd.GetArg<string>(0);
			bool requiresApproval = cmd.GetArg<bool>(1);

			if (!String.IsNullOrEmpty(name))
			{
				// Ensure the current player is not already a household member.
				if (client.Player.Household != null && client.Player.Household.HouseholdID > 0)
				{
					client.Context.Add(new RdlErrorMessage(
						String.Format("You are already a member of '{0}'. You must leave your current Household in order to create a new Household.",
						client.Player.Household.HouseholdName)));
				}
				else
				{
					// TODO: Creating a household is free at the moment, should charge for it later on.
					
					// Check to ensure a household does not already exist.
					var household = server.World.Households.GetHousehold(name);
					if (household != null)
					{
						client.Context.Add(new RdlErrorMessage(
							String.Format("A household already exists with the name '{0}'.", household.Name)));
					}
					else
					{
						// Create the new household and assign the owner the head of household rank.
						household = new Household() { Name = name, MembershipRequiresApproval = requiresApproval };
						server.World.Provider.SaveHousehold(household);

						var rank = server.World.Provider.GetHeadOfHouseholdRank(1);

						client.Player.Household = HouseholdInfo.GetHouseholdInfo(client.Player.Gender, household, rank);

						// Send down the household information for the player.
						client.Context.AddRange(client.Player.Household.ToRdlProperties(client.Player));

						client.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive, 
							String.Format("You have successfully created a new Household named '{0}'. You have been granted the title of '{1}' as Head of Household and may create custom titles for your members.",
							household.Name, rank.GetTitle(client.Player.Gender))));
					}
				}
			}
			else
			{
				client.Context.Add(new RdlErrorMessage("A Household Name must be supplied in order to create a new Household."));
				this.ShowHelp(server, client, "HOUSEHOLDCREATE");
			}
		}
		private void HouseholdDelete(Server server, RdlCommand cmd, IClient client)
		{
			// Deletes the household.
			// Must not have any members associated with the household.

			// Args:
			// 0 = Household Name
		}
		private void HouseholdView(Server server, RdlCommand cmd, IClient client)
		{
			// Views the household details, including the memebers, armory, ranks, honors, relations and status updates.

			// Args:
			// 0 = Household Name
			string name = cmd.GetArg<string>(0);

			if (!String.IsNullOrEmpty(name))
			{
				// Household
				var household = server.World.Households.GetHousehold(name);
				if (household != null)
				{
					// Send down:
					// Household
					// Ranks
					// Relations
					// Members (1, 10)
					// Armory (1, 10)
				}
				else
				{
					client.Context.Add(new RdlErrorMessage(String.Format("A Household with the name of '{0}' could not be found.", name)));
				}
			}
			else
			{
				client.Context.Add(new RdlErrorMessage("The name of the Household was not specified."));
			}
		}
		private void HouseholdImage(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Household ID
			// 1 = Image Bytes
		}
		private void HouseholdMotto(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Household ID
			// 1 = Motto
		}
		private void HouseholdDesc(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Household ID
			// 1 = Description
		}
		private void HouseholdMembers(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Household ID
			// 1 = StartRowIndex
			// 2 = MaxRows
		}
		private void HouseholdAddMember(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Household ID
			// 1 = MemberID
		}
		private void HouseholdRemoveMember(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Household ID
			// 1 = MemberID
		}
		private void HouseholdAdvanceMember(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Household ID
			// 1 = MemberID
			// 2 = RankOrder
		}
		private void HouseholdStatus(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Household ID
		}
		private void HouseholdUpdateStatus(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Household ID
			// 1 = Status
		}
		private void HouseholdSaveRank(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Household ID
			// 1 = RankName
			// 2 = RankOrder
			// 3 = Permissions
			// 4 = TitleMale
			// 5 = TitleFemale
			// 6 = Image Bytes
		}
		private void HouseholdRemoveRank(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Household ID
			// 1 = RankName
		}
		private void HouseholdSaveHonor(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Household ID
			// 1 = HonorName
		}
		private void HouseholdRemoveHonor(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Household ID
			// 1 = HonorName
		}
		private void HouseholdArmory(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Household ID
			// 1 = StartRowIndex
			// 2 = MaxRows
		}
		private void HouseholdAddItem(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Household ID
			// 1 = ItemID
		}
		private void HouseholdRemoveItem(Server server, RdlCommand cmd, IClient client)
		{
			// Args:
			// 0 = Household ID
			// 1 = ItemID
		}
		#endregion

		#region Helper Methods
		private bool EnsureParameters(Server server, RdlCommand cmd, IClient client)
		{
			if (server.World.Commands.ContainsKey(cmd.TypeName))
			{
				CommandInfo info = server.World.Commands[cmd.TypeName];

				if (cmd.Args.Count < info.GetArgumentsCount())
				{
					client.Context.Add(new RdlErrorMessage(String.Format(Resources.CmdSyntax,
						info.CommandName, info.Syntax)));
					return false;
				}
				return true;
			}
			return false;
		}
		#endregion
	}
}
