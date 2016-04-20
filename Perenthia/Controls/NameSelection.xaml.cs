
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

using Radiance.Markup;

namespace Perenthia.Controls
{
	public partial class NameSelection : UserControl
	{
		public event NameCheckEventHandler NameCheckComplete = delegate { };
		public event TextChangedEventHandler NameChanged = delegate { };

		public bool IsNameAvailable { get; private set; }	

		public string AvatarName
		{
			get { return txtName.Text; }
		}

		public NameSelection()
		{
			this.Loaded += new RoutedEventHandler(NameSelection_Loaded);
			InitializeComponent();
		}

		void NameSelection_Loaded(object sender, RoutedEventArgs e)
		{
			ServerManager.Instance.Reset();
			ServerManager.Instance.Response += new ServerResponseEventHandler(_server_Response);

			txtName.TextChanged += new TextChangedEventHandler(txtName_TextChanged);
		}

		private void txtName_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.IsNameAvailable = false;
			this.NameChanged(this, e);
		}

		void _server_Response(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.ProcessCheckNameResponse(e.Tags));
		}

		private void ProcessCheckNameResponse(RdlTagCollection tags)
		{
			RdlCommandResponse response = tags.GetTags<RdlCommandResponse>(RdlTagName.RESP.ToString(), "CHECKNAME").FirstOrDefault();
			if (response != null)
			{
				this.IsNameAvailable = response.Result;
				this.NameCheckComplete(new NameCheckEventArgs { IsAvailable = response.Result, Message = response.Message });
			}
			//else
			//{
			//    this.IsNameAvailable = false;
			//    this.NameCheckComplete(new NameCheckEventArgs { IsAvailable = false, Message = "Name check failed, please try again." });
			//}
			this.Cursor = Cursors.Arrow;
		}

		private void btnCheckName_Click(object sender, RoutedEventArgs e)
		{
			this.Cursor = Cursors.Wait;
			this.CheckName();
		}

		public void CheckName()
		{
			//if (!this.IsNameAvailable)
			//{
				ServerManager.Instance.SendUserCommand("CHECKNAME", txtName.Text);
			//}
		}
	}

	public delegate void NameCheckEventHandler(NameCheckEventArgs e);

	public class NameCheckEventArgs : EventArgs
	{
		public bool IsAvailable { get; set; }
		public string Message { get; set; }	
	}
}
