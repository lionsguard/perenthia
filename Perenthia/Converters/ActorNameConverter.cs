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
using Radiance;
using Radiance.Markup;

namespace Perenthia.Converters
{
	public class ActorNameConverter : IValueConverter
	{

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			// value == RdlActor
			// parameter == ArticleType
			if (value == null || parameter == null)
				return null;

			var actor = value as RdlActor;
			var articleType = (ArticleType)Enum.Parse(typeof(ArticleType), parameter.ToString(), true);

			var isDead = actor.Properties.GetValue<bool>("IsDead");

			var name = Strings.GetDescriptionName(actor.Name, actor.Properties.GetValue<bool>("HasProperName"), articleType, true, actor.Properties.GetValue<int>("Quantity"));

			if (isDead)
				return String.Concat(name, " (DEAD)");

			return name;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}

		#endregion
	}
}
