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
	public partial class Host : UserControl, IScreenHost
	{
		public Host()
		{
			InitializeComponent();
		}

		#region IScreenHost Members

		public void SetScreen(IScreen screen)
		{
			if (this.LayoutRoot.Children.Count > 0)
			{
				var currentScreen = this.LayoutRoot.Children[0] as IScreen;
				if (currentScreen != null)
					currentScreen.OnRemovedFromHost();
			}
			this.LayoutRoot.Children.Clear();
			this.LayoutRoot.Children.Add(screen.Element);
			screen.OnAddedToHost();
		}

		#endregion
	}
}
