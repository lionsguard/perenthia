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
using System.Windows.Data;
using Radiance.Markup;

namespace Perenthia.Converters
{
	public class ActorImageSourceConverter : IValueConverter
	{

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			// value == RdlActor
			if (value == null) return null;

			var actor = value as RdlActor;

			string imageUri = actor.Properties.GetValue<string>("ImageUri");
			if (String.IsNullOrEmpty(imageUri))
			{
				imageUri = String.Format(Asset.AVATAR_FORMAT,
					actor.Properties.GetValue<string>("Race"),
					actor.Properties.GetValue<string>("Gender"));
			}
			return Asset.GetImageSource(imageUri);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}

		#endregion
	}
}
