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
	public partial class AlertDialog : UserControl
	{
		public UIElementCollection DialogContent
		{
			get { return this.dialog.DialogContent; }
		}

		public bool ShowCloseButton
		{
			get { return dialog.ShowCloseButton; }
			set { dialog.ShowCloseButton = value; }
		}

		public AlertDialog()
		{
			this.Loaded += new RoutedEventHandler(AlertDialog_Loaded);
			InitializeComponent();
		}

		void AlertDialog_Loaded(object sender, RoutedEventArgs e)
		{
			dialog.Closed += new EventHandler(dialog_Closed);
		}

		void dialog_Closed(object sender, EventArgs e)
		{
			this.Close();
		}

		public void Show()
		{
			this.Visibility = Visibility.Visible;
		}

		public void Close()
		{
			this.Visibility = Visibility.Collapsed;
		}
	}
}
