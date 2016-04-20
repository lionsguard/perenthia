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

using Lionsguard;
using Radiance;
using Radiance.Markup;

namespace Perenthia
{
	public static class SlotHelper
	{
		public static void SetSlotItem(Slot slot, RdlActor item)
		{
			if (slot != null)
			{
				if (item != null)
				{
					int quantity = item.Properties.GetValue<int>("Quantity");
					slot.ToolTip = item.Name;
					slot.Item = new SlotItem(item,
						Asset.GetImageSource(item.Properties.GetValue<string>("ImageUri")),
						quantity, item.Properties.GetValue<bool>("IsStackable"));
				}
				else
				{
					slot.Item = null;
				}
			}
		}
	}
}
