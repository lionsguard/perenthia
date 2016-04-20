using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Radiance.Markup
{
	/// <summary>
	/// Represents a Radiance Definition Language (RDL) tag: $TAG_NAME|TYPE_NAME|ARGS
	/// </summary>
	public class RdlTag
	{
		public static readonly RdlTag Empty = new RdlTag(RdlTagName.EMPTY);

		/// <summary>
		/// Gets the string character used to denote the start of a Tag.
		/// </summary>
		public static readonly char TagStartChar = '{';

		/// <summary>
		/// Gets the string character used to denote the end of a Tag.
		/// </summary>
		public static readonly char TagEndChar = '}';

		public static readonly char TagSeparatorChar = '|';

		/// <summary>
		/// Gets the name of the current tag.
		/// </summary>
		public string TagName { get; private set; }

		/// <summary>
		/// Gets or sets the type name of the current tag.
		/// </summary>
		public string TypeName { get; set; }
		
		/// <summary>
		/// Gets a collection of arguments for the current tag.
		/// </summary>
		public List<object> Args { get; private set; }

		/// <summary>
		/// Initializes a new instance of the RdlTag class.
		/// </summary>
		/// <param name="tagName">The name of the current tag.</param>
		public RdlTag(string tagName)
		{
			this.TagName = tagName.ToUpper();
			this.Args = new List<object>();
		}

		/// <summary>
		/// Initializes a new instance of the RdlTag class.
		/// </summary>
		/// <param name="tagName">The name of the current tag.</param>
		/// <param name="typeName">The name of the tag type.</param>
		public RdlTag(string tagName, string typeName)
		{
			this.TagName = tagName.ToUpper();
			this.TypeName = typeName.ToUpper();
			this.Args = new List<object>();
		}

		/// <summary>
		/// Initializes a new instance of the RdlTag class.
		/// </summary>
		/// <param name="tagName">The name of the current tag.</param>
		public RdlTag(RdlTagName tagName)
			: this(tagName.ToString())
		{
		}

		/// <summary>
		/// Initializes a new instance of the RdlTag class.
		/// </summary>
		/// <param name="tagName">The name of the current tag.</param>
		/// <param name="typeName">The name of the tag type.</param>
		public RdlTag(RdlTagName tagName, string typeName)
			: this(tagName.ToString(), typeName)
		{
		}

		private int _index = 0;
		/// <summary>
		/// Gets the next index available for inserting a new object into the args list.
		/// </summary>
		/// <returns>The next available index.</returns>
		protected int GetNextIndex()
		{
			return _index++;
		}

		/// <summary>
		/// Gets the tags string representation $TAG_NAME|TYPE_NAME|ARGS
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			using (RdlTagWriter writer = new RdlTagWriter())
			{
				this.WriteTag(writer);
				return writer.ToString();
			}
		}

		/// <summary>
		/// Gets a byte array of the current RdlTag instance.
		/// </summary>
		/// <returns>A byte array of the current RdlTag instance.</returns>
		public byte[] ToBytes()
		{
			return Encoding.UTF8.GetBytes(this.ToString());
		}

		/// <summary>
		/// Writes the current tag to the specified RdlTagWriter instance.
		/// </summary>
		/// <param name="writer">The RdlTagWriter instance to write the current tag to.</param>
		public void WriteTag(RdlTagWriter writer)
		{
			writer.WriteBeginTag(this.TagName, this.TypeName);
			for (int i = 0; i < this.Args.Count; i++)
			{
				writer.Write(this.Args[i]);
			}
			writer.WriteEndTag();
		}

		/// <summary>
		/// Gets the value of the argument at the specified index.
		/// </summary>
		/// <typeparam name="T">The System.Type of the value to return.</typeparam>
		/// <param name="index">The index position of the argument to return.</param>
		/// <returns>The typed value of the argument or the default value of the type if the argument is null or not present.</returns>
		public T GetArg<T>(int index)
		{
			if (index < this.Args.Count)
			{
				return (T)Convert.ChangeType(this.Args[index], typeof(T), null);
			}
			return default(T);
		}

		#region Static Methods
		/// <summary>
		/// Creates a Tag instance from the specified tag string.
		/// </summary>
		/// <param name="tagString">The string representation of the tag to create.</param>
		/// <returns>A new instance of the Tag class containing the information from the tag string or 
		/// Tag.Empty if the tag could not be parsed.</returns>
		public static RdlTag FromString(string tagString)
		{
			using (RdlTagReader reader = new RdlTagReader(tagString))
			{
				return reader.ReadTag();
			}
        }

        /// <summary>
        /// Creates a Tag instance from the specified byte array.
        /// </summary>
        /// <param name="data">The byte array containing the tag to create.</param>
        /// <returns>A new instance of the Tag class containing the information from the tag byte array or 
        /// Tag.Empty if the tag could not be parsed.</returns>
        public static RdlTag FromBytes(byte[] data)
        {
            return FromString(Encoding.UTF8.GetString(data, 0, data.Length));
        }
		#endregion
	}

	public class RdlAuthKey : RdlTag
	{
		private int _authKeyIndex = 0;
		private int _persistLoginTokenIndex = 1;

		public string Key
		{
			get { return this.GetArg<string>(_authKeyIndex); }
			set { this.Args[_authKeyIndex] = value; }
		}

		public string PersistLoginToken
		{
			get { return this.GetArg<string>(_persistLoginTokenIndex); }
			set { this.Args[_persistLoginTokenIndex] = value; }
		}

		public RdlAuthKey()
			: this(String.Empty, String.Empty)
		{
		}

		public RdlAuthKey(string authKey, string authKeyType)
			: base(RdlTagName.AUTH, (authKeyType ?? String.Empty).ToUpper())
		{
			_authKeyIndex = this.GetNextIndex();
			this.Args.Insert(_authKeyIndex, authKey);
			this.Args.Insert(_persistLoginTokenIndex, String.Empty);
		}
	}
}
