using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Perenthia.Controls;

namespace Perenthia
{
	public static class QuantityPopupManager
	{
		private static Dictionary<string, QuantitySelectedEventHandler> _handlers = new Dictionary<string, QuantitySelectedEventHandler>(StringComparer.InvariantCultureIgnoreCase);

		private static Panel _host = null;
		public static void Init(Panel host)
		{
			_host = host;
		}

		public static void ShowQuantity(string key, bool isSell, int itemId, int maxQuantity, QuantitySelectedEventHandler selectedHandler)
		{
			if (_host != null)
			{
				if (_handlers.ContainsKey(key))
				{
					_handlers.Remove(key);
				}
				_handlers.Add(key, selectedHandler);

				QuantityPopup ctl = new QuantityPopup();
				ctl.Tag = key;
				ctl.IsSell = isSell;
				ctl.SetValue(Canvas.ZIndexProperty, 1000);
				ctl.QuantitySelected += new QuantitySelectedEventHandler(ctl_QuantitySelected);
				_host.Children.Add(ctl);
				ctl.Show(itemId, maxQuantity);
			}
		}

		static void ctl_QuantitySelected(object sender, QuantitySelectedEventArgs e)
		{
			QuantityPopup ctl = sender as QuantityPopup;
			if (ctl != null)
			{
				string key = ctl.Tag as string;
				if (!String.IsNullOrEmpty(key) && _handlers.ContainsKey(key))
				{
					_handlers[key](ctl, e);
					_handlers.Remove(key);
					_host.Children.Remove(ctl);
				}
			}
		}
	}
}
