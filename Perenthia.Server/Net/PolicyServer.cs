using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace Perenthia.Net
{
	/// <summary>
	/// Provides a TCP listener for serving cross domain policy files.
	/// </summary>
	/// <remarks>
	/// http://blogs.silverlight.net/blogs/msnow/archive/2008/06/26/full-implementation-of-a-silverlight-policy-server.aspx
	/// </remarks>
	public class PolicyServer
	{
		private byte[] _policyData;
		private Socket _listener;
		private bool _running;
		private IPEndPoint _endPoint;

		public event NetworkExceptionEventHandler Error = delegate { };
		public event PolicyRequestReceivedEventHandler RequestReceived = delegate { };

		public PolicyServer(byte[] policyFileData, IPEndPoint endPoint)
		{
			_policyData = policyFileData;
			_endPoint = endPoint;
		}

		public void Start()
		{
			_running = true;
			if (_listener == null)
			{
				_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

				_listener.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 0);
			}

			_listener.Bind(_endPoint);
			_listener.Listen(10);

			_listener.BeginAccept(OnBeginAcceptClientComplete, null);
		}

		public void Stop()
		{
			_running = false;
			try
			{
				if (_listener != null)
					_listener.Shutdown(SocketShutdown.Both);
			}
			catch (SocketException) { }
			catch (ObjectDisposedException) { }
		}

		private void OnBeginAcceptClientComplete(IAsyncResult ar)
		{
			if (!_running)
				return;

			Socket socket = null;

			try
			{
				socket = _listener.EndAccept(ar);
			}
			catch (Exception e)
			{
				Error(new NetworkExceptionEventArgs(e));
				return;
			}

			var client = new PolicyClient(socket, _policyData);
			client.Error += Error;
			client.RequestReceived += RequestReceived;

			_listener.BeginAccept(OnBeginAcceptClientComplete, null);
		}
	}

	public class PolicyClient
	{
		private const string PolicyRequestString = "<policy-file-request/>";

		private Socket _socket;
		private byte[] _buffer;
		private byte[] _policyData;

		public event NetworkExceptionEventHandler Error = delegate { };
		public event PolicyRequestReceivedEventHandler RequestReceived = delegate { };

		public PolicyClient(Socket socket, byte[] policyData)
		{
			_socket = socket;
			_policyData = policyData;
			_buffer = new byte[PolicyRequestString.Length];

			try
			{
				_socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceived, null);
			}
			catch (SocketException e)
			{
				Error(new NetworkExceptionEventArgs(e));
				_socket.Close();
			}
			catch (ObjectDisposedException) { }
		}

		private void OnReceived(IAsyncResult ar)
		{
			try
			{
				var received = _socket.EndReceive(ar);

				if (received < PolicyRequestString.Length)
				{
					_socket.BeginReceive(_buffer, received, PolicyRequestString.Length - received, SocketFlags.None, OnReceived, null);
					return;
				}

				var request = Encoding.UTF8.GetString(_buffer, 0, received);
				RequestReceived(new PolicyRequestReceivedEventArgs { Request = request });
				if (StringComparer.InvariantCultureIgnoreCase.Compare(request, PolicyRequestString) != 0)
				{
					_socket.Close();
					return;
				}

				_socket.BeginSend(_policyData, 0, _policyData.Length, SocketFlags.None, OnSend, null);
			}
			catch (SocketException e)
			{
				Error(new NetworkExceptionEventArgs(e));
				_socket.Close();
			}
			catch (ObjectDisposedException) { }
		}

		private void OnSend(IAsyncResult ar)
		{
			try
			{
				_socket.EndSend(ar);
				_socket.Close();
			}
			catch (SocketException e)
			{
				Error(new NetworkExceptionEventArgs(e));
				_socket.Close();
			}
			catch (ObjectDisposedException) { }
		}
	}
}
