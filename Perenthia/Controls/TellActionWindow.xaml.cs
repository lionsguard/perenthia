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

using Lionsguard;

namespace Perenthia.Controls
{
	public partial class TellActionWindow : UserControl, IActionWindow
	{
		private ActionEventArgs _args = null;

		public bool IsKeyDown { get; set; }	

		public TellActionWindow()
		{
			this.Loaded += new RoutedEventHandler(TellActionWindow_Loaded);
			InitializeComponent();
		}

		private void TellActionWindow_Loaded(object sender, RoutedEventArgs e)
		{
			txtTell.KeyDown += new KeyEventHandler(txtTell_KeyDown);
		}

		void txtTell_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				this.btnSend_Click(btnSend, new RoutedEventArgs());
				Game.FocusState = FocusState.Main;
			}
			this.IsKeyDown = true;
		}

		private void btnSend_Click(object sender, RoutedEventArgs e)
		{
			if (_args != null)
			{
				this.Action(this, new ActionEventArgs(Actions.Tell, _args.ActorAlias, _args.ActorName, txtTell.Text));
			}
			txtTell.Text = String.Empty;
		}

		#region IActionWindow Members
		public Window ParentWindow { get; set; }

		public event ActionEventHandler Action = delegate { };

		public void Load(ActionEventArgs args)
		{
			lblName.Text = args.ActorName;
			_args = args;
		}

		#endregion

		private void txtTell_GotFocus(object sender, RoutedEventArgs e)
		{
			Game.FocusState = FocusState.Tell;
		}

		private void txtTell_LostFocus(object sender, RoutedEventArgs e)
		{
			Game.FocusState = FocusState.Main;
		}
	}
}
