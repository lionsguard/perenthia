using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Runtime.Serialization;

using Radiance.Handlers;
using Radiance.Services;

namespace Radiance.Web
{
	public class PollingDuplexClient : Client
    {
        private string _soapAction;
        private object _lock = new object();
        private DataContractSerializer _serializer = new DataContractSerializer(typeof(string));

        /// <summary>
        /// Gets a value indicating whether or not this client instance has faulted and can not receive responses.
        /// </summary>
        public bool HasFaulted { get; private set; }

        /// <summary>
        /// An event that is raised when a client is placed into the faulted state.
        /// </summary>
        public event ClientEventHandler Faulted = delegate { };

		public PollingDuplexClient(IGameClient gameClient)
			: base()
		{
			PollingDuplexMessageContext context = new PollingDuplexMessageContext(gameClient);
            context.Faulted += new EventHandler(context_Faulted);
            this.Context = context;
		}

        private void context_Faulted(object sender, EventArgs e)
        {
            this.HasFaulted = true;
            this.Faulted(new ClientEventArgs { Client = this });
        }
	}
}
