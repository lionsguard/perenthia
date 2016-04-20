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

namespace Perenthia.Household
{
	public partial class Lobby : UserControl
	{
		public Lobby()
		{
			InitializeComponent();
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
		}
	}
}
