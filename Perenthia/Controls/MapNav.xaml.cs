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
	public partial class MapNav : UserControl
	{
		public event DirectionEventHandler DirectionClick = delegate { };

		public MapNav()
		{
			InitializeComponent();
		}
		private void HandleDirectionClick(string direction)
		{
			this.DirectionClick(new DirectionEventArgs { Direction = direction });
		}

		private void north_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("North");
		}

		private void south_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("South");
		}

		private void east_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("East");
		}

		private void west_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("West");
		}

		private void northwest_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("Northwest");
		}

		private void northeast_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("Northeast");
		}

		private void southwest_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("Southwest");
		}

		private void southeast_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("Southeast");
		}

		private void up_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("Up");
		}

		private void down_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("Down");
		}
	}
}
