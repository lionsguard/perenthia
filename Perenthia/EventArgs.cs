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

using Radiance;
using Radiance.Markup;

namespace Perenthia
{
	public delegate void NotificationEventHandler(object sender, NotificationEventArgs e);
	public class NotificationEventArgs : EventArgs
	{
		public string Message { get; set; }

		public NotificationEventArgs(string message)
		{
			this.Message = message;
		}
	}

	public delegate void ActorEventHandler(object sender, ActorEventArgs e);
	public class ActorEventArgs : EventArgs
	{
		public RdlActor Actor { get; set; }	
	}
}
