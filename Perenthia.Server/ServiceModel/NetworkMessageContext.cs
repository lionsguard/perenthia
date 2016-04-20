using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radiance.Markup;
using Radiance;

namespace Perenthia.ServiceModel
{
	public class NetworkMessageContext : IMessageContext
	{
		private object _lockReceive = new object();
		private object _lockRead = new object();
		private Queue<RdlTag> _receiveQueue = new Queue<RdlTag>();
		private Queue<RdlTag> _readQueue = new Queue<RdlTag>();

		/// <summary>
		/// Gets a value indicating whether or not the current message context has queued messages.
		/// </summary>
		public bool HasMessages
		{
			get
			{
				lock (_lockReceive)
				{
					return _receiveQueue.Count > 0;
				}
			}
		}

		/// <summary>
		/// Gets the total number of messages currently available to be read.
		/// </summary>
		public int Count
		{
			get
			{
				lock (_lockRead)
				{
					return _readQueue.Count;
				}
			}
		}

		/// <summary>
		/// Adds a Tag instance to the context.
		/// </summary>
		/// <param name="tag">The Tag instance to add to the context.</param>
		public void Add(RdlTag tag)
		{
			lock (_receiveQueue)
			{
				_receiveQueue.Enqueue(tag);
			}
		}

		/// <summary>
		/// Adds a range of RdlTag instances to the context.
		/// </summary>
		/// <param name="tags">The tags to add to the context.</param>
		public void AddRange(RdlTag[] tags)
		{
			lock (_receiveQueue)
			{
				foreach (var tag in tags)
				{
					_receiveQueue.Enqueue(tag);
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
		/// Flushes all queued tags and make the tags available to the Read method.
		/// </summary>
		public void Flush()
		{
			lock (_receiveQueue)
			{
				while (_receiveQueue.Count > 0)
				{
					lock (_readQueue)
					{
						_readQueue.Enqueue(_receiveQueue.Dequeue());
					}
				}
			}
		}
	}
}
