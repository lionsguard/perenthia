using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Radiance.Markup
{
	/// <summary>
	/// Represents a reader than can read RDL tags.
	/// </summary>
	public class RdlTagReader : TextReader
	{
		private string _s;
		private int _pos = 0;
		private int _length = 0;
		private char _quote = '"';

		/// <summary>
		/// Initializes a new instance of the RdlTagReader.
		/// </summary>
		/// <param name="tagString">The tag string to parse.</param>
		public RdlTagReader(string tagString)
		{
			_s = tagString;
			_length = (_s == null) ? 0 : _s.Length;
		}

		/// <summary>
		/// Releases the unmanaged resources used by the RdlTagReader and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
		protected override void Dispose(bool disposing)
		{
			_s = null;
			base.Dispose(disposing);
		}

		/// <summary>
		/// Returns the next available character but does not consume it.
		/// </summary>
		/// <returns>The next character to be read, or -1 if no more characters are available or the stream does not support seeking.</returns>
		public override int Peek()
		{
			if (_s != null && _pos != _length)
			{
				return _s[_pos];
			}
			return -1;
		}

		/// <summary>
		/// Reads the next character from the input string and advances the character position by one character.
		/// </summary>
		/// <returns>The next character from the underlying string, or -1 if no more characters are available.</returns>
		public override int Read()
		{
			if (_s != null && _pos != _length)
			{
				return _s[_pos++];
			}
			return -1;
		}

		/// <summary>
		/// Reads the next RdlTag and advances the reader.
		/// </summary>
		/// <returns>An RdlTag instance.</returns>
		public RdlTag ReadTag()
		{
			try
			{
				RdlTag tag = null;
				if (!String.IsNullOrEmpty(_s) && _pos < _length)
				{
					string tagName = null;
					string typeName = null;
					int quoteCount = 0;
					int argIndex = 0;
					object value = null;
					bool parsingTagName = false;
					bool parsingTypeName = false;
					bool parsingString = false; // Indicates a string value is being parsed, ignore special chars.

					// Start at the current position and find starting tag char.
					// {OBJ|PROP|5|2|"Json Value"|""{Name:"Test",Value:"Equals"}""}
					do
					{
						// If the start character is found and not parsing a string value then
						// start the tag.
						if (_s[_pos] == RdlTag.TagStartChar && quoteCount == 0 && !parsingString)
						{
							// Being parsing the tag name.
							parsingTagName = true;
							continue;
						}

						// If a quote character is found then a string is being parsed until the quote count is down to 0.
						if (_s[_pos] == _quote)
						{
							// When the first quote is found start parsing the string until an even number of quotes exist
							// and a separator or end char is found.
							if (quoteCount == 0)
							{
								parsingString = true;
								quoteCount++;
								continue;
							}
							else
							{
								quoteCount++;

								if ((quoteCount % 2) == 0)
								{
									// Even number of quotes, check the next char to see if it is a separator or end char.
									if ((_pos + 1) < _length)
									{
										if (_s[_pos + 1] == RdlTag.TagSeparatorChar || _s[_pos + 1] == RdlTag.TagEndChar)
										{
											parsingString = false;
											quoteCount = 0;
											continue;
										}
									}
								}
							}
						}

						if ((_pos + 1) == _length)
						{
							parsingString = false;
						}

						if ((_s[_pos] == RdlTag.TagSeparatorChar || (_s[_pos] == RdlTag.TagEndChar))
							&& !parsingString)
						{
							if (parsingTagName)
							{
								parsingTagName = false;
								parsingTypeName = true;
							}
							else if (parsingTypeName)
							{
								parsingTypeName = false;

								// Create the RdlTag instance.
								tag = RdlTagConverter.CreateTag(tagName, typeName);
							}
							else
							{
								// Value can be added to tag.
								if (tag != null)
								{
									if (argIndex < tag.Args.Count)
									{
										tag.Args[argIndex] = this.FormatValue(value);
									}
									else
									{
										tag.Args.Insert(argIndex, this.FormatValue(value));
									}
									value = null;
									argIndex++;
								}
							}

							// If the end character is found the exit the loop.
							if (_s[_pos] == RdlTag.TagEndChar)
							{
								// Advance the position.
								_pos++;
								break;
							}

							continue;
						}

						// If made it to here then check to see if a value is being parsed.
						if (parsingTagName)
						{
							tagName = String.Concat(tagName, _s[_pos]);
						}
						else if (parsingTypeName)
						{
							typeName = String.Concat(typeName, _s[_pos]);
						}
						else
						{
							if (value == null) value = _s[_pos];
							else value = String.Concat(value, _s[_pos]);
						}
					} while (_pos++ < _length);
				}
				return tag;
			}
			catch (Exception ex)
			{
				throw (ex);
			}
		}

		private static Regex _regexDigits = new Regex(@"^[\d]*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
		private object FormatValue(object value)
		{
			if (value != null)
			{
				// Analyze the value to see what type it is and cast it as such.
				if (Boolean.TrueString.Equals(value) || Boolean.FalseString.Equals(value))
				{
					return Boolean.Parse(value.ToString());
				}
				if (_regexDigits.IsMatch(value.ToString()))
				{
					int intValue;
					if (Int32.TryParse(value.ToString(), out intValue))
					{
						return intValue;
					}
					long longValue;
					if (Int64.TryParse(value.ToString(), out longValue))
					{
						return longValue;
					}
				}

				return value.ToString();
			}
			return String.Empty;
		}
	}
}
