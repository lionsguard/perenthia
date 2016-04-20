using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Radiance;
using Perenthia.ServiceModel;
using Radiance.Markup;
using Lionsguard;
using System.Threading;

namespace Perenthia.Web.Services
{
	/// <summary>
	/// Summary description for $codebehindclassname$
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class HttpGameService : IHttpHandler, ICommunicationHandler
	{
		private ManualResetEvent _waitHandle = new ManualResetEvent(false);

		public bool IsReusable
		{
			get { return false; }
		}

		public void ProcessRequest(HttpContext context)
		{
			var response = new List<byte>();

			try
			{
				// Read the bytes from the request stream and create a Message from it.
				var buffer = new byte[context.Request.ContentLength];
				context.Request.InputStream.Read(buffer, 0, context.Request.ContentLength);
				if (buffer != null && buffer.Length > 0)
				{
					var perenthiaSessionId = context.Request.Headers.Get(HttpHeaders.RadianceSessionIdHeaderKey);

					Guid sessionId;
					if (!Lionsguard.Util.GuidTryParse(perenthiaSessionId, out sessionId))
						sessionId = Guid.NewGuid();

					IClient client = null;
					var waitForTags = false;
					var commands = RdlCommandGroup.FromBytes(buffer);
					if (commands.Count == 1 && commands[0].TypeName == RdlCommand.Heartbeat.TypeName)
					{
						Game.Server.Clients.TryGetClient(sessionId, out client);
					}
					else
					{
						waitForTags = true;
						client = Game.Server.ProcessCommands(this, commands, sessionId, context.Request.UserHostAddress);
					}
					if (client != null)
					{
						// Wait for the client to get some tags.
						if (waitForTags)
						{
							while (client.Context.Count == 0)
							{
								_waitHandle.WaitOne(TimeSpan.FromSeconds(1));
							}
						}

						RdlTag tag;
						while (client.Context.Read(out tag))
						{
							response.AddRange(tag.ToBytes());
						}
						Logger.LogDebug("SERVER: Sending {0} bytes to the client.", response.Count);
					}
				}
			}
			catch (Exception ex)
			{
#if DEBUG
				throw ex;
#else
				Lionsguard.Log.Write(ex.ToString(), true);
#endif
			}

			context.Response.ContentType = "binary/octet-stream";

			if (response.Count > 0)
				context.Response.BinaryWrite(response.ToArray());
			else
				context.Response.Write(String.Empty);
		}

		#region ICommunicationHandler Members

		public IClient CreateClient(Guid sessionId)
		{
			return new NetworkClient(sessionId);
		}

		#endregion
	}
}
