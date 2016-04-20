using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lionsguard
{
	public static class EnumHelper
	{
		public static bool TryParse<T>(string value, out T result)
		{
			if (typeof(T).IsEnum)
			{
				var data = Enum.GetNames(typeof(T)).Where(s => s.ToLower() == value.ToLower()).FirstOrDefault();
				if (data != null)
				{
					result = (T)Enum.Parse(typeof(T), value, true);
					return true;
				}
			}
			result = (T)Enum.GetValues(typeof(T)).GetValue(0);
			return false;
		}
	}
}
