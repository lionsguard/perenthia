using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lionsguard
{
	public delegate void LoggerEventHandler(LoggerEventArgs e);

	public class LoggerEventArgs : EventArgs
	{
		public string Text { get; set; }
		public LogSeverity Severity { get; set; }	
	}
}
