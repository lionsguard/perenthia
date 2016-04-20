using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Radiance;
using Radiance.Markup;
using System.IO;

namespace Perenthia.Utility.Net
{
	public class DepotClient
	{
		private const int ReceiveBufferSize = 131071;

		private Socket Socket;
		private byte[] Buffer = new byte[ReceiveBufferSize];
		private List<byte> PacketData = new List<byte>();

		public DepotClient(Socket socket)
		{
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
			catch (ObjectDisposedException) { }
		}
		private static void OnBeginReceiveComplete(IAsyncResult ar)
		{
			var client = (DepotClient)ar.AsyncState;

			int bytesReceived = 0;
			try
			{
				// Complete the call.
				bytesReceived = client.Socket.EndReceive(ar);
			}
			catch (SocketException)
			{
				// Disconnect the current client.
				client.Socket.Close();
				return;
			}
			catch (ObjectDisposedException)
			{
				return;
			}

			try
			{
				if (bytesReceived > 0)
				{
					client.PacketData.AddRange(client.Buffer.Take(bytesReceived));

					if (client.PacketData.Count >= 4)
					{
						int index = 0;

						// Convert the current packet data into a byte array.
						byte[] data = client.PacketData.ToArray();

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

								ProcessDepotCommand(buffer, client);

								index += length;

								if (index >= data.Length)
								{
									client.PacketData.Clear();
									readComplete = true;
								}
							}
							else
								readComplete = true;
						}
					}
				}

				client.BeginReceive();
			}
			catch (SocketException)
			{
				client.Socket.Close();
			}
		}

		public void Send(byte[] data)
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
				}
				catch (ObjectDisposedException) { return; }
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
			}
			catch (ObjectDisposedException) { }
		}

		private static void ProcessDepotCommand(byte[] data, DepotClient client)
		{
			var group = RdlCommandGroup.FromBytes(data);
			foreach (var cmd in group)
			{
				if (cmd.TypeName.ToUpper().Equals("MAPNAMES"))
				{
					client.Send(Depot.GetMapNames().ToBytes());
				}
				else if (cmd.TypeName.ToUpper().Equals("MAPCHUNK"))
				{
					var mapName = cmd.GetArg<string>(0);
					var startX = cmd.GetArg<int>(1);
					var startY = cmd.GetArg<int>(2);
					var includeActors = cmd.GetArg<bool>(3);

					var result = Depot.GetMapChunk(mapName, startX, startY, includeActors).Tags;
					client.Send(Encoding.UTF8.GetBytes(result));
				}
				else
				{
					client.Send(RdlTag.Empty.ToBytes());
				}
			}
		}
	}
}
