using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.ServiceModel.Syndication;
using System.Xml;

using Radiance;
using Radiance.Markup;
using Perenthia.Dialogs;
using Perenthia.Screens;
using Perenthia.Controls;
using Perenthia.Windows;
using Perenthia.Models;

namespace Perenthia.Screens
{
	public partial class HomeScreen : UserControl, IScreen
	{
		private bool _hasErrors = false;
		private bool _isAdmin = false;
		private bool _hasRequiredQuota = false;

		private WaitDialog _waitDialog = new WaitDialog();

		public HomeScreen()
		{
			this.Loaded += new RoutedEventHandler(HomeScreen_Loaded);
			InitializeComponent();
		}

		void HomeScreen_Loaded(object sender, RoutedEventArgs e)
		{
			ctlResetCharacter.Completed += new EventHandler(ctlResetCharacter_Completed);

			ServerManager.Instance.Reset();
			ServerManager.Instance.Response += new ServerResponseEventHandler(this.HandleServerResponse);

			// Check and prompt for ISO quota increase.
			_hasRequiredQuota = StorageManager.HasRequiredQuota();
			cbxEnableLocalStorage.IsChecked = StorageManager.HasDeclinedIncrease() ? false : true;
			if (!StorageManager.HasDeclinedIncrease())
			{
				if (!_hasRequiredQuota)
				{
					// Display the increase quota screen.
					ShowIncreaseStorageWindow();
				}
				else
				{
					this.StartLoading();
				}
			}
			else
			{
				this.StartLoading();
			}

			cbxEnableLocalStorage.Checked += new RoutedEventHandler(cbxEnableLocalStorage_Checked);
			cbxEnableLocalStorage.Unchecked += new RoutedEventHandler(cbxEnableLocalStorage_Unchecked);
		}

		private void ShowIncreaseStorageWindow()
		{
			var win = new IncreaseIsoQuotaWindow();
			win.Closed += (o, args) =>
			{
				if (win.DialogResult == true)
				{
					_hasRequiredQuota = true;
					StorageManager.AcceptIncrease();
				}
				else
				{
					StorageManager.DeclineIncrease();
				}
				this.StartLoading();

			};
			win.Show();
		}

		private void cbxEnableLocalStorage_Checked(object sender, RoutedEventArgs e)
		{
			ShowIncreaseStorageWindow();
		}

		private void cbxEnableLocalStorage_Unchecked(object sender, RoutedEventArgs e)
		{
			StorageManager.DeclineIncrease();
			ScreenManager.SetScreen(new HomeScreen());
		}

		private void StartLoading()
		{
			_waitDialog.Show("Connecting to the game server...");
			//ServerManager.Instance.SendUserCommand("CHECKROLE", "God");
			ServerManager.Instance.SendUserCommand("CONNECT");
			this.LoadNews();
		}

		private void ctlResetCharacter_Completed(object sender, EventArgs e)
		{
		}

		#region Server Response and Tag Processing
		private void HandleServerResponse(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.ProcessTags(e.Tags));
		}
		private void ProcessFileUpdates(RdlTagCollection tags)
		{
			var updates = tags.GetTags<RdlTag>("FILEUPDATE", "FILEUPDATE");
			if (updates != null && updates.Count > 0)
			{
				StorageManager.LoadFileUpdatesFromServer(updates);
			}
		}
		private void ProcessSkills(RdlTagCollection tags)
		{
			List<RdlSkill> skills = tags.GetTags<RdlSkill>(RdlTagName.OBJ.ToString(), RdlObjectTypeName.SKILL.ToString());
			if (skills.Count > 0)
			{
				Game.Skills.Clear();
				foreach (var item in skills)
				{
					Game.Skills.Add(new Skill { Name = item.Name, Description = item.Description, Value = item.Value, GroupName = item.GroupName });
				}
				if (_hasRequiredQuota && StorageManager.RequiresFileUpdate(FileNames.Skills))
				{
					StorageManager.WriteTags(FileNames.Skills, skills.ToTagCollection());
				}
			}
		}
		private void ProcessSkillGroups(RdlTagCollection tags)
		{
			List<RdlActor> groups = tags.GetTags<RdlActor>(RdlTagName.OBJ.ToString(), RdlObjectTypeName.ACTOR.ToString());
			List<RdlProperty> skills = tags.GetTags<RdlProperty>(RdlTagName.OBJ.ToString(), RdlObjectTypeName.PROP.ToString());
			if (groups.Count > 0)
			{
				Game.SkillGroups.Clear();
				foreach (var group in groups)
				{
					List<Skill> list = new List<Skill>();
					foreach (var skill in skills.Where(s => s.ID == group.ID))
					{
						list.Add(new Skill { Name = skill.Name, Value = Convert.ToInt32(skill.Value) });
					}
					Game.SkillGroups.Add(group.Name, list);
				}
				if (_hasRequiredQuota && StorageManager.RequiresFileUpdate(FileNames.SkillGroups))
				{
					RdlTagCollection groupTags = new RdlTagCollection();
					groupTags.AddRange(groups.ToTagCollection());
					groupTags.AddRange(skills.ToTagCollection());
					StorageManager.WriteTags(FileNames.SkillGroups, groupTags);
				}
			}
		}
		private void ProcessRaces(RdlTagCollection tags)
		{
			List<RdlRace> races = tags.GetTags<RdlRace>(RdlTagName.OBJ.ToString(), RdlObjectTypeName.RACE.ToString());
			if (races.Count > 0)
			{
				Game.Races.Clear();
				RdlTagCollection raceTags = new RdlTagCollection();
				foreach (var item in races)
				{
					raceTags.Add(item);
					Race race = new Race
					{
						Name = item.Name,
						Description = item.Description,
					};
					RdlProperty prop = tags.GetProperty(item.ID, "Attr_Strength");
					if (prop != null)
					{
						race.Strength = Convert.ToInt32(prop.Value);
						raceTags.Add(prop);
					}
					prop = tags.GetProperty(item.ID, "Attr_Dexterity");
					if (prop != null)
					{
						race.Dexterity = Convert.ToInt32(prop.Value);
						raceTags.Add(prop);
					}
					prop = tags.GetProperty(item.ID, "Attr_Stamina");
					if (prop != null)
					{
						race.Stamina = Convert.ToInt32(prop.Value);
						raceTags.Add(prop);
					}
					prop = tags.GetProperty(item.ID, "Attr_Beauty");
					if (prop != null)
					{
						race.Beauty = Convert.ToInt32(prop.Value);
						raceTags.Add(prop);
					}
					prop = tags.GetProperty(item.ID, "Attr_Intelligence");
					if (prop != null)
					{
						race.Intelligence = Convert.ToInt32(prop.Value);
						raceTags.Add(prop);
					}
					prop = tags.GetProperty(item.ID, "Attr_Perception");
					if (prop != null)
					{
						race.Perception = Convert.ToInt32(prop.Value);
						raceTags.Add(prop);
					}
					prop = tags.GetProperty(item.ID, "Attr_Endurance");
					if (prop != null)
					{
						race.Endurance = Convert.ToInt32(prop.Value);
						raceTags.Add(prop);
					}
					prop = tags.GetProperty(item.ID, "Attr_Affinity");
					if (prop != null)
					{
						race.Affinity = Convert.ToInt32(prop.Value);
						raceTags.Add(prop);
					}

					Game.Races.Add(item.Name, race);
				}
				if (_hasRequiredQuota && StorageManager.RequiresFileUpdate(FileNames.Races))
				{
					StorageManager.WriteTags(FileNames.Races, raceTags);
				}
			}
		}
		private void ProcessTerrain(RdlTagCollection tags)
		{
			List<RdlTerrain> terrain = tags.GetObjects<RdlTerrain>();
			if (terrain.Count > 0)
			{
				Game.Terrain.Clear();
				foreach (var t in terrain)
				{
					Game.Terrain.Add(new Terrain { ID = t.ID, Name = t.Name, Color = t.Color, ImageUri = t.ImageUrl });
				}
				if (_hasRequiredQuota && StorageManager.RequiresFileUpdate(FileNames.Terrain))
				{
					StorageManager.WriteTags(FileNames.Terrain, terrain.ToTagCollection());
				}
			}
		}
		private void ProcessNews(RdlTagCollection tags)
		{
			//var messages = tags.GetMessages().Where(t => t.TypeName == "NEWS").Select(t => t as RdlNewsMessage);
			//StringBuilder sb = new StringBuilder();
			//foreach (var msg in messages)
			//{
			//    sb.Append("<strong>").Append(msg.Title.Replace("&nbsp;", "&#160;")).Append(" - ").Append(msg.Date).Append("</strong>");
			//    sb.Append(msg.Text.Replace("&nbsp;", "&#160;"));
			//}
			//txtNews.Text = sb.ToString();
		}
		private void ProcessCharacters(RdlTagCollection tags)
		{
			List<RdlPlayer> players = tags.GetObjects<RdlPlayer>();
			if (players.Count > 0)
			{
				// Load the player's character avatar panels.
				var avatars = new List<Avatar>();
				for (int i = 0; i < players.Count; i++)
				{
					avatars.Add(new Avatar(players[i]));
				}
				lstAvatars.ItemsSource = avatars;
			}
			else
			{
				// Display the new player dialog.
				var win = new NewPlayerIntroWindow();
				win.Closed += (o, args) =>
					{
						if (win.DialogResult == true)
						{
							ScreenManager.SetScreen(new CreateCharacterWizardScreen());
						}
					};
				win.Show();
			}

			// Get a USER tag that contains the max number of characters available to this user.
			RdlUser user = tags.Where(t => t.TagName == "USER").FirstOrDefault() as RdlUser;
			if (user != null)
			{
				if (players.Count >= user.MaxCharacters)
				{
					btnCreateCharacter.IsEnabled = false;
				}
				// TODO: Purchase Button
			}
		}
		private void ProcessTags(RdlTagCollection tags)
		{
			if (tags.Count == 0)
				return;

			this.ProcessFileUpdates(tags);
			this.ProcessCommands(tags);
			//this.ProcessSkills(tags);
			//this.ProcessTerrain(tags);
			//this.ProcessSkillGroups(tags);
			//this.ProcessRaces(tags);
			//this.ProcessNews(tags);

			//if (_step == Step.Characters)
			//{
			this.ProcessCharacters(tags);
			//}

			// Process any messages, send errors and system messages to the alert panel.
			List<RdlMessage> messages = tags.GetMessages();
			foreach (var msg in messages)
			{
				switch (msg.TypeName)
				{
					case "ERROR":
					case "SYSTEM":
						_waitDialog.Text = msg.Text;
						_hasErrors = true;
						break;
				}
			}

			//if (_step == Step.Role) _step = Step.FileUpdates;
			//else if (_step == Step.FileUpdates) _step = Step.Skills;
			//else if (_step == Step.Skills) _step = Step.SkillGroups;
			//else if (_step == Step.SkillGroups) _step = Step.Races;
			//else if (_step == Step.Races) _step = Step.Terrain;
			//else if (_step == Step.Terrain) _step = Step.Characters;
			//else if (_step == Step.Characters) _step = Step.News;
			//else if (_step == Step.News) _step = Step.None;

			//this.ExecuteCommand();
		}
		private void ProcessCommands(RdlTagCollection tags)
		{
			List<RdlCommand> commands = tags.GetCommands();
			if (commands != null && commands.Count > 0)
			{
				foreach (var cmd in commands)
				{
					if (cmd.TypeName.Equals("EXIT"))
					{
#if DEBUG
						StorageManager.WriteError(String.Format("[ {0} ]{1}{2}", DateTime.Now, Environment.NewLine, cmd.ToString()));
#endif
						ScreenManager.SetScreen(new GameOfflineScreen());
						//System.Windows.Browser.HtmlPage.Window.Eval("window.location.reload();");
					}
					if (cmd.TypeName.Equals("CHECKROLE"))
					{
						// Args:
						// 0 = role
						// 1 = is in role
						Settings.Role = cmd.GetArg<string>(0);
						bool isInRole = cmd.GetArg<bool>(1);
						if (!String.IsNullOrEmpty(Settings.Role) && Settings.Role.ToLower().Equals("god") && isInRole)
						{
							btnAdmin.Visibility = Visibility.Visible;
						}
						else
						{
							btnAdmin.Visibility = Visibility.Collapsed;
						}
					}
					if (cmd.TypeName.Equals("CONNECT"))
					{
						if (_hasErrors)
						{
							_waitDialog.HasCloseButton = true;
						}
						else
						{
							_waitDialog.Close();
						}
					}
				}

			}
		}
		#endregion

		#region Create or Purchase Character Button Event Handlers
		private void btnCreateCharacter_Click(object sender, RoutedEventArgs e)
		{
			ScreenManager.SetScreen(new CreateCharacterWizardScreen());
		}

		private void btnPurchaseCharacter_Click(object sender, RoutedEventArgs e)
		{

		}
		#endregion

		#region News
		private void LoadNews()
		{
			//ServerManager.Instance.SendUserCommand("NEWS");
			this.LoadSyndicationFeed("http://blog.perenthia.com/syndication.axd");
		}

		private void link_Click(object sender, RoutedEventArgs e)
		{
			HyperlinkButton link = sender as HyperlinkButton;
			if (link != null)
			{
				SyndicationItem item = link.Tag as SyndicationItem;
				if (item != null)
				{
					System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(item.Id));
				}
			}
		}

		private void LoadSyndicationFeed(string feedUri)
		{
			WebClient client = new WebClient();
			client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
			client.DownloadStringAsync(new Uri(feedUri));
		}

		private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			try
			{
				using (StringReader sr = new StringReader(e.Result))
				{
					using (XmlReader reader = XmlReader.Create(sr))
					{
						SyndicationFeed blog = SyndicationFeed.Load(reader);
						if (blog != null)
						{
							lstNews.Children.Clear();
							foreach (var item in blog.Items.Take(10))
							{
								TextBlock txt = new TextBlock();
								txt.Foreground = Brushes.TextAltBrush;
								txt.Text = item.PublishDate.ToString();
								lstNews.Children.Add(txt);

								HyperlinkButton link = new HyperlinkButton();
								link.Click += new RoutedEventHandler(link_Click);
								link.Foreground = Brushes.LinkBrush;
								link.Content = item.Title.Text;
								link.Tag = item;
								lstNews.Children.Add(link);

								TextBlock br = new TextBlock();
								br.Text = " ";
								lstNews.Children.Add(br);
							}
						}
					}
				}
			}
			catch (Exception) { }
		}
		#endregion

		#region IScreen Members

		public UIElement Element
		{
			get { return this; }
		}

		public void OnAddedToHost()
		{
			//CompositionTarget.Rendering += OnTick;
		}

		public void OnRemovedFromHost()
		{
			//CompositionTarget.Rendering -= OnTick;
		}

		private void OnTick(object sender, EventArgs e)
		{
			//this.Dispatcher.BeginInvoke(() =>
			//{
			//    RdlTagCollection tags;
			//    while (ServerManager.Instance.ReadTags(out tags))
			//    {
			//        ProcessTags(tags);
			//    }
			//});
		}

		#endregion

		private void OnAvatarPanelClick(object sender, RoutedEventArgs e)
		{
			AvatarPanel pnl = sender as AvatarPanel;
			if (pnl != null)
			{
				if (_isAdmin)
				{
					ScreenManager.SetScreen(new AdminScreen(pnl.AvatarID));
				}
				else
				{
					// ALPHA / BETA
					// If RequiresReset is true then prompt for attributes and skills and save them to the player.
					Avatar avatar = pnl.GetAvatar();
					if (avatar.Properties.GetValue<bool>("RequiresReset"))
					{
						// Reset Character Dialog
						ctlResetCharacter.Show(avatar);
					}
					else
					{
						ScreenManager.SetScreen(new PlayScreen(pnl.AvatarID));
					}
				}
			}
		}

		private void OnAvatarDeleteClick(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;
			if (btn != null)
			{
				var avatar = btn.DataContext as Avatar;
				if (avatar == null) return;

				var win = new DeleteCharacterWindow();
				win.Closed += (o, args) =>
				{
					if (win.DialogResult == true)
					{
						ServerManager.Instance.SendUserCommand("DELETECHARACTER", avatar.ID);
						ScreenManager.SetScreen(new HomeScreen());
					}
				};
				win.Show();
			}
		}

		private void btnAdmin_Click(object sender, RoutedEventArgs e)
		{
			_isAdmin = !_isAdmin;
			if (_isAdmin) btnAdmin.Content = "ADMIN - SET";
			else btnAdmin.Content = "ADMIN";
			Settings.IsAdminMode = _isAdmin;
			//ScreenManager.SetScreen(new AdminScreen());
		}

		private void lstAvatars_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			// ALPHA / BETA
			// If RequiresReset is true then prompt for attributes and skills and save them to the player.
			var avatar = e.AddedItems[0] as Avatar;
			if (avatar == null) return;

			if (avatar.Properties.GetValue<bool>("RequiresReset"))
			{
				// Reset Character Dialog
				ctlResetCharacter.Show(avatar);
			}
			else
			{
				ScreenManager.SetScreen(new PlayScreen(avatar.ID));
			}
		}
	}
}
