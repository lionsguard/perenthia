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
	public partial class Details : UserControl, IHouseholdScreen
	{
		public Details()
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

		private void mnuRanks_Click(object sender, EventArgs e)
		{

		}

		private void mnuHonors_Click(object sender, EventArgs e)
		{

		}

		private void mnuMembers_Click(object sender, EventArgs e)
		{

		}

		private void mnuArmory_Click(object sender, EventArgs e)
		{

		}

		private void mnuStatus_Click(object sender, EventArgs e)
		{

		}

		private void mnuStanding_Click(object sender, EventArgs e)
		{

		}

		private void mnuLand_Click(object sender, EventArgs e)
		{

		}

		private void lnkChangeImage_Click(object sender, RoutedEventArgs e)
		{

		}

		private void lnkEditMotto_Click(object sender, RoutedEventArgs e)
		{

		}

		private void lnkEditDesc_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
