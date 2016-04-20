using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace Lionsguard
{
	public static class Util
	{
		public static string GetServerUrl(HttpContext context)
		{
            return GetServerUrl(context.Request.Url);
        }
        public static string GetServerUrl(HttpRequest request)
        {
            return GetServerUrl(request.Url);
        }
        public static string GetServerUrl(Uri uri)
        {
            if (!uri.IsDefaultPort)
            {
                return String.Concat("http://", uri.Host, ":", uri.Port);
            }
            return String.Concat("http://", uri.Host);
        }
        public static string GetLionsguardUrl(HttpContext context)
        {
            if (!context.Request.Url.IsDefaultPort)
            {
                return "http://localhost:3452";
            }
            return "http://www.lionsguard.com";
        }

		public static string FormatDisplayText(string text, int maxChars)
		{
			if (text.Length > maxChars)
			{
				for (int i = maxChars - 1; i >= 0; i--)
				{
					if (Char.IsWhiteSpace(text[i]))
					{
						return String.Concat(text.Substring(0, i), "...");
					}
				}
			}
			return text;
		}

		public static string EncryptQueryString(string queryString)
		{
			SecureQueryString qs = new SecureQueryString();
			qs.Deserialize(queryString);
			return qs.ToString();			
		}

		private static bool ByteIsBase64(byte i)
		{
			if (i == 43) return true;//'+'
			else if (i >= 47 && i <= 57) return true;//'/','0' - '9'
			else if (i == 61) return true;//'='
			else if (i >= 65 && i <= 90) return true;//'A' - 'Z'
			else if (i >= 97 && i <= 122) return true;//'a' - 'z'
			else return false;
		}

		public static bool IsBase64(string str)
		{
			foreach (byte b in Encoding.ASCII.GetBytes(str))
			{
				if (!ByteIsBase64(b))
				{
					return false;
				}
			}
			return true;
		}

		public static void Perform301Redirect(HttpContext context, string newLocation)
		{
			if (context != null && context.Response != null)
			{
				try
				{
					context.Response.Status = "301 Moved Permanently";
					context.Response.AddHeader("Location", newLocation);
				}
				catch (System.Threading.ThreadAbortException) { }
			}
		}

		/// <summary>
		/// Converts the string representation of a Guid to its Guid 
		/// equivalent. A return value indicates whether the operation 
		/// succeeded. 
		/// </summary>
		/// <param name="s">A string containing a Guid to convert.</param>
		/// <param name="result">
		/// When this method returns, contains the Guid value equivalent to 
		/// the Guid contained in <paramref name="s"/>, if the conversion 
		/// succeeded, or <see cref="Guid.Empty"/> if the conversion failed. 
		/// The conversion fails if the <paramref name="s"/> parameter is a 
		/// <see langword="null" /> reference (<see langword="Nothing" /> in 
		/// Visual Basic), or is not of the correct format. 
		/// </param>
		/// <value>
		/// <see langword="true" /> if <paramref name="s"/> was converted 
		/// successfully; otherwise, <see langword="false" />.
		/// </value>
		/// <exception cref="ArgumentNullException">
		///        Thrown if <pararef name="s"/> is <see langword="null"/>.
		/// </exception>
		public static bool GuidTryParse(string s, out Guid result)
		{
			if (s == null)
				throw new ArgumentNullException("s");
			Match match = GuidFormat.Match(s);
			if (match.Success)
			{
				result = new Guid(s);
				return true;
			}
			else
			{
				result = Guid.Empty;
				return false;
			}
		}
		private static readonly Regex GuidFormat = new Regex(
				"^[A-Fa-f0-9]{32}$|" +
				"^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
				"^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$");
	}
}
