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

namespace Perenthia
{
	public static class PopupManager
	{
		private static Canvas _host = null;
		private static UIElement _element = null;
		public static void Init(Canvas host)
		{
			_host = host;
		}

		public static void Add(UIElement element, Point mousePosition)
		{
			if (_host != null)
			{
				element.SetValue(Canvas.LeftProperty, mousePosition.X);
				element.SetValue(Canvas.TopProperty, mousePosition.Y);
				element.SetValue(Canvas.ZIndexProperty, 10000);

				if (_element != null)
				{
					Remove();
				}
				if (_element == null)
				{
					_element = element;
					_host.Children.Add(_element);
				}
			}
		}

		public static void Remove()
		{
			if (_host != null)
			{
				if (_element != null)
				{
					_host.Children.Remove(_element);
					_element = null;
				}
			}
		}
	}
}
