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
    /// A delegate used to specifiy a handler for ICommunicator events.
    /// </summary>
    /// <param name="e">The event arguments for the current event.</param>
    public delegate void CommunicatorEventHandler(CommunicatorEventArgs e);

    /// <summary>
    /// Provides an event arguments class for ICommunicator events.
    /// </summary>
    public class CommunicatorEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the current ICommunicator instance.
        /// </summary>
        public ICommunicator Communicator { get; set; }

        /// <summary>
        /// Initializes a new instance of the CommunicatorEventArgs class.
        /// </summary>
        /// <param name="communicator">The ICommunicator instance for the current event.</param>
        public CommunicatorEventArgs(ICommunicator communicator)
        {
            this.Communicator = communicator;
        }
    }

    /// <summary>
    /// A delegate used to specifiy a handler for ICommunicator.Response events.
    /// </summary>
    /// <param name="e">The event arguments for the current event.</param>
    public delegate void CommunicatorResponseEventHandler(CommunicatorResponseEventArgs e);

    /// <summary>
    /// Provides an event arguments class for ICommunicator.Response events.
    /// </summary>
    public class CommunicatorResponseEventArgs : CommunicatorEventArgs
    {
        /// <summary>
        /// Gets the collection of tags sent back from the server.
        /// </summary>
        public RdlTagCollection Tags { get; private set; }

        /// <summary>
        /// Initializes a new instance of the CommunicatorEventArgs class.
        /// </summary>
        /// <param name="communicator">The ICommunicator instance for the current event.</param>
        public CommunicatorResponseEventArgs(ICommunicator communicator, RdlTagCollection tags)
            : base(communicator)
        {
            this.Tags = tags;
        }
    }

	public delegate void CommunicatorErrorEventHandler(CommunicatorErrorEventArgs e);

	public class CommunicatorErrorEventArgs : CommunicatorEventArgs
	{
		public Exception Error { get; private set; }	

		public CommunicatorErrorEventArgs(ICommunicator communicator, Exception error)
			: base(communicator)
		{
			this.Error = error;
		}
	}
}
