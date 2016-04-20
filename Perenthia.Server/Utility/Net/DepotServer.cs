using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Radiance;
using Perenthia.Net;

namespace Perenthia.Utility.Net
{
	public class DepotServer : SocketServerBase
	{
		private List<DepotClient> _clients = new List<DepotClient>();

		public DepotServer(IPEndPoint endPoint)
			: base(endPoint)
		{
		}

		protected override void HandleAcceptedClient(Socket listener, Socket socket)
		{
			_clients.Add(new DepotClient(socket));
		}
	}
}
