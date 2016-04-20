using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lionsguard.Data;

namespace Lionsguard.Forums
{
	public class Reply : Post
	{
		public Reply() { }

		internal Reply(SqlNullDataReader reader)
			: base(reader)
		{
		}
	}
}
