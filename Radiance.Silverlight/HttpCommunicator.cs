using System;
using System.IO;
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
    /// Provides a class for sending information via HTTP.
    /// </summary>
    public class HttpCommunicator : ICommunicator
    {
		private Guid SessionId { get; set; }
		private HttpWebRequest Request { get; set; }
        private Uri RequestUri { get; set; }
        private string RequestMethod { get; set; }
        private byte[] Buffer { get; set; }
        private CommunicatorResponseEventHandler AltCallback { get; set; }

        /// <summary>
        /// Initializes a new instance of the HttpHelper class.
        /// </summary>
        /// <param name="requestUri">The URI of the server where the request will be sent.</param>
        /// <param name="method">The request method, either POST or GET.</param>
        public HttpCommunicator(Uri requestUri, string method, Guid sessionId)
        {
            this.RequestUri = requestUri;
            this.RequestMethod = method;
			this.SessionId = sessionId;
            this.CreateRequest();
        }

        private void CreateRequest()
        {
            this.Request = (HttpWebRequest)WebRequest.Create(this.RequestUri);
			this.Request.Headers[HttpHeaders.RadianceSessionIdHeaderKey] = SessionId.ToString();
            this.Request.ContentType = "application/x-www-form-urlencoded";
            this.Request.Method = this.RequestMethod;
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
            this.Buffer = commands.ToBytes();
            this.AltCallback = responseCallback;
            this.CreateRequest();
            this.Request.BeginGetRequestStream(new AsyncCallback(HttpCommunicator.BeginRequest), this);
        }

		public void Close()
		{
		}

		public void Connect()
		{
			this.Connected(new CommunicatorEventArgs(this));
		}

        #endregion

        private static void BeginRequest(IAsyncResult ar)
        {
            HttpCommunicator communicator = ar.AsyncState as HttpCommunicator;
            if (communicator != null)
            {
                Stream stream = communicator.Request.EndGetRequestStream(ar);
                if (stream != null)
                {
                    if (communicator.Buffer != null && communicator.Buffer.Length > 0)
                    {
                        stream.Write(communicator.Buffer, 0, communicator.Buffer.Length);
                    }
                    stream.Close();
                }
                communicator.Request.BeginGetResponse(new AsyncCallback(HttpCommunicator.BeginResponse), communicator);
            }
        }

        private static void BeginResponse(IAsyncResult ar)
        {
            HttpCommunicator communicator = ar.AsyncState as HttpCommunicator;
            if (communicator != null)
            {
                HttpWebResponse response = (HttpWebResponse)communicator.Request.EndGetResponse(ar);
                if (response != null)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream stream = response.GetResponseStream();
                        if (stream != null)
                        {
                            byte[] buffer = new byte[(int)stream.Length];
                            stream.Read(buffer, 0, (int)stream.Length);
                            RdlTagCollection tags = RdlTagCollection.FromBytes(buffer);
                            CommunicatorResponseEventArgs args = new CommunicatorResponseEventArgs(communicator, tags);

                            if (communicator.AltCallback != null)
                            {
                                communicator.AltCallback(args);
                            }
                            else
                            {
                                communicator.Response(args);
                            }
                            stream.Close();
                        }
                    }
                    else
                    {
                        communicator.Failed(new CommunicatorEventArgs(communicator));
                    }
                }
            }
        }
    }
}
