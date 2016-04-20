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
using Radiance.Serialization;

namespace Perenthia.Windows
{
	public interface IWindow
	{
		event EventHandler Maximized;

		string WindowID { get; set; }
		Point Position { get; set; }
		Size Size { get; set; }

		void Minimize();
		void Maximize();
	}
}
