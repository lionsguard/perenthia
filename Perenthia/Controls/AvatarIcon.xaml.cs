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
	public partial class AvatarIcon : UserControl
	{
		public event RoutedEventHandler Click = delegate{};
		
		public AvatarIcon()
		{
			InitializeComponent();
		}

		private void btnTarget_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Click(sender, e);
		}
	}
}
