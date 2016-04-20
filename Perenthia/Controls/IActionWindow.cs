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

namespace Perenthia.Controls
{
	public interface IActionWindow
	{
		Window ParentWindow { get; set; }
		event ActionEventHandler Action;
		void Load(ActionEventArgs args);
	}

	public static class Actions
	{
		public const string Tell = "TELL";
		public const string Attack = "ATTACK";
		public const string Cast = "CAST";
		public const string Goods = "GOODS";
		public const string Buy = "BUY";
		public const string Sell = "SELL";
		public const string Target = "TARGET";
		public const string Loot = "LOOT";
		public const string Get = "GET";
		public const string Drop = "DROP";
		public const string Quests = "QUESTS";
		public const string StartQuest = "STARTQUEST";
		public const string CompleteQuest = "COMPLETEQUEST";
		public const string Use = "USE";
		public const string SetAction = "SETACTION";
		public const string Equip = "EQUIP";
		public const string Unequip = "UNEQUIP";
	}
}
