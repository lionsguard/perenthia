using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Configuration;

using Lionsguard.Configuration;
using Lionsguard.Data;
using Lionsguard.Providers;

namespace Lionsguard.Forums
{
	[DataObject]
	public static class ForumManager
	{
		#region Properties
		private static ForumProvider _provider = null;
		private static ForumProviderCollection _providers = null;
		private static string _boardName = null;

		public static ForumProvider Provider
		{
			get
			{
				Initialize();
				return _provider;
			}
		}

		public static ForumProviderCollection Providers
		{
			get
			{
				Initialize();
				return _providers;
			}
		}

		public static string BoardName
		{
			get
			{
				Initialize();
				return _boardName;
			}
		}
		#endregion

		#region Initialize
		private static bool _initialized = false;
		private static object _lock = new object();
		private static Exception _initException = null;

		private static void Initialize()
		{
			if (!_initialized)
			{
				lock (_lock)
				{
					if (!_initialized)
					{
						try
						{
							ForumSection section = ConfigurationManager.GetSection("lionsguard/forum") as ForumSection;
							if (section != null)
							{
								_providers = new ForumProviderCollection();
								ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof(ForumProvider));
								_provider = _providers[section.DefaultProvider];
								if (_provider == null)
								{
									throw new ConfigurationErrorsException("Default ForumProvider not found in application configuration file.", section.ElementInformation.Properties["defaultProvider"].Source, section.ElementInformation.Properties["defaultProvider"].LineNumber);
								}

								if (!String.IsNullOrEmpty(section.BoardName))
								{
									_boardName = section.BoardName;
								}
							}
						}
						catch (Exception ex)
						{
							_initException = ex;
						}
						_initialized = true;
					}
				}
			}
			if (_initException != null)
			{
				throw _initException;
			}
		}
		#endregion

		public static Board GetBoard(int id)
		{
			return Provider.GetBoard(id);
		}

		[DataObjectMethod(DataObjectMethodType.Select, false)]
		public static List<Board> GetBoards()
		{
			return Provider.GetBoards();
		}

		public static List<Forum> GetForums()
		{
			return Provider.GetForums();
		}

		public static Forum GetForum(int id)
		{
			return Provider.GetForum(id);
		}

		public static List<Forum> GetForums(Board board)
		{
			return Provider.GetForums(board);
		}

		[DataObjectMethod(DataObjectMethodType.Select, false)]
		public static List<Forum> GetForums(int boardId)
		{
			return Provider.GetForums(new Board { ID = boardId });
		}

		public static Topic GetTopic(int id)
		{
			return Provider.GetTopic(id);
		}

		[DataObjectMethod(DataObjectMethodType.Select, false)]
		public static int GetTopicsCount(int forumId)
		{
			return Provider.GetTopicsCount(forumId);
		}

		public static List<Topic> GetTopics(Forum forum, int startingRowIndex, int maxRows)
		{
			return Provider.GetTopics(forum, startingRowIndex, maxRows);
		}

		[DataObjectMethod(DataObjectMethodType.Select, false)]
		public static List<Topic> GetTopics(int forumId, int startingRowIndex, int maxRows)
		{
			return Provider.GetTopics(new Forum { ID = forumId }, startingRowIndex, maxRows);
		}

		[DataObjectMethod(DataObjectMethodType.Select, false)]
		public static int GetRepliesCount(int topicId)
		{
			return Provider.GetRepliesCount(topicId);
		}

		public static List<Reply> GetReplies(Topic topic, int startingRowIndex, int maxRows)
		{
			return Provider.GetReplies(topic, startingRowIndex, maxRows);
		}

		[DataObjectMethod(DataObjectMethodType.Select, false)]
		public static List<Reply> GetReplies(int topicId, int startingRowIndex, int maxRows)
		{
			return Provider.GetReplies(new Topic { ID = topicId }, startingRowIndex, maxRows);
		}

		[DataObjectMethod(DataObjectMethodType.Select, false)]
		public static List<Post> GetTopPosts(int boardId, int count)
		{
			return Provider.GetTopPosts(boardId, count);
		}

		[DataObjectMethod(DataObjectMethodType.Select, false)]
		public static List<Topic> GetTopTopics(int boardId, int count)
		{
			return Provider.GetTopTopics(boardId, count);
		}

		public static void SaveTopic(Topic topic, Forum forum, int userId, string ipAddress)
		{
			Provider.SaveTopic(topic, forum, userId, ipAddress);
		}

		public static void SaveReply(Reply reply, Topic topic, int userId, string ipAddress)
		{
			Provider.SaveReply(reply, topic, userId, ipAddress);
		}

		public static void UpdateTopicViewCount(int topicId)
		{
			Provider.UpdateTopicViewCount(topicId);
		}
	}
}
