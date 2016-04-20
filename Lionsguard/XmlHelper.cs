using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Lionsguard
{
	public static class XmlHelper
	{
		public static string ToXml(this object obj)
		{
			XmlSerializer serializer = new XmlSerializer(obj.GetType());
			using (StringWriter sw = new StringWriter())
			{
				serializer.Serialize(sw, obj);
				return sw.ToString();
			}
		}

		public static object FromXml(XmlReader reader, Type objectType)
		{
			XmlSerializer serializer = new XmlSerializer(objectType);
			return serializer.Deserialize(reader);
		}

		public static T FromXml<T>(XmlReader reader)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			return (T)serializer.Deserialize(reader);
		}

		public static T FromXml<T>(string xml)
		{
			using (StringReader sr = new StringReader(xml))
			{
				using (XmlReader reader = new XmlTextReader(sr))
				{
					return (T)FromXml(reader, typeof(T));
				}
			}
		}
	}
}
