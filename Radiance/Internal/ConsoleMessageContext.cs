using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance.Internal
{
	public class ConsoleMessageContext : IMessageContext
	{
		#region IMessageContext Members
		public bool HasMessages { get; private set; }

		public int Count { get; private set; }	

		public RdlTagCollection Message
		{
			get { return null; }
		}

		public void Add(RdlTag tag)
		{
			Console.WriteLine(tag.ToString());
		}

		public void AddRange(RdlTag[] tags)
		{
			foreach (var tag in tags)
			{
				this.Add(tag);
			}

		}

		public void Flush() { }

		#endregion

		#region IMessageContext Members


		public bool Read(out RdlTag tag)
		{
			tag = null;
			return false;
		}

		#endregion
	}
}
