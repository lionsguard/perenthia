using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance.Markup
{
	/// <summary>
	/// Represents a command in the Radiance Definition Language (RDL).
	/// </summary>
	public class RdlCommand : RdlTag
	{
		/// <summary>
		/// Gets the default type name of the CMD tag when initialized without providing the type name.
		/// </summary>
		public static readonly string DefaultTypeName = KnownCommands.None;

		public static readonly RdlCommand Heartbeat = new RdlCommand(KnownCommands.Heartbeat);

        /// <summary>
        /// Gets or sets the RdlCommandGroup this command belongs to.
        /// </summary>
        public RdlCommandGroup Group { get; set; }

		/// <summary>
		/// Gets the first argument of the command as text, used for chat commands.
		/// </summary>
		public string Text
		{
			get { return this.GetArg<string>(0); }
		}

		/// <summary>
		/// Initializes a new instance of the CMD tag.
		/// </summary>
		public RdlCommand()
			: this(DefaultTypeName)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CMD tag and sets the TypeName value.
		/// </summary>
		/// <param name="typeName">The type of CMD to create.</param>
		public RdlCommand(string typeName)
			: base(RdlTagName.CMD)
		{
			this.TypeName = typeName;
		}

		/// <summary>
		/// Initializes a new instance of the CMD tag and sets the TypeName value.
		/// </summary>
		/// <param name="typeName">The type of CMD to create.</param>
		/// <param name="args">The arguments for the current command.</param>
		public RdlCommand(string typeName, params object[] args)
			: this(typeName)
		{
			this.Args.AddRange(args);
		}

		#region Static Methods

		/// <summary>
		/// Creates a new instance of the RdlCommand object using the specified RdlCommonCommand as the TypeName value.
		/// </summary>
		/// <param name="commonCommand">The RdlCommonCommand used as the TypeName value of the CMD tag.</param>
		/// <returns>A new RdlCommand instance.</returns>
		public static RdlCommand FromCommonCommand(RdlCommonCommand commonCommand)
		{
			RdlCommand cmd = new RdlCommand(commonCommand.ToString());

			switch (commonCommand)
			{	
				case RdlCommonCommand.QUIT:
					break;
				case RdlCommonCommand.HELP:
					break;
				case RdlCommonCommand.BUG:
					break;
				case RdlCommonCommand.SAY:
					break;
				case RdlCommonCommand.SHOUT:
					break;
				case RdlCommonCommand.TELL:
					break;
				case RdlCommonCommand.LOOK:
					break;
				case RdlCommonCommand.MOVE:
					break;
				case RdlCommonCommand.ATTACK:
					break;
				case RdlCommonCommand.CAST:
					break;
				case RdlCommonCommand.GET:
					break;
				case RdlCommonCommand.DROP:
					break;
			}
			return cmd;
		}

		/// <summary>
		/// Attempts to parse the input string into an RDL command.
		/// </summary>
		/// <param name="input">The input from a chat or command console.</param>
		/// <param name="command">An RdlCommand instance representing the parsed input.</param>
		/// <param name="errorType">The RdlCommandParserErrorType value of any parsing errors.</param>
		/// <returns>True if the input was successfully parsed; otherwise false. If false the errorType 
		/// parameter will contain information regarding the reason parsing failed.</returns>
		public static bool TryParse(string input, out RdlCommand command, out RdlCommandParserErrorType errorType)
		{
			bool result = false;
			errorType = RdlCommandParserErrorType.None;
			command = new RdlCommand();
			if (!input.StartsWith("/")) input = String.Concat("/SAY ", input);

			string[] words = input.Split(' ');
			if (words != null && words.Length > 0)
			{
				// First word is the command, minus the "/"
				command.TypeName = words[0].Replace("/", "").ToUpper();

				// Command shortcuts.
				if (command.TypeName == "'") command.TypeName = "SAY";
				if (command.TypeName == "\"") command.TypeName = "SHOUT";
				if (command.TypeName == ":") command.TypeName = "EMOTE";
				if (command.TypeName == ";") command.TypeName = "EMOTE";
				if (command.TypeName == "r") command.TypeName = "REPLY";

				// Handle parsing of the command arguments based on common commands.
				switch (command.TypeName)
				{
					//case "SAY":
					//case "SHOUT":
					//case "EMOTE":
					//    // The remainder of the words should be re-joined to form the text of the message.
					//    if (words.Length >= 2)
					//    {
					//        command.Args.Add(JoinWords(words, 1));
					//        result = true;
					//    }
					//    else
					//    {
					//        errorType = RdlCommandParserErrorType.InvalidNumberOfArguments;
					//    }
					//    break;
					case "TELL":
					case "REPLY":
						// The next word is the name of the TELL target, the words following can be joined
						// to form the text of the message.
						if (words.Length >= 2)
						{
							if (words.Length >= 3)
							{
								command.Args.Add(words[1]);
								command.Args.Add(JoinWords(words, 2));
								result = true;
							}
							else
							{
								errorType = RdlCommandParserErrorType.InvalidNumberOfArguments;
							}
						}
						else
						{
							errorType = RdlCommandParserErrorType.NoTargetForTell;
						}
						break;
					default:
						//// Just parse all the words as separate args.
						//if (words.Length > 1)
						//{
						//    for (int i = 1; i < words.Length; i++)
						//    {
						//        command.Args.Add(words[i]);
						//    }
						//}

						// The remainder of the words should be re-joined to form the text of the message.
						if (words.Length >= 2)
						{
							command.Args.Add(JoinWords(words, 1));
						}

						// Allow no arguments.
						result = true;
						break;
				}
			}
			else
			{
				errorType = RdlCommandParserErrorType.NoArgumentsSpecified;
			}
			return result;
		}
		private static string JoinWords(string[] words, int startIndex)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = startIndex; i < words.Length; i++)
			{
				if (i > startIndex) sb.Append(" ");
				sb.Append(words[i]);
			}
			return sb.ToString();
		}
		#endregion
	}

	/// <summary>
	/// Represents a response to a command.
	/// </summary>
	public class RdlCommandResponse : RdlTag
	{
		private int _resultIndex;
		private int _messageIndex;

		/// <summary>
		/// Gets or sets the name of the command this is in response to.
		/// </summary>
		public string CommandName
		{
			get { return this.TypeName; }
			set { this.TypeName = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating the result of the command.
		/// </summary>
		public bool Result
		{
			get { return this.GetArg<bool>(_resultIndex); }
			set { this.Args[_resultIndex] = value; }
		}

		/// <summary>
		/// Gets or sets a message describing the result of the command.
		/// </summary>
		public string Message
		{
			get { return this.GetArg<string>(_messageIndex); }
			set { this.Args[_messageIndex] = value; }
		}

		/// <summary>
		/// Initializes a new instance of the RESP tag.
		/// </summary>
		public RdlCommandResponse()
			: this(RdlCommand.DefaultTypeName, false, String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the RESP tag and presets the property values.
		/// </summary>
		/// <param name="commandName">The name of the command this responds to.</param>
		/// <param name="result">The result of the command execution.</param>
		/// <param name="message">The message describing the result.</param>
		public RdlCommandResponse(string commandName, bool result, string message)
			: base(RdlTagName.RESP)
		{
			// Args = CMD|result|message
			// CMD = the name of the command for the response
			// result = the result of the command (true or false)
			// message = a message associated with the command response.
			this.TypeName = commandName;
			_resultIndex = this.GetNextIndex();
			_messageIndex = this.GetNextIndex();
			this.Args.Insert(_resultIndex, result);
			this.Args.Insert(_messageIndex, message);
		}
	}

	/// <summary>
	/// Provides an enumeration of common virtual world commands.
	/// </summary>
	public enum RdlCommonCommand
	{
		/// <summary>
		/// Specifies a QUIT command.
		/// </summary>
		QUIT,
		/// <summary>
		/// Specifies a HELP command.
		/// </summary>
		HELP,
		/// <summary>
		/// Specifies a BUG command.
		/// </summary>
		BUG,
		/// <summary>
		/// Specifies a SAY command.
		/// </summary>
		SAY,
		/// <summary>
		/// Specifies a SHOUT command.
		/// </summary>
		SHOUT,
		/// <summary>
		/// Specifies a TELL command.
		/// </summary>
		TELL,
		/// <summary>
		/// Specifies a LOOK command.
		/// </summary>
		LOOK,
		/// <summary>
		/// Specifies a MOVE command.
		/// </summary>
		MOVE,
		/// <summary>
		/// Specifies a ATTACK command.
		/// </summary>
		ATTACK,
		/// <summary>
		/// Specifies a CAST command.
		/// </summary>
		CAST,
		/// <summary>
		/// Specifies a GET command.
		/// </summary>
		GET,
		/// <summary>
		/// Specifies a DROP command.
		/// </summary>
		DROP,
	}
}
