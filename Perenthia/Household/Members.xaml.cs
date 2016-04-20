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
using Perenthia.Models;

namespace Perenthia.Household
{
	public partial class Members : UserControl
	{
		public IEnumerable<Avatar> Avatars
		{
			get { return (IEnumerable<Avatar>)GetValue(AvatarsProperty); }
			set { SetValue(AvatarsProperty, value); }
		}
		public static readonly DependencyProperty AvatarsProperty = DependencyProperty.Register("Avatars", typeof(IEnumerable<Avatar>), typeof(Members), new PropertyMetadata(null, new PropertyChangedCallback(Members.OnAvatarsPropertyChanged)));
		private static void OnAvatarsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)	
		{
			Members ctl = obj as Members;
			if (ctl != null)
			{
				ctl.grdMembers.ItemsSource = ctl.Avatars;
			}
		}

		public Members()
		{
			InitializeComponent();
		}

		private void lnkAdd_Click(object sender, RoutedEventArgs e)
		{

		}

		private void lnkPromote_Click(object sender, RoutedEventArgs e)
		{

		}

		private void lnkDemote_Click(object sender, RoutedEventArgs e)
		{

		}

		private void lnkRemove_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
