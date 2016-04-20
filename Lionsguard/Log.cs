using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Configuration;

using Lionsguard.Configuration;
using Lionsguard.Data;
using Lionsguard.Providers;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Lionsguard
{
	public static class Log
	{
		public static event LogWriteEventHandler WriteLogEntry = delegate { };

		public static string LogFilePath { get; set; }

		public static void Write(string text)
		{
			Log.Write(text, false, null);
		}

		public static void Write(string text, params object[] args)
		{
			Log.Write(text, false, args);
		}

		public static void Write(string text, bool sendEmail)
		{
			Log.Write(text, sendEmail, null);
		}

		public static void Write(string text, bool sendEmail, params object[] args)
		{
			ThreadPool.QueueUserWorkItem((o) =>
			{
				try
				{
					var msg = o as LogMessage;
					if (msg == null) return;

					if (msg.Args != null && msg.Args.Length > 0)
					{
						msg.Text = String.Format(msg.Text, msg.Args);
					}

					LogWriteEventArgs e = new LogWriteEventArgs() { Message = msg.Text };
					WriteLogEntry(e);
					StringBuilder sb = new StringBuilder();

					if (e.Handled)
					{
						sb.Append(e.Message);
					}
					else
					{
						// Append the current request details to the log.
						HttpContext context = HttpContext.Current;
						if (context != null)
						{
							sb.AppendFormat("RequestUrl: {0}", context.Request.Url.ToString()).Append(Environment.NewLine);
							sb.AppendFormat("IP Address: {0}", context.Request.UserHostAddress).Append(Environment.NewLine);
							sb.AppendFormat("User Agent: {0}", context.Request.UserAgent).Append(Environment.NewLine);
							sb.AppendFormat("IsAuthenticated: {0}", context.Request.IsAuthenticated).Append(Environment.NewLine);
							sb.AppendFormat("AUTH_USER: {0}", context.Request.ServerVariables["AUTH_USER"]).Append(Environment.NewLine);
							if (context.Request.QueryString.Count > 0)
							{
								sb.AppendFormat("QueryString: {0}", context.Request.QueryString.ToString());
							}
							if (context.Request.Form.Count > 0)
							{
								sb.Append("Form:").AppendLine();
								foreach (var item in context.Request.Form.AllKeys)
								{
									sb.AppendFormat("{0} = {1}", item, context.Request.Form[item]).AppendLine();
								}
							}
						}
						sb.AppendFormat("{0}", msg.Text);

#if !DEBUG
						Notification.Provider.AddLogEntry(sb.ToString());
#endif
					}

#if !DEBUG
					if (msg.SendEmail)
					{
						Notification.SendEmail(new MailAddress(Notification.AdminEmail, "Admin"),
							new MailAddress(Notification.AdminEmail, "Admin"), Notification.EmailSubject, sb.ToString(), MailPriority.High);
					}

					// Write to a log file.
					if (!String.IsNullOrEmpty(LogFilePath))
					{
						var now = DateTime.Now;
						string path = Path.Combine(LogFilePath, String.Format("Log_{0}{1}{2}.txt", now.Year,
							now.Month.ToString().PadLeft(2, '0'),
							now.Day.ToString().PadLeft(2, '0')));

						if (!Directory.Exists(LogFilePath))
						{
							Directory.CreateDirectory(LogFilePath);
						}
						using (var fs = File.Open(path, FileMode.Append, FileAccess.Write))
						{
							using (var writer = new StreamWriter(fs))
							{
								writer.WriteLine("[{0}] {1}", now.ToString("yyyyMMdd hh:mm:ss t"), sb.ToString());
							}
						}
					}
#endif
				}
				catch (Exception) { }
			},
			new LogMessage
			{
				Text = text,
				SendEmail = sendEmail,
				Args = args
			});
		}
		private class LogMessage
		{
			public string Text { get; set; }
			public bool SendEmail { get; set; }
			public object[] Args { get; set; }
		}

		public static List<LogInfo> GetLogs(int startingRowIndex, int maxRows)
		{
			return Notification.Provider.GetLogs(startingRowIndex, maxRows);
		}

		public static int GetLogCount()
		{
			return Notification.Provider.GetLogCount();
		}

		public static void DeleteLog(int ID)
		{
			Notification.Provider.DeleteLog(ID);
		}

		public static void ClearLogs()
		{
			Notification.Provider.DeleteAllLogs();
		}
	}

	public class LogInfo
	{
		public int ID { get; set; }
		public string Text { get; set; }
		public DateTime DateCreated { get; set; }

		public LogInfo(int id, string text, DateTime dateCreated)
		{
			this.ID = id;
			this.Text = text;
			this.DateCreated = dateCreated;
		}
	}

	public delegate void LogWriteEventHandler(LogWriteEventArgs e);
	public class LogWriteEventArgs : EventArgs
	{
		public bool Handled { get; set; }
		public string Message { get; set; }
	}

	public class LogTraceListener : TraceListener
	{
		public override void Write(string message)
		{
			WriteLine(message);
		}

		public override void WriteLine(string message)
		{
			Log.Write(message);
		}
	}
}
