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
using System.Diagnostics.CodeAnalysis;

namespace Perenthia
{
	/// <summary>
	/// Provides a custom implementation of DesignerProperties.GetIsInDesignMode
	/// to work around an issue.
	/// </summary>
	internal static class DesignerProperties
	{
		/// <summary>
		/// Returns whether the control is in design mode (running under Blend
		/// or Visual Studio).
		/// </summary>
		/// <param name="element">The element from which the property value is
		/// read.</param>
		/// <returns>True if in design mode.</returns>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "element", Justification =
			"Matching declaration of System.ComponentModel.DesignerProperties.GetIsInDesignMode (which has a bug and is not reliable).")]
		public static bool GetIsInDesignMode(DependencyObject element)
		{
			if (!_isInDesignMode.HasValue)
			{
				_isInDesignMode =
					(null == Application.Current) ||
					Application.Current.GetType() == typeof(Application);
			}
			return _isInDesignMode.Value;
		}

		/// <summary>
		/// Stores the computed InDesignMode value.
		/// </summary>
		private static bool? _isInDesignMode;
	}
}
