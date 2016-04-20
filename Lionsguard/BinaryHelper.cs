using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lionsguard
{
	public static class BinaryHelper
	{
		public static byte[] ToBinary(this object obj)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			byte[] buffer = new byte[0];
			using (MemoryStream ms = new MemoryStream())
			{
				formatter.Serialize(ms, obj);
				ms.Seek(0, SeekOrigin.Begin);
				ms.Read(buffer, 0, (int)ms.Length);
			}
			return buffer;
		}

		//public static T FromBinary<T>(byte[] buffer)
		//{
		//    BinaryFormatter formatter = new BinaryFormatter();
		//    using (MemoryStream ms = new MemoryStream())
		//    {
		//        ms.Write(buffer, 0, buffer.Length);
		//        return formatter.Deserialize(ms) as T;
		//    }
		//}
	}
}
