using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perenthia.Net
{

	#region NetworkExceptionEventArgs
	public delegate void NetworkExceptionEventHandler(NetworkExceptionEventArgs e);
	public class NetworkExceptionEventArgs : EventArgs
	{
		public Exception Exception { get; set; }

		public NetworkExceptionEventArgs(Exception e)
		{
			this.Exception = e;
		}
	}
	#endregion

	#region PolicyRequestReceivedEventArgs
	public delegate void PolicyRequestReceivedEventHandler(PolicyRequestReceivedEventArgs e);
	public class PolicyRequestReceivedEventArgs : EventArgs
	{
		public string Request { get; set; }
	}
	#endregion
}
