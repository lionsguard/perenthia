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

using Radiance.Markup;

namespace Radiance
{
    /// <summary>
    /// Represents a manager class used to send commands to the server.
    /// </summary>
    public class CommunicationManager
    {
		private Guid _sessionId = Guid.NewGuid();
        private ICommunicator _communicator = null;

        /// <summary>
        /// Gets the CommunicationProtocol for the current server.
        /// </summary>
        public CommunicationProtocol Protocol { get; private set; }

		/// <summary>
		/// Gets or sets the URI to the server.
		/// </summary>
		public Uri ServerUri { get; set; }

		/// <summary>
		/// Gets or sets the port used for socket connections.
		/// </summary>
		public int SocketPort { get; set; }

        /// <summary>
        /// Gets or sets the user token used to validate the current user on the server.
        /// </summary>
        public string AuthKey { get; set; }

        /// <summary>
        /// Gets or sets the type of user token used to validate the current user on the server.
        /// </summary>
        public string AuthKeyType { get; set; }

        /// <summary>
        /// An event that is raised when a server command response is received.
        /// </summary>
        public event CommunicatorResponseEventHandler Response = delegate { };

        /// <summary>
        /// An event that is raised when communication with the server fails.
        /// </summary>
		public event CommunicatorEventHandler Failed = delegate { };

		/// <summary>
		/// An event that is raised when communication with the server error.
		/// </summary>
		public event CommunicatorErrorEventHandler Error = delegate { };

		public event CommunicatorErrorEventHandler ConnectFailed = delegate { };

		public event CommunicatorEventHandler Connected = delegate { };

        /// <summary>
        /// Initializes a new instance of the ServerManager class.
        /// </summary>
        /// <param name="protocol">The CommunicationProtocol for the current server.</param>
        /// <param name="serverUri">The URI of the server.</param>
        /// <param name="authKey">The token used to validate the current user on the server.</param>
        public CommunicationManager(CommunicationProtocol protocol, Uri serverUri, string authKey)
            : this(protocol, serverUri, authKey, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServerManager class.
        /// </summary>
        /// <param name="protocol">The CommunicationProtocol for the current server.</param>
        /// <param name="serverUri">The URI of the server.</param>
        /// <param name="authKey">The token used to validate the current user on the server.</param>
        /// <param name="port">The port to use with the Sockets protocol.</param>
        public CommunicationManager(CommunicationProtocol protocol, Uri serverUri, string authKey, int socketPort)
        {
            this.Protocol = protocol;
            this.AuthKey = authKey;
			this.ServerUri = serverUri;
			this.SocketPort = socketPort;

			Init();
        }

		private void Init()
		{
			if (this.Protocol != CommunicationProtocol.Http && _communicator != null)
				return;

			switch (this.Protocol)
			{
				case CommunicationProtocol.Http:
					_communicator = new HttpCommunicator(this.ServerUri, "POST", _sessionId);
					break;
				case CommunicationProtocol.PollingDuplex:
					_communicator = new WcfCommunicator(this.ServerUri);
					break;
				case CommunicationProtocol.Sockets:
					_communicator = new SocketCommunicator(this.ServerUri.DnsSafeHost, this.SocketPort);
					break;
			}
			if (_communicator == null)
			{
				throw new InvalidOperationException("A CommunicationProtocol must be specified.");
			}

			_communicator.Response += (e) => { this.Response(e); };
			_communicator.Error += (e) => { this.Error(e); };
			_communicator.Failed += (e) => { this.Failed(e); };
			_communicator.Connected += (e) => { this.Connected(e); };
			_communicator.ConnectFailed += (e) => { this.ConnectFailed(e); };
		}

		public void Connect()
		{
			if (_communicator != null)
				_communicator.Connect();
		}
        
        /// <summary>
        /// Sends the specified command to the server using the current CommunicationProtocol.
        /// </summary>
        /// <param name="command">The CommandTag to send to the server.</param>
        public void SendCommand(RdlCommand command)
        {
            this.SendCommand(command, null);
        }

        /// <summary>
        /// Sends the specified command to the server using the current CommunicationProtocol and provides an 
        /// alternate handler for the server response.
        /// </summary>
        /// <param name="command">The CommandTag to send to the server.</param>
        /// <param name="serverResponseCallback">The CommunicatorResponseEventHandler that will handle the 
        /// response from this command.</param>
        public void SendCommand(RdlCommand command, CommunicatorResponseEventHandler serverResponseCallback)
        {
            RdlCommandGroup group = new RdlCommandGroup(new RdlCommand[] { command });
            group.AuthKey = this.AuthKey;
            group.AuthKeyType = this.AuthKeyType;

			this.Init();
            _communicator.Execute(group, serverResponseCallback);
        }

		public void Close()
		{
			if (_communicator != null)
				_communicator.Close();
		}
    }
}
