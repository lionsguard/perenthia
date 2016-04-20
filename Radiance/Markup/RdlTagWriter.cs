using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Radiance.Markup
{
	/// <summary>
	/// Represents a writer that can write RDL tags.
	/// </summary>
	public class RdlTagWriter : TextWriter
	{
		StringBuilder _sb;

		/// <summary>
		/// Returns the Encoding in which the output is written.
		/// </summary>
		public override Encoding Encoding
		{
			get { return Encoding.UTF8; }
		}

		/// <summary>
		/// Initializes a new instance of the RdlTagWriter class.
		/// </summary>
		public RdlTagWriter()
			: this(new StringBuilder(), null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the RdlTagWriter class.
		/// </summary>
		/// <param name="sb">The StringBuilder instance to write the output to.</param>
		public RdlTagWriter(StringBuilder sb)
			: this(sb, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the RdlTagWriter class.
		/// </summary>
		/// <param name="formatProvider">The IFormatProvider used when converting object values.</param>
		public RdlTagWriter(IFormatProvider formatProvider)
			: this(new StringBuilder(), null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the RdlTagWriter class.
		/// </summary>
		/// <param name="sb">The StringBuilder instance to write the output to.</param>
		/// <param name="formatProvider">The IFormatProvider used when converting object values.</param>
		public RdlTagWriter(StringBuilder sb, IFormatProvider formatProvider)
			: base(formatProvider)
		{
			_sb = sb;
			this.CoreNewLine = String.Empty.ToCharArray();
		}

		/// <summary>
		/// Releases the unmanaged resources used by the RdlTagWriter and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
		protected override void Dispose(bool disposing)
		{
			_sb = null;
			base.Dispose(disposing);
		}

		/// <summary>
		/// Writes the begin tag characters of an RDL tag including the tag name and type.
		/// </summary>
		/// <param name="tagName">The name of the tag to write.</param>
		/// <param name="typeName">THe type of the tag to write.</param>
		public void WriteBeginTag(string tagName, string typeName)
		{
			if (tagName == null) tagName = String.Empty;
			if (typeName == null) typeName = String.Empty;

			this.Write(RdlTag.TagStartChar);
			this.Write(tagName.ToUpper().ToCharArray());
			this.Write(RdlTag.TagSeparatorChar);
			this.Write(typeName.ToUpper().ToCharArray());
			this.Write(RdlTag.TagSeparatorChar);
		}

		/// <summary>
		/// Write the end tag characters of an RDL tag.
		/// </summary>
		public void WriteEndTag()
		{
			this.Write(RdlTag.TagEndChar);
		}

		/// <summary>
		/// Writes the separator character to the output.
		/// </summary>
		public void WriteSeparator()
		{
			this.Write(RdlTag.TagSeparatorChar);
		}

		/// <summary>
		/// Writes a boolean to this instance of the RdlTagWriter.
		/// </summary>
		/// <param name="value">The boolean to write. </param>
		public override void Write(bool value)
		{
			this.Write(value.ToString().ToCharArray());
		}

		/// <summary>
		/// Writes a decimal to this instance of the RdlTagWriter.
		/// </summary>
		/// <param name="value">The decimal to write. </param>
		public override void Write(decimal value)
		{
			this.Write(value.ToString().ToCharArray());
		}

		/// <summary>
		/// Writes a double to this instance of the RdlTagWriter.
		/// </summary>
		/// <param name="value">The double to write. </param>
		public override void Write(double value)
		{
			this.Write(value.ToString().ToCharArray());
		}

		/// <summary>
		/// Writes a float to this instance of the RdlTagWriter.
		/// </summary>
		/// <param name="value">The float to write. </param>
		public override void Write(float value)
		{
			this.Write(value.ToString().ToCharArray());
		}

		/// <summary>
		/// Writes a int to this instance of the RdlTagWriter.
		/// </summary>
		/// <param name="value">The int to write. </param>
		public override void Write(int value)
		{
			this.Write(value.ToString().ToCharArray());
		}

		/// <summary>
		/// Writes a long to this instance of the RdlTagWriter.
		/// </summary>
		/// <param name="value">The long to write. </param>
		public override void Write(long value)
		{
			this.Write(value.ToString().ToCharArray());
		}

		/// <summary>
		/// Writes a uint to this instance of the RdlTagWriter.
		/// </summary>
		/// <param name="value">The uint to write. </param>
		public override void Write(uint value)
		{
			this.Write(value.ToString().ToCharArray());
		}

		/// <summary>
		/// Writes a ulong to this instance of the RdlTagWriter.
		/// </summary>
		/// <param name="value">The ulong to write. </param>
		public override void Write(ulong value)
		{
			this.Write(value.ToString().ToCharArray());
		}

		private void EnsureSeparator()
		{
			if (_sb != null && _sb.Length > 0)
			{
				if (_sb[_sb.Length - 1] != RdlTag.TagSeparatorChar)
				{
					this.WriteSeparator();
				}
			}
		}

		/// <summary>
		/// Writes the text representation of an object to the current instance of the RdlTagWriter.
		/// </summary>
		/// <param name="value">The object to write. </param>
		public override void Write(object value)
		{
			if (value != null)
			{
				// If the last written character was not a separator then write one now.
				this.EnsureSeparator();

				TypeCode typeCode = Convert.GetTypeCode(value);
				switch (typeCode)
				{
					case TypeCode.Boolean:
						this.Write((bool)value);
						break;
					case TypeCode.Char:
						this.Write((char)value);
						break;
					case TypeCode.Decimal:
						this.Write((decimal)value);
						break;
					case TypeCode.Double:
						this.Write((double)value);
						break;	
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.Int32:
						this.Write((int)value);
						break;
					case TypeCode.Int64:
						this.Write((long)value);
						break;
					case TypeCode.UInt16:
					case TypeCode.UInt32:
						this.Write((uint)value);
						break;
					case TypeCode.UInt64:
						this.Write((ulong)value);
						break;
					default:
						base.Write(value);
						break;
				}
			}
		}

		/// <summary>
		/// Writes a string to this instance of the RdlTagWriter.
		/// </summary>
		/// <param name="value">The string to write. </param>
		public override void Write(string value)
		{
			if (value != null)
			{
				this.EnsureSeparator();

				// All strings are quoted, regardless of whether quotes already exist or not.
				this.Write('"');
				this.Write(value.ToCharArray());
				this.Write('"');
			}
		}

		/// <summary>
		/// Writes a character to this instance of the RdlTagWriter.
		/// </summary>
		/// <param name="value">The character to write. </param>
		public override void Write(char value)
		{
			_sb.Append(value);
		}

		/// <summary>
		/// Returns a string containing the characters written to the current RdlTagWriter so far.
		/// </summary>
		/// <returns>The string containing the characters written to the current RdlTagWriter.</returns>
		public override string ToString()
		{
			return _sb.ToString();
		}
	}
}
