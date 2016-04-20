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
using Perenthia.Models;

namespace Perenthia.Converters
{
	public class AvatarImageConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			// value == Mobile
			var mobile = value as Avatar;
			if (mobile == null)
				return null;

			return Asset.GetImageSource(String.Format(Asset.AVATAR_FORMAT, mobile.Race.Name, mobile.Gender));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}

		#endregion

	}
}
