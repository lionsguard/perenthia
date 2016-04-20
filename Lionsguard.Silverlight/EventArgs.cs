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

namespace Lionsguard
{
	public delegate void InputReceivedEventHandler(object sender, InputReceivedEventArgs e);
	public class InputReceivedEventArgs : EventArgs
	{
		public string Input { get; set; }

		public InputReceivedEventArgs(string input)
		{
			this.Input = input;
		}
	}
}
