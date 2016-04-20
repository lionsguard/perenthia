using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Radiance.Markup;

namespace Radiance.Web
{
	public class HttpMessageContext : IMessageContext
	{
		private Queue<RdlTag> _sendQueue = new Queue<RdlTag>();

		#region IMessageContext Members
		public bool HasMessages { get; private set; }	

		/// <summary>
		/// Reads and removes the next tag in the queue and returns true if a tag was found.
		/// </summary>
		/// <param name="tag">The next available tag in the queue.</param>
		/// <returns>True if a tag exists; otherwise false.</returns>
		public bool Read(out RdlTag tag)
		{
			lock (_sendQueue)
			{
				if (_sendQueue.Count > 0)
				{
					tag = _sendQueue.Dequeue();
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

		#endregion

		/// <summary>
		/// Initializes a new instance of the HttpMessageContext and specifies the collection in which to store queued tags.
		/// </summary>
		/// <param name="message">The collection instance used to store tags.</param>
		public HttpMessageContext(RdlTagCollection message)
		{
		}
	}
}
