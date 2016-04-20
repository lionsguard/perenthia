using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

using Lionsguard.Forums;

namespace Lionsguard.Providers
{
	public abstract class ForumProvider : ProviderBase
	{
		public abstract List<Board> GetBoards();
		public abstract List<Forum> GetForums();
		public abstract List<Forum> GetForums(Board board);
		public abstract List<Topic> GetTopics(Forum forum, int startingRowIndex, int maxRows);
		public abstract List<Reply> GetReplies(Topic topic, int startingRowIndex, int maxRows);

		public abstract int GetTopicsCount(int forumId);
		public abstract int GetRepliesCount(int topicId);

		public abstract void UpdateTopicViewCount(int topicId);

		public abstract List<Post> GetTopPosts(int boardId, int count);
		public abstract List<Topic> GetTopTopics(int boardId, int count);

		public abstract Board GetBoard(int id);
		public abstract Forum GetForum(int id);
		public abstract Topic GetTopic(int id);

		public abstract void SaveTopic(Topic topic, Forum forum, int userId, string ipAddress);
		public abstract void SaveReply(Reply reply, Topic topic, int userId, string ipAddress);
	}
}
