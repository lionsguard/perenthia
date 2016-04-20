using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using Radiance.Configuration;
using Radiance.Providers;

namespace Radiance
{
	/// <summary>
	/// Represents static methods for logging virtual world activity.
	/// </summary>
	public static class Log
	{
		#region Initialize
		private static LogProvider _provider = null;
		private static LogProviderCollection _providers = null;
		internal static void Initialize(LogSection section)
		{
			_providers = new LogProviderCollection();
			_provider = ProviderUtil.InstantiateProviders<LogProvider, LogProviderCollection>(
				section.Providers, section.DefaultProvider, _providers);
			if (_provider == null)
			{
				throw new ConfigurationErrorsException(
					SR.ConfigDefaultLogProviderNotFound,
					section.ElementInformation.Properties["defaultProvider"].Source,
					section.ElementInformation.Properties["defaultProvider"].LineNumber);
			}
		}
		#endregion

		/// <summary>
		/// Write the specified text to the log.
		/// </summary>
		/// <param name="logType">The type of log event to write.</param>
		/// <param name="text">The text to write to the log.</param>
		public static void Write(LogType logType, string text)
		{
			_provider.Write(logType, text);
		}

		/// <summary>
		/// Writes the specified text to the log after performing formatting on the string using the specified args.
		/// </summary>
		/// <param name="logType">The type of log event to write.</param>
		/// <param name="text">The text to format and add to the log.</param>
		/// <param name="args">The arguments to use when formatting the text, uses String.Format.</param>
		public static void Write(LogType logType, string text, params object[] args)
		{
			_provider.Write(logType, text, args);
		}

		/// <summary>
		/// Writes the specified text to the log and sends and email with the results.
		/// </summary>
		/// <param name="logType">The type of log event to write.</param>
		/// <param name="text">The text to write to the log.</param>
		/// <param name="sendEmail">A value indicating whether or not to send an email with the log details.</param>
		public static void Write(LogType logType, string text, bool sendEmail)
		{
			_provider.Write(logType, text, sendEmail);
		}

		/// <summary>
		/// Writes the specified text to the log after performing formatting on the string using the specified args 
		/// and sends and email with the results.
		/// </summary>
		/// <param name="logType">The type of log event to write.</param>
		/// <param name="text">The text to format and add to the log.</param>
		/// <param name="sendEmail">A value indicating whether or not to send an email with the log details.</param>
		/// <param name="args">The arguments to use when formatting the text, uses String.Format.</param>
		public static void Write(LogType logType, string text, bool sendEmail, params object[] args)
		{
			_provider.Write(logType, text, sendEmail, args);
		}

		/// <summary>
		/// Write a command specified log entry.
		/// </summary>
		/// <param name="userName">The username of the current user.</param>
		/// <param name="authKey">The AuthKey of the current user.</param>
		/// <param name="text">The command or response text.</param>
		/// <param name="isCommand">A value indicating whether or not this is a command or response.</param>
		public static void WriteCommand(string userName, string authKey, string text, bool isCommand)
		{
			string msg = String.Format("{0}: [UserName: {1}] [AuthKey: {2}] {3}", 
				(isCommand ? "COMMANDS" : "RESPONSE"), userName, authKey, text);

			_provider.Write(LogType.Command, msg);
		}
	}

	/// <summary>
	/// Provides an enumeration of log types.
	/// </summary>
	public enum LogType : byte
	{
		/// <summary>
		/// Specifies an error.
		/// </summary>
		Error		= 0x0,
		/// <summary>
		/// Specifies a command.
		/// </summary>
		Command		= 0x1,
		Information = 0x2,
		Warning		= 0x3,
	}
}
