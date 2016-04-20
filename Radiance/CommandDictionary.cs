using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance
{
	/// <summary>
	/// Provides a dictionary of commands where the key is the command name and the value is the role required to execute the command.
	/// </summary>
	public class CommandDictionary : Dictionary<string, CommandInfo>
	{
		/// <summary>
		/// Initializes a new instance of the CommandDictionary class.
		/// </summary>
		public CommandDictionary()
			: base(StringComparer.InvariantCultureIgnoreCase)
		{
		}
	}

	/// <summary>
	/// Represents information regarding a command in the virtual world.
	/// </summary>
	public class CommandInfo
	{
		private const char Quote = '"';
		private const char Separator = '|';

		/// <summary>
		/// Gets or sets the unique ID of the command.
		/// </summary>
		public int ID { get; set; }
		/// <summary>
		/// Gets or sets the name of the command.
		/// </summary>
		public string CommandName { get; set; }
		/// <summary>
		/// Gets or sets the user role required to execute this command.
		/// </summary>
		public string RequiredRole { get; set; }
		/// <summary>
		/// Gets or sets the syntax used to execute the command, incuding parameters, assumptions, etc.
		/// </summary>
		public string Syntax { get; set; }
		/// <summary>
		/// Gets the help text used to provide help information on the command.
		/// </summary>
		public string Help { get; set; }
		/// <summary>
		/// Gets the description of each argument, quoted and pipe-delimited.
		/// </summary>
		public string Arguments { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether or not the specified command is visible to players.
		/// </summary>
		public bool IsVisible { get; set; }	

		private bool _argsInit = false;
		private int _argCount = 0;

		public int GetArgumentsCount()
		{
			if (!_argsInit)
			{
				this.GetArgumentsForDisplay();
			}
			return _argCount;
		}

		/// <summary>
		/// Parses and returns the arguments value into readable descriptions of the arguments.
		/// </summary>
		/// <returns></returns>
		public string GetArgumentsForDisplay()
		{
			int quoteCount = 0;
			StringBuilder sb = new StringBuilder();
			if (this.Arguments.Length > 0 && !_argsInit)
			{
				_argCount = 1;
			}
			for (int i = 0; i < this.Arguments.Length; i++)
			{
				if (this.Arguments[i] == Quote)
				{
					if (quoteCount == 0)
					{
						quoteCount++;
						continue;
					}
					else
					{
						quoteCount++;

						if ((quoteCount % 2) == 0)
						{
							// Even number of quotes, check the next char to see if it is a separator or end char.
							if ((i + 1) < this.Arguments.Length)
							{
								if (this.Arguments[i + 1] == Separator)
								{
									quoteCount = 0;
									continue;
								}
							}
						}
					}
				}
				else if (this.Arguments[i] == Separator)
				{
					if (!_argsInit) _argCount++;
					sb.AppendLine();
				}
				else
				{
					sb.Append(this.Arguments[i]);
				}
			}
			_argsInit = true;
			return sb.ToString();
		}

		/// <summary>
		/// Gets the full help documentation for the current command.
		/// </summary>
		/// <returns>A display string with the full help instructions for the command.</returns>
		public string GetFullHelp()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(this.Syntax).AppendLine().AppendLine();
			sb.AppendLine(this.GetArgumentsForDisplay()).AppendLine();
			sb.AppendLine(this.Help).AppendLine();

			return sb.ToString();
		}
	}
}
