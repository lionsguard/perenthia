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
using Radiance;
using Perenthia.Models;

namespace Perenthia.Converters
{
	public class ActorPropertyConverter : IValueConverter
	{

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			// value == Actor
			// parameter == PropertyName
			if (value == null || parameter == null)
				goto ReturnDefault;

			var val = (value as Actor).Properties[parameter.ToString()].Value;
			if (val != null)
				return System.Convert.ChangeType(val, targetType, null);

			ReturnDefault:
			return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}

		#endregion
	}
}
