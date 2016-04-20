using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Lionsguard.Data;
using Lionsguard.Providers;

namespace Lionsguard.Providers
{
	public class SqlNotificationProvider : NotificationProvider
	{
		private string _connectionString;

		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			base.Initialize(name, config);

			_connectionString = ProviderUtil.GetConnectionString(config);

			ProviderUtil.CheckUnrecognizedAttributes(config);
		}

		public override void AddLogEntry(string text)
		{
			SqlHelper.ExecuteNonQuery(_connectionString, "INSERT INTO dbo.lg_Log (LogText) VALUES (@Text);", CommandType.Text, 
				SqlHelper.CreateInputParam("@Text", SqlDbType.NVarChar, text));
		}

		public override List<LogInfo> GetLogs(int startingRowIndex, int maxRows)
		{
			List<LogInfo> list = new List<LogInfo>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Log_GetLogs",
				SqlHelper.CreateInputParam("@StartRowIndex", SqlDbType.Int, startingRowIndex),
				SqlHelper.CreateInputParam("@MaxRows", SqlDbType.Int, maxRows)))
			{
				while (reader.Read())
				{
					list.Add(new LogInfo(reader.GetInt32("LogId"), reader.GetString("LogText"), reader.GetDateTime("DateCreated")));
				}
			}
			return list;
		}

		public override int GetLogCount()
		{
			object obj = SqlHelper.ExecuteScalar(_connectionString, "SELECT COUNT(*) FROM dbo.lg_Log", CommandType.Text);
			if (obj != null && obj != DBNull.Value)
			{
				return Convert.ToInt32(obj);
			}
			return 0;
		}

		public override void DeleteLog(int logId)
		{
			SqlHelper.ExecuteNonQuery(_connectionString, "DELETE FROM dbo.lg_Log WHERE LogId = @LogId;", CommandType.Text,
				SqlHelper.CreateInputParam("@LogId", SqlDbType.Int, logId));
		}

		public override void DeleteAllLogs()
		{
			SqlHelper.ExecuteNonQuery(_connectionString, "DELETE FROM dbo.lg_Log", CommandType.Text);
		}
	}
}
