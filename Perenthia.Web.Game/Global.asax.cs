using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Lionsguard;
using Lionsguard.Security;
using Radiance;
using System.Text;
using System.Threading;
using System.IO;
using Perenthia.Net;
using Perenthia.Utility;
using System.Net;
using System.Diagnostics;
using System.Web.Hosting;
using Radiance.Contract;
using Perenthia.ServiceModel;

namespace Perenthia.Web
{
	public class Global : System.Web.HttpApplication
	{
		private Thread _serverThread = new Thread(StartGameServer);
		private PolicyServer _policyServer;
		private SocketServer _socketServer;

		protected void Application_Start(object sender, EventArgs e)
		{
			//===========================================================================
			// Setup logging.
			//===========================================================================
			Lionsguard.Log.LogFilePath = Server.MapPath("/Logs");
			Lionsguard.Log.WriteLogEntry += new LogWriteEventHandler(Log_WriteLogEntry);

			Logger.LogMessage += Logger_LogMessage;

			//===========================================================================
			// Setup world event handlers.
			//===========================================================================
			Game.Server.World.PlayerAdvanced += new PlayerAdvancedEventHandler(World_PlayerAdvanced);
			Game.Server.World.AvatarKilledAvatar += new AvatarKilledAvatarEventHandler(World_AvatarKilledAvatar);

			//===========================================================================
			// Start the policy server.
			//===========================================================================
			var policyEndPoint = new IPEndPoint(IPAddress.Any, 943);
			_policyServer = new PolicyServer(Encoding.UTF8.GetBytes(Depot.ClientAccessPolicyData), policyEndPoint);
			_policyServer.Error += new NetworkExceptionEventHandler(_policyServer_Error);
			_policyServer.RequestReceived += new PolicyRequestReceivedEventHandler(_policyServer_RequestReceived);
			_policyServer.Start();

			Logger.LogInformation("Policy Server Started on {0}", policyEndPoint);

			//===========================================================================
			// Start the game socket server.
			//===========================================================================
			var gameEndPoint = new IPEndPoint(IPAddress.Any, 4530);
			_socketServer = new SocketServer(gameEndPoint);
			_socketServer.Start();

			Logger.LogInformation("Game Server Started on {0}", gameEndPoint);


			_serverThread.Name = "MainGameThread";
			_serverThread.Start();
		}

		private static void StartGameServer()
		{
			Logger.LogInformation("Starting Game Server");
			Game.Server.Start(Game.HeartbeatInterval);
		}

		#region Policy Server Event Handlers
		private void _policyServer_RequestReceived(PolicyRequestReceivedEventArgs e)
		{
		}

		private void _policyServer_Error(NetworkExceptionEventArgs e)
		{
			Logger.LogError(e.Exception.ToString());
		}
		#endregion

		#region World Event Handlers
		private static string[] VictoryMessages = new string[]
        {
            "{0} has defeated {1}!",
            "{0} has defeated {1} in a battle to the death!",
            "{1} was thrashed by {0}!",
            "{0} delivered a solid beating to {1}!",
            "{1} was beaten into a bloody pulp by {0}!",
            "{0} mopped the floor with {1}!"
        };
		private void World_AvatarKilledAvatar(IAvatar attacker, IAvatar defender)
		{
			int index = Dice.Random(0, VictoryMessages.Length - 1);
			string msg = String.Format(VictoryMessages[index], attacker.AUpper(), defender.A());
			Game.UpdateTwitter(Strings.EnsureProperSentence(msg));
		}

		private void World_PlayerAdvanced(IPlayer player, string message)
		{
			Game.UpdateTwitter(message);
		}

		private void Logger_LogMessage(LoggerEventArgs e)
		{
#if DEBUG
			switch (e.Severity)
			{
				case LogSeverity.Warning:
					Trace.TraceWarning(e.Text);
					break;
				case LogSeverity.Error:
					Trace.TraceError(e.Text);
					break;
				case LogSeverity.Information:
					Trace.TraceInformation(e.Text);
					break;
				case LogSeverity.Command:
					Trace.TraceInformation(e.Text);
					break;
				default:
					// Debug
					Debug.WriteLine(e.Text);
					break;
			}
#else
			var sendEmail = (e.Severity == LogSeverity.Warning || e.Severity == LogSeverity.Error);
			if (e.Severity != LogSeverity.Debug && e.Severity != LogSeverity.Information)
				Lionsguard.Log.Write(e.Text, sendEmail);
#endif
		}

		private void Log_WriteLogEntry(LogWriteEventArgs e)
		{
			StringBuilder sb = new StringBuilder();

			HttpContext context = HttpContext.Current;
			if (context != null)
			{
				string username = context.Request.ServerVariables["AUTH_USER"];
				sb.AppendFormat("RequestUrl: {0}", context.Request.Url.ToString()).Append(Environment.NewLine);
				sb.AppendFormat("IP Address: {0}", context.Request.UserHostAddress).Append(Environment.NewLine);
				sb.AppendFormat("User Agent: {0}", context.Request.UserAgent).Append(Environment.NewLine);
				sb.AppendFormat("IsAuthenticated: {0}", context.Request.IsAuthenticated).Append(Environment.NewLine);
				sb.AppendFormat("AUTH_USER: {0}", username).Append(Environment.NewLine);

				IClient client = null;
				if (Game.Server.Clients.ContainsKey(username))
				{
					client = Game.Server.Clients[username];
				}

				if (client != null)
				{
					sb.AppendFormat("LastCommandGroup: {0}", client.LastCommandGroup).AppendLine();
					if (client.Player != null)
					{
						sb.AppendFormat("Player ID: {0}", client.Player.ID).AppendLine();
						sb.AppendFormat("Player Name: {0}", client.Player.Name).AppendLine();
						sb.AppendFormat("Location: {0}", client.Player.Location.ToString(true, true)).AppendLine();
						sb.AppendFormat("Player Level: {0}", client.Player.Properties.GetValue<int>("Level")).AppendLine();
					}
				}
			}
			sb.AppendFormat("{0}", e.Message);

			e.Handled = true;
			e.Message = sb.ToString();
		}
		#endregion

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{
			SecurityManager.AuthenticateRequest(HttpContext.Current);
		}

		protected void Application_Error(object sender, EventArgs e)
		{
			var ex = Server.GetLastError();
			if (ex != null)
			{
				if (ex is FileNotFoundException)
				{
					Logger.LogError("File Not Found:{0}, Exception:{1}", (ex as FileNotFoundException).FileName, ex.ToString());
				}
				else if (ex is HttpException)
				{
					Logger.LogError("Request Url:{0}, Exception:{1}", this.Request.Url, ex.ToString());
				}
				else
				{
					Logger.LogError(ex.ToString());
				}
			}
			Server.ClearError();
		}

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{
			Logger.LogInformation("Stopping Game Server");
			if (_policyServer != null) _policyServer.Stop();
			if (_socketServer != null) _socketServer.Stop();
			Game.Server.Stop();
			_serverThread.Abort();
		}
	}
}