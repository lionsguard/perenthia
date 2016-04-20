using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace Lionsguard
{
	public static partial class Logger
	{
		public static void Write(LogSeverity severity, string message, params object[] args)
		{
			var msg = message;
			if (args != null && args.Length > 0)
				msg = String.Format(message, args);

			switch (severity)
			{
				case LogSeverity.Warning:
					Debug.WriteLine(String.Concat("WARNING: ", msg));
					break;
				case LogSeverity.Error:
					Debug.WriteLine(String.Concat("ERROR: ", msg));
					break;
				case LogSeverity.Information:
					Debug.WriteLine(String.Concat("INFO: ", msg));
					break;
				default:
					Debug.WriteLine(String.Concat("DEBUG: ", msg));
					break;
			}
		}
	}
}
