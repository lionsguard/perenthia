using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance.Markup
{
	/// <summary>
	/// Defines an enumerator of Radiance Definition Language (RDL) tag names.
	/// </summary>
	public enum RdlTagName
	{
		/// <summary>
		/// Specifies an empty tag with no arguments.
		/// </summary>
		EMPTY,
		/// <summary>
		/// Specifies a virtual world object tag.
		/// </summary>
		OBJ,
		/// <summary>
		/// Specifies a message tag.
		/// </summary>
		MSG,
		/// <summary>
		/// Specifies a command tag.
		/// </summary>
		CMD,
		/// <summary>
		/// Specifies a user tag.
		/// </summary>
		USER,
		/// <summary>
		/// Secifies a system tag.
		/// </summary>
		SYS,
		/// <summary>
		/// Specifies a response to a specific command.
		/// </summary>
		RESP,
		/// <summary>
		/// Specifies an authentication tag.
		/// </summary>
		AUTH,
	}

	public enum RdlObjectTypeName
	{
		PROP,
		ACTOR,
		PLAYER,
		PLACE,
		RACE,
		TERRAIN,
		SKILL,
	}

	public enum RdlMessageTypeName
	{
		ERROR,
		SYSTEM,
		NEWS,
		CHAT,
		TELL,
	}

	public enum RdlSystemTypeName
	{
		LOGINFAILED,
	}

	public enum RdlCommandParserErrorType
	{
		None,
		NoTargetForTell,
		NoArgumentsSpecified,
		InvalidNumberOfArguments,
	}
}
