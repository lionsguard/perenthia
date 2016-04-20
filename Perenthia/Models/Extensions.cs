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
using System.Collections.Generic;
using Radiance.Markup;
using System.Linq;

namespace Perenthia.Models
{
	public static class Extensions
	{
		public static IEnumerable<Actor> Convert(this IEnumerable<RdlActor> actors)
		{
			var list = new List<Actor>();
			foreach (var actor in actors)
			{
				list.Add(new Actor(actor));
			}
			return list;
		}
	}
}
