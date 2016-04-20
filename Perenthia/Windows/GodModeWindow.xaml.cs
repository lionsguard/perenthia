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
using Perenthia.Dialogs;
using Perenthia.Models;

namespace Perenthia.Windows
{
	public partial class GodModeWindow : FloatableWindow
	{
		public GodModeWindow()
		{
			InitializeComponent();
		}

		private void btnManageProps_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ActorsPanel.Visibility = Visibility.Collapsed;
			PropertiesPanel.Visibility = Visibility.Visible;
		}

		private void btnManageActors_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ActorsPanel.Visibility = Visibility.Visible;
			PropertiesPanel.Visibility = Visibility.Collapsed;
		}

		private void btnNewPlace_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var win = new NewPlaceDialog();
			win.Closed += (o, ea) =>
				{
				};
			win.Show();
		}

		private void btnUpdate_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var place = this.DataContext as Place;
			if (place == null)
				return;

			ServerManager.Instance.SendCommand("SAVEPLACE", place.ToRdlActor());
			//var alert = new AlertWindow();
			//alert.Show("", "");
		}

		private void btnAddActor_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var win = new NewActorDialog();
			win.Closed += (o, ea) =>
			{
			};
			win.Show();
		}

		private void btnAddProperty_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var place = this.DataContext as Place;
			if (place == null)
				return;

			place.Properties.Add("NewProperty", "Value");

			this.DataContext = null;
			this.DataContext = place;
		}
	}
}

