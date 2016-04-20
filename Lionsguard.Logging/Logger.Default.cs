using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Lionsguard
{
	public static partial class Logger
	{
		public static event LoggerEventHandler LogMessage = delegate { };

		public static void Write(LogSeverity severity, string message, params object[] args)
		{
			var msg = message;
			if (args != null && args.Length > 0)
				msg = String.Format(message, args);

			LogMessage(new LoggerEventArgs { Text = msg, Severity = severity });

			//switch (severity)
			//{
			//    case LogSeverity.Warning:
			//        Trace.TraceWarning(msg);
			//        break;
			//    case LogSeverity.Error:
			//        Trace.TraceError(msg);
			//        break;
			//    case LogSeverity.Information:
			//        Trace.TraceInformation(msg);
			//        break;
			//    case LogSeverity.Command:
			//        Trace.TraceInformation(msg);
			//        break;
			//    default:
			//        // Debug
			//        Debug.WriteLine(msg);
			//        break;
			//}
		}
	}
}
