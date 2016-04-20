using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radiance.Markup;
using System.Net.Sockets;
using System.IO;
using Radiance;
using Lionsguard;

namespace Perenthia.Net
{
	public class SocketMessageContext : IMessageContext
	{
		private const int ReceiveBufferSize = 131071;

		private IClient Client;
		private Socket Socket;
		private byte[] Buffer = new byte[ReceiveBufferSize];
		private List<byte> PacketData = new List<byte>();

		public SocketMessageContext(Socket socket, IClient client)
		{
			Client = client;
			Socket = socket;
			Socket.Blocking = false;

			BeginReceive();
		}

		private void BeginReceive()
		{
			try
			{
				Socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, OnBeginReceiveComplete, this);
			}
			catch (ObjectDisposedException) { if (this.Client != null) this.Client.Expire(); }
		}
		private static void OnBeginReceiveComplete(IAsyncResult ar)
		{
			var context = (SocketMessageContext)ar.AsyncState;

			int bytesReceived = 0;
			try
			{
				// Complete the call.
				bytesReceived = context.Socket.EndReceive(ar);
			}
			catch (SocketException)
			{
				// Disconnect the current client.
				context.Socket.Close();
				if (context.Client != null) context.Client.Expire();
				return;
			}
			catch (ObjectDisposedException)
			{
				if (context.Client != null) context.Client.Expire();
				return;
			}

			try
			{
				if (bytesReceived > 0)
				{
					context.PacketData.AddRange(context.Buffer.Take(bytesReceived));

					if (context.PacketData.Count >= 4)
					{
						int index = 0;

						// Convert the current packet data into a byte array.
						byte[] data = context.PacketData.ToArray();

						bool readComplete = false;
						while (!readComplete)
						{
							// Get the length of the bytes to process. Length does do not include
							// the 4 bytes required for the actual length int value itself.
							int length = BitConverter.ToInt32(data, index);

							// Advance the index 4 bytes to account for the length value.
							index += 4;

							if (data.Length >= length + 4) // Account for the length value
							{
								byte[] buffer = new byte[length];
								Array.Copy(data, index, buffer, 0, length);

								// Completed adding packet data, create the reader and allow the network to process the packet.
								Game.Server.ProcessCommands(context.Client, RdlCommandGroup.FromBytes(buffer));

								index += length;

								if (index >= data.Length)
								{
									context.PacketData.Clear();
									readComplete = true;
								}
							}
							else
								readComplete = true;
						}
					}
				}

				context.BeginReceive();
			}
			catch (SocketException)
			{
				context.Socket.Close(); 
				if (context.Client != null) context.Client.Expire();
			}
		}

		#region IMessageContext Members
		public bool HasMessages
		{
			get
			{
				lock (_sendQueue)
				{
					return _sendQueue.Count > 0;
				}
			}
		}

		public int Count
		{
			get
			{
				lock (_sendQueue)
				{
					return _sendQueue.Count;
				}
			}
		}

		private Queue<RdlTag> _sendQueue = new Queue<RdlTag>();

		public RdlTagCollection Message
		{
			get { return new RdlTagCollection(); }
		}

		public bool Read(out RdlTag tag)
		{
			tag = null;
			return false;
		}

		public void Add(RdlTag tag)
		{	
			lock (_sendQueue)
			{
				_sendQueue.Enqueue(tag);
			}
		}

		public void AddRange(RdlTag[] tags)
		{
			var data = new List<byte>();
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
			var tags = new RdlTagCollection();
			lock (_sendQueue)
			{
				while (_sendQueue.Count > 0)
				{
					tags.Add(_sendQueue.Dequeue());
				}
			}
			if (tags.Count > 0)
			{
				var data = tags.ToBytes();
				//Logger.LogDebug("SERVER: Sending tags to client {0}: {1}", this.Client.UserName, tags);
				Logger.LogDebug("SERVER: Sending {0} tags for a total of {1} bytes to the client.", tags.Count, data.Length);
				Send(data);
			}
		}

		private void Send(byte[] data)
		{
			if (data == null || data.Length == 0)
				return;

			// Add a length value to the start of the byte array.
			var length = BitConverter.GetBytes(data.Length);
			using (var ms = new MemoryStream())
			{
				ms.Write(length, 0, length.Length);
				ms.Write(data, 0, data.Length);

				ms.Seek(0, SeekOrigin.Begin);
				var buffer = new byte[ms.Length];
				ms.Read(buffer, 0, buffer.Length);

				try
				{
					Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, OnBeginSendComplete, Socket);
				}
				catch (SocketException)
				{
					// Disconnect the current Socket.
					if (Socket != null)
						Socket.Close();
					if (this.Client != null) this.Client.Expire();
				}
				catch (ObjectDisposedException) { if (this.Client != null) this.Client.Expire(); }
			}
		}
		private void OnBeginSendComplete(IAsyncResult ar)
		{
			try
			{
				Socket handler = (Socket)ar.AsyncState;
				// Complete the sending of data.
				int bytesSent = handler.EndSend(ar);
			}
			catch (SocketException)
			{
				// Raise the disconnect event and close the current Socket.
				if (Socket != null)
					Socket.Close();
				if (this.Client != null) this.Client.Expire();
			}
			catch (ObjectDisposedException) { if (this.Client != null) this.Client.Expire(); }
		}

		#endregion
	}
}
