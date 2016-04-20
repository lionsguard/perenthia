using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Radiance.Serialization
{
	/// <summary>
	/// Provides static methods for serializing and deserializing binary data.
	/// </summary>
	public static class BinarySerializer
	{
		/// <summary>
		/// Serializes the specified object into a byte array.
		/// </summary>
		/// <typeparam name="T">The System.Type that implements IBinarySerializable.</typeparam>
		/// <param name="obj">The object to serialize.</param>
		/// <returns>A byte array of the serialized object.</returns>
		public static byte[] Serialize<T>(T obj) where T : IBinarySerializable
		{
			using (MemoryStream ms = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(ms))
				{
					obj.Write(writer);
					return ms.GetBuffer();
				}
			}
		}

		/// <summary>
		/// Serializes the specified object array into a byte array.
		/// </summary>
		/// <typeparam name="T">The System.Type that implements IBinarySerializable.</typeparam>
		/// <param name="list">The list of objects to serialize.</param>
		/// <returns>A byte array of the serialized objects.</returns>
		public static byte[] Serialize<T>(IEnumerable<T> list) where T : IBinarySerializable
		{
			using (MemoryStream ms = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(ms))
				{
					writer.Write(list.Count());
					foreach (var item in list)
					{
						item.Write(writer);
					}
					return ms.GetBuffer();
				}
			}
		}

		/// <summary>
		/// Deserializes the specified byte array into an object of type T.
		/// </summary>
		/// <typeparam name="T">The System.Type that implements IBinarySerializable.</typeparam>
		/// <param name="buffer">The byte array to deserialize.</param>
		/// <returns>An instance of T, deserialized from the specified byte array.</returns>
		public static T Deserialize<T>(byte[] buffer) where T : IBinarySerializable
		{
			T obj = Activator.CreateInstance<T>();
			return Deserialize<T>(buffer, obj);
		}

		/// <summary>
		/// Deserializes the specified byte array into an object of type T.
		/// </summary>
		/// <typeparam name="T">The System.Type that implements IBinarySerializable.</typeparam>
		/// <param name="buffer">The byte array to deserialize.</param>
		/// <param name="obj">The IBinarySerializable to deserialize data into.</param>
		/// <returns>An instance of T, deserialized from the specified byte array.</returns>
		public static T Deserialize<T>(byte[] buffer, T obj) where T : IBinarySerializable
		{
			using (MemoryStream ms = new MemoryStream(buffer))
			{
				using (BinaryReader reader = new BinaryReader(ms))
				{
					if (buffer != null && buffer.Length > 0)
					{
						obj.Read(reader);
					}
					return obj;
				}
			}
		}

		/// <summary>
		/// Deserializes the specified byte array into an object of type T.
		/// </summary>
		/// <typeparam name="T">The System.Type that implements IBinarySerializable.</typeparam>
		/// <param name="buffer">The byte array to deserialize.</param>
		/// <returns>An instance of T, deserialized from the specified byte array.</returns>
		public static IEnumerable<T> DeserializeToEnumerable<T>(byte[] buffer) where T : IBinarySerializable
		{
			if (buffer == null || buffer.Length == 0) return new List<T>();
			List<T> objects = new List<T>();
			using (MemoryStream ms = new MemoryStream(buffer))
			{
				using (BinaryReader reader = new BinaryReader(ms))
				{
					var count = reader.ReadInt32();
					for (int i = 0; i < count; i++)
					{
						T obj = Activator.CreateInstance<T>();
						obj.Read(reader);
						objects.Add(obj);
					}
				}
			}
			return objects;
		}
	}
}
