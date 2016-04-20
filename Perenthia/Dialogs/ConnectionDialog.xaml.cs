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

namespace Perenthia.Dialogs
{
	public partial class ConnectionDialog : ChildWindow
	{
		public ConnectionDialog()
		{
			InitializeComponent();
			this.Loaded += new RoutedEventHandler(ConnectionDialog_Loaded);
		}

		void ConnectionDialog_Loaded(object sender, RoutedEventArgs e)
		{
			MessageText.Text = String.Format(SR.ConnectionTestFormat, Settings.GameServerPort);

			// Check socket port, if fails to open a connection then use the http handler.
			ServerManager.Connected += (o, args) =>
				{
					this.Dispatcher.BeginInvoke(() => this.Close());
				};
			ServerManager.Configure();
		}
	}
}

