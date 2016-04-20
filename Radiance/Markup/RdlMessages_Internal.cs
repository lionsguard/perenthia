using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance.Markup
{
	/// <summary>
	/// Represents an MSG|ERROR tag in the Radiance Definition Language (RDL).
	/// </summary>
	public partial class RdlErrorMessage
	{
		/// <summary>
		/// Gets a MessageErrorTag for an invalid system command.
		/// </summary>
		public static RdlErrorMessage InvalidCommand = new RdlErrorMessage(SR.InvalidCommand);

		/// <summary>
		/// Gets a MessageErrorTag for a generic internal error.
		/// </summary>
		public static RdlErrorMessage InternalError = new RdlErrorMessage(SR.InternalError);

		/// <summary>
		/// Gets a MessageErrorTag for an unauthenticated attempt to access the server.
		/// </summary>
		public static RdlErrorMessage LoginRequiredError = new RdlErrorMessage(SR.LoginRequired);
	}
}
