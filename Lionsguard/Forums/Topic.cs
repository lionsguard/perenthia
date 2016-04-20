using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lionsguard.Data;

namespace Lionsguard.Forums
{
	public class Topic : Post
	{
		public int ViewCount { get; set; }
		public int ReplyCount { get; set; }
		public string LastAuthorDisplayName { get; set; }
		public string LastAuthorImageUrl { get; set; }

		public DateTime LastReplyDate { get; set; }
		public Forum Forum { get; set; }	

		public List<Reply> Replies { get; private set; }

		public Topic()
		{
			this.Replies = new List<Reply>();
			this.Forum = null;
		}

		internal Topic(SqlNullDataReader reader)
			: base (reader)
		{
			this.Load(reader);
		}
		internal void Load(SqlNullDataReader reader)
		{
			this.Replies = new List<Reply>();
			this.Forum = null;
			this.ViewCount = reader.GetInt32("ViewCount");
			this.ReplyCount = reader.GetInt32("ReplyCount");
			this.LastReplyDate = reader.GetDateTime("LastReplyDate");
			this.LastAuthorDisplayName = reader.GetString("LastAuthorDisplayName");
			this.LastAuthorImageUrl = reader.GetString("LastAuthorImageUrl");
		}
	}
}
