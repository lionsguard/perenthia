using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

using Radiance.Markup;
using System.Net.Sockets;
using System.IO;

namespace Radiance
{
    /// <summary>
    /// Provides a class for socket basec server communication.
    /// </summary>
    public class SocketCommunicator : ICommunicator
	{
		private const int ReceiveBufferSize = 131071;

		private int _port;
		private Socket _socket;

		public CommunicatorResponseEventHandler AltCallback { get; set; }

		private List<byte> _sendBuffer = new List<byte>();
		private List<byte> _receiveBuffer = new List<byte>();

		public string ServiceUri { get; set; }

		public bool IsConnected
		{
			get { return _socket.Connected; }
		}

		/// <summary>
		/// Initializes a new instance of the SocketHelper class.
		/// </summary>
		/// <param name="serverUri">The Uri to the server.</param>
		/// <param name="port">The port in which to connect to the server, limited to the port range of 4502-4532.</param>
		public SocketCommunicator(string serviceUri, int port)
		{
			if (port < 4502 || port > 4532)
			{
				throw new ArgumentException(String.Format("Invalid port specified {0}. The valid port range is 4502 - 4532.", port));
			}
			ServiceUri = serviceUri;
			_port = port;

			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			//Connect();
		}

		public void Connect()
		{
			if (this.IsConnected)
				return;

			var endPoint = new DnsEndPoint(ServiceUri, _port);
			var args = new SocketAsyncEventArgs { RemoteEndPoint = endPoint };
			args.Completed += OnConnectCompleted;

			_socket.ConnectAsync(args);
		}

		private void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
		{
//#if DEBUG
//            this.ConnectFailed(new CommunicatorErrorEventArgs(this, new Exception("Testing failed socket connection.")));
//            return;
//#endif

			if (e.SocketError != SocketError.Success)
			{
				this.ConnectFailed(new CommunicatorErrorEventArgs(this, new SocketException((int)e.SocketError)));
				return;
			}

			if (!this.IsConnected)
				return;

			this.Connected(new CommunicatorEventArgs(this));

			if (_sendBuffer.Count > 0)
			{
				lock (_sendBuffer)
				{
					this.Send(_sendBuffer.ToArray());
					_sendBuffer.Clear();
				}
			}

			var response = new byte[ReceiveBufferSize];
			e.SetBuffer(response, 0, response.Length);
			e.Completed -= OnConnectCompleted;
			e.Completed += OnSocketReceive;

			_socket.ReceiveAsync(e);
		}

		private void OnSocketReceive(object sender, SocketAsyncEventArgs e)
		{
			if (e.SocketError != SocketError.Success)
			{
				this.Error(new CommunicatorErrorEventArgs(this, new SocketException((int)e.SocketError)));
				return;
			}

			try
			{
				if (e.BytesTransferred == 0)
				{
					// TODO: Raise an event when the socket is disconnected.
					_socket.Close();
					return;
				}

				_receiveBuffer.AddRange(e.Buffer.Take(e.BytesTransferred));
				if (_receiveBuffer.Count >= 4)
				{
					int index = 0;

					byte[] data = _receiveBuffer.ToArray();

					bool readComplete = false;
					while (!readComplete)
					{
						int length = BitConverter.ToInt32(data, index);

						// Increment index to account for the length value bytes.
						index += 4;

						if (_receiveBuffer.Count >= length + 4)
						{
							byte[] buffer = new byte[length];
							Array.Copy(data, index, buffer, 0, length);

							var args = new CommunicatorResponseEventArgs(this, RdlTagCollection.FromBytes(buffer));
							if (AltCallback != null)
								AltCallback(args);
							else
								Response(args);

							index += length;

							if (index >= data.Length)
							{
								_receiveBuffer.Clear();
								readComplete = true;
							}
						}
						else
							readComplete = true;
					}
				}

				_socket.ReceiveAsync(e);
			}
			catch (Exception ex)
			{
				Error(new CommunicatorErrorEventArgs(this, ex));
			}
		}
		private void Send(byte[] data)
		{
			try
			{
				if (!this.IsConnected)
				{
					_sendBuffer.AddRange(data);
					return;
				}

				var length = BitConverter.GetBytes(data.Length);
				using (var ms = new MemoryStream())
				{
					ms.Write(length, 0, length.Length);
					ms.Write(data, 0, data.Length);
					ms.Seek(0, SeekOrigin.Begin);

					byte[] buffer = new byte[ms.Length];
					ms.Read(buffer, 0, buffer.Length);

					var args = new SocketAsyncEventArgs();
					args.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
					args.SetBuffer(buffer, 0, buffer.Length);

					_socket.SendAsync(args);
				}
			}
			catch (Exception ex)
			{
				Error(new CommunicatorErrorEventArgs(this, ex));
			}
		}

		private void OnSendCompleted(object sender, SocketAsyncEventArgs e)
		{
			if (e.SocketError != SocketError.Success)
			{
				this.Error(new CommunicatorErrorEventArgs(this, new SocketException((int)e.SocketError)));
			}
		}

		#region ICommunicator Members

		/// <summary>
		/// An event that is raised when the response is received from the server.
		/// </summary>
		public event CommunicatorResponseEventHandler Response = delegate { };

		/// <summary>
		/// An event that is raised when communication to the server fails.
		/// </summary>
		public event CommunicatorEventHandler Failed = delegate { };

		/// <summary>
		/// An event that is raised when the communication to the server errors.
		/// </summary>
		public event CommunicatorErrorEventHandler Error = delegate { };

		public event CommunicatorErrorEventHandler ConnectFailed = delegate { };

		public event CommunicatorEventHandler Connected = delegate { };

		/// <summary>
		/// Sends the specified command to the server to be executed.
		/// </summary>
		/// <param name="commands">The command to execute.</param>
		public void Execute(RdlCommandGroup commands)
		{
			Execute(commands, null);
		}

		/// <summary>
		/// Sends the specified command to the server to be executed.
		/// </summary>
		/// <param name="commands">The command to execute.</param>
		/// <param name="responseCallback">The CommunicatorResponseEventHandler to use for handling the 
		/// response from this command execution. If this value is provided the Response event will not be raised.</param>
		public void Execute(RdlCommandGroup commands, CommunicatorResponseEventHandler responseCallback)
		{
			this.AltCallback = responseCallback;
			this.Send(commands.ToBytes());
		}

		/// <summary>
		/// Closes the current connection.
		/// </summary>
		public void Close()
		{
			try
			{
				if (_socket != null)
					_socket.Close();
			}
			catch (ObjectDisposedException) { }
		}

		#endregion
    }
}
