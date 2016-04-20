using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

using Lionsguard.Content;

namespace Lionsguard.Providers
{
	public abstract class ContentProvider : ProviderBase
	{
		public abstract List<Source> GetSources();
		public abstract List<Category> GetCategories();
		public abstract List<Category> GetCategories(string sourceName);

		public abstract List<Post> GetPosts(int startingRowIndex, int maxRows);
		public abstract List<Post> GetPosts(int year, int month, string categoryName);
		public abstract List<Post> GetPosts(string categoryName, int startingRowIndex, int maxRows);

		public abstract Post GetPost(int postId);

		public abstract int GetPostCount();

		public abstract List<Archive> GetArchives(string categoryName);

		public abstract List<Comment> GetComments(Post post, int startingRowIndex, int maxRows);

		public abstract void SaveSource(Source source);
		public abstract void SaveCategory(Category category);
		public abstract void SaveCategory(Category category, params Source[] sources);
		public abstract void SavePost(Post post, int userId);
		public abstract void SavePost(Post post, int userId, params Source[] sources);
		public abstract void SaveComment(Comment comment, Post post, int userId);

		public abstract bool IsSafeWord(string word);
		public abstract bool IsSafeText(string text);
	}
}
