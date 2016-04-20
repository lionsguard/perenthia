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
    /// Defines an enumeration of protocol types used to connect to the server.
    /// </summary>
    public enum CommunicationProtocol
    {
        /// <summary>
        /// Specifies a standard HTTP connection.
        /// </summary>
        Http,
        /// <summary>
        /// Specifies a WCF Polling Duplex connection.
        /// </summary>
        PollingDuplex,
        /// <summary>
        /// Specifies a socket connection.
        /// </summary>
        Sockets,
    }

    /// <summary>
    /// Defines an object used to communicate with a remote server.
    /// </summary>
    public interface ICommunicator
    {
        /// <summary>
        /// An event that is raised when the response is received from the server.
        /// </summary>
        event CommunicatorResponseEventHandler Response;

        /// <summary>
        /// An event that is raised when communication to the server fails.
        /// </summary>
        event CommunicatorEventHandler Failed;

		event CommunicatorErrorEventHandler Error;

		event CommunicatorErrorEventHandler ConnectFailed;

		event CommunicatorEventHandler Connected;

        /// <summary>
        /// Sends the specified command to the server to be executed.
        /// </summary>
        /// <param name="commands">The command to execute.</param>
        void Execute(RdlCommandGroup commands);

        /// <summary>
        /// Sends the specified command to the server to be executed.
        /// </summary>
        /// <param name="commands">The command to execute.</param>
        /// <param name="responseCallback">The CommunicatorResponseEventHandler to use for handling the 
        /// response from this command execution. If this value is provided the Response event will not be raised.</param>
        void Execute(RdlCommandGroup commands, CommunicatorResponseEventHandler responseCallback);

		void Close();

		void Connect();
    }
}
