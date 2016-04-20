using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Lionsguard.Data;
using Lionsguard.Content;
using Lionsguard.Providers;

namespace Lionsguard.Providers
{
	public class SqlContentProvider : ContentProvider
	{
		private string _connectionString;
		private object _lock = new object();
		private List<string> _restrictedWords = new List<string>();

		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			base.Initialize(name, config);

			_connectionString = ProviderUtil.GetConnectionString(config);

			ProviderUtil.CheckUnrecognizedAttributes(config);
		}

		public override List<Source> GetSources()
		{
			List<Source> list = new List<Source>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Content_GetSources"))
			{
				while (reader.Read())
				{
					list.Add(new Source(reader));
				}
			}
			return list;
		}

		public override List<Category> GetCategories()
		{
			List<Category> list = new List<Category>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Content_GetCategories"))
			{
				while (reader.Read())
				{
					list.Add(new Category(reader));
				}
			}
			return list;
		}

		public override List<Category> GetCategories(string sourceName)
		{
			List<Category> list = new List<Category>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Content_GetCategoriesBySource",
				SqlHelper.CreateInputParam("@SourceName", SqlDbType.NVarChar, sourceName)))
			{
				while (reader.Read())
				{
					list.Add(new Category(reader));
				}
			}
			return list;
		}

		public override List<Post> GetPosts(int startingRowIndex, int maxRows)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Content_GetPosts",
				SqlHelper.CreateInputParam("@SourceName", SqlDbType.NVarChar, ContentManager.SourceName),
				SqlHelper.CreateInputParam("@StartRowIndex", SqlDbType.Int, startingRowIndex),
				SqlHelper.CreateInputParam("@MaxRows", SqlDbType.Int, maxRows)))
			{
				return this.GetPostsFromReader(reader);
			}
		}

		public override List<Post> GetPosts(int year, int month, string categoryName)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Content_GetPostsByMonthYear",
				SqlHelper.CreateInputParam("@SourceName", SqlDbType.NVarChar, ContentManager.SourceName),
				SqlHelper.CreateInputParam("@CategoryName", SqlDbType.NVarChar, categoryName),
				SqlHelper.CreateInputParam("@Year", SqlDbType.Int, year),
				SqlHelper.CreateInputParam("@Month", SqlDbType.Int, month)))
			{
				return this.GetPostsFromReader(reader);
			}
		}

		public override List<Post> GetPosts(string categoryName, int startingRowIndex, int maxRows)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Content_GetPostsByCategory",
				SqlHelper.CreateInputParam("@SourceName", SqlDbType.NVarChar, ContentManager.SourceName),
				SqlHelper.CreateInputParam("@CategoryName", SqlDbType.NVarChar, categoryName),
				SqlHelper.CreateInputParam("@StartRowIndex", SqlDbType.Int, startingRowIndex),
				SqlHelper.CreateInputParam("@MaxRows", SqlDbType.Int, maxRows)))
			{
				return this.GetPostsFromReader(reader);
			}
		}

		public override Post GetPost(int postId)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Content_GetPost",
				SqlHelper.CreateInputParam("@ContentPostId", SqlDbType.Int, postId)))
			{
				List<Post> posts = this.GetPostsFromReader(reader);
				if (posts != null && posts.Count > 0)
				{
					return posts[0];
				}
				return new Post();
			}
		}

		public override int GetPostCount()
		{
			return Convert.ToInt32(SqlHelper.ExecuteScalar(_connectionString, "dbo.lg_Content_GetPostCount", 
				SqlHelper.CreateInputParam("@SourceName", SqlDbType.NVarChar, ContentManager.SourceName)));
		}

		private List<Post> GetPostsFromReader(SqlNullDataReader reader)
		{
			List<Post> list = new List<Post>();
			while (reader.Read())
			{
				list.Add(new Post(reader));
			}
			// Categories
			if (reader.NextResult())
			{
				while (reader.Read())
				{
					Post post = list.Find(new Predicate<Post>(delegate(Post p) { return p.ID == reader.GetInt32("ContentPostId"); }));
					if (post != null)
					{
						post.Categories.Add(new Category(reader));
					}
				}
			}
			// Tags
			if (reader.NextResult())
			{
				while (reader.Read())
				{
					Post post = list.Find(new Predicate<Post>(delegate(Post p) { return p.ID == reader.GetInt32("ContentPostId"); }));
					if (post != null)
					{
						post.Tags.Add(reader.GetString("TagName"));
					}
				}
			}
			return list;
		}

		public override List<Archive> GetArchives(string categoryName)
		{
			List<Archive> list = new List<Archive>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Content_GetArchives",
				SqlHelper.CreateInputParam("@SourceName", SqlDbType.NVarChar, ContentManager.SourceName),
				SqlHelper.CreateInputParam("@CategoryName", SqlDbType.NVarChar, categoryName)))
			{
				while (reader.Read())
				{
					list.Add(new Archive(reader));
				}
			}
			return list;
		}

		public override List<Comment> GetComments(Post post, int startingRowIndex, int maxRows)
		{
			List<Comment> list = new List<Comment>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Content_GetComments",
				SqlHelper.CreateInputParam("@ContentPostId", SqlDbType.Int, post.ID)))
			{
				while (reader.Read())
				{
					list.Add(new Comment(reader));
				}
			}
			return list;
		}

		public override void SaveSource(Source source)
		{
			using (SqlCommand cmd = SqlHelper.ExecuteNonQuery(_connectionString, "dbo.lg_Content_SaveSource",
				SqlHelper.CreateInputParam("@SourceName", SqlDbType.NVarChar, source.Name),
				SqlHelper.CreateInputParam("@Url", SqlDbType.NVarChar, source.Url),
				SqlHelper.CreateInputParam("@Description", SqlDbType.NVarChar, source.Description),
				SqlHelper.CreateInputOutputParam("@ContentSourceId", SqlDbType.Int, source.ID)))
			{
				source.ID = Convert.ToInt32(cmd.Parameters["@ContentSourceId"].Value);
			}
		}

		public override void SaveCategory(Category category)
		{
			using (SqlCommand cmd = SqlHelper.ExecuteNonQuery(_connectionString, "dbo.lg_Content_SaveCategory",
				SqlHelper.CreateInputParam("@CategoryName", SqlDbType.NVarChar, category.Name),
				SqlHelper.CreateInputOutputParam("@ContentCategoryId", SqlDbType.Int, category.ID)))
			{
				category.ID = Convert.ToInt32(cmd.Parameters["@ContentCategoryId"].Value);
			}
		}

		public override void SaveCategory(Category category, params Source[] sources)
		{
			using (SqlCommand cmd = SqlHelper.ExecuteNonQuery(_connectionString, "dbo.lg_Content_SaveCategory",
				SqlHelper.CreateInputParam("@CategoryName", SqlDbType.NVarChar, category.Name),
				SqlHelper.CreateInputOutputParam("@ContentCategoryId", SqlDbType.Int, category.ID)))
			{
				category.ID = Convert.ToInt32(cmd.Parameters["@ContentCategoryId"].Value);
			}

			if (sources != null && sources.Length > 0)
			{
				foreach (Source source in sources)
				{
					SqlHelper.ExecuteNonQuery(_connectionString, "dbo.lg_Content_AddCategoryToSource",
						SqlHelper.CreateInputParam("@SourceName", SqlDbType.NVarChar, source.Name),
						SqlHelper.CreateInputParam("@CategoryId", SqlDbType.Int, category.ID));
				}
			}
		}

		public override void SavePost(Post post, int userId)
		{
			this.SavePostInternal(post, new Source[] { new Source { Name = ContentManager.SourceName } }, userId);
		}

		public override void SavePost(Post post, int userId, params Source[] sources)
		{
			this.SavePostInternal(post, sources, userId);
		}

		private void SavePostInternal(Post post, Source[] sources, int userId)
		{
			using (SqlCommand cmd = SqlHelper.ExecuteNonQuery(_connectionString, "dbo.lg_Content_SavePost",
				SqlHelper.CreateInputParam("@Title", SqlDbType.NVarChar, post.Title),
				SqlHelper.CreateInputParam("@PostText", SqlDbType.NVarChar, post.Text),
				SqlHelper.CreateInputParam("@AuthorUserId", SqlDbType.Int, userId),
				SqlHelper.CreateInputParam("@DateCreated", SqlDbType.DateTime, post.DateCreated),
				SqlHelper.CreateInputParam("@IsPublished", SqlDbType.Bit, post.IsPublished),
				SqlHelper.CreateInputParam("@IsCommentsEnabled", SqlDbType.Bit, post.IsCommentsEnabled),
				SqlHelper.CreateInputOutputParam("@ContentPostId", SqlDbType.Int, post.ID)))
			{
				post.ID = Convert.ToInt32(cmd.Parameters["@ContentPostId"].Value);
			}

			if (sources != null && sources.Length > 0)
			{
				foreach (Source source in sources)
				{
					SqlHelper.ExecuteNonQuery(_connectionString, "dbo.lg_Content_AddPostToSource",
						SqlHelper.CreateInputParam("@SourceName", SqlDbType.NVarChar, source.Name),
						SqlHelper.CreateInputParam("@ContentPostId", SqlDbType.Int, post.ID));
				}
				
			}

			// Categories
			foreach (var cat in post.Categories)
			{
				SqlHelper.ExecuteNonQuery(_connectionString, "dbo.lg_Content_AddCategoryToPost",
						SqlHelper.CreateInputParam("@CategoryName", SqlDbType.NVarChar, cat.Name),
						SqlHelper.CreateInputParam("@ContentPostId", SqlDbType.Int, post.ID));
			}

			// Tags
			foreach (var tag in post.Tags)
			{
				SqlHelper.ExecuteNonQuery(_connectionString, "dbo.lg_Content_AddTagToPost",
						SqlHelper.CreateInputParam("@TagName", SqlDbType.NVarChar, tag),
						SqlHelper.CreateInputParam("@ContentPostId", SqlDbType.Int, post.ID));
			}
		}

		public override void SaveComment(Comment comment, Post post, int userId)
		{
			using (SqlCommand cmd = SqlHelper.ExecuteNonQuery(_connectionString, "dbo.lg_Content_SaveComment",
				SqlHelper.CreateInputParam("@ContentPostId", SqlDbType.Int, post.ID),
				SqlHelper.CreateInputParam("@CommentText", SqlDbType.NVarChar, comment.Text),
				SqlHelper.CreateInputParam("@AuthorUserId", SqlDbType.Int, userId),
				SqlHelper.CreateInputParam("@DateCreated", SqlDbType.DateTime, comment.DateCreated),
				SqlHelper.CreateInputOutputParam("@ContentCommentId", SqlDbType.Int, comment.ID)))
			{
				comment.ID = Convert.ToInt32(cmd.Parameters["@ContentCommentId"].Value);
			}
		}

		public override bool IsSafeWord(string word)
		{
			this.EnsureRestrictedWords();
			return !_restrictedWords.Contains(word.ToLower());
		}

		public override bool IsSafeText(string text)
		{
			this.EnsureRestrictedWords();
			string[] words = text.Split(' ');
			if (words != null && words.Length > 0)
			{
				foreach (var word in words)
				{
					if (_restrictedWords.Contains(word.ToLower())) return false;
				}
			}
			return true;
		}

		private void EnsureRestrictedWords()
		{
			if (_restrictedWords.Count == 0)
			{
				lock (_lock)
				{
					if (_restrictedWords.Count == 0)
					{
						using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "SELECT Word FROM dbo.lg_RestrictedWords", CommandType.Text))
						{
							while (reader.Read())
							{
								_restrictedWords.Add(reader.GetString(0));
							}
						}
					}
				}
			}
		}
	}
}
