using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radiance;
using Radiance.Markup;
using System.ServiceModel;
using Radiance.Contract;
using Lionsguard;

namespace Perenthia.ServiceModel
{
	public class DuplexMessageContext : IMessageContext
	{
		private IClient _client;
		private Queue<RdlTag> _sendQueue = new Queue<RdlTag>();
		private Queue<RdlTag> _readQueue = new Queue<RdlTag>();

		public DuplexMessageContext(IClient client)
		{
			_client = client;
		}

		#region IMessageContext Members
		/// <summary>
		/// Gets a value indicating whether or not the current message context has queued messages.
		/// </summary>
		public bool HasMessages
		{
			get
			{
				lock (_readQueue)
				{
					return _readQueue.Count > 0;
				}
			}
		}

		/// <summary>
		/// Reads and removes the next tag in the queue and returns true if a tag was found.
		/// </summary>
		/// <param name="tag">The next available tag in the queue.</param>
		/// <returns>True if a tag exists; otherwise false.</returns>
		public bool Read(out RdlTag tag)
		{
			lock (_readQueue)
			{
				if (_readQueue.Count > 0)
				{
					tag = _readQueue.Dequeue();
					return true;
				}
			}
			tag = null;
			return false;
		}

		/// <summary>
		/// Adds a Message instance to the context.
		/// </summary>
		/// <param name="tag">The Message instance to add to the context.</param>
		public void Add(RdlTag tag)
		{
			lock (_sendQueue)
			{
				_sendQueue.Enqueue(tag);
			}
		}

		/// <summary>
		/// Adds a range of RdlTag instances to the context.
		/// </summary>
		/// <param name="tags">The tags to add to the context.</param>
		public void AddRange(RdlTag[] tags)
		{
			foreach (var tag in tags)
			{
				lock (_sendQueue)
				{
					_sendQueue.Enqueue(tag);
				}
			}
		}

		public void Flush()
		{
			lock (_sendQueue)
			{
				while (_sendQueue.Count > 0)
				{
					lock (_readQueue)
					{
						_readQueue.Enqueue(_sendQueue.Dequeue());	
					}
				}
			}
		}

		//public void Flush()
		//{
		//    var tags = new RdlTagCollection();
		//    lock (_sendQueue)
		//    {
		//        while (_sendQueue.Count > 0)
		//        {
		//            tags.Add(_sendQueue.Dequeue());
		//        }
		//    }
		//    if (tags.Count > 0)
		//    {
		//        var data = tags.ToBytes();
		//        Logger.LogDebug("Sending {0} tags to the client, a total of {1} bytes.", tags.Count, data.Length);
		//        try
		//        {
		//            _receiver.BeginReceive(data, new AsyncCallback(ReceiveComplete), _receiver);
		//        }
		//        catch (CommunicationException ce) { Logger.LogDebug(ce.ToString()); _client.Expire(); }
		//        catch (TimeoutException te) { Logger.LogDebug(te.ToString()); _client.Expire(); }
		//    }
		//}

		#endregion

		//private void ReceiveComplete(IAsyncResult ar)
		//{
		//    try
		//    {
		//        ((IGameClient)(ar.AsyncState)).EndReceive(ar);
		//    }
		//    catch (CommunicationException ce) { Logger.LogDebug(ce.ToString()); _client.Expire(); }
		//    catch (TimeoutException te) { Logger.LogDebug(te.ToString()); _client.Expire(); }
		//}
	}
}
