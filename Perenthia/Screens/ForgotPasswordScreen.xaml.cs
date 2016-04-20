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
using Perenthia.Windows;

namespace Perenthia.Screens
{
	public partial class ForgotPasswordScreen : UserControl, IScreen
	{
		public ForgotPasswordScreen()
		{
			InitializeComponent();
		}

		public ForgotPasswordScreen(string username)
			: this()
		{
			txtUsername.Text = username;
			if (!String.IsNullOrEmpty(username))
				SendForgotPasswordCommand(username);
		}

		private void btnSend_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			valMain.Errors.Clear();
			if (String.IsNullOrEmpty(txtUsername.Text))
			{
				valMain.Errors.Add(new ValidationSummaryItem("Username is required."));
				return;
			}
			SendForgotPasswordCommand(txtUsername.Text);
		}

		private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ScreenManager.SetScreen(new LoginScreen(txtUsername.Text));
		}

		private void SendForgotPasswordCommand(string username)
		{
			ServerManager.Instance.Reset();
			ServerManager.Instance.SendUserCommand("FORGOTPASSWORD", username);

			var alert = new AlertWindow();
			alert.Closed += (o, e) =>
				{
					ScreenManager.SetScreen(new LoginScreen());
				};
			alert.Show("Password Sent", "Your password has been sent to the email address you used when the account was created.");
		}

		#region IScreen Members

		public UIElement Element
		{
			get { return this; }
		}

		public void OnAddedToHost()
		{
		}

		public void OnRemovedFromHost()
		{
		}

		#endregion
	}
}
