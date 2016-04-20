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

using Radiance;
using Radiance.Markup;

using Perenthia.Household;

namespace Perenthia.Dialogs
{
	public partial class HouseholdDialog : UserControl, IHouseholdScreenManager
	{
		private IHouseholdScreen Screen { get; set; }

		public HouseholdScreenState State
		{
			get { return (HouseholdScreenState)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}
		public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(HouseholdScreenState), typeof(HouseholdDialog), new PropertyMetadata(HouseholdScreenState.SearchOrCreate, new PropertyChangedCallback(HouseholdDialog.OnStatePropertyChanged)));
		private static void OnStatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as HouseholdDialog).Refresh();
		}

		public HouseholdDialog()
		{
			this.Loaded += new RoutedEventHandler(HouseholdDialog_Loaded);
			InitializeComponent();
		}

		private void HouseholdDialog_Loaded(object sender, RoutedEventArgs e)
		{
			ctlCreate.Manager = ctlDetails.Manager = ctlJoin.Manager = ctlJoinRequest.Manager = ctlJoinRequestComplete.Manager =
				ctlSearch.Manager = ctlSearchOrCreate.Manager = this;
		}

		private void HideAll()
		{
			foreach (var item in this.LayoutRoot.Children)
			{
				item.Visibility = Visibility.Collapsed;
			}
		}

		public void Refresh()
		{
			this.HideAll();

			if (!String.IsNullOrEmpty(Game.Player.HouseholdName))
			{
				this.State = HouseholdScreenState.Details;
			}

			this.Screen = null;
			switch (this.State)
			{
				case HouseholdScreenState.SearchOrCreate:
					this.Screen = ctlSearchOrCreate;
					break;
				case HouseholdScreenState.Create:
					this.Screen = ctlCreate;
					break;
				case HouseholdScreenState.Search:
					this.Screen = ctlSearch;
					break;
				case HouseholdScreenState.Join:
					this.Screen = ctlJoin;
					break;
				case HouseholdScreenState.JoinRequest:
					this.Screen = ctlJoinRequest;
					break;
				case HouseholdScreenState.Details:
					this.Screen = ctlDetails;
					break;
			}
			if (this.Screen != null)
			{
				this.Screen.Show();
			}
		}
	}
}
