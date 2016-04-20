using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lionsguard.Data;

namespace Lionsguard.Content
{
	public class Comment
	{
		public int ID { get; set; }
		public string Author { get; set; }
		public string Text { get; set; }
		public DateTime DateCreated { get; set; }

		public Comment()
		{
			this.DateCreated = DateTime.Now;
		}

		internal Comment(SqlNullDataReader reader)
			: this()
		{
			this.ID = reader.GetInt32("ContentCommentId");
			this.Author = reader.GetString("Author");
			this.Text = reader.GetString("CommentText");
			this.DateCreated = reader.GetDateTime("DateCreated");
		}
	}
}
