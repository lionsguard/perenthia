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
	public partial class QuestFacilitatorDialog : UserControl
	{
		public event QuestFacilitatorTypeSelectedEventHandler QuestFacilitatorTypeSelected = delegate { };

		public QuestFacilitatorDialog()
		{
			InitializeComponent();
		}

		private void btnSubmit_Click(object sender, RoutedEventArgs e)
		{
			QuestFacilitatorType type = QuestFacilitatorType.None;
			if (rdoStartsWith.IsChecked.GetValueOrDefault(false)) type = QuestFacilitatorType.StartsWith;
			else if (rdoEndsWith.IsChecked.GetValueOrDefault(false)) type = QuestFacilitatorType.EndsWith;

			this.QuestFacilitatorTypeSelected(this, new QuestFacilitatorTypeSelectedEventArgs { Type = type });
		}
	}

	public enum QuestFacilitatorType
	{
		None,
		StartsWith,
		EndsWith,
	}

	public delegate void QuestFacilitatorTypeSelectedEventHandler(object sender, QuestFacilitatorTypeSelectedEventArgs e);
	public class QuestFacilitatorTypeSelectedEventArgs : EventArgs
	{
		public QuestFacilitatorType Type { get; set; }	
	}
}
