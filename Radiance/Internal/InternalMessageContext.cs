using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance.Internal
{
	public class InternalMessageContext : IMessageContext
	{
		#region IMessageContext Members

		public Radiance.Markup.RdlTagCollection Message
		{
			get { return new Radiance.Markup.RdlTagCollection(); }
		}

		public void Add(Radiance.Markup.RdlTag tag)
		{
		}

		public void AddRange(Radiance.Markup.RdlTag[] tags)
		{
		}

		#endregion

		#region IMessageContext Members

		public bool HasMessages { get; private set; }

		public int Count { get; private set; }	

		public bool Read(out Radiance.Markup.RdlTag tag)
		{
			tag = null;
			return false;
		}

		public void Flush()
		{
		}

		#endregion
	}
}
