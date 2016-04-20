using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lionsguard.Data;

namespace Lionsguard.Content
{
	public class Archive
	{
		public int Year { get; set; }
		public int Month { get; set; }
		public int PostCount { get; set; }	
		public string MonthName { get; set; }	

		public Archive() { }

		internal Archive(SqlNullDataReader reader)
		{
			this.Year = reader.GetInt32("Year");
			this.Month = reader.GetInt32("Month");
			this.MonthName = reader.GetString("MonthName");
			this.PostCount = reader.GetInt32("PostCount");
		}
	}
}
