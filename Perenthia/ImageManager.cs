using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace Perenthia
{
	public static class ImageManager
	{
		private static Dictionary<string, ImageSource> _images = new Dictionary<string, ImageSource>(StringComparer.InvariantCultureIgnoreCase);

		public static void Load(byte[] data)
		{
			StreamResourceInfo contents = Application.GetResourceStream(new StreamResourceInfo(new MemoryStream(data), null), new Uri("contents.txt", UriKind.Relative));
			using (StreamReader reader = new StreamReader(contents.Stream))
			{
				string line = null;
				while ((line = reader.ReadLine()) != null)
				{
					StreamResourceInfo src = Application.GetResourceStream(new StreamResourceInfo(new MemoryStream(data), null), new Uri(line, UriKind.Relative));
					BitmapImage img = new BitmapImage();
					img.SetSource(src.Stream);
					if (!_images.ContainsKey(line))
						_images.Add(line, img);
				}
			}
		}

		public static ImageSource GetImageSource(string sourceUri)
		{
			if (!String.IsNullOrEmpty(sourceUri))
			{
				if (!sourceUri.StartsWith("http"))
				{
					string sep = String.Empty;
					if (!sourceUri.StartsWith("/")) sep = "/";
					sourceUri = String.Concat(Settings.MediaUri, sep, sourceUri);
				}
				return Asset.GetImageSource(sourceUri);
			}
			//if (!String.IsNullOrEmpty(sourceUri) && _images.ContainsKey(sourceUri))
			//{
			//    return _images[sourceUri];
			//}
			return null;
		}
	}
}
