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
using Perenthia.Dialogs;
using System.Threading;
using Perenthia.Windows;

namespace Perenthia.Screens
{
	public partial class LoginScreen : UserControl, IScreen
	{
		private WaitDialog _waitDialog = new WaitDialog();
		private string _username = String.Empty;

		public LoginScreen()
		{
			this.Loaded += new RoutedEventHandler(LoginScreen_Loaded);
			InitializeComponent();
		}
		public LoginScreen(string username)
			: this()
		{
			_username = username;
		}

		void LoginScreen_Loaded(object sender, RoutedEventArgs e)
		{
			// Check socket port, if fails to open a connection then use the http handler.
			ServerManager.Connected += (o, args) =>
			{
				this.Dispatcher.BeginInvoke(() =>
				{
					ServerManager.Instance.Reset();
					ServerManager.Instance.Response += new ServerResponseEventHandler(_server_Response);

					txtUsername.Focus();

					// Check local storage for the remember me token.
					var token = StorageManager.GetPersistLoginToken();
					if (!String.IsNullOrEmpty(token))
					{
						_waitDialog.Show("Attempting login...");
						ServerManager.Instance.SendUserCommand("LOGIN", token);
					}

					txtUsername.Text = _username;
				});
			};
			ServerManager.ConnectFailed += (o, args) =>
			{
				this.Dispatcher.BeginInvoke(() =>
					{
						var alert = new AlertWindow();
						alert.Closed += (s, a) =>
							{
								System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("http://www.perenthia.com"));
							};
						alert.Show("Connection to Server Failed", String.Format(SR.ConnectionFailed, Settings.GameServerPort));
					});
			};
			ServerManager.Configure();
		}

		void _server_Response(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.ProcessResponse(e.Tags));
		}

		#region IScreen Members

		public UIElement Element
		{
			get { return this; }
		}

		public void OnAddedToHost()
		{
			//CompositionTarget.Rendering += OnTick;
		}

		public void OnRemovedFromHost()
		{
			//CompositionTarget.Rendering -= OnTick;
		}

		private void OnTick(object sender, EventArgs e)
		{
			//this.Dispatcher.BeginInvoke(() =>
			//    {
			//        RdlTagCollection tags;
			//        while (ServerManager.Instance.ReadTags(out tags))
			//        {
			//            ProcessResponse(tags);
			//        }
			//    });
		}

		#endregion

		private void btnLogin_Click(object sender, RoutedEventArgs e)
		{
			string username = txtUsername.Text;
			string password = txtPassword.Password;
			var rememberMe = cbxRememberMe.IsChecked.GetValueOrDefault(false);

			valMain.Errors.Clear();
			if (String.IsNullOrEmpty(username))
			{
				valMain.Errors.Add(new ValidationSummaryItem("Username is required."));
			}

			if (String.IsNullOrEmpty(password))
			{
				valMain.Errors.Add(new ValidationSummaryItem("Password is required."));
			}

			if (!valMain.HasErrors)
			{
				_waitDialog.Show("Attempting login...");
				ServerManager.Instance.SendUserCommand("LOGIN", username, password, rememberMe);
			}
		}

		private void ProcessResponse(RdlTagCollection tags)
		{
			bool validLogin = false;
			RdlAuthKey key = tags.Where(t => t.TagName == "AUTH").FirstOrDefault() as RdlAuthKey;
			if (key != null)
			{
				if (!String.IsNullOrEmpty(key.Key))
				{
					validLogin = true;
				}

				// Check for persist login token.
				if (!String.IsNullOrEmpty(key.PersistLoginToken))
				{
					StorageManager.SetPersistLoginToken(key.PersistLoginToken);
				}
			}
			 
			this.Dispatcher.BeginInvoke(() =>
			{
				_waitDialog.Close();
			});

			if (validLogin)
			{
				Settings.UserAuthKey = key.Key;
				ScreenManager.SetScreen(new HomeScreen());
			}
			else
			{
				valMain.Errors.Add(new ValidationSummaryItem("The username/password combination you supplied is invalid. Please ensure caps lock is not on and try again."));
			}
		}

		private void txtPassword_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				btnLogin_Click(this, new RoutedEventArgs());
			}
		}

		private void lnkSignUp_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ScreenManager.SetScreen(new SignUpScreen());
		}

		private void lnkForgotPassword_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ScreenManager.SetScreen(new ForgotPasswordScreen(txtUsername.Text));
		}
	}
}
