using System.IO;

namespace Radiance.Serialization
{
	/// <summary>
	/// Provides an object the ability to binary serialize and deserialize itself.
	/// </summary>
	public interface IBinarySerializable
	{
		/// <summary>
		/// Reads data from the specified reader and initializes the properties of the object.
		/// </summary>
		/// <param name="reader">The BinaryReader containing the information to reader.</param>
		void Read(BinaryReader reader);

		/// <summary>
		/// Writes property data from the current object to the specified writer.
		/// </summary>
		/// <param name="writer">The BinaryWriter in which to output the properties of the current object.</param>
		void Write(BinaryWriter writer);
	}
}
