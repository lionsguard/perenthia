using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lionsguard
{
	public delegate void ExceptionEventHandler(object sender, ExceptionEventArgs e);

	public class ExceptionEventArgs : EventArgs
	{
		public ExceptionEventArgs(Exception exception)
		{
			this.Exception = exception;
		}

		public Exception Exception { get; set; }
	}
}
