using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lionsguard.Data;

namespace Lionsguard.Content
{
	public class Category
	{
		public int ID { get; set; }
		public string Name { get; set; }

		public Category() { }

		internal Category(SqlNullDataReader reader)
		{
			this.ID = reader.GetInt32("ContentCategoryId");
			this.Name = reader.GetString("CategoryName");
		}
	}
}
