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
using Perenthia.Models;

namespace Perenthia
{
	public static class MobileHelper
	{
		public static bool IsGoodsAndServicesSeller(Avatar mobile)
		{
			return IsGoodsAndServicesSellerInternal(mobile.Properties.GetValue<string>("MobileType"));
		}

		public static bool IsGoodsAndServicesSeller(RdlActor mobile)
		{
			return IsGoodsAndServicesSellerInternal(mobile.Properties.GetValue<string>("MobileType"));
		}

		private static bool IsGoodsAndServicesSellerInternal(string mobileTypeString)
		{
			if (!String.IsNullOrEmpty(mobileTypeString))
			{
				MobileTypes type = (MobileTypes)Enum.Parse(typeof(MobileTypes), mobileTypeString, true);
				if (((type & MobileTypes.Banker) == MobileTypes.Banker)
				   || ((type & MobileTypes.Merchant) == MobileTypes.Merchant)
				   || ((type & MobileTypes.Innkeeper) == MobileTypes.Innkeeper)
				   || ((type & MobileTypes.Priest) == MobileTypes.Priest)
				   || ((type & MobileTypes.Trainer) == MobileTypes.Trainer))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsQuestGiver(Avatar mobile)
		{
			return IsQuestGiverInternal(mobile.Properties.GetValue<string>("MobileType"));
		}

		public static bool IsQuestGiver(RdlActor mobile)
		{
			return IsQuestGiverInternal(mobile.Properties.GetValue<string>("MobileType"));
		}

		private static bool IsQuestGiverInternal(string mobileTypeString)
		{
			if (!String.IsNullOrEmpty(mobileTypeString))
			{
				MobileTypes type = (MobileTypes)Enum.Parse(typeof(MobileTypes), mobileTypeString, true);
				if ((type & MobileTypes.QuestGiver) == MobileTypes.QuestGiver)
				{
					return true;
				}
			}
			return false;
		}
	}
}
