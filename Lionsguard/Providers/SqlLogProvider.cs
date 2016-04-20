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
	public class SqlLogProvider : LogProvider
	{
		private string _connectionString;

		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			base.Initialize(name, config);

			_connectionString = ProviderUtil.GetConnectionString(config);

			ProviderUtil.CheckUnrecognizedAttributes(config);
		}

		public override void Write(string text)
		{
			SqlHelper.ExecuteNonQuery(_connectionString, "INSERT INTO dbo.lg_Log (LogText) VALUES (@Text);", CommandType.Text, 
				SqlHelper.CreateInputParam("@Text", SqlDbType.NVarChar, text));
		}
	}
}
