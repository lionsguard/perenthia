using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;

using Radiance;
using Radiance.Markup;
using Radiance.Serialization;
using System.Runtime.Serialization;
using System.Xml;

namespace Perenthia
{
    public static class StorageManager
    {
        public static readonly long RequiredQuota = 1048576;
        public static readonly string RootDirectory = "perenthia";
        public static readonly string MapsDirectory = "maps";
        public static readonly string UserDirectory = "user";
        private static readonly string FileUpdateFileName = "updates";
		private static readonly string DeclineFileName = "decline";
		private static readonly string ErrorFileName = "errors";
		private static readonly string PersistLoginTokenFileName = "login";
		private static readonly string SettingsFileName = "settings";

        private static Dictionary<string, bool> FileUpdates = new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);

        static StorageManager()
        {
            RequiredQuota = 1048576 * 25;
        }

        public static bool HasRequiredQuota()
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return store.Quota == RequiredQuota;
            }
        }

		public static bool HasDeclinedIncrease()
		{
			byte[] buffer = Read(RootDirectory, DeclineFileName);
			if (buffer != null && buffer.Length > 0)
			{
				return Boolean.Parse(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
			}
			return false;
		}

		public static void AcceptIncrease()
		{
			Write(RootDirectory, DeclineFileName, Encoding.UTF8.GetBytes(Boolean.FalseString));
		}

		public static void DeclineIncrease()
		{
			Write(RootDirectory, DeclineFileName, Encoding.UTF8.GetBytes(Boolean.TrueString));
		}

		public static string GetPersistLoginToken()
		{
			var data = Read(RootDirectory, PersistLoginTokenFileName);
			if (data != null && data.Length > 0)
				return Encoding.UTF8.GetString(data, 0, data.Length);
			else
				return String.Empty;
		}
		public static void SetPersistLoginToken(string token)
		{
			Write(RootDirectory, PersistLoginTokenFileName, Encoding.UTF8.GetBytes(token));
		}

        public static void LoadFileUpdatesFromServer(List<RdlTag> serverTags)
        {
            // Load the files updates from disk.
            RdlTagCollection localTagCollection = ReadTags(FileUpdateFileName);
            var localTags = localTagCollection.GetTags<RdlTag>("FILEUPDATE", "FILEUPDATE");
            FileUpdates.Clear();
            foreach (var serverTag in serverTags)
            {
                bool requiresUpdate = true;
                string fileName = serverTag.GetArg<string>(0);
                // Loop through all of the server tags, attempt to find a matching local tag by file name.
                var matchingLocalTag = localTags.Where(t => t.GetArg<string>(0) == fileName).FirstOrDefault();
                if (matchingLocalTag != null)
                {
                    // If a matching local tags exists compare the dates, if they don't match this file requires an update.
                    DateTime localDt = new DateTime(matchingLocalTag.GetArg<long>(1));
                    DateTime serverDt = new DateTime(serverTag.GetArg<long>(1));
                    if (localDt.CompareTo(serverDt) == 0)
					{
						requiresUpdate = false;
						if (!FileExists(RootDirectory, fileName))
						{
							if (!FileExists(GetPath(MapsDirectory), fileName))
							{
								if (!FileExists(GetPath(UserDirectory), fileName))
								{
									// Could not find any version or form of the file.
									requiresUpdate = true;
								}
							}
						}
                    }
                }

                if (FileUpdates.ContainsKey(fileName))
                {
                    FileUpdates[fileName] = requiresUpdate;
                }
                else
                {
                    FileUpdates.Add(fileName, requiresUpdate);
                }
            }

            // Write the values from the server to disk.
            WriteTags(FileUpdateFileName, serverTags.ToTagCollection());
        }

        public static bool RequiresFileUpdate(string fileName)
        {
            if (FileUpdates.ContainsKey(fileName))
            {
                return FileUpdates[fileName];
            }
            return true;
        }

        public static string GetLastMap(string playerName)
        {
            byte[] buffer = Read(GetPath(UserDirectory), playerName);
            if (buffer != null && buffer.Length > 0)
            {
                RdlActor player = RdlActor.FromBytes(buffer) as RdlActor;
                if (player != null)
                {
                    return player.Properties.GetValue<string>("Zone");
                }
            }
            return String.Empty;
        }

        public static void SavePlayer(RdlActor player)
        {
            Write(GetPath(UserDirectory), player.Name, player.ToBytes());
        }

		public static UISettings GetUISettings()
		{
			var buffer = Read(RootDirectory, SettingsFileName);
			if (buffer == null || buffer.Length == 0)
				return new UISettings();

			using (var ms = new MemoryStream(buffer))
			{
				using (var reader = XmlReader.Create(ms))
				{
					var settings = new UISettings();
					settings.ReadXml(reader);
					return settings;
				}
			}
		}

		public static void SaveUISettings(UISettings settings)
		{
			using (var ms = new MemoryStream())
			{
				using (var writer = XmlWriter.Create(ms))
				{
					settings.WriteXml(writer);
				}
				var buffer = new byte[ms.Length];
				ms.Seek(0, SeekOrigin.Begin);
				ms.Read(buffer, 0, buffer.Length);
				Write(RootDirectory, SettingsFileName, buffer);
			}
		}

        private static string GetPath(string directory)
        {
            return System.IO.Path.Combine(RootDirectory, directory);
        }

        private static string FormatFileName(string name)
        {
            string fileName = name.Replace(" ", String.Empty).Replace(".", "-").Replace("\\", "_").Replace("'", "-");
            if (fileName.Length > 55) fileName = fileName.Substring(0, 55);

            return fileName;
        }

        public static RdlTagCollection ReadMap(string mapName)
        {
            return RdlTagCollection.FromBytes(Read(GetPath(MapsDirectory), mapName));
        }

        public static RdlTagCollection ReadTags(string fileName)
        {
            return RdlTagCollection.FromBytes(Read(RootDirectory, fileName));
        }

        private static byte[] Read(string directory, string fileName)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!store.DirectoryExists(directory))
                {
                    store.CreateDirectory(directory);
                }

                string path = System.IO.Path.Combine(directory, FormatFileName(fileName));
                if (store.FileExists(path))
                {
                    try
                    {
                        using (IsolatedStorageFileStream file = store.OpenFile(path, FileMode.Open, FileAccess.Read))
                        {
                            int length = 1048576; // 1MB
                            if (file.Length < Int32.MaxValue) length = (int)file.Length;

                            byte[] buffer = new byte[length];
                            file.Read(buffer, 0, length);
							if (buffer.Length == 0 && FileUpdates.ContainsKey(fileName))
							{
                                FileUpdates[fileName] = true;
							}
                            return buffer;
                        }
                    }
                    catch (IsolatedStorageException)
                    {
#if DEBUG
                        // Could not access store?
                        throw;
#endif
                    }
                }
                return new byte[0];
            }
        }

		private static bool FileExists(string directory, string fileName)
		{
			using (var store = IsolatedStorageFile.GetUserStoreForApplication())
			{
				if (!store.DirectoryExists(directory))
				{
					store.CreateDirectory(directory);
				}

				string path = System.IO.Path.Combine(directory, FormatFileName(fileName));
				return store.FileExists(path);
			}
		}

        public static void WriteMap(string mapName, RdlTagCollection tags)
        {
            Write(GetPath(MapsDirectory), mapName, tags.ToBytes());
        }

        public static void WriteTags(string fileName, RdlTagCollection tags)
        {
            Write(RootDirectory, fileName, tags.ToBytes());
        }

		public static string GetAndClearErrors()
		{
			var data = Read(RootDirectory, ErrorFileName);
			if (data != null && data.Length > 0)
			{
				var error = Encoding.UTF8.GetString(data, 0, data.Length);
				ClearError();
				return error;
			}
			return null;
		}

		public static void WriteError(string text)
		{
			Write(RootDirectory, ErrorFileName, Encoding.UTF8.GetBytes(text));
		}
		private static void ClearError()
		{
			using (var store = IsolatedStorageFile.GetUserStoreForApplication())
			{
				if (!store.DirectoryExists(RootDirectory))
				{
					store.CreateDirectory(RootDirectory);
				}

				string path = System.IO.Path.Combine(RootDirectory, FormatFileName(ErrorFileName));
				if (!store.FileExists(path))
				{
					store.DeleteFile(path);
				}
			}
		}

        private static void Write(string directory, string fileName, byte[] data)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!store.DirectoryExists(directory))
                {
                    store.CreateDirectory(directory);
                }

                string path = System.IO.Path.Combine(directory, FormatFileName(fileName));
                if (!store.FileExists(path))
                {
                    IsolatedStorageFileStream newFile = store.CreateFile(path);
                    newFile.Close();
                }

                if (store.FileExists(path))
                {
                    try
                    {
                        using (IsolatedStorageFileStream file = store.OpenFile(path, FileMode.Truncate, FileAccess.Write))
                        {
                            file.Write(data, 0, data.Length);
                        }
                        if (FileUpdates.ContainsKey(fileName))
                        {
                            FileUpdates[fileName] = false;
                        }
                    }
                    catch (IsolatedStorageException)
                    {
#if DEBUG
                        // Could not access store?
                        throw;
#endif
                    }
                }
            }
        }
    }
}
