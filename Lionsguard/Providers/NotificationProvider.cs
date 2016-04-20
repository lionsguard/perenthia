using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

namespace Lionsguard.Providers
{
	public abstract class NotificationProvider : ProviderBase
	{
		public abstract void AddLogEntry(string text);
		public abstract List<LogInfo> GetLogs(int startingRowIndex, int maxRows);
		public abstract int GetLogCount();
		public abstract void DeleteLog(int logId);
		public abstract void DeleteAllLogs();
	}
}
