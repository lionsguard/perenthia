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

namespace Perenthia
{
	public partial class App : Application
	{

		public App()
		{ 
			this.Startup += this.Application_Startup;
			this.Exit += this.Application_Exit;
			this.UnhandledException += this.Application_UnhandledException;

			InitializeComponent();
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			this.RootVisual = new Page();

			Settings.LoadSettings(e.InitParams);
		}

		private void Application_Exit(object sender, EventArgs e)
		{
			ServerManager.Instance.Dispose();
		}
		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
#if !DEBUG
			Logger.LogError("[ {0} ]{1}{2}", DateTime.Now, Environment.NewLine, e.ExceptionObject.ToString());
#endif

			// If the app is running outside of the debugger then report the exception using
			// the browser's exception mechanism. On IE this will display it a yellow alert 
			// icon in the status bar and Firefox will display a script error.
			if (!System.Diagnostics.Debugger.IsAttached)
			{

				// NOTE: This will allow the application to continue running after an exception has been thrown
				// but not handled. 
				// For production applications this error handling should be replaced with something that will 
				// report the error to the website and stop the application.
				e.Handled = true;

				try
				{
					MessageBox.Show("An unexpected error has caused the Silverlight plugin to shutdown. Please close and restart your browser. The error information has been saved to your local system and will be transmitted the next time you connect to the game.", "ERROR", MessageBoxButton.OK);

					string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
					// Redirect the application to the error page.
					System.Windows.Browser.HtmlPage.Window.Eval(String.Format("handleError('/Error.aspx?e={0}');", System.Windows.Browser.HttpUtility.UrlEncode(errorMsg)));
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
