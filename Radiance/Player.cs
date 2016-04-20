using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance
{
	#region Player
	/// <summary>
	/// Represents an Avatar that is controlled by a physical player connected to the game engine.
	/// </summary>
	public class Player : Avatar
	{
		/// <summary>
		/// Gets the username of the user that own the current player instance.
		/// </summary>
		public string UserName { get; private set; }

		/// <summary>
		/// Gets the client of the connected real world human player.
		/// </summary>
		public IClient Client { get; private set; }	

		/// <summary>
		/// Initializes a new instance of the Player class.
		/// </summary>
		/// <param name="userName">Specifies the user who owns the player object.</param>
		/// <param name="client">The IClient instance representing the connected player.</param>
		public Player(string userName, IClient client)
		{
			this.UserName = userName;
			this.Client = client;
			this.Client.Player = this;
		}

		#region Messaging
		/// <summary>
		/// Adds an output tag specific to the current avatar. In the case of a player instance the tag might 
		/// contain the results of an action, combat, chat, etc.
		/// </summary>
		/// <param name="tag">The Tag instance to add.</param>
		public override void AddTag(RdlTag tag)
		{
			this.Client.Context.Add(tag);
		}

		/// <summary>
		/// Tells the current player the specified text from the specified avatar.
		/// </summary>
		/// <param name="from">The avatar sending the tell.</param>
		/// <param name="text">The text of the tell message.</param>
		public override void Tell(Avatar from, string text)
		{
			this.AddTag(new RdlTellMessage(from.Name, text));
		}
		#endregion
	}
	#endregion
}
