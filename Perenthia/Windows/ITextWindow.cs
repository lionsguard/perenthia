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

namespace Perenthia.Windows
{
	public interface ITextWindow
	{
		StackPanel TextContainer { get; }

		void Append(TextType type, string text, object tag, RoutedEventHandler linkCallback);

		void ScrollToEnd();
	}


	public enum TextType
	{
		Say,
		Shout,
		Tell,
		System,
		Error,
		News,
		PlaceDesc,
		Positive,
		Negative,
		Emote,
		PlaceName,
		PlaceExits,
		PlaceActors,
		PlaceAvatars,
		Help,
		Level,
		Cast,
		Melee,
		Award,
	}
}
