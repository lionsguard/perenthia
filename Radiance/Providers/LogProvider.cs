using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

namespace Radiance.Providers
{
	/// <summary>
	/// Represents the abstract base class used to provide application level logging.
	/// </summary>
	public abstract class LogProvider : ProviderBase
	{
		/// <summary>
		/// Gets the name of the current world.
		/// </summary>
		public string WorldName { get; set; }

		/// <summary>
		/// Write the specified text to the log.
		/// </summary>
		/// <param name="logType">The type of log event to write.</param>
		/// <param name="text">The text to write to the log.</param>
		public abstract void Write(LogType logType, string text);

		/// <summary>
		/// Writes the specified text to the log after performing formatting on the string using the specified args.
		/// </summary>
		/// <param name="logType">The type of log event to write.</param>
		/// <param name="text">The text to format and add to the log.</param>
		/// <param name="args">The arguments to use when formatting the text, uses String.Format.</param>
		public abstract void Write(LogType logType, string text, params object[] args);

		/// <summary>
		/// Writes the specified text to the log and sends and email with the results.
		/// </summary>
		/// <param name="logType">The type of log event to write.</param>
		/// <param name="text">The text to write to the log.</param>
		/// <param name="sendEmail">A value indicating whether or not to send an email with the log details.</param>
		public abstract void Write(LogType logType, string text, bool sendEmail);

		/// <summary>
		/// Writes the specified text to the log after performing formatting on the string using the specified args 
		/// and sends and email with the results.
		/// </summary>
		/// <param name="logType">The type of log event to write.</param>
		/// <param name="text">The text to format and add to the log.</param>
		/// <param name="sendEmail">A value indicating whether or not to send an email with the log details.</param>
		/// <param name="args">The arguments to use when formatting the text, uses String.Format.</param>
		public abstract void Write(LogType logType, string text, bool sendEmail, params object[] args);
	}
}
