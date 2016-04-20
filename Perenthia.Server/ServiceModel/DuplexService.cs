using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Radiance;
using Radiance.Markup;
using Radiance.Contract;
using Microsoft.ServiceModel.PollingDuplex.Scalable;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Threading;
using System.ServiceModel.Channels;
using Lionsguard;
using System.ServiceModel.Dispatcher;

namespace Perenthia.ServiceModel
{
	/// <summary>
	/// Represents a WCF polling duplex network service.
	/// </summary>
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
	public class DuplexService : IGameService, ICommunicationHandler, IPollingDuplex
	{
		private TimeSpan _pollTimeout = TimeSpan.FromMilliseconds(Game.HeartbeatInterval);

		#region IDuplexService Members

		/// <summary>
		/// Processes the packet data for a connected client.
		/// </summary>
		/// <param name="data">The packet data to process.</param>
		public void Process(byte[] data)
		{
			Logger.LogDebug("SERVER: DuplexService.Process Entered.");
			ThreadPool.QueueUserWorkItem((o) =>
			{
				var info = (SessionInfo)o;

				Logger.LogDebug("SERVER: Executing DuplexService.Process on Thread {0}", Thread.CurrentThread.ManagedThreadId);
				var commands = RdlCommandGroup.FromBytes(info.Data);
				if (commands.Count > 0)
				{
					var client = Game.Server.Clients.Where(c => c.SessionId.ToString().Equals(info.Session.SessionId)).FirstOrDefault();
					if (client == null)
					{
						Game.Server.ProcessCommands(this, commands, info.Session);
					}
					else
					{
						Game.Server.QueueRequest(this, commands, info.Session);
					}
				}
				Logger.LogDebug("SERVER: Completed DuplexService.Process.");

			}, new SessionInfo { Data = data, Session = OperationContext.Current.GetPollingDuplexSession() });

			//var commands = RdlCommandGroup.FromBytes(data);
			//if (commands.Count > 0)
			//{
			//    var session = OperationContext.Current.GetPollingDuplexSession();
			//    var client = Game.Server.Clients.Where(c => c.SessionId.ToString().Equals(session.SessionId)).FirstOrDefault();
			//    if (client == null)
			//    {
			//        Game.Server.ProcessCommands(this, commands, OperationContext.Current.GetPollingDuplexSession());
			//    }
			//    else
			//    {
			//        Game.Server.QueueRequest(this, commands, OperationContext.Current.GetPollingDuplexSession());
			//    }
			//}

			Logger.LogDebug("SERVER: DuplexService.Process Exited.");
		}

		class SessionInfo
		{
			public byte[] Data { get; set; }
			public PollingDuplexSession Session { get; set; }	
		}

		#endregion

		#region ICommunicationHandler Members

		public IClient CreateClient(object arg)
		{
			return new DuplexNetworkClient(arg as PollingDuplexSession);
		}

		#endregion

		#region IPollingDuplex Members

		public IAsyncResult BeginMakeConnect(MakeConnection poll, AsyncCallback callback, object state)
		{
			Logger.LogDebug("SERVER: BeginMakeConnect {0}", poll.Address);
			var client = Game.Server.Clients.Where(c => c.Address == poll.Address).FirstOrDefault();
			PollingDuplexSession session = null;
			if (client != null)
				session = new PollingDuplexSession(client.Address, client.SessionId.ToString());
			else
				session = new PollingDuplexSession(poll.Address, String.Empty);

			var ar = new DequeueAsyncResult(poll, session, _pollTimeout, callback, state);

			ThreadPool.QueueUserWorkItem(StartMessageRead, ar);

			return ar;
		}

		public Message EndMakeConnect(IAsyncResult result)
		{
			var asyncResult = (DequeueAsyncResult)result;
			Logger.LogDebug("SERVER: EndMakeConnect {0}", asyncResult.Poll.Address);
			var response = asyncResult.Poll.CreateEmptyResponse();
			if (asyncResult.Tags.Count > 0)
			{
				Logger.LogDebug("Sending {0} tags to the client.", asyncResult.Tags.Count);
				response = Message.CreateMessage(MessageVersion.Default, Constants.GameServiceReceive, asyncResult.Tags.ToBytes());
				response = asyncResult.Poll.PrepareRespose(response, asyncResult.Session.SessionId);
			}

			return response;
		}

		private void StartMessageRead(object ar)
		{
			var asyncResult = (DequeueAsyncResult)ar;
			try
			{
				if (String.IsNullOrEmpty(asyncResult.Session.SessionId))
					return;

				Logger.LogDebug("SERVER: Start polling client {0} on Thread {1}", asyncResult.Session.Address, Thread.CurrentThread.ManagedThreadId);

				var elapsedTime = 0D;
				while (elapsedTime < asyncResult.Timeout.TotalSeconds)
				{
					var client = Game.Server.Clients.Where(c => c.SessionId.ToString().Equals(asyncResult.Session.SessionId)).FirstOrDefault();

					if (client != null)
					{
						if (client.Context.HasMessages)
						{
							RdlTag tag;
							while (client.Context.Read(out tag))
							{
								asyncResult.Tags.Add(tag);
							}
							break;
						}
					}
					Logger.LogDebug("SERVER: Polling client, ElapsedTime = {0}", elapsedTime);
					Thread.Sleep(1000);
					elapsedTime += 1D;
				}
			}
			catch (ThreadAbortException) { }
			catch (Exception ex) { Logger.LogError(ex.ToString()); }
			finally
			{
				asyncResult.Callback(asyncResult);
			}
		}

		#endregion
	}

	/// <summary>
	/// Helper class for catching asynchronous messages from scalable storage
	/// </summary>
	public class DequeueAsyncResult : IAsyncResult
	{
		ManualResetEvent waitHandle = new ManualResetEvent(false);

		public object AsyncState { get; set; }
		public WaitHandle AsyncWaitHandle { get { return this.waitHandle; } }
		public bool CompletedSynchronously { get { return true; } }
		public bool IsCompleted { get { return true; } }

		public AsyncCallback Callback { get; set; }
		
		public PollingDuplexSession Session { get; set; }	
		public RdlTagCollection Tags { get; private set; }	
		public MakeConnection Poll { get; set; }
		public TimeSpan Timeout { get; set; }

		public DequeueAsyncResult(MakeConnection poll, PollingDuplexSession session, TimeSpan timeout, AsyncCallback callback, object state)
		{
			this.AsyncState = state;
			this.Callback = callback;
			this.Session = session;
			this.Poll = poll;
			this.Timeout = timeout;
			this.Tags = new RdlTagCollection();
		}
	}
}
