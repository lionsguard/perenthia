using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using Lionsguard;
using Lionsguard.Data;
using Lionsguard.Providers;
using Radiance;
using Radiance.Markup;
using Radiance.Providers;

namespace Perenthia
{
	public class SqlLogProvider : LogProvider
	{
		#region Initialize
		private string _connectionString;
		private object _lock = new object();

		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			base.Initialize(name, config);

			_connectionString = ProviderUtil.GetConnectionString(config);

			this.WorldName = ProviderUtil.GetAndRemoveStringAttribute(config, "worldName");

			ProviderUtil.CheckUnrecognizedAttributes(config);
		}
		#endregion

		public override void Write(LogType logType, string text)
		{
			this.Write(logType, text, false, null);
		}

		public override void Write(LogType logType, string text, params object[] args)
		{
			this.Write(logType, text, false, args);
		}

		public override void Write(LogType logType, string text, bool sendEmail)
		{
			this.Write(logType, text, sendEmail, null);
		}

		public override void Write(LogType logType, string text, bool sendEmail, params object[] args)
		{
			this.WriteInternal(logType, text, sendEmail, args);
		}

		private void WriteInternal(LogType logType, string text, bool sendEmail, params object[] args)
		{
			Lionsguard.Log.Write(text, sendEmail, args);
			//string msg = text;
			//if (args != null && args.Length > 0) msg = String.Format(text, args);

			////using (SqlCommand cmd = SqlHelper.ExecuteNonQuery(_connectionString, "dbo.rad_Logs_Save",
			////    SqlHelper.CreateInputParam("@WorldName", SqlDbType.NVarChar, this.WorldName),
			////    SqlHelper.CreateInputParam("@LogTypeId", SqlDbType.TinyInt, (byte)logType),
			////    SqlHelper.CreateInputParam("@LogText", SqlDbType.NVarChar, msg))) { }

			//// Write to a log file rather than the database.
			//try
			//{
			//    HttpContext context = HttpContext.Current;
			//    if (context != null)
			//    {
			//        if (!Directory.Exists(context.Server.MapPath("/Logs")))
			//        {
			//            lock (_lock)
			//            {
			//                if (!Directory.Exists(context.Server.MapPath("/Logs")))
			//                {
			//                    Directory.CreateDirectory(context.Server.MapPath("/Logs"));
			//                }
			//            }
			//        }
			//        string fileName = String.Concat(context.Server.MapPath("/Logs/"), "PerenthiaLog", DateTime.Now.ToString("yyyyMMdd"), ".log");
			//        FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
			//        using (StreamWriter writer = new StreamWriter(fs))
			//        {
			//            writer.WriteLine("[ {0} ] [ {1} ] {2}", DateTime.Now, logType, msg);
			//        }
			//    }
			//}
			//catch (Exception ex)
			//{
			//    Lionsguard.Log.Write(ex.ToString(), true);
			//}

			//if (logType == LogType.Error && sendEmail)
			//{
			//    // Log this with the Lionsguard Logging and send the email.
			//    Lionsguard.Log.Write(msg, sendEmail);
			//}
		}
	}
}
