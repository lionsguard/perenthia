using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Windows.Threading;

using Radiance.Markup;

namespace Radiance
{
    /// <summary>
    /// Provides a helper for WCF Polling Duplex services.
    /// </summary>
    /// <remarks>
    /// Utilized the sample code found at http://petermcg.wordpress.com/2008/09/03/silverlight-polling-duplex-part-1-architecture/ 
    /// for a good portion of the code included with this helper class.
    /// </remarks>
    public class WcfCommunicator : ICommunicator
    {
        // Asynchronously begins an open operation on an ICommunicationObject with code to call EndOpen when it completes
        private static readonly Action<ICommunicationObject> Open = ico => ico.BeginOpen(iar => ico.EndOpen(iar), ico);
        // Asynchronously begins a send operation on an IDuplexSessionChannel with code to call EndSend when it completes
        private static readonly Action<IDuplexSessionChannel, Message> Send = (idc, msg) => idc.BeginSend(msg, iar => idc.EndSend(iar), idc);
		// Asynchronously begins a receive operation on an IDuplexSessionChannel with code to call an Action<Message> when it completes
        private static readonly Action<IDuplexSessionChannel, Action<Message>> Receive = (idc, act) => idc.BeginReceive(iar => act(idc.EndReceive(iar)), idc);

        private AutoResetEvent _waitObject = new AutoResetEvent(false);
        private IDuplexSessionChannel _channel = null;
        private CommunicatorResponseEventHandler _altResponse = null;

        /// <summary>
        /// Initializes a new instance of the WcfHelper class.
        /// </summary>
        /// <param name="serverUri">The URI to the server service.</param>
        public WcfCommunicator(Uri serverUri)
        {
            this.Init(serverUri);
        }

        private void Init(Uri serverUri)
        {
            // Create a channel factory capable of producing a channel of type IDuplexSessionChannel
            IChannelFactory<IDuplexSessionChannel> factory = new PollingDuplexHttpBinding().BuildChannelFactory<IDuplexSessionChannel>();
            Open(factory);

            // Address of the polling duplex server and creation of the channel to that endpoint
            EndpointAddress endPoint = new EndpointAddress(serverUri.ToString());
            _channel = factory.CreateChannel(endPoint);
            Open(_channel);

            // Use the thread pool to start only one asynchronous request to Receive messages from the server
            // Only start another asynchronous request when a signal is received that the first thread pool thread has received something
            ThreadPool.RegisterWaitForSingleObject(_waitObject, delegate { Receive(_channel, CompleteReceive); }, null, Timeout.Infinite, false);
            _waitObject.Set();
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
            this.Execute(commands, null);
        }

        /// <summary>
        /// Sends the specified command to the server to be executed.
        /// </summary>
        /// <param name="commands">The command to execute.</param>
        /// <param name="responseCallback">The CommunicatorResponseEventHandler to use for handling the 
        /// response from this command execution. If this value is provided the Response event will not be raised.</param>
        public void Execute(RdlCommandGroup commands, CommunicatorResponseEventHandler responseCallback)
        {
            _altResponse = responseCallback;
            // Create a message with the appropriate SOAPAction and asynchronously send it on the channel with the serialized Stock as the body of the envelope
            Message message = Message.CreateMessage(MessageVersion.Soap11, "Radiance.Contract.IGameServer/Process", commands.ToBytes());
            Send(_channel, message);
		}

		public void Close()
		{
		}

		public void Connect()
		{
			this.Connected(new CommunicatorEventArgs(this));
		}

        private void CompleteReceive(Message message)
        {
            CommunicatorResponseEventArgs args = new CommunicatorResponseEventArgs(this,
                RdlTagCollection.FromString(message.GetBody<string>()));
            if (_altResponse != null)
            {
                _altResponse(args);
            }
            else
            {
                this.Response(args);
            }

            // Signal the thread pool to start another single asynchronous request to Receive messages from the server
            _waitObject.Set();
        }

        #endregion
    }
}
