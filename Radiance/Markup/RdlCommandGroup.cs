using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance.Markup
{
	/// <summary>
	/// Represents a group of commands to be executed by the server.
	/// </summary>
	public class RdlCommandGroup : List<RdlCommand>
	{
		/// <summary>
		/// Gets or sets the AuthKey of the user sending the command group.
		/// </summary>
		public string AuthKey { get; set; }

		/// <summary>
		/// Gets or sets the type of the AuthKey sent with the commands, either Player or User.
		/// </summary>
		public string AuthKeyType { get; set; }	

        /// <summary>
        /// Gets a collection of tags associated with the current command group.
        /// </summary>
        public RdlTagCollection Tags { get; internal set; }  

		/// <summary>
		/// Initializes a new instance of the RdlCommandGroup class.
		/// </summary>
		public RdlCommandGroup()
        {
            this.Tags = new RdlTagCollection();
		}

		/// <summary>
		/// Initializes a new instance of the RdlCommandGroup class and prefills the collection.
		/// </summary>
		/// <param name="commands">The collection of commands to prefill the collection.</param>
		public RdlCommandGroup(IEnumerable<RdlCommand> commands)
			: this()
		{
            if (commands != null)
            {
                foreach (var item in commands)
                {
                    this.Add(item); 
                }
            }
		}

        /// <summary>
        /// Adds a new RdlCommand instance to the command group.
        /// </summary>
        /// <param name="item">The RdlCommand instance to add to the group.</param>
        public new void Add(RdlCommand item)
        {
            item.Group = this;
            base.Add(item);
        }
		
		/// <summary>
		/// Gets the string representation of the commands contained within the collection.
		/// </summary>
		/// <returns>The string representation of the commands contained within the collection.</returns>
		public override string ToString()
		{
			RdlAuthKey user = new RdlAuthKey();
			if (!String.IsNullOrEmpty(this.AuthKey) && !String.IsNullOrEmpty(this.AuthKeyType))
			{
				user = new RdlAuthKey(this.AuthKey, this.AuthKeyType);
			}
			using (RdlTagWriter writer = new RdlTagWriter())
			{
				user.WriteTag(writer);
				foreach (var item in this)
				{
					item.WriteTag(writer);
				}
				foreach (var tag in this.Tags)
				{
					tag.WriteTag(writer);
				}
				return writer.ToString();
			}
		}

		/// <summary>
		/// Converts the collection of RdlCommand instances to a byte array.
		/// </summary>
		/// <returns>An array of bytes of the current collection.</returns>
		public byte[] ToBytes()
		{
			return Encoding.UTF8.GetBytes(this.ToString());
		}

		/// <summary>
		/// Creates a new RdlCommandGroup from the specified tagString.
		/// </summary>
		/// <param name="tagString">The string containing multiple commands that will become part of the new collection.</param>
		/// <returns>A new RdlCommandGroup instance containing the parsed commands.</returns>
		public static RdlCommandGroup FromString(string tagString)
		{
			RdlCommandGroup col = new RdlCommandGroup();
			using (RdlTagReader reader = new RdlTagReader(tagString))
			{
				RdlTag tag = null;
				while ((tag = reader.ReadTag()) != null)
				{
					if (tag.TagName.Equals(RdlTagName.CMD.ToString()))
					{
						col.Add((RdlCommand)tag);
					}
					else if (tag.TagName.Equals(RdlTagName.AUTH.ToString()))
					{
						col.AuthKey = (tag as RdlAuthKey).Key;
						col.AuthKeyType = (tag as RdlAuthKey).TypeName;
					}
					else
					{
						col.Tags.Add(tag);
					}
				}
			}
			return col;
		}

		/// <summary>
		/// Creates a new RdlTagCollection instance from the specified byte array.
		/// </summary>
		/// <param name="data">The array bytes containing the tags to parse.</param>
		/// <returns>A new RdlTagCollection from the specified byte array.</returns>
		public static RdlCommandGroup FromBytes(byte[] data)
		{
			return FromString(Encoding.UTF8.GetString(data, 0, data.Length));
		}
	}
}
