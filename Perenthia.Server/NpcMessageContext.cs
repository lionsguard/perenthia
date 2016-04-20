using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia
{
	public class NpcMessageContext : IMessageContext
	{
		public event NpcMessageReceivedEventHandler MessageReceived = delegate { };

		#region IMessageContext Members
		public bool HasMessages { get; private set; }

		public int Count { get; private set; }

		public RdlTagCollection Message	
		{
			get { return new RdlTagCollection(); }
		}

		public void Add(RdlTag tag)
		{
			if (tag is RdlMessage)
			{
				this.MessageReceived(new NpcMessageReceivedEventArgs(tag as RdlMessage));
			}
		}

		public void AddRange(RdlTag[] tags)
		{
			foreach (var tag in tags)
			{
				this.Add(tag);
			}
		}

		public void Flush()
		{
		}

		#endregion

		#region IMessageContext Members


		public bool Read(out RdlTag tag)
		{
			tag = null;
			return false;
		}

		#endregion
	}

	public delegate void NpcMessageReceivedEventHandler(NpcMessageReceivedEventArgs e);
	public class NpcMessageReceivedEventArgs : EventArgs
	{
		public RdlMessage Message { get; set; }

		public NpcMessageReceivedEventArgs(RdlMessage message)
		{
			this.Message = message;
		}
	}
}
