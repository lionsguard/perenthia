using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Radiance;
using Lionsguard;

namespace Perenthia.Net
{
	public class SocketServer : SocketServerBase, ICommunicationHandler
	{
		public SocketServer(IPEndPoint endPoint)
			: base(endPoint)
		{
		}

		protected override void HandleAcceptedClient(Socket listener, Socket socket)
		{
			// Initialize the Connection class for handling client connections.
			SocketClient client = new SocketClient(socket);
			client.CommunicationHandler = this;
			client.UserName = String.Empty;
			Game.Server.World.Provider.CreateSession(client, socket.RemoteEndPoint.ToString());
			Logger.LogDebug("SERVER: New connection from: {0}", socket.RemoteEndPoint);
			Game.Server.Clients.Add(client);
		}

		#region ICommunicationHandler Members

		public IClient CreateClient(Guid sessionId)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
