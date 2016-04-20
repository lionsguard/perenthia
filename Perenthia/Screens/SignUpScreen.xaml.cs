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
using Perenthia.Models;
using Perenthia.Dialogs;

namespace Perenthia.Screens
{
	public partial class SignUpScreen : UserControl, IScreen
	{
		private WaitDialog _wait = new WaitDialog();

		public SignUpScreen()
		{
			InitializeComponent();
		}

		private void btnSignUp_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SignupForm.ValidationSummary.Errors.Clear();
			if (SignupForm.ValidateItem())
			{
				var user = SignupForm.CurrentItem as User;

				if (user.Password != user.PasswordConfirm)
				{
					SignupForm.ValidationSummary.Errors.Add(new ValidationSummaryItem("Password and Confirm Password must match."));
					return;
				}

				_wait.Show("Contacting server to register account information...");

				ServerManager.Instance.SendUserCommand("SIGNUP",
					HandleServerResponse,
					user.UserName, 
					user.Password, 
					user.Email, 
					user.DisplayName, 
					user.BirthDate.Ticks, 
					user.SecurityQuestion, 
					user.SecurityAnswer);
			}
		}

		private void HandleServerResponse(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() =>
				{
					_wait.Close();
					// Sign Up Response
					var cmd = e.Tags.GetCommands().Where(c => c.TypeName == "SIGNUP").FirstOrDefault();
					if (cmd != null)
					{
						var success = cmd.GetArg<bool>(0);
						if (success)
						{
							ScreenManager.SetScreen(new LoginScreen());
						}
						else
						{
							SignupForm.ValidationSummary.Errors.Add(new ValidationSummaryItem(cmd.GetArg<string>(1)));
						}
					}
					else
					{
						SignupForm.ValidationSummary.Errors.Add(new ValidationSummaryItem("An error occured on the server preventing the registration from completing. The error information has been recorded and will be addressed ASAP. Please try back again soon or contact membership@lionsguard.com for assistance."));
					}
				});
		}

		private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ScreenManager.SetScreen(new LoginScreen());
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
