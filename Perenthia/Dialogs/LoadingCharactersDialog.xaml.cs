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
	public partial class LoadingCharactersDialog : UserControl
	{
		public LoadingCharactersDialog()
		{
			InitializeComponent();
		}

		public void SetStatus(string text)
		{
			lblStatus.Text = text;
		}
	}
}
