using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance.Markup
{
	public static class RdlExtensions
	{
		public static string ToRdlString(this IEnumerable<RdlTag> tags)
		{
			RdlTagCollection col = new RdlTagCollection();
			col.AddRange(tags);
			return col.ToString();
		}

        public static RdlTagCollection ToTagCollection<T>(this IEnumerable<T> tags) where T : RdlTag
        {
            RdlTagCollection col = new RdlTagCollection();
            foreach (var item in tags)
            {
                col.Add(item);
				if (item is RdlObject)
				{
					foreach (var prop in (item as RdlObject).Properties)
					{
						col.Add(prop);	
					}
				}
            }
            return col;
        }
	}
}
