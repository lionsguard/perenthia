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
	public class SqlSettingsProvider : SettingsProvider
	{
		private string _connectionString;

		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			base.Initialize(name, config);

			_connectionString = ProviderUtil.GetConnectionString(config);

			ProviderUtil.CheckUnrecognizedAttributes(config);
		}

		public override Dictionary<string, string> GetSettings()
		{
			Dictionary<string, string> dic = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Settings_GetAll"))
			{
				while (reader.Read())	
				{
					dic.Add(reader.GetString("SettingName"), reader.GetString("SettingValue"));
				}
			}
			return dic;
		}

		public override void SaveSetting(string name, string value)
		{
			SqlHelper.ExecuteNonQuery(_connectionString, "dbo.lg_Settings_SaveSetting",
				SqlHelper.CreateInputParam("@SettingName", SqlDbType.NVarChar, name),
				SqlHelper.CreateInputParam("@SettingValue", SqlDbType.NVarChar, value));
		}
	}
}
