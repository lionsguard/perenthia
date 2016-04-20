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

namespace Perenthia.Household
{
	public partial class JoinRequestComplete : UserControl, IHouseholdScreen
	{
		public JoinRequestComplete()
		{
			InitializeComponent();
		}

		#region IHouseholdScreen Members

		public IHouseholdScreenManager Manager { get; set; }

		public void Show()
		{
			this.Visibility = Visibility.Visible;
		}

		#endregion
	}
}
