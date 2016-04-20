using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lionsguard.Data;

namespace Lionsguard.Forums
{
	public class Post
	{
		public int ID { get; set; }
		public int ParentID { get; set; }
		public string Title { get; set; }
		public string Text { get; set; }
		public string DisplayName { get; set; }
		public string ImageUrl { get; set; }
		public string ForumName { get; set; }
		public string BoardName { get; set; }	
		public DateTime PostDate { get; set; }

		public int PostID
		{
			get { return (this.ParentID > 0 ? this.ParentID : this.ID); }
		}

		public Post() { }

		internal Post(SqlNullDataReader reader)
		{
			this.ID = reader.GetInt32("ID");
			this.ParentID = reader.GetInt32("ParentID");
			this.Title = reader.GetString("Title");
			this.Text = reader.GetString("Text");
			this.PostDate = reader.GetDateTime("PostDate");
			this.DisplayName = reader.GetString("DisplayName");
			this.ImageUrl = reader.GetString("ImageUrl");
			this.ForumName = reader.GetString("ForumName");
			this.BoardName = reader.GetString("BoardName");
		}
	}
}
