using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

using Lionsguard;

using Radiance;
using Radiance.Markup;

using Perenthia.Dialogs;
using Perenthia.Controls;
using Perenthia.Windows;
using System.Threading;
using Perenthia.Models;

namespace Perenthia.Screens
{
	public partial class PlayScreen : UserControl, IScreen
	{
		private int _playerId = 0;
		private int _targetId = 0;

		private bool _isCommandKeyDown = false;
        private bool _hasRequiredQuota = false;
		private bool _loadComplete = false;

		private object _uiSettingsLock = new object();

		private UISettings _uiSettings = new UISettings();

		private GodModeWindow _diagGodMode;

		public PlayScreen()
		{
			this.Loaded += new RoutedEventHandler(PlayScreen_Loaded);
			InitializeComponent();
		}
		public PlayScreen(int playerId)
			: this()
		{
			_playerId = playerId;
		}

		private void PlayScreen_Loaded(object sender, RoutedEventArgs e)
		{
			if (DesignerProperties.GetIsInDesignMode(this))
				return;

			Game.Player = null;
			Game.Target = null;

			// Key Event Handlers
			App.Current.RootVisual.KeyDown += new KeyEventHandler(RootVisual_KeyDown);

			Game.DragDropManager = new DragDropManager(this.LayoutRoot);

			PopupManager.Init(this.LayoutRoot);

			QuantityPopupManager.Init(this.LayoutRoot);

			ServerManager.Instance.Reset();
			ServerManager.Instance.Response += new ServerResponseEventHandler(OnServerResponse);

			ItemSlot.SlotClick += new ItemSlotEventHandler(ItemSlot_Click);

			// Initialize the SlotManager.
			SlotManager.Instance.Attach(this.LayoutRoot);
			SlotManager.Instance.AddSearchElements(diagBag0Content);
			SlotManager.Instance.AddSearchElements(diagBag1Content);
			SlotManager.Instance.AddSearchElements(diagBag2Content);
			SlotManager.Instance.AddSearchElements(diagBag3Content);
			SlotManager.Instance.AddSearchElements(diagBag4Content);
			SlotManager.Instance.AddSearchElements(diagSpellbookContent);
			SlotManager.Instance.AddSearchElements(diagCharacterSheetContent);
			SlotManager.Instance.AddSearchElements(ActionsContainer);
            SlotManager.Instance.AddSearchElements(BagsContainer);
            SlotManager.Instance.AddSearchElements(ActionDropContainer);
			SlotManager.Instance.SlotItemChange += new SlotItemChangeEventHandler(SlotManagerSlotItemChange);
			SlotManager.Instance.SlotItemChanging += new SlotItemChangingEventHandler(SlotManagerSlotItemChanging);
			SlotManager.Instance.SlotClick += new RoutedEventHandler(SlotManagerSlotClick);
			SlotManager.Instance.SlotUse += new EventHandler(SlotManagerSlotUse);
			SlotManager.Instance.SlotDrop += new EventHandler(SlotManagerSlotDrop);

			// Set up events for controls.
			diagTellContent.Action += new ActionEventHandler(diagTellContent_Action);
			diagCommerceContent.Action += new ActionEventHandler(OnIActionWindowAction);
			diagInventoryContent.Action += new ActionEventHandler(OnIActionWindowAction);
			diagQuestsContent.Action += new ActionEventHandler(OnIActionWindowAction);
			ctlMap.TileNotFound += new EventHandler(ctlMap_TileNotFound);
			ctlMap.ActiveTileChanged += new EventHandler(ctlMap_ActiveTileChanged);
			ctlChat.Action += ctlRoom_Action;

			// Initialize child controls.
			diagMapContent.SetViewPort(new RectangleGeometry { Rect = new Rect(0, 0, 400, 400) });

			// Initialize control properties.
			ctlPlayer.EnableAffects = true;
			ctlTarget.EnableDetails = true;
			this.ActivateTab(TextPanelTab.Chat);

            _hasRequiredQuota = StorageManager.HasRequiredQuota();

			// Initialize Window IDs
			ctlPlayer.WindowID = UISettings.PlayerWindowID;
			ctlChat.WindowID = UISettings.ChatWindowID;
			ctlMap.WindowID = UISettings.MapWindowID;

			// Load the player instance.
			this.StepLoadProgress("Loading player object...", 10);
			ServerManager.Instance.SendUserCommand("PLAY", new ServerResponseEventHandler(this.OnPlayCommandResponse), _playerId);

			//// Load settings based on role.
			//if (Settings.Role.ToLower().Equals("god"))
			//{
			//    ctlSlotBuild.Visibility = Visibility.Visible;
			//}

			this.Focus();
		}

		private void ItemSlot_Click(ItemSlotEventArgs e)
		{
			if (e.Slot != null && e.Slot.Item != null)
			{
				ServerManager.Instance.SendCommand(new RdlCommand("USE", e.Slot.Item.ID));
			}
		}

		#region Key Events

		private void RootVisual_KeyDown(object sender, KeyEventArgs e)
		{
			if (!Game.ProcessInput)
				return;


			if (diagTellContent.IsKeyDown)
			{
				diagTellContent.IsKeyDown = false;
				return;
			}
			if (_isCommandKeyDown)
			{
				_isCommandKeyDown = false;
				return;
			}

			if (!e.Handled)
			{
				Logger.LogDebug("Key Down: {0}", e.Key);

				// <cr>- move focus to the command line
				if (e.Key == Key.Enter)
				{
					ctlChat.SetCommandLine("/");
					return;
				}

				// MOVEMENT
				string dir = String.Empty;
				if (e.Key == Key.W) dir = "North";
				if (e.Key == Key.S) dir = "South";
				if (e.Key == Key.A) dir = "West";
				if (e.Key == Key.D) dir = "East";
				if (e.Key == Key.Q) dir = "Northwest";
				if (e.Key == Key.E) dir = "Northeast";
				if (e.Key == Key.Z) dir = "Southwest";
				if (e.Key == Key.X) dir = "Southeast";
				if (e.Key == Key.Up) dir = "Up";
				if (e.Key == Key.Down) dir = "Down";

				if (!String.IsNullOrEmpty(dir))
				{
					Logger.LogDebug("MOVE {0}", dir);
					this.ctlMap_DirectionClick(new DirectionEventArgs() { Direction = dir });
				}

				// ACTIONS
				// c - Character Sheet
				if (e.Key == Key.P || e.Key == Key.C)
				{
					ctlPlayer_Click(ctlPlayer, new RoutedEventArgs());
				}
				// b - Backpack0
				if (e.Key == Key.B)
				{
					ctlSlotBag0_Click(new ItemSlotEventArgs(ctlSlotBag0));
				}
				// v - Spellbook
				if (e.Key == Key.V)
				{
					ctlSlotSpells_Click(new ItemSlotEventArgs(ctlSlotSpells));
				}
				// m - Map
				if (e.Key == Key.M)
				{
					ctlSlotMap_Click(new ItemSlotEventArgs(ctlSlotMap));
				}

				// Action Keys
				if (e.Key == Key.D1)
				{
					ctlSlot0.Use();
				}
				if (e.Key == Key.D2)
				{
					ctlSlot1.Use();
				}
				if (e.Key == Key.D3)
				{
					ctlSlot2.Use();
				}
				if (e.Key == Key.D4)
				{
					ctlSlot3.Use();
				}
				if (e.Key == Key.D5)
				{
					ctlSlot4.Use();
				}
				if (e.Key == Key.D6)
				{
					ctlSlot5.Use();
				}
				if (e.Key == Key.D7)
				{
					ctlSlot6.Use();
				}
				if (e.Key == Key.D8)
				{
					ctlSlot7.Use();
				}
				if (e.Key == Key.D9)
				{
					ctlSlot8.Use();
				}
				if (e.Key == Key.D0)
				{
					ctlSlot9.Use();
				}
			}
		}

		public new bool Focus()
		{
			this.IsTabStop = true;
			return base.Focus();
		}
		#endregion

		#region Slot Event Handlers
		private void SlotManagerSlotDrop(object sender, EventArgs e)
		{
			Slot slot = sender as Slot;
			if (slot != null && slot.Item != null)
			{
				RdlActor item = slot.Item.Item as RdlActor;
				if (item != null)
				{
					ServerManager.Instance.SendCommand(new RdlCommand("DROP", item.ID));
					slot.Item = null;
				}
			}
		}

		private void SlotManagerSlotUse(object sender, EventArgs e)
		{
			// All Slot USE events will be handled by this method.
			Slot slot = sender as Slot;
			if (slot != null)
			{
				if (slot.Item != null)
				{
					RdlActor item = slot.Item.Item as RdlActor;
					if (item != null)
					{
						// When using an item a command must be sent to the server (USE <ID>).
						// If successful the server will return the item being used, minus 1. If the item comes
						// back as a quantity of 0 then remove it from the inventory of the player.
						//this.Write(MessageType.System, String.Format("You attempt to use {0}.", item.Name.A(false)));
						ServerManager.Instance.SendCommand(new RdlCommand("USE", item.ID));
					}
				}
			}
		}
		private void Slot_Click(object sender, RoutedEventArgs e)
		{
			SlotManagerSlotUse(sender, e);
		}

		private void SlotManagerSlotClick(object sender, RoutedEventArgs e)
		{
		}
		private void SlotManagerSlotItemChange(SlotItemChangeEventArgs e)
		{
			if (e.SourceValue == null) return;

			RdlActor item = e.SourceValue.Item as RdlActor;

			// Different destinations will determine whether or not the item is dropped into the new container.
			if (e.DestinationElement is BagDialog)
			{
				RdlActor bag = (e.DestinationElement as BagDialog).Item;
				if (bag != null)
				{
					string slotTag = e.DestinationSlot.Tag as String; // EquipmentSlot
					if (String.IsNullOrEmpty(slotTag)) slotTag = "None";
					EquipmentSlot slot = (EquipmentSlot)Enum.Parse(typeof(EquipmentSlot), slotTag, true);
					// If unequipping an item, need to send the unequip command.
					if (item.Properties.GetValue<bool>("IsEquipped"))
					{
						item.OwnerID = bag.ID;
						item.Properties.SetValue("IsEquipped", false);
						ServerManager.Instance.SendCommand(new RdlCommand("UNEQUIP", item.ID, slot, bag.ID));
					}
					else
					{
						// Might just be moving an item to a bag.
						ServerManager.Instance.SendCommand(new RdlCommand("GIVE", item.ID, bag.ID, 1));
					}
				}
				else
				{
					// No bag present, do not move the item.
					e.Handled = true;
				}
			}
			else if (e.DestinationElement is SpellbookDialog)
			{
				// If the destination item is not of equipLocation spell then it can not be dropped here.
				if (!item.Properties.GetValue<string>("EquipLocation").Equals(EquipLocation.Spell.ToString(), StringComparison.InvariantCultureIgnoreCase))
				{
					// Can not drop this item in the spellbook, it is not a spell.
					MessageBox.Show("You can not drop this item here, it is not a Spell!", "Invalid Spellbook Item", MessageBoxButton.OK);
					e.Handled = true;
				}
				else
				{
					item.Properties.SetValue("IsEquipped", true);
					ServerManager.Instance.SendCommand(new RdlCommand("EQUIP", item.ID, EquipmentSlot.None));
				}
			}
			else if (e.DestinationElement is CharacterSheetDialog)
			{
				// Need to ensure that the destination slot has the same equip location as the destination item.
				string slotTag = e.DestinationSlot.Tag as String; // EquipmentSlot
				if (String.IsNullOrEmpty(slotTag)) slotTag = "None";
				EquipmentSlot destSlotLoc = (EquipmentSlot)Enum.Parse(typeof(EquipmentSlot), slotTag, true);
				EquipLocation destItemLoc = (EquipLocation)Enum.Parse(typeof(EquipLocation), item.Properties.GetValue<string>("EquipLocation"), true);

				//if (destSlotLoc == destItemLoc)
				//{
					//item.OwnerID = Game.Player.ID;
					//item.Properties.SetValue("IsEquipped", true);
					//Game.Player.Properties.SetValue(String.Concat("Equipment_", destSlotLoc), item.ID);
					ServerManager.Instance.SendCommand(new RdlCommand("EQUIP", item.ID, destSlotLoc));
				//}
				//else
				//{
				//    // Can not equip this item in the destination slot.
				//    MessageBox.Show("You can not equip this item here.", "Invalid Equip Location", MessageBoxButton.OK);
				//    e.Handled = true;
				//}
			}
			else if (e.DestinationElement is Canvas)
			{
				if ((e.DestinationElement as Canvas).Name.Equals(ActionsContainer.Name))
				{
					string slotTag = e.DestinationSlot.Tag as String; // Action Slot Number
					if (!String.IsNullOrEmpty(slotTag))
					{
						// Set the current item into the specified action slot for this player but leave the item in the slot
						// it was dragged from.
						Game.Player.Properties.SetValue(String.Concat("Action_", slotTag), item.ID);
						ServerManager.Instance.SendCommand(new RdlCommand("SETACTION", item.ID, slotTag));

						// Set the dragged item into the specified slot.
						SlotHelper.SetSlotItem(e.DestinationSlot, item);

						// Set handled to true to prevent the item from bein removed from its original container.
						e.Handled = true;
					}
					else
					{
						// Can not drop this item into this slot.
						MessageBox.Show("You can not set this item as an action.", "Invalid Action Item", MessageBoxButton.OK);
						e.Handled = true;
					}
                }
                else if ((e.DestinationElement as Canvas).Name.Equals(ActionDropContainer.Name))
                {
                    string slotTag = e.DestinationSlot.Tag as String; // Action Slot Number
                    int slotNumber;
                    if (Int32.TryParse(slotTag, out slotNumber))
                    {
                        if (slotNumber > -1)
                        {
                            // Clear the specified slot.
                            Game.Player.Properties.SetValue(String.Concat("Action_", slotNumber), 0);
                            ServerManager.Instance.SendCommand(new RdlCommand("SETACTION", 0, slotNumber));
                            return;
                        }
                    }

                    // Can not drop this item here.
                    e.Handled = true;
                }
				else if ((e.DestinationElement as Canvas).Name.Equals(BagsContainer.Name))
				{
					string slotTag = e.SourceSlot.Tag as String; // EquipmentSlot
					if (String.IsNullOrEmpty(slotTag)) slotTag = "None";
					EquipmentSlot equipSlot = equipSlot = (EquipmentSlot)Enum.Parse(typeof(EquipmentSlot), slotTag, true);

					if (item.Properties.GetValue<string>("EquipLocation").Equals("Bag"))
					{
						// If the destiantion slot is already occupied then give a warning message.
						if (e.DestinationSlot.Item != null)
						{
							// Can not drop this item into this slot.
							MessageBox.Show("You can not set equip a bag over another bag, the existing bag must be unequipped.", "Invalid Action Item", MessageBoxButton.OK);
							e.Handled = true;
						}
						else
						{
							ServerManager.Instance.SendCommand("EQUIP", item.ID, EquipmentSlot.None);

							// Set the dragged item into the specified slot.
							SlotHelper.SetSlotItem(e.DestinationSlot, item);
						}
					}
					else
					{
						// Can add the item to bag, if a bag already exists on the destination slot.
						if (e.DestinationSlot.Item != null)
						{
							RdlActor destItem = e.DestinationSlot.Item.Item as RdlActor;
							if (destItem != null)
							{
								ServerManager.Instance.SendCommand("GIVE", item.ID, destItem.ID);
								// Do not drop the actual image into the bag slot.
								e.Handled = true;

								// Remove the item from the source slot.
								e.SourceSlot.Item = null;
							}
							else
							{
								// Can not drop this item into this slot.
								e.Handled = true;
							}
						}
						else
						{
							// Can not drop this item into this slot.
							e.Handled = true;
						}
					}
				}
			}
		}

		private void SlotManagerSlotItemChanging(SlotItemChangingEventArgs e)
		{
		}
		#endregion

		#region Chat Panel
		private void ctlChat_ChatInputReceived(Perenthia.Windows.ChatInputReceivedEventArgs e)
		{
			RdlCommandParserErrorType errType = RdlCommandParserErrorType.None;
			RdlCommand cmd;
			if (RdlCommand.TryParse(e.Text, out cmd, out errType))
			{
				// Only echo say, shout, tell and emotes.
				this.EchoInput(cmd);

				ServerManager.Instance.SendCommand(cmd);
			}
			else
			{
				switch (errType)
				{
					case RdlCommandParserErrorType.NoTargetForTell:
						this.Write(TextType.Error, "TELL requires the name of the person you wish to send a message.");
						break;
					case RdlCommandParserErrorType.NoArgumentsSpecified:
						this.Write(TextType.Error, String.Format("No arguments were specified for the {0} command.", cmd.TypeName));
						break;
					case RdlCommandParserErrorType.InvalidNumberOfArguments:
						this.Write(TextType.Error, String.Format("An invalid number of arguments were specified for the {0} command.", cmd.TypeName));
						break;
				}
			}
		}
		private void OnChatTellLinkClick(object sender, RoutedEventArgs e)
		{
			HyperlinkButton lnk = sender as HyperlinkButton;
			if (lnk != null && lnk.Tag != null)
			{
				// Tag is the Alias of the tell recipient.
				ctlRoom_Action(this, new ActionEventArgs(Actions.Tell, lnk.Tag, lnk.Tag.ToString()));
			}
		}
		private void EchoInput(RdlCommand cmd)
		{
			switch (cmd.TypeName)
			{
				case "SAY":
					if (!String.IsNullOrEmpty(cmd.Text))
					{
						this.Write(TextType.Say, String.Concat("You say: ", cmd.Text));
					}
					break;
				case "SHOUT":
					if (!String.IsNullOrEmpty(cmd.Text))
					{
						this.Write(TextType.Say, String.Concat("You shout: ", cmd.Text.ToUpper()));
					}
					break;
				case "TELL":
				case "REPLY":
					string msg = cmd.GetArg<string>(1);
					if (!String.IsNullOrEmpty(msg))
					{
						this.Write(TextType.Tell, String.Format("You tell {0}: {1}", cmd.Args[0], msg));
					}
					break;
				case "EMOTE":
					// TODO: Echo Emotes...
					break;
			}
		}
		private void Write(TextType type, string text)
		{
			switch (type)
			{
				case TextType.System:
				case TextType.Error:
				case TextType.PlaceDesc:
				case TextType.PlaceName:
				case TextType.PlaceExits:
				case TextType.PlaceActors:
				case TextType.PlaceAvatars:
				case TextType.Positive:
				case TextType.Negative:
				case TextType.Help:
				case TextType.Level:
				case TextType.Cast:
				case TextType.Melee:
				case TextType.Award:
					ctlChat.AppendGeneral(type, text, null, null);
					this.TabContentAdded(TextPanelTab.Actions);
                    break;
				case TextType.Say:
				case TextType.Shout:
				case TextType.Emote:
				case TextType.Tell:
					string who = String.Empty;
					if (type == TextType.Tell)
					{
						who = text.Split(' ')[0];
						Game.LastTellReceivedFrom = who;
					}
					ctlChat.AppendGeneral(type, text, who, OnTellLinkClick);
					ctlChat.AppendChat(type, text, who, OnTellLinkClick);
					if (type == TextType.Tell)
					{
						ctlChat.AppendTells(type, text, who, OnTellLinkClick);
					}
					this.TabContentAdded(TextPanelTab.Chat);
					break;
				default:
					ctlChat.AppendGeneral(TextType.Say, text, null, null);
					this.TabContentAdded(TextPanelTab.Chat);
					break;
			}
		}
		private void OnTellLinkClick(object sender, RoutedEventArgs e)
		{
			var lnk = sender as HyperlinkButton;
			if (lnk != null && lnk.Tag != null)
			{
				// Set the command line with the command format for the reply.
				ctlChat.SetCommandLine(String.Format("/REPLY {0} ", lnk.Tag));
			}
		}
		#endregion

		#region Server Event Handlers
		private void OnServerResponse(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.ProcessTags(e.Tags));
		}

		private void OnPlayCommandResponse(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.ProcessPlayResponseTags(e.Tags));
		}

		private void OnInventoryCommandResponse(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.ProcessInventoryResponseTags(e.Tags));
		}

		private void OnMapCommandResponse(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.ProcessMapResponseTags(e.Tags));
		}

		private void OnQuitCommandResponse(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.ProcessQuitResponseTags(e.Tags));
		}
		#endregion

		#region Process Server Responses
		private void ProcessPlayResponseTags(RdlTagCollection tags)
		{
			// Player AuthKey
			RdlAuthKey key = tags.Where(t => t.TagName == "AUTH").FirstOrDefault() as RdlAuthKey;
			if (key != null)
			{
				// The player auth key will come down in a user tag.
				Settings.PlayerAuthKey = key.Key;
			}

			// Player
			var player = tags.GetObjects<RdlPlayer>().Where(p => p.ID == _playerId).FirstOrDefault();
			if (player != null)
			{
				Game.Player = new Avatar(player);
				ctlPlayer.DataContext = Game.Player;
				ctlSlotCharacterSheet.Source = Asset.GetImageSource(String.Format(
					   Asset.AVATAR_FORMAT,
					   Game.Player.Race.Name,
					   Game.Player.Gender));
                StorageManager.SavePlayer(player);
				ctlChat.SetPrompt();
			}

			// Load the player inventory.
			this.StepLoadProgress("Loading player inventory...", 10);
			ProcessInventoryResponseTags(tags);
			//ServerManager.Instance.SendCommand("INVENTORY", new ServerResponseEventHandler(this.OnInventoryCommandResponse));
		}

		private void ProcessInventoryResponseTags(RdlTagCollection tags)
		{
			this.StepLoadProgress("Loading map objects...", 10);

			// Inventory
			if (Game.Player != null)
			{
                if (_hasRequiredQuota)
                {
					if (!String.IsNullOrEmpty(Game.Player.Zone) && !StorageManager.RequiresFileUpdate(Game.Player.Zone))
                    {
						RdlTagCollection mapTags = StorageManager.ReadMap(Game.Player.Zone);
                        if (mapTags != null && mapTags.Count > 0)
                        {
							Logger.LogDebug("Loading map from disk...");
                            this.ProcessMapResponseTags(mapTags);
                            return;
                        }
                    }
                }

				//// Kick off the process of downloading maps.
				//if (_hasRequiredQuota)
				//{
				//    Logger.LogDebug("Starting map doanloads...");
				//    MapManager.BeginMapDownload(Game.Player.Zone, false, null, null, null);
				//}

				// Load the map for the current player.
				Logger.LogDebug("Loading map from server...");
				ServerManager.Instance.SendCommand("MAP",
					new ServerResponseEventHandler(this.OnMapCommandResponse),
					Game.Player.Zone);

				this.ProcessTags(tags);
			}
		}

		private void ProcessMapResponseTags(RdlTagCollection tags)
		{
			// ** NOTE: Do not write out any messages here since MAP sends down a map loaded message, no need to display that.
			ctlMap.LoadMap(tags);
			ctlMap.HideLoading();
			ctlMap.SetView(new Point3(Game.Player.X, Game.Player.Y, Game.Player.Z));

			//// Kick off the process of downloading maps.
			//if (_hasRequiredQuota)
			//{
			//    Logger.LogDebug("Starting map doanloads...");
			//    MapManager.BeginMapDownload(Game.Player.Zone, false, null, null, null);
			//}

			this.LoadUISettings();

			this.CompleteLoading();
		}

		private void CompleteLoading()
		{
			// Start the heartbeat timer.
			ServerManager.Instance.StartHeartbeat();

			// Close the splash screen so the user can play.
			this.CloseSplash();

			// Reset character and target to load inventory images.
			this.SetCharacterDetails();
			this.SetTargetDetails();

			if (Settings.Role.ToUpper() == "GOD")
			{
				_diagGodMode = new GodModeWindow();
				_diagGodMode.ParentLayoutRoot = this.LayoutRoot;
				_diagGodMode.Show();
			}

			_loadComplete = true;
		}

		private void ProcessDownloadAssetsResponse(byte[] data)
		{
			//ImageManager.Load(data);
		}

		private void ProcessQuitResponseTags(RdlTagCollection tags)
		{
			ServerManager.Instance.Reset();
			ScreenManager.SetScreen(new HomeScreen());			
		}

		private void ProcessTags(RdlTagCollection tags)
		{
			//=====================================================================================
			// MAIN GAME LOOP
			//=====================================================================================

			// AuthKey tags
			this.UpdateAuthKeys(tags.Where(t => t.TagName == "AUTH").Select(t => t as RdlAuthKey));

			// Command Response Tags
			this.HandleCommandResponse(tags.GetCommandResponse());

			// The Server can send Commands to the client, handle those first.
			this.HandleCommands(tags.GetCommands());

			// Update Player
			this.UpdatePlayer(tags);

			// Update Target
			this.UpdateTarget(tags);

			// Update Map
			this.UpdateMap(tags);

			// Update Room
			this.UpdateRoom(tags);

			// Update Globals
			this.UpdateGlobals(tags);

			// Write Messages
			this.WriteMessages(tags);

			if (Game.FocusState == FocusState.Main)
				this.Focus();
		}

		private void WriteMessages(RdlTagCollection tags)
		{
			List<RdlMessage> messages = tags.GetMessages();
			if (messages.Count > 0)
			{
				foreach (var msg in messages)
				{
					switch (msg.TypeName)
					{
						case "ERROR":
							this.Write(TextType.Error, msg.Text);
							break;
						case "SYSTEM":
							RdlSystemMessage sysMsg = msg as RdlSystemMessage;
							if (sysMsg != null)
							{
								switch ((RdlSystemMessage.PriorityType)sysMsg.Priority)
								{
									case RdlSystemMessage.PriorityType.PlaceDescription:
										this.Write(TextType.PlaceDesc, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.PlaceName:
										this.Write(TextType.PlaceName, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.PlaceExits:
										this.Write(TextType.PlaceExits, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.PlaceActors:
										this.Write(TextType.PlaceActors, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.PlaceAvatars:
										this.Write(TextType.PlaceAvatars, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.Positive:
										this.Write(TextType.Positive, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.Negative:
										this.Write(TextType.Negative, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.Emote:
										this.Write(TextType.Emote, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.Help:
										this.Write(TextType.Help, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.Level:
										this.Write(TextType.Level, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.Cast:
										this.Write(TextType.Cast, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.Melee:
										this.Write(TextType.Melee, msg.Text);
                                        break;
                                    case RdlSystemMessage.PriorityType.Award:
										this.Write(TextType.Award, msg.Text);
                                        break;
									default:
										this.Write(TextType.System, msg.Text);
										break;	
								}
							}
							break;
						case "NEWS":
							this.Write(TextType.News, msg.Text);
							break;
						case "TELL":
							this.Write(TextType.Tell, msg.Text);
							break;
						case "SHOUT":
							this.Write(TextType.Shout, msg.Text);
							break;
						default:
							this.Write(TextType.Say, msg.Text);
							break;
					}
				}
			}
		}

		private void UpdateAuthKeys(IEnumerable<RdlAuthKey> keys)
		{
			foreach (var key in keys)
			{
				if (key.TypeName.ToUpper() == "USER")
				{
					Settings.UserAuthKey = key.Key;
				}
				else if (key.TypeName.ToUpper() == "PLAYER")
				{
					Settings.PlayerAuthKey = key.Key;
				}
			}
		}

		private void HandleCommands(List<RdlCommand> commands)
		{
			if (commands != null && commands.Count > 0)
			{
				int ownerId = 0;
				int itemId = 0;

				// Commands expected from the server:
				foreach (var cmd in commands)
				{
					switch (cmd.TypeName)
					{	
						case Commands.ItemRemove:
							// ITEMREMOVE OwnerID ItemID
							ownerId = cmd.GetArg<int>(0);
							itemId = cmd.GetArg<int>(1);
							Game.Player.Inventory.Remove(ownerId, itemId);
							if (Game.Target != null)
							{
								Game.Target.Inventory.Remove(ownerId, itemId);
							}
							break;
							// TODO: Implement AvatarRemove command.
						//case Commands.AvatarRemove:
						//    ctlRoom.Remove(cmd.GetArg<int>(0));
						//    break;
						case Commands.AffectRemove:
							Game.Player.Properties.Remove(cmd.GetArg<string>(0));
							break;
						case Commands.Exit:
#if DEBUG
							StorageManager.WriteError(String.Format("[ {0} ]{1}{2}", DateTime.Now, Environment.NewLine, cmd.ToString()));
#endif
							// Redirect to app_offline page.
							ScreenManager.SetScreen(new GameOfflineScreen());
							//System.Windows.Browser.HtmlPage.Window.Eval("window.location.reload();");
							break;
					}
				}
			}
		}

		private void HandleCommandResponse(RdlCommandResponse response)
		{
			if (response != null && !response.Result)
			{
				this.Write(TextType.Error, String.Format("The {0} command failed for the following reason: {1}",
					response.CommandName, response.Message));
			}
		}
		#endregion

		#region Updates
		private void UpdatePlayer(RdlTagCollection tags)
		{
			if (Game.Player != null)
			{
				// Get any properties related to the player instance.
                var properties = tags.GetProperties(_playerId);
				if (properties != null && properties.Count > 0)
				{
					// Location
					Point3 location = new Point3(Game.Player.X, Game.Player.Y, Game.Player.Z);

                    // Zone
                    string zone = Game.Player.Zone;

					// Loop through all of the properties.
					foreach (var item in properties)
					{
						// Target
						if (item.Name.Equals("TargetID"))
						{
							_targetId = item.GetValue<int>();
							Logger.LogDebug("UpdatePlayer: TargetID set to {0}", _targetId);
						}

						// All other properties.
						Game.Player.Properties.SetValue(item.Name, item.Value);

						// Skills
						if (item.Name.StartsWith("Skill_"))
						{
							Game.Player.UpdateSkill(item.Name, (int)item.GetValue<double>());
						}
                    }

                    if (Game.Player.Zone != zone)
                    {
                        if (_hasRequiredQuota)
                        {
                            RdlTagCollection mapTags = StorageManager.ReadMap(Game.Player.Zone);
                            if (mapTags != null && mapTags.Count > 0)
                            {
                                ctlMap.LoadMap(mapTags);
                                ctlMap.HideLoading();
                            }
                        }
                        else
                        {
                            ctlMap.ShowLoading();
                            ServerManager.Instance.SendCommand("MAP", Game.Player.Zone);
                        }
                    }

					Point3 playerLoc = new Point3(Game.Player.X, Game.Player.Y, Game.Player.Z);
					if (location != playerLoc)
					{
						ctlMap.SetView(playerLoc);
						ctlChat.ClearRoom();
					}
				}

				// Get any actors associated with the player.
				var items = tags.GetActors().Where(a => a.OwnerID == _playerId);
				foreach (var item in items)
				{
					// If the item already exits then remove it.
					Game.Player.Inventory.Update(item);

					// Remove the actor from the tags collection.
					tags.Remove(item);
					
				}

				// Grandchildren of the player.
				var playerItems = Game.Player.Inventory.ToList();
				foreach (var item in playerItems)
				{
					// Get any actors associated with the actors associated with the player.
					var children = tags.GetActors().Where(a => a.OwnerID == item.ID);
					foreach (var child in children)
					{
						// If the item already exits then remove it.
						Game.Player.Inventory.Update(child);

						// Remove the actor from the tags collection.
						tags.Remove(child);
					}
				}

				// Remove any actors currently associated with the player that are no longer associated
				// with the player such as items sold to other players or NPCs.
				var previousItems = tags.GetActors().Where(a => Game.Player.Inventory.Count(i => i.ID == a.ID) > 0 && a.OwnerID != _playerId);
				foreach (var previousItem in previousItems)
				{
					Game.Player.Inventory.Update(previousItem);

					// If ownerId == 0 then this item came down to notify the player it was removed.
					if (previousItem.OwnerID == 0)
					{
						tags.Remove(previousItem);
					}
				}

				// Update the character details.
				this.SetCharacterDetails();
			}
		}
		private void UpdateTarget(RdlTagCollection tags)
		{
			if (_targetId == 0)
			{
				// Reset the target.
				Game.Target = null;
				ctlTarget.Visibility = Visibility.Collapsed;
			}
			else
			{
				Logger.LogDebug("UpdateTarget: TargetID = {0}", _targetId);
				var target = tags.GetActors().Where(a => a.ID == _targetId).FirstOrDefault();
				if (target != null)
				{
					Logger.LogDebug("UpdateTarget: Target {0} found within the tags collection.", _targetId);
					// Set the new target.
					Game.Target = new Avatar(target);
				}
				else
					Logger.LogDebug("UpdateTarget: Target not found in tags collection.");
			}

			if (Game.Target != null)
			{
				// Target Properties
				var properties = tags.GetProperties(_targetId);
				if (properties != null && properties.Count > 0)
				{
					// Loop through all of the properties.
					foreach (var item in properties)
					{
						// All other properties.
						Game.Target.Properties.SetValue(item.Name, item.Value);
					}
				}

				// Get any actors associated with the target.
				var items = tags.GetActors().Where(a => a.OwnerID == _targetId);
				foreach (var item in items)
				{
					// If the item already exits then remove it.
					Game.Target.Inventory.Update(item);

					// Remove the actor from the tags collection.
					tags.Remove(item);
				}
				
				// Grandchildren
				var targetItems = Game.Target.Inventory.ToList();
				foreach (var item in targetItems)
				{
					// Get any actors associated with the actors associated with the target.
					var children = tags.GetActors().Where(a => a.OwnerID == item.ID);
					foreach (var child in children)
					{
						// If the item already exits then remove it.
						Game.Target.Inventory.Update(child);

						// Remove the actor from the tags collection.
						tags.Remove(child);
					}
				}

				// Remove any actors currently associated with the target that are no longer associated
				// with the target such as items sold to players.
				var previousItems = tags.GetActors().Where(a => Game.Target.Inventory.Where(i => i.ID == a.ID).Count() > 0 && a.OwnerID != _targetId);
				foreach (var previousItem in previousItems)
				{
					Game.Target.Inventory.Update(previousItem);

					// If ownerId == 0 then this item came down to notify the player it was removed.
					if (previousItem.OwnerID == 0)
					{
						tags.Remove(previousItem);
					}
				}

				this.SetTargetDetails();
			}
		}
		private void UpdateMap(RdlTagCollection tags)
		{
            if (tags.Count(p => p is RdlPlace) > 0)
            {
                ctlMap.LoadMap(tags);
                ctlMap.SetView(new Point3(Game.Player.X, Game.Player.Y, Game.Player.Z));
                ctlMap.HideLoading();
            }
		}
		private void UpdateRoom(RdlTagCollection tags)
		{
			var actors = tags.GetActors().Where(a => a.ID != _playerId 
				//&& a.ID != _targetId
				&& a.ID != 0 // Negative ID will mean a temporary object or randomly generated object.
				&& a.Properties.GetValue<ObjectType>("ObjectType") != ObjectType.Place);
			if (actors.Count() > 0 && Game.Player != null)
			{
				var location = new Point3(Game.Player.X, Game.Player.Y, Game.Player.Z);
				ctlChat.SetRoom(location.ToString(true, true), location, actors);

				var place = ctlMap.FindPlace(location);
				if (place != null)
				{
					place.Actors.Clear();
					place.Actors.AddRange(actors.Convert());

					BindGodeModeDialog(place);
				}
			}
		}
		private void UpdateGlobals(RdlTagCollection tags)
		{
			var properties = tags.GetProperties(0);
			if (properties != null && properties.Count > 0)
			{
				foreach (var item in properties)
				{
					// Time
					if (item.Name.Equals("Time")) ctlMap.SetTime(item);
				}
			}
		}
		#endregion

		#region Splash Screen
		private void StepLoadProgress(string text, int percentage)
		{
			lblLoadText.Text = text;
			ctlLoadbar.Value += percentage;
		}
		private void CloseSplash()
		{
			SplashScreen.Visibility = Visibility.Collapsed;
		}
		#endregion

		#region IScreen Members

		public UIElement Element
		{
			get { return this; }
		}

		public void OnAddedToHost()
		{
		}

		public void OnRemovedFromHost()
		{
		}

		#endregion

		#region Menu Commands
		private void ctlSlotCharacterSheet_Click(ItemSlotEventArgs e)
		{
			e.Handled = true;
			this.SetCharacterDetails();
			diagCharacterSheet.ToggleWindow();
		}

		private void ctlSlotSpells_Click(ItemSlotEventArgs e)
		{
			e.Handled = true;
			diagSpellbook.ToggleWindow();
		}

		private void ctlSlotQuests_Click(ItemSlotEventArgs e)
		{
			e.Handled = true;
			diagQuestLog.ToggleWindow();
		}

		private void ctlSlotWho_Click(ItemSlotEventArgs e)
		{
			e.Handled = true;
			ServerManager.Instance.SendCommand("WHO");
		}

		private void ctlSlotCraft_Click(ItemSlotEventArgs e)
		{
			e.Handled = true;
		}

		private void ctlSlotHousehold_Click(ItemSlotEventArgs e)
		{
			e.Handled = true;
			ctlHousehold.Refresh();
			diagHousehold.Show();
		}

		private void ctlSlotMap_Click(ItemSlotEventArgs e)
		{
			e.Handled = true;
			if (diagMap.Visibility == Visibility.Collapsed)
			{
				// Toggle will show the window so load up the map if the diag is currently hidden.
				diagMapContent.LoadMap(ctlMap.GetTags());
				diagMapContent.SetView(new Point3(Game.Player.X, Game.Player.Y, Game.Player.Z));
			}
			diagMap.ToggleWindow();
		}

		private void ctlSlotHelp_Click(ItemSlotEventArgs e)
		{
			e.Handled = true;
			diagHelp.ToggleWindow();
		}

		private void ctlSlotQuit_Click(ItemSlotEventArgs e)
		{
			e.Handled = true;
			if (MessageBox.Show("Are you sure you wish to quit this session?", "Quit Game Session", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
			{
				ServerManager.Instance.SendCommand(RdlCommand.FromCommonCommand(RdlCommonCommand.QUIT), new ServerResponseEventHandler(this.OnQuitCommandResponse));
			}
		}

		private void ctlSlotBuild_Click(ItemSlotEventArgs e)
		{
			e.Handled = true;
			//ScreenManager.SetScreen(new BuildScreen());
		}
		#endregion

		#region Key Slot Event Handlers
		private void ctlSlotKey0_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ctlSlotKey1_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ctlSlotKey2_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ctlSlotKey3_Click(object sender, RoutedEventArgs e)
		{

		}
		#endregion

		#region Bag Slot Event Handlers
		private void ctlSlotBag0_Click(ItemSlotEventArgs e)
		{
			e.Handled = true;
			diagBag0.ToggleWindow();
		}

		private void ctlSlotBag1_Click(ItemSlotEventArgs e)
		{
			e.Handled = true;
			diagBag1.ToggleWindow();
		}

		private void ctlSlotBag2_Click(ItemSlotEventArgs e)
		{
			e.Handled = true;
			diagBag2.ToggleWindow();
		}

		private void ctlSlotBag3_Click(ItemSlotEventArgs e)
		{
			e.Handled = true;
			diagBag3.ToggleWindow();
		}

		private void ctlSlotBag4_Click(ItemSlotEventArgs e)
		{
			e.Handled = true;
			diagBag4.ToggleWindow();
		}
		#endregion

		#region Avatar Panel Event Handlers
		private void ctlPlayer_Click(object sender, EventArgs e)
		{
			this.SetCharacterDetails();
			diagCharacterSheet.ToggleWindow();

			ServerManager.Instance.SendCommand("TARGET", Game.Player.ID);
		}

		private void ctlTarget_Click(object sender, EventArgs e)
		{
			// TODO: Target Click?
		}
		#endregion

		#region Map Event Handlers
		private void ctlMap_TileNotFound(object sender, EventArgs e)
		{
			ServerManager.Instance.SendCommand("MAP", Game.Player.X, Game.Player.Y, Game.Player.Z, 25, 25);
		}
		private void ctlMap_ActiveTileChanged(object sender, EventArgs e)
		{
			BindGodeModeDialog(ctlMap.ActiveTile.Place);
		}
		private void ctlMap_DirectionClick(Perenthia.Controls.DirectionEventArgs e)
		{
            if (ctlMap.CanMove(e.Direction))
            {
                // Clear the room control.
				//ctlRoom.Clear();
				//ctlRoomMsg.Clear();

                // Close the commerce dialog.
                diagCommerce.Close();

                // Close the quests dialog.
                diagQuests.Close();

                if (e.Direction == "Up" || e.Direction == "Down")
                {
                    ctlMap.ShowLoading();
                }
                ServerManager.Instance.SendCommand(new RdlCommand("MOVE", e.Direction));
            }
            else
            {
                this.Write(TextType.Error, "You can not move in that direction.");
            }
		}
		#endregion

		#region Room Action Event Handler
		private void ctlRoom_Action(object sender, ActionEventArgs e)
		{
			IActionWindow actionWin = null;
			Window dialog = null;
			switch (e.ActionName)
			{	
				case Actions.Tell:
					// Display window for entering a message.
					dialog = diagTell;
					actionWin = diagTellContent;
					break;
				case Actions.Goods:
					// Display the commerce window and then make the call to the server to retrieve the
					// goods and services to display.
					dialog = diagCommerce;
					actionWin = diagCommerceContent;
					diagCommerceContent.ShowLoading();
					if (diagCommerceContent.Target != null && diagCommerceContent.Target.ID != (int)e.ActorAlias)
					{
						diagCommerceContent.Target = null;
					}
					else
					{
						diagCommerceContent.Refresh();
					}
					ServerManager.Instance.SendCommand(new RdlCommand(Actions.Goods, e.ActorAlias));
					break;
				case Actions.Loot:
					dialog = diagInventory;
					actionWin = diagInventoryContent;
					diagInventoryContent.Refresh();
					ServerManager.Instance.SendCommand(new RdlCommand("INVENTORY", e.ActorAlias));
					break;
				case Actions.Quests:
					dialog = diagQuests;
					actionWin = diagQuestsContent;
					diagQuestsContent.Player = Game.Player;
					diagQuestsContent.ShowLoader();
					if (diagQuestsContent.Target != null && diagQuestsContent.Target.ID != (int)e.ActorAlias)
					{
						diagQuestsContent.Target = null;
					}
					else
					{
						if (diagQuestsContent.Target != null) diagQuestsContent.Refresh();
					}
					ServerManager.Instance.SendCommand(new RdlCommand(Actions.Quests, e.ActorAlias));
					break;
			}
			if (dialog != null && actionWin != null)
			{
				actionWin.ParentWindow = dialog;
				actionWin.Load(e);
				dialog.Show();
				dialog.BringToFront();
			}
			else
			{
				this.OnIActionWindowAction(sender, e);
			}
		}
		private void OnIActionWindowAction(object sender, ActionEventArgs e)
		{
			RdlCommand cmd = new RdlCommand(e.ActionName);
			cmd.Args.Add(e.ActorAlias);
			cmd.Args.AddRange(e.Args.ToArray());
			ServerManager.Instance.SendCommand(cmd);

			// Echo certain commands.
			if (e.ActionName.Equals(Actions.Tell))
			{
				this.Write(TextType.Tell, String.Format("You tell {0}: {1}", e.ActorName, e.Args[0]));
			}
		}
		private void diagTellContent_Action(object sender, ActionEventArgs e)
		{
			diagTell.Close();
			this.OnIActionWindowAction(sender, e);
		}
		#endregion

		#region Player & Target Methods
		private void SetCharacterDetails()
		{
			if (Game.Player != null)
			{
				// Avatar Panel
				ctlPlayer.Avatar = Game.Player;

				// Character Sheet
				diagCharacterSheetContent.Avatar = Game.Player;
				diagCharacterSheetContent.Refresh();

				diagCommerceContent.Player = Game.Player;
				diagCommerceContent.Refresh();

				// Quest Log
				diagQuestLogContent.Player = Game.Player;
				diagQuestLogContent.Refresh();

				// Experience
				ctlXp.Maximum = Game.Player.ExperienceMax;
				ctlXp.Value = Game.Player.Experience;
				ToolTipService.SetToolTip(ctlXp, String.Format("Experience {0}/{1}", Game.Player.Experience, Game.Player.ExperienceMax));

				// Bags
				List<RdlActor> bags = Game.Player.Inventory.GetContainers();
				if (bags != null && bags.Count > 0)
				{
					for (int i = 0; i < bags.Count; i++)	
					{
						// Window Title
						Window window = this.FindName(String.Concat("diagBag", i)) as Window;
						if (window != null)
						{
							window.Title = bags[i].Name;
						}
						// Bag Contents
						BagDialog bagDialog = this.FindName(String.Concat("diagBag", i, "Content")) as BagDialog;
						if (bagDialog != null)
						{
							bagDialog.Item = bags[i];
							bagDialog.Contents = Game.Player.Inventory.GetContents(bags[i].ID);
						}
						// Menu Slot
						ItemSlot slot = this.FindName(String.Concat("ctlSlotBag", i)) as ItemSlot;
						if (slot != null)
						{
							slot.ToolTip = bags[i].Name;
							slot.IsEnabled = true;
							slot.Item = bags[i];
						}
					}
				}

				// Spells
				var spells = Game.Player.Inventory.GetSpells();
				if (spells.Count > 0)
				{
					diagSpellbookContent.Clear();
					for (int i = 0; i < spells.Count; i++)
					{
						ItemSlot slot = diagSpellbookContent.FindName(String.Concat("ctlSlot", i)) as ItemSlot;
						if (slot.Item == null || (slot.Item == null))
						{
							slot.Item = spells[i];
						}
					}
				}

				// TODO: Keys

				// Action Slots
				var actions = Game.Player.Properties.Values.Where(p => p.Name.StartsWith("Action_"));
				foreach (var action in actions)
				{
					ItemSlot slot = ActionsContainer.FindName(String.Concat("ctlSlot", action.Name.Replace("Action_", ""))) as ItemSlot;
					if (slot != null)
					{
						RdlActor item = Game.Player.Inventory.FirstOrDefault(p => p.ID == action.GetValue<int>());
						if (item != null)
						{
							slot.Item = item;
						}
						else
						{
							slot.Item = null;
						}
					}
				}

                // Persist locally.
                if (DateTime.Now.Subtract(Game.PlayerLastSave).TotalMinutes >= 15)
                {
                    StorageManager.SavePlayer(Game.Player.ToRdlActor());
                    Game.PlayerLastSave = DateTime.Now;
                }
			}
		}

		private void SetTargetDetails()
		{
			if (Game.Target != null)
			{
				// Target Panel
				ctlTarget.Visibility = Visibility.Visible;
				ctlTarget.Avatar = Game.Target;

				diagCommerceContent.Target = Game.Target;
				diagCommerceContent.Refresh();

				// If the quest dialog is loading then go ahead and render the contents.
				if (diagQuestsContent.IsLoading)
				{
					diagQuestsContent.Target = Game.Target;
					diagQuestsContent.Refresh();
				}

				diagInventoryContent.Target = Game.Target;
				diagInventoryContent.Refresh();
			}
			else
			{
				ctlTarget.Visibility = Visibility.Collapsed;
			}
		}
		#endregion

		#region Dialog Event Handlers
		private void diagCharacterSheetContent_SkillChanged(object sender, SkillChangedEventArgs e)
		{
			Game.Player.Skills[e.Skill.Name].Value = e.NewValue;
			ServerManager.Instance.SendCommand(new RdlCommand("SETSKILL", e.Skill.Name, e.NewValue));
		}
		#endregion

		#region Chat Windows

		private void tabChat_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.ActivateTab(TextPanelTab.Chat);
		}

		private void tabActions_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.ActivateTab(TextPanelTab.Actions);
		}

		private void tabRoom_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.ActivateTab(TextPanelTab.Room);
		}

		private void tabRoomDesc_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.ActivateTab(TextPanelTab.RoomDescription);
		}

		private void TabContentAdded(TextPanelTab tab)
		{
			switch (tab)
			{
				case TextPanelTab.Chat:
					//tabChat.BlinkTab();
					break;
				case TextPanelTab.Actions:
					//tabActions.BlinkTab();
					break;
				case TextPanelTab.Room:
					this.ActivateTab(TextPanelTab.Room);
					break;	
			}
		}

		private void ActivateTab(TextPanelTab tab)
		{
			ctlChat.ActivateTab(tab);
			//switch (tab)
			//{
			//    //case ChatTab.Chat:
			//    //    ctlChat.Visibility = Visibility.Visible;
			//    //    tabChat.ActivateTab();
			//    //    break;
			//    //case ChatTab.Actions:
			//    //    ctlActions.Visibility = Visibility.Visible;
			//    //    tabActions.ActivateTab();
			//    //    break;
			//    case ChatTab.Room:
			//        ctlRoomMsg.Visibility = Visibility.Collapsed;
			//        ctlRoom.Visibility = Visibility.Visible;
			//        tabRoomDesc.DeactivateTab();
			//        tabRoom.ActivateTab();
			//        break;	
			//    case ChatTab.RoomDescription:
			//        ctlRoomMsg.Visibility = Visibility.Visible;
			//        ctlRoom.Visibility = Visibility.Collapsed;
			//        tabRoom.DeactivateTab();
			//        tabRoomDesc.ActivateTab();
			//        break;	
			//}
		}
		#endregion

		#region Load and Save UISettings
		private void LoadUISettings()
		{
			var settings = StorageManager.GetUISettings();
			if (settings != null)
			{
				_uiSettings = settings;

				// Position Windows accordingly.
				_uiSettings.SetWindowValues(ctlPlayer as IWindow);
				_uiSettings.SetWindowValues(ctlChat as IWindow);
				_uiSettings.SetWindowValues(ctlMap as IWindow);
			}
		}
		private void SaveUISettings()
		{
			if (!_loadComplete)
				return;

			// Player Panel
			_uiSettings.AddWindow(ctlPlayer as IWindow);
			_uiSettings.AddWindow(ctlChat as IWindow);
			_uiSettings.AddWindow(ctlMap as IWindow);

			var t = new Thread(() =>
				{
					lock (_uiSettingsLock)
					{
						StorageManager.SaveUISettings(_uiSettings);
					}
				});
			t.Name = "SaveUISettingsThread";
			t.Start();
		}
		#endregion

		#region Window Event Handlers
		private void WindowDragCompleted(object sender, System.EventArgs e)
		{
			SaveUISettings();
		}

		private void WindowResizeCompleted(object sender, System.Windows.SizeChangedEventArgs e)
		{
			SaveUISettings();
		}
		#endregion

		#region God Mode
		private void BindGodeModeDialog(Place place)
		{
			if (_diagGodMode != null)
			{
				_diagGodMode.DataContext = null;
				_diagGodMode.DataContext = place;
			}
		}
		#endregion
	}

	public class DownloaderState
	{
		public string Text { get; set; }
		public int MaxPercentage { get; set; }
	}
}
