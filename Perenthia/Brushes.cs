using System;
using System.Collections.Generic;
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
	public static class Brushes
	{
		public static readonly SolidColorBrush TextBrush;
		public static readonly SolidColorBrush TextAltBrush;
		public static readonly SolidColorBrush PanelBackgrounBrush;
		public static readonly SolidColorBrush BorderBrush;
		public static readonly SolidColorBrush AlertBrush;
		public static readonly SolidColorBrush HighlightBrush;
		public static readonly SolidColorBrush PositiveBrush;
		public static readonly SolidColorBrush NegativeBrush;
		public static readonly LinearGradientBrush HealthBrush;
		public static readonly LinearGradientBrush WillpowerBrush;
		public static readonly LinearGradientBrush ExperienceBrush;
		public static readonly SolidColorBrush WallBrush;
		public static readonly SolidColorBrush WalkBrush;
		public static readonly SolidColorBrush MsgErrorBrush;
		public static readonly SolidColorBrush MsgSystemBrush;
		public static readonly SolidColorBrush MsgSystemPlaceDescBrush;
		public static readonly SolidColorBrush MsgSystemPlaceNameBrush;
		public static readonly SolidColorBrush MsgSystemPlaceExitsBrush;
		public static readonly SolidColorBrush MsgSystemPlaceActorsBrush;
		public static readonly SolidColorBrush MsgSystemPlaceAvatarsBrush;
		public static readonly SolidColorBrush MsgSystemAdminBrush;
		public static readonly SolidColorBrush MsgTellBrush;
		public static readonly SolidColorBrush MsgSayBrush;
		public static readonly SolidColorBrush MsgShoutBrush;
		public static readonly SolidColorBrush MsgNewsBrush;
		public static readonly SolidColorBrush MsgEmoteBrush;
		public static readonly SolidColorBrush MsgHelpBrush;
		public static readonly SolidColorBrush MsgLevelBrush;
		public static readonly SolidColorBrush MsgCastBrush;
        public static readonly SolidColorBrush MsgMeleeBrush;
        public static readonly SolidColorBrush MsgAwardBrush;
		public static readonly SolidColorBrush HeadingBrush;
		public static readonly SolidColorBrush DialogFillBrush;
		public static readonly SolidColorBrush SelectedBrush;
		public static readonly SolidColorBrush LinkBrush;

		public static readonly SolidColorBrush StatSuperbBrush;
		public static readonly SolidColorBrush StatExcellentBrush;
		public static readonly SolidColorBrush StatAboveAverageBrush;
		public static readonly SolidColorBrush StatAverageBrush;
		public static readonly SolidColorBrush StatBelowAverageBrush;
		public static readonly SolidColorBrush StatPoorBrush;
		public static readonly SolidColorBrush StatBadBrush;
		public static readonly SolidColorBrush StatTerribleBrush;
		public static readonly SolidColorBrush StatEmptyBrush;

		public static readonly SolidColorBrush DirectionOnBrush;
		public static readonly SolidColorBrush DirectionOffBrush;

		public static readonly SolidColorBrush MenuHighlightBorderBrush;
		public static readonly SolidColorBrush MenuHighlightBackgroundBrush;

		static Brushes()
		{
			TextBrush = App.Current.Resources["TextBrush"] as SolidColorBrush;
			TextAltBrush = App.Current.Resources["TextAltBrush"] as SolidColorBrush;
			PanelBackgrounBrush = App.Current.Resources["PanelBackgrounBrush"] as SolidColorBrush;
			BorderBrush = App.Current.Resources["BorderBrush"] as SolidColorBrush;
			AlertBrush = App.Current.Resources["AlertBrush"] as SolidColorBrush;
			HighlightBrush = App.Current.Resources["HighlightBrush"] as SolidColorBrush;
			PositiveBrush = App.Current.Resources["PositiveBrush"] as SolidColorBrush;
			NegativeBrush = App.Current.Resources["NegativeBrush"] as SolidColorBrush;
			HealthBrush = App.Current.Resources["HealthBrush"] as LinearGradientBrush;
			WillpowerBrush = App.Current.Resources["WillpowerBrush"] as LinearGradientBrush;
			ExperienceBrush = App.Current.Resources["ExperienceBrush"] as LinearGradientBrush;
			WallBrush = App.Current.Resources["WallBrush"] as SolidColorBrush;
			WalkBrush = App.Current.Resources["WalkBrush"] as SolidColorBrush;
			MsgErrorBrush = App.Current.Resources["MsgErrorBrush"] as SolidColorBrush;
			MsgSystemBrush = App.Current.Resources["MsgSystemBrush"] as SolidColorBrush;
			MsgSystemPlaceDescBrush = App.Current.Resources["MsgSystemPlaceDescBrush"] as SolidColorBrush;
			MsgSystemPlaceNameBrush = App.Current.Resources["MsgSystemPlaceNameBrush"] as SolidColorBrush;
			MsgSystemPlaceExitsBrush = App.Current.Resources["MsgSystemPlaceExitsBrush"] as SolidColorBrush;
			MsgSystemPlaceActorsBrush = App.Current.Resources["MsgSystemPlaceActorsBrush"] as SolidColorBrush;
			MsgSystemPlaceAvatarsBrush = App.Current.Resources["MsgSystemPlaceAvatarsBrush"] as SolidColorBrush;
			MsgSystemAdminBrush = App.Current.Resources["MsgSystemAdminBrush"] as SolidColorBrush;
			MsgTellBrush = App.Current.Resources["MsgTellBrush"] as SolidColorBrush;
			MsgSayBrush = App.Current.Resources["MsgSayBrush"] as SolidColorBrush;
			MsgShoutBrush = App.Current.Resources["MsgShoutBrush"] as SolidColorBrush;
			MsgNewsBrush = App.Current.Resources["MsgNewsBrush"] as SolidColorBrush;
			MsgEmoteBrush = App.Current.Resources["MsgEmoteBrush"] as SolidColorBrush;
			MsgHelpBrush = App.Current.Resources["MsgHelpBrush"] as SolidColorBrush;
			MsgLevelBrush = App.Current.Resources["MsgLevelBrush"] as SolidColorBrush;
			MsgCastBrush = App.Current.Resources["MsgCastBrush"] as SolidColorBrush;
            MsgMeleeBrush = App.Current.Resources["MsgMeleeBrush"] as SolidColorBrush;
            MsgAwardBrush = App.Current.Resources["MsgAwardBrush"] as SolidColorBrush;
			HeadingBrush = App.Current.Resources["HeadingBrush"] as SolidColorBrush;
			DialogFillBrush = App.Current.Resources["DialogFillBrush"] as SolidColorBrush;
			SelectedBrush = App.Current.Resources["SelectedBrush"] as SolidColorBrush;
			LinkBrush = App.Current.Resources["LinkBrush"] as SolidColorBrush;

			StatSuperbBrush = App.Current.Resources["StatSuperbBrush"] as SolidColorBrush;
			StatExcellentBrush = App.Current.Resources["StatExcellentBrush"] as SolidColorBrush;
			StatAboveAverageBrush = App.Current.Resources["StatAboveAverageBrush"] as SolidColorBrush;
			StatAverageBrush = App.Current.Resources["StatAverageBrush"] as SolidColorBrush;
			StatBelowAverageBrush = App.Current.Resources["StatBelowAverageBrush"] as SolidColorBrush;
			StatPoorBrush = App.Current.Resources["StatPoorBrush"] as SolidColorBrush;
			StatBadBrush = App.Current.Resources["StatBadBrush"] as SolidColorBrush;
			StatTerribleBrush = App.Current.Resources["StatTerribleBrush"] as SolidColorBrush;
			StatEmptyBrush = App.Current.Resources["StatEmptyBrush"] as SolidColorBrush;

			DirectionOnBrush = App.Current.Resources["DirectionOnBrush"] as SolidColorBrush;
			DirectionOffBrush = App.Current.Resources["DirectionOffBrush"] as SolidColorBrush;

			MenuHighlightBackgroundBrush = App.Current.Resources["MenuHighlightBackgroundBrush"] as SolidColorBrush;
			MenuHighlightBorderBrush = App.Current.Resources["MenuHighlightBorderBrush"] as SolidColorBrush;
		}

		public static Brush GetBrush(string key)
		{
			if (App.Current.Resources.Contains(key))
			{
				Brush b = App.Current.Resources[key] as Brush;
				if (b != null)
					return b;
			}
			return new SolidColorBrush(Colors.Transparent);
		}

		private static Dictionary<Color, Brush> _customBrushes = new Dictionary<Color, Brush>();

		public static Brush GetBrush(Color color)
		{
			if (!_customBrushes.ContainsKey(color))
			{
				_customBrushes.Add(color, new SolidColorBrush(color));
			}
			return _customBrushes[color];
		}
	}
}
