using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lionsguard.Data;

namespace Lionsguard.Content
{
	public class Source
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Url { get; set; }
		public string Description { get; set; }

		public Source() { }

		internal Source(SqlNullDataReader reader)
		{
			this.ID = reader.GetInt32("ContentSourceId");
			this.Name = reader.GetString("SourceName");
			this.Url = reader.GetString("Url");
			this.Description = reader.GetString("Description");
		}
	}
}
