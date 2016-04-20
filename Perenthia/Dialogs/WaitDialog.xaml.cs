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
	public partial class WaitDialog : ChildWindow
	{
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(WaitDialog), new PropertyMetadata("Processing Request...", (d, e) =>
			{
				var diag = d as WaitDialog;
				if (diag == null)
					return;

				diag.DialogText.Text = diag.Text;
			}));
            
		public WaitDialog()
		{
			InitializeComponent();
		}

		public void Show(string text)
		{
			this.Text = text;
			this.Show();
		}
	}
}

