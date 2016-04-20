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

namespace Perenthia.Controls
{
	public partial class TravelActionWindow : UserControl, IActionWindow
	{
		public TravelActionWindow()
		{
			this.Loaded += new RoutedEventHandler(TravelActionWindow_Loaded);
			InitializeComponent();
		}

		private void TravelActionWindow_Loaded(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		#region IActionWindow Members

		public event ActionEventHandler Action = delegate { };

		public void Show(ActionEventArgs args)
		{
			this.Visibility = Visibility.Visible;
		}

		public void Close()
		{
			this.Visibility = Visibility.Collapsed;
		}

		#endregion
	}
}
