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
	public partial class Status : UserControl
	{
		public event StatusChangedEventHandler Update = delegate { };

		public Status()
		{
			InitializeComponent();
		}

		private void txtStatus_TextChanged(object sender, TextChangedEventArgs e)
		{
		}

		private void txtStatus_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				this.OnUpdate();
			}
		}

		private void btnUpdate_Click(object sender, RoutedEventArgs e)
		{
			this.OnUpdate();
		}

		private void OnUpdate()
		{
			this.Update(new StatusChangedEventArgs { Status = txtStatus.Text });
		}
	}

	public delegate void StatusChangedEventHandler(StatusChangedEventArgs e);
	public class StatusChangedEventArgs : EventArgs
	{
		public string Status { get; set; }	
	}
}
