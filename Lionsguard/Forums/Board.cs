using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lionsguard.Data;

namespace Lionsguard.Forums
{
	public class Board
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime DateCreated { get; set; }

		public Board() { }

		internal Board(SqlNullDataReader reader)
		{
			this.ID = reader.GetInt32("BoardId");
			this.Name = reader.GetString("BoardName");
			this.Description = reader.GetString("Description");
			this.DateCreated = reader.GetDateTime("DateCreated");
		}
	}
}
