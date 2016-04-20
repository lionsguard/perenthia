using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Runtime.Serialization;

using Radiance.Markup;
using Radiance.Services;
using Lionsguard;

namespace Radiance.Web
{
	public class PollingDuplexMessageContext : IMessageContext
	{
        public IGameClient GameClient { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not this client instance has faulted and can not receive responses.
        /// </summary>
        public bool HasFaulted { get; private set; }

        /// <summary>
        /// An event that is raised when the IWcfClient is placed into the faulted state.
        /// </summary>
        public event EventHandler Faulted = delegate { };

		public PollingDuplexMessageContext(IGameClient gameClient)
		{
			this.GameClient = gameClient;
			this.Message = new RdlTagCollection();
		}

		private void SendData()
		{
            try
            {
                if (!this.HasFaulted)
                {
                    lock (this.Message.SyncLock)
                    {
                        Message msg = System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Soap11, "http://perenthia.com/GameService/Receive", this.Message.ToString());
                        this.Message.Clear();

                        this.GameClient.BeginReceive(msg, this.OnSendComplete, null);
                    }
                }
            }
            catch (Exception ex)
            {
                this.HasFaulted = true;
				Logger.LogWarning(ex.ToString());
            }
            finally
            {
                if (this.HasFaulted)
                {
                    this.Faulted(this.GameClient, EventArgs.Empty);
                }
            }
        }

        private void OnSendComplete(IAsyncResult ar)
        {
            try
            {
                this.GameClient.EndReceive(ar);
            }
            catch (Exception ex)
            {
				this.HasFaulted = true;
				Logger.LogWarning(ex.ToString());
            }
            finally
            {
                if (this.HasFaulted)
                {
                    this.Faulted(this.GameClient, EventArgs.Empty);
                }
            }
        }

		#region IMessageContext Members
		public bool HasMessages { get; private set; }	

		public RdlTagCollection Message { get; private set; }

		/// <summary>
		/// Reads and removes the next tag in the queue and returns true if a tag was found.
		/// </summary>
		/// <param name="tag">The next available tag in the queue.</param>
		/// <returns>True if a tag exists; otherwise false.</returns>
		public bool Read(out RdlTag tag)
		{
			tag = null;
			return false;
		}

		public void Add(RdlTag tag)
		{
			this.Message.Add(tag);
			this.SendData();
		}

		public void AddRange(RdlTag[] tags)
		{
			this.Message.AddRange(tags);
			this.SendData();
		}

		#endregion
	}
}
