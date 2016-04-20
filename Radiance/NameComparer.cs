using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
	public class NameComparer : IEqualityComparer<string>
	{
		#region IEqualityComparer<string> Members

		public bool Equals(string x, string y)
		{
			return StringComparer.InvariantCultureIgnoreCase.Equals(this.GetFirstName(x), this.GetFirstName(y));
		}

		public int GetHashCode(string obj)
		{
			return StringComparer.InvariantCultureIgnoreCase.GetHashCode(this.GetFirstName(obj));
		}

		#endregion

		private string GetFirstName(string value)
		{
			if (!String.IsNullOrEmpty(value))
			{
				string[] words = value.Split(' ');
				if (words != null && words.Length > 0)
				{
					return words[0];
				}
			}
			return value;
		}
	}
}
