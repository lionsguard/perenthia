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

using Perenthia.Screens;

namespace Perenthia
{
	public partial class Page : UserControl
	{
		public Page()
		{
			this.Loaded += new RoutedEventHandler(Page_Loaded);
			InitializeComponent();
		}

		void Page_Loaded(object sender, RoutedEventArgs e)
		{
			lblVersion.Text = String.Format(SR.VersionFormat, Settings.GameVersion);

			Game.Initialize();

			// Set the host control.
			ScreenManager.SetHost(this.host);

			// Go to the login screen.
			ScreenManager.SetScreen(new LoginScreen());

			this.Dispatcher.BeginInvoke(() =>
			{
				try
				{
					var errorData = StorageManager.GetAndClearErrors();
					if (!String.IsNullOrEmpty(errorData))
					{
						var depot = ServiceManager.CreateDepotServiceClient();
						depot.SubmitErrorAsync(Settings.UserAuthKey, errorData);
					}
				}
				catch (Exception) { }
			});
		}
	}
}
