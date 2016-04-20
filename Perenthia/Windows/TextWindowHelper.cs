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
	public static class TextWindowHelper
	{
		private const int MaxDisplayItems = 100;

		public static FrameworkElement AppendTextBlock(this ITextWindow win, TextType type, string text, object tag, RoutedEventHandler linkCallback)
		{
			var brush = Brushes.MsgSayBrush;
			var fontSize = 12.0;
			var fontWeight = FontWeights.Normal;
			var fontStyle = FontStyles.Normal;

			bool createLink = false;

			switch (type)
			{
				case TextType.Shout:
					brush = Brushes.MsgShoutBrush;
					break;
				case TextType.Tell:
					createLink = true;
					brush = Brushes.MsgTellBrush;
					break;
				case TextType.System:
					brush = Brushes.MsgSystemBrush;
					fontWeight = FontWeights.Bold;
					break;
				case TextType.Error:
					brush = Brushes.MsgErrorBrush;
					fontWeight = FontWeights.Bold;
					break;
				case TextType.News:
					brush = Brushes.MsgNewsBrush;
					break;
				case TextType.PlaceDesc:
					brush = Brushes.MsgSystemPlaceDescBrush;
					break;
				case TextType.Positive:
					brush = Brushes.PositiveBrush;
					break;
				case TextType.Negative:
					brush = Brushes.NegativeBrush;
					break;
				case TextType.Emote:
					brush = Brushes.MsgEmoteBrush;
					break;
				case TextType.PlaceName:
					brush = Brushes.MsgSystemPlaceNameBrush;
					fontWeight = FontWeights.Bold;
					break;
				case TextType.PlaceExits:
					brush = Brushes.MsgSystemPlaceExitsBrush;
					fontStyle = FontStyles.Italic;
					break;
				case TextType.PlaceActors:
					brush = Brushes.MsgSystemPlaceActorsBrush;
					break;
				case TextType.PlaceAvatars:
					brush = Brushes.MsgSystemPlaceAvatarsBrush;
					break;
				case TextType.Help:
					brush = Brushes.MsgHelpBrush;
					fontWeight = FontWeights.Bold;
					break;
				case TextType.Level:
					brush = Brushes.MsgLevelBrush;
					fontWeight = FontWeights.Bold;
					break;
				case TextType.Cast:
					brush = Brushes.MsgCastBrush;
					break;
				case TextType.Melee:
					brush = Brushes.MsgMeleeBrush;
					break;
				case TextType.Award:
					brush = Brushes.MsgAwardBrush;
					break;
			}

			if (win.TextContainer.Children.Count == MaxDisplayItems)
			{
				win.TextContainer.Children.RemoveAt(0);
			}

			FrameworkElement element = null;

			if (createLink && linkCallback != null)
				element = CreateHyperLink(text, brush, fontWeight, fontStyle, fontSize, tag, linkCallback);
			else
				element = CreateTextBlock(text, brush, fontWeight, fontStyle, fontSize);

			win.TextContainer.Children.Add(element);
			return element;
		}

		private static TextBlock CreateTextBlock(string text, Brush textColor, FontWeight fontWeight, FontStyle fontStyle, double fontSize)
		{
			var txt = new TextBlock();
			txt.FontFamily = new FontFamily("Courier New");
			txt.FontSize = fontSize;
			txt.FontWeight = fontWeight;
			txt.FontStyle = fontStyle;
			txt.Text = text;
			txt.Foreground = textColor;
			txt.TextWrapping = TextWrapping.Wrap;
			return txt;
		}

		private static HyperlinkButton CreateHyperLink(string text, Brush textColor, FontWeight fontWeight, FontStyle fontStyle, double fontSize, object tag, RoutedEventHandler callback)
		{
			TextBlock txt = new TextBlock();
			txt.TextWrapping = TextWrapping.Wrap;
			txt.Text = text;

			HyperlinkButton lnk = new HyperlinkButton();
			lnk.Content = txt;
			lnk.FontFamily = new FontFamily("Courier New");
			lnk.FontSize = fontSize;
			lnk.FontWeight = fontWeight;
			lnk.FontStyle = fontStyle;
			lnk.Foreground = textColor;
			lnk.Tag = tag;
			if (callback != null) lnk.Click += new RoutedEventHandler(callback);
			return lnk;
		}

		//public static TextType TranslateMessageType(ChatType messageType)
		//{
		//    switch (messageType)
		//    {	
		//        case ChatType.Say:
		//        case ChatType.Shout: return TextType.Shout;
		//        case ChatType.Tell: return TextType.Tell;
		//        case ChatType.Emote: return TextType.Emote;
		//        default: return TextType.Say;
		//    }
		//}

		//public static TextType TranslateMessageType(NotificationType messageType)
		//{
		//    switch (messageType)
		//    {
		//        case NotificationType.None: return TextType.System;
		//        case NotificationType.PlaceDescription: return TextType.PlaceDesc;
		//        case NotificationType.Admin: return TextType.System;
		//        case NotificationType.Positive: return TextType.Positive;
		//        case NotificationType.Negative: return TextType.Negative;
		//        case NotificationType.Neutral: return TextType.Say;
		//        case NotificationType.PlaceName: return TextType.PlaceName;
		//        case NotificationType.PlaceExits: return TextType.PlaceExits;
		//        case NotificationType.PlaceActors: return TextType.PlaceActors;
		//        case NotificationType.PlaceAvatars: return TextType.PlaceAvatars;
		//        case NotificationType.Help: return TextType.Help;
		//        case NotificationType.Level: return TextType.Level;
		//        case NotificationType.Cast: return TextType.Cast;
		//        case NotificationType.Melee: return TextType.Melee;
		//        case NotificationType.Award: return TextType.Positive;
		//        default: return TextType.System;
		//    }
		//}
	}
}
