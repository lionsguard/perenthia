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
using System.Threading;

using Lionsguard;

using Radiance;
using Radiance.Markup;

using Perenthia.Controls;
using Perenthia.Services.Builder;

namespace Perenthia.Screens
{
    public partial class BuildScreen : UserControl, IScreen
    {
        // NOTE: ** Disable actor control properties where the property is a template property.

        private enum PanelType
        {
            Map,
            ActorDetails,
        }

        private PanelType _panelType = PanelType.Map;

		private UserToken _token;

		private DragDropManager _dragDropManager;

		private List<RdlActor> _itemTemplates = new List<RdlActor>();
		private List<RdlActor> _creatureTemplates = new List<RdlActor>();
		private List<RdlActor> _npcTemplates = new List<RdlActor>();
		private List<RdlActor> _questTemplates = new List<RdlActor>();

        public BuildScreen()
        {
			this.Loaded += new RoutedEventHandler(BuildScreen_Loaded);
            InitializeComponent();
        }

		private void BuildScreen_Loaded(object sender, RoutedEventArgs e)
		{
			// Load the game component.
			_token = new UserToken() { AuthKey = Settings.UserAuthKey };

			ctlWait.Hide();

			PopupManager.Init(this.PopupContainer);

			// Stop issuing commands to the server.
			ServerManager.Instance.Reset();

			_dragDropManager = new DragDropManager(this.LayoutRoot);
			lstCreatures.DragDropManager = _dragDropManager;
			lstItems.DragDropManager = _dragDropManager;
			lstNpcs.DragDropManager = _dragDropManager;
			lstQuests.DragDropManager = _dragDropManager;
			lstCreatures.EnableDrag = true;
			lstItems.EnableDrag = true;
			lstNpcs.EnableDrag = true;
			lstQuests.EnableDrag = true;

			ctlMap.StatusUpdated += new MapEditor.StatusUpdatedEventHandler(ctlMap_StatusUpdated);
			ctlMap.PlaceChanged += new PlaceEventHanlder(ctlMap_PlaceChanged);
			ctlMap.MapLoadComplete += new EventHandler(ctlMap_MapLoadComplete);

			ctlProperties.PropertyChanged += new PropertyChangedEventHandler(ctlProperties_PropertyChanged);
			ctlActors.EnableDrag = true;
            ctlActors.ActorDrop += new ActorEventHandler(ctlActors_ActorDrop);
            ctlActors.ActorClick += new ActorEventHandler(ctlActors_ActorClick);

			this.LoadTemplates();
		}

        private void ctlActors_ActorClick(object sender, ActorEventArgs e)
        {
            ctlActorDetails.Actor = e.Actor;
            this.SetMainPanel(PanelType.ActorDetails);
        }

        private void ctlActors_ActorDrop(object sender, ActorEventArgs e)
        {
            ctlWait.Show("Saving actor information...");
			BuilderServiceClient client = ServiceManager.CreateBuilderServiceClient();
            client.SaveActorCompleted += new EventHandler<SaveActorCompletedEventArgs>(client_SaveActorCompleted);
            client.SaveActorAsync(_token, e.Actor.ToString());
        }

        private void client_SaveActorCompleted(object sender, SaveActorCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Success)
                {
                    RdlTagCollection tags = RdlTagCollection.FromString(e.Result.TagString);
                    if (tags != null)
                    {
                        foreach (var actor in tags.GetActors())
                        {
                            ctlMap.UpdateActor(actor);
                        }
                    }
                }
            }
            finally
            {
                ctlWait.Hide();
            }
        }

		private void ctlProperties_PropertyChanged(PropertyChangedEventArgs e)
		{
			// TODO: Tell the map a property changed so that it can mark the tile as modified.
		}

		private void ctlMap_MapLoadComplete(object sender, EventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.LoadComplete());
		}

		private void ctlMap_PlaceChanged(PlaceEventArgs e)
		{
			// Load the properties of the current place.
			ctlProperties.SetProperties(e.Place);
			ctlActors.ItemsSource = e.Place.Actors;
			pnlProperties.IsExpanded = true;
			pnlActors.IsExpanded = true;
		}

		private void ctlMap_StatusUpdated(string status)
		{
			this.UpdateStatus(status);
		}

		#region Template Load Methods
		private void LoadTemplates()
		{
			this.StepLoadProgress("Loading item templates...", 25);
			BuilderServiceClient client = ServiceManager.CreateBuilderServiceClient();
			client.GetItemTemplatesCompleted += new EventHandler<GetItemTemplatesCompletedEventArgs>(client_GetItemTemplatesCompleted);
			client.GetItemTemplatesAsync(_token);
		}

		private void client_GetItemTemplatesCompleted(object sender, GetItemTemplatesCompletedEventArgs e)
		{
			if (e.Result.Success)
			{
				// Load the templates into the controls.
				this.Dispatcher.BeginInvoke(() => this.LoadTemplates(e.Result.TagString, _itemTemplates, lstItems));

				this.StepLoadProgress("Loading creature templates...", 40);
				BuilderServiceClient client = ServiceManager.CreateBuilderServiceClient();
				client.GetCreatureTemplatesCompleted += new EventHandler<GetCreatureTemplatesCompletedEventArgs>(client_GetCreatureTemplatesCompleted);
				client.GetCreatureTemplatesAsync(_token);
			}
			else
			{
				this.Dispatcher.BeginInvoke(() => this.StepLoadProgress(e.Result.Message, 0));
			}
		}

		private void client_GetCreatureTemplatesCompleted(object sender, GetCreatureTemplatesCompletedEventArgs e)
		{
			if (e.Result.Success)
			{
				// Load the templates into the controls.
				this.Dispatcher.BeginInvoke(() => this.LoadTemplates(e.Result.TagString, _creatureTemplates, lstCreatures));

				this.StepLoadProgress("Loading NPC templates...", 55);
				BuilderServiceClient client = ServiceManager.CreateBuilderServiceClient();
				client.GetNpcTemplatesCompleted += new EventHandler<GetNpcTemplatesCompletedEventArgs>(client_GetNpcTemplatesCompleted);
				client.GetNpcTemplatesAsync(_token);
			}
			else
			{
				this.Dispatcher.BeginInvoke(() => this.StepLoadProgress(e.Result.Message, 0));
			}
		}

		private void client_GetNpcTemplatesCompleted(object sender, GetNpcTemplatesCompletedEventArgs e)
		{
			if (e.Result.Success)
			{
				// Load the templates into the controls.
				this.Dispatcher.BeginInvoke(() => this.LoadTemplates(e.Result.TagString, _npcTemplates, lstNpcs));

				this.StepLoadProgress("Loading Quest templates...", 70);
				BuilderServiceClient client = ServiceManager.CreateBuilderServiceClient();
				client.GetQuestTemplatesCompleted += new EventHandler<GetQuestTemplatesCompletedEventArgs>(client_GetQuestTemplatesCompleted);
				client.GetQuestTemplatesAsync(_token);
			}
			else
			{
				this.Dispatcher.BeginInvoke(() => this.StepLoadProgress(e.Result.Message, 0));
			}
		}

		private void client_GetQuestTemplatesCompleted(object sender, GetQuestTemplatesCompletedEventArgs e)
		{
			if (e.Result.Success)
			{
				// Load the templates into the controls.
				this.Dispatcher.BeginInvoke(() => this.LoadTemplates(e.Result.TagString, _questTemplates, lstQuests));
				this.Dispatcher.BeginInvoke(() => this.LoadMaps());
			}
			else
			{
				this.Dispatcher.BeginInvoke(() => this.StepLoadProgress(e.Result.Message, 0));
			}
		}

		private void LoadTemplates(string tagString, List<RdlActor> list, ActorList control)
		{
			List<RdlActor> actors = RdlTagCollection.FromString(tagString).GetActors();
			if (actors.Count > 0)
			{
				list.Clear();
				list.AddRange(actors);
				control.ItemsSource = list;
			}
		}

		private void LoadMaps()
		{
			this.StepLoadProgress("Loading map components...", 10);
			ctlMap.Initialize(_token);
		}

		private void LoadComplete()
		{
			LoaderElement.Visibility = Visibility.Collapsed;
		}
		#endregion

		#region IScreen Members

		public UIElement Element
        {
            get { return this; }
        }

        #endregion

		#region Panels
		private void pnlItems_Collapsed(object sender, RoutedEventArgs e)
		{

		}

		private void pnlItems_Expanded(object sender, RoutedEventArgs e)
		{

		}

		private void pnlCreatures_Collapsed(object sender, RoutedEventArgs e)
		{

		}

		private void pnlCreatures_Expanded(object sender, RoutedEventArgs e)
		{

		}

		private void pnlNpcs_Collapsed(object sender, RoutedEventArgs e)
		{

		}

		private void pnlNpcs_Expanded(object sender, RoutedEventArgs e)
		{

		}

		private void pnlQuests_Collapsed(object sender, RoutedEventArgs e)
		{

		}

		private void pnlQuests_Expanded(object sender, RoutedEventArgs e)
		{

		}
		#endregion

        #region Main Panels
        private void SetMainPanel(PanelType panelType)
        {
            ctlActorDetails.Visibility = ctlMap.Visibility = Visibility.Collapsed;
            switch (panelType)
            {
                case PanelType.ActorDetails:
                    ctlActorDetails.Visibility = Visibility.Visible;
                    break;
                default:
                    // Map
                    ctlMap.Visibility = Visibility.Visible;
                    break;
            }
        }
        #endregion

        public void UpdateStatus(string text)
        {
            this.UpdateStatus(text, null);
        }
		public void UpdateStatus(string text, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                text = String.Format(text, args);
            }
            txtStatus.Text = text;
        }

		private void StepLoadProgress(string message, int percent)
		{
			if (this.CheckAccess())
			{
				txtLoader.Text = message;
				pgLoader.Value = percent;
			}
			else
			{
				this.Dispatcher.BeginInvoke(() => this.StepLoadProgress(message, percent));
			}
		}
	}
}
