using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lionsguard.Data;

namespace Lionsguard.Forums
{
	public class Forum
	{
		public int ID { get; set; }

		public int TopicCount { get; set; }
		public int ReplyCount { get; set; }	

		public string Title { get; set; }
		public string Description { get; set; }

		public string DisplayName { get; set; }
		public string ImageUrl { get; set; }
		public DateTime LastPostDate { get; set; }
		public Board Board { get; set; }

		public List<string> RequiredRoles { get; private set; }	

		public Forum()
		{
			this.Board = null;
			this.RequiredRoles = new List<string>();
		}

		internal Forum(SqlNullDataReader reader)
			: this()
		{
			this.ID = reader.GetInt32("ForumId");
			this.Title = reader.GetString("Title");
			this.Description = reader.GetString("Description");
			this.TopicCount = reader.GetInt32("TopicCount");
			this.ReplyCount = reader.GetInt32("ReplyCount");
			this.LastPostDate = reader.GetDateTime("LastPostDate");
			this.DisplayName = reader.GetString("DisplayName");
			this.ImageUrl = reader.GetString("ImageUrl");
		}
	}
}
