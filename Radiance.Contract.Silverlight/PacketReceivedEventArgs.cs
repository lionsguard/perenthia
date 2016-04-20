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
using System.ServiceModel.Channels;

namespace Radiance.Contract
{
	public class PacketReceivedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
	{

		private object[] results;

		public PacketReceivedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
			base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public byte[] Data
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				var msg = (Message)this.results[0];
				return msg.GetBody<byte[]>();
			}
		}
	}
}
