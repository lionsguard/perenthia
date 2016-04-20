using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Radiance
{
	/// <summary>
	/// Provides static methods for formatting object strings.
	/// </summary>
	public static class Strings
	{
		#region Static Constructor
		private static Dictionary<string, string> IrregularPluralNouns = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

		static Strings()
		{
			IrregularPluralNouns.Add("ale", "ale");
			IrregularPluralNouns.Add("beer", "beer");
			IrregularPluralNouns.Add("bread", "bread");
			IrregularPluralNouns.Add("child", "children");
			IrregularPluralNouns.Add("cod", "cod");
			IrregularPluralNouns.Add("copper", "copper");
			IrregularPluralNouns.Add("deer", "deer");
			IrregularPluralNouns.Add("domino", "dominoes");
			IrregularPluralNouns.Add("echo", "echoes");
			IrregularPluralNouns.Add("fish", "fish");
			IrregularPluralNouns.Add("foot", "feet");
			IrregularPluralNouns.Add("gold", "gold");
			IrregularPluralNouns.Add("goose", "geese");
			IrregularPluralNouns.Add("hero", "heroes");
			IrregularPluralNouns.Add("man", "men");
			IrregularPluralNouns.Add("mango", "mangoes");
			IrregularPluralNouns.Add("meat", "meat");
			IrregularPluralNouns.Add("moose", "moose");
			IrregularPluralNouns.Add("motto", "mottoes");
			IrregularPluralNouns.Add("ogre", "ogre");
			IrregularPluralNouns.Add("ore", "ore");
			IrregularPluralNouns.Add("ox", "oxen");
			IrregularPluralNouns.Add("person", "people");
			IrregularPluralNouns.Add("potato", "potatoes");
			IrregularPluralNouns.Add("quiz", "quizzes");
			IrregularPluralNouns.Add("runner-up", "runners-up");
			IrregularPluralNouns.Add("series", "series");
			IrregularPluralNouns.Add("sheep", "sheep");
			IrregularPluralNouns.Add("silver", "silver");
			IrregularPluralNouns.Add("son-in-law", "sons-in-law");
			IrregularPluralNouns.Add("species", "species");
			IrregularPluralNouns.Add("tomato", "tomatoes");
			IrregularPluralNouns.Add("tooth", "teeth");
			IrregularPluralNouns.Add("tornado", "tornadoes");
			IrregularPluralNouns.Add("volcano", "volcanoes");
			IrregularPluralNouns.Add("wine", "wine");
			IrregularPluralNouns.Add("woman", "women");

		}
		#endregion

		#region Alias
		/// <summary>
		/// Gets an alias for the specified name and id, removing spaces and concatinating the id value.
		/// </summary>
		/// <param name="name">The name to alias.</param>
		/// <param name="id">The id to append to the alias.</param>
		/// <returns>The current aliased name.</returns>
		public static string Alias(this string name, int id)
		{
			return String.Concat(FormatAliasName(name), id).ToLower();
		}

		private static Regex AliasRegEx = new Regex(@"(?<NAME>[^\d]*)(?<ID>[\d]*)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
		private static Regex IDRegEx = new Regex(@"[-{0,1}\d]*", RegexOptions.Multiline | RegexOptions.IgnoreCase);
		/// <summary>
		/// Parses an alias and sets the output id and name from the parsed values.
		/// </summary>
		/// <param name="alias">The alias to parse.</param>
		/// <param name="id">The output id, if found in the alias; otherwise will default to 0.</param>
		/// <param name="name">The output name, if found in the alias; otherwise will default to null.</param>
		public static void ParseAlias(string alias, out int id, out string name)
		{
			id = 0;
			name = null;
			// Text for ID first.
			if (IDRegEx.IsMatch(alias))
			{
				if (Int32.TryParse(alias, out id))
					return;
			}

			var match = AliasRegEx.Match(alias);
			if (match != null)
			{
				Group idGroup = match.Groups["ID"];
				if (idGroup != null && !String.IsNullOrEmpty(idGroup.Value))
				{
					Int32.TryParse(idGroup.Value, out id);
				}
				else
				{
					Group nameGroup = match.Groups["NAME"];
					if (nameGroup != null && !String.IsNullOrEmpty(nameGroup.Value))
					{
						name = Strings.FormatAliasName(nameGroup.Value.ToLower());
					}
				}
			}
		}

		/// <summary>
		/// Formats a name for use with an Alias.
		/// </summary>
		/// <param name="name">The name to format.</param>
		/// <returns>The formatted name.</returns>
		public static string FormatAliasName(string name)
		{
			return name.Replace(" ", String.Empty).Replace("'", String.Empty).ToLower();
		}
		#endregion

		#region The
		public static string The(this string name, bool isProperName)
		{
			return name.The(isProperName, 0);
		}
		public static string The(this string name, bool isProperName, int quantity)
		{
			return GetDescriptionName(name, isProperName, ArticleType.The, false, quantity);
		}

		public static string TheUpper(this string name, bool isProperName)
		{
			return name.TheUpper(isProperName, 0);
		}
		public static string TheUpper(this string name, bool isProperName, int quantity)
		{
			return GetDescriptionName(name, isProperName, ArticleType.The, true, quantity);
		}
		#endregion

		#region A
		public static string A(this string name, bool isProperName)
		{
			return name.A(isProperName, 0);
		}
		public static string A(this string name, bool isProperName, int quantity)
		{
			return GetDescriptionName(name, isProperName, ArticleType.A, false, quantity);
		}

		public static string AUpper(this string name, bool isProperName)
		{
			return name.AUpper(isProperName, 0);
		}
		public static string AUpper(this string name, bool isProperName, int quantity)
		{
			return GetDescriptionName(name, isProperName, ArticleType.A, true, quantity);
		}
		#endregion

		#region Your
		public static string Your(this string name, bool isProperName)
		{
			return name.Your(isProperName, 0);
		}
		public static string Your(this string name, bool isProperName, int quantity)
		{
			return GetDescriptionName(name, isProperName, ArticleType.Your, false, quantity);
		}

		public static string YourUpper(this string name, bool isProperName)
		{
			return name.YourUpper(isProperName, 0);
		}
		public static string YourUpper(this string name, bool isProperName, int quantity)
		{
			return GetDescriptionName(name, isProperName, ArticleType.Your, true, quantity);
		}
		#endregion

		#region None
		public static string None(this string name, bool isProperName)
		{
			return name.None(isProperName, 0);
		}
		public static string None(this string name, bool isProperName, int quantity)
		{
			return GetDescriptionName(name, isProperName, ArticleType.None, false, quantity);
		}

		public static string NoneUpper(this string name, bool isProperName)
		{
			return name.NoneUpper(isProperName, 0);
		}
		public static string NoneUpper(this string name, bool isProperName, int quantity)
		{
			return GetDescriptionName(name, isProperName, ArticleType.None, true, quantity);
		}
		#endregion

		public static string EnsureProperSentence(string message)
		{
			if (!String.IsNullOrEmpty(message))
			{
				if (!Char.IsUpper(message[0]))
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(message[0].ToString().ToUpper());
					sb.Append(message.Substring(1));
					return sb.ToString();
				}
			}
			return message;
		}

		/// <summary>
		/// Formats the specified name to be displyed properly within a sentence, such as displaying the name of a creature or number of creatures.
		/// </summary>
		/// <param name="name">The name of the object to describe.</param>
		/// <param name="isProperName">A value indicating whether or not the name is a proper name.</param>
		/// <param name="articleType">The article to use as a prefix to the name; sets the context of the sentence.</param>
		/// <param name="isSentenceStart">A value indicating whether the name starts the sentence and should be proper cased.</param>
		/// <param name="quantity">The number being described in order to pluralize the name.</param>
		/// <returns>The name formatted for display.</returns>
		public static string GetDescriptionName(string name, bool isProperName, ArticleType articleType, bool isSentenceStart, int quantity)
		{
			string startWord = String.Empty;
			switch (articleType)
			{
				case ArticleType.A:
					if (!isProperName)
					{
						if (name.ToLower().StartsWith("a") ||
							name.ToLower().StartsWith("e") ||
							name.ToLower().StartsWith("i") ||
							name.ToLower().StartsWith("o") ||
							name.ToLower().StartsWith("u"))
						{
							startWord = "an";
						}
						else
						{
							startWord = "a";
						}
					}
					break;
				case ArticleType.None:
					startWord = String.Empty;
					break;
				default:
					if (!isProperName)
					{
						startWord = articleType.ToString().ToLower();
					}
					break;
			}
			if (isSentenceStart)
			{
				if (startWord.Length > 1)
				{
					startWord = String.Concat(startWord.Substring(0, 1).ToUpper(), startWord.Substring(1));
				}
				else
				{
					startWord = startWord.ToUpper();
				}
			}

			if (quantity > 1)
			{
				// Start word should be the quantity.
				startWord = quantity.ToString();

				// Lower the name for easy comaparison.
				string loweredName = name.ToLower();

				// Make the name plural.
				if (IrregularPluralNouns.ContainsKey(loweredName))
				{
					// Check the list of irregular nouns.
					bool isFirstCharUpper = Char.IsUpper(name, 0);
					name = IrregularPluralNouns[loweredName];
					if (isFirstCharUpper) name = String.Concat(name.Substring(0, 1).ToUpper(), name.Substring(1));
				}
				else if (loweredName.EndsWith("s")
					|| loweredName.EndsWith("x")
					|| loweredName.EndsWith("ch")
					|| loweredName.EndsWith("sh")
					|| loweredName.EndsWith("z"))
				{
					name = String.Concat(name, "es");
				}
				else if (loweredName.EndsWith("y")
					&& (loweredName.Length >= 2 && !IsVowel(loweredName[loweredName.Length - 2])))
				{
					name = String.Concat(name.Substring(0, name.Length - 1), "ies");
				}
				else if (loweredName.EndsWith("is"))
				{
					name = String.Concat(name.Substring(0, name.Length - 2), "es");
				}
				else if (loweredName.EndsWith("ix"))
				{
					name = String.Concat(name.Substring(0, name.Length - 2), "ices");
				}
				else if (loweredName.EndsWith("ouse"))
				{
					name = String.Concat(name.Substring(0, name.Length - 4), "ice");
				}
				else if (loweredName.EndsWith("eau"))
				{
					name = String.Concat(name, "x");
				}
				else if (loweredName.EndsWith("f"))
				{
					name = String.Concat(name.Substring(0, name.Length - 1), "ves");
				}
				else if (loweredName.EndsWith("fe"))
				{
					name = String.Concat(name.Substring(0, name.Length - 2), "ves");
				}
				else if (loweredName.EndsWith("us"))
				{
					name = String.Concat(name.Substring(0, name.Length - 2), "i");
				}
				else if (loweredName.EndsWith("um"))
				{
					name = String.Concat(name.Substring(0, name.Length - 2), "a");
				}
				else
				{
					name = String.Concat(name, "s");
				}
			}

			if (startWord.Length > 0)
			{
				return String.Concat(startWord, " ", name);
			}
			return name;
		}

		private static bool IsVowel(char c)
		{
			return (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u');
		}
	}
}
