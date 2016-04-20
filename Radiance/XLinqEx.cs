using System;
using System.Xml.Linq;

namespace System.Xml.Linq
{
	public static class XLinqEx
	{
		/// <summary>
		/// Gets a short value for the specified attribute.
		/// </summary>
		/// <param name="element">The element to parse.</param>
		/// <param name="attributeName">The XName of the attribute to parse.</param>
		/// <returns>A short value representation of the attribute value.</returns>
		public static short GetInt16Value(this XElement element, XName attributeName)
		{
			var attribute = element.Attribute(attributeName);
			if (attribute == null)
				return 0;

			short result = 0;
			Int16.TryParse(attribute.Value, out result);
			return result;
		}

		/// <summary>
		/// Gets an int value for the specified attribute.
		/// </summary>
		/// <param name="element">The element to parse.</param>
		/// <param name="attributeName">The XName of the attribute to parse.</param>
		/// <returns>An int value representation of the attribute value.</returns>
		public static int GetInt32Value(this XElement element, XName attributeName)
		{
			var attribute = element.Attribute(attributeName);
			if (attribute == null)
				return 0;

			int result = 0;
			Int32.TryParse(attribute.Value, out result);
			return result;
		}

		/// <summary>
		/// Gets a long value for the specified attribute.
		/// </summary>
		/// <param name="element">The element to parse.</param>
		/// <param name="attributeName">The XName of the attribute to parse.</param>
		/// <returns>A long value representation of the attribute value.</returns>
		public static long GetInt64Value(this XElement element, XName attributeName)
		{
			var attribute = element.Attribute(attributeName);
			if (attribute == null)
				return 0;

			long result = 0;
			Int64.TryParse(attribute.Value, out result);
			return result;
		}

		/// <summary>
		/// Gets a double value for the specified attribute.
		/// </summary>
		/// <param name="element">The element to parse.</param>
		/// <param name="attributeName">The XName of the attribute to parse.</param>
		/// <returns>A double value representation of the attribute value.</returns>
		public static double GetDoubleValue(this XElement element, XName attributeName)
		{
			var attribute = element.Attribute(attributeName);
			if (attribute == null)
				return 0;

			double result = 0;
			Double.TryParse(attribute.Value, out result);
			return result;
		}

		/// <summary>
		/// Gets a bool value for the specified attribute.
		/// </summary>
		/// <param name="element">The element to parse.</param>
		/// <param name="attributeName">The XName of the attribute to parse.</param>
		/// <returns>A bool value representation of the attribute value.</returns>
		public static bool GetBooleanValue(this XElement element, XName attributeName)
		{
			var attribute = element.Attribute(attributeName);
			if (attribute == null)
				return false;

			bool result = false;
			Boolean.TryParse(attribute.Value, out result);
			return result;
		}

		/// <summary>
		/// Gets a string value for the specified attribute.
		/// </summary>
		/// <param name="element">The element to parse.</param>
		/// <param name="attributeName">The XName of the attribute to parse.</param>
		/// <returns>A string value representation of the attribute value.</returns>
		public static string GetStringValue(this XElement element, XName attributeName)
		{
			var attribute = element.Attribute(attributeName);
			if (attribute == null)
				return String.Empty;

			return attribute.Value;
		}

		/// <summary>
		/// Gets a byte array from the base 64 encoded value of the current element.
		/// </summary>
		/// <param name="element">The element to parse.</param>
		/// <returns>A byte array of the base 64 encoded element value.</returns>
		public static byte[] GetByteArray(this XElement element)
		{
			var value = element.Value;
			if (!String.IsNullOrEmpty(value))
			{
				return Convert.FromBase64String(value);
			}
			return null;
		}
	}
}
