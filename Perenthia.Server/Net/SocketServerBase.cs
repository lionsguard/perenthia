using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Radiance;

namespace Perenthia.Net
{
	public abstract class SocketServerBase
	{
		protected bool IsRunning { get; private set; }
		protected Socket Listener { get; set; }

		protected SocketServerBase(IPEndPoint endPoint)
		{
			Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			Listener.Bind(endPoint);
		}

		/// <summary>
		/// Starts the TCP listener and begins waiting for connections.
		/// </summary>
		public void Start()
		{
			IsRunning = true;
			Listener.Listen(100);
			Listener.BeginAccept(OnAcceptCompleted, Listener);
		}

		/// <summary>
		/// Stops the TCP listener and disconnects all connected clients.
		/// </summary>
		public void Stop()
		{
			try
			{
				IsRunning = false;
				if (Listener != null)
					Listener.Shutdown(SocketShutdown.Both);
			}
			catch (SocketException) { }
			catch (ObjectDisposedException) { }
		}

		private void OnAcceptCompleted(IAsyncResult ar)
		{
			if (!IsRunning)
				return;

			try
			{
				// Get the listener socket.
				Socket listener = (Socket)ar.AsyncState;

				HandleAcceptedClient(listener, listener.EndAccept(ar));

				// Continue to listen for incoming connections.
				listener.BeginAccept(OnAcceptCompleted, listener);
			}
			catch (InvalidOperationException) { }
			catch (SocketException) { }
		}

		protected abstract void HandleAcceptedClient(Socket listener, Socket socket);
	}
}
