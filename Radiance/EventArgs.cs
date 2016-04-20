using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
	public delegate void ActorEventHandler(ActorEventArgs e);
	public class ActorEventArgs : EventArgs
	{
		public IActor Actor { get; set; }

		public ActorEventArgs(IActor actor)
		{
			this.Actor = actor;
		}
	}

    public delegate void ActorEventHandler<T>(ActorEventArgs<T> e) where T : IActor;
    public class ActorEventArgs<T> : EventArgs where T : IActor
    {
        public T Actor { get; set; }

        public ActorEventArgs(T actor)
        {
            this.Actor = actor;
        }
    }

	public delegate void AvatarKilledAvatarEventHandler(IAvatar attacker, IAvatar defender);

    public delegate void PlayerAdvancedEventHandler(IPlayer player, string message);


    /// <summary>
    /// Provides a delegate for client based event handling.
    /// </summary>
    /// <param name="e">The ClientEventArgs for the event.</param>
    public delegate void ClientEventHandler(ClientEventArgs e);

    /// <summary>
    /// Event arguments for client based events.
    /// </summary>
    public class ClientEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the client for the current event.
        /// </summary>
        public IClient Client { get; set; }
    }
}
