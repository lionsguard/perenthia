using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;
using Radiance.Handlers;

namespace Radiance.Internal
{
	public class Client : IClient
	{
		#region IClient Members

		public Guid SessionId { get; private set; }

		public string Address { get; set; }	

		public bool Connected { get { return true; } }

		public AuthKey AuthKey { get; set; }

		public string UserName { get; set; }

		public DateTime LastHeartbeatDate { get; set; }

		public IMessageContext Context { get; private set; }

		public IPlayer Player { get; set; }

		public CommandHandler Handler { get; set; }

		public RdlCommandGroup LastCommandGroup { get; set; }	

		public double GetLastHeartbeatInterval()
		{
			return 0;
		}

		public void Expire()
		{
		}

		public void Flush()
		{
		}

		#endregion

		public Client()
		{
			this.Context = new ConsoleMessageContext();
			this.Handler = new LoginCommandHandler(this);
			this.SessionId = Guid.NewGuid();
		}
	}
}
