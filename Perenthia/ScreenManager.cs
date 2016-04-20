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

using Perenthia.Screens;

namespace Perenthia
{
	public static class ScreenManager
	{
		private static IScreenHost _host;

		public static void SetHost(IScreenHost host)
		{
			_host = host;
		}

		public static void SetScreen(IScreen screen)
		{
			_host.SetScreen(screen);
		}
	}
}
