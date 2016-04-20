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

namespace Lionsguard.Content
{
	[DataObject]
	public static class ContentManager
	{
		#region Properties
		private static ContentProvider _provider = null;
		private static ContentProviderCollection _providers = null;
		private static string _sourceName = null;

		public static ContentProvider Provider
		{
			get
			{
				Initialize();
				return _provider;
			}
		}

		public static ContentProviderCollection Providers
		{
			get
			{
				Initialize();
				return _providers;
			}
		}

		public static string SourceName
		{
			get
			{
				Initialize();
				return _sourceName;
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
							ContentSection section = ConfigurationManager.GetSection("lionsguard/content") as ContentSection;
							if (section != null)
							{
								_providers = new ContentProviderCollection();
								ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof(ContentProvider));
								_provider = _providers[section.DefaultProvider];
								if (_provider == null)
								{
									throw new ConfigurationErrorsException("Default ContentProvider not found in application configuration file.", section.ElementInformation.Properties["defaultProvider"].Source, section.ElementInformation.Properties["defaultProvider"].LineNumber);
								}

								if (!String.IsNullOrEmpty(section.SourceName))
								{
									_sourceName = section.SourceName;
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

		public static void ChangeSource(string sourceName)
		{
			_sourceName = sourceName;
		}

		[DataObjectMethod(DataObjectMethodType.Select, true)]
		public static List<Post> GetPosts(string sourceName, int startingRowIndex, int maxRows)
		{
			_sourceName = sourceName;
			return ContentManager.Provider.GetPosts(startingRowIndex, maxRows);
		}

		public static int GetPostCount(string sourceName)
		{
			_sourceName = sourceName;
			return ContentManager.Provider.GetPostCount();
		}

		public static Post GetTopPost(string categoryName)
		{
			List<Post> list = Provider.GetPosts(categoryName, 0, 1);
			if (list.Count > 0)
			{
				return list[0];
			}
			return new Post();
		}

		public static List<Post> GetTopPosts(string categoryName, int count)
		{
			return Provider.GetPosts(categoryName, 0, count);
		}

		public static bool IsWordSafe(string word)
		{
			return Provider.IsSafeWord(word);
		}

		public static bool IsTextSafe(string text)
		{
			return Provider.IsSafeText(text);
		}
	}
}
