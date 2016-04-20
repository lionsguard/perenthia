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

namespace Perenthia.Screens
{
	public partial class ArmorialScreen : UserControl, IScreen
	{
		public ArmorialScreen()
		{
			InitializeComponent();
		}

		#region IScreen Members

		public UIElement Element
		{
			get { return this; }
		}

		public void OnAddedToHost()
		{
		}

		public void OnRemovedFromHost()
		{
		}

		#endregion
	}
}
