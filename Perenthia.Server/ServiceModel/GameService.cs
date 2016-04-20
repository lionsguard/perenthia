using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radiance;
using Radiance.Contract;
using System.ServiceModel.Activation;
using System.ServiceModel;
using Radiance.Markup;
using Lionsguard;
using System.Threading;
using Microsoft.ServiceModel.PollingDuplex.Scalable;
using System.ServiceModel.Channels;

namespace Perenthia.ServiceModel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
	public class GameService : IGameService, ICommunicationHandler
	{
		public static readonly TimeSpan PollTimeout = TimeSpan.FromSeconds(10);

		#region IGameService Members

		public void Process(byte[] data)
		{
			var session = OperationContext.Current.GetPollingDuplexSession();

			Logger.LogDebug("SERVER: Recieved data from {0} on Thread [ {1} ]", session.SessionId, Thread.CurrentThread.ManagedThreadId);
			var commands = RdlCommandGroup.FromBytes(data);
			if (commands != null && commands.Count > 0)
			{
				Game.Server.ProcessCommands(this, commands, new Guid(session.SessionId), session.Address);
			}
		}

		#endregion

		#region ICommunicationHandler Members

		public IClient CreateClient(Guid sessionId)
		{
			return new NetworkClient(sessionId);
		}

		#endregion

		#region IPollingDuplex Members

		public IAsyncResult BeginMakeConnect(MakeConnection poll, AsyncCallback callback, object state)
		{
			Logger.LogDebug("SERVER: BeginMakeConnect on Thread [ {0} ]", Thread.CurrentThread.ManagedThreadId);

			IClient client;
			Game.Server.Clients.TryGetClient(poll.Address, out client);

			return new HeartbeatAsyncResult(poll, client, PollTimeout, callback, state);
		}

		public Message EndMakeConnect(IAsyncResult result)
		{
			Logger.LogDebug("SERVER: EndMakeConnect on Thread [ {0} ]", Thread.CurrentThread.ManagedThreadId);
			return HeartbeatAsyncResult.End(result);
		}

		#endregion

	}

	public class HeartbeatAsyncResult : AsyncResult
	{
		private delegate RdlTag[] MethodCall(IClient client, TimeSpan timeout);

		public string SessionId { get; set; }
		public IClient Client { get; set; }
		public RdlTagCollection Tags { get; private set; }
		public MakeConnection Poll { get; set; }
		public TimeSpan Timeout { get; set; }

		public HeartbeatAsyncResult(MakeConnection poll, IClient client, TimeSpan timeout, AsyncCallback callback, object state)
			: base(callback, state)
		{
			this.Poll = poll;
			this.Timeout = timeout;
			this.Tags = new RdlTagCollection();
			this.Client = client;
			if (client != null)
				this.SessionId = client.SessionId.ToString();

			MethodCall method = HeartbeatAsyncResult.PollClient;
			method.BeginInvoke(this.Client, this.Timeout, (ar) =>
				{
					var tags = method.EndInvoke(ar);
					var result = ar.AsyncState as HeartbeatAsyncResult;

					result.Tags.AddRange(tags);

					result.Complete(ar.CompletedSynchronously);

				}, this);
		}

		private static RdlTag[] PollClient(IClient client, TimeSpan timeout)
		{
			var waitTime = 0D;
			var tags = new List<RdlTag>();
			var waitHandle = new ManualResetEvent(false);

			try
			{
				if (client != null)
				{
					if (client.Context.Count > 0)
					{
						WriteTags(client, tags);
					}
					else
					{
						while (waitTime < timeout.TotalSeconds)
						{
							if (waitHandle.WaitOne(1000))
							{
								if (client.Context.Count > 0)
								{
									WriteTags(client, tags);
									break;
								}
							}
							waitTime++;
							Logger.LogDebug("Polling client for messages on Thread [ {0} ]: {1} | {2}",
								Thread.CurrentThread.ManagedThreadId, waitTime, DateTime.Now.Ticks);
						}
					}
				}
				return tags.ToArray();
			}
			finally
			{
				waitHandle.Close();
			}
		}

		private static void WriteTags(IClient client, List<RdlTag> tags)
		{
			if (client == null)
				return;

			RdlTag tag;
			while (client.Context.Read(out tag))
			{
				tags.Add(tag);
			}
		}

		public static Message End(IAsyncResult result)
		{
			var asyncResult = (HeartbeatAsyncResult)result;

			AsyncResult.End<HeartbeatAsyncResult>(result);

			var response = asyncResult.Poll.CreateEmptyResponse();
			if (asyncResult.Tags.Count > 0)
			{
				Logger.LogDebug("Sending {0} tags to the client.", asyncResult.Tags.Count);
				response = Message.CreateMessage(MessageVersion.Default, Constants.GameServiceReceive, asyncResult.Tags.ToBytes());

				response = asyncResult.Poll.PrepareRespose(response, asyncResult.SessionId);
			}

			return response;
		}
	}
}
