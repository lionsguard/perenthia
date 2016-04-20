using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Radiance
{
	public static class JsonHelper
	{
		public static string ToJson(this object obj)
		{
			DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
			using (MemoryStream ms = new MemoryStream())
			{
				serializer.WriteObject(ms, obj);
				ms.Seek(0, SeekOrigin.Begin);
				using (StreamReader reader = new StreamReader(ms))
				{
					return reader.ReadToEnd();
				}
			}
		}

		public static T FromJson<T>(string json)
		{
			DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
			using (MemoryStream ms = new MemoryStream())
			{
				using (StreamWriter writer = new StreamWriter(ms))
				{
					writer.Write(json);
					writer.Flush();
					ms.Seek(0, SeekOrigin.Begin);
					return (T)serializer.ReadObject(ms);
				}
			}
		}
	}
}
