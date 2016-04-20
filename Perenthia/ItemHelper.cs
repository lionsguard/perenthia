using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia
{
	public static class ItemHelper
	{
		public static void EnsureBuyCost(RdlActor item, double markupPercentage)
		{
			int cost = item.Properties.GetValue<int>("Cost");
			int buyCost = (int)(cost * markupPercentage);
			if (markupPercentage == 0) buyCost = cost;
			item.Properties.SetValue("BuyCost", buyCost);
		}

		public static void EnsureSellCost(RdlActor item, double markdownPercentage)
		{
			if (markdownPercentage > 0) markdownPercentage *= -1;
			int cost = item.Properties.GetValue<int>("Cost");
			int sellCost = (int)(cost * markdownPercentage);
			if (sellCost <= 0) sellCost = cost;
			item.Properties.SetValue("SellCost", sellCost);
		}

		public static string GetQuestRewardItemsDisplay(RdlActor quest)
		{
			StringBuilder sb = new StringBuilder();
			var items = quest.Properties.Where(p => p.Name.StartsWith("RewardItem_"));
			if (items.Count() > 0)
			{
				int count = 0;
				foreach (var item in items)
				{
					if (count > 0) sb.Append(", ");
					TemplateItem temp = JsonHelper.FromJson<TemplateItem>(item.Value.ToString());
					if (temp.Quantity > 1)
					{
						sb.AppendFormat("{0} ({1})", temp.Name, temp.Quantity);
					}
					else
					{
						sb.Append(temp.Name);
					}
					count++;
				}
			}
			return sb.ToString();
		}
	}
}
