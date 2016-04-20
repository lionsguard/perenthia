using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance
{
	public static class Extensions
	{
		/// <summary>
		/// Gets the value of the object at the specified index, cast as the generic type.
		/// </summary>
		/// <typeparam name="T">The System.Type to cast the object value as.</typeparam>
		/// <param name="args">The array of object values to search.</param>
		/// <param name="index">The index position at which to retrieve an object value.</param>
		/// <returns>The value of the object at the specified index, cast as the specified type.</returns>
		public static T GetValue<T>(this object[] args, int index)
		{
			if (args != null && args.Length > index)
			{
				object val = args[index];
				if (val != null)
				{
					if (typeof(T) == typeof(bool))
					{
						val = Boolean.Parse(val.ToString());
					}
					else if (typeof(T).IsEnum)
					{
						return (T)Enum.Parse(typeof(T), val.ToString(), true);
					}
					return (T)Convert.ChangeType(val, typeof(T), null);
				}
			}
			return default(T);
		}

		/// <summary>
		/// Creates an array of RdlObject tags for the specified enumerable list of IActor instances.
		/// </summary>
		/// <param name="collection">The collection of IActor instance to parse as RDL.</param>
		/// <returns>An array of RdlObject instances for the specified collection.</returns>
		public static RdlObject[] ToRdl(IEnumerable<IActor> collection)
		{
			List<RdlObject> list = new List<RdlObject>();
			foreach (var item in collection)
			{
				list.AddRange(item.ToRdl());
			}
			return list.ToArray();
		}
	}
}
