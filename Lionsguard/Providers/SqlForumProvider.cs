using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Lionsguard.Data;
using Lionsguard.Forums;
using Lionsguard.Providers;

namespace Lionsguard.Providers
{
	public class SqlForumProvider : ForumProvider
	{
		protected string ConnectionString { get; set; }

		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			base.Initialize(name, config);

			ConnectionString = ProviderUtil.GetConnectionString(config);

			ProviderUtil.CheckUnrecognizedAttributes(config);
		}

		public override List<Board> GetBoards()
		{
			List<Board> list = new List<Board>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(ConnectionString, "dbo.lg_Forums_GetBoards"))
			{
				while (reader.Read())
				{
					list.Add(new Board(reader));
				}
			}
			return list;
		}

		public override List<Forum> GetForums()
		{
			List<Forum> list = new List<Forum>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(ConnectionString, "dbo.lg_Forums_GetForums",
				SqlHelper.CreateInputParam("@BoardName", SqlDbType.NVarChar, ForumManager.BoardName)))
			{
				while (reader.Read())
				{
					list.Add(new Forum(reader));
				}
			}
			return list;
		}

		public override List<Forum> GetForums(Board board)
		{
			List<Forum> list = new List<Forum>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(ConnectionString, "dbo.lg_Forums_GetForumsForBoard",
				SqlHelper.CreateInputParam("@BoardId", SqlDbType.Int, board.ID)))
			{
				while (reader.Read())
				{
					list.Add(new Forum(reader));
				}
			}
			return list;
		}

		public override List<Topic> GetTopics(Forum forum, int startingRowIndex, int maxRows)
		{
			List<Topic> list = new List<Topic>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(ConnectionString, "dbo.lg_Forums_GetTopics",
				SqlHelper.CreateInputParam("@ForumId", SqlDbType.Int, forum.ID),
				SqlHelper.CreateInputParam("@StartRowIndex", SqlDbType.Int, startingRowIndex),
				SqlHelper.CreateInputParam("@MaxRows", SqlDbType.Int, maxRows)))
			{
				while (reader.Read())
				{
					list.Add(new Topic(reader));
				}
			}
			return list;
		}

		public override List<Reply> GetReplies(Topic topic, int startingRowIndex, int maxRows)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(ConnectionString, "dbo.lg_Forums_GetReplies",
				SqlHelper.CreateInputParam("@ForumTopicId", SqlDbType.Int, topic.ID),
				SqlHelper.CreateInputParam("@StartRowIndex", SqlDbType.Int, startingRowIndex),
				SqlHelper.CreateInputParam("@MaxRows", SqlDbType.Int, maxRows)))
			{
				if (reader.Read())
				{
					topic.Load(reader);
				}
				if (reader.NextResult())
				{
					while (reader.Read())
					{
						topic.Replies.Add(new Reply(reader));
					}
				}
			}
			return topic.Replies;
		}

		public override void SaveTopic(Topic topic, Forum forum, int userId, string ipAddress)
		{
			//@ForumId				int, 
			//@PostedByUserId			int, 
			//@PostDate				datetime, 
			//@Title					nvarchar(128), 
			//@TopicText				nvarchar(max), 
			//@IPAddress				nvarchar(15), 
			//@ForumTopicId			int output
			using (SqlCommand cmd = SqlHelper.ExecuteNonQuery(ConnectionString, "dbo.lg_Forums_SaveTopic",
				SqlHelper.CreateInputParam("@ForumId", SqlDbType.Int, forum.ID),
				SqlHelper.CreateInputParam("@PostedByUserId", SqlDbType.Int, userId),
				SqlHelper.CreateInputParam("@PostDate", SqlDbType.DateTime, DateTime.Now),
				SqlHelper.CreateInputParam("@Title", SqlDbType.NVarChar, topic.Title),
				SqlHelper.CreateInputParam("@TopicText", SqlDbType.NVarChar, topic.Text),
				SqlHelper.CreateInputParam("@IPAddress", SqlDbType.NVarChar, ipAddress),
				SqlHelper.CreateInputOutputParam("@ForumTopicId", SqlDbType.Int, topic.ID)))
			{
				topic.ID = Convert.ToInt32(cmd.Parameters["@ForumTopicId"].Value);
			}
		}

		public override void SaveReply(Reply reply, Topic topic, int userId, string ipAddress)
		{
			//@ForumTopicId		int, 
			//@PostedByUserId		int, 
			//@PostDate			datetime,	 
			//@ReplyText			nvarchar(max), 
			//@IPAddress			nvarchar(15),
			//@ForumReplyId		int output
			using (SqlCommand cmd = SqlHelper.ExecuteNonQuery(ConnectionString, "dbo.lg_Forums_SaveReply",
				SqlHelper.CreateInputParam("@ForumTopicId", SqlDbType.Int, topic.ID),
				SqlHelper.CreateInputParam("@PostedByUserId", SqlDbType.Int, userId),
				SqlHelper.CreateInputParam("@PostDate", SqlDbType.DateTime, DateTime.Now),
				SqlHelper.CreateInputParam("@ReplyText", SqlDbType.NVarChar, reply.Text),
				SqlHelper.CreateInputParam("@IPAddress", SqlDbType.NVarChar, ipAddress),
				SqlHelper.CreateInputOutputParam("@ForumReplyId", SqlDbType.Int, reply.ID)))
			{
				reply.ID = Convert.ToInt32(cmd.Parameters["@ForumReplyId"].Value);
			}
		}

		public override Board GetBoard(int id)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(ConnectionString, "dbo.lg_Forums_GetBoard",
				SqlHelper.CreateInputParam("@BoardId", SqlDbType.Int, id)))
			{
				if (reader.Read())
				{
					return new Board(reader);
				}
			}
			return null;
		}

		public override Forum GetForum(int id)
		{
			Forum forum = null;
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(ConnectionString, "dbo.lg_Forums_GetForum",
				SqlHelper.CreateInputParam("@ForumId", SqlDbType.Int, id)))
			{
				if (reader.Read())
				{
					forum = new Forum(reader);
				}
				if (reader.NextResult())
				{
					// Roles
					while (reader.Read())
					{
						forum.RequiredRoles.Add(reader.GetString("RoleName"));
					}
				}
				if (reader.NextResult())
				{
					// Board
					if (reader.Read())
					{
						forum.Board = new Board(reader);
					}
				}
			}
			return forum;
		}

		public override Topic GetTopic(int id)
		{
			Topic topic = null;
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(ConnectionString, "dbo.lg_Forums_GetTopic",
				SqlHelper.CreateInputParam("@TopicId", SqlDbType.Int, id)))
			{
				if (reader.Read())
				{
					topic = new Topic(reader);
				}
				if (reader.NextResult())
				{
					// Forum
					if (reader.Read())
					{
						topic.Forum = new Forum(reader);
					}
				}
				if (reader.NextResult())
				{
					// Roles
					while (reader.Read())
					{
						topic.Forum.RequiredRoles.Add(reader.GetString("RoleName"));
					}
				}
				if (reader.NextResult())
				{
					// Board
					if (reader.Read())
					{
						topic.Forum.Board = new Board(reader);
					}
				}
			}
			return topic;
		}

		public override int GetTopicsCount(int forumId)
		{
			object obj = SqlHelper.ExecuteScalar(ConnectionString, "dbo.lg_Forums_GetTopicsCount",
				SqlHelper.CreateInputParam("@ForumId", SqlDbType.Int, forumId));
			if (obj != null && obj != DBNull.Value)
			{
				return Convert.ToInt32(obj);
			}
			return 0;
		}

		public override int GetRepliesCount(int topicId)
		{
			object obj = SqlHelper.ExecuteScalar(ConnectionString, "dbo.lg_Forums_GetRepliesCount",
				SqlHelper.CreateInputParam("@TopicId", SqlDbType.Int, topicId));
			if (obj != null && obj != DBNull.Value)
			{
				return Convert.ToInt32(obj);
			}
			return 0;
		}

		public override List<Post> GetTopPosts(int boardId, int count)
		{
			List<Post> list = new List<Post>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(ConnectionString, "dbo.lg_Forums_GetTopPosts",
				SqlHelper.CreateInputParam("@BoardId", SqlDbType.Int, boardId),
				SqlHelper.CreateInputParam("@Count", SqlDbType.Int, count)))
			{
				while (reader.Read())
				{
					list.Add(new Post(reader));
				}
			}
			return list;
		}

		public override List<Topic> GetTopTopics(int boardId, int count)
		{
			List<Topic> list = new List<Topic>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(ConnectionString, "dbo.lg_Forums_GetTopTopics",
				SqlHelper.CreateInputParam("@BoardId", SqlDbType.Int, boardId),
				SqlHelper.CreateInputParam("@Count", SqlDbType.Int, count)))
			{
				while (reader.Read())
				{
					list.Add(new Topic(reader));
				}
			}
			return list;
		}

		public override void UpdateTopicViewCount(int topicId)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, "dbo.lg_Forums_UpdateTopicViewCount",
				SqlHelper.CreateInputParam("@TopicId", SqlDbType.Int, topicId));
		}
	}
}
