using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lionsguard.Data;

namespace Lionsguard.Content
{
	public class Post
	{
		public int ID { get; set; }
		public string Author { get; set; }
		public string Title { get; set; }
		public string Text { get; set; }
		public bool IsPublished { get; set; }
		public bool IsCommentsEnabled { get; set; }	
		public DateTime DateCreated { get; set; }
		public List<string> Tags { get; private set; }
		public List<Category> Categories { get; private set; }

		public Post()
		{
			this.Tags = new List<string>();
			this.Categories = new List<Category>();
			this.DateCreated = DateTime.Now;
		}

		internal Post(SqlNullDataReader reader)
			: this()
		{
			this.ID = reader.GetInt32("ContentPostId");
			this.Author = reader.GetString("Author");
			this.Title = reader.GetString("Title");
			this.Text = reader.GetString("PostText");
			this.IsPublished = reader.GetBoolean("IsPublished");
			this.IsCommentsEnabled = reader.GetBoolean("IsCommentsEnabled");
			this.DateCreated = reader.GetDateTime("DateCreated");
		}
	}
}
