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

using Lionsguard;

namespace Perenthia.Controls
{
	public partial class AttackActionWindow : UserControl, IActionWindow
	{
		public AttackActionWindow()
		{
			this.Loaded += new RoutedEventHandler(AttackActionWindow_Loaded);
			InitializeComponent();
		}

		void AttackActionWindow_Loaded(object sender, RoutedEventArgs e)
		{
		}

		#region IActionWindow Members
		public Window ParentWindow { get; set; }

		public event ActionEventHandler Action = delegate { };

		public void Load(ActionEventArgs args)
		{
		}

		#endregion
	}
}
