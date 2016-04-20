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
	public partial class CreateCharacterChoiceDialog : UserControl
	{
		public event CreateCharacterChoiceEventHandler Selected = delegate { };

		public CreateCharacterChoiceDialog()
		{
			InitializeComponent();
		}

		private void btnPro_Click(object sender, RoutedEventArgs e)
		{
			this.Selected(new CreateCharacterChoiceEventArgs { Choice = CreateCharacterChoice.VirtualWorldPro });
		}

		private void btnNew_Click(object sender, RoutedEventArgs e)
		{
			this.Selected(new CreateCharacterChoiceEventArgs { Choice = CreateCharacterChoice.NewToVirtualWorlds });
		}
	}
	public enum CreateCharacterChoice
	{
		NewToVirtualWorlds,
		VirtualWorldPro
	}

	public delegate void CreateCharacterChoiceEventHandler(CreateCharacterChoiceEventArgs e);
	public class CreateCharacterChoiceEventArgs : EventArgs
	{
		public CreateCharacterChoice Choice { get; set; }	
	}
}
