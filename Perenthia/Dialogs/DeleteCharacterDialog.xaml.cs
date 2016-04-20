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

namespace Perenthia.Dialogs
{
	public partial class DeleteCharacterDialog : UserControl
	{
		public event EventHandler Delete = delegate { };

		public DeleteCharacterDialog()
		{
			InitializeComponent();
		}

		private void btnYes_Click(object sender, RoutedEventArgs e)
		{
			this.Delete(this, e);
		}

		private void btnNo_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void diagDeleteWindow_Closed(object sender, EventArgs e)
		{
			this.Close();
		}

		public void Show()
		{
			diagDeleteWindow.Show();
			this.Visibility = Visibility.Visible;
		}

		public void Close()
		{
			this.Visibility = Visibility.Collapsed;
		}
	}
}
