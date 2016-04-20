using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Lionsguard
{
	public static partial class Logger
	{
		public static void LogInformation(string message)
		{
			Write(LogSeverity.Information, message, null);
		}
		public static void LogInformation(string message, params object[] args)
		{
			Write(LogSeverity.Information, message, args);
		}

		public static void LogWarning(string message)
		{
			Write(LogSeverity.Warning, message, null);
		}
		public static void LogWarning(string message, params object[] args)
		{
			Write(LogSeverity.Warning, message, args);
		}

		public static void LogError(string message)
		{
			Write(LogSeverity.Error, message, null);
		}
		public static void LogError(string message, params object[] args)
		{
			Write(LogSeverity.Error, message, args);
		}

		public static void LogDebug(string message)
		{
			Write(LogSeverity.Debug, message, null);
		}
		public static void LogDebug(string message, params object[] args)
		{
			Write(LogSeverity.Debug, message, args);
		}

		public static void LogCommand(string message)
		{
			Write(LogSeverity.Command, message, null);
		}
		public static void LogCommand(string message, params object[] args)
		{
			Write(LogSeverity.Command, message, args);
		}

		public static void Write(LogSeverity severity, string message)
		{
			Write(severity, message, null);
		}
		
	}
}
