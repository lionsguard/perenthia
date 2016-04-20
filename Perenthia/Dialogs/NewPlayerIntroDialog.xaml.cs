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
	public partial class NewPlayerIntroDialog : UserControl
	{
		public event CreateCharacterChoiceEventHandler Selected = delegate { };

		public NewPlayerIntroDialog()
		{
			this.Loaded += new RoutedEventHandler(NewPlayerIntroDialog_Loaded);
			InitializeComponent();
		}

		void NewPlayerIntroDialog_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void btnNew_Click(object sender, RoutedEventArgs e)
		{
			this.Selected(new CreateCharacterChoiceEventArgs() { Choice = CreateCharacterChoice.NewToVirtualWorlds });
		}
	}
}
