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
	public class YafForumProvider : SqlForumProvider
	{
		public override List<Post> GetTopPosts(int boardId, int count)
		{
			List<Post> list = new List<Post>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(this.ConnectionString, "dbo.yaf_topic_latest",
				SqlHelper.CreateInputParam("@BoardID", SqlDbType.Int, 1),
				SqlHelper.CreateInputParam("@NumPosts", SqlDbType.Int, count),
				SqlHelper.CreateInputParam("@UserID", SqlDbType.Int, 2)))
			{
				while (reader.Read())
				{
					//LastPosted              
					//ForumID     
					//Forum                                              
					//Topic                                                                                                
					//TopicID     
					//LastMessageID 
					//LastUserID  
					//NumPosts    
					//LastUserName
					// Views
					list.Add(new Post()
						{
							ID = reader.GetInt32("TopicID"),
							ForumName = reader.GetString("Forum"),
							DisplayName = reader.GetString("LastUserName"),
							ParentID = reader.GetInt32("ForumID"),
							PostDate = reader.GetDateTime("LastPosted"),
							Title = reader.GetString("Forum")
						});
				}
			}
			return list;
		}

		public override List<Topic> GetTopTopics(int boardId, int count)
		{
			List<Topic> list = new List<Topic>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(this.ConnectionString, "dbo.yaf_topic_latest",
				SqlHelper.CreateInputParam("@BoardID", SqlDbType.Int, 1),
				SqlHelper.CreateInputParam("@NumPosts", SqlDbType.Int, count),
				SqlHelper.CreateInputParam("@UserID", SqlDbType.Int, 2)))
			{
				while (reader.Read())
				{
					//LastPosted              
					//ForumID     
					//Forum                                              
					//Topic                                                                                                
					//TopicID     
					//LastMessageID 
					//LastUserID  
					//NumPosts    
					//LastUserName
					// Views
					list.Add(new Topic()
					{
						ID = reader.GetInt32("TopicID"),
						ForumName = reader.GetString("Forum"),
						DisplayName = reader.GetString("LastUserName"),
						ParentID = reader.GetInt32("ForumID"),
						PostDate = reader.GetDateTime("LastPosted"),
						Title = reader.GetString("Topic"),
						LastAuthorDisplayName = reader.GetString("LastUserName"),
						ReplyCount = reader.GetInt32("NumPosts"),
						ViewCount = reader.GetInt32("Views")
					});
				}
			}
			return list;
		}
	}
}
