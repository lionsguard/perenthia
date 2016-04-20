using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Radiance.Markup
{
	/// <summary>
	/// Provides static properties and methods for formatting and parsing formatted text included with RDL tags.
	/// </summary>
	public static class RdlTextFormatter
	{
		private static readonly Regex FormattedString = new Regex(@"(?<FORMAT>\[f\:\w+\])*", RegexOptions.IgnoreCase | RegexOptions.Multiline);

		/// <summary>
		/// Parses the specified formatted text into a collection of RdlText objects.
		/// </summary>
		/// <param name="text">The formatted text from an RDL tag in which to parse.</param>
		/// <returns>A collection of RdlText instances for the specified fromatted text.</returns>
		public static List<RdlText> Parse(string text)
		{
			List<RdlText> list = new List<RdlText>();

			return list;
		}
	}

	/// <summary>
	/// Represents a text node in an RDL tag.
	/// </summary>
	public class RdlText
	{
		/// <summary>
		/// Gets or sets the format of the text.
		/// </summary>
		public string Format { get; set; }

		/// <summary>
		/// Gets or sets the text, without formatting information.
		/// </summary>
		public string Text { get; set; }	
	}
}
