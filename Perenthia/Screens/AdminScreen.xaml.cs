using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Lionsguard;

using Radiance;
using Radiance.Markup;

using Perenthia.Controls;
using Perenthia.Models;

namespace Perenthia.Screens
{
	public partial class AdminScreen : UserControl, IScreen
	{
		private int _playerId = 0;
		private int _placeId = 0;
		private Point3 _currentLocation = Point3.Empty;

		private DragDropManager _dragDropManager;

		public Avatar Player
		{
			get { return Game.Player; }
			set { Game.Player = value; }
		}

		public List<RdlActor> ItemTemplates { get; set; }
		public List<RdlActor> CreatureTemplates { get; set; }
		public List<RdlActor> NpcTemplates { get; set; }
		public List<RdlActor> QuestTemplates { get; set; }	

		public AdminScreen()
		{
			InitializeComponent();
			this.Loaded += new RoutedEventHandler(AdminScreen_Loaded);
		}

		public AdminScreen(int playerId)
			: this()
		{
			_playerId = playerId;
		}

		private void AdminScreen_Loaded(object sender, RoutedEventArgs e)
		{
			PopupManager.Init(this.LayoutRoot);

			ServerManager.Instance.Reset();
			ServerManager.Instance.Response += new ServerResponseEventHandler(OnServerResponse);

			_dragDropManager = new DragDropManager(LayoutRoot);
			lstActors.DragDropManager = _dragDropManager;
			lstTemplates.DragDropManager = _dragDropManager;

			// Event Hanlders
			ctlMap.CanAlwaysMove = true;
			ctlMap.DirectionClick += new Perenthia.Controls.DirectionEventHandler(ctlMap_DirectionClick);

			ServerManager.Instance.SendUserCommand("PLAY", this.OnPlayCommandResponse, _playerId);
		}

		private void OnServerResponse(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.ProcessTags(e.Tags));
		}
		private void OnPlayCommandResponse(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.ProcessPlayCommandResponse(e.Tags));
		}
		private void OnGetItemTemplatesResponse(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.ProcessItemTemplates(e.Tags));
		}
		private void OnGetCreatureTemplatesResponse(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.ProcessCreatureTemplates(e.Tags));
		}
		private void OnGetNpcTemplatesResponse(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.ProcessNpcTemplates(e.Tags));
		}
		private void OnGetQuestTemplatesResponse(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.ProcessQuestTemplates(e.Tags));
		}
		private void ProcessPlayCommandResponse(RdlTagCollection tags)
		{
			this.ProcessTags(tags);

			// Player
			var player = tags.GetObjects<RdlPlayer>().Where(p => p.ID == _playerId).FirstOrDefault();
			if (player != null)
			{
				this.Player = new Avatar(player);

				// Start Admin Mode
				ServerManager.Instance.SendUserCommand("ADMINBUILD");

				// Load Maps
				ServerManager.Instance.SendUserCommand("ADMINGETMAPS");

				// Place Types
				ServerManager.Instance.SendUserCommand("ADMINGETPLACETYPES");

				// Terrain
				ddlTerrain.ItemsSource = Game.Terrain;

				// Templates
				this.ItemTemplates = new List<RdlActor>();
				this.CreatureTemplates = new List<RdlActor>();
				this.NpcTemplates = new List<RdlActor>();
				this.QuestTemplates = new List<RdlActor>();
				ServerManager.Instance.SendUserCommand("ADMINGETITEMTEMPLATES", this.OnGetItemTemplatesResponse);
				ServerManager.Instance.SendUserCommand("ADMINGETCREATURETEMPLATES", this.OnGetCreatureTemplatesResponse);
				ServerManager.Instance.SendUserCommand("ADMINGETNPCTEMPLATES", this.OnGetNpcTemplatesResponse);
				ServerManager.Instance.SendUserCommand("ADMINGETQUESTTEMPLATES", this.OnGetQuestTemplatesResponse);

				// Map
				ServerManager.Instance.SendCommand("MAP", this.Player.X, this.Player.Y, this.Player.Z, 15, 15);
			}

		}
		private void ProcessItemTemplates(RdlTagCollection tags)
		{
			// Templates will be actors.
			List<RdlActor> actors = tags.GetActors();
			if (actors.Count > 0)
			{
				this.ItemTemplates.Clear();
				this.ItemTemplates.AddRange(actors);
			}
		}
		private void ProcessCreatureTemplates(RdlTagCollection tags)
		{
			// Templates will be actors.
			List<RdlActor> actors = tags.GetActors();
			if (actors.Count > 0)
			{
				this.CreatureTemplates.Clear();
				this.CreatureTemplates.AddRange(actors);
			}
		}
		private void ProcessNpcTemplates(RdlTagCollection tags)
		{
			// Templates will be actors.
			List<RdlActor> actors = tags.GetActors();
			if (actors.Count > 0)
			{
				this.NpcTemplates.Clear();
				this.NpcTemplates.AddRange(actors);
			}
		}
		private void ProcessQuestTemplates(RdlTagCollection tags)
		{
			// Templates will be actors.
			List<RdlActor> actors = tags.GetActors();
			if (actors.Count > 0)
			{
				this.QuestTemplates.Clear();
				this.QuestTemplates.AddRange(actors);
			}
		}
		private void LoadTemplates(List<RdlActor> list)
		{
			lstTemplates.ItemsSource = list;
		}
		private void ProcessTags(RdlTagCollection tags)
		{
			// AuthKey tags
			this.UpdateAuthKeys(tags.Where(t => t.TagName == "AUTH").Select(t => t as RdlAuthKey));

			// Place Types
			var placeTypes = tags.Where(t => t.TagName == "PLACETYPE" && t.TypeName == "PLACETYPE");
			if (placeTypes.Count() > 0)
			{
				List<string> types = new List<string>();
				foreach (var pt in placeTypes)
				{
					types.Add(pt.GetArg<string>(0));
				}
				ddlPlaceTypes.ItemsSource = types;
			}

			// Properties
			if (this.Player != null)
			{
				var properties = tags.GetProperties(_playerId);
				if (properties != null && properties.Count > 0)
				{
					// Location
					Point3 location = new Point3(this.Player.X, this.Player.Y, this.Player.Z);

					// Loop through all of the properties.
					foreach (var item in properties)
					{
						// All other properties.
						this.Player.Properties.SetValue(item.Name, item.Value);
					}

					Point3 playerLoc = new Point3(this.Player.X, this.Player.Y, this.Player.Z);
					if (location != playerLoc)
					{
						ctlMap.SetView(playerLoc);
					}
				}
			}

			int count = tags.GetObjects<RdlPlace>().Count;
			if (count == 1)
			{
				// Place
				RdlPlace place = tags.GetObjects<RdlPlace>().FirstOrDefault();
				if (place != null)
				{
					_placeId = place.ID;
					_currentLocation = new Point3(place.X, place.Y, place.Z);
					txtPlaceName.Text = place.Name;
					txtPlaceDesc.Text = place.Properties.GetValue<string>("Description") ?? String.Empty;
					ddlPlaceTypes.SelectedItem = place.Properties.GetValue<string>("RuntimeType");
					ddlTerrain.SelectedItem = Game.Terrain.Where(t => t.ID == place.Properties.GetValue<int>("Terrain")).FirstOrDefault();
					ctlMap.Tiles.Add(new Tile(place));
					ctlMap.SetView(new Point3(this.Player.X, this.Player.Y, this.Player.Z));
					ctlMap.HideLoading();
				}
				List<RdlActor> actors = tags.GetActors();
				if (actors.Count > 0)
				{
					lstActors.ItemsSource = actors;
				}
			}
			else if (count > 1)
			{
				// Map
				ctlMap.LoadMap(tags);
				ctlMap.SetView(new Point3(this.Player.X, this.Player.Y, this.Player.Z));
				ctlMap.HideLoading();
			}
			
			// Chat Messages
			ctlChat.WriteMessages(tags);
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

		private void ctlChat_TellLinkClick(object sender, Perenthia.Controls.ActionEventArgs e)
		{
			ctlChat.SetFocus(String.Concat("/REPLY ", e.ActorAlias, " "));
		}

		private void ddlMaps_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//MapDetail detail = ddlMaps.SelectedItem as MapDetail;
			//if (detail != null)
			//{
			//    ServerManager.Instance.SendUserCommand("MAP", 12, 12, 0, detail.Width, detail.Height);
			//}
		}

		private void ctlMap_DirectionClick(Perenthia.Controls.DirectionEventArgs e)
		{
			if (e.Direction == "Up" || e.Direction == "Down")
			{
				ctlMap.ShowLoading();
			}
			ServerManager.Instance.SendCommand("ADMINMOVE", e.Direction);
		}

		private void btnSavePlace_Click(object sender, RoutedEventArgs e)
		{
			RdlPlace place = new RdlPlace(_placeId, txtPlaceName.Text, _currentLocation.X, _currentLocation.Y, _currentLocation.Z);
			List<RdlProperty> props = new List<RdlProperty>();
			props.Add(new RdlProperty(place.ID, "Description", txtPlaceDesc.Text));
			props.Add(new RdlProperty(place.ID, "RuntimeType", ddlPlaceTypes.SelectedItem));
			props.Add(new RdlProperty(place.ID, "Terrain", ((Terrain)ddlTerrain.SelectedItem).ID));

			RdlTagCollection tags = new RdlTagCollection();
			tags.Add(place);
			tags.AddRange(props.ToArray());

			string cmdName = "ADMINCREATE";
			if (_placeId > 0) cmdName = "ADMINSAVE";

			RdlCommand cmd = new RdlCommand(cmdName);
			if (_placeId > 0)
			{
				cmd.Args.Add(_placeId);
			}
			else
			{
				cmd.Args.Add("place");
			}

			//ServerManager.Instance.SendCommand(Settings.UserAuthKey, "User", cmd, null, tags);
		}

		private void btnReloadMap_Click(object sender, RoutedEventArgs e)
		{
			ServerManager.Instance.SendCommand("MAP", this.Player.X, this.Player.Y, this.Player.Z, 15, 15);
		}

		private void ctlCreatures_Click(object sender, RoutedEventArgs e)
		{
			this.LoadTemplates(CreatureTemplates);
		}

		private void ctlNpcs_Click(object sender, RoutedEventArgs e)
		{
			this.LoadTemplates(NpcTemplates);
		}

		private void ctlItems_Click(object sender, RoutedEventArgs e)
		{
			this.LoadTemplates(ItemTemplates);
		}

		private void ctlQuests_Click(object sender, RoutedEventArgs e)
		{
			this.LoadTemplates(QuestTemplates);
		}

		private void lstActors_ActorDrop(object sender, ActorEventArgs e)
		{
			// Add the actor to the current place and reload the place.
			ServerManager.Instance.SendCommand("ADMINADDACTOR", e.Actor.Name, this.Player.X, this.Player.Y, this.Player.Z);
		}

		private void btnPlay_Click(object sender, RoutedEventArgs e)
		{
			ScreenManager.SetScreen(new PlayScreen(this.Player.ID));
		}
	}
}
